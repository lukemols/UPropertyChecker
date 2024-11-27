using System.Text.RegularExpressions;

namespace UpropertyChecker
{
	public class PropertyChecker
	{
		private OutputPrinter OutputPrinter;

		public void StartPropertyCheck()
		{
			OutputPrinter = new OutputPrinter();

			string welcomeMessage = "Welcome in UPropertyChecker." + Environment.NewLine +
				"This application is meant to find missing UPROPERTY macros in source code of projects made with Unreal Engine." + Environment.NewLine +
				"Only .h files will be checked. *.generated.h files will not be considered." + Environment.NewLine +
				"Expect potential false positive while running the program." + Environment.NewLine + Environment.NewLine +
				"Write or paste folder path to run the program. Leave empty to use sample file and test the application.";

			Console.WriteLine(welcomeMessage);
			string path = Console.ReadLine();

			bool isSample = String.IsNullOrWhiteSpace(path);

			if (isSample)
			{
				path = AppContext.BaseDirectory;
			}

			DateTime start = DateTime.Now;
			if (Directory.Exists(path))
			{
				OutputPrinter.WriteHeader(path);

				// enumerate .h files but remove *.generated.h
				List<string> ext = OutputPrinter.ConfigData.FileExtensions;
				List<string> foldersToAvoid = OutputPrinter.ConfigData.FoldersToAvoid;
				string generatedFilesExt = "generated.h";

				List<string> myFiles = Directory
					.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
					.Where(s =>	
								ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant())
								&& !s.EndsWith(generatedFilesExt)
								&& !foldersToAvoid.Any(s.Contains))
					.ToList();

				OutputPrinter.WriteFileCount(myFiles.Count());

				int count = 0;
				int filesWithErrors = 0;
				foreach (string file in myFiles)
				{
					if (!ValidateFile(file, isSample, path))
					{
						filesWithErrors++;
					}

					count++;
					if (count % 10 == 0)
					{
						float percent = (float)count / (float)myFiles.Count();
						Console.WriteLine($"Examinated {count} files out of {myFiles.Count()} ({percent.ToString("P2")}).");
					}
				}
				OutputPrinter.WriteErrorCount(filesWithErrors, myFiles.Count());
			}
			else
			{
				Console.WriteLine($"No valid folder found, ending check routine.", true);
			}
			OutputPrinter.WriteFile();
			TimeSpan ts = DateTime.Now - start;
			Console.WriteLine($"Check routine executed in {ts.ToString()}. Press ENTER to exit program.");
			Console.ReadLine();
		}

		private bool ValidateFile(string filePath, bool isSample, string startFolder)
		{
			string data = File.ReadAllText(filePath);
			string commentPattern = "//.*|/\\*[\\s\\S]*?\\*/|(\"(\\\\.|[^\"])*\")";
			string propertyPattern = "UPROPERTY+\\([^\\)]*\\){1,}(\\.[^\\)]*\\))?";
			string functionPattern = "[a-z_A-Z]+\\([^\\)]*\\)(\\.[^\\)]*\\))?";

			// replace all comments
			data = DoRegex(data, commentPattern);
			// shorten uproperties
			data = DoRegex(data, propertyPattern, "UPROPERTY");
			// shorten all the functions
			data = DoRegex(data, functionPattern, "UFUNCTION");
			// do additional regex expressions
			foreach (RegexExpr regex in OutputPrinter.ConfigData.AdditionalRegexExpressions)
			{
				data = DoRegex(data, regex.Expression, regex.Replacement);
			}
			// remove tabs
			data = data.Replace("\t", "");


			List<string> lines = data.Split(';').ToList();
			lines.RemoveAll(s => String.IsNullOrWhiteSpace(s));

			List<string> errors = new List<string>();
			foreach (string line in lines)
			{
				if (line.Contains("UFUNCTION"))
				{
					continue;
				}

				if (line.Contains('*') && !line.Contains("UPROPERTY"))
				{
					errors.Add(line.Trim());
				}
			}

			bool hasErrors = errors.Count > 0;
			if (hasErrors)
			{
				Entry newEntry = new Entry(filePath, startFolder);
				newEntry.Errors = errors;
				OutputPrinter.WriteErrorFile(newEntry);
			}

			if (isSample)
			{
				string outputPath = Path.Combine(AppContext.BaseDirectory, "out.txt");
				File.WriteAllLines(outputPath, lines);
			}

			return !hasErrors;
		}

		private string DoRegex(string input, string pattern, string replacement = "")
		{
			return Regex.Replace(input, pattern, replacement);
		}
	}
}