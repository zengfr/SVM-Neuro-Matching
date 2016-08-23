using AForge.Neuro;
using AForge.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class A
    {
        public static void cc()
        {
            //整理输入输出数据 
            double[][] input = new double[4][]; 
            double[][] output = new double[4][];
            input[0] = new double[] { 0, 0 }; output[0] = new double[] { 0 };
            input[1] = new double[] { 0, 1 }; output[1] = new double[] { 0 };
            input[2] = new double[] { 1, 0 }; output[2] = new double[] { 0 };
            input[3] = new double[] { 1, 1 }; output[3] = new double[] { 1 };

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("input{0}:  ===>  {1},{2}  output{0}:  ===>  {3}", i, input[i][0], input[i][1], output[i][0]);
            }

            //建立网络，层数1，输入2，输出1，激励函数阈函数 
            ActivationNetwork network = new ActivationNetwork(new ThresholdFunction(), 2, 1);

            //学习方法为感知器学习算法 
            PerceptronLearning teacher = new PerceptronLearning(network);

            //定义绝对误差 
            double error = 1.0;
            Console.WriteLine();
            Console.WriteLine("learning error  ===>  {0}", error);

            //输出学习速率 
            Console.WriteLine();
            Console.WriteLine("learning rate ===>  {0}", teacher.LearningRate);

            //迭代次数 
            int iterations = 0;
            Console.WriteLine();
            while (error > 0.001)
            {
                error = teacher.RunEpoch(input, output);
                Console.WriteLine("learning error  ===>  {0}", error);
                iterations++;
            }
            Console.WriteLine("iterations  ===>  {0}", iterations);
            Console.WriteLine();
            Console.WriteLine("sim:");

            //模拟 
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("input{0}:  ===>  {1},{2}  sim{0}:  ===>  {3}", i, input[i][0], input[i][1], network.Compute(input[i])[0]);
            }


        }
    }
}
