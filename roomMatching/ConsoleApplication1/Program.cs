using BeanboxSoftware.BeanMap;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using LibSVMsharp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    var segment=new PanGuSegment();
        //    var rooms1 = File.ReadAllText("../../room1.txt");
        //    var rooms2 = File.ReadAllText("../../room2.txt");

        //    var words1 = segment.ChineseSegemnt(rooms1);
        //    var words2 = segment.ChineseSegemnt(rooms2);

        //    foreach (var word in words1)
        //    {
        //        Console.WriteLine("{0}",word);
        //    }
        //    Console.ReadLine();
        //}
        static void Main(string[] args)
        {
            wordsSvm();
            //Console.ReadLine();

            Console.WindowWidth = 188;
            Console.WindowHeight = 33;
            var rooms11 = File.ReadAllText("../../room7.txt");
            var rooms22 = File.ReadAllText("../../room8.txt");

            var termFreqWeight = new  PanGuTermFreqWeight();

            var rooms1=rooms11.Split(';');
            var rooms2 = rooms22.Split(';');
            termFreqWeight.computerFW(rooms1, rooms2);
            var vv = termFreqWeight.computerSimilar();
            //foreach (var v in vv)
            //{
            //    if (v.Value > 0)
            //    {
            //        Console.WriteLine("{0},{1},{2}", v.Key.Item1, v.Key.Item2, v.Value);
            //    }
            //}

            var vv2 = vv.GroupBy(t => t.Key);
            foreach (var v2 in vv2)
            {
                var v22=v2.SelectMany(t=>t.Value).OrderByDescending(t=>t.Value);
                foreach (var item in v22)
                {
                    if (item.Value > 0)
                    {
                        Console.WriteLine("{0},{1},{2}", v2.Key, item.Key, item.Value);
                    }
                }
                Console.WriteLine("".PadLeft(40,'-'));
            }
            Console.ReadLine();
        }

        static void wordsSvm()
        {//http://www.svm-tutorial.com/2014/10/svm-tutorial-classify-text-csharp/

            
            var rooms11 = File.ReadAllText("../../room7.txt");
            var rooms22 = File.ReadAllText("../../room8.txt");

            var rooms1 = rooms11.Split(';');
            var rooms2 = rooms22.Split(';');

            var termFreqWeight = new PanGuTermFreqWeight();
            termFreqWeight.computerFW(rooms1, rooms2);
            //var maxFraq = termFreqWeight.TermFWByGlobal.Values.Max(t=>t.Freq);
           
            var problem = new SVMProblem();

            var lableFeaturesBuilder = new LableFeaturesBuilder();
            var segment = new PanGuSegment();
            foreach (var room in rooms2)
            {
                var words = termFreqWeight.TermsByDoc[1, room];
                words = words.Where(t => t.Length > 1 && termFreqWeight.TermFWByGlobal.ContainsKey(t) && termFreqWeight.TermFWByType.ContainsKey(0, t)).ToArray();
                lableFeaturesBuilder.AddToProblem(problem, room, words.Select(t => new KeyValuePair<string, double>(t, termFreqWeight.TermFWByDoc[1, room, t].Freq)));
            }

            SVMParameter parameter=new SVMParameter();
            parameter.Type=SVMType.C_SVC;
            parameter.Kernel=SVMKernelType.LINEAR;
            parameter.C=1;

            problem = problem.Normalize(SVMNormType.L1);
            var model= SVM.Train(problem, parameter);
           
            foreach (var room in rooms1)
            {
                var words = termFreqWeight.TermsByDoc[0,room];
                words = words.Where(t => t.Length > 1 && termFreqWeight.TermFWByGlobal.ContainsKey(t) && termFreqWeight.TermFWByType.ContainsKey(1, t)).ToArray();

                var nodes = lableFeaturesBuilder.CreateNodes(words.Select(t => new KeyValuePair<string, double>(t, termFreqWeight.TermFWByDoc[0,room,t].Freq)));
                nodes = nodes.Normalize(SVMNormType.L1);
                var predictedY = nodes.Predict(model);

                double[] values = null;
                var v = nodes.PredictValues(model, out values);

                double[] est = null; double probability = 0;
                probability = nodes.PredictProbability(model, out est);

                Console.WriteLine("{0,22}\t{1},{2},{3}", room, "" + lableFeaturesBuilder.GetLable(predictedY), v, probability);
            }

            Console.WriteLine(new string('=', 50));
        }
    

    }
    }

