﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class HtmlSnippetSection : PageSection
	{
		public HtmlSnippet Snippet { get; }

		public override string Format => "Html";

		public HtmlSnippetSection(string fileName, PageVariables pageVariables, HtmlSnippet snippet)
			: base(fileName, pageVariables)
		{
			this.Snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
		}

		public override object SectionContentsToHtml(PageSectionContext ctx)
		{
			return Snippet.ToHtml(ctx.ExternalReg);
		}
	}
}
