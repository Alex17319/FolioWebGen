using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class ExternalHtmlSection : PageSection
	{
		public HtmlPage Page { get; }

		public override string Format => "External Page";

		public ExternalHtmlSection(string name, HtmlPage page) : base(name)
		{
			this.Page = page ?? throw new ArgumentNullException(nameof(page));
		}

		public override object SectionContentsToHtml(ExternalContentReg extReg)
		{
			extReg.HtmlPages.Register(Page);

			return Page.ToHtml(extReg);
		}
	}
}
