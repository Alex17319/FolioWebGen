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

namespace FolioWebGen.Utilities
{
	/// <summary>
	/// Not sure why microsoft didn't make <see cref="EqualityComparer{T}"/> have a Create() method like <see cref="Comparer{T}"/> now does.
	/// Now I've gotta make this extra class.
	/// </summary>
	public class CustomEqualityComparer<T> : EqualityComparer<T>
	{
		private Func<T, T, bool> EqualsFunc;
		private Func<T, int> GetHashCodeFunc;

		public CustomEqualityComparer(Func<T, T, bool> equalsFunc, Func<T, int> getHashCodeFunc)
		{
			if (equalsFunc == null) throw new ArgumentNullException(nameof(equalsFunc));
			if (getHashCodeFunc == null) throw new ArgumentNullException(nameof(getHashCodeFunc));

			this.EqualsFunc = equalsFunc;
			this.GetHashCodeFunc = getHashCodeFunc;
		}

		public override bool Equals(T x, T y) => EqualsFunc(x, y);
		public override int GetHashCode(T obj) => GetHashCodeFunc(obj);
	}

	public static class CustomEqualityComparer
	{
		public static CustomEqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
		{
			return new CustomEqualityComparer<T>(equals, getHashCode);
		}
	}
}

//*/