// --------------------------------------------------------------------------------
// Copyright (c) J.D. Purcell
//
// Licensed under the MIT License (see LICENSE.txt)
// --------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace UnindentFix {
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("Unindent Fix", "Fixes unindent behavior.", VersionInfo.Version)]
	[Guid("d10f95f0-7264-47f2-9cf9-2092406dc906")]
	[ProvideAutoLoad(VSConstants.VsEditorFactoryGuid.TextEditor_string)]
	public sealed class MainPackage : Package {
		private DTE2 _dte2;
		private CommandEvents _std2KCommandEvents; // This reference must be held

		protected override void Initialize() {
			_dte2 = (DTE2)GetService(typeof(DTE));

			_std2KCommandEvents = _dte2.Events.CommandEvents[VSConstants.CMDSETID.StandardCommandSet2K_string];
			_std2KCommandEvents.BeforeExecute += Std2KCommandEvents_BeforeExecute;

			base.Initialize();
		}

		private void Std2KCommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault) {
			switch (ID) {
				case (int)VSConstants.VSStd2KCmdID.UNINDENT:
				case (int)VSConstants.VSStd2KCmdID.BACKTAB:
					CancelDefault = HandleUnindent();
					break;
			}
		}

		private bool HandleUnindent() {
			Document document = _dte2.ActiveDocument;
			if (document.TabSize != document.IndentSize) {
				return false;
			}
			int tabSize = document.TabSize;
			using (var undoContext = new UndoContextHelper(_dte2, "Unindent Selection")) {
				TextSelection selection = (TextSelection)document.Selection;
				bool isReverse = !selection.IsActiveEndGreater;
				int startLine = selection.TopPoint.Line;
				int startLineCharOffset = selection.TopPoint.LineCharOffset;
				int endLine = selection.BottomPoint.Line;
				int endLineCharOffset = selection.BottomPoint.LineCharOffset;
				for (int iLine = startLine; iLine <= endLine; iLine++) {
					if (iLine == endLine && endLineCharOffset == 1) break;
					selection.GotoLine(iLine, true);
					string lineText = selection.Text;
					int removedCharCount = 0;
					while (removedCharCount < lineText.Length) {
						char c = lineText[removedCharCount];
						if (c != '\t' && c != ' ') break;
						removedCharCount++;
						if (c == '\t' || removedCharCount == tabSize) break;
					}
					if (removedCharCount != 0) {
						selection.StartOfLine();
						selection.Delete(removedCharCount);
						if (iLine == startLine) {
							startLineCharOffset = Math.Max(startLineCharOffset - removedCharCount, 1);
						}
						if (iLine == endLine) {
							endLineCharOffset = Math.Max(endLineCharOffset - removedCharCount, 2);
						}
					}
				}
				selection.MoveToLineAndOffset(startLine, startLineCharOffset);
				selection.MoveToLineAndOffset(endLine, endLineCharOffset, true);
				if (isReverse) selection.SwapAnchor();
				undoContext.Commit();
			}
			return true;
		}
	}
}
