using FolioWebGen.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class Page
	{
		private static string DirSeparatorStr => Path.DirectorySeparatorChar.ToString();

		/// <summary>Note: This is the name that is used for sorting</summary>
		public string FileName { get; }
		public string DisplayName { get; }

		public string UrlName { get; }

		private Page _parent;
		public Page Parent {
			get => _parent;
			private set {
				if (this._parent != null) throw new InvalidOperationException(
					"Page '" + this.FileName + "' already has a parent '" + this.Parent.FileName + "'."
				);
				else if (this.IsLockedAsRoot) throw new InvalidOperationException(
					"Page '" + this.FileName + "' is locked as a root page "
					+ "so cannot be used as a child page."
				);
				else this.Parent = value;
			}
		}
		public bool IsRoot => Parent == null;
		public bool IsLockedAsRoot { get; private set; }
		public void LockAsRoot() {
			if (!IsRoot) throw new InvalidOperationException("Cannot lock as root because the page is not the root.");
			IsLockedAsRoot = true;
		}

		/// <summary>Returns the current page, parent page, and so on up to the root.</summary>
		public IEnumerable<Page> ChainToRoot {
			get {
				var current = this.Parent;
				while (current != null) {
					yield return current;
					current = current.Parent;
				}
			}
		}
		/// <summary>Returns <see cref="ChainToRoot"/>, but excluding the current page.</summary>
		public IEnumerable<Page> Ancestors => ChainToRoot.Skip(1);
		/// <summary>Returns the root page, child-of-root page, and so on down to the current page.</summary>
		public IEnumerable<Page> ChainFromRoot => ChainToRoot.Reverse(); //There isn't really anything more efficient than this

		public Page Root => Ancestors.LastOrDefault() ?? this;
		public string UrlPathToRoot   => string.Join(DirSeparatorStr, Enumerable.Repeat("..", this.Ancestors.Count())  );
		public string UrlPathFromRoot => string.Join(DirSeparatorStr, this.ChainFromRoot.Skip(1).Select(a => a.UrlName));
		public string UrlPathTo(Page other) => UrlPathBetween(this, other);
		public string UrlPathFrom(Page other) => UrlPathBetween(other, this);

		public ReadOnlyCollection<PageSection> Sections { get; }

		public ReadOnlyCollection<Page> Children { get; }

		public IReadOnlyDictionary<string, string> PageMetadata { get; }

		/// <summary></summary>
		/// <param name="fileName"></param>
		/// <param name="sections"></param>
		/// <param name="children">can be unordered</param>
		/// <param name="pageMetadata"></param>
		public Page(string fileName, IEnumerable<PageSection> sections, IEnumerable<Page> children, IReadOnlyDictionary<string, string> pageMetadata)
		{
			if (fileName == null) throw new ArgumentNullException(nameof(fileName));
			if (sections == null) throw new ArgumentNullException(nameof(sections));
			if (children == null) throw new ArgumentNullException(nameof(children));
			if (pageMetadata == null) throw new ArgumentNullException(nameof(pageMetadata));

			foreach (var child in children)
			{
				try {
					child.Parent = this;
				} catch (InvalidOperationException e) {
					throw new ArgumentException("Error setting parent of provided child page '" + child.FileName + "'.", e);
				}
			}

			this.FileName = fileName;
			this.DisplayName = StringUtils.GetItemDisplayName(fileName: this.FileName);
			this.UrlName = StringUtils.GetItemUrlName(displayName: this.DisplayName);
			this.Sections = sections.OrderByNatural(x => x.FileName).ToList().AsReadOnly();
			this.Children = children.OrderByNatural(x => x.FileName).ToList().AsReadOnly();
			this.PageMetadata = pageMetadata;
		}

		/// <summary>
		/// Returns the contents of the &lt;html&gt; tag, but not the &lt;html&gt; tag itself.
		/// </summary>
		public object ContentsToHtml(PageContext ctx)
		{
			//Note: Some parts of this will be constructed for every page, despite being identical.
			//However, performance isn't important here (as File IO, OneDrive uploads, etc. will be
			//much more significant), so that's fine.

			return new[] {
				new XElement(
					"head",
					new XElement(
						"meta",
						new XAttribute("charset", "utf-8")
					),
					new XElement(
						"title",
						this.DisplayName + " - " + ctx.Website.SiteName
					),
					from meta in PageMetadata
					select new XElement(
						"meta",
						new XAttribute("name", meta.Key),
						new XAttribute("content", meta.Value)
					),
					new XElement(
						"link",
						new XAttribute("rel", "stylesheet"),
						new XAttribute("href", Path.Combine(UrlPathToRoot, "styles.css")) //TODO: Maybe add a ?v=(hash) to the end of the url later to fix any caching issues (if that even applies)
					),
					new XComment(
						//Explanation at https://www.sitepoint.com/a-basic-html5-template/ ("Leveling the Playing Field")
						@"[if lt IE 9]>"
						+ @"<script src=""https://cdnjs.cloudflare.com/ajax/libs/html5shiv/3.7.3/html5shiv.js""></script>"
						+ @"<![endif]"
					)
				),
				new XElement(
					"body",
					new XElement(
						"div", //Idk whether <header> element applies to siteHeaderBar, siteTitle, or pageTitle
						new XAttribute("class", "siteHeaderBar"),
						new XElement(
							"div",
							new XAttribute("class", "siteTitle"),
							ctx.Website.SiteName
						),
						new XElement(
							"nav",
							new XAttribute("class", "navBar"),
							new XElement(
								"ul",
								from childOfRoot in this.Root.Children
								select childOfRoot.GetNavBarItem()
							)
						)
					),
					new XElement(
						"div",
						new XAttribute("class", "pageTitle"),
						this.DisplayName
					),
					new XElement(
						"div",
						new XAttribute("class", "pagePathBar"),
						new XElement(
							"ul",
							this.Ancestors.SelectMany(
								ancestor => new object[] {
									new XElement(
										"li",
										new XAttribute("class", "pagePathBarDelimiter"),
										" > "
									),
									ancestor.GetPagePathBarItem(),
								}
							)
						)
					),
					new XElement(
						"main",
						from section in this.Sections
						select section.ToHtml(new PageSectionContext(this, ctx))
					)
				)
			};
		}

		public object GetNavBarItem()
		{
			//See https://codepen.io/andornagy/pen/xhiJH

			return new XElement(
				"li",
				new XAttribute("class", "navBarItem"),
				new XElement(
					"a",
					new XAttribute("class", "navBarItemText"),
					this.DisplayName
				),
				new XElement(
					"ul",
					new XAttribute("class", "dropDownBelow"),
					from child in Children
					select child.GetDropdownItem()
				)
			);
		}

		public object GetPagePathBarItem()
		{
			//See https://codepen.io/andornagy/pen/xhiJH

			return new XElement(
				"li",
				new XAttribute("class", "pagePathBarItem"),
				new XElement(
					"a",
					new XAttribute("class", "pagePathBarItemText"),
					this.DisplayName
				),
				new XElement(
					"ul",
					new XAttribute("class", "dropDownBelow"),
					from child in Children
					select child.GetDropdownItem()
				)
			);
		}

		public object GetDropdownItem()
		{
			//See https://codepen.io/andornagy/pen/xhiJH

			return new XElement(
				"li",
				new XAttribute("class", "dropDownItem"),
				new XElement(
					"a",
					new XAttribute("class", "dropDownItemText"),
					this.DisplayName
				),
				new XElement(
					"ul",
					new XAttribute("class", "dropDownRight"),
					from child in Children
					select child.GetDropdownItem()
				)
			);
		}

		public static string UrlPathBetween(Page start, Page end)
		{
			var chain = ChainBetween(start, end);
			return string.Join(
				separator: Path.DirectorySeparatorChar.ToString(),
				values: (
					Enumerable.Repeat("..", chain.up.Length)
					//(don't include the peak of the chain)
					.Concat(chain.down.Select(x => x.UrlName))
				)
			);
		}
		public static string FilePathBetween(Page start, Page end)
		{
			var chain = ChainBetween(start, end);
			return string.Join(
				separator: Path.DirectorySeparatorChar.ToString(),
				values: (
					Enumerable.Repeat("..", chain.up.Length)
					//(don't include the peak of the chain)
					.Concat(chain.down.Select(x => x.FileName))
				)
			);
		}
		/// <summary>
		/// Return the chain of pages needed to navigate between a start and end page,
		/// first travelling up the tree, then peaking at the closest common ancestor,
		/// then descending down to the destination.
		/// </summary>
		public static (Page[] up, Page peak, Page[] down) ChainBetween(Page start, Page end)
		{
			if (start == null) throw new ArgumentNullException(nameof(start));
			if (end == null) throw new ArgumentNullException(nameof(end));

			if (start.Root != end.Root) throw new InvalidOperationException(
				$"Pages '{start.DisplayName}' and '{end.DisplayName}' do not share a common root."
			);

			var startRootChain = start.ChainToRoot.ToList();
			var endRootChain = end.ChainToRoot.ToList();
			startRootChain.Reverse();
			endRootChain.Reverse();

			int numCommonAncestors = startRootChain.Zip(endRootChain, (s, e) => (s, e)).Count(x => object.ReferenceEquals(x.s, x.e));

			int numUniqueStartAncestors = startRootChain.Count - numCommonAncestors;
			int numUniqueEndAncestors = endRootChain.Count - numCommonAncestors;

			//Set up result variables
			var up = new Page[numUniqueStartAncestors];
			var peak = (Page)null;
			var down = new Page[numUniqueEndAncestors];

			//Assign result variables & return

			if (numCommonAncestors < startRootChain.Count) { //Avoid index out-of-range exception
				startRootChain.CopyTo(index: numCommonAncestors, array: up, arrayIndex: 0, count: numUniqueStartAncestors);
			}
			Array.Reverse(up);

			peak = startRootChain[numCommonAncestors]; //Closest common ancestor

			if (numCommonAncestors < endRootChain.Count) { //Avoid index out-of-range exception
				endRootChain.CopyTo(index: numCommonAncestors, array: down, arrayIndex: 0, count: numUniqueEndAncestors);
			}

			return (up, peak, down);
		}
	}

	public struct PageContext
	{
		public readonly Website Website;

		public ExternalContentReg ExtReg => Website.ExtReg;

		public PageContext(Website website) {
			this.Website = website;
		}
	}
}
