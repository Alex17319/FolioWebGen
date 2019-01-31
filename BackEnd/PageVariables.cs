using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FolioWebGen.Utilities;

namespace FolioWebGen.BackEnd
{
	public class PageVariables
	{
		public IReadOnlyDictionary<string, string> Variables { get; }

		public const string HiddenFilesVarName = "hidden-files";
		private ReadOnlyCollection<Regex> _hiddenFilePatterns;
		public ReadOnlyCollection<Regex> HiddenFilePatterns
			=> _hiddenFilePatterns ?? (_hiddenFilePatterns = GetHiddenFilePatterns(this.Variables));

		public const string HtmlMetaVarName = "html-meta";
		private ReadOnlyDictionary<string, string> _htmlPageMetaProperties;
		public ReadOnlyDictionary<string, string> HtmlPageMetaProperties
			=> _htmlPageMetaProperties ?? (_htmlPageMetaProperties = GetHtmlPageMetaProperties(this.Variables));

		//	public PageVariables(PageDirContents pageDirContents)
		//		: this(
		//			new ReadOnlyDictionary<string, string>(
		//				pageDirContents.Contents.Where(x => x.type == PageDirContentType.Variable)
		//				.Select(x => VariableReader.ReadVar(x.file))
		//				.ToDictionary(
		//					keySelector: x => x.name,
		//					elementSelector: x => x.value
		//				)
		//			)
		//		)
		//	{ }

		public PageVariables(IEnumerable<FileInfo> varFiles)
			: this(
				new ReadOnlyDictionary<string, string>(
					varFiles
					.Select(x => VariableReader.ReadVar(x))
					.Where(x => !string.IsNullOrEmpty(x.name))
					.Distinct(
						CustomEqualityComparer.Create<(string name, string value)>(
							equals: (x, y) => x.name == y.name,
							getHashCode: x => x.name.GetHashCode()
						)
					)
					.ToDictionary(
						keySelector: x => x.name,
						elementSelector: x => x.value
					)
				)
			)
		{ }

		public PageVariables(IReadOnlyDictionary<string, string> variables)
		{
			this.Variables = variables;
		}

		private static ReadOnlyDictionary<string, string> GetHtmlPageMetaProperties(IReadOnlyDictionary<string, string> variables)
		{
			if (variables.TryGetValue(HtmlMetaVarName, out string v))
			{
				return new ReadOnlyDictionary<string, string>(
					Enumerable.ToDictionary(
						from line in v.Split('\r', '\n')
						let eqIndex = line.IndexOf("=")
						where eqIndex >= 1
						let name = line.Substring(0, eqIndex)
						let value = line.Substring(eqIndex + 1)
						select (name, value),
						keySelector: x => x.name,
						elementSelector: x => x.value
					)
				);
			}

			return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
		}

		/// <summary>
		/// Note: Hidden file lists don't use regex, but rather just use the following pattern
		/// rules: '?'=(any character) and '*'=(any number of any character).
		/// This method converts these patterns to regex patterns for easy evaluation.
		/// </summary>
		private static ReadOnlyCollection<Regex> GetHiddenFilePatterns(IReadOnlyDictionary<string, string> variables)
		{
			if (variables.TryGetValue(HiddenFilesVarName, out string v))
			{
				return (
					v
					.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(pattern => new Regex(Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".")))
					.ToList()
					.AsReadOnly()
				);
			}
			else return new List<Regex>().AsReadOnly();
		}
	}
}

//*/