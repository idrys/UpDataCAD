using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpDataCAD
{
    public class UpdatePath
    {
        public string ID;
        public string Name;
        public string FullName;
        public string WebSide;
        public string Path;
        public string Update_at;
        public string WebPath;
        public string LocalPath;
        public string ControllFile;

        [JsonIgnore]
        public string FileName
        {
            get {
                var t = new Uri(Path).AbsolutePath;
                var g = System.IO.Path.GetFileName(t);
                return g;
            }
        }

        [JsonIgnore]
        public int LP
        {
            get { return int.Parse(this.ID); }
        }
    }
}
