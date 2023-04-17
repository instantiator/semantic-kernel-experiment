using System;
namespace SKE
{
    public static class DotEnv
    {
        public static void Load(string path)
        {
            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    Environment.SetEnvironmentVariable(parts[0], parts[1]);
                }
            }
        }
    }
}

