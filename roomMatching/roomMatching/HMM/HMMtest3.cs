using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HMM
{
   public  class HMMtest3
    {

        public static void run()
      {

        ////隐马尔科夫模型的研究建模
        ////http://www.zhihu.com/question/20962240
          var pS = new double[] { 1.0 / 3, 1.0 / 3, 1.0 / 3 };
          var p4N = new double[] { 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4 };
          var p6N = new double[] { 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6, 1.0 / 6 };
          var p8N = new double[] { 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8, 1.0 / 8 };
          var pN = new double[][] { p4N, p6N, p8N };

        ////问题0
        ////状态链
        //var sn0 = new int[,] { { 1, 1 }, { 2, 6 }, { 2, 3 } };
        //var pI = CountProbability(pS, pN, sn0);

        ////问题1：看见不可见的，破解骰子序列（取3个序列,计算出隐式链中最大可能链与其出现概率,我这里只计算了概率，最大可能链的计算方法同理）
        ////解法1：穷举所有可能的骰子序列
        //var pList = new List<double>();
        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        for (int k = 0; k < 3; k++)
        //        {
        //            //状态链
        //            var sn1 = new int[,] { { i, 1 }, { j, 6 }, { k, 3 } };
        //            var pI1 = CountProbability(pS, pN, sn1);

        //            pList.Add(pI1);
        //        }
        //    }
        //}

        ////解法2：Viterbi algorithm (最大概率累加 1 2 3 .... n)
          var sn2 = new int[] { 1, 6, 3 };
          var pI2 = ViterbiAlgorithm(pS, pN, sn2);

        ////问题2：谁动了我的骰子？(取3个序列,计算出显式链序列的出现概率)
        ////解法1：穷举所有可能的骰子序列（上面就有了，所以不写了）

        ////解法2：前向算法（forward algorithm）
        //var sn3 = new int[] { 1, 6, 3 };

        //var pI3 = ForwardAlgorithm(pS, pN, sn3, sn3.Length).Sum();

        //问题3：BaumWelch算法 http://www.52nlp.cn/hmm-learn-best-practices-seven-forward-backward-algorithm-5
        Double LogProbInit = 0.0;
        Double LogProbFinal = 0.0;
        var sn4 = new int[] { 1, 6, 3 };
       // var hmm = new Splash.HMM(3, 8);
       //var round = hmm.BaumWelch(sn4, out LogProbInit, out LogProbFinal);

//        Result = round.ToString() + ":" + LogProbInit + ":" + LogProbFinal;
//        Result = pI3.ToString();
//        Result = string.Join("   ", pList.OrderByDescending(p => p)) + "---:" + pI2;
//        Result = total.ToString();
    }

    private static double[] ForwardAlgorithm(double[] pS, double[][] pN, int[] sn, int count = 0)
    {
        if (count == 0)
        {
            return new[] {1.0};
        }
        else if (count > sn.Length)
        {
            count = sn.Length;
        }

        //递归获取前一个概率的集合,用于这一次的计算
        var preventResult = ForwardAlgorithm(pS, pN, sn, count - 1);

        //获取当前的显式值
        var nowS = sn[count - 1];
        var psLength = pS.Length;
        
        //定义返回值
        var res = new double[psLength];
        for (int i = 0; i < psLength; i++)
        {
            var sntemp = new int[,] { { i, nowS } };
            var pi = CountProbability(pS, pN, sntemp);
            var sump = preventResult.Sum();
            res[i] = sump*pi;
        }
        return res;
    }

    /// <summary>
    /// Viterbi algorithm算法计算可能的马尔科夫1阶序列最大概率
    /// </summary>
    /// <param name="pS"></param>
    /// <param name="pN"></param>
    /// <param name="sn"></param>
    /// <returns></returns>
    private static double ViterbiAlgorithm(double[] pS, double[][] pN, int[] sn)
    {
        var res = 1.0;
        foreach (var s in sn)
        {
            res *= CountMaxProbabilityFromSn(pS, pN, s);
        }
        return res;
    }

    private static double CountMaxProbabilityFromSn(double[] pS, double[][] pN, int oneSn)
    {
        var max = 0.0;
        var length = pS.Length;
        for (int i = 0; i < length; i++)
        {
            var sn = new int[,] { { i, oneSn } };
            var pi = CountProbability(pS, pN, sn);
            if (pi > max)
            {
                max = pi;
            }
        }
        return max;
    }

    /// <summary>
    /// 计算状态链的概率
    /// </summary>
    /// <param name="pS"></param>
    /// <param name="pN"></param>
    /// <param name="sn"></param>
    /// <param name="snlength"></param>
    /// <returns></returns>
    private static double CountProbability(double[] pS, double[][] pN, int[,] sn)
    {
        try
        {
            var snlength = sn.Length/2;
            var pI = 1.0;
            for (int i = 0; i < snlength; i++)
            {
                var ps = pS[sn[i, 0]];
                var pn = pN[sn[i, 0]][sn[i, 1] - 1];
                
                pI *= ps * pn;
            }
            return pI;
        }
        catch
        {
            return 0.0;
        }
    }
    }
}
