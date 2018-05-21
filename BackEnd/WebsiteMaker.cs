using FolioWebGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public static class WebsiteMaker
	{
		public static Website MakeWebsite(DirectoryInfo root)
		{
			return new Website(
				siteName: "E-Portfolio",
				root: MakePage(root),
				extReg: new ExternalContentReg(
					new SimpleEmbedReg<Image>(
						sourceRoot: new DirectoryInfo("TODO"),
						onlineRoot: "Resources/Images"
					),
					new SimpleEmbedReg<Pdf>(
						sourceRoot: new DirectoryInfo("TODO"),
						onlineRoot: "Resources/Pdfs"
					),
					new SimpleEmbedReg<HtmlPage>(
						sourceRoot: new DirectoryInfo("TODO"),
						onlineRoot: "Resources/HtmlPages"
					)
				)
			);
		}

		public static Page MakePage(DirectoryInfo dir)
		{
			var dirContents = new PageDirContents(dir);

			return new Page(
				name: GetPageName(dirContents),
				sections: GetPageSections(
					dirContents.PageContent
					.GroupBy(file => file.GetNameWithoutExt(), (name, files) => new MultiFormatFile(name, files))
					.ToList()
				).ToList(),
				children: dirContents.Children.Select(c => MakePage(c)).ToList(),
				pageMetadata: new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())
			);
		}

		public static string GetPageName(PageDirContents page)
		{
			if (page == null) throw new ArgumentNullException(nameof(page));

			return page.GetVarValueOrNull("pagename")
					   ?? StringUtils.RemoveEnclosedSubstrings(page.Dir.Name, delimiter: '~');
		}

		public static string GetSectionName(FileInfo file)
		{
			if (file == null) throw new ArgumentNullException(nameof(file));

			return StringUtils.RemoveEnclosedSubstrings(file.GetNameWithoutExt(), delimiter: '~');
		}

		public static IEnumerable<PageSection> GetPageSections(IReadOnlyList<MultiFormatFile> pageContent)
		{
			for (int i = 0; i < pageContent.Count; i++) 
			{
				var multiFile = pageContent[i];
				string fileName = multiFile.Name;

				var formats = new List<PageSection>();

				foreach (var f in multiFile.Files)
				{
					if (FileTypes.IsRawTextDocument(f.Extension)) {
						formats.Add(new RawTextSection(fileName, f.OpenText().ReadToEnd()));
						continue;
					}

					if (FileTypes.IsImage(f.Extension)) {
						formats.Add(new ImageSection(fileName, new[] { new Image(f) }));
						continue;
					}

					if (FileTypes.IsPdf(f.Extension)) {
						formats.Add(new PdfSection(fileName, new Pdf(f)));
						continue;
					}

					if (FileTypes.IsExternalPage(f.Extension)) {
						formats.Add(new ExternalHtmlSection(fileName, new HtmlPage(f)));
						continue;
					}

					if (FileTypes.IsHtmlSnippet(f.Extension)) {
						formats.Add(new HtmlSnippetSection(fileName, new HtmlSnippet(f.OpenText().ReadToEnd())));
						continue;
					}

					formats.Add(
						new RawTextSection(
							fileName,
							"ERROR: PAGE CONTENT ITEM TYPE \"" + f + "\" IS NOT RECOGNISED\r\n"
							+ "FILE LOCATION IN WEBSITE GENERATOR INPUT: \"" + f.FullName + "\"\r\n"
							+ "FILE CONTENTS:"
							+ f.OpenText().ReadToEnd()
						)
					);
				}
				
				if (formats.Count == 1)
				{
					yield return formats[0];
				}
				else
				{
					yield return new MultiFormatSection(formats);
				}
			}
		}

		//Now that multiple formats need to be combined into a single section, displaying
		//multiple images in one section is too difficult (though I'll still leave ImageSection
		//capable of doing so)
		//	public static List<Image> GetConsecutiveImages(IReadOnlyList<MultiFormatFile> pageContent, int startIndex, out int nextIndex)
		//	{
		//		var images = new List<Image>();
		//	
		//		nextIndex = startIndex;
		//	
		//		for (int i = startIndex; i < pageContent.Count; i++)
		//		{
		//			if (FileTypes.IsImage(pageContent[i].Extension))
		//			{
		//				images.Add(new Image(pageContent[i]));
		//				nextIndex = i;
		//			}
		//			else break;
		//		}
		//	
		//		return images;
		//	}


		
	}
}
