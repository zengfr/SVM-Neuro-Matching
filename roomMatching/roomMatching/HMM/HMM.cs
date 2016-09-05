/* ----------------------------------------------------------
文件名称：HMM.cs

作者：秦建辉

MSN：splashcn@msn.com
QQ：36748897

版本历史：
    V1.1    2011年06月09日
            修正UMDHMM在Baum-Welch算法中存在的模型参数调整错误。

    V1.0	2011年06月08日
			将C语言实现的隐马尔科夫模型算法（UMDHMM）改为C#语言实现。

功能描述：
	1.前向算法（forward algorithm）：给定HMM求一个观察序列的概率（评估）
    2.后向算法（backward algorithm）：给定HMM求一个观察序列的概率（评估）
    3.前向-后向算法（forward-backward algorithm）：根据观察序列生成隐马尔科夫模型（学习）
    4.维特比算法（Viterbi algorithm）：搜索最有可能生成一个观察序列的隐藏状态序列（解码）

参考资料：
    C代码：http://www.kanungo.com/software/umdhmm-v1.02.zip
    学习资料：http://www.52nlp.cn/category/hidden-markov-model
 ------------------------------------------------------------ */
using System;

namespace Splash
{
    public partial class HMM
    {
        /// <summary>
        /// 隐藏状态数目 N
        /// </summary>
        public readonly Int32 N;

        /// <summary>
        /// 观察符号数目 M
        /// </summary>
        public readonly Int32 M;

        /// <summary>
        /// 状态转移矩阵 A
        /// </summary>
        public Double[,] A;

        /// <summary>
        /// 混淆矩阵（confusion matrix）B
        /// </summary>
        public Double[,] B;

        /// <summary>
        /// 初始概率向量 PI
        /// </summary>
        public Double[] PI;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="StatesNum">隐藏状态数目</param>
        /// <param name="ObservationSymbolsNum">观察符号数目</param>
        public HMM(Int32 StatesNum, Int32 ObservationSymbolsNum)
        {
            N = StatesNum;              // 隐藏状态数目
            M = ObservationSymbolsNum;  // 观察符号数目

            A = new Double[N, N];   // 状态转移矩阵
            B = new Double[N, M];   // 混淆矩阵 
            PI = new Double[N];     // 初始概率向量
        }
    }
}
