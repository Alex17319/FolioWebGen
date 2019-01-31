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
		public string FileName { get; }
		public string DisplayName { get; }
		public string UrlName { get; }

		public PageDirFileType Type { get; }
		public bool IsHidden => Type == PageDirFileType.Hidden;

		public abstract string Format { get; }

		public PageSection(string fileName, PageDirFileType type)
		{
			if (type == PageDirFileType.Variable) throw new InvalidOperationException(
				"Cannot create a page section when the provided type specifies that the source page is a variable "
				+ "(file: '" + fileName + "')."
			);

			this.FileName = string.IsNullOrWhiteSpace(fileName) ? "" : fileName;
			this.DisplayName = StringUtils.GetItemDisplayName(fileName: this.FileName);
			this.UrlName = StringUtils.GetItemUrlName(displayName: this.DisplayName);

			this.Type = type;
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
