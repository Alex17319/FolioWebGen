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
		public ReadOnlyCollection<SingleFormatFile> Files { get; }
		/// <summary>
		/// The file name (without an extension) that is the same for all different-format files
		/// in the multi-format-file.
		/// </summary>
		public string ExtensionlessFileName { get; }

		public MultiFormatFile(IEnumerable<SingleFormatFile> files)
		{
			if (files == null) throw new ArgumentNullException(nameof(files));

			this.Files = files.ToList().AsReadOnly();
			if (this.Files.Count == 0) throw new ArgumentException("Cannot be empty.", nameof(files));

			//In order to sort files in the same order as when they are created in explorer (i.e. in order
			//to pay attention to files starting with "~1~", "~2~", etc), the original file names need
			//to be used for sorting. In order to make this as predictable and reasonable as possible,
			//this means that for all files in a MultiFormatFile, the original file names must be the
			//same (not just the display names as I'd previously intended).
			//Additionally, the filename has to be kept until the Page/PageSections are created,
			//so combined with the above new sorting/duplication rule that means that the display
			//names don't need to be generated until then.

			this.ExtensionlessFileName = Path.GetFileNameWithoutExtension(this.Files[0].FileName);

			//All just error checking
			List<string> differentFileNames = null; //Avoid allocating if everything's valid
			for (var i = 1; i < this.Files.Count; i++)
			{
				if (Path.GetFileNameWithoutExtension(this.Files[i].FileName) != this.ExtensionlessFileName)
				{
					differentFileNames = differentFileNames ?? new List<string>();
					differentFileNames.Add(this.Files[i].FileName);
				}
			}
			if (differentFileNames != null) throw new ArgumentException(
				$"The first file has file name '{this.Files[0].FileName}' " //don't use this.ExtensionlessFileName as the extension is useful information
				+ "but some other files have different file names: "
				+ string.Join(", ", differentFileNames)
				+ "."
			);
		}
	}
}
