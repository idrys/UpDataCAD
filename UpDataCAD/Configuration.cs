using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
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
        public string[] LocalFiles = null;

        public Configuration()
        {
            
            List<Source> update = new List<Source>();

            StartupFoldersConfigSection section = (StartupFoldersConfigSection)ConfigurationManager.GetSection("StartupFolders");
            foreach (FolderElement item in section.FolderItems)
            {
                Source tmp = new Source();
                tmp.Page = item.Web;
                tmp.Path = item.Path;
                update.Add(tmp);
            }
            

            this.Update = update.ToArray<Source>();

            List<string> localFiles = new List<string>();
            this.LocalFiles = localFiles.ToArray<string>();
        }

       
    }


  // Klasy niezbędne do linków stron na których znajdują się bazy CADDecor

    public class StartupFoldersConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Folders")]
        public FoldersCollection FolderItems
        {
            //FoldersCollection t = ((FoldersCollection)(base["Folders"]));

            get { return ((FoldersCollection)(base["Folders"])); }
        }
    }

    [ConfigurationCollection(typeof(FolderElement))]
    public class FoldersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FolderElement)(element)).Company;
        }
        public FolderElement this[int idx]
        {
            get { return (FolderElement)BaseGet(idx); }
        }
    }

    public class FolderElement : ConfigurationElement
    {
        [ConfigurationProperty("company", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Company
        {
            get { return ((string)(base["company"])); }
            set { base["company"] = value; }
        }

        [ConfigurationProperty("path", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Path
        {
            get { return ((string)(base["path"])); }
            set { base["path"] = value; }
        }

        [ConfigurationProperty("web", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Web
        {
            get { return ((string)(base["web"])); }
            set { base["web"] = value; }
        }
    }
}

