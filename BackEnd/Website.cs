using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolioWebGen.BackEnd
{
	public class Website
	{
		public string SiteName { get; }
		public Page Root { get; }
		public ExternalContentReg ExternalReg { get; }

		public Website(string siteName, Page root, ExternalContentReg extReg)
		{
			this.SiteName = siteName ?? throw new ArgumentNullException(nameof(siteName));
			this.Root = root ?? throw new ArgumentNullException(nameof(root));
			this.ExternalReg = extReg ?? throw new ArgumentNullException(nameof(extReg));
		}

		//TODO: Add (asynchronous?) functions to convert this into all the files, folders,
		//cloud stuff etc needed for the website

		public async Task WriteToDirectoryAsync(DirectoryInfo dir)
		{
			await Task.Run(() => WriteToDirectory(dir));
		}

		public void WriteToDirectory(DirectoryInfo dir)
		{

		}

		public void PageToDirectory(Page page, DirectoryInfo parent)
		{
			using (var pageFile = File.CreateText(Path.Combine(parent.FullName, page.UrlName) + ".html"))
			{
				pageFile.Write(
					"<!DOCTYPE html>\r\n"
					+ new XElement(
						"html",
						page.ContentsToHtml(new PageContext(website: this))
					).ToString()
				);
			}

			var subPagesDir = Directory.CreateDirectory(Path.Combine(parent.FullName, page.UrlName));
			foreach (var childPage in page.Children)
			{
				PageToDirectory(childPage, subPagesDir);
			}
		}
	}
}
