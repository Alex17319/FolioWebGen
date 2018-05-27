using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class SiteMapSection : PageSection
	{
		private const string siteMapClass = "siteMap";
		private const string siteMapPartClass = "siteMapPart";

		public override string Format => "Auto Site Map";

		public SiteMapSection(string fileName) : base(fileName)
		{

		}

		public override object SectionContentsToHtml(PageSectionContext ctx)
		{
			return new XElement(
				"div",
				new XAttribute("class", siteMapClass),
				new XElement(
					"div",
					new XAttribute("class", siteMapPartClass),
					PageToSiteMapPart(ctx.Website.Root)
				)
			);
		}

		private object PageToSiteMapPart(Page page)
		{
			return new XElement(
				"a",
				new XAttribute("href", page.PathFromRoot),
				page.DisplayName,
				new XElement(
					"div",
					new XAttribute("class", siteMapPartClass),
					from child in page.Children
					select PageToSiteMapPart(child)
				)
			);
		}
	}
}
