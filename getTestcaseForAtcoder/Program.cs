using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using static System.Console;
using System.IO;

namespace getTestcaseForAtcoder
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Input Atcoder Problem URL : ");
            string testcases = getTestCase(getContext(ReadLine()));

            using (StreamWriter sw = new StreamWriter("ProgramTest.cs"))
            {
                sw.Write(testcases);
            }

            WriteLine("Output to \"ProgramTest.cs\"");
        }

        static string getTestCase(string html)
        {
            string anchor = "<pre class=\"prettyprint linenums\">(?<testcase>.*?)</pre>";

            Regex re = new Regex(anchor, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("");
            sb.AppendLine("namespace Atcoder.Tests");
            sb.AppendLine("{");
            sb.AppendLine("\t[TestClass()]");
            sb.AppendLine("\tpublic class ProgramTests");
            sb.AppendLine("\t{");

            int i = 1;
            for (Match m = re.Match(html); m.Success; m = m.NextMatch(), i++)
            {
                string testCase1 = m.Groups["testcase"].Value; m = m.NextMatch();
                string testCase2 = m.Groups["testcase"].Value;

                testCase1 = testCase1.TrimStart('\r');
                testCase1 = testCase1.TrimStart('\n');

                testCase2 = testCase2.TrimStart('\r');
                testCase2 = testCase2.TrimStart('\n');
                testCase2 = testCase2.TrimEnd('\n');
                testCase2 = testCase2.TrimEnd('\r');

                testCase1 = testCase1.Replace("\r\n", "\\n");
                testCase2 = testCase2.Replace("\r\n", "\\n");
                
                sb.AppendLine("\t\t[TestMethod()]");
                sb.AppendLine("\t\t[Timeout(2000)]");
                sb.AppendLine($"\t\tpublic void Test{i.ToString()}()");
                sb.AppendLine("\t\t{");
                sb.AppendLine($"\t\t\tstring input  = \"{testCase1}\";");
                sb.AppendLine($"\t\t\tstring output = \"{testCase2}\";");
                sb.AppendLine("");
                sb.AppendLine("\t\t\tAssertIO(input, output);");
                sb.AppendLine("\t\t}");
                sb.AppendLine("");
            }

            sb.AppendLine("\t\tprivate void AssertIO(string input, string output)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tStringReader reader = new StringReader(input);");
            sb.AppendLine("\t\t\tConsole.SetIn(reader);");
            sb.AppendLine("");
            sb.AppendLine("\t\t\tStringWriter writer = new StringWriter();");
            sb.AppendLine("\t\t\tConsole.SetOut(writer);");
            sb.AppendLine("");
            sb.AppendLine("\t\t\tProgram.Main(new string[0]);");
            sb.AppendLine("");
            sb.AppendLine("\t\t\tAssert.AreEqual(output + Environment.NewLine, writer.ToString());");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return  sb.ToString();
        }

        static string getContext(string url)
        {
            WebClient wc  = new WebClient();
            byte[]    data = wc.DownloadData(url);
            Encoding  enc = Encoding.GetEncoding("utf-8");
            string    ret = enc.GetString(data);
            wc.Dispose();
            return ret;
        }
    }
}
