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

namespace FolioWebGen.BackEnd
{
	public class PageVariables
	{
		public IReadOnlyDictionary<string, string> Variables { get; }

		public const string HtmlMetaVarName = "html-meta";
		public IReadOnlyDictionary<string, string> HtmlPageMetaProperties { get; }

		public const string HiddenFilesVarName = "hidden-files";

		public PageVariables(PageDirContents pageDirContents)
			: this(
				new ReadOnlyDictionary<string, string>(
					pageDirContents.Contents.Where(x => x.type == PageDirContentType.Variable)
					.Select(x => VariableReader.ReadVar(x.file))
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

			this.HtmlPageMetaProperties = (
				Variables.TryGetValue(HtmlMetaVarName, out string fullMetaVar)
				? new ReadOnlyDictionary<string, string>(
					Enumerable.ToDictionary(
						from line in fullMetaVar.Split('\r', '\n')
						let eqIndex = line.IndexOf("=")
						where eqIndex >= 0
						let name = line.Substring(0, eqIndex)
						let value = line.Substring(eqIndex + 1)
						select (name, value),
						keySelector: x => x.name,
						elementSelector: x => x.value
					)
				)
				: new ReadOnlyDictionary<string, string>(new Dictionary<string, string>())
			);


		}
	}
}

//*/