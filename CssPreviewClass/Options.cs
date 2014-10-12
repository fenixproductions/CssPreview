using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssPreviewClass {
	public class Options {
		public Options(string filename) {
			this._cssFileNameFull = filename;
			this._cssFileName = filename.Substring(filename.LastIndexOf(@"\") + 1);
			this.CssFolderName = filename.Substring(0, filename.LastIndexOf(@"\") + 1);
			this.HtmlFileName = this._cssFileName.Substring(0, this._cssFileName.LastIndexOf('.')) + ".html";
			this.InnerText = "";
		}

		private String _cssFileName;
		private String _cssFileNameFull;

		public String CssFileName {
			get {
				return this._cssFileName;
			}
		}
		
		public String CssFileNameFull {
			get {
				return this._cssFileNameFull;
			}
		}

		public String HtmlFileName { get; set; }
		public String CssFolderName { get; set; }
		public String InnerText { get; set; }


		
	}
}
