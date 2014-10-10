using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssPreviewClass {

	public class CssTreeItem {
		public CssTreeItem(string sel, string med) {
			this.Selector = sel;
			this.MediaQuery = med;
			this.Tag = "";
			this.TagId = "";
			this.TagClass = "";
			this.FullSelector = this.Selector;
		}

		public CssTreeItem(string sel, string med, string full) {
			this.Selector = sel;
			this.MediaQuery = med;
			this.Tag = "";
			this.TagId = "";
			this.TagClass = "";
			this.FullSelector = full;
		}

		public CssTreeItem(string sel, string med, string full, string tag) {
			this.Selector = sel;
			this.MediaQuery = med;
			this.Tag = tag;
			this.TagId = "";
			this.TagClass = "";
			this.FullSelector = full;
		}

		public string Selector { get; set; }
		public string Tag { get; set; }
		public string TagId { get; set; }
		public string TagClass { get; set; }
		public string MediaQuery { get; set; }
				
		public string FullSelector { get; set; }
	}

	public class CssTree<CssTreeItem> {
		private readonly CssTreeItem _value;
		private readonly List<CssTree<CssTreeItem>> _children = new List<CssTree<CssTreeItem>>();

		public CssTree(CssTreeItem value) {
			_value = value;
		}

		public CssTree<CssTreeItem> this[int i] {
			get { return _children[i]; }
		}

		public CssTree<CssTreeItem> Parent { get; private set; }

		public CssTreeItem Value { get { return _value; } }

		public ReadOnlyCollection<CssTree<CssTreeItem>> Children {
			get { return _children.AsReadOnly(); }
		}

		public CssTree<CssTreeItem> AddChild(CssTreeItem value) {
			var node = new CssTree<CssTreeItem>(value) { Parent = this };
			_children.Add(node);
			return node;
		}

		public CssTree<CssTreeItem>[] AddChildren(params CssTreeItem[] values) {
			return values.Select(AddChild).ToArray();
		}

		public bool RemoveChild(CssTree<CssTreeItem> node) {
			return _children.Remove(node);
		}

		public void Traverse(Action<CssTreeItem> action) {
			action(Value);
			foreach (var child in _children)
				child.Traverse(action);
		}

		public IEnumerable<CssTreeItem> Flatten() {
			return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
		}
	}

	public class TreeNode<T> : IEnumerable<TreeNode<T>> {
		public T Data { get; set; }
		public TreeNode<T> Parent { get; set; }
		public ICollection<TreeNode<T>> Children { get; set; }

		public Boolean IsRoot {
			get { return Parent == null; }
		}

		public Boolean IsLeaf {
			get { return Children.Count == 0; }
		}

		public int Level {
			get {
				if (this.IsRoot)
					return 0;
				return Parent.Level + 1;
			}
		}

		public TreeNode(T data) {
			this.Data = data;
			this.Children = new LinkedList<TreeNode<T>>();

			this.ElementsIndex = new LinkedList<TreeNode<T>>();
			this.ElementsIndex.Add(this);
		}

		public TreeNode<T> AddChild(T child) {			
			TreeNode<T> childNode = new TreeNode<T>(child) { Parent = this };
			this.Children.Add(childNode);
			this.RegisterChildForSearch(childNode);
			return childNode;
		}

		public override string ToString() {
			return Data != null ? Data.ToString() : "[data null]";
		}

		#region searching

		private ICollection<TreeNode<T>> ElementsIndex { get; set; }
		
		private void RegisterChildForSearch(TreeNode<T> node) {
			ElementsIndex.Add(node);
			if (Parent != null)
				Parent.RegisterChildForSearch(node);
		}

		public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate) {
			return this.ElementsIndex.FirstOrDefault(predicate);
		}

		#endregion
		
		#region iterating

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}


		public IEnumerator<TreeNode<T>> GetEnumerator() {
			yield return this;
			foreach (var directChild in this.Children) {
				foreach (var anyChild in directChild)
					yield return anyChild;
			}
		}

		#endregion
	}

}
