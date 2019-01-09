using FolioWebGen.Utilities;
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
	class VariableReader
	{
		//	/// <param name="name">Case insensitive</param>
		//	public bool TryGetVarValue(string name, out string value)
		//	{
		//		return TryGetLongVarValue(name, out value) || TryGetShortVarValue(name, out value);
		//	}
		//	
		//	/// <param name="name">Case insensitive</param>
		//	public string GetVarValueOrNull(string name)
		//	{
		//		return TryGetVarValue(name, out string value) ? value : null;
		//	}
		//	
		//	private bool TryGetLongVarValue(string name, out string value)
		//	{
		//		string lookup = "$" + name + "$.var";
		//	
		//		var varFile = Variables.SingleOrDefault(
		//			v => string.Equals(v.Name, lookup, StringComparison.InvariantCultureIgnoreCase)
		//		);
		//	
		//		if (varFile == null) { value = null; return false; }
		//	
		//		value = File.ReadAllText(varFile.FullName);
		//		return true;
		//	}
		//	
		//	private bool TryGetShortVarValue(string name, out string value)
		//	{
		//		string lookup = "$" + name + "=";
		//	
		//		var varFile = Variables.SingleOrDefault(
		//			v => v.Name.StartsWith(lookup, StringComparison.InvariantCultureIgnoreCase)
		//		);
		//	
		//		if (varFile == null) { value = null; return false; }
		//	
		//		value = Path.GetFileNameWithoutExtension(varFile.Name.Substring(startIndex: lookup.Length));
		//		return true;
		//	}

		/// <summary>
		/// Only use this when necessesary. It is preferable to index all the
		/// variables in an instance of <see cref="PageVariables"/> and use that instead.
		/// This method enumerates part or all of vars and runs one or two regexes on each item.
		/// </summary>
		public static string ReadVar(IEnumerable<FileInfo> vars, string name)
		{
			if (vars == null) throw new ArgumentNullException(nameof(vars));
			if (name == null) throw new ArgumentNullException(nameof(name));

			return TryReadVar(vars, name, out string value) ? value : null;
		}

		/// <summary>
		/// Only use this when necessesary. It is preferable to index all the
		/// variables in an instance of <see cref="PageVariables"/> and use that instead.
		/// This method enumerates part or all of vars and runs one or two regexes on each item.
		/// </summary>
		public static bool TryReadVar(IEnumerable<FileInfo> vars, string name, out string value)
		{
			if (vars == null) throw new ArgumentNullException(nameof(vars));
			if (name == null) throw new ArgumentNullException(nameof(name));

			foreach (var v in vars)
			{
				if (v == null) continue;

				if (TryReadVarName(v.Name, out string vname))
				{
					if (vname == name)
					{
						return TryReadVarValue(v, out value);
					}
				}
			}

			value = null;
			return false;
		}
		
		/// <summary>
		/// Reads a variable-file (long or short).
		/// Returns (<see langword="null"/>, <see langword="null"/>) if the file is not a valid variable file.
		/// </summary>
		public static (string name, string value) ReadVar(FileInfo varFile)
		{
			if (varFile == null) throw new ArgumentNullException(nameof(varFile));

			return TryReadVar(varFile, out (string, string) result) ? result : (null, null);
		}

		public static bool TryReadVar(FileInfo varFile, out (string name, string value) result)
		{
			if (varFile == null) throw new ArgumentNullException(nameof(varFile));

			result = default; //allow use of short-circuiting && operator
			return TryReadVarName(varFile.Name, out result.name) && TryReadVarValue(varFile, out result.value);
		}

		public static bool TryReadVarName(string varFileName, out string name)
		{
			if (varFileName == null) throw new ArgumentNullException(nameof(varFileName));

			return TryReadShortVarName(varFileName, out name) || TryReadLongVarName(varFileName, out name);
		}

		public static bool TryReadVarValue(FileInfo varFile, out string value)
		{
			if (varFile == null) throw new ArgumentNullException(nameof(varFile));

			return TryReadShortVarValue(varFile.Name, out value) || TryReadLongVarValue(varFile, out value);
		}

		//	public static bool TryReadLongVar(FileInfo varFile, out (string name, string value) result)
		//	{
		//		if (varFile == null) throw new ArgumentNullException(nameof(varFile));
		//	
		//		result = default; //allow use of short-circuiting && operator
		//		return TryReadLongVarName(varFile.Name, out result.name) && TryReadLongVarValue(varFile, out result.value);
		//	}
		//	
		//	public static bool TryReadShortVar(string varFileName, out (string name, string value) result)
		//	{
		//		if (varFileName == null) throw new ArgumentNullException(nameof(varFileName));
		//	
		//		result = default; //allow use of short-circuiting && operator
		//		return TryReadShortVarName(varFileName, out result.name) && TryReadShortVarValue(varFileName, out result.value);
		//	}

		public static bool TryReadLongVarName(string varFileName, out string name)
		{
			if (varFileName == null) throw new ArgumentNullException(nameof(varFileName));

			varFileName = RemoveComments(varFileName);

			name = Regex.Match(varFileName, @"(?<=\$).+(?=\.var)").Value;
			return name != ""; //empty string if no match
		}

		public static bool TryReadLongVarValue(FileInfo varFile, out string value)
		{
			if (varFile == null) throw new ArgumentNullException(nameof(varFile));

			if (!varFile.Exists) {
				value = null;
				return false;
			}

			value = File.ReadAllText(varFile.FullName);
			return true;
		}

		public static bool TryReadShortVarName(string varFileName, out string name)
		{
			if (varFileName == null) throw new ArgumentNullException(nameof(varFileName));

			varFileName = RemoveComments(varFileName);

			name = Regex.Match(varFileName, @"(?<=\$).+(?=\$\=.+\.var)").Value;
			return name != ""; //empty string if no match
		}

		public static bool TryReadShortVarValue(string varFileName, out string value)
		{
			if (varFileName == null) throw new ArgumentNullException(nameof(varFileName));

			value = Regex.Match(varFileName, @"(?<=\$.+\$\=).+(?=\.var)").Value;
			return value != ""; //empty string if no match
		}

		/// <summary>
		/// Cuts out text surrounded by ~ or ` characters (interchangeably), but leaves the file extension intact
		/// </summary>
		public static string RemoveComments(string varFileName)
		{
			return StringUtils.RemoveComments(Path.GetFileNameWithoutExtension(varFileName)) + Path.GetExtension(varFileName);
		}
	}
}

//*/