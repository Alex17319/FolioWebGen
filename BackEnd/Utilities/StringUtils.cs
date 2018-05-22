using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolioWebGen.Utilities
{
	public static class StringUtils
	{
		/// <summary>
		/// Removes all substrings that are enclosed in (or prefixed with) the specified characters.
		/// For example, RemoveEnclosedSubstrings("ab~cd`ef~gh", '~', '`') returns "abef"
		/// </summary>
		public static string RemoveEnclosedSubstrings(string source, params char[] delimiters)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			List<char> dest = new List<char>();

			bool isIncluding = true;
			for (int i = 0; i < source.Length; i++)
			{
				if (Array.IndexOf(delimiters, source[i]) >= 0) {
					isIncluding = !isIncluding;
					continue;
				}

				if (isIncluding) dest.Add(source[i]);
			}

			return string.Concat(dest);
		}

		private static Regex _orderByNatural_DigitRegex = new Regex(@"\d+", RegexOptions.Compiled);

		//Adapted from https://stackoverflow.com/a/22323356/4149474 by Michael Parker
		public static IEnumerable<T> OrderByNatural<T>(this IEnumerable<T> items, Func<T, string> selector, StringComparer stringComparer = null)
		{
			int maxDigits = items.SelectMany(
				item => _orderByNatural_DigitRegex.Matches(selector(item)).Cast<Match>()
				.Select(digitChunk => (int?)digitChunk.Value.Length)
			).Max() ?? 0;

			return items.OrderBy(
				item => _orderByNatural_DigitRegex.Replace(
					selector(item),
					match => match.Value.PadLeft(maxDigits, '0')
				),
				stringComparer ?? StringComparer.CurrentCulture
			);
		}

		public static string GetItemUrlName(string displayName)
		{
			return Regex.Replace(displayName, @"([^\w\d\+\'\,\.-]|\s|_)+", "-");
		}

		//Idk why I was trying to expand camelCase or similar into
		//text with spaces, rather than just taking text with spaces (which
		//can easily be used instead) and then replacing the spaces with dashes
		//	public static string GetItemDisplayName(string extensionlessFileName)
		//	{
		//		//Test Case:
		//		//"thisIsATest!willItWork!?!?IDK,maybe;it probably won't(thingsUsuallyHaveBugs&fail:)).ItMightJustBeAVeryDifficultProblem-RequiringDictionaryUse/AI.IBet$0ThatItWILLWork.Anyone Want To Bet More?Ok,It won't work-but ifItPartiallyWorksThat'sGood Enough!!"
		//		//Result:
		//		//"This Is ATest! Will It Work!?!? IDK, Maybe; It Probably Won't (Things Usually Have Bugs & Fail:)). It Might Just Be AVery Difficult Problem - Requiring Dictionary Use/AI. IBet $0That It WILLWork. Anyone Want To Bet More? Ok, It Won't Work - But If It Partially Works That's Good Enough!!"
		//		//This is reasonable, and not much more can be expected
		//	
		//		string title = extensionlessFileName;
		//	
		//		//Replace duplicate & non-spacebar whitespace characters with normal space characters
		//		title = Regex.Replace(title, @"\s+", " ");
		//	
		//		//Insert space between lowercase and uppercase letters
		//		title = Regex.Replace(title, @"(\p{Ll})(\p{Lu})", @"$1 $2");
		//	
		//		const string spaceSurroundedChars = @"\&\+\=\*\>\<\|\-"; //Hyphen must be last (AFAIK)
		//		const string spacePrependedChars = @"\(\[\{\$\#" + spaceSurroundedChars;
		//		const string spaceAppendedChars = @"\)\]\}\!\?\%\,\.\:\;" + spaceSurroundedChars;
		//	
		//		//Add spaces around special characters as appropriate
		//		title = Regex.Replace(title, $@"(\w)([{spacePrependedChars}])", @"$1 $2");
		//		title = Regex.Replace(title, $@"([{spaceAppendedChars}])(\w)", @"$1 $2");
		//	
		//		//	//Insert spaces around some punctuation characters (or consecutive occurences of those characters)
		//		//	title = Regex.Replace(title, @"(\w|[])([\&\-\+\=\*\>\<]+)(\w)", @"$1 $2 $3");
		//		//	
		//		//	//Insert spaces before some punctuation characters (or consecutive occurences of those characters)
		//		//	title = Regex.Replace(title, @"(\w)([\(\[\{\$\#]+)(\w)", @"$1 $2$3");
		//		//	
		//		//	//Insert spaces after some punctuation characters (or consecutive occurences of those characters)
		//		//	title = Regex.Replace(title, @"(\w)([\)\]\}\!\?\%\,\.\:\;]+)(\w)", @"$1 $2$3");
		//	
		//		//When these characters are followed by a letter, it is ok to capitalise that letter
		//		const string titleCaseResetCharacters = @" \~\!\#\$\%\^\&\*\(\)\-_\=\+\[\{\]\}\\\|\;\:""\,\<\.\>\/\?";
		//		//Note that @ and ' are not included here
		//		//Neither are numbers, as they can be followed by measuremnt units
		//		//Note: _ does not need a backslash (and adding one causes an error)
		//	
		//		//Capitalise letters after spaces, punctuation, and other symbols
		//		title = Regex.Replace(
		//			title,
		//			@"(^|[" + titleCaseResetCharacters + @"}])\p{Ll}", //Start of string or title case reset char, followed by lowercase char
		//			m => m.Value.ToUpper()
		//		);
		//	
		//		return title;
		//	}
	}
}
