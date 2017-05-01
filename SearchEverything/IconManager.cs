using System;
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
        public static ImageSource GetImageSource(string fullPath, ResultType type)
        {
            string iconPath = GetIconPath(fullPath, type);
            if (iconPath != null)
            {
                var imageUri = new Uri(iconPath, UriKind.Relative);
                var imageSource = new BitmapImage(imageUri);
                imageSource.Freeze();
                return imageSource;
            }
            return ToImageSource(GetFileIcon(fullPath));
        }

        private static string GetIconPath(string fullPath, ResultType type)
        {
            if (type == ResultType.Folder)
            {
                return "Resources/Icons/folder.png";
            }
            string extension = Path.GetExtension(fullPath);
            string iconName = null;
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
                case ".xml":
                    iconName = "xml.png";
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
                case ".java":
                    iconName = "java.png";
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

            var fileName = Path.GetFileName(fullPath)?.ToLower();
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
                case "app.config":
                case "web.config":
                    iconName = "xml.png";
                    break;
            }
            if (iconName == null)
            {
                return null;
            }
            return "Resources/Icons/" + iconName;
        }

        private static ImageSource ToImageSource(Icon icon)
        {
            var imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            imageSource.Freeze();
            return imageSource;
        }

        private static Icon GetFileIcon(string name)
        {
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

            flags += Shell32.SHGFI_SMALLICON; // include the small icon flag

            Shell32.SHGetFileInfo(name,
                Shell32.FILE_ATTRIBUTE_NORMAL,
                ref shfi,
                (uint)Marshal.SizeOf(shfi),
                flags);

            // Copy (clone) the returned icon to a new object, thus allowing us 
            // to call DestroyIcon immediately
            var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon); // Cleanup
            return icon;
        }
    }
}