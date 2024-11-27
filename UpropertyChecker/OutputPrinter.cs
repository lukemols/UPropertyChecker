using System.Text.Json;

namespace UpropertyChecker
{
    internal class OutputPrinter
    {
        private FileOutput FileOutput;
        private string OutputFilename;

        public Config ConfigData { get; private set; }

        public OutputPrinter()
        {
            LoadOrCreateConfig();
			StartPrinter();
        }

        private void LoadOrCreateConfig()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "UPC_Config.json");

            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
				Config? newConfig = JsonSerializer.Deserialize<Config>(content);
                ConfigData = newConfig ?? new Config();
            }
            else
            {
                ConfigData = new Config();
                ConfigData.SetDefaults();
				var options = new JsonSerializerOptions { WriteIndented = true };
				var jsonString = JsonSerializer.Serialize(ConfigData, options);
                File.WriteAllText(path, jsonString);
            }
            // ensure every folder to exclude has a correct path
            ConfigData.SanitizeFolders();
        }

        private void StartPrinter()
        {
            OutputFilename = "UPC_Out_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
            FileOutput = new FileOutput();
        }

        public void WriteFile()
        {
			var options = new JsonSerializerOptions { WriteIndented = true };
			var jsonString = JsonSerializer.Serialize(FileOutput, options);
            File.WriteAllText(OutputFilename, jsonString);
        }

        public void WriteHeader(string baseFolder)
        {
            FileOutput.Start(baseFolder);
            Console.WriteLine($"UProperty Checker started in folder: {baseFolder}");
        }

        public void WriteFileCount(int FileCount)
        {
            FileOutput.SetFileCount(FileCount);
            Console.WriteLine($"Found {FileCount} files in folder. Starting check routine.");
        }

        public void WriteErrorCount(int Errors, int TotalFiles)
        {
            FileOutput.SetFileWithErrors(Errors);
            float percent = (float)Errors / TotalFiles;
            Console.WriteLine($"Found {Errors} errors in {TotalFiles} files ({percent.ToString("P1")}% of files). Starting check routine.");
        }

        public void WriteErrorFile(Entry error)
        {
            FileOutput.AddError(error);

            Console.WriteLine(Environment.NewLine +
                              $"Found {error.Errors.Count()} errors in {error.FileName} (path: {error.FilePath})."
                              + Environment.NewLine);
        }
    }
}