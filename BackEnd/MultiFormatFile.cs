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
		/// The file display name, without an extension, that applies to all contained files.
		/// '~' characters (and anything else similar) are already handled.
		/// </summary>
		public string DisplayName { get; }

		public MultiFormatFile(IEnumerable<SingleFormatFile> files)
		{
			if (files == null) throw new ArgumentNullException(nameof(files));

			this.Files = files.ToList().AsReadOnly();
			if (this.Files.Count == 0) throw new ArgumentException("Cannot be empty.", nameof(files));

			this.DisplayName = this.Files[0].DisplayName;

			//All just error checking
			List<string> differentDisplayNames = null; //Avoid allocating if everything's valid
			for (var i = 1; i < this.Files.Count; i++)
			{
				if (this.Files[i].DisplayName != DisplayName)
				{
					differentDisplayNames = differentDisplayNames ?? null;
					differentDisplayNames.Add(this.Files[i].DisplayName);
				}
			}
			if (differentDisplayNames != null) throw new ArgumentException(
				$"The first file has display name '{this.DisplayName}' "
				+ "but some other files have different display names: "
				+ string.Join(", ", differentDisplayNames)
				+ "."
			);
		}
	}
}
