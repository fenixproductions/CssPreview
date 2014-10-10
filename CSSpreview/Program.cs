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
				//remove pseudoselectors
				//mypat = @"(:[^:]+?)+\s";
				//tmpstr = Regex.Replace(tmpstr, mypat, "", RegexOptions.Singleline);
				sb.Clear();
				sb.Append(tmpstr);
			}
		}

		private static String CreateIndent(int depth) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < depth; i++) {
				sb.Append('\t');
			}
			return sb.ToString();
		}

		private static void CreateTreeNodes(string[] tmpsels, string tmpmed, ref TreeNode<CssTreeItem> tn) {
			CssTreeItem cti = new CssTreeItem(tmpsels[0], tmpmed, tn.Data.FullSelector + ">" + tmpsels[0]);
			TreeNode<CssTreeItem> myfind = tn.FindTreeNode(node => node.Data != null && node.Data.FullSelector.Equals(cti.FullSelector));
									
			if (myfind == null) {
				tn.AddChild(cti);
			}
			
			string tmpiter = cti.FullSelector;
			for (int i = 1; i < tmpsels.Length; i++) {				
				TreeNode<CssTreeItem> myfind2 = tn.FindTreeNode(node => node.Data != null && node.Data.FullSelector.Equals(tmpiter));
				tmpiter = tmpiter + ">" + tmpsels[i];
				TreeNode<CssTreeItem> myfind3 = myfind2.FindTreeNode(node => node.Data != null && node.Data.FullSelector.Equals(tmpiter));

				if (myfind3 == null) {
					myfind2.AddChild(new CssTreeItem(tmpsels[i], tmpmed, tmpiter));
				}			
			}			
		}

		private static string GetTag(string str) {
			string tmptag = Regex.Replace(str, @"(\.[^.]+?)+$", "", RegexOptions.Multiline);
			tmptag = Regex.Replace(tmptag, @"(#[^#]+?)+$", "", RegexOptions.Multiline);

			if (!String.IsNullOrEmpty(tmptag)) {
				return tmptag;
			}

			return "div";
		}
		private static string GetClass(string str) {
			string tmpclass = Regex.Replace(str, @"([^.]*?)([.].*)?$", "$2", RegexOptions.Multiline);
			if (!String.IsNullOrEmpty(tmpclass)) {
				return String.Join(" ", tmpclass.Split('.')).ToString().Trim();
			}
			return "";
		}
		private static string GetId(string str) {
			string tmpid = Regex.Replace(str, @"([^#]*?)([#].*)?$", "$2", RegexOptions.Multiline);
			if (!String.IsNullOrEmpty(tmpid)) {
				return String.Join(" ", tmpid.Split('#')).ToString().Trim();
			}
			return "";
		}

		private static void FixTreeNodes(ref TreeNode<CssTreeItem> tn) {			
			foreach (TreeNode<CssTreeItem> node in tn) {
				if (node.Parent != null) {
					node.Data.FullSelector = node.Parent.Data.FullSelector + ">" + node.Data.Selector;

					node.Data.Selector = node.Data.Selector;
					node.Data.Tag = GetTag(node.Data.Selector);
					node.Data.TagClass = GetClass(node.Data.Selector);
					node.Data.TagId = GetId(node.Data.Selector);

					//if (stags.Length > 1) {					
					//	node.Data.Selector = stags[0];
					//	node.Data.Tag = GetTag(node.Data.Selector);
					//	node.Data.TagClass = GetClass(node.Data.Selector);
					//	node.Data.TagId = GetId(node.Data.Selector);						
					//	for (int j = 1; j < stags.Length; j++) {
					//		TreeNode<CssTreeItem> tmptn = node;
					//		tmptn.Data.Selector = stags[j];
					//		tmptn.Data.Tag = GetTag(tmptn.Data.Selector);
					//		tmptn.Data.TagClass = GetClass(tmptn.Data.Selector);
					//		tmptn.Data.TagId = GetId(tmptn.Data.Selector);
					//		node.Parent.AddChild(tmptn);
					//	}
					//}										
				}
			}
		}

		private static string SplitSelectors(string mybase, string tmp) {			
			//get first plus sign
			int plusind = tmp.IndexOf('+');
			//get substring
			string str1 = tmp.Substring(0, plusind);
			string str2 = tmp.Substring(plusind + 1);
			//get base
			string strbase = str1.Substring(0, str1.LastIndexOf('>') + 1);
			str1 = str1.Substring(str1.LastIndexOf('>') + 1);
			
			if (!String.IsNullOrEmpty(mybase)) {
				strbase = mybase + ">" + strbase;
			}
			string str1out = strbase + str1;
			string str2out = strbase + str2;
			string strout = str1out;

			if (str2.Contains('+')) {
				strout = strout + @"\" + SplitSelectors(strbase, str2);
			} else {
				strout = strout + @"\" + str2out;
			}
			strout = strout.Replace(">>", ">");

			return strout;
		}

		static void Main(string[] args) {			
			StringBuilder sb = new StringBuilder();
			String tmpcss = "";

			try {
				tmpcss = File.ReadAllText(@"d:\code\csharp\Projects\_sample-files_\test.css");

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
					foreach (string sel in rule.Selectors) {
						string tmpsel = sel.Replace(" > ", ">").Replace(" >", ">").Replace("> ", ">");
						tmpsel = tmpsel.Replace(" + ", "+").Replace(" +", "+").Replace("+ ", "+");
						tmpsel = tmpsel.Replace(" ", ">");
						//remove pseudoselectors						
						tmpsel = Regex.Replace(tmpsel, @"(::?[^:]+?)+$", "", RegexOptions.Multiline);
						//reove attributes
						tmpsel = Regex.Replace(tmpsel, @"(\[[^[]+?])+$", "", RegexOptions.Multiline);
						//Console.WriteLine(tmpsel);
						if (!ch.HasItem(tmpsel, rule.Media)) {
							ch.HolderItems.Add(new CssHolderItem(tmpsel, rule.Media));
						}						
					}
				}
				
				ch.SortIt();
			
				TreeNode<CssTreeItem> tn = new TreeNode<CssTreeItem>(new CssTreeItem("html", "", "html", "html"));
			
				for ( int x = 0; x < ch.HolderItems.Count; x++) {					
					string tmpmed = ch.HolderItems[x].MediaQuery;
					string tmpsel = ch.HolderItems[x].Selector;
					tmpsel = tmpsel.Trim();
					//Console.WriteLine(tmpsel);
					string[] plussplit = tmpsel.Split('+');
					string[] tmpsels;

					if (tmpsel.Contains('+')) {
						//Console.WriteLine("------");
						//Console.WriteLine(tmpsel);
						//Console.WriteLine("---");
						string[] splits = SplitSelectors("", tmpsel).Split('\\');

						foreach (string sp in splits) {
							tmpsels = sp.Split('>');
							CreateTreeNodes(tmpsels, tmpmed, ref tn);
						}

					} else {
						tmpsels = tmpsel.Split('>');
						CreateTreeNodes(tmpsels, tmpmed, ref tn);
					}
				}

				FixTreeNodes(ref tn);
				Console.WriteLine("-----------------------");
				foreach (TreeNode<CssTreeItem> node in tn) {
					string indent = CreateIndent(node.Level);
					Console.WriteLine(node.Data.Tag  + indent + (node.Data.Selector ?? "null"));
				}

			}						
			Console.ReadKey();
		}		
	}
}
