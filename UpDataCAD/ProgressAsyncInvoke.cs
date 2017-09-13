using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpDataCAD
{
    public delegate int AsyncInvoke();

    public class ProgressAsyncInvoke
    {
        static int progress = 0;
        private string pathToFile;
        private string pathDestFolder;

        public ProgressAsyncInvoke(string _pathToFile, string _pathDestFolder)
        {
             this.pathToFile = _pathToFile;
             this.pathDestFolder = _pathDestFolder;

        }

    /// <summary>
    /// Wypakowanie wskazanego pliku do określonego folderu
    /// </summary>
    /// <param name="pathToFile">Zarchiwizowany plik do rozpakowania </param>
    /// <param name="pathDestFolder">Miejsce wypakowania plików</param>
    /// <returns></returns>
    public int ExtractFile()
        {
            Extract ext = new Extract();
            Debug.WriteLine(this.pathToFile + "; " + this.pathDestFolder );

            ext.SevenZipExtractProgress(this.pathToFile, this.pathDestFolder, onProgress);
            //ext.UnpackProgress(this.pathToFile, this.pathDestFolder, onProgress);
            return 1;
        }

        public int Progress()
        {
            return progress;
        }

        public void Zero()
        {
            progress = 0;
        }

        private void onProgress(int obj)
        {
            
            progress = obj;
        }
    }
}
