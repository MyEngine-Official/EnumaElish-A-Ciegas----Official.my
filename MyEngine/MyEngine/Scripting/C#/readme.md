🔹 Uso en el motor

Ejemplo de cómo unir todo dentro de MonoGame:

// Crear la entidad
var entity = new MainEntity(1);

// Cargar script desde archivo
var script = ScriptLoader.LoadScriptInstance("./Scripts/PlayerScript.cs", entity);

// Guardar script en una lista para ejecutarlo cada frame
_scripts.Add(script);


En tu Game.Update(GameTime gameTime):
foreach (var script in _scripts)
{
    script.Update(gameTime, entity); // ejecuta la lógica del script
}

Y cuando ya no lo necesites:
script.Cleanup();
