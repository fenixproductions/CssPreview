using CssPreviewClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CSSpreview
{
	class Program
	{
		static void CleanUpCss(ref StringBuilder sb) {
			if (sb.Length > 0) {
				sb = sb.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");				
				//remove comments
				string mypat =@"/\*((?!\*/).)*\*/";
				string tmpstr = Regex.Replace(sb.ToString(), mypat, "", RegexOptions.Singleline);
								
				//remove charset and import
				mypat = "@(charset|import) .+?;";
				tmpstr = Regex.Replace(tmpstr, mypat, "", RegexOptions.Singleline);				
				sb.Clear();
				sb.Append(tmpstr);
			}
		}

		static void Main(string[] args)
		{			
			StringBuilder sb = new StringBuilder();
			String tmpcss = "";

			try {
				tmpcss = File.ReadAllText(@"d:\code\csharp\Projects\_sample-files_\test-query-mix.css");

			} catch (FileNotFoundException e) {
				Console.WriteLine(e.Message);
			}
			
			if (!String.IsNullOrEmpty(tmpcss)) {
				sb.Append(tmpcss);
				CleanUpCss(ref sb);

				CssParser cp = new CssParser();
				IEnumerable<CssParserRule> rules = cp.ParseAll(sb.ToString());
				CssHolder ch = new CssHolder();

				foreach (CssParserRule rule in rules) {
					Console.WriteLine(rule.Selectors.Count());
					foreach (string sel in rule.Selectors) {
						ch.HolderItems.Add(new CssHolderItem(sel, rule.Media));
					}
				}
				//TODO: sort items first!
				//ch.HolderItems.Add(new CssHolderItem(x,yield,));
				//var sortedRules = rules.OrderBy(r => r.Media);//.ThenBy(r => r.Selectors.ToString()); // cars.OrderBy(c => c.Model).ThenBy(c => c.Year);
				int z = 0;
			}			

			Console.Write(sb.ToString());
			Console.ReadKey();
		}		
	}
}
