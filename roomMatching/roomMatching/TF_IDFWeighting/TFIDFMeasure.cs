using System;
using System.Collections;
using System.Collections.Generic;
//using TF_IDFWeighting;
using System.Linq;
using System.Windows.Forms.VisualStyles;


namespace ServiceRanking
{
	/// <summary>
	/// Summary description for TF_IDFLib.
	/// </summary>
	public class TFIDFMeasure
	{
        // Define private members of class in this region
        #region PRIVATE MEMBERS

        /// <summary>
        /// 
        /// </summary>
        private string[] _docs;
   
        /// <summary>
        /// 
        /// </summary>
        private string[][] _ngramDoc;
        
        /// <summary>
        /// 
        /// </summary>
        private int _numDocs = 0;
        
        /// <summary>
        /// 
        /// </summary>
        private int _numTerms = 0;
        
        /// <summary>
        /// 
        /// </summary>
        private ArrayList _terms;
        
        /// <summary>
        /// 
        /// </summary>
        private int[][] _termFreq;
        
        /// <summary>
        /// 
        /// </summary>
        private float[][] _termWeight;

        /// <summary>
        /// 
        /// </summary>
	    private float[] _termWeightSum;
	    
        /// <summary>
        /// 
        /// </summary>
        private float[] _docWeightSum;
        
        /// <summary>
        /// 
        /// </summary>
        private int[] _maxTermFreq;
        
        /// <summary>
        /// 
        /// </summary>
        private int[] _docFreq;

        /// <summary>
        /// 
        /// </summary>
        private IDictionary _wordsIndex = new Hashtable();

        public List<int> DocResult = new List<int>();  
        #endregion
        
        // Define class constructor in this region
        #region CONSTRUCTOR

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        public TFIDFMeasure(string[] documents)
        {
            _docs = documents;
            _numDocs = documents.Length;
            MyInit();
            SummaryHelper();
        }

        /// <summary>
        /// This function does all work
        /// </summary>
        private void SummaryHelper()
        {
            _termWeightSum = new float[_termWeight.GetLength(0)];
            _docWeightSum = new float[_numDocs];

            var finalList = new List<int>();
            
            var DocDictionary = new List<Tuple<int,float,string>>();

            var row = _termWeight.GetLength(0);
            var column = _termWeight.Rank;
            for (int i = 0; i < _termWeightSum.Length; i++)
            {
                _termWeightSum[i] = 0;
            }

            for (int i = 0; i < _termWeight.GetLength(0); i++)
            {
        
                for (int j = 0; j < _numDocs; j++)
                {
                    _termWeightSum[i] += _termWeight[i][j];
                }

                Console.WriteLine(  );
            }

            for (int i = 0; i < _numDocs; i++)
            {
        
                for (int j = 0; j < _termWeight.GetLength(0); j++)
                {
                    _docWeightSum[i] += _termWeight[j][i];
                }

                DocDictionary.Add(new Tuple<int,float,string>(i,_docWeightSum[i], _docs[i]));

                Console.WriteLine(  );
            }

            var sortedWeights = _docWeightSum.ToList();

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
            Console.WriteLine(  );
        }

        #endregion

        // Define all events of class in this region
        #region PUBLIC/PRIVATE EVENTS
        #endregion

        // Define all event handlers in this region
        #region PUBLIC/PRIVATE EVENT HANDLERS
        #endregion

        // Define all private methods in this region
        #region PRIVATE METHODS

        /// <summary>
        /// 
        /// </summary>
        private void GeneratNgramText()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docs"></param>
        /// <returns></returns>
        private ArrayList GenerateTerms(string[] docs)
        {
            ArrayList uniques = new ArrayList();
            _ngramDoc = new string[_numDocs][];
            for (int i = 0; i < docs.Length; i++)
            {
                Tokeniser tokenizer = new Tokeniser();
                string[] words = tokenizer.Partition(docs[i]);

                for (int j = 0; j < words.Length; j++)
                    if (!uniques.Contains(words[j]))
                        uniques.Add(words[j]);

            }
            return uniques;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private static object AddElement(IDictionary collection, object key, object newValue)
        {
            object element = collection[key];
            collection[key] = newValue;
            return element;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private int GetTermIndex(string term)
        {
            object index = _wordsIndex[term];
            if (index == null) return -1;
            return (int)index;
        }

        /// <summary>
        /// 
        /// </summary>
        private void MyInit()
        {
            _terms = GenerateTerms(_docs);
            _numTerms = _terms.Count;

            _maxTermFreq = new int[_numDocs];
            _docFreq = new int[_numTerms];
            _termFreq = new int[_numTerms][];
            _termWeight = new float[_numTerms][];

            for (int i = 0; i < _terms.Count; i++)
            {
                _termWeight[i] = new float[_numDocs];
                _termFreq[i] = new int[_numDocs];

                AddElement(_wordsIndex, _terms[i], i);
            }

            GenerateTermFrequency();
            GenerateTermWeight();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private float Log(float num)
        {
            return (float)Math.Log(num);//log2
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateTermFrequency()
        {
            for (int i = 0; i < _numDocs; i++)
            {
                string curDoc = _docs[i];
                IDictionary freq = GetWordFrequency(curDoc);
                IDictionaryEnumerator enums = freq.GetEnumerator();
                _maxTermFreq[i] = int.MinValue;
                while (enums.MoveNext())
                {
                    string word = (string)enums.Key;
                    int wordFreq = (int)enums.Value;
                    int termIndex = GetTermIndex(word);

                    _termFreq[termIndex][i] = wordFreq;
                    _docFreq[termIndex]++;

                    if (wordFreq > _maxTermFreq[i]) _maxTermFreq[i] = wordFreq;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void GenerateTermWeight()
        {
            for (int i = 0; i < _numTerms; i++)
            {
                for (int j = 0; j < _numDocs; j++)
                    _termWeight[i][j] = ComputeTermWeight(i, j);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private float GetTermFrequency(int term, int doc)
        {
            int freq = _termFreq[term][doc];
            int maxfreq = _maxTermFreq[doc];

            return ((float)freq / (float)maxfreq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private float GetInverseDocumentFrequency(int term)
        {
            int df = _docFreq[term];
            return Log((float)(_numDocs) / (float)df);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private float ComputeTermWeight(int term, int doc)
        {
            float tf = GetTermFrequency(term, doc);
            float idf = GetInverseDocumentFrequency(term);
            return tf * idf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private float[] GetTermVector(int doc)
        {
            float[] w = new float[_numTerms];
            for (int i = 0; i < _numTerms; i++)
                w[i] = _termWeight[i][doc];


            return w;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IDictionary GetWordFrequency(string input)
        {
            string convertedInput = input.ToLower();

            Tokeniser tokenizer = new Tokeniser();
            String[] words = tokenizer.Partition(convertedInput);
            Array.Sort(words);

            String[] distinctWords = GetDistinctWords(words);

            IDictionary result = new Hashtable();
            for (int i = 0; i < distinctWords.Length; i++)
            {
                object tmp;
                tmp = CountWords(distinctWords[i], words);
                result[distinctWords[i]] = tmp;

            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string[] GetDistinctWords(String[] input)
        {
            if (input == null)
                return new string[0];
            else
            {
                ArrayList list = new ArrayList();

                for (int i = 0; i < input.Length; i++)
                    if (!list.Contains(input[i])) // N-GRAM SIMILARITY?				
                        list.Add(input[i]);

                return Tokeniser.ArrayListToArray(list);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        private int CountWords(string word, string[] words)
        {
            int itemIdx = Array.BinarySearch(words, word);

            if (itemIdx > 0)
                while (itemIdx > 0 && words[itemIdx].Equals(word))
                    itemIdx--;

            int count = 0;
            while (itemIdx < words.Length && itemIdx >= 0)
            {
                if (words[itemIdx].Equals(word)) count++;

                itemIdx++;
                if (itemIdx < words.Length)
                    if (!words[itemIdx].Equals(word)) break;

            }

            return count;
        }				

        #endregion

        // Define all public methods in this region
        #region PUBLIC METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc_i"></param>
        /// <param name="doc_j"></param>
        /// <returns></returns>
        public float GetSimilarity(int doc_i, int doc_j)
        {
            float[] vector1 = GetTermVector(doc_i);
            float[] vector2 = GetTermVector(doc_j);

            return TermVector.ComputeCosineSimilarity(vector1, vector2);

        }

        #endregion

        #region REQUIRED PUBLIC CLASS
        
        /// <summary>
        /// 
        /// </summary>
        public class TermVector
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="vector1"></param>
            /// <param name="vector2"></param>
            /// <returns></returns>
            public static float ComputeCosineSimilarity(float[] vector1, float[] vector2)
            {
                if (vector1.Length != vector2.Length)
                    throw new Exception("DIFER LENGTH");


                float denom = (VectorLength(vector1) * VectorLength(vector2));
                if (denom == 0F)
                    return 0F;
                else
                    return (InnerProduct(vector1, vector2) / denom);

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="vector1"></param>
            /// <param name="vector2"></param>
            /// <returns></returns>
            public static float InnerProduct(float[] vector1, float[] vector2)
            {

                if (vector1.Length != vector2.Length)
                    throw new Exception("DIFFER LENGTH ARE NOT ALLOWED");


                float result = 0F;
                for (int i = 0; i < vector1.Length; i++)
                    result += vector1[i] * vector2[i];

                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="vector"></param>
            /// <returns></returns>
            public static float VectorLength(float[] vector)
            {
                float sum = 0.0F;
                for (int i = 0; i < vector.Length; i++)
                    sum = sum + (vector[i] * vector[i]);

                return (float)Math.Sqrt(sum);
            }

        }

        #endregion

    }
}
