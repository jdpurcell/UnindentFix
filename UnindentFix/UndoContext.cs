// --------------------------------------------------------------------------------
// Copyright (c) J.D. Purcell
//
// Licensed under the MIT License (see LICENSE.txt)
// --------------------------------------------------------------------------------
using System;
using EnvDTE;
using EnvDTE80;

namespace UnindentFix {
	internal class UndoContextHelper : IDisposable {
		private UndoContext _context;
		private bool _ownsContext;

		public UndoContextHelper(DTE2 dte2, string name) {
			_context = dte2.UndoContext;
			_ownsContext = !_context.IsOpen;
			if (_ownsContext) {
				_context.Open(name);
			}
		}

		public void Commit() {
			if (_context == null) return;
			if (_ownsContext && _context.IsOpen) {
				_context.Close();
			}
			_context = null;
		}

		public void Dispose() {
			if (_context == null) return;
			if (_context.IsOpen) {
				_context.SetAborted();
			}
			_context = null;
		}
	}
}
