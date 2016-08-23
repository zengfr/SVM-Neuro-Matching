using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using System.Text.RegularExpressions;



namespace Service
{
    public class TFIDF
    {

        #region PRIVATE MEMBERS


        private string[] _docs;

        private string[][] ngramDoc;

        private int numDocs = 0;

        private int numTerms = 0;

        private ArrayList terms;

        private int[][] termFreq;

        private float[][] termWeight;

        private float[] termWeightSum;

        private float[] docWeightSum;

        private int[] maxTermFreq;

        private int[] docFreq;

        private IDictionary wordsIndex = new Hashtable();

        public List<int> DocResult = new List<int>();
        #endregion


        #region CONSTRUCTOR

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        public TFIDF(string[] documents)
        {
            _docs = documents;
            numDocs = documents.Length;
            MyInit();
            SummaryHelper();
        }

        /// <summary>
        /// This function does all work
        /// </summary>
        private void SummaryHelper()
        {
            termWeightSum = new float[termWeight.GetLength(0)];
            docWeightSum = new float[numDocs];

            var finalList = new List<int>();

            var DocDictionary = new List<Tuple<int, float, string>>();

            var row = termWeight.GetLength(0);
            var column = termWeight.Rank;
            for (int i = 0; i < termWeightSum.Length; i++)
            {
                termWeightSum[i] = 0;
            }

            for (int i = 0; i < termWeight.GetLength(0); i++)
            {

                for (int j = 0; j < numDocs; j++)
                {
                    termWeightSum[i] += termWeight[i][j];
                }

                Console.WriteLine();
            }

            for (int i = 0; i < numDocs; i++)
            {

                for (int j = 0; j < termWeight.GetLength(0); j++)
                {
                    docWeightSum[i] += termWeight[j][i];
                }

                DocDictionary.Add(new Tuple<int, float, string>(i, docWeightSum[i], _docs[i]));

                Console.WriteLine();
            }

            var sortedWeights = docWeightSum.ToList();

            var descendingWeight = sortedWeights.OrderByDescending(i => i).ToList();

            // you can change percentage of summary here
            var top30Percent = Convert.ToInt32(descendingWeight.Count * (0.3));

            var top30PercentElements = descendingWeight.Take(top30Percent).ToList();

            foreach (var item in top30PercentElements)
            {
                //if(DocDictionary.Any(t => t.Item2 == ))

                var filteredChainList = (from aObject in DocDictionary
                                         where aObject.Item2 == item
                                         select aObject.Item1).ToList();


                foreach (var item1 in filteredChainList)
                {
                    finalList.Add(item1);
                }

            }

            finalList.Sort();
            DocResult = finalList.Distinct().ToList();
            Console.WriteLine();
        }

        #endregion


        #region PUBLIC/PRIVATE EVENTS
        #endregion


        #region PUBLIC/PRIVATE EVENT HANDLERS
        #endregion


        #region PRIVATE METHODS

        private void GeneratNgramText()
        {

        }

        private ArrayList GenerateTerms(string[] docs)
        {
            ArrayList uniques = new ArrayList();
            ngramDoc = new string[numDocs][];
            for (int i = 0; i < docs.Length; i++)
            {
                Tokening tokenizer = new Tokening();
                string[] words = tokenizer.Partition(docs[i]);

                for (int j = 0; j < words.Length; j++)
                    if (!uniques.Contains(words[j]))
                        uniques.Add(words[j]);

            }
            return uniques;
        }


        private static object AddElement(IDictionary collection, object key, object newValue)
        {
            object element = collection[key];
            collection[key] = newValue;
            return element;
        }

        private int GetTermIndex(string term)
        {
            object index = wordsIndex[term];
            if (index == null) return -1;
            return (int)index;
        }

        private void MyInit()
        {
            terms = GenerateTerms(_docs);
            numTerms = terms.Count;

            maxTermFreq = new int[numDocs];
            docFreq = new int[numTerms];
            termFreq = new int[numTerms][];
            termWeight = new float[numTerms][];

            for (int i = 0; i < terms.Count; i++)
            {
                termWeight[i] = new float[numDocs];
                termFreq[i] = new int[numDocs];

                AddElement(wordsIndex, terms[i], i);
            }

            GenerateTermFrequency();
            GenerateTermWeight();

        }

        private float Log(float num)
        {
            return (float)Math.Log(num);//log2
        }

        private void GenerateTermFrequency()
        {
            for (int i = 0; i < numDocs; i++)
            {
                string curDoc = _docs[i];
                IDictionary freq = GetWordFrequency(curDoc);
                IDictionaryEnumerator enums = freq.GetEnumerator();
                maxTermFreq[i] = int.MinValue;
                while (enums.MoveNext())
                {
                    string word = (string)enums.Key;
                    int wordFreq = (int)enums.Value;
                    int termIndex = GetTermIndex(word);

                    termFreq[termIndex][i] = wordFreq;
                    docFreq[termIndex]++;

                    if (wordFreq > maxTermFreq[i]) maxTermFreq[i] = wordFreq;
                }
            }
        }

        private void GenerateTermWeight()
        {
            for (int i = 0; i < numTerms; i++)
            {
                for (int j = 0; j < numDocs; j++)
                    termWeight[i][j] = ComputeTermWeight(i, j);
            }
        }

        private float GetTermFrequency(int term, int doc)
        {
            int freq = termFreq[term][doc];
            int maxfreq = maxTermFreq[doc];

            return ((float)freq / (float)maxfreq);
        }

        private float GetInverseDocumentFrequency(int term)
        {
            int df = docFreq[term];
            return Log((float)(numDocs) / (float)df);
        }

        private float ComputeTermWeight(int term, int doc)
        {
            float tf = GetTermFrequency(term, doc);
            float idf = GetInverseDocumentFrequency(term);
            return tf * idf;
        }

        private float[] GetTerVec(int doc)
        {
            float[] w = new float[numTerms];
            for (int i = 0; i < numTerms; i++)
                w[i] = termWeight[i][doc];


            return w;
        }

        private IDictionary GetWordFrequency(string input)
        {
            string convertedInput = input.ToLower();

            Tokening tokenizer = new Tokening();
            String[] words = tokenizer.Partition(convertedInput);
            Array.Sort(words);

            String[] distinctWords = GetDistinctWords(words);

            IDictionary result = new Hashtable();
            for (int i = 0; i < distinctWords.Length; i++)
            {
                object tmp;
                tmp = CntWrd(distinctWords[i], words);
                result[distinctWords[i]] = tmp;

            }

            return result;
        }

        private string[] GetDistinctWords(String[] ip)
        {
            if (ip == null)
                return new string[0];
            else
            {
                ArrayList list = new ArrayList();

                for (int i = 0; i < ip.Length; i++)
                    if (!list.Contains(ip[i])) // N-GRAM SIMILARITY?				
                        list.Add(ip[i]);

                return Tokening.ArrLstToArr(list);
            }
        }

        private int CntWrd(string word, string[] words)
        {
            int item = Array.BinarySearch(words, word);

            if (item > 0)
                while (item > 0 && words[item].Equals(word))
                    item--;

            int count = 0;
            while (item < words.Length && item >= 0)
            {
                if (words[item].Equals(word)) count++;

                item++;
                if (item < words.Length)
                    if (!words[item].Equals(word)) break;

            }

            return count;
        }

        #endregion


        #region PUBLIC METHODS

        public float GiveEqual(int doc_i, int doc_j)
        {
            float[] v1 = GetTerVec(doc_i);
            float[] v2 = GetTerVec(doc_j);

            return TermVector.CosineEquality(v1, v2);

        }

        #endregion

        #region REQUIRED PUBLIC CLASS

        public class TermVector
        {
            public static float CosineEquality(float[] vector1, float[] vector2)
            {
                if (vector1.Length != vector2.Length)
                    throw new Exception("DIFER LENGTH");

                float de = (VectLen(vector1) * VectLen(vector2));
                if (de == 0F)
                    return 0F;
                else
                    return (InPrd(vector1, vector2) / de);

            }

            public static float InPrd(float[] vector1, float[] vector2)
            {

                if (vector1.Length != vector2.Length)
                    throw new Exception("DIFFER LENGTH ARE NOT ALLOWED");


                float result = 0F;
                for (int i = 0; i < vector1.Length; i++)
                    result += vector1[i] * vector2[i];

                return result;
            }

            public static float VectLen(float[] vector)
            {
                float sum = 0.0F;
                for (int i = 0; i < vector.Length; i++)
                    sum = sum + (vector[i] * vector[i]);

                return (float)Math.Sqrt(sum);
            }
        }
        #endregion
    }




    internal class Tokening
    {

        public static string[] ArrLstToArr(ArrayList arraylist)
        {
            string[] array = new string[arraylist.Count];
            for (int i = 0; i < arraylist.Count; i++) array[i] = (string)arraylist[i];
            return array;
        }

        public string[] Partition(string input)
        {
            Regex r = new Regex("([ \\t{}():;. \n])");
            input = input.ToLower();

            String[] tokens = r.Split(input);

            ArrayList fltr = new ArrayList();

            for (int i = 0; i < tokens.Length; i++)
            {
                MatchCollection mc = r.Matches(tokens[i]);
                if (mc.Count <= 0 && tokens[i].Trim().Length > 0
                    && !WordHandle.IsStopword(tokens[i]))
                    fltr.Add(tokens[i]);
            }
            return ArrLstToArr(fltr);
        }

        public Tokening()
        {
        }
    }

    public class WordHandle
    {
        public static string[] stopWordsList = new string[] {

															  "a", 																													  
															  "about", 
															  "above", 
															  "across", 
															  "afore", 
															  "aforesaid", 
															  "after", 
															  "again", 
															  "against", 
															  "agin", 
															  "ago", 
															  "aint", 
															  "albeit", 
															  "all", 
															  "almost", 
															  "alone", 
															  "along", 
															  "alongside", 
															  "already", 
															  "also", 
															  "although", 
															  "always", 
															  "am", 
															  "american", 
															  "amid", 
															  "amidst", 
															  "among", 
															  "amongst", 
															  "an", 
															  "and", 
															  "anent", 
															  "another", 
															  "any", 
															  "anybody", 
															  "anyone", 
															  "anything", 
															  "are", 
															  "aren't", 
															  "around", 
															  "as", 
															  "aslant", 
															  "astride", 
															  "at", 
															  "athwart", 
															  "away", 
															  "b", 
															  "back", 
															  "bar", 
															  "barring", 
															  "be", 
															  "because", 
															  "been", 
															  "before", 
															  "behind", 
															  "being", 
															  "below", 
															  "beneath", 
															  "beside", 
															  "besides", 
															  "best", 
															  "better", 
															  "between", 
															  "betwixt", 
															  "beyond", 
															  "both", 
															  "but", 
															  "by", 
															  "c", 
															  "can", 
															  "cannot", 
															  "can't", 
															  "certain", 
															  "circa", 
															  "close", 
															  "concerning", 
															  "considering", 
															  "cos", 
															  "could", 
															  "couldn't", 
															  "couldst", 
															  "d", 
															  "dare", 
															  "dared", 
															  "daren't", 
															  "dares", 
															  "daring", 
															  "despite", 
															  "did", 
															  "didn't", 
															  "different", 
															  "directly", 
															  "do", 
															  "does", 
															  "doesn't", 
															  "doing", 
															  "done", 
															  "don't", 
															  "dost", 
															  "doth", 
															  "down", 
															  "during", 
															  "durst", 
															  "e", 
															  "each", 
															  "early", 
															  "either", 
															  "em", 
															  "english", 
															  "enough", 
															  "ere", 
															  "even", 
															  "ever", 
															  "every", 
															  "everybody", 
															  "everyone", 
															  "everything", 
															  "except", 
															  "excepting", 
															  "f", 
															  "failing", 
															  "far", 
															  "few", 
															  "first", 
															  "five", 
															  "following", 
															  "for", 
															  "four", 
															  "from", 
															  "g", 
															  "gonna", 
															  "gotta", 
															  "h", 
															  "had", 
															  "hadn't", 
															  "hard", 
															  "has", 
															  "hasn't", 
															  "hast", 
															  "hath", 
															  "have", 
															  "haven't", 
															  "having", 
															  "he", 
															  "he'd", 
															  "he'll", 
															  "her", 
															  "here", 
															  "here's", 
															  "hers", 
															  "herself", 
															  "he's", 
															  "high", 
															  "him", 
															  "himself", 
															  "his", 
															  "home", 
															  "how", 
															  "howbeit", 
															  "however", 
															  "how's", 
															  "i", 
															  "id", 
															  "if", 
															  "ill", 
															  "i'm", 
															  "immediately", 
															  "important", 
															  "in", 
															  "inside", 
															  "instantly", 
															  "into", 
															  "is", 
															  "isn't", 
															  "it", 
															  "it'll", 
															  "it's", 
															  "its", 
															  "itself", 
															  "i've", 
															  "j", 
															  "just", 
															  "k", 
															  "l", 
															  "large", 
															  "last", 
															  "later", 
															  "least", 
															  "left", 
															  "less", 
															  "lest", 
															  "let's", 
															  "like", 
															  "likewise", 
															  "little", 
															  "living", 
															  "long", 
															  "m", 
															  "many", 
															  "may", 
															  "mayn't", 
															  "me", 
															  "mid", 
															  "midst", 
															  "might", 
															  "mightn't", 
															  "mine", 
															  "minus", 
															  "more", 
															  "most", 
															  "much", 
															  "must", 
															  "mustn't", 
															  "my", 
															  "myself", 
															  "n", 
															  "near", 
															  "'neath", 
															  "need", 
															  "needed", 
															  "needing", 
															  "needn't", 
															  "needs", 
															  "neither", 
															  "never", 
															  "nevertheless", 
															  "new", 
															  "next", 
															  "nigh", 
															  "nigher", 
															  "nighest", 
															  "nisi", 
															  "no", 
															  "no-one", 
															  "nobody", 
															  "none", 
															  "nor", 
															  "not", 
															  "nothing", 
															  "notwithstanding", 
															  "now", 
															  "o", 
															  "o'er", 
															  "of", 
															  "off", 
															  "often", 
															  "on", 
															  "once", 
															  "one", 
															  "oneself", 
															  "only", 
															  "onto", 
															  "open", 
															  "or", 
															  "other", 
															  "otherwise", 
															  "ought", 
															  "oughtn't", 
															  "our", 
															  "ours", 
															  "ourselves", 
															  "out", 
															  "outside", 
															  "over", 
															  "own", 
															  "p", 
															  "past", 
															  "pending", 
															  "per", 
															  "perhaps", 
															  "plus", 
															  "possible", 
															  "present", 
															  "probably", 
															  "provided", 
															  "providing", 
															  "public", 
															  "q", 
															  "qua", 
															  "quite", 
															  "r", 
															  "rather", 
															  "re", 
															  "real", 
															  "really", 
															  "respecting", 
															  "right", 
															  "round", 
															  "s", 
															  "same", 
															  "sans", 
															  "save", 
															  "saving", 
															  "second", 
															  "several", 
															  "shall", 
															  "shalt", 
															  "shan't", 
															  "she", 
															  "shed", 
															  "shell", 
															  "she's", 
															  "short", 
															  "should", 
															  "shouldn't", 
															  "since", 
															  "six", 
															  "small", 
															  "so", 
															  "some", 
															  "somebody", 
															  "someone", 
															  "something", 
															  "sometimes", 
															  "soon", 
															  "special", 
															  "still", 
															  "such", 
															  "summat", 
															  "supposing", 
															  "sure", 
															  "t", 
															  "than", 
															  "that", 
															  "that'd", 
															  "that'll", 
															  "that's", 
															  "the", 
															  "thee", 
															  "their", 
															  "theirs", 
															  "their's", 
															  "them", 
															  "themselves", 
															  "then", 
															  "there", 
															  "there's", 
															  "these", 
															  "they", 
															  "they'd", 
															  "they'll", 
															  "they're", 
															  "they've", 
															  "thine", 
															  "this", 
															  "tho", 
															  "those", 
															  "thou", 
															  "though", 
															  "three", 
															  "thro'", 
															  "through", 
															  "throughout", 
															  "thru", 
															  "thyself", 
															  "till", 
															  "to", 
															  "today", 
															  "together", 
															  "too", 
															  "touching", 
															  "toward", 
															  "towards", 
															  "true", 
															  "'twas", 
															  "'tween", 
															  "'twere", 
															  "'twill", 
															  "'twixt", 
															  "two", 
															  "'twould", 
															  "u", 
															  "under", 
															  "underneath", 
															  "unless", 
															  "unlike", 
															  "until", 
															  "unto", 
															  "up", 
															  "upon", 
															  "us", 
															  "used", 
															  "usually", 
															  "v", 
															  "versus", 
															  "very", 
															  "via", 
															  "vice", 
															  "vis-a-vis", 
															  "w", 
															  "wanna", 
															  "wanting", 
															  "was", 
															  "wasn't", 
															  "way", 
															  "we", 
															  "we'd", 
															  "well", 
															  "were", 
															  "weren't", 
															  "wert", 
															  "we've", 
															  "what", 
															  "whatever", 
															  "what'll", 
															  "what's", 
															  "when", 
															  "whencesoever", 
															  "whenever", 
															  "when's", 
															  "whereas", 
															  "where's", 
															  "whether", 
															  "which", 
															  "whichever", 
															  "whichsoever", 
															  "while", 
															  "whilst", 
															  "who", 
															  "who'd", 
															  "whoever", 
															  "whole", 
															  "who'll", 
															  "whom", 
															  "whore", 
															  "who's", 
															  "whose", 
															  "whoso", 
															  "whosoever", 
															  "will", 
															  "with", 
															  "within", 
															  "without", 
															  "wont", 
															  "would", 
															  "wouldn't", 
															  "wouldst", 
															  "x", 
															  "y", 
															  "ye", 
															  "yet", 
															  "you", 
															  "you'd", 
															  "you'll", 
															  "your", 
															  "you're", 
															  "yours", 
															  "yourself", 
															  "yourselves", 
															  "you've", 
															  "z", 
		};

        private static Hashtable _stopwords = null;

        public static object AddElement(IDictionary collection, Object key, object newValue)
        {
            object element = collection[key];
            collection[key] = newValue;
            return element;
        }

        public static bool IsStopword(string str)
        {


            return _stopwords.ContainsKey(str.ToLower());
        }


        public WordHandle()
        {
            if (_stopwords == null)
            {
                _stopwords = new Hashtable();
                double dummy = 0;
                foreach (string word in stopWordsList)
                {
                    AddElement(_stopwords, word, dummy);
                }
            }
        }
    }
}
