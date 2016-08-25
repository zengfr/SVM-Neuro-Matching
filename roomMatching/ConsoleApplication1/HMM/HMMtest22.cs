using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HMM
{
    public class HMMtest22
    {
        public static void BaumWelchLearning()
        {
            //var pS = new double[] { 1.0 / 3, 1.0 / 3, 1.0 / 3 };
            //var p4N = new double[] { 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4 };
            //var p6N = new double[] { 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6 };
            //var p8N = new double[] { 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8 };
            //var pN = new double[][] { p4N, p6N, p8N };

            int[][] sequences = new int[][] 
{
  new  int[] { 1, 1 },
 new  int[] { 2, 6 }, 
 new  int[] { 2, 3 } 
};

            HiddenMarkovModel hmm = new HiddenMarkovModel(3,8);
            var teacher = new BaumWelchLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };

            var m2=teacher.Learn(sequences);
            double l0 = Math.Exp(m2.Evaluate(new int[] { 1, 6,3 }));  
             
        }
        public static void Viterbi()
        {
            // Create the transition matrix A
            double[,] transition = 
{  
    { 1.0 / 3, 1.0 / 3, 1.0 / 3 },
    { 1.0 / 3, 1.0 / 3, 1.0 / 3 },
    { 1.0 / 3, 1.0 / 3, 1.0 / 3 }
};

            // Create the emission matrix B
            double[,] emission = 
{  
              { 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4,0,0,0,0 },
              { 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6,0,0 },
              { 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8 },
};

            // Create the initial probabilities pi
            double[] initial =
{
    0.3,0.3,0.4
};

            //var pS = new double[] { 1.0 / 3, 1.0 / 3, 1.0 / 3 };
            //var p4N = new double[] { 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4 };
            //var p6N = new double[] { 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6 };
            //var p8N = new double[] { 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8 };
            //var pN = new double[][] { p4N, p6N, p8N };

            HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);

            int[] sequence = new int[] { 1, 6, 3 };
            double logLikelihood = hmm.Evaluate(sequence);

       
            int[] path = hmm.Decode(sequence, out logLikelihood);
        }
    }
}
