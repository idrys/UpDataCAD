using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpDataCAD
{
    public class Configuration
    {
        public class Source
        {
            public string Page = "";
            public string Path = string.Empty;
        }

        public Source[] Update ;
        //public ArrayList Update ;
        public string[] LocalFiles = null;

        public Configuration()
        {
            List<Source> update = new List<Source>();
            Source tmp = new Source();
            tmp.Page = "http://www.opoczno.eu/o-opoczno/#!/dla-architektow-i-inwestorow/";
            tmp.Path = "http://opoczno.eu/uploads/";
            update.Add(tmp);

            tmp = new Source();
            tmp.Page = "http://www.cersanit.com.pl/page/dla-architektow-pliki-do-pobrania";
            tmp.Path = "http://www.cersanit.com.pl/public/";
            update.Add(tmp);

            this.Update = update.ToArray<Source>();

            List<string> localFiles = new List<string>();
            this.LocalFiles = localFiles.ToArray<string>();
        }

        private void  ReadConfig()
        {
            /*
            Product product = new Product();
            product.Name = "Apple";
            product.Expiry = new DateTime(2008, 12, 28);
            product.Sizes = new string[] { "Small" };

            string json = JsonConvert.SerializeObject(product);
            */
        }
    }
}
