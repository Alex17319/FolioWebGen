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

		public bool IsHidden { get; }

		protected PageVariables PageVariables { get; }

		public abstract string Format { get; }

		public PageSection(string fileName, PageVariables pageVariables)
		{
			this.FileName = string.IsNullOrWhiteSpace(fileName) ? "" : fileName;
			this.DisplayName = StringUtils.GetItemDisplayName(fileName: this.FileName);
			this.UrlName = StringUtils.GetItemUrlName(displayName: this.DisplayName);

			this.PageVariables = pageVariables;
			this.IsHidden = IsPageSectionHidden(fileName, pageVariables);
		}

		public abstract object SectionContentsToHtml(PageSectionContext ctx);

		public object ToHtml(PageSectionContext ctx)
		{
			return new XElement(
				"section",
				SectionContentsToHtml(ctx)
			);
		}

		public static bool IsPageSectionHidden(string fileName, PageVariables variables)
			=> fileName.StartsWith(".") || variables.HiddenFilePatterns.Any(p => p.IsMatch(fileName));
	}

	public struct PageSectionContext
	{
		public readonly Page Page;
		public PageContext PageCtx;

		public Website Website => PageCtx.Website;
		public ExternalContentReg ExternalReg => PageCtx.Website.ExternalReg;

		public PageSectionContext(Page page, PageContext ctx) {
			this.Page = page;
			this.PageCtx = ctx;
		}
	}
}
