using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public static class FileTypes
	{
		private static readonly IReadOnlyList<string> _commonImageExtensions = new[] {
			".bmp",
			".png", ".apng",
			".jpg", ".jpeg", ".jif", ".jfif", ".jp2", ".jpx", ".j2k", ".j2c",
			".gif",
			".svg",
			".ico",
			".webp",
			".tif", ".tiff",
			".ani",
			".wmf", ".emf",
			".tga",
			".pcd",
			".fpx",
			".psd",
		};
		public static bool IsImage(string extension)
		{
			return _commonImageExtensions.Any(x => extension.Equals(x, StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsRawTextDocument(string extension)
		{
			return extension.Equals(".txt", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsPdf(string extension)
		{
			return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
		}

		private static readonly IReadOnlyList<string> _externalPageExtensions = new[] {
			".html",
			".mhtml",
		};
		public static bool IsExternalPage(string extension)
		{
			return _externalPageExtensions.Any(x => extension.Equals(x, StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsHtmlSnippet(string extension)
		{
			return extension.Equals(".htmlsnip", StringComparison.OrdinalIgnoreCase);
		}
	}
}
