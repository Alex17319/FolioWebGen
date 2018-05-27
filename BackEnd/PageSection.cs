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
		public string DisplayName { get; }
		public string CodeName { get; }

		public abstract string Format { get; }

		public PageSection(string displayName)
		{
			this.DisplayName = string.IsNullOrWhiteSpace(displayName) ? "" : displayName;
			this.CodeName = string.IsNullOrWhiteSpace(displayName) ? "" : StringUtils.GetItemUrlName(CodeName);
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
		public PageContext PageCtx;

		public Website Website => PageCtx.Website;
		public ExternalContentReg ExtReg => PageCtx.Website.ExtReg;

		public PageSectionContext(Page page, PageContext ctx) {
			this.Page = page;
			this.PageCtx = ctx;
		}
	}
}
