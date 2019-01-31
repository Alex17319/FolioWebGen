﻿using FolioWebGen.Utilities;
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

		public static Page MakePage(DirectoryInfo dir, PageDirFolderType type)
		{
			var dirContents = new PageDirContents(dir);

			return new Page(
				fileName: dirContents.FileName,
				type: type,
				sections: GetPageSections(
					dirContents.Contents
					.Where(x => x.type == PageDirFileType.PageSection)
					.Select(x => new SingleFormatFile(x.file))
					.GroupBy(file => file.FileNameWithoutExtension, (name, files) => new MultiFormatFile(files))
					.ToList()
				),
				children: dirContents.Children.Select(c => MakePage(c.dir, )),
				variables: dirContents.Variables
			);
		}

		public static IEnumerable<PageSection> GetPageSections(IReadOnlyList<MultiFormatFile> pageContent)
		{
			for (int i = 0; i < pageContent.Count; i++) 
			{
				var multiFile = pageContent[i];
				string fileName = multiFile.ExtensionlessFileName;

				var formats = new List<PageSection>();

				foreach (var f in multiFile.Files)
				{
					if (FileTypes.IsRawTextDocument(f.Extension)) {
						formats.Add(new RawTextSection(fileName, f.FileInfo.OpenText().ReadToEnd()));
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
						formats.Add(new HtmlSnippetSection(fileName, new HtmlSnippet(f.FileInfo.OpenText().ReadToEnd())));
						continue;
					}

					formats.Add(
						new RawTextSection(
							fileName,
							"ERROR: PAGE CONTENT ITEM TYPE \"" + f.Extension + "\" IS NOT RECOGNISED\r\n"
							+ "FILE LOCATION IN WEBSITE GENERATOR INPUT: \"" + f.Path + "\"\r\n"
							+ "FILE CONTENTS:\r\n"
							+ f.FileInfo.OpenText().ReadToEnd()
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
