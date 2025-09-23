using MyEngine.MyCore.MyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.Scripting
{
    public static class ScriptLoader
    {
        public static IScript LoadScriptInstance(string scriptPath, MainEntity entity)
        {
            var assembly = ScriptCompiler.CompileScript(scriptPath);

            var scriptType = assembly.GetTypes()
             .FirstOrDefault(t => typeof(IScript).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if(scriptType == null)
                throw new Exception($"No se encontró ninguna clase que implemente IScript en {scriptPath}");

            var instance = (IScript)Activator.CreateInstance(scriptType)!;

            // Inicializamos el script con la entidad asignada
            instance.Initialize(entity);

            return instance;
        }
    }
}
