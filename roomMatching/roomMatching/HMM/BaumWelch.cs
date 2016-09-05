using System;

namespace Splash
{
    partial class HMM
    {
        /// <summary>
        /// 前向-后向算法，用于参数学习
        /// Forward-backward algorithm: Generating a HMM from a sequence of obersvations
        /// </summary>
        /// <param name="OB">已知的观察序列</param>        
        /// <param name="LogProbInit">初始自然对数概率</param>
        /// <param name="LogProbFinal">最终自然对数概率</param>
        /// <param name="ExitError">迭代中允许的自然对数概率误差，缺省0.001</param>
        /// <param name="MSP">状态概率最小值，缺省0.001</param>
        /// <param name="MOP">观察概率最小值，缺省0.001</param>
        /// <returns>迭代次数</returns>
        /// <remarks>修正UMDHMM在模型参数调整中的错误</remarks>        
        public Int32 BaumWelch(Int32[] OB, out Double LogProbInit, out Double LogProbFinal,
            Double ExitError = 0.001, Double MSP = 0.001, Double MOP = 0.001)
        {
            Double[,] ALPHA = null;
            Double[,] BETA = null;
            Double[] SCALE = null;
            Double[,] GAMMA = null;
            Double[, ,] XI = null;

            Double LogProbForward = LogProbInit = ForwardWithScale(OB, ref ALPHA, ref SCALE); // 前向算法
            BackwardWithScale(OB, SCALE, ref BETA); // 后向算法
            ComputeGamma(ALPHA, BETA, ref GAMMA);   // 求解各时刻位于各隐藏状态的概率矩阵
            ComputeXI(OB, ALPHA, BETA, ref XI);     // 求解各时刻位于各隐藏状态及下一时刻位于各隐藏状态的关联概率矩阵

            Int32 Iterations;
            Double LogProbPrev = LogProbForward;
            for (Iterations = 1; ; Iterations++)
            {   // 重新估计初始概率向量
                for (Int32 i = 0; i < N; i++)
                {   // 注意：此处修正UMDHMM错误，以保证概率总和为1
                    PI[i] = MSP + (1 - MSP * N) * GAMMA[0, i];
                }

                for (Int32 i = 0; i < N; i++)
                {   // 重新估计状态转移矩阵
                    Double DenominatorA = 0;
                    for (Int32 t = 0; t < OB.Length - 1; t++)
                        DenominatorA += GAMMA[t, i];

                    for (Int32 j = 0; j < N; j++)
                    {
                        Double NumeratorA = 0;
                        for (Int32 t = 0; t < OB.Length - 1; t++)
                            NumeratorA += XI[t, i, j];

                        // 注意：此处修正UMDHMM错误，以保证概率总和为1
                        A[i, j] = MSP + (1 - MSP * N) * NumeratorA / DenominatorA;
                    }

                    // 重新估计混淆矩阵
                    Double DenominatorB = DenominatorA + GAMMA[OB.Length - 1, i];
                    for (Int32 k = 0; k < M; k++)
                    {
                        Double NumeratorB = 0;
                        for (Int32 t = 0; t < OB.Length; t++)
                        {
                            if (OB[t] == k) NumeratorB += GAMMA[t, i];
                        }

                        // 注意：此处修正UMDHMM错误，以保证概率总和为1
                        B[i, k] = MOP + (1 - MOP * M) * NumeratorB / DenominatorB;
                    }
                } // End for i

                // 计算概率差，决定是否停止迭代
                LogProbForward = ForwardWithScale(OB, ref ALPHA, ref SCALE);
                if (LogProbForward - LogProbPrev <= ExitError) break;

                BackwardWithScale(OB, SCALE, ref BETA);
                ComputeGamma(ALPHA, BETA, ref GAMMA);
                ComputeXI(OB, ALPHA, BETA, ref XI);
                LogProbPrev = LogProbForward;
            } // End for Iterations

            LogProbFinal = LogProbForward;  // 最终概率

            // 返回迭代次数
            return Iterations;
        }

        /// <summary>
        /// 求解t时刻位于隐藏状态Si的概率矩阵
        /// </summary>
        /// <param name="ALPHA">前向算法局部概率</param>
        /// <param name="BETA">后向算法局部概率</param>
        /// <param name="GAMMA">输出：各时刻位于各隐藏状态的概率矩阵</param>
        private void ComputeGamma(Double[,] ALPHA, Double[,] BETA, ref Double[,] GAMMA)
        {
            Int32 T = ALPHA.GetLength(0);
            if (GAMMA == null) GAMMA = new Double[T, N];

            for (Int32 t = 0; t < T; t++)
            {
                Double Denominator = 0;
                for (Int32 i = 0; i < N; i++)
                {
                    GAMMA[t, i] = ALPHA[t, i] * BETA[t, i];
                    Denominator += GAMMA[t, i];
                }

                for (Int32 i = 0; i < N; i++)
                {
                    GAMMA[t, i] /= Denominator; // 保证各时刻的概率总和等于1
                }
            }
        }

        /// <summary>
        /// 求解t时刻位于隐藏状态Si及t+1时刻位于隐藏状态Sj的概率矩阵
        /// </summary>
        /// <param name="OB">观察序列</param>
        /// <param name="ALPHA">前向算法局部概率</param>
        /// <param name="BETA">后向算法局部概率</param>
        /// <param name="XI">输出：求解各时刻位于各隐藏状态及下一时刻位于各隐藏状态的关联概率矩阵</param>
        private void ComputeXI(Int32[] OB, Double[,] ALPHA, Double[,] BETA, ref Double[, ,] XI)
        {
            Int32 T = OB.Length;
            if (XI == null) XI = new Double[T, N, N];

            for (Int32 t = 0; t < T - 1; t++)
            {
                Double Sum = 0;
                for (Int32 i = 0; i < N; i++)
                {
                    for (Int32 j = 0; j < N; j++)
                    {
                        XI[t, i, j] = ALPHA[t, i] * A[i, j] * B[j, OB[t + 1]] * BETA[t + 1, j];
                        Sum += XI[t, i, j];
                    }
                }

                // 保证各时刻的概率总和等于1
                for (Int32 i = 0; i < N; i++)
                    for (Int32 j = 0; j < N; j++)
                        XI[t, i, j] /= Sum;
            }
        }
    }
}
