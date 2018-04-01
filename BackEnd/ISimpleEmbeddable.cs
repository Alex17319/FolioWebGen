using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public interface ISimpleEmbeddable : IHtmlPrintable
	{
		FileInfo SourcePath { get; }
	}

	public abstract class SimpleEmbeddable<TSelf> : ISimpleEmbeddable
		where TSelf : SimpleEmbeddable<TSelf>
	{
		public FileInfo SourcePath { get; }

		public SimpleEmbeddable(FileInfo sourcePath)
		{
			this.SourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
		}

		public abstract object ToHtml(ExternalContentReg extReg);

		/// <summary>
		/// True if <paramref name="obj"/> is a <see cref="SimpleEmbeddable{TSelf}"/>
		/// and it's SourcePath is equivalent to the current instances's SourcePath.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return obj is SimpleEmbeddable<TSelf> typedObj && Equals(this, typedObj);
		}

		/// <summary>
		/// True if the SourcePaths are equivalent.
		/// </summary>
		public static bool Equals(SimpleEmbeddable<TSelf> a, SimpleEmbeddable<TSelf> b)
		{
			return a.SourcePath.FullName == b.SourcePath.FullName
				|| Path.GetFullPath(a.SourcePath.FullName) == Path.GetFullPath(b.SourcePath.FullName);
			//Idk if the second part is actually needed or if the first part does the same thing
		}

		public override int GetHashCode()
		{
			return Path.GetFullPath(SourcePath.FullName).GetHashCode(); //Idk if GetFullPath is needed
		}

		public static bool operator ==(SimpleEmbeddable<TSelf> a, SimpleEmbeddable<TSelf> b) => Equals(a, b);
		public static bool operator !=(SimpleEmbeddable<TSelf> a, SimpleEmbeddable<TSelf> b) => !(a == b);
	}
}
