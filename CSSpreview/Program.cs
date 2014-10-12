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
		

		private static String CreateIndent(int depth) {
			StringBuilder sb = new StringBuilder();
			for (int i = 1; i < depth; i++) {
				sb.Append('\t');
			}
			return sb.ToString();
		}

		private static void CreateTreeNodes(string[] tmpsels, string tmpmed, ref TreeNode<CssTreeItem> tn) {
			string tmpt = HtmlAddons.GetTag(tmpsels[0]);
			string tmps = tmpsels[0];
			if (String.IsNullOrEmpty(tmpt)) {
				tmpt = "|";
				tmps = tmpt + tmps;
			}

			CssTreeItem cti = new CssTreeItem(tmps, tmpmed, tn.Data.FullSelector + ">" + tmps, tmpt);
			TreeNode<CssTreeItem> myfind = tn.FindTreeNode(node => node.Data != null && node.Data.FullSelector.Equals(cti.FullSelector));
									
			if (myfind == null) {
				tn.AddChild(cti);
			}
			
			string tmpiter = cti.FullSelector;
			for (int i = 1; i < tmpsels.Length; i++) {
				TreeNode<CssTreeItem> myfind2 = tn.FindTreeNode(node => node.Data != null && node.Data.FullSelector.Equals(tmpiter));
				tmpiter = tmpiter + ">" + tmpsels[i];
				TreeNode<CssTreeItem> myfind3 = myfind2.FindTreeNode(node => node.Data != null && node.Data.FullSelector.Equals(tmpiter));
				tmps = tmpsels[i];
				tmpt = HtmlAddons.GetTag(tmpsels[i]);
				
				if (String.IsNullOrEmpty(tmpt)) {
					tmpt = "|";
					tmps = tmpt + tmps;
				}

				if (myfind3 == null) {
					myfind2.AddChild(new CssTreeItem(tmps, tmpmed, tmpiter, tmpt));
				}			
			}			
		}
				
		private static void FixTreeNodes(ref TreeNode<CssTreeItem> tn) {			
			foreach (TreeNode<CssTreeItem> node in tn) {
				if (node.Parent != null) {
					node.Data.FullSelector = node.Parent.Data.FullSelector + ">" + node.Data.Selector;

					node.Data.Tag = HtmlAddons.GetTag(node.Data.Selector);
					node.Data.TagClass = HtmlAddons.GetClass(node.Data.Selector);
					node.Data.TagId = HtmlAddons.GetId(node.Data.Selector);									
				}
			}

			foreach (TreeNode<CssTreeItem> node in tn) {
				if (node.Parent != null) {
					if (node.Data.Tag.Equals("|")) {
						node.Data.Tag = "div";
						node.Data.FullSelector = node.Data.FullSelector.Replace("|", "div");
						node.Data.Selector = node.Data.Selector.Replace("|", "");
					}
					if (node.Data.FullSelector.Contains("|")) {						
						node.Data.FullSelector = node.Data.FullSelector.Replace("|", "div");
						node.Data.Selector = node.Data.Selector.Replace("|", "");
					}
				}
			}

		}
				
		private static string CrawlTree(string fselector, int lev, TreeNode<CssTreeItem> tn, Options options) {
			TreeNode<CssTreeItem> tmpnode = tn.FindTreeNode(node => node.Data != null && node.Data.FullSelector.Equals(fselector));
			string fsel = "";
			string txt = "";
			
			lev = lev + 1;
			foreach (TreeNode<CssTreeItem> mynode in tmpnode) {				
				if (mynode.Level == lev) {
					fsel = mynode.Data.FullSelector;
					//txt = txt + "<hr class=\"clear-csspreview\">";
					if (mynode.IsLeaf) {
						txt = txt + CreateIndent(mynode.Level) + HtmlAddons.GiveStartElem(mynode.Data.Tag, mynode.Data.TagId, mynode.Data.TagClass) + HtmlAddons.GiveTagContent(mynode.Data.Tag, options.InnerText, mynode.Data.Selector) + HtmlAddons.GiveEndElem(mynode.Data.Tag) + "\r\n";
					} else {
						txt = txt + CreateIndent(mynode.Level) + HtmlAddons.GiveStartElem(mynode.Data.Tag, mynode.Data.TagId, mynode.Data.TagClass) + "\r\n";
						txt = txt + CrawlTree(fsel, lev, tmpnode, options);
						txt = txt + CreateIndent(mynode.Level) + HtmlAddons.GiveEndElem(mynode.Data.Tag) + "\r\n";
					}
				}
			}			
			return txt;
		}
		
		static void Main(string[] args) {
			Options options = new Options(@"d:\code\csharp\Projects\_sample-files_\connect.css");
			
			String tmpcss = "";			
			
			try {
				tmpcss = File.ReadAllText(options.CssFileNameFull);

			} catch (FileNotFoundException e) {
				Console.WriteLine(e.Message);
			}
			
			if (!String.IsNullOrEmpty(tmpcss)) {				
				HtmlAddons.CleanUpCss(ref tmpcss);

				CssParser cp = new CssParser();
				IEnumerable<CssParserRule> rules = cp.ParseAll(tmpcss);
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
					string[] plussplit = tmpsel.Split('+');
					string[] tmpsels;

					if (tmpsel.Contains('+')) {						
						string[] splits = HtmlAddons.SplitSelectors("", tmpsel).Split('\\');

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

				string tmpstr = "<html>\r\n<head>\r\n<link rel=\"stylesheet\" href=\"" + options.CssFileName + "\">\r\n";
				//tmpstr = tmpstr + "<style type=\"text/css\">" + ".clear-csspreview:before,.clear-csspreview:after { content:\"\"; display:table; } .clear-csspreview:after { clear:both;} .clear-csspreview {margin:20px 0;  zoom:1;}"+ "</style>";
				//tmpstr = tmpstr + "<style type=\"text/css\">" + "div:before,div:after { content:\"\"; display:table; } div:after { clear:both;} div { zoom:1;}"+ "</style>";

				tmpstr = tmpstr + "</head>\r\n" + CrawlTree("html", 0, tn, options) + "\r\n</html>";
				try {
					File.WriteAllText(options.CssFolderName + options.HtmlFileName, tmpstr, Encoding.UTF8);

				} catch (Exception e) {
					Console.WriteLine(e.Message);
				}
				
				Console.WriteLine(tmpstr);				

			}
			Console.ReadKey();
		}		
	}
}
