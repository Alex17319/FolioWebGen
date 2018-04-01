using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class SimpleEmbedReg<T>
		where T : ISimpleEmbeddable
	{
		public DirectoryInfo SourceRoot { get; }
		public string OnlineRoot { get; }

		private readonly HashSet<T> _items;
		public IEnumerable<T> Items { get { return _items; } }

		public SimpleEmbedReg(DirectoryInfo sourceRoot, string onlineRoot, IEqualityComparer<T> comparer = null)
		{
			this.SourceRoot = sourceRoot ?? throw new ArgumentNullException(nameof(sourceRoot));
			this.OnlineRoot = onlineRoot ?? throw new ArgumentNullException(nameof(onlineRoot));

			this._items = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
		}

		public void Register(T item)
		{
			_items.Add(item);
		}
		public void Register(IEnumerable<T> items) { foreach (var item in items) Register(item); }
		public void Register(params T[] items) { Register(items.AsEnumerable()); }

		public string GetPath(T item)
		{
			string relativePath = FileUtils.GetRelativePath(
				fromPath: SourceRoot.FullName,
				toPath: item.SourcePath.FullName
			);

			string fileName = Path.Combine(
				Regex.Replace(
					Regex.Replace(
						Path.GetDirectoryName(relativePath) ?? "",
						@"[^a-zA-Z0-9\+\&\#\'\.\_\/\\-]",
						""
					),
					@"[\/\\]",
					";"
				),
				Path.GetFileName(relativePath)
			);

			const int maxFileNameLength = 100; //100 characters should be safe (full path safe limit ~= 250)
			if (fileName.Length > maxFileNameLength)
			{
				//Take the first 17 characters and last 80 characters, and join them with an ellipsis
				fileName = (
					fileName.Substring(0, length: 17)
					+ "..."
					+ fileName.Substring(startIndex: fileName.Length - 80)
				);
			}

			return Path.Combine("Resources", "Images", fileName);
		}
	}
}
