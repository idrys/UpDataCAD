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
        public string Date;
        public string WebPath;
        public string LocalPath;
        public string FileName
        {
            get { return Path.GetFileName(new Uri(WebPath).AbsolutePath); }
        }
    }
}
