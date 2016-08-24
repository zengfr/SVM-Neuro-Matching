using System;

namespace Splash
{
    partial class HMM
    {
        /// <summary>
        /// 后向算法：计算观察序列的概率
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <returns>观察序列的概率</returns>
        public Double Backward(Int32[] OB)
        {
            Double[,] BETA;     // 只声明，不定义

            return Backward(OB, out BETA);
        }

        /// <summary>
        /// 后向算法：计算观察序列的概率
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="BETA">中间结果</param>
        /// <returns>观察序列的概率</returns>
        public Double Backward(Int32[] OB, out Double[,] BETA)
        {
            BETA = new Double[OB.Length, N];

            // 初始化
            for (Int32 j = 0; j < N; j++)
            {
                BETA[OB.Length - 1, j] = 1.0;
            }

            // 归纳
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += A[j, i] * B[i, OB[t + 1]] * BETA[t + 1, i];
                    }

                    BETA[t, j] = Sum;
                }
            }

            // 终止
            Double Probability = 0;
            for (Int32 i = 0; i < N; i++)
            {
                Probability += BETA[0, i];
            }

            return Probability;
        }

        /// <summary>
        /// 带比例因子修正的后向算法
        /// </summary>
        /// <param name="OB">已知的观察序列</param>
        /// <param name="SCALE">用于修正的比例因子</param>
        /// <param name="BETA">中间结果：局部概率</param>
        private void BackwardWithScale(Int32[] OB, Double[] SCALE, ref Double[,] BETA)
        {
            if (BETA == null) BETA = new Double[OB.Length, N];

            // 初始化
            for (Int32 j = 0; j < N; j++)
            {
                BETA[OB.Length - 1, j] = 1.0 / SCALE[OB.Length - 1];
            }

            // 归纳
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += A[j, i] * B[i, OB[t + 1]] * BETA[t + 1, i];
                    }

                    BETA[t, j] = Sum / SCALE[t];
                }
            }
        }
    }
}
