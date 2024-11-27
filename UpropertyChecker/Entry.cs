namespace UpropertyChecker;

public class Entry
{
    public string FileName { get; private set; }
    public string RelativePath { get; set; }
    public string FilePath { get; private set; }
    private List<string> errors;
    public List<string> Errors
    {
        get { return errors; }
        set {
            foreach (string entry in value)
            {
                errors.Add(entry);
            }
        }
    }

    public Entry(string Path) : this(Path, "")
    {
    }

    public Entry(string Path, string StartFolder)
    {
        FilePath = Path;
        RelativePath = Path.Remove(0, StartFolder.Length);
        ComputeNameFromPath();
        errors = new List<string>();
    }

    private void ComputeNameFromPath()
    {
        FileInfo info = new FileInfo(FilePath);
        FileName = info.Name;
    }
}