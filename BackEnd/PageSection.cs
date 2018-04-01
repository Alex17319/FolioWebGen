using FolioWebGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public abstract class PageSection
	{
		/// <summary>Note: This is the name that is used for sorting</summary>
		public string Name { get; }
		public string DisplayName { get; }

		public abstract string Format { get; }

		public PageSection(string name)
		{
			this.Name = string.IsNullOrWhiteSpace(name) ? "" : name;
			this.DisplayName = string.IsNullOrWhiteSpace(name) ? "" : StringUtils.GetItemDisplayName(name);
		}

		public abstract object SectionContentsToHtml(PageSectionContext ctx);

		public object ToHtml(PageSectionContext ctx)
		{
			return new XElement(
				"section",
				SectionContentsToHtml(ctx)
			);
		}
	}

	public struct PageSectionContext
	{
		public readonly Page Page;
		public PageContext Ctx;

		public PageSectionContext(Page page, PageContext ctx) {
			this.Page = page;
			this.Ctx = ctx;
		}
	}
}
