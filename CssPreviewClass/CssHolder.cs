using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssPreviewClass {

	/// <summary>
	/// holds single selector+media rules
	/// </summary>
	public class CssHolderItem {
		/// <summary>
		/// init CssHolderItem
		/// </summary>
		/// <param name="sel">selector</param>
		/// <param name="med">media query</param>
		public CssHolderItem(string sel, string med) {
			this.Selector = sel;
			this.MediaQuery = med;
		}
		public string Selector { get; set; }
		public string MediaQuery { get; set; }
	}
	
	/// <summary>
	/// all selector/media pairs in one class
	/// </summary>
	public class CssHolder {
		public CssHolder() {
			HolderItems = new List<CssHolderItem>();			
		}
				
		public List<CssHolderItem> HolderItems { get; set; }		
	}
}
