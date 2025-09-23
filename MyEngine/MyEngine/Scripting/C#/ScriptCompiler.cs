using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.Scripting
{
    public static class ScriptCompiler
    {
        public static Assembly CompileScript(string scriptPath)
        {
            // Parseamos el código fuente
            string code = File.ReadAllText(scriptPath);

            //parseamos el codigo fuente
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            //Referencias necesarias (ensamblados ya cargados en el dominio actual)
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location));

            // configuracion de la compilacion
            var compilacion = CSharpCompilation.Create(
                assemblyName: Path.GetRandomFileName(), //nombre unico
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

            using var ms = new MemoryStream();
            var result = compilacion.Emit(ms);

            if (!result.Success)
            {
                var errors = string.Join("\n", result.Diagnostics);
                throw new Exception($"Errores compilando script:\n{errors}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }

    }
}
