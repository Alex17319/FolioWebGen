using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class PdfSection : PageSection
	{
		public Pdf Pdf { get; }

		public override string Format => "Pdf";

		public PdfSection(string displayName, Pdf pdf) : base(displayName)
		{
			this.Pdf = pdf ?? throw new ArgumentNullException(nameof(pdf));
		}

		public override object SectionContentsToHtml(PageSectionContext ctx)
		{
			ctx.PageCtx.Website.ExtReg.Pdfs.Register(Pdf);

			return Pdf.ToHtml(ctx.PageCtx.Website.ExtReg);
		}
	}
}
