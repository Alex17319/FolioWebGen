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
	public class PageDirContents
	{
		public DirectoryInfo Dir { get; }
		public string FileName { get; }

		//	public ReadOnlyCollection<FileInfo> Variables { get; }
		//	/// <summary>Unsorted (sorted later - the original file names are kept to keep the original order)</summary>
		//	public ReadOnlyCollection<FileInfo> PageContent { get; }
		
		/// <summary>Unsorted (sorted later - the original file names are kept to keep the original order)</summary>
		public ReadOnlyCollection<(FileInfo file, PageDirFileType type)> Contents { get; }
		/// <summary>Unsorted (sorted later - the original file names are kept to keep the original order)</summary>
		public ReadOnlyCollection<DirectoryInfo> Children { get; }

		public PageVariables Variables { get; }

		public PageDirContents(DirectoryInfo dir)
		{
			this.Dir = dir ?? throw new ArgumentNullException(nameof(dir));

			this.FileName = dir.Name;

			this.Contents = (
				dir.EnumerateFiles()
				.Select(f => (file: f, type: Categorise(f)))
				.ToList()
				.AsReadOnly()
			);
			this.Children = (
				dir.EnumerateDirectories()
				.ToList()
				.AsReadOnly()
			);

			this.Variables = new PageVariables(
				this.Contents.Where(x => x.type == PageDirFileType.Variable).Select(x => x.file)
			);
			
			//This will now be done later, by the Page and PageSection classes
			//	//Flag additional hidden files and folders
			//	foreach (var pattern in this.Variables.HiddenFilePatterns)
			//	{
			//		for (int i = 0; i < contents.Count; i++) {
			//			if (contents[i].type != PageDirFileType.PageSection) continue;
			//			if (pattern.IsMatch(contents[i].file.Name)) {
			//				contents[i] = (contents[i].file, PageDirFileType.Hidden);
			//			}
			//		}
			//	
			//		for (int i = 0; i < children.Count; i++) {
			//			if (children[i].type != PageDirFolderType.SubPage) continue;
			//			if (pattern.IsMatch(children[i].file.Name)) {
			//				children[i] = (children[i].file, PageDirFolderType.Hidden);
			//			}
			//		}
			//	}
		}

		/// <summary>
		/// Returns one of <see cref="PageDirFileType.Hidden"/>, <see cref="PageDirFileType.PageSection"/>, or <see cref="PageDirFileType.Variable"/>.
		/// </summary>
		public static PageDirFileType Categorise(FileInfo file)
		{
			//This will now be done later, by the Page and PageSection classes
			// if (file.Name.StartsWith(".")) return PageDirFileType.Hidden;
			if (Regex.IsMatch(file.Name, @"^\$.+\$\.var$")) return PageDirFileType.Variable;
			else if (Regex.IsMatch(file.Name, @"^\$.+\$=.*\.var$")) return PageDirFileType.Variable;
			else return PageDirFileType.PageSection;
		}

		//This will now be done later, by the Page and PageSection classes
		//	/// <summary>
		//	/// Returns either <see cref="PageDirFileType.Hidden"/> or <see cref="PageDirFileType.SubPage"/>.
		//	/// </summary>
		//	public static PageDirFolderType Categorise(DirectoryInfo file)
		//	{
		//		if (file.Name.StartsWith(".")) return PageDirFolderType.Hidden;
		//		else return PageDirFolderType.SubPage;
		//	}
	}
}
