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
                Lables.Add(lable, Lables.Count + 100+1);
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
            }
            nodes.AddRange(CreateSpecialNodes(xValues));
            return nodes.ToArray();
        }
        protected IEnumerable<SVMNode> CreateSpecialNodes(IEnumerable<KeyValuePair<string, double>> xValues)
        {
            var nodes = new List<SVMNode>();

            var nodesDict = new Dictionary<string,SVMNode>();
            foreach (var xValue in xValues)
            {
                AddSpecialNode(nodesDict, xValue.Key, "大", xValue.Key.Where(t => t == '大').Count());
                AddSpecialNode(nodesDict, xValue.Key, "双", xValue.Key.Where(t => t == '双').Count());
                AddSpecialNode(nodesDict, xValue.Key, "套", xValue.Key.Where(t => t == '套').Count());
            }
            //Parallel.ForEach(nodesDict, (kv) =>
            //{
            //    if (kv.Value.Value <= 0) kv.Value.Value = 0;//-1
            //});
            nodes.AddRange(nodesDict.Values.Where(t=>t.Value>0));
            return nodes;
        }
        protected void AddSpecialNode(IDictionary<string,SVMNode> nodesDict, string xValue, string key, double value)
        {
            if (xValue != key && !key.Equals(xValue))
            {
                if (!nodesDict.ContainsKey(key))
                {
                    var node = CreateNode(new KeyValuePair<string, double>(key, 0));
                    nodesDict.Add(key, node);
                }
                if (xValue.IndexOf(key) != -1)
                {
                    nodesDict[key].Value += value;
                }
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
            var fs = new List<int>();
            foreach (var feature in features)
            {
                if(Features.ContainsKey(feature))
                {
                  fs.Add(Features[feature]);
                }
            }
            return fs;
        }
        public virtual IEnumerable<T> CreateWeightLables<T>(params string[] lables) where T:IConvertible
        {
            return CreateWeightLables(lables).Select(t=>(T)Convert.ChangeType(t,typeof(T)));
        }
        public virtual IEnumerable<double> CreateWeightLables(params string[] lables)
        {
            var ls = new List<double>();
            foreach (var lable in lables)
            {
                if (Lables.ContainsKey(lable))
                {
                    ls.Add(Lables[lable]);
                }
            }
            return ls;
        }
    }
}
