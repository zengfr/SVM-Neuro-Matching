using System;

namespace Splash
{
    partial class HMM
    {
        /// <summary>
        /// 前向算法：计算观察序列的概率
        /// Forward Algorithm: Finding the probability of an observed sequence
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <returns>观察序列的概率</returns>
        /// <remarks>使用双精度运算，不输出中间结果</remarks>
        public Double Forward(Int32[] OB)
        {
            Double[,] ALPHA;    // 只声明，不定义

            return Forward(OB, out ALPHA);
        }

        /// <summary>
        /// 前向算法：计算观察序列的概率
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="ALPHA">输出中间结果：局部概率</param>
        /// <returns>观察序列的概率</returns>
        /// <remarks>使用双精度运算，输出中间结果</remarks>
        public Double Forward(Int32[] OB, out Double[,] ALPHA)
        {
            ALPHA = new Double[OB.Length, N];   // 局部概率

            // 1. 初始化：计算初始时刻所有状态的局部概率
            for (Int32 j = 0; j < N; j++)
            {
                ALPHA[0, j] = PI[j] * B[j, OB[0]];
            }

            // 2. 归纳：递归计算每个时间点的局部概率
            for (Int32 t = 1; t < OB.Length; t++)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += ALPHA[t - 1, i] * A[i, j];
                    }

                    ALPHA[t, j] = Sum * B[j, OB[t]];
                }
            }

            // 3. 终止：观察序列的概率等于最终时刻所有局部概率之和
            Double Probability = 0;
            for (Int32 i = 0; i < N; i++)
            {
                Probability += ALPHA[OB.Length - 1, i];
            }

            return Probability;
        }

        /// <summary>
        /// 带比例因子修正的前向算法：计算观察序列的概率
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="ALPHA">中间结果：局部概率</param>
        /// <param name="SCALE">中间结果：比例因子</param>
        /// <returns>观察序列的概率（自然对数值）</returns>
        private Double ForwardWithScale(Int32[] OB, ref Double[,] ALPHA, ref Double[] SCALE)
        {
            if (ALPHA == null) ALPHA = new Double[OB.Length, N];
            if (SCALE == null) SCALE = new Double[OB.Length];

            // 1. 初始化
            SCALE[0] = 0;
            for (Int32 j = 0; j < N; j++)
            {
                ALPHA[0, j] = PI[j] * B[j, OB[0]];
                SCALE[0] += ALPHA[0, j];
            }

            for (Int32 j = 0; j < N; j++)
            {
                ALPHA[0, j] /= SCALE[0];
            }

            // 2. 归纳
            for (Int32 t = 1; t < OB.Length; t++)
            {
                SCALE[t] = 0;
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += ALPHA[t - 1, i] * A[i, j];
                    }

                    ALPHA[t, j] = Sum * B[j, OB[t]];
                    SCALE[t] += ALPHA[t, j];
                }

                for (Int32 j = 0; j < N; j++)
                {
                    ALPHA[t, j] /= SCALE[t];
                }
            }

            // 3. 终止
            Double Probability = 0;
            for (Int32 t = 0; t < OB.Length; t++)
            {
                Probability += Math.Log(SCALE[t]);
            }

            return Probability;     // 自然对数值
        }
    }
}
