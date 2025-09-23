🔹 Cargar y ejecutar

Se hace igual que con C#:

var assembly = FSharpScriptCompiler.CompileScript("./Scripts/PlayerScript.fs");

var scriptType = assembly.GetTypes()
    .FirstOrDefault(t => typeof(IScript).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

var instance = (IScript)Activator.CreateInstance(scriptType)!;
instance.Initialize(entity);
