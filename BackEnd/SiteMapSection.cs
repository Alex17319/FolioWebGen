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
		public override string Format => "Auto Site Map";

		public SiteMapSection(string name) : base(name)
		{

		}

		public override object SectionContentsToHtml(PageSectionContext ctx)
		{
			//TODO
			return "TODO: Implement site map";
		}
	}
}
