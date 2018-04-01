﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.Utilities
{
	public static class FileUtils
	{

		#region Get Relative Path
		//By Muhammad Rehan Saeed
		//Source: https://stackoverflow.com/a/32113484/4149474

		/// <summary>
		/// Creates a relative path from one file or folder to another.
		/// </summary>
		/// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
		/// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
		/// <returns>The relative path from the start directory to the end path.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
		/// <exception cref="UriFormatException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static string GetRelativePath(string fromPath, string toPath)
		{
			if (string.IsNullOrEmpty(fromPath))
			{
				throw new ArgumentNullException("fromPath");
			}

			if (string.IsNullOrEmpty(toPath))
			{
				throw new ArgumentNullException("toPath");
			}

			Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
			Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

			if (fromUri.Scheme != toUri.Scheme)
			{
				return toPath;
			}

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
			{
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}

		private static string AppendDirectorySeparatorChar(string path)
		{
			// Append a slash only if the path is a directory and does not have a slash.
			if (!Path.HasExtension(path) &&
				!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				return path + Path.DirectorySeparatorChar;
			}

			return path;
		}
		#endregion


		public static string GetNameWithoutExt(this FileInfo file)
		{
			return Path.GetFileNameWithoutExtension(file.Name);
		}


		public static string GetPathWithoutExtension(string path)
		{
			return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
		}


		public static string GetPathWithoutExtension(this FileInfo file)
		{
			return Path.Combine(file.DirectoryName, file.GetNameWithoutExt());
		}
	}
}
