using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceWizard
{
    public static class Worker
    {

        public static IEnumerable<Stream> GetAllFiles(string directoryPath, bool recursive)
        {
            List<string> dirsAndSubdirs = new List<string>();

            // Get all directories to search
            dirsAndSubdirs.Add(directoryPath);
            if(recursive)
            {
                dirsAndSubdirs.AddRange(Directory.GetDirectories(directoryPath));
            }

            List<String> allFilesFullNames = new List<string>();
            
            // Get all names of files in the set of directories.
            foreach(string currentDir in dirsAndSubdirs)
            {
                List<String> fileNames = Directory.EnumerateFiles(currentDir).ToList();

                foreach(string fileName in fileNames)
                {
                    allFilesFullNames.Add(Path.Combine(currentDir, fileName));
                }
            }

            // Open all files.
            List<Stream> filesInDirs = OpenAllFiles(allFilesFullNames).ToList();

            return filesInDirs;
        }

        private static IEnumerable<Stream> OpenAllFiles(IEnumerable<string> filesPath)
        {
            List<Stream> files = new List<Stream>();
            foreach(string filePath in filesPath)
            {
                StreamReader file = new StreamReader(filePath, Encoding.Default, true);
                files.Add(file.BaseStream);
            }

            return files;
        }

        /// <summary>
        /// Returns new strings where all occurrences of a given string are replaced by another specified string.
        /// </summary>
        /// <param name="sourceTexts"></param>
        /// <param name="oldString"></param>
        /// <param name="newString"></param>
        /// <param name="keepUnchanged"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<String> ReplaceAll(IEnumerable<String> sourceTexts, string oldString, string newString, bool keepUnchanged)
        {
            List<String> alteredTexts = new List<string>();
            foreach(String currentText in sourceTexts)
            {
                string replacedText = currentText.Replace(oldString, newString);
                if (replacedText != currentText || keepUnchanged)
                {
                    alteredTexts.Add(replacedText);
                }
            }

            return alteredTexts;
        }

        public static IEnumerable<String> ReplaceAll(IEnumerable<String> sourceTexts, string oldString, string newString)
        {
            return ReplaceAll(sourceTexts, oldString, newString, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IEnumerable<String> ReadAllFiles(IEnumerable<Stream> files, Encoding encoding)
        {
            List<String> filesContent = new List<String>();

            foreach(Stream file in files)
            {
                StreamReader reader = new StreamReader(file, encoding, true);

                filesContent.Add(reader.ReadToEnd());
            }

            return filesContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static IEnumerable<String> ReadAllFiles(IEnumerable<Stream> files)
        {
            return ReadAllFiles(files, Encoding.Default);
        }
    }
}
