using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HMM
{
    public class HMMtest2
    {
        public static void BaumWelchLearning()
        {
            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
            int[][] sequences = new int[][] 
{
    new int[] { 0,1,1,1,1,0,1,1,1,1 },
    new int[] { 0,1,1,1,0,1,1,1,1,1 },
    new int[] { 0,1,1,1,1,1,1,1,1,1 },
    new int[] { 0,1,1,1,1,1         },
    new int[] { 0,1,1,1,1,1,1       },
    new int[] { 0,1,1,1,1,1,1,1,1,1 },
    new int[] { 0,1,1,1,1,1,1,1,1,1 },
};

            // Creates a new Hidden Markov Model with 3 states for
            //  an output alphabet of two characters (zero and one)
            HiddenMarkovModel hmm = new HiddenMarkovModel(3, 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };

            double ll = teacher.Run(sequences);
            double l0 = Math.Exp(hmm.Evaluate(new int[] { 1, 0 }));  
            // Calculate the probability that the given
            //  sequences originated from the model
            double l1 = Math.Exp(hmm.Evaluate(new int[] { 0, 1 }));       // 0.999
            double l2 = Math.Exp(hmm.Evaluate(new int[] { 0, 1, 1, 1 })); // 0.916

            // Sequences which do not start with zero have much lesser probability.
            double l3 = Math.Exp(hmm.Evaluate(new int[] { 1, 1 }));       // 0.000
            double l4 = Math.Exp(hmm.Evaluate(new int[] { 1, 0, 0, 0 })); // 0.000

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            double l5 = Math.Exp(hmm.Evaluate(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 })); // 0.034
            double l6 = Math.Exp(hmm.Evaluate(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 })); // 0.034
        }
        public static void Viterbi()
        {
            // Create the transition matrix A
            double[,] transition = 
{  
    { 0.7, 0.3 },
    { 0.4, 0.6 }
};

            // Create the emission matrix B
            double[,] emission = 
{  
    { 0.1, 0.4, 0.5 },
    { 0.6, 0.3, 0.1 }
};

            // Create the initial probabilities pi
            double[] initial =
{
    0.6, 0.4
};

            // Create a new hidden Markov model
            HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);

            // After that, one could, for example, query the probability
            // of a sequence occurring. We will consider the sequence
            int[] sequence = new int[] { 0, 1, 2 };

            // And now we will evaluate its likelihood
            double logLikelihood = hmm.Evaluate(sequence);

            // At this point, the log-likelihood of the sequence
            // occurring within the model is -3.3928721329161653.

            // We can also get the Viterbi path of the sequence
            int[] path = hmm.Decode(sequence, out logLikelihood);

            // At this point, the state path will be 1-0-0 and the
            // log-likelihood will be -4.3095199438871337
        }
    }
}
