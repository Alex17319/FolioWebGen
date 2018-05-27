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
		public string DisplayName { get; }

		public IReadOnlyList<FileInfo> Variables { get; }
		/// <summary>Sorted by name</summary> //TODO: Remove
		public IReadOnlyList<FileInfo> PageContent { get; }
		/// <summary>Sorted by name</summary> //TODO: Remove
		public IReadOnlyList<DirectoryInfo> Children { get; }

		public PageDirContents(DirectoryInfo dir)
		{
			this.Dir = dir ?? throw new ArgumentNullException(nameof(dir));

			this.FileName = dir.Name;

			this.Variables = new List<FileInfo>(
				Enumerable.Concat(
					dir.GetFiles("$*=*.var"), //TODO: Change to "$*$=*.var" (have to search for .var files to make this change)
					dir.GetFiles("$*$.var")
				)
			).AsReadOnly();

			this.PageContent = (
				dir.EnumerateFiles()
				.Where(f => !f.Name.StartsWith("."))
				//.OrderByNatural(x => x.Name) //Sort now (before formatting the names) so that the sort order is the same as when the files are created
				.ToList()
			);
			this.Children = (
				dir.EnumerateDirectories()
				.Where(f => !f.Name.StartsWith("."))
				//.OrderByNatural(x => x.Name) //See above
				.ToList()
			);
		}

		/// <param name="name">Case insensitive</param>
		public bool TryGetVarValue(string name, out string value)
		{
			return TryGetLongVarValue(name, out value) || TryGetShortVarValue(name, out value);
		}

		private bool TryGetLongVarValue(string name, out string value)
		{
			string lookup = "$" + name + "$.var";

			var varFile = Variables.SingleOrDefault(
				v => string.Equals(v.Name, lookup, StringComparison.InvariantCultureIgnoreCase)
			);

			if (varFile == null) { value = null; return false; }

			value = File.ReadAllText(varFile.FullName);
			return true;
		}

		private bool TryGetShortVarValue(string name, out string value)
		{
			string lookup = "$" + name + "=";

			var varFile = Variables.SingleOrDefault(
				v => v.Name.StartsWith(lookup, StringComparison.InvariantCultureIgnoreCase)
			);

			if (varFile == null) { value = null; return false; }

			value = Path.GetFileNameWithoutExtension(varFile.Name.Substring(startIndex: lookup.Length));
			return true;
		}

		/// <param name="name">Case insensitive</param>
		public string GetVarValueOrNull(string name)
		{
			return TryGetVarValue(name, out string value) ? value : null;
		}
	}
}
