using PanGu;
using PanGu.Dict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class PanGuSegment  
    {
        private Segment segment = null;
        static PanGuSegment()
        {
            Segment.Init();
            string dictFile = "../../dict/dict.txt";
            ReloadWordDictionary(dictFile);
        }
        static BindingFlags BindingFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Default;
        protected static void ReloadWordDictionary(string dictFile)
        {
            var field = typeof(Segment).GetField("_WordDictionary", BindingFlags);
           var wordDictionary = field.GetValue(null) as WordDictionary;
           string v = string.Empty;
           wordDictionary.Load(dictFile,true,out v);
        }
        public PanGuSegment()
        {
            this.segment = new Segment();
        }
        public ICollection<WordInfo> ChineseSegemntWordInfo(string text)
        {
            ICollection<WordInfo> collection = this.segment.DoSegment(text);
            return collection;
        }
        public IEnumerable<string> ChineseSegemnt(string text)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(text))
            {
                var collection=ChineseSegemntWordInfo(text);
                foreach (WordInfo current in collection)
                {
                    list.Add(current.Word);
                }
            }
            return list;
        }
    }
}
