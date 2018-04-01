using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class Image : SimpleEmbeddable<Image>
	{
		public Image(FileInfo sourcePath) : base(sourcePath) { }

		public override object ToHtml(ExternalContentReg extReg)
		{
			extReg.Images.Register(this);

			return new XElement(
				"img",
				new XAttribute("src", extReg.Images.GetPath(this)),
				new XAttribute("alt", "(Image from source path '" + this.SourcePath + "' should be here)"),
				new XAttribute("class", "embeddedImage")
			);
		}
	}
}
