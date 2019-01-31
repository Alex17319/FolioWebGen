using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class RawTextSection : PageSection
	{
		public string Text { get; }

		public override string Format => "Text";

		public RawTextSection(string fileName, PageVariables pageVariables, string text) : base(fileName, pageVariables)
		{
			this.Text = text;
		}

		public override object SectionContentsToHtml(PageSectionContext ctx)
		{
			return WebUtility.HtmlEncode(
				Regex.Replace( //Remove all special characters
					Text,
					@"\p{C}+",
					// "\p{C}" matches characters in Unicode category 'C' (AKA 'Other').
					// See http://www.regular-expressions.info/unicode.html
					// and the column at the bottom right of http://www.unicode.org/charts/
					//Source: https://stackoverflow.com/a/4501246/4149474
					""
				)
			);
		}
	}
}
