using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpDataCAD
{
    public class FileParts
    {
        public string FullPath = string.Empty;
        public string FullName = string.Empty;
        public string OnlyName = string.Empty;
        public string Extention = string.Empty;

        /// <summary>
        /// Metoda odzielająca nazwę pliku od jego rozszerzenia
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Klasa z nazwami w osobnych strumieniach, nazwa rozrzeszenie, ścieżka i pełna nazwa pliku</returns>
        public FileParts GetExtantion(string path)
        {
            //FileParts file = new FileParts();
            string fileName = path.Substring(path.LastIndexOf(((char)47)) + 1);
            int index = fileName.LastIndexOf('.');
            string onyName = fileName.Substring(0, index);
            string fileExtension = fileName.Substring(index + 1);

            this.FullPath = path;
            this.FullName = fileName;
            this.OnlyName = onyName;
            this.Extention = fileExtension;

            return this;
        }
    }
}
