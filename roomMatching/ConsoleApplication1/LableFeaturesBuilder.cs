using LibSVMsharp;
using LibSVMsharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
   public class LableFeaturesBuilder
    {
       public virtual IDictionary<string,double> Lables { get; protected set; }
       public virtual IDictionary<string, int> Features { get; protected set; }
       public LableFeaturesBuilder()
       {
           Lables = new Dictionary<string, double>();
           Features = new Dictionary<string, int>();
       }
       public string GetLable(double lable)
       {
           return Lables.FirstOrDefault(t => t.Value == lable).Key;
       }
       public string GetFeature(int feature)
       {
          return Features.FirstOrDefault(t => t.Value == feature).Key;
       }
       public void AddToProblem(SVMProblem problem, string lable, IEnumerable<KeyValuePair<string, double>> xValues)
        {
            var xx = CreateNodes(xValues);
            if (!Lables.ContainsKey(lable))
            {
                Lables.Add(lable, Lables.Count + 1);
            }
            problem.Add(xx, Lables[lable]);
        }

       public SVMNode[] CreateNodes(IEnumerable<KeyValuePair<string, double>> xValues)
        {
            var node = new List<SVMNode>();
            foreach (var xValue in xValues)
            {
                if (!Features.ContainsKey(xValue.Key))
                {
                    Features.Add(xValue.Key, Features.Count+1);
                }
                node.Add(new SVMNode { Index = Features[xValue.Key], Value = xValue.Value });
            }
            return node.ToArray();
        }
    }
}
