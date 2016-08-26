using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VectorSpaceModel.Components
{
    public class Document<T> : IEnumerable<T>
    {
        private readonly IDictionary<T, double> _augmentedFrequency = new Dictionary<T, double>();
        private readonly IDictionary<T, bool> _booleanFrequency = new Dictionary<T, bool>();
        private readonly IDictionary<T, double> _logaritmicFrequency = new Dictionary<T, double>();
        private readonly IDictionary<T, int> _regularFrequency = new Dictionary<T, int>();
        private readonly IList<T> _terms;

        private double? _maxFrequency;


        public Document(string id, params T[] terms)
            : this(terms)
        {
            ID = id;
        }

        public Document(params T[] terms)
        {
            _terms = terms.ToList();
        }

        public string ID { get; private set; }

        public int Count
        {
            get { return _terms.Count; }
        }

        public T this[int index]
        {
            get { return _terms[index]; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _terms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public double BooleanSimilarity(Document<T> query, IList<Document<T>> corpus)
        {
            List<T> vocabulary = corpus.SelectMany(term => term).Distinct().ToList();

            double[] queryWeights = vocabulary.Select(query.BooleanTermFrequency).ToArray();
            double[] documentWeights = vocabulary.Select(BooleanTermFrequency).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }

        public double AugumentedSimilarity(Document<T> query, IList<Document<T>> corpus)
        {
            List<T> vocabulary = corpus.SelectMany(term => term).Distinct().ToList();

            double[] queryWeights = vocabulary.Select(query.AugmentedTermFrequency).ToArray();
            double[] documentWeights = vocabulary.Select(AugmentedTermFrequency).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }

        public double TFIDFSimilarity(Document<T> query, Corpus<T> corpus)
        {
            double[] queryWeights = corpus.Select(term => query.TFIDF(term, corpus)).ToArray();
            double[] documentWeights = corpus.Select(term => TFIDF(term, corpus)).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }

        public double BooleanTFIDFSimilarity(Document<T> query, Corpus<T> corpus)
        {
            double[] queryWeights =
                corpus.Select(term => query.TFIDF(term, corpus, query.BooleanTermFrequency)).ToArray();
            double[] documentWeights = corpus.Select(term => TFIDF(term, corpus, BooleanTermFrequency)).ToArray();

            var vectorSpaceModel = new VectorSpaceModel(queryWeights, documentWeights);

            return vectorSpaceModel.CalculateCosineSimilarity();
        }


        private void CalculateMaxFrequency()
        {
            _maxFrequency = _terms.Select(RegularTermFrequency).Max();
        }

        public int RegularTermFrequency(T term)
        {
            if (!_regularFrequency.ContainsKey(term))
            {
                _regularFrequency[term] = _terms.Where(dt => dt.Equals(term)).Select(dt => 1).Sum();
            }

            return _regularFrequency[term];
        }

        public double LogaritmicTermFrequency(T term)
        {
            if (!_logaritmicFrequency.ContainsKey(term))
            {
                _logaritmicFrequency[term] = Math.Log10(RegularTermFrequency(term) + 1d);
            }
            return _logaritmicFrequency[term];
        }

        public double AugmentedTermFrequency(T term)
        {
            if (_maxFrequency == null)
            {
                CalculateMaxFrequency();
            }
            Debug.Assert(_maxFrequency != null, "_maxFrequency != null");

            if (!_augmentedFrequency.ContainsKey(term))
            {
                _augmentedFrequency[term] = 0.5d + (0.5d*RegularTermFrequency(term))/(double) _maxFrequency;
            }

            return _augmentedFrequency[term];
        }

        public double TFIDF(T term, Corpus<T> corpus, Func<T, double> termFrequencyFunction)
        {
            return termFrequencyFunction(term)*corpus.IDF(term);
        }

        public double TFIDF(T term, Corpus<T> corpus)
        {
            return TFIDF(term, corpus, AugmentedTermFrequency);
        }


        public double BooleanTermFrequency(T term)
        {
            if (!_booleanFrequency.ContainsKey(term))
            {
                _booleanFrequency[term] = _terms.Contains(term);
            }
            return _booleanFrequency[term] ? 1d : 0d;
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, Terms: [{1}]", ID,
                string.Join(",", _terms.Select(term => term.ToString()).ToArray()));
        }
    }
}