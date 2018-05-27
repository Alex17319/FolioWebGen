using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class MultiFormatSection : MultiSection
	{
		public MultiFormatSection(IEnumerable<PageSection> formats)
			: base(
				fileName: "", //Don't want to display the name twice //TODO: Investigate & clarify
				labelledSections: formats.Select(x => (section: x, label: x.Format))
			)
		{ }
	}
}
