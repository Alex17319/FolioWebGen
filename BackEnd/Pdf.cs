using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class Pdf : SimpleEmbeddable<Pdf>
	{
		public Pdf(SingleFormatFile sourcePath) : base(sourcePath) { }

		public override object ToHtml(ExternalContentReg extReg)
		{
			extReg.Pdfs.Register(this);

			return new XElement(
				"embed",
				new XAttribute("src", extReg.Pdfs.GetPath(this)),
				new XAttribute("type", "application/pdf"),
				new XAttribute("class", "embeddedPdf")
			);
		}
	}
}
