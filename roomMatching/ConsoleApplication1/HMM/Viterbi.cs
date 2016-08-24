using System;

namespace Splash
{
    partial class HMM
    {
        /// <summary>
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列
        /// Viterbi Algorithm: Finding most probable sequence of hidden states
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>
        /// <returns>可能性最大的隐藏状态序列</returns>
        /// <remarks>使用双精度运算，不输出中间结果</remarks>
        public Int32[] Viterbi(Int32[] OB, out Double Probability)
        {
            Double[,] DELTA;
            Int32[,] PSI;

            return Viterbi(OB, out DELTA, out PSI, out Probability);
        }

        /// <summary>
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="DELTA">输出中间结果：局部最大概率</param>
        /// <param name="PSI">输出中间结果：反向指针指示最可能路径</param>
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>
        /// <returns>可能性最大的隐藏状态序列</returns> 
        /// <remarks>使用双精度运算，且输出中间结果</remarks>
        public Int32[] Viterbi(Int32[] OB, out Double[,] DELTA, out Int32[,] PSI, out Double Probability)
        {
            DELTA = new Double[OB.Length, N];   // 局部概率
            PSI = new Int32[OB.Length, N];      // 反向指针

            // 1. 初始化
            for (Int32 j = 0; j < N; j++)
            {
                DELTA[0, j] = PI[j] * B[j, OB[0]];
            }

            // 2. 递归
            for (Int32 t = 1; t < OB.Length; t++)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double MaxValue = DELTA[t - 1, 0] * A[0, j];
                    Int32 MaxValueIndex = 0;
                    for (Int32 i = 1; i < N; i++)
                    {
                        Double Value = DELTA[t - 1, i] * A[i, j];
                        if (Value > MaxValue)
                        {
                            MaxValue = Value;
                            MaxValueIndex = i;
                        }
                    }

                    DELTA[t, j] = MaxValue * B[j, OB[t]];
                    PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态
                }
            }

            // 3. 终止
            Int32[] Q = new Int32[OB.Length];   // 定义最佳路径
            Probability = DELTA[OB.Length - 1, 0];
            Q[OB.Length - 1] = 0;
            for (Int32 i = 1; i < N; i++)
            {
                if (DELTA[OB.Length - 1, i] > Probability)
                {
                    Probability = DELTA[OB.Length - 1, i];
                    Q[OB.Length - 1] = i;
                }
            }

            // 4. 路径回溯
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                Q[t] = PSI[t + 1, Q[t + 1]];
            }

            return Q;
        }

        /// <summary>
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>
        /// <returns>可能性最大的隐藏状态序列</returns>
        /// <remarks>使用对数运算，不输出中间结果</remarks>
        public Int32[] ViterbiLog(Int32[] OB, out Double Probability)
        {
            Double[,] DELTA;
            Int32[,] PSI;

            return ViterbiLog(OB, out DELTA, out PSI, out Probability);
        }

        /// <summary>
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="DELTA">输出中间结果：局部最大概率。结果为自然对数值</param>
        /// <param name="PSI">输出中间结果：反向指针指示最可能路径</param>
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>
        /// <returns>可能性最大的隐藏状态序列</returns> 
        /// <remarks>使用对数运算，且输出中间结果</remarks>
        public Int32[] ViterbiLog(Int32[] OB, out Double[,] DELTA, out Int32[,] PSI, out Double Probability)
        {
            DELTA = new Double[OB.Length, N];   // 局部概率
            PSI = new Int32[OB.Length, N];      // 反向指针

            // 0. 预处理
            Double[,] LogA = new Double[N, N];
            for (Int32 i = 0; i < N; i++)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    LogA[i, j] = Math.Log(A[i, j]);
                }
            }

            Double[,] LogBIOT = new Double[N, OB.Length];
            for (Int32 i = 0; i < N; i++)
            {
                for (Int32 t = 0; t < OB.Length; t++)
                {
                    LogBIOT[i, t] = Math.Log(B[i, OB[t]]);
                }
            }

            Double[] LogPI = new Double[N];
            for (Int32 i = 0; i < N; i++)
            {
                LogPI[i] = Math.Log(PI[i]);
            }

            // 1. 初始化
            for (Int32 j = 0; j < N; j++)
            {
                DELTA[0, j] = LogPI[j] + LogBIOT[j, 0];
            }

            // 2. 递归
            for (Int32 t = 1; t < OB.Length; t++)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double MaxValue = DELTA[t - 1, 0] + LogA[0, j];
                    Int32 MaxValueIndex = 0;
                    for (Int32 i = 1; i < N; i++)
                    {
                        Double Value = DELTA[t - 1, i] + LogA[i, j];
                        if (Value > MaxValue)
                        {
                            MaxValue = Value;
                            MaxValueIndex = i;
                        }
                    }

                    DELTA[t, j] = MaxValue + LogBIOT[j, t];
                    PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态
                }
            }

            // 3. 终止
            Int32[] Q = new Int32[OB.Length];   // 定义最佳路径
            Probability = DELTA[OB.Length - 1, 0];
            Q[OB.Length - 1] = 0;
            for (Int32 i = 1; i < N; i++)
            {
                if (DELTA[OB.Length - 1, i] > Probability)
                {
                    Probability = DELTA[OB.Length - 1, i];
                    Q[OB.Length - 1] = i;
                }
            }

            // 4. 路径回溯
            Probability = Math.Exp(Probability);
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                Q[t] = PSI[t + 1, Q[t + 1]];
            }

            return Q;
        }
    }
}
