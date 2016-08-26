using System;
using System.Collections.Generic;
using System.Linq;

namespace VectorSpaceModel.Components
{
    internal class VectorSpaceModel
    {
        private readonly IList<double> _first;
        private readonly IList<double> _second;

        public VectorSpaceModel(IList<double> first, IList<double> second)
        {
            if (first == null || second == null)
                throw new ArgumentException("Error: collection was null.");

            if (first.Count() != second.Count())
                throw new ArgumentException(
                    "Error: collection cardinality different, need to be same in order to perform dot product operation");

            _second = second;
            _first = first;
        }

        public double CalculateCosineSimilarity(Func<double, double> predicate)
        {
            Enumerable.Range(0, _first.Count()).ToList().ForEach(i => _first[i] = predicate(_first[i]));
            Enumerable.Range(0, _second.Count()).ToList().ForEach(i => _second[i] = predicate(_second[i]));

            return CalculateCosineSimilarity();
        }

        public double CalculateCosineSimilarity()
        {
            double dotProduct = Enumerable.Range(0, _first.Count()).Sum(i => _first[i]*_second[i]);

            double firstVectorLength = Math.Sqrt(_first.Sum(weight => Math.Pow(weight, 2)));
            double secondVectorLength = Math.Sqrt(_second.Sum(weight => Math.Pow(weight, 2)));

            double lengths = (firstVectorLength*secondVectorLength);

            return (Math.Abs(lengths - 0.0) < double.Epsilon) ? 0 : dotProduct/lengths;
        }
    }
}