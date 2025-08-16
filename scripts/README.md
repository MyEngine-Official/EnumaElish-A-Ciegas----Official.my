# Scripts — Verificación de README

Este archivo contiene comandos y ejemplos para ejecutar las comprobaciones automáticas de README del repositorio.

Requisitos

- Windows PowerShell (ej. PowerShell 5.1 o PowerShell 7+).
- .NET SDK 7.0+ instalado para ejecutar el programa `ReadmeChecker`.

1) Ejecutar el verificador PowerShell (analiza `EnumaElishStructure.html`)

Desde la raíz del repositorio (donde está `EnumaElishStructure.html`):

```powershell
# Ejecuta el script que analiza el HTML y comprueba READMEs según la estructura declarada
powershell -NoProfile -ExecutionPolicy Bypass -File ".\scripts\check_readmes.ps1" -HtmlPath ".\EnumaElishStructure.html" -Root ".\GamePlatform"
```

Salida esperada:
- Código 0: todos los README.md esperados están presentes.
- Código 1: faltan README.md (se listarán).
- Código 2: no se encontró el archivo HTML proporcionado.

2) Ejecutar el comprobador .NET (imprime el contenido de cada README)

Desde la carpeta `scripts` o usando la ruta del proyecto:

```powershell
# Desde la carpeta scripts
cd .\scripts
# Ejecuta y analiza todo el repositorio (pasa la ruta absoluta del root si prefieres)
dotnet run --project .\ReadmeChecker.csproj "C:\Users\Usuario\Desktop\EnumaElish_A CIEGAS"

# O analiza sólo GamePlatform (más habitual para este repo)
dotnet run --project .\ReadmeChecker.csproj "C:\Users\Usuario\Desktop\EnumaElish_A CIEGAS\GamePlatform"
```

Notas importantes
- Pasa siempre la ruta raíz que quieras comprobar para evitar que el programa escanee fuera del repositorio.
- El programa excluye carpetas comunes de build/IDE (`.git`, `bin`, `obj`, `.vs`).
- Si obtienes errores sobre ExecutionPolicy, ejecuta PowerShell como administrador y ajusta la política temporalmente o ejecuta con `-ExecutionPolicy Bypass` como en los ejemplos.

Si quieres que el script cree automáticamente README faltantes (con plantilla), puedo añadir esa opción o crear los archivos faltantes ahora.
