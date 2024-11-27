using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpropertyChecker
{
	internal class FileOutput
	{
		public string StartFolder { get; private set; }
		public string FileCount { get; private set; }
		public string FileWithErrors { get; private set; }
		public List<Entry> Entries { get; private set; }

		public void Start(string InFolder)
		{
			StartFolder = InFolder;
			Entries = new List<Entry>();
		}

		public void SetFileCount(int Count)
		{
			FileCount = Count.ToString();
		}
		
		public void SetFileWithErrors(int Count)
		{
			FileWithErrors = Count.ToString();
		}

		public void AddError(Entry error)
		{
			Entry copy = new Entry(error.FilePath);
			copy.RelativePath = error.RelativePath;
			copy.Errors = error.Errors;
			Entries.Add(copy);
		}
	}
}
