using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class HtmlPage : SimpleEmbeddable<HtmlPage>
	{
		public HtmlPage(SingleFormatFile sourcePath) : base(sourcePath) { }

		public override object ToHtml(ExternalContentReg extReg)
		{
			extReg.HtmlPages.Register(this);

			return new XElement(
				"object",
				new XAttribute("data", SourcePath),
				new XAttribute("class", "embeddedHtmlPage")
			);
		}
	}
}
