# Unindent Fix
An extension for Visual Studio 2015, 2017, and 2019 to fix its unindent behavior (when pressing Shift+Tab or using the Decrease Line Indent command). By default, Visual Studio aligns everything to tab stops which destroys partial indentation.

Example code:
```
    int x =
        1 +
         1 +
          1 +
           1 +
            1 +
             1;
```

Visual Studio unindent:
```
int x =
    1 +
        1 +
        1 +
        1 +
        1 +
            1;
```

As for real-world code using partial indentation, common examples include LINQ:
```
char[] everyOtherLetter =
    (from n in Enumerable.Range(0, 26)
     where n % 2 == 0
     select (char)('a' + n)).ToArray();
```

And multi-line comments:
```
    /*
     * My comment
     */
```
