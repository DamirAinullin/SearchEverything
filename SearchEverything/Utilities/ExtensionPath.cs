using System;
using System.IO;

namespace SearchEverything.Utilities
{
    public class ExtensionUtility
    {
        private readonly Type _type;

        public ExtensionUtility(Type type)
        {
            _type = type;
        }

        public string GetExtensionPath()
        {
            string codebase = _type.Assembly.CodeBase;
            var uri = new Uri(codebase, UriKind.Absolute);
            return Directory.GetParent(uri.LocalPath).FullName;
        }
    }
}
