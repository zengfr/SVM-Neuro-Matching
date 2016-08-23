using EnglishStemmer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimilarityMeasure
{
    class Tokenizer
    {

        // Parses and tokenizes a list of documents, returning a vocabulary of words
        public List<string> GetVocabulary(List<string> docs, out List<List<string>> stemmedDocs, int vocabularyThreshold)
        {
            List<string> vocabulary = new List<string>();
            Dictionary<string, int> wordCountList = new Dictionary<string, int>();
            stemmedDocs = new List<List<string>>();

            foreach (var doc in docs)
            {
                List<string> stemmedDoc = new List<string>();

                string[] parts2 = Tokenize(doc);

                List<string> words = new List<string>();
                foreach (string part in parts2)
                {
                    // Strip non-alphanumeric characters
                    string stripped = Regex.Replace(part, "[^a-zA-Z0-9]", "");

                    if (!StopWords.stopWordsList.Contains(stripped.ToLower()))
                    {
                        try
                        {
                            var english = new EnglishWord(stripped);
                            string stem = english.Stem;
                            words.Add(stem);

                            if (stem.Length > 0)
                            {
                                // Build the word count list
                                if (wordCountList.ContainsKey(stem))
                                {
                                    wordCountList[stem]++;
                                }
                                else
                                {
                                    wordCountList.Add(stem, 0);
                                }

                                stemmedDoc.Add(stem);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Tokenizer exception source: {0}", e.Message);
                        }
                    }
                }

                stemmedDocs.Add(stemmedDoc);
            }

            // Get the top words
            var vocabList = wordCountList.Where(w => w.Value >= vocabularyThreshold);
            foreach (var item in vocabList)
            {
                vocabulary.Add(item.Key);
            }

            return vocabulary;
        }

        // Tokenizes a string, returning its list of words
        private static string[] Tokenize(string text)
        {
            // Strip all HTML
            text = Regex.Replace(text, "<[^<>]+>", "");

            // Strip numbers
            text = Regex.Replace(text, "[0-9]+", "number");

            // Strip urls
            text = Regex.Replace(text, @"(http|https)://[^\s]*", "httpaddr");

            // Strip email addresses
            text = Regex.Replace(text, @"[^\s]+@[^\s]+", "emailaddr");

            // Strip dollar sign
            text = Regex.Replace(text, "[$]+", "dollar");

            // Tokenize and also get rid of any punctuation
            return text.Split(" @$/#.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray());
        }
    }
}
