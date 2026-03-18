public class CSVHelper
{
    private static readonly object FileLock = new object();

    public static List<string[]> ReadCSV(string filePath)
    {
        lock (FileLock)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{filePath}' was not found.");
            }

            var lines = File.ReadAllLines(filePath);

            List<string[]> result = new List<string[]>();
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                result.Add(lines[i].Split(','));
            }

            return result;
        }
    }

    public static void WriteCSV(string filePath, List<string[]> data)
    {
        lock (FileLock)
        {
            var lines = data.Select(fields => string.Join(",", fields)).ToArray();
            File.WriteAllLines(filePath, lines);
        }
    }
}

