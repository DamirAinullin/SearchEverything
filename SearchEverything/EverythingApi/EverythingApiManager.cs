using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SearchEverything.EverythingApi.Exceptions;

namespace SearchEverything.EverythingApi
{
    public sealed class EverythingApiManager
    {
        #region DllImport
        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        private static extern int Everything_SetSearchW(string lpSearchString);
        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_SetMatchPath(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_SetMatchCase(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_SetMatchWholeWord(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_SetRegex(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_SetMax(int dwMax);
        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_SetOffset(int dwOffset);

        [DllImport(EverythingManager.Dll)]
        private static extern bool Everything_GetMatchPath();
        [DllImport(EverythingManager.Dll)]
        private static extern bool Everything_GetMatchCase();
        [DllImport(EverythingManager.Dll)]
        private static extern bool Everything_GetMatchWholeWord();
        [DllImport(EverythingManager.Dll)]
        private static extern bool Everything_GetRegex();
        [DllImport(EverythingManager.Dll)]
        private static extern UInt32 Everything_GetMax();
        [DllImport(EverythingManager.Dll)]
        private static extern UInt32 Everything_GetOffset();
        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        private static extern string Everything_GetSearchW();
        [DllImport(EverythingManager.Dll)]
        private static extern StateCode Everything_GetLastError();

        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        private static extern bool Everything_QueryW(bool bWait);

        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_SortResultsByPath();

        [DllImport(EverythingManager.Dll)]
        private static extern int Everything_GetNumFileResults();
        [DllImport(EverythingManager.Dll)]
        private static extern int Everything_GetNumFolderResults();
        [DllImport(EverythingManager.Dll)]
        private static extern int Everything_GetNumResults();
        [DllImport(EverythingManager.Dll)]
        private static extern int Everything_GetTotFileResults();
        [DllImport(EverythingManager.Dll)]
        private static extern int Everything_GetTotFolderResults();
        [DllImport(EverythingManager.Dll)]
        private static extern int Everything_GetTotResults();
        [DllImport(EverythingManager.Dll)]
        private static extern bool Everything_IsVolumeResult(int nIndex);
        [DllImport(EverythingManager.Dll)]
        private static extern bool Everything_IsFolderResult(int nIndex);
        [DllImport(EverythingManager.Dll)]
        private static extern bool Everything_IsFileResult(int nIndex);
        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        private static extern void Everything_GetResultFullPathNameW(int nIndex, StringBuilder lpString, int nMaxCount);
        [DllImport(EverythingManager.Dll)]
        private static extern void Everything_Reset();
        #endregion

        enum StateCode
        {
            OK,
            MemoryError,
            IPCError,
            RegisterClassExError,
            CreateWindowError,
            CreateThreadError,
            InvalidIndexError,
            InvalidCallError
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match path].
        /// </summary>
        /// <value><c>true</c> if [match path]; otherwise, <c>false</c>.</value>
        public bool MatchPath
        {
            get
            {
                return Everything_GetMatchPath();
            }
            set
            {
                Everything_SetMatchPath(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match case].
        /// </summary>
        /// <value><c>true</c> if [match case]; otherwise, <c>false</c>.</value>
        public bool MatchCase
        {
            get
            {
                return Everything_GetMatchCase();
            }
            set
            {
                Everything_SetMatchCase(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match whole word].
        /// </summary>
        /// <value><c>true</c> if [match whole word]; otherwise, <c>false</c>.</value>
        public bool MatchWholeWord
        {
            get
            {
                return Everything_GetMatchWholeWord();
            }
            set
            {
                Everything_SetMatchWholeWord(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable regex].
        /// </summary>
        /// <value><c>true</c> if [enable regex]; otherwise, <c>false</c>.</value>
        public bool EnableRegex
        {
            get
            {
                return Everything_GetRegex();
            }
            set
            {
                Everything_SetRegex(value);
            }
        }

        public bool IncludeFolders { get; set; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Everything_Reset();
        }

        /// <summary>
        /// Searches the specified key word.
        /// </summary>
        /// <param name="keyWord">The key word.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="maxCount">The max count.</param>
        /// <returns></returns>
        public IEnumerable<SearchResult> Search(string keyWord, int offset = 0, int maxCount = 100)
        {
            if (string.IsNullOrEmpty(keyWord))
                throw new ArgumentNullException(nameof(keyWord));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (maxCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxCount));

            Everything_SetSearchW((IncludeFolders ? "" : "file:") + keyWord);
            Everything_SetOffset(offset);
            Everything_SetMax(maxCount);

            if (!Everything_QueryW(true))
            {
                switch (Everything_GetLastError())
                {
                    case StateCode.CreateThreadError:
                        throw new CreateThreadException();
                    case StateCode.CreateWindowError:
                        throw new CreateWindowException();
                    case StateCode.InvalidCallError:
                        throw new InvalidCallException();
                    case StateCode.InvalidIndexError:
                        throw new InvalidIndexException();
                    case StateCode.IPCError:
                        throw new IPCErrorException();
                    case StateCode.MemoryError:
                        throw new MemoryErrorException();
                    case StateCode.RegisterClassExError:
                        throw new RegisterClassExException();
                }
                yield break;
            }

            const int bufferSize = 4096;
            StringBuilder buffer = new StringBuilder(bufferSize);
            var everythingGetNumResults = Everything_GetNumResults();
            for (int idx = 0; idx < everythingGetNumResults; ++idx)
            {
                Everything_GetResultFullPathNameW(idx, buffer, bufferSize);

                var fullPath = buffer.ToString();
                var result = new SearchResult
                {
                    FullPath = fullPath,
                    ShowPath = GetShowPath(fullPath),
                    IconPath = GetIconPath(fullPath)
                };
                if (Everything_IsFolderResult(idx))
                {
                    result.Type = ResultType.Folder;
                }
                else if (Everything_IsFileResult(idx))
                {
                    result.Type = ResultType.File;
                }

                yield return result;
            }
        }

        private string GetIconPath(string fullPath)
        {
            string extension = Path.GetExtension(fullPath);
            string iconName = "default.png"; 
            switch (extension?.ToLower())
            {
                case ".png":
                case ".ico":
                case ".bmp":
                case ".jpg":
                case ".jpeg":
                    iconName = "image.png";
                    break;
                case ".txt":
                    iconName = "text.png";
                    break;
                case ".zip":
                    iconName = "zip.png";
                    break;
                case ".pdf":
                    iconName = "pdf.png";
                    break;
                case ".html":
                case ".htm":
                    iconName = "html.png";
                    break;
                case ".css":
                case ".less":
                case ".sass":
                    iconName = "css.png";
                    break;
                case ".dll":
                    iconName = "dll.png";
                    break;
                case ".json":
                    iconName = "json.png";
                    break;
                case ".cs":
                case ".csx":
                    iconName = "csharp.png";
                    break;
                case ".js":
                case ".jsx":
                    iconName = "javascript.png";
                    break;
                case ".ts":
                    iconName = "typescript.png";
                    break;
                case ".py":
                    iconName = "python.png";
                    break;
                case ".csproj":
                    iconName = "csproj.png";
                    break;
                case ".cpp":
                case ".cc":
                    iconName = "cplusplus.png";
                    break;
                case ".fs":
                    iconName = "fsharp.png";
                    break;
                case ".vb":
                case ".vba":
                    iconName = "visualbasic.png";
                    break;
                case ".as":
                    iconName = "actionscript.png";
                    break;
                case ".scpt":
                case ".scptd":
                case ".AppleScript":
                    iconName = "applescript.png";
                    break;
                case ".clj":
                case ".cljs":
                case ".cljc":
                case ".edn":
                    iconName = "closure.png";
                    break;
                case ".cbl":
                case ".cob":
                case ".cpy":
                case ".cl2":
                    iconName = "cobol.png";
                    break;
                case ".cfm":
                case ".cfc":
                    iconName = "coldfusion.png";
                    break;
                case ".d":
                    iconName = "d.png";
                    break;
                case ".dart":
                    iconName = "dart.png";
                    break;
                case ".pas":
                case ".dpr":
                case ".dfm":
                    iconName = "delphi.png";
                    break;
                case ".ex":
                case ".exs":
                    iconName = "elixir.png";
                    break;
                case ".elm":
                    iconName = "elm.png";
                    break;
                case ".erb":
                    iconName = "erb.png";
                    break;
                case ".erl":
                    iconName = "erlang.png";
                    break;
                case ".swf":
                    iconName = "flash.png";
                    break;
                case ".for":
                case ".f":
                    iconName = "fortran.png";
                    break;
                case ".go":
                    iconName = "go.png";
                    break;
                case ".groovy":
                    iconName = "groovy.png";
                    break;
                case ".haml":
                    iconName = "haml.png";
                    break;
                case ".hs":
                case ".lhs":
                    iconName = "haskell.png";
                    break;
                case ".hx":
                case ".hxml":
                    iconName = "haxe.png";
                    break;
                case ".idr":
                case ".lidr":
                    iconName = "idris.png";
                    break;
                case ".jade":
                    iconName = "jade.png";
                    break;
                case ".jl":
                    iconName = "julia.png";
                    break;
                case ".latex":
                    iconName = "latex.png";
                    break;
                case ".lime":
                    iconName = "lime.png";
                    break;
                case ".lisp":
                case ".cl":
                    iconName = "lisp.png";
                    break;
                case "l.ua":
                    iconName = "lua.png";
                    break;
                case ".m":
                case ".mlx":
                case ".mat":
                case ".fig":
                    iconName = "matlab.png";
                    break;
                case ".mustache":
                    iconName = "mustache.png";
                    break;
                case ".neo":
                case ".neo4j":
                    iconName = "neo4j.png";
                    break;
                case ".nim":
                    iconName = "nim.png";
                    break;
                case ".nunjucks":
                    iconName = "nunjucks.png";
                    break;
                case ".ml":
                case ".mli":
                    iconName = "ocaml.png";
                    break;
                case ".paket":
                    iconName = "paket.png";
                    break;
                case ".patch":
                    iconName = "patch.png";
                    break;
                case ".pl":
                case ".pm":
                case ".pod":
                    iconName = "perl.png";
                    break;
                case ".psd":
                case ".psb":
                    iconName = "photoshop.png";
                    break;
                case ".plantuml":
                    iconName = "plantuml.png";
                    break;
                case ".po":
                    iconName = "poedit.png";
                    break;
                case ".proto":
                    iconName = "proto.png";
                    break;
                case ".pp":
                    iconName = "puppet.png";
                    break;
                case ".qt":
                    iconName = "qt.png";
                    break;
                case ".r":
                    iconName = "r.png";
                    break;
                case ".raml":
                    iconName = "raml.png";
                    break;
                case ".rs":
                case ".rlib":
                    iconName = "rust.png";
                    break;
                case ".scala":
                case ".sc":
                    iconName = "scala.png";
                    break;
                case ".scm":
                case ".ss":
                    iconName = "scheme.png";
                    break;
                case ".sol":
                    iconName = "solidity.png";
                    break;
                case ".sqlite":
                    iconName = "sqlite.png";
                    break;
                case ".swift":
                    iconName = "swift.png";
                    break;
                case ".tcl":
                    iconName = "tcl.png";
                    break;
                case ".twig":
                    iconName = "twig.png";
                    break;
                case ".volt":
                    iconName = "volt.png";
                    break;
            }

            var fileName = Path.GetFileName(fullPath);
            switch (fileName)
            {
                case "ansible.cfg":
                    iconName = "ansible.png";
                    break;
                case ".eslintrc.json":
                    iconName = "csslint.png";
                    break;
                case ".handlebars":
                    iconName = "handlebars.png";
                    break;
                case ".jshintrc":
                    iconName = "jshint.png";
                    break;
                case ".stylelintrc":
                    iconName = "stylelint.png";
                    break;
            }

            return "Resources/Icons/" + iconName;
        }

        private string GetShowPath(string fullPath)
        {
            int maxNumChars = 45;
            if (fullPath.Length <= maxNumChars)
            {
                return fullPath;
            }
            var pathParts = fullPath.Split('\\');
            string filename = pathParts[pathParts.Length - 1];
            if (7 + filename.Length >= maxNumChars)
            {
                return pathParts[0] + "\\...\\" + filename;
            }
            int lastFolderChars = maxNumChars - filename.Length + 5;
            return fullPath.Remove(lastFolderChars) + "\\...\\" + filename;
        }
    }
}
