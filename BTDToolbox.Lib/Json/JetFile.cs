using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using BTDToolbox.Extensions;

namespace BTDToolbox.Lib.Json
{
    public class JetFile : ZipFile
    {
        public JetFile(string name) : base(name)
        {
            
        }

        public List<string> GetDirectories()
        {
            List<string> directories = new List<string>();
            var entries = this.FindAll(entry => entry.IsDirectory);
            entries.ForEach(entry => directories.Add(entry.Name));
            return directories;
        }

        public bool SetPassword(string password)
        {
            return false;
        }
    }
}
