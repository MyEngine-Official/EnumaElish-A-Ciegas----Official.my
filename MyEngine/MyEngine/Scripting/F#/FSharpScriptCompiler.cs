using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Text;
using Microsoft.FSharp.Control;
using System;
using System.IO;
using System.Reflection;


namespace MyEngine.Scripting.F_
{
    public class FSharpScriptCompiler
    {
        public static Assembly CompileScript(string scriptPath)
        {
            string code = File.ReadAllText(scriptPath);
            var checker = FSharpChecker.Create(null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            // Obtener las opciones del proyecto desde el script (es un FSharpAsync, así que hay que esperar el resultado)
            var projectOptionsAsync = checker.GetProjectOptionsFromScript(
                scriptPath,
                SourceText.ofString(code),
                null, null, null, null, null, null,
                assumeDotNetFramework: false,
                null, null, null
            );

            // Esperar el resultado de la tarea asíncrona
            var projectOptionsResult = FSharpAsync.RunSynchronously(projectOptionsAsync, null, null);
            var projectOptions = projectOptionsResult.Item1;

            // Ahora sí, llamar a ParseAndCheckFileInProject con los argumentos correctos
            var parseAndCheckAsync = checker.ParseAndCheckFileInProject(
                scriptPath,
                0, // fileVersion
                SourceText.ofString(code),
                projectOptions,
                null
            );

            var parseAndCheckResult = FSharpAsync.RunSynchronously(parseAndCheckAsync, null, null);

            if (parseAndCheckResult.Item1.ParseHadErrors)
            {
                var errors = string.Join("\n", parseAndCheckResult.Item1.Diagnostics.ToString());
                throw new Exception($"Errores compilando script F#:\n{errors}");
            }

            try
            {
                //emitir el ensamblado
                var outPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".dll");
                var errorsAndExitCode = checker.Compile(new[] { "fsc.exe", "-a", scriptPath, "-o", outPath }, null);

                return Assembly.LoadFile(outPath);

            }catch(Exception ex)
            {
                throw new Exception($"Errores compilando script F#:\n{ex}");
            }
        }
    }
}
