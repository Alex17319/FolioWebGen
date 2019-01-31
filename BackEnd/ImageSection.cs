using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class ImageSection : PageSection
	{
		public ReadOnlyCollection<Image> Images { get; }

		public override string Format => "Image";

		public ImageSection(string fileName, PageVariables pageVariables, IList<Image> images) : base(fileName, pageVariables)
		{
			this.Images = new ReadOnlyCollection<Image>(images ?? throw new ArgumentNullException(nameof(images)));
		}

		public override object SectionContentsToHtml(PageSectionContext ctx)
		{
			ctx.ExternalReg.Images.Register(Images);

			return Images.Select(x => x.ToHtml(ctx.ExternalReg));
		}
	}
}
