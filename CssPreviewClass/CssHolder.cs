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

		/// <summary>
		/// checks item existence
		/// </summary>
		/// <param name="sel">item's selector</param>
		/// <param name="med">item's media query</param>
		/// <returns>if item exists</returns>
		public Boolean HasItem(string sel, string med) {
			foreach (CssHolderItem chi in this.HolderItems) {
				if ((med != null) && (chi.MediaQuery != null)) {
					if (chi.Selector.Equals(sel) && chi.MediaQuery.Equals(med)) {
						return true;
					}
				} else {
					if (chi.Selector.Equals(sel)) {
						return true;
					}
				}				
			}

			return false;
		}

		/// <summary>
		/// Sorts CSS selectors
		/// </summary>
		public void SortIt() {
			this.HolderItems = this.HolderItems.OrderBy(s => s.MediaQuery).ThenBy(s => s.Selector).ToList();
		}		
	}

}
