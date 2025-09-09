using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text.Json.Nodes;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Escriba lo que quiere reemplazar");
        string textoAReemplazar = Console.ReadLine() ?? "";

        Console.WriteLine("Escriba con que lo reemplazara");
        string reemplazo = Console.ReadLine() ?? "";

        string filePath = args[0];

        string originalContent = File.ReadAllText(filePath);
        string editContent;
        editContent = originalContent.Replace(textoAReemplazar, reemplazo);

        File.WriteAllText(filePath, editContent);

        ProcessStartInfo processStart = new()
        {
            FileName = filePath,
            CreateNoWindow = true,
            UseShellExecute = true
        };

        Process.Start(processStart);


    }
}
