using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class MultiFormatFile
	{
		public ReadOnlyCollection<FileInfo> Files { get; }
		/// <summary>
		/// The file name, without an extension, that applies to all contained files.
		/// Should have '~' characters (and anything else similar) handled already.
		/// </summary>
		public string Name { get; }

		//Can't ensure exact path equality as '~' characters (and enclosed strings) are removed before
		//names are compared to check for alternative formats
		//	public MultiFormatFile(IEnumerable<FileInfo> files)
		//	{
		//		if (files == null) throw new ArgumentNullException(nameof(files));
		//		if (!files.Any()) throw new ArgumentException("Cannot be empty.", nameof(files));
		//	
		//		string firstPath = files.First().GetPathWithoutExtension();
		//	
		//		if (!files.Skip(1).All(f => f.GetPathWithoutExtension() == firstPath)) throw new ArgumentException(
		//			message: "Not all files have the same path (ignoring extensions) "
		//			+ "as that of the first file '" + firstPath + "'.",
		//			paramName: nameof(files)
		//		);
		//	
		//		this.Files = files.ToList().AsReadOnly();
		//		this.CommonPath = firstPath;
		//		this.CommonName = Path.GetFileName(firstPath);
		//	}

		public MultiFormatFile(string name, IEnumerable<FileInfo> files)
		{
			if (files == null) throw new ArgumentNullException(nameof(files));
			if (!files.Any()) throw new ArgumentException("Cannot be empty.", nameof(files));

			this.Files = files.ToList().AsReadOnly();
			this.Name = name;
		}
	}
}
