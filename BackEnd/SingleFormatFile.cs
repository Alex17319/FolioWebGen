using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FolioWebGen.Utilities;

namespace FolioWebGen.BackEnd
{
	public class SingleFormatFile
	{
		/// <summary>The full file name, including the extension</summary>
		public string FileName { get; }
		public string FileNameWithoutExtension { get; }
		public string DisplayName { get; }

		public FileInfo FileInfo { get; }

		public string Extension => FileInfo.Extension;
		
		/// <summary>
		/// The path, including the extension (but not the absolute path
		/// returned by <see cref="Path.GetFullPath(string)"/>
		/// </summary>
		public string Path => FileInfo.FullName;

		public SingleFormatFile(FileInfo fileInfo)
		{
			this.FileInfo = fileInfo;
			this.FileName = fileInfo.Name;
			this.FileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name);
			this.DisplayName = StringUtils.GetItemDisplayName(fileInfo.Name);
		}
	}
}

//*/