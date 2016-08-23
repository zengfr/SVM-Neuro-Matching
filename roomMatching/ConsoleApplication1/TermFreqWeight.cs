using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanboxSoftware.BeanMap;
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
    public class PanGuTermFreqWeight : AbstractTermFreqWeight
    {
        protected virtual PanGuSegment Segment { get; set; }
        public PanGuTermFreqWeight()
        {
            Segment = new PanGuSegment();
        }
        protected override string[] Segemnt(string doc)
        {
            doc = relpace(doc);
            var terms = Segment.ChineseSegemnt(doc);
            return terms.ToArray();

        }

    }
    public class TermFreqWeight : AbstractTermFreqWeight
    {
        public TermFreqWeight()
        {

        }
        protected override string[] Segemnt(string doc)
        {
            doc = relpace(doc);
            var terms = doc.ToCharArray().Select(t => t.ToString());
            return terms.ToArray();

        }
    }
    public abstract class AbstractTermFreqWeight
    {

        public virtual IMap<string, TermFW> TermFWByGlobal { get; set; }
        public virtual IMap<int, string, TermFW> TermFWByType { get; set; }
        public virtual IMap<int, string, string, TermFW> TermFWByDoc { get; set; }
        public virtual IMap<int, string, string[]> TermsByDoc { get; set; }
        public AbstractTermFreqWeight()
        {
            Init();
        }
        public virtual void Init()
        {

            TermFWByGlobal = new Map<string, TermFW>();
            TermFWByType = new Map<int, string, TermFW>();
            TermFWByDoc = new Map<int, string, string, TermFW>();
            TermsByDoc = new Map<int, string, string[]>();
        }
        // public string GetTermInfo(int type,string term)
        //{
        //string.Format("", term, TermFWByGlobal[term], TermFWByType[type][term], TermFWByType[type][term]);
        //}
        #region computersimilar
        public IMap<string, string, double> computerSimilar()
        {
            IMap<string, string, double> similars = new Map<string, string, double>();
            double similar;
            foreach (var doc1 in TermsByDoc[0].Keys)
            {
                foreach (var doc2 in TermsByDoc[1].Keys)
                {
                    similar = computerSimilar(doc1, doc2);

                    similars[doc1].Add(doc2, similar);
                }
            }
            return similars;
        }
        /// <summary>
        /// 2文档中的相似度
        /// </summary>
        /// <param name="doc1"></param>
        /// <param name="doc2"></param>
        /// <returns></returns>
        private double computerSimilar(string doc1, string doc2)
        {
            var terms1 = TermsByDoc[0,doc1];
            var terms2 = TermsByDoc[1,doc2];

            var terms = terms1.Union(terms2).ToArray();

            //var tCount1 = TermsByDoc[0][doc1].Length;
            //var tCount2 = TermsByDoc[1][doc2].Length;

            double f, f1, f2;
            f = 0; f1 = 0; f2 = 0;
            foreach (var term in terms)
            {
                double v1 = 0;
                double v2 = 0;
                if (TermFWByGlobal[term].Freq > 1)
                {
                    if (TermFWByType.ContainsKey(0, term) && TermFWByType.ContainsKey(1, term))
                    {
                        v1 = GetTermFreq(doc1, doc2, 0, doc1, term) * GetTermWeight(doc1, doc2, 0, doc1, term);
                        v2 = GetTermFreq(doc1, doc2, 1, doc2, term) * GetTermWeight(doc1, doc2, 1, doc2, term);
                    }
                }
                f += v1 * v2;
                f1 += v1 * v1;
                f2 += v2 * v2;
            }
            double similarity = -1;
            if (f1 * f2 > 0)
            {
                similarity = f / (Math.Sqrt(f1 * f2));
            }
            return similarity;
        }
        protected int GetDocTermsTrueCount(string doc1, string doc2)
        {
            var count = 0;
            var terms1 = TermsByDoc[0,doc1];
            var terms2 = TermsByDoc[1,doc2];
            var terms = terms1.Union(terms2);
            foreach (var term in terms)
            {
                if (TermFWByGlobal[term].Freq > 1)
                {
                    if (TermFWByType.ContainsKey(0, term))
                    {
                        count += 1;
                    }
                    if (TermFWByType.ContainsKey(1, term))
                    {
                        count += 1;
                    }
                }
            }
            return count;
        }
        protected double GetTermFreq(string doc1, string doc2, int type, string doc, string term)
        {
            double freq = 0;
            if (TermFWByDoc.ContainsKey(type, doc, term))
            {
                freq = TermFWByDoc[type,doc,term].Freq;/// (double)(TermsByDoc[0][doc1].Length + TermsByDoc[1][doc2].Length);
            }
            return freq;
        }
        protected double GetTermWeight(string doc1, string doc2, int type, string doc, string term)
        {
            double weight = 1.0;
            var max = Math.Max(TermFWByType[1, term].Freq, TermFWByType[0, term].Freq);
            var min = Math.Min(TermFWByType[1, term].Freq, TermFWByType[0, term].Freq);
            weight = min / max / (TermsByDoc[0,doc1].Length + TermsByDoc[1,doc2].Length);// (GetDocTermsTrueCount(doc1,doc2));// TermFWByGlobal[term].Freq;
            //weight = max / TermFWByGlobal[term].Freq;
            //switch (type)
            //{
            //    case 0:
            //        weight =  TermFWByType[1, term].Freq;// / TermFWByType[0, term].Freq;
            //        break;
            //    case 1:
            //        weight = TermFWByType[0, term].Freq;/// TermFWByType[1, term].Freq;
            //        break;
            //}
            //Console.WriteLine("{0},{1},{2},{3}",type,  doc,  term, weight);
            //if (term.Length>=1&&term.IndexOfAny(new char[]{'大','双'}) != -1)
            //{
            //    weight = weight * 10.0;
            //}
            if (term.Length > 1 && term.IndexOfAny(new char[] { '床' }) != -1)
            {
                weight = weight * 100.0;
            }
            return weight;
        }







        //            本次所用到的相似度计算公式是 相似度=Kq*q/(Kq*q+Kr*r+Ks*s) (Kq > 0 , Kr>=0,Ka>=0)
        //其中，q是字符串1和字符串2中都存在的单词的总数，s是字符串1中存在，字符串2中不存在的单词总数，r是字符串2中存在，字符串1中不存在的单词总数. Kq,Kr和ka分别是q,r,s的权重，根据实际的计算情况，我们设Kq=2，Kr=Ks=1.
        //public static decimal GetSimilarityWith(string sourceString, string str)
        // {

        // decimal Kq = 2;
        // decimal Kr = 1;
        // decimal Ks = 1;

        // char[] ss = sourceString.ToCharArray();
        // char[] st = str.ToCharArray();

        // //获取交集数量
        // int q = ss.Intersect(st).Count();
        // int s = ss.Length-q;
        // int r = st.Length-q;

        // return Kq * q / (Kq * q + Kr * r + Ks * s);
        // }
        #endregion
        #region computerFW
        public void computerFW(string[] docs1, string[] docs2)
        {
            string[][] terms1 = Segemnt(docs1);
            string[][] terms2 = Segemnt(docs2);
            int index = 0;
            foreach (var terms in terms1)
            {
                AddTerm(0, docs1[index++], terms);
            }
            index = 0;
            foreach (var terms in terms2)
            {
                AddTerm(1, docs2[index++], terms);
            }
        }
        protected void AddTerm(int docType, string doc, string[] terms)
        {
            TermsByDoc[docType, doc] = terms;
            foreach (var term in terms)
            {
                AddTerm(docType, doc, term);
            }
        }
        protected void AddTerm(int docType, string doc, string term)
        {
            if (!TermFWByGlobal.ContainsKey(term))
            {
                TermFWByGlobal.Add(term, new TermFW(term));
            }
            if (!TermFWByType.ContainsKey(docType,term))
            {
                TermFWByType[docType].Add(term, new TermFW(term));
            }
            if (!TermFWByDoc.ContainsKey(docType,doc,term))
            {
                TermFWByDoc[docType][doc].Add(term, new TermFW(term));
            }

            TermFWByGlobal[term].Freq += 1;
            TermFWByType[docType][term].Freq += 1;
            TermFWByDoc[docType][doc][term].Freq += 1;
        }
        protected string[][] Segemnt(string[] docs)
        {
            var terms = new string[docs.Length][];
            for (int index = 0; index < docs.Length; index++)
            {
                terms[index] = Segemnt(docs[index]);
            }
            return terms;
        }
        protected abstract string[] Segemnt(string doc);

        public static string relpace(string str)
        {
            return Regex.Replace(str, @"[\p{P}\p{S}\s]", "").Replace("间", "房").Replace("房", "");
        }
        #endregion
    }
    public class TermFW
    {
        public virtual string Term { get; set; }
        public virtual double Freq { get; set; }
        public virtual decimal Weight { get; set; }
        public TermFW(string term)
        {
            Term = term;
        }

    }
}
