using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class ExternalContentReg
	{
		public SimpleEmbedReg<Image> Images { get; }
		public SimpleEmbedReg<Pdf> Pdfs { get; }
		public SimpleEmbedReg<HtmlPage> HtmlPages { get; }

		public ExternalContentReg(SimpleEmbedReg<Image> images, SimpleEmbedReg<Pdf> pdfs, SimpleEmbedReg<HtmlPage> htmlPages)
		{
			this.Images = images ?? throw new ArgumentNullException(nameof(images));
			this.Pdfs = pdfs ?? throw new ArgumentNullException(nameof(pdfs));
			this.HtmlPages = htmlPages ?? throw new ArgumentNullException(nameof(htmlPages));
		}


	}
}
