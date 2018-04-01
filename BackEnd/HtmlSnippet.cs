using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class HtmlSnippet : IHtmlPrintable
	{
		public string Html { get; }

		public HtmlSnippet(string html)
		{
			this.Html = html ?? throw new ArgumentNullException(nameof(html));
		}

		public object ToHtml(ExternalContentReg extReg)
		{
			return Html;
		}
	}
}
