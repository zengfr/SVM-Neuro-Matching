using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SimilarityMeasure
{
    class SimilarityCalculator
    {
        public double CosineSimilarity(Vector v1, Vector v2, double norm)
        {
            v1.Normalize(p: norm);
            v2.Normalize(p: norm);
            double l1 = Math.Sqrt(v1 * v1);
            double l2 = Math.Sqrt(v2 * v2);
            double similarity = (v1 * v2) / (l1 * l2);

            return similarity;
        }

        public double CosineSimilarity(Vector v1, Vector v2)
        {
            double l1 = Math.Sqrt(v1 * v1);
            double l2 = Math.Sqrt(v2 * v2);
            double similarity = (v1 * v2) / (l1 * l2);

            return similarity;
        }

        public double Compare(string url1, string url2, int vocabularyThreshold)
        {
            Spider spider = new Spider();
            spider.AddUrl(url1);
            spider.AddUrl(url2);
            spider.Fetch();

            Dictionary<string, string> data = spider.Data;

            List<string> docs = new List<string>();
            foreach (string doc in data.Values) { docs.Add(doc); }

            List<List<string>> stemmedDocs;
            List<string> vocabulary;
            Tokenizer tk = new Tokenizer();
            vocabulary = tk.GetVocabulary(docs, out stemmedDocs, vocabularyThreshold: vocabularyThreshold);

            TFIDFModel _tfIDFModel = new TFIDFModel(vocabulary);
            stemmedDocs.ForEach(sd => _tfIDFModel.LoadDocument(sd));
            _tfIDFModel.UpdateTFIDFVectorRepresenation();

            SparseVector v1 = _tfIDFModel._vectorRepresentation[0];
            SparseVector v2 = _tfIDFModel._vectorRepresentation[1];

            double sim = CosineSimilarity(v1, v2, norm: 2.0);

            Console.WriteLine("url1 consists of {0} words, url2 consists of {1} words.", stemmedDocs[0].Count, stemmedDocs[1].Count);
            Console.WriteLine("Vocabulary contains {0} words after tokenization and thresholding.", vocabulary.Count);
            Console.WriteLine("Similarity is {0}.", sim);
            Console.WriteLine("\n");

            return sim;
        }
    }
}
