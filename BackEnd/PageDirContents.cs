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

		public ReadOnlyCollection<FileInfo> Variables { get; }
		/// <summary>Unsorted (sorted later - the original file names are kept to keep the original order)</summary>
		public ReadOnlyCollection<FileInfo> PageContent { get; }
		/// <summary>Unsorted (sorted later - the original file names are kept to keep the original order)</summary>
		public ReadOnlyCollection<DirectoryInfo> Children { get; }

		public PageDirContents(DirectoryInfo dir)
		{
			this.Dir = dir ?? throw new ArgumentNullException(nameof(dir));

			this.FileName = dir.Name;

			this.Variables = new List<FileInfo>(
				Enumerable.Concat(
					dir.GetFiles("$*$=*.var"),
					dir.GetFiles("$*$.var")
				)
			).AsReadOnly();

			this.PageContent = (
				dir.EnumerateFiles()
				.Where(f => !f.Name.StartsWith("."))
				.ToList()
				.AsReadOnly()
			);
			this.Children = (
				dir.EnumerateDirectories()
				.Where(f => !f.Name.StartsWith("."))
				.ToList()
				.AsReadOnly()
			);
		}

		/// <param name="name">Case insensitive</param>
		public bool TryGetVarValue(string name, out string value)
		{
			return TryGetLongVarValue(name, out value) || TryGetShortVarValue(name, out value);
		}

		/// <param name="name">Case insensitive</param>
		public string GetVarValueOrNull(string name)
		{
			return TryGetVarValue(name, out string value) ? value : null;
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

		public bool TryReadVar(FileInfo varFile, out (string name, string value) result)
		{
			return TryReadLongVar(varFile, out result) || TryReadShortVar(varFile.Name, out result);
		}

		/// <summary>
		/// Reads a variable-file.
		/// Returns (<see langword="null"/>, <see langword="null"/>) if the file is not a valid variable file.
		/// </summary>
		public (string name, string value) ReadVar(FileInfo varFile)
		{
			return TryReadVar(varFile, out (string, string) result) ? result : default;
		}

		public bool TryReadLongVar(FileInfo varFile, out (string name, string value) result)
		{
			if (varFile == null) throw new ArgumentNullException(nameof(varFile));

			result = (
				name: Regex.Match(varFile.Name, @"(?<=\$).+(?=\.var)").Value,
				value: File.ReadAllText(varFile.FullName)
			);
			return result.name != "";
		}

		public bool TryReadShortVar(string varFileName, out (string name, string value) result)
		{
			if (varFileName == null) throw new ArgumentNullException(nameof(varFileName));

			result = (
				name: Regex.Match(varFileName, @"(?<=\$).+(?=\$\=.+\.var)").Value,
				value: Regex.Match(varFileName, @"(?<=\$.+\$\=).+(?=\.var)").Value
			);
			return result.name != "";
		}
	}
}
