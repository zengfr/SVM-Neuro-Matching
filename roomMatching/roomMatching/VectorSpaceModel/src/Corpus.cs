using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VectorSpaceModel.Components
{
    public class Corpus<T> : IEnumerable<T>
    {
        private readonly IList<Document<T>> _documents;
        private readonly IDictionary<T, double> _invertedDocumentFrequency = new Dictionary<T, double>();
        private readonly IList<T> _vocabulary;

        public Corpus(params Document<T>[] documents)
            : this(documents.ToList())
        {
        }

        public Corpus(IEnumerable<Document<T>> documents) : this(documents, true)
        {
        }

        public Corpus(IEnumerable<Document<T>> documents, bool calculateIDF)
        {
            if (documents == null)
            {
                throw new ArgumentException("Error! documents collection is null.");
            }
            _documents = documents.ToList();
            _vocabulary = _documents.SelectMany(term => term).Distinct().ToList();
            if (calculateIDF)
            {
                CalculateInverseDocumentFrequency();
            }
        }


        public IList<Document<T>> Documents
        {
            get { return _documents; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _vocabulary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void CalculateInverseDocumentFrequency()
        {
            foreach (T term in _vocabulary)
            {
                if (_invertedDocumentFrequency.ContainsKey(term)) continue;
                double termCount = _documents.Select(document => document.BooleanTermFrequency(term)).Sum();
                _invertedDocumentFrequency[term] = ((int) termCount) == 0 ? 0 : Math.Log10(_documents.Count/termCount);
            }
        }

        public double IDF(T index)
        {
            return _invertedDocumentFrequency[index];
        }
    }
}