using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public interface IHtmlPrintable
	{
		object ToHtml(Page page, Website site);
	}
}
