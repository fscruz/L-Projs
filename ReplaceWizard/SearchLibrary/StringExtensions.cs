using SearchLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLibrary
{
    public static class StringExtensions
    {

        // TODO: Finish
        /// <summary>
        /// Split current instance 
        /// </summary>
        /// <param name="originalText"></param>
        /// <returns></returns>
        public static string[] SplitToMinimun(this string originalText)
        {
            List<string> splittedWords = new List<string>();
            List<string> quotedPhrases = new List<string>();
            string splittedText = originalText;

            // Separate quoted texts.
            while(splittedText.Contains("\""))
            {
                try
                {
                    string quoted = splittedText.RemoveQuotedWord();
                    quotedPhrases.Add(quoted);

                    // quoted comes without quotes (on purpose); turn it to "quoted" to fully remove;
                    quoted = "\"" + quoted + "\"";
                    splittedText = splittedText.RemoveFirst(quoted);
                }
                catch (NotFullyQuotedException)
                {
                    // Warn user about lack of quotes
                }
            }

            // Separate remaining words (through spacing).
            splittedWords.AddRange(splittedText.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries));

            // Concatenate result, sorting by appearance.
            List<string> results = new List<string>();
            while(splittedWords.Count > 0 & quotedPhrases.Count > 0)
            {
                if(originalText.IndexOf(splittedWords[0]) > originalText.IndexOf(quotedPhrases[0]))
                {
                    results.Add(quotedPhrases[0]);
                    quotedPhrases.RemoveAt(0);
                }
                else
                {
                    results.Add(splittedWords[0]);
                    splittedWords.RemoveAt(0);
                }
            }

            // At this point one of the lists have been completed added to the results so 
            //all we need is to add the rest of the other.
            if(splittedWords.Count > 0)
            {
                results.AddRange(splittedWords);
            }
            else if (quotedPhrases.Count > 0)
            {
                results.AddRange(quotedPhrases);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Returns a string containing the first ocurrence of a quoted text in the current instance.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>String containing the first ocurrence of a quoted text.</returns>
        //protected
        public static string RemoveQuotedWord(this string text)
        {
            int firstQuoteIndex = text.IndexOf('\"');
            int secondQuoteIndex = text.IndexOf('\"', firstQuoteIndex + 1);

            // Certify there is more than one quote.
            if (secondQuoteIndex < firstQuoteIndex)
            {
                throw new NotFullyQuotedException("There is no text surronded by quotes in: " + text);
            }

            // Split in three parts: One pre-quotes;
            string firstNonQuoted = text.Substring(0, firstQuoteIndex);
            //one between quotes;
            string quoted = text.Substring(firstQuoteIndex, secondQuoteIndex - firstQuoteIndex);
            //and one after quotes;
            string secondNonQuoted = text.Substring(secondQuoteIndex + 1, text.Length - secondQuoteIndex - 1);
            
            return quoted.Replace("\"", "");
        }


        /// <summary>
        /// Returns a string where the first ocurrence of the specified string has been removed.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="removeValue">String to remove.</param>
        /// <returns></returns>
        public static string RemoveFirst(this string text, string removeValue)
        {
            int startOfValue = text.IndexOf(removeValue);

            return text.Remove(startOfValue, removeValue.Length);
        }


        /// <summary>
        /// Searchs current instance for a occurence of a given string. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="desiredWord"></param>
        /// <returns>Returns the index of the first occurence of desiredWord and -1 if it doesn't find any.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int Find(this string source, string desiredWord)
        {
            if (desiredWord == null || desiredWord == "")
            {
                throw new ArgumentNullException("Argument desiredWord in method Find cannot be null or empty");
            }

            if (source.Contains(desiredWord))
            {
                return source.IndexOf(desiredWord);
            }
            else
            {
                return -1;
            }

        }

        /// <summary>
        /// Returns the indexes of all ocurrences of a given string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="desiredWord"></param>
        /// <returns></returns>
        public static IEnumerable<int> FindAll(this string source, string desiredWord)
        {
            List<int> indexes = new List<int>();
            int index = source.IndexOf(desiredWord);

            while (index > -1)
            {
                indexes.Add(index);

                index = source.IndexOf(desiredWord, index + 1);
            }

            return indexes;
        }

        /// <summary>
        /// Removes all ocurrences of a given string, considering only those surrounded by a given set of characters
        /// </summary>
        /// <param name="currentText"></param>
        /// <param name="word">The word to remove.</param>
        /// <param name="ignorableChars">Set of characters to consider as possible frontiers for the word. Example: Spacing, ' ', or line endings, '\n'</param>
        /// <returns></returns>
        /// <remarks>The char ' ' is always included in the set of possibilities for word surronding</remarks>
        public static string RemoveWord(this string currentText, string word, params char[] ignorableChars)
        {
            // Get all position indexes for chosen word in current instance.
            List<int> indexesOfOccurence = currentText.FindAll(word).ToList();
            
            List<char> ignChars;
            
            if(ignorableChars != null)
            {
                ignChars = ignorableChars.ToList();
            }
            else
            {
                ignChars = new List<char>();
            }

            // Sort indexes descending to avoid changing position indexes.
            indexesOfOccurence.Sort((i, j) => -i.CompareTo(j));

            string result = currentText;

            // Add space to list of ignorable characters.
            ignChars.Add(' ');

            foreach(int index in indexesOfOccurence)
            {
                // If trailing char is ignorable
                if(index > 0 && IsIgnorable(result[index - 1], ignChars.ToArray()))
                {
                    //and also the following char.
                    if(index < result.Length - word.Length && IsIgnorable(result[index + word.Length], ignChars.ToArray()))
                    {
                        // Remove word.
                        result = result.Remove(index, word.Length);
                    }
                }
            }

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toIgnoreChar"></param>
        /// <param name="ignorableChars"></param>
        /// <returns></returns>
        private static bool IsIgnorable(char toIgnoreChar, char[] ignorableChars)
        {
            return ignorableChars.Contains(toIgnoreChar);
        }

    }
}
