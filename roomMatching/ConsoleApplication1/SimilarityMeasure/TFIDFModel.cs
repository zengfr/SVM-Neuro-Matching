using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SimilarityMeasure
{
    class TFIDFModel
    {
        public List<List<string>> _documents = new List<List<string>>();

        private readonly List<string> _vocabulary = new List<string>();
        private readonly Dictionary<string, int> _termIdx = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _termCount = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _termIDF = new Dictionary<string, int>();

        public readonly List<SparseVector> _vectorRepresentation = new List<SparseVector>();

        private static int _termIdxCounter = 0;

        public TFIDFModel(List<string> vocabulary)
        {
            _vocabulary = vocabulary;
        }

        public void LoadDocument(List<string> document)
        {
            _documents.Add(document);
            LoadToDictionaries(document);
        }

        public void UpdateTFIDFVectorRepresenation()
        {
            foreach (List<string> d in _documents)
            {
                List<double> vector = new List<double>();

                foreach (string term in _termIdx.Keys)
                {
                    double _termFrequency = TermFrequency(d, term);
                    double _inverseDocumentFrequency = InverseDocumentFrequency((double)_documents.Count, (double)_termCount[term]);
                    //Console.WriteLine("Num of docs in corpora: " + _documents.Count);
                    //Console.WriteLine(term + " : " + _termCount[term]);
                    //Console.WriteLine(term + " IDF : " + _inverseDocumentFrequency);

                    vector.Add(_termFrequency * _inverseDocumentFrequency);
                }

                _vectorRepresentation.Add(SparseVector.OfEnumerable(vector));
            }
        }

        private double TermFrequency(List<string> document, string term)
        {
            double tf = document.Count(x => x == term);
            if (tf > 0.0)  { tf = 1.0 + Math.Log(tf); }
            return tf;
        }

        private double InverseDocumentFrequency(double numOfDocs, double numOfDocsContTerm)
        {
            double idf = Math.Log(numOfDocs / numOfDocsContTerm) + 1.0;
            // double idf = Math.Log((numOfDocs - numOfDocsContTerm + 0.5) / (0.5 + numOfDocsContTerm)) + 1.0;
            return idf;
            
        }

        private void LoadToDictionaries(List<string> document)
        {
            foreach (string term in _vocabulary)
            {
                if (!_termIdx.ContainsKey(term))
                {
                    _termIdx[term] = _termIdxCounter;
                    System.Threading.Interlocked.Increment(ref _termIdxCounter);
                }

                if (!_termCount.ContainsKey(term))
                {
                    _termCount[term] = 1;
                }
                else
                {
                    _termCount[term]++;
                }
            }
        }
    }
}
