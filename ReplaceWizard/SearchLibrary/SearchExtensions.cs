using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchLibrary
{
    public static class SearchExtensions
    {
        static private Dictionary<String,List<Stream>> _cacheStream;
        static private Dictionary<String,List<String>> _cacheString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filesToSearch"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static IEnumerable<Stream> SingleSearch(this IEnumerable<Stream> filesToSearch, String word)
        {
            return SingleSearch(filesToSearch, word, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filesToSearch"></param>
        /// <param name="word"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public static IEnumerable<Stream> SingleSearch(this IEnumerable<Stream> filesToSearch, String word, bool searchName)
        {
            List<Stream> filesWithWord = new List<Stream>();

            foreach(Stream file in filesToSearch)
            {
                if(file.SimpleSearch(word, searchName))
                {
                    filesWithWord.Add(file);
                }
            }

            return filesWithWord;
        }

          /// <summary>
        /// Search current instance for a given string.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="word">String to search</param>
        /// <returns>True if there is a occurence of word in the current instance</returns>
        public static bool SimpleSearch(this Stream file, string word)
        {
            return SimpleSearch(file, word, false);
        }

        /// <summary>
        /// Search current instance for a given string.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="word">String to search</param>
        /// <param name="searchName">True if the name of the file should be considered in the search.</param>
        /// <returns>True if there is a occurence of word in the current instance</returns>
        /// <remarks>Only use searchName if current instance is a FileStream.</remarks>
        public static bool SimpleSearch(this Stream file, string word, bool searchName)
        {
            string fileDump;
            long originalStreamPosition = file.Position;

            // Read file from start to end.
            file.Position = 0;
            StreamReader reader = new StreamReader(file, true);
            fileDump = reader.ReadToEnd();

            // Return stream to original position.
            file.Position = originalStreamPosition;

            // Search in the stream text for given string.
            if(fileDump.Find(word) > -1)
            {
                return true;
            }
            else
            {
                // Search in the stream name if such a name exists.
                if(searchName)
                {
                    try
                    {
                        FileStream fileWithInfo = (FileStream)file;

                        if (fileWithInfo.Name.Find(word) > -1)
                        {
                            return true;
                        }
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new SearchLibrary.Exceptions.NotAFileException("Argument file is not a FileStream", ex);
                    }
                }

                return false;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filesToSearch"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        //public static List<Task<Stream>> SingleSearchAsync(this List<Stream> filesToSearch, String word, bool searchName)
        //{
        //    List<Task<Stream>> tasks = new List<Task<Stream>>();

        //    foreach (Stream file in filesToSearch)
        //    {
        //        Task<Stream> task = new Task<Stream>(() =>
        //            {
        //                if (file.SimpleSearch(word, searchName))
        //                {
        //                    return file;
        //                }

        //                return null;
        //            });

        //        tasks.Add(task);
        //        task.Start();
        //    }
        //    return tasks;
        //}

        public static async Task<IEnumerable<Stream>> SingleSearchAsync(this IEnumerable<Stream> filesToSearch, String word, bool searchName)
        {
            List<Stream> filesWithWord = new List<Stream>();

            MultiThreadHelper helper = new MultiThreadHelper(filesToSearch.Count());

            foreach (Stream file in filesToSearch)
            {
                Task task = new Task(() =>
                {
                    if (file.SimpleSearch(word, searchName))
                    {
                        lock (filesWithWord)
                        {
                            filesWithWord.Add(file);
                        }
                    }
                });

                // TODO: Create structure for task completion.
                //task.ContinueWith((t) => Interlocked.Increment(ref helper.CompletedTasks));
                
                task.Start();
            }

            return filesWithWord;
        }

        public static async Task<IEnumerable<Stream>> SingleSearchAsync(this IEnumerable<Stream> filesToSearch, String word)
        {
            return SingleSearchAsync(filesToSearch, word, false).Result;
        }

        //public static List<Task<Stream>> SingleSearchAsync(this List<Stream> filesToSearch, String word)
        //{
        //    return SingleSearchAsync(filesToSearch, word, false);
        //}



    }
}
