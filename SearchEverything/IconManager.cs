using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SearchEverything.EverythingApi;

namespace SearchEverything
{
    public static class IconManager
    {
        private static readonly uint SizeOfShFileInfo = (uint)Marshal.SizeOf(new Shell32.SHFILEINFO());
        private static readonly Dictionary<string, ImageSource> ImageSources = new Dictionary<string, ImageSource>(25);
        private static readonly Dictionary<string, string>  IconsByExtension = new Dictionary<string, string>
        {
            { ".png", "image.png" }, { ".ico", "image.png" }, { ".bmp", "image.png" }, { ".jpg", "image.png" },
            { ".jpeg", "image.png" }, { ".txt", "text.png" }, { ".zip", "zip.png" }, { ".pdf", "pdf.png" },
            { ".mp3", "mp3.png" }, { ".xml", "xml.png" }, { ".html", "html.png"}, { ".htm", "html.png" },
            { ".css", "css.png" }, { ".less", "css.png" }, { ".sass", "css.png" }, { ".dll", "dll.png" },
            { ".json", "json.png" }, { ".cs", "csharp.png" }, { ".csx", "csharp.png" }, { ".java", "java.png" },
            { ".js", "javascript.png" }, { ".jsx", "javascript.png" }, { ".ts", "typescript.png" },
            { ".py", "python.png" }, { ".csproj", "csproj.png" }, { ".cpp", "cplusplus.png" }, { ".cc", "cplusplus.png" },
            { ".fs", "fsharp.png" }, { ".vb", "visualbasic.png" }, { ".vba", "visualbasic.png" }, { ".as", "actionscript.png" },
            { ".scpt", "applescript.png" }, { ".scptd" , "applescript.png" }, { ".AppleScript", "applescript.png" },
            { ".clj", "closure.png" }, {".cljs", "closure.png" }, { ".cljc", "closure.png" }, { ".cbl", "cobol.png" },
            { ".cob", "cobol.png"}, { ".cpy", "cobol.png" }, { ".cl2", "cobol.png" }, { ".cfm", "coldfusion.png" },
            { ".d", "d.png" }, { ".dart", "dart.png" }, { ".pas", "delphi.png" }, { ".dpr", "delphi.png" },
            { ".dfm", "delphi.png" }, { ".ex", "elixir.png" }, { ".exs", "elixir.png" }, { ".elm", "elm.png" },
            { ".erb", "erb.png" }, { ".erl", "erlang.png" }, { ".swf", "flash.png" }, { ".for", "fortran.png" },
            { ".f", "fortran.png" }, { ".go", "go.png" }, { ".groovy", "groovy.png" }, { ".haml", "haml.png" },
            { ".hs", "haskell.png" }, { ".lhs", "haskell.png" }, { ".hx", "haxe.png" }, { ".hxml", "haxe.png" },
            { ".idr", "idris.png" }, { ".lidr", "idris.png" }, { ".jade", "jade.png" }, { ".jl", "julia.png" },
            { ".latex", "latex.png" }, { ".lime", "lime.png" }, { ".lisp", "lisp.png"}, { ".cl", "lisp.png"},
            { ".lua", "lua.png" }, { ".m", "matlab.png" }, { ".mlx", "matlab.png" }, { ".mat", "matlab.png"},
            { ".fig", "matlab.png" }, { ".mustache", "mustache.png" }, { ".neo", "neo4j.png" }, { ".neo4j", "neo4j.png" },
            { ".nim", "nim.png" }, { ".nunjucks", "nunjucks.png" }, { ".ml", "ocaml.png" }, { ".mli", "ocaml.png" },
            { ".paket", "paket.png" }, { ".patch", "patch.png" }, { ".pl", "perl.png" }, { ".pm", "perl.png" },
            { ".pod", "perl.png" }, { ".psd", "photoshop.png" }, { ".psb", "photoshop.png" }, { ".plantuml", "plantuml.png" },
            { ".po", "poedit.png" }, { ".proto", "proto.png" }, { ".pp", "puppet.png" }, { ".qt", "qt.png" },
            { ".r", "r.png" }, { ".raml", "raml.png" }, { ".rs", "rust.png" }, { ".rlib", "rust.png" },
            { ".scala", "scala.png" }, { ".sc", "scala.png" }, { ".scm", "scheme.png" }, { ".ss", "scheme.png" },
            { ".sol", "solidity.png" }, { ".sqlite", "sqlite.png" }, { ".swift", "swift.png" }, { ".tcl", "tcl.png" },
            { ".twig", "twig.png" }, { ".volt", "volt.png" }
        };

        private static readonly Dictionary<string, string> IconsByFilename = new Dictionary<string, string>
        {
            { "ansible.cfg", "ansible.png" }, { ".eslintrc.json", "csslint.png" },
            { ".handlebars", "handlebars.png" }, { ".jshintrc", "jshint.png" },
            { ".stylelintrc", "stylelint.png" }, { "app.config", "xml.png" },
            { "web.config", "xml.png" }
        };

        public static ImageSource GetImageSource(string fullPath, ResultType type)
        {
            string extension = Path.GetExtension(fullPath)?.ToLower();
            ImageSource imageSource;
            if (!String.IsNullOrEmpty(extension) && ImageSources.TryGetValue(extension, out imageSource))
            {
                return imageSource;
            }

            string iconPath = GetIconPath(fullPath, extension, type);
            imageSource = iconPath != null ? new BitmapImage(new Uri(iconPath, UriKind.Relative))
                : ToImageSource(GetFileIcon(fullPath));
            imageSource.Freeze();
            if (!String.IsNullOrEmpty(extension))
            {
                ImageSources.Add(extension, imageSource);
            }
            return imageSource;
        }

        private static string GetIconPath(string fullPath, string extension, ResultType type)
        {
            if (type == ResultType.Folder)
            {
                return "Resources/Icons/folder.png";
            }
            if (String.IsNullOrEmpty(extension))
            {
                return "Resources/Icons/default.png";
            }
            string iconName;
            if (IconsByExtension.TryGetValue(extension, out iconName) ||
                IconsByFilename.TryGetValue(Path.GetFileName(fullPath)?.ToLower(), out iconName))
            {
                return "Resources/Icons/" + iconName;
            }
            return null;
        }

        private static ImageSource ToImageSource(Icon icon)
        {
            var imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        private static Icon GetFileIcon(string name)
        {
            var shfi = new Shell32.SHFILEINFO();
            uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES | Shell32.SHGFI_SMALLICON;

            Shell32.SHGetFileInfo(name,
                Shell32.FILE_ATTRIBUTE_NORMAL,
                ref shfi,
                SizeOfShFileInfo,
                flags);

            // Copy (clone) the returned icon to a new object, thus allowing us 
            // to call DestroyIcon immediately
            var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }
    }
}