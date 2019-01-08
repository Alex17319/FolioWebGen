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
		public ReadOnlyCollection<(FileInfo file, PageDirContentType type)> Contents { get; }
		/// <summary>Unsorted (sorted later - the original file names are kept to keep the original order)</summary>
		public ReadOnlyCollection<(DirectoryInfo, PageDirContentType type)> Children { get; }

		public PageDirContents(DirectoryInfo dir)
		{
			this.Dir = dir ?? throw new ArgumentNullException(nameof(dir));

			this.FileName = dir.Name;

			var contents = (
				dir.EnumerateFiles()
				.Select(f => (file: f, type: Categorise(f)))
				.ToList()
			);
			var children = (
				dir.EnumerateDirectories()
				.Select(f => (file: f, type: Categorise(f)))
				.ToList()
			);
			
			//Flag additional hidden files and folders
			foreach (var pattern in GetHiddenFilePatterns(contents))
			{
				for (int i = 0; i < contents.Count; i++) {
					if (contents[i].type != PageDirContentType.PageSection) continue;
					if (pattern.IsMatch(contents[i].file.Name)) {
						contents[i] = (contents[i].file, PageDirContentType.Hidden);
					}
				}

				for (int i = 0; i < children.Count; i++) {
					if (children[i].type != PageDirContentType.SubPage) continue;
					if (pattern.IsMatch(children[i].file.Name)) {
						children[i] = (children[i].file, PageDirContentType.Hidden);
					}
				}
			}

			this.Contents = contents.AsReadOnly();
			this.Children = children.AsReadOnly();
		}

		/// <summary>
		/// Returns one of <see cref="PageDirContentType.Hidden"/>, <see cref="PageDirContentType.PageSection"/>, or <see cref="PageDirContentType.Variable"/>.
		/// </summary>
		public static PageDirContentType Categorise(FileInfo file)
		{
			if (file.Name.StartsWith(".")) return PageDirContentType.Hidden;
			else if (Regex.IsMatch(file.Name, @"^\$.+\$\.var$")) return PageDirContentType.Variable;
			else if (Regex.IsMatch(file.Name, @"^\$.+\$=.*\.var$")) return PageDirContentType.Variable;
			else return PageDirContentType.PageSection;
		}

		/// <summary>
		/// Returns either <see cref="PageDirContentType.Hidden"/> or <see cref="PageDirContentType.SubPage"/>.
		/// </summary>
		public static PageDirContentType Categorise(DirectoryInfo file)
		{
			if (file.Name.StartsWith(".")) return PageDirContentType.Hidden;
			else return PageDirContentType.PageSection;
		}

		/// <summary>
		/// Note: Hidden file lists don't use regex, but rather just use the pattern
		/// rules '?'='any character' and '*'='any number of any character'.
		/// This method converts these patterns to regex patterns for easy evaluation.
		/// </summary>
		private static IEnumerable<Regex> GetHiddenFilePatterns(IEnumerable<(FileInfo file, PageDirContentType type)> contents)
		{
			var hiddenFilesVar = VariableReader.ReadVar(
				vars: contents.Where(x => x.type == PageDirContentType.Variable).Select(x => x.file),
				name: PageVariables.HiddenFilesVarName
			);
			return (
				hiddenFilesVar
				.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(pattern => new Regex(Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".")))
			);
		}
	}
}
