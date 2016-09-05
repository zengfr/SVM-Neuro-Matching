using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Service
{
	class Class1
	{
		[STAThread]
		static void MainT(string[] args)
		{
		    var src = @"F:\ToSummarize.txt";

            string str = System.IO.File.ReadAllText(src);

            string[] Lines = str.Split(new string[] { "\r\n", "\n","." }, StringSplitOptions.RemoveEmptyEntries);

            WordHandle stopword = new WordHandle();
           
            TFIDF tf = new TFIDF(Lines);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"F:\Summary.txt"))
            {
                foreach (var item in tf.DocResult)
                {
                     file.WriteLine(Lines[item]);
                }
            }
		}
	}
}