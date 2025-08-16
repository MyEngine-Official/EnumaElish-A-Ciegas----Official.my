using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static int Main(string[] args)
    {
        // Accept first arg as root path, otherwise default to parent directory (repo root)
        string root;
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            root = args[0];
        }
        else
        {
            // assume scripts is in repo root\scripts, so parent is repo root
            var cwd = Directory.GetCurrentDirectory();
            root = Path.GetFullPath(Path.Combine(cwd, ".."));
        }

        if (!Directory.Exists(root))
        {
            Console.Error.WriteLine($"Root path no encontrado: '{root}'");
            return 2;
        }

        const string readmeName = "README.md";

        // Get all directories under root (include root itself)
        var dirs = new List<string> { root };
        try
        {
            dirs.AddRange(Directory.GetDirectories(root, "*", SearchOption.AllDirectories));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error enumerando carpetas: {ex.Message}");
            return 3;
        }

        // Ensure uniqueness and order
        dirs = dirs.Distinct().OrderBy(d => d).ToList();

        // Filter: keep only directories that are inside the provided root and exclude common build/git folders
        string rootFull = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var excludeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".git", "bin", "obj", ".vs" };

        dirs = dirs.Where(d =>
        {
            var full = Path.GetFullPath(d);
            // must start with root path
            if (!full.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase)) return false;
            // split into segments and exclude if any segment matches an excluded name
            var parts = full.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (excludeNames.Contains(part)) return false;
            }
            return true;
        }).ToList();

        bool anyMissing = false;

        foreach (var dir in dirs)
        {
            var readmePath = Path.Combine(dir, readmeName);
            if (File.Exists(readmePath))
            {
                Console.WriteLine($"OK     : {readmePath}");
                Console.WriteLine(new string('-', 37));
                try
                {
                    string text = File.ReadAllText(readmePath);
                    Console.WriteLine(text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error leyendo archivo: {ex.Message}]");
                }
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"MISSING: {readmePath}");
                Console.ResetColor();
                anyMissing = true;
            }
        }

        return anyMissing ? 1 : 0;
    }
}
