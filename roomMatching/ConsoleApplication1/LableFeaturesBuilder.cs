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
            var nodes = new List<SVMNode>();
            foreach (var xValue in xValues)
            {
                var node =CreateNode(xValue);
                nodes.Add(node);
                
                AddSPNode(nodes,xValue.Key,"大", xValue.Key.Where(t => t == '大').Count());
                AddSPNode(nodes,xValue.Key,"双", xValue.Key.Where(t => t == '双').Count());
                AddSPNode(nodes,xValue.Key,"套", xValue.Key.Where(t => t == '套').Count());
                
            }
            return nodes.ToArray();
        }
        protected void AddSPNode(IList<SVMNode> nodes, string xValue, string key, double value)
        {
            var node = CreateNode(new KeyValuePair<string, double>(key, value));
            if (xValue.IndexOf(key)!= -1)
            {
                nodes.Add(node);
            }
        }
        protected SVMNode CreateNode(KeyValuePair<string, double> xValue)
        {
            if (!Features.ContainsKey(xValue.Key))
            {
                Features.Add(xValue.Key, Features.Count+1);
            }
            var node=new SVMNode(Features[xValue.Key],xValue.Value);
            return node;
        }
        public virtual IEnumerable<int> CreateWeightFeatures(params string[] features)
        {
            var ls = new List<int>();
            foreach (var feature in features)
            {
                if(Features.ContainsKey(feature))
                {
                  ls.Add(Features[feature]);
                }
            }
            return ls;
        }
    }
}
