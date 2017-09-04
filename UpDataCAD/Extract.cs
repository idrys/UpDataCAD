using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpDataCAD
{
    public class Extract
    {
        /// <summary>
        /// Określa docelową nazwę folderu gdzie mają zostać wypakowane pliki 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Zwraca nazwe docelowego folderu</returns>
        public  string TargetName(string filePath)
        {
            string postfix = @"Plytki\";

            string targetName = File.ReadLines(filePath)
                                        .Where(x => x.Contains("TARGETNAME=")) // Odnajduję pozycję z zadanym ciągiem
                                        .ToList() // wyniki wrzuca do listy
                                        .First() // Wybiera pierwszą pozycję
                                        .Substring(11); // Wycina pierszych jedenaście znaków czyli "TARGETNAME="

            
            return targetName;
        }

        /// <summary>
        /// Wypakowuje wskazany plik do katalogo folderExtract
        /// </summary>
        /// <param name="folder">Folder w którym znajduje się archiwum</param>
        /// <param name="pathFile">pełna ścieżka do spakowanego pliku</param>
        public bool SevenZipExtractProgress(string pathFile, string folderExtract, Action<int> onProgress)
        {
            Regex REX_SevenZipStatus = new Regex(@"(?<p>[0-9]+)%");

            int EverythingOK = -1;
            string testInfo = string.Empty;
            string path7zip = "x86\\7z.exe";

            if (Environment.Is64BitOperatingSystem)
                path7zip = "x64\\7z.exe";

            Process p = new Process();
            p.StartInfo.FileName = path7zip;
            p.StartInfo.Arguments = "e " + "\"" + pathFile + "\"" + " -o\"" + folderExtract + "\"" + " -y -bsp1 -bse1 -bso1";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            p.OutputDataReceived += (sender, e) => {



                if (onProgress != null)
                {
                    Match m = REX_SevenZipStatus.Match(e.Data ?? "");
                    if (m != null && m.Success)
                    {
                        int procent = int.Parse(m.Groups["p"].Value);
                        onProgress(procent);
                    }
                }
            };

            p.Start();

            p.BeginOutputReadLine();

            p.WaitForExit();


            p.Close();

            EverythingOK = testInfo.IndexOf("Everything is Ok");

            return EverythingOK == -1 ? false : true;
        }

        /// <summary>
        /// Wypakowuje wskazany plik do katalogo folderExtract
        /// </summary>
        /// <param name="folder">Folder w którym znajduje się archiwum</param>
        /// <param name="pathFile">pełna ścieżka do spakowanego pliku</param>
        public bool SevenZipExtract(string pathFile, string folderExtract)
        {
            int EverythingOK = -1;
            string testInfo = string.Empty;
            string path7zip = "x86\\7z.exe";

            if (Environment.Is64BitOperatingSystem)
                path7zip = "x64\\7z.exe";

            Process p = new Process();
            p.StartInfo.FileName = path7zip;
            p.StartInfo.Arguments = "e " + "\"" + pathFile + "\"" + " -o\"" + folderExtract + "\"" + " -y";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            testInfo = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            EverythingOK = testInfo.IndexOf("Everything is Ok");

            return EverythingOK == -1 ? false : true;
        }

        /// <summary>
        /// Sprawdza archiwum i podaje liczbę spakowanych plików
        /// </summary>
        /// <param name="pathFile">Scieżka do archiwum</param>
        /// <returns>Zwraca liczbę spakowanych plików</returns>
        public int SevenZipTest(string pathFile)
        {
            string path7zip = "x86\\7z.exe";

            if (Environment.Is64BitOperatingSystem)
                path7zip = "x64\\7z.exe";

            Process p = new Process();
            p.StartInfo.FileName = path7zip;
            p.StartInfo.Arguments = "t " + "\"" + pathFile + "\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string testInfo = p.StandardOutput.ReadToEnd();
            int filesNumber = AnalizeTestOutput(testInfo);
            p.Close();
            //Console.WriteLine("Wynik testu " + filesNumber);
            //Console.ReadKey();
            return filesNumber;
        }


        /// <summary>
        /// Analiza treści wyjścia konsoli po teście archiwum .zip próba wyciągnięcia liczby plików w archiwum
        /// </summary>
        /// <param name="stringTest">treśc do analizy</param>
        /// <returns></returns>
        static int AnalizeTestOutput(string stringTest)
        {
            //string sentence = "This sentence has five words.";
            // Extract the second word.

            Console.WriteLine(stringTest);
            int EverythingOK = stringTest.IndexOf("Everything is Ok");
            int n = 0;

            Console.WriteLine("EverythingOK = " + EverythingOK);

            if (EverythingOK > 0)
            {
                if (stringTest.IndexOf("Files:") == -1)
                {
                    return 1;
                }
                else
                {
                    int startPosition = stringTest.IndexOf("Files:") + 7;
                    Console.WriteLine("startPosition = " + startPosition);
                    string word2 = stringTest.Substring(startPosition, stringTest.IndexOf(" ", startPosition) - startPosition - 7);

                    int.TryParse(word2, out n);
                    //Console.WriteLine("Second word: " + word2);
                }
                // Console.WriteLine(n);
                //Console.ReadKey();
            }

            return n; // int.Parse(word2);
        }

        /// <summary>
        /// Odczytuje dane z pliku konfiguracyjnego utworzonego podczas instalacji
        /// </summary>
        /// <param name="fileName"></param>
        static void ReadConfigFile(string fileName)
        {
            string pathCADDecor = "";
            string patchFolderRepository = "";

        }

        /// <summary>
        /// Odczytuje listę wszystkich plików znajdujących się w repozytorium
        /// </summary>
        /// <param name="folderName">Ścieżka do folderu w którym znajdują się pliki do rozpakowania</param>
        /// <returns>Zwraca LISTE pełnych ścierzek do wszystkich plików w folderze</returns>
        public string[] ReadListFilesFromRepository(string folderName)
        {

            string[] fileEntries = null;
            if (Directory.Exists(folderName))
            {
                // Process the list of files found in the directory.
                fileEntries = Directory.GetFiles(folderName);

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(folderName);
            }
            else
            {
                Console.WriteLine("{0} to niej prawidłowa sciezka do katalogu.", folderName);
            }

            return fileEntries;
        }

        /// <summary>
        /// Główna metoda programu - rozpakowuje plik i przerzuca do prawidłowego katalogu
        /// </summary>
        /// <param name="fileName"></param>
        public void Unpack(string folderRepo, string fileRepo)
        {
            string zipPath = folderRepo + fileRepo;

            Unpack(zipPath);

        }
 
        /// <summary>
        /// Główna metoda programu - rozpakowuje plik i przerzuca do prawidłowego katalogu
        /// </summary>
        /// <param name="fileName"></param>
        public void Unpack(string zipPath)
        {
            // TODO: Przenieść to do pliku konfiguracyjnego
            string extractPath = @"C:\CADProject\CAD Decor Paradyz v. 2.3.0";

            //string zipPath = Path.GetFullPath(folderRepo) + fileRepo;
            if (File.Exists(zipPath))
            {
                int filesCount = this.SevenZipTest(zipPath);

                if (filesCount > 3)
                    extractPath = extractPath + "\\dodatki\\" + TargetName(zipPath);
                else
                {
                    if (filesCount == 3)
                        extractPath = extractPath + "\\Plytki\\";
                    else
                        throw new Exception("Coś poszło nie tak przy rozpakowywaniu pliku: " + zipPath);
                }
                SevenZipExtract(zipPath, extractPath);
                //SevenZipExtractor.ExtractToDirectory(folderRepo + fileRepo, extractPath);
                //ZipFile
                //File.Delete(folderRepo + fileRepo);

            }
        }

        /// <summary>
        /// Główna metoda programu - rozpakowuje plik i przerzuca do prawidłowego katalogu
        /// </summary>
        /// <param name="fileName"></param>
        public void UnpackProgress(string zipPath, Action<int> onProgress)
        {
            // TODO: Przenieść to do pliku konfiguracyjnego
            string extractPath = @"C:\CADProject\CAD Decor Paradyz v. 2.3.0";

            //string zipPath = Path.GetFullPath(folderRepo) + fileRepo;
            if (File.Exists(zipPath))
            {
                int filesCount = this.SevenZipTest(zipPath);

                if (filesCount > 3)
                    extractPath = extractPath + "\\dodatki\\" + TargetName(zipPath);
                else
                {
                    if (filesCount == 3)
                        extractPath = extractPath + "\\Plytki\\";
                    else
                        throw new Exception("Coś poszło nie tak przy rozpakowywaniu pliku: " + zipPath);
                }
                SevenZipExtractProgress(zipPath, extractPath, onProgress);
                //SevenZipExtractor.ExtractToDirectory(folderRepo + fileRepo, extractPath);
                //ZipFile
                //File.Delete(folderRepo + fileRepo);

            }
        }

        /// <summary>
        /// Określa rodzaj pliku - jego rozezrzenie, zip, msi, exe
        /// </summary>
        /// <param name="fileName"></param>
        static string Extention(string fileName)
        {

            return null;
        }

        /// <summary>
        /// Określenie czy plik jest .exe czy .zip
        /// </summary>
        /// <param name="fileName">Ścieżka do pliku</param>
        /// <returns>Zwraca True jeśli rozrzerzenie będzie .zip</returns>
        static bool checkFile(string fileName)
        {
            //string ext = Extention(fileName);
            string extension = "";
            extension = Path.GetExtension(fileName);
            //Console.WriteLine("GetExtension('{0}') returns '{1}'", fileName, extension);

            if (extension == ".zip")
            {
                // if( checkArchiw(fileNameZip) )
                // UnPackFirstZip(string fileNameZip)
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNameZip"></param>
        /// <returns></returns>
        static string UnPackFirstZip(string fileNameZip)
        {
            // uzip c:\patchFolderRepository\
            // fileName = newFileName.exe
            // remove fileNameZip
            return String.Empty;
        }

        /// <summary>
        /// Sprawdzam czy w archiwum jest tylko jeden plik typu .exe
        /// </summary>
        /// <param name="fileNameZip"></param>
        static bool checkArchiw(string fileNameZip)
        {
            return false;
        }
    }
}
