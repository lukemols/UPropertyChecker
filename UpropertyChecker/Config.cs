using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UpropertyChecker
{
	internal class RegexExpr
	{
		[JsonInclude]
		public string Expression { get; private set; }
		[JsonInclude]
		public string Replacement { get; private set; }
	}

	internal class Config
	{
		[JsonInclude]
		public List<string> FoldersToAvoid { get; private set; }
		[JsonInclude]
		public List<string> FileExtensions { get; private set; }
		[JsonInclude]
		public List<RegexExpr> AdditionalRegexExpressions { get; private set; }

		public void SetDefaults()
		{
			FoldersToAvoid = new List<string> { "Intermediate" };
			FileExtensions = new List<string> { "h" };
			AdditionalRegexExpressions = new List<RegexExpr>();
		}

		public void SanitizeFolders()
		{
			for (int i = 0; i < FoldersToAvoid.Count; i++)
			{
				string[] Subfolders = FoldersToAvoid[i].Split('/', '\\');
				if (Subfolders.Length > 1)
				{
					FoldersToAvoid[i] = Path.Combine(Subfolders);
				}
			}
		}
	}
}
