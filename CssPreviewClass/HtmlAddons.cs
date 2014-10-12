using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CssPreviewClass {
	public class HtmlAddons {
		/// <summary>
		/// Dictionary for tags' additional attributes
		/// </summary>
		public static Dictionary<String, String> tagsAddons;

		/// <summary>
		/// Dictionary for tags' contents
		/// </summary>
		public static Dictionary<String, String> tagsContents;

		/// <summary>
		/// Dictionary for tags' endings
		/// </summary>
		public static Dictionary<String, String> tagsEndings;

		/// <summary>
		/// Initialize Dictionary for tags additions (ex. href for <a href> link)
		/// </summary>
		private static void InitializeTagsAddons() {
			tagsAddons = new Dictionary<string, string>();			
			tagsAddons.Add("a", " href=\"http://www.google.com\"");
			tagsAddons.Add("img", " src=\"http://lorempixel.com/24/24" + "\" alt=\"sample image\"");
		}

		/// <summary>
		/// Initialize Dictionary for tags contents (ex. list elements)
		/// </summary>
		private static void InitializeTagsContents() {
			tagsContents = new Dictionary<string, string>();
			tagsContents.Add("ul", "<li>list 1</li><li>list2</li>");
			tagsContents.Add("ol", "<li>list 1</li><li>list2</li>");
			tagsContents.Add("img", "");
			tagsContents.Add("br", "");
			tagsContents.Add("hr", "");
		}

		/// <summary>
		/// Initialize Dictionary for tags cendings (ex. for clearing self-closing tags)
		/// </summary>
		private static void InitializeTagsEndings() {
			tagsEndings = new Dictionary<string, string>();
			tagsEndings.Add("img", "");
			tagsEndings.Add("br", "");
			tagsEndings.Add("hr", "");
		}

		/// <summary>
		/// Initialize htmlAddons class
		/// </summary>
		public HtmlAddons() {
			InitializeTagsAddons();
			InitializeTagsContents();
			InitializeTagsEndings();
		}
		
		/// <summary>
		/// Gives additional tag attributes
		/// </summary>
		/// <param name="tag">tag to check in Dictionary</param>
		/// <returns>additional attributes</returns>
		public static String GiveTagAddon(string tag) {
			if (tagsAddons == null) {
				InitializeTagsAddons();
			}
			string tmpstr = "";
			if (tagsAddons.TryGetValue(tag, out tmpstr)) {
				return tmpstr;
			}
			return "";		
		}

		/// <summary>
		/// Gives content for tag
		/// </summary>
		/// <param name="tag">tag to check in Dictionary</param>
		/// <param name="innertxt">inner text from options</param>
		/// <param name="selector">selector is used by default</param>
		/// <returns>tag's content</returns>
		public static String GiveTagContent(string tag, string innertxt, string selector) {			
			if (tagsContents == null) {
				InitializeTagsContents();
			}
			string tmpstr = "";
			bool z = tagsContents.TryGetValue(tag, out tmpstr);
			
			if ( z ) {
				return tmpstr;
			} else if (!String.IsNullOrEmpty(innertxt)) {
				tmpstr = innertxt;
			} else {
				tmpstr = selector;
			}
			
			return tmpstr;
		}

		/// <summary>
		/// Gives tag's ending
		/// </summary>
		/// <param name="tag">tag to check in Dictionary</param>
		/// <returns>ending tag</returns>
		public static String GiveTagEnding(string tag) {
			string tmpstr = "</" +  tag + ">";
			
			if (tagsEndings == null) {
				InitializeTagsEndings();
			}
			
			bool z = tagsEndings.TryGetValue(tag, out tmpstr);
			
			if (z) {
				return tmpstr;
			}
			tmpstr = "</" +  tag + ">";
			return tmpstr;			
		}

		/// <summary>
		/// checks for tag
		/// </summary>
		/// <param name="str">selector to check</param>
		/// <returns>tag found in selector</returns>
		public static string GetTag(string str) {
			string tmptag = Regex.Replace(str, @"(\.[^.]+?)+$", "", RegexOptions.Multiline);
			tmptag = Regex.Replace(tmptag, @"(#[^#]+?)+$", "", RegexOptions.Multiline);

			if (!String.IsNullOrEmpty(tmptag)) {
				return tmptag;
			}

			return "";
		}

		/// <summary>
		/// checks for tag's class
		/// </summary>
		/// <param name="str">selector to check</param>
		/// <returns>tag's class found in selector</returns>
		public static string GetClass(string str) {
			string tmpclass = Regex.Replace(str, @"([^.]*?)([.].*)?$", "$2", RegexOptions.Multiline);
			if (!String.IsNullOrEmpty(tmpclass)) {
				return String.Join(" ", tmpclass.Split('.')).ToString().Trim();
			}
			return "";
		}

		/// <summary>
		/// checks for tag's id
		/// </summary>
		/// <param name="str">selector to check</param>
		/// <returns>tag's id found in selector</returns>
		public static string GetId(string str) {
			string tmpid = Regex.Replace(str, @"([^#]*?)([#].*)?$", "$2", RegexOptions.Multiline);
			if (!String.IsNullOrEmpty(tmpid)) {
				return String.Join(" ", tmpid.Split('#')).ToString().Trim();
			}
			return "";
		}

		/// <summary>
		/// removes unnecessary and unhandled CSS elements
		/// </summary>
		/// <param name="sb">CSS text to clean</param>
		public static void CleanUpCss(ref String sb) {
			if (sb.Length > 0) {
				sb = sb.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
				//remove comments
				string mypat =@"/\*((?!\*/).)*\*/";
				string tmpstr = Regex.Replace(sb.ToString(), mypat, "", RegexOptions.Singleline);

				//remove charset and import
				mypat = "@(charset|import) .+?;";
				tmpstr = Regex.Replace(tmpstr, mypat, "", RegexOptions.Singleline);
				//remove viewport
				mypat = @"@viewport\s?{.+?}";
				tmpstr = Regex.Replace(tmpstr, mypat, "", RegexOptions.Singleline);
				//sb.Clear();
				//sb.Append(tmpstr);
			}
		}

		/// <summary>
		/// splits and dupliactes selectors with plus sign (ex. from div>h2+p we get div>h2 and div>p )
		/// </summary>
		/// <param name="mybase">starting part of base selector</param>
		/// <param name="tmp">selector for split</param>
		/// <returns>split and duplicated selectors for further use</returns>
		public static string SplitSelectors(string mybase, string tmp) {
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

		/// <summary>
		/// combines starting tag
		/// </summary>
		/// <param name="tag">tag to use</param>
		/// <param name="tagid">tag's id</param>
		/// <param name="tagclass">tag's class</param>
		/// <returns>starting tag</returns>
		public static string GiveStartElem(string tag, string tagid, string tagclass) {
			string tmpstr = "<" + tag;

			if (!String.IsNullOrEmpty(tagid)) {
				tmpstr = tmpstr + " id=\"" + tagid + "\"";
			}

			if (!String.IsNullOrEmpty(tagclass)) {
				tmpstr = tmpstr + " class=\"" + tagclass + "\"";
			}


			tmpstr = tmpstr + HtmlAddons.GiveTagAddon(tag) + ">";

			return tmpstr;
		}

		/// <summary>
		/// combines ending tag
		/// </summary>
		/// <param name="tag">tag to use</param>
		/// <returns>ending tag</returns>
		public static string GiveEndElem(string tag) {
			return GiveTagEnding(tag);
		}
	}
}
