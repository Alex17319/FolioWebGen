using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioWebGen.BackEnd
{
	public class Website
	{
		public string SiteName { get; }
		public Page Root { get; }
		public ExternalContentReg ExtReg { get; }

		public Website(string siteName, Page root, ExternalContentReg extReg)
		{
			this.SiteName = siteName ?? throw new ArgumentNullException(nameof(siteName));
			this.Root = root ?? throw new ArgumentNullException(nameof(root));
			this.ExtReg = extReg ?? throw new ArgumentNullException(nameof(extReg));
		}

		//TODO: Add (asynchronous?) functions to convert this into all the files, folders,
		//cloud stuff etc needed for the website

		public async Task WriteToDirectory(DirectoryInfo dir)
		{

		}
	}
}
