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
		/// <summary>Note: This is the name that is used for sorting</summary>
		public string Name { get; }
		public string DisplayName { get; }

		public Page Parent { get; private set; }
		public bool IsRoot => Parent == null;
		public bool IsLockedAsRoot { get; private set; }
		public void LockAsRoot() {
			if (!IsRoot) throw new InvalidOperationException("Cannot lock as root because the page is not the root.");
			IsLockedAsRoot = true;
		}

		public IEnumerable<Page> Ancestors {
			get {
				var current = this.Parent;
				while (current != null) {
					yield return current;
					current = current.Parent;
				}
			}
		}
		public Page Root => Ancestors.LastOrDefault() ?? this;
		public string PathToRoot => string.Join(
			separator: Path.DirectorySeparatorChar.ToString(),
			values: Enumerable.Repeat("..", this.Ancestors.Count())
		);

		public IReadOnlyList<PageSection> Sections { get; }

		public IReadOnlyList<Page> Children { get; }

		public IReadOnlyDictionary<string, string> PageMetadata { get; }

		public Page(string name, IReadOnlyList<PageSection> sections, IEnumerable<Page> children, IReadOnlyDictionary<string, string> pageMetadata)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (sections == null) throw new ArgumentNullException(nameof(sections));
			if (children == null) throw new ArgumentNullException(nameof(children));
			if (pageMetadata == null) throw new ArgumentNullException(nameof(pageMetadata));

			int i = 0;
			foreach (var child in children)
			{
				if (child.Parent != null) throw new ArgumentException(
					"Page at index " + i + " in child pages list already has a parent '" + child.Parent + "'."
				);
				else if (child.IsLockedAsRoot) throw new ArgumentException(
					"Page at index " + i + " in child pages list is locked as a root page "
					+ "so cannot be used as a child page."
				);
				else child.Parent = this;

				i++;
			}

			this.Name = name;
			this.DisplayName = StringUtils.GetItemDisplayName(name);
			this.Sections = sections;
			this.Children = children.OrderByNatural(x => x.DisplayName).ToList();
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
						new XAttribute("href", Path.Combine(PathToRoot, "styles.css")) //TODO: Maybe add a ?v=(hash) to the end of the url later to fix any caching issues (if that even applies)
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
						select new XElement(
							"section",
							section.SectionContentsToHtml(new PageSectionContext(this, ctx))
						)
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
	}

	public struct PageContext
	{
		public readonly Website Website;

		public PageContext(Website website) {
			this.Website = website;
		}
	}
}
