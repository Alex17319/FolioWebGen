using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class MultiSection : PageSection
	{
		public ReadOnlyCollection<(PageSection section, string label)> LabelledSections { get; }

		public override string Format => " * ";

		public MultiSection(string fileName, IEnumerable<(PageSection section, string label)> labelledSections)
			: base(fileName)
		{
			this.LabelledSections = labelledSections.ToList().AsReadOnly();
		}

		public override object SectionContentsToHtml(PageSectionContext ctx)
		{
			return new XElement(
				"div",
				new XAttribute("class", "multiSectionContainer"),
				GetTabBar(),
				new XElement(
					"div",
					new XAttribute("class", "multiSectionBody"),
					from labelledSection in LabelledSections
					select new XElement(
						"div",
						new XAttribute("class", "multiSectionTabContent"),
						labelledSection.section.SectionContentsToHtml(ctx)
					)
				)
			);
		}

		private XElement GetTabBar()
		{
			if (LabelledSections.Count <= 1) return null;
			else return new XElement(
				"div",
				new XAttribute("class", "multiSectionTabBar"),
				from section in LabelledSections
				select new XElement(
					"button",
					new XAttribute("class", "multiSectionTabLabel"),
					new XAttribute("onclick", "activateMultiSectionTab(this)"),
					section.label
				)
			);
		}
	}
}
