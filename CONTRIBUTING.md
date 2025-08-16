# Contribuir a EnumaElish: A Ciegas

Este documento define las normas y el flujo de trabajo para contribuir al proyecto. Está pensado para mantener la calidad, coherencia de arquitectura y facilitar las revisiones.

## Índice

- Propósito
- Flujo de ramas y Pull Requests
- Mensajes de commit
- Revisión de código y checklist de PR
- Estándares de código y Clean Architecture
- Tests y CI
- Requisitos para nuevas carpetas/proyectos
- Uso del comprobador automático de README

## Propósito

Asegurar que las contribuciones sean consistentes, revisables y que respeten las decisiones arquitectónicas del proyecto (Clean Architecture).

## Flujo de ramas

- Rama principal: `main` o `master` (protegida).
- Trabaja en ramas temáticas: `feature/<breve-descripción>`, `fix/<id>-<breve>`, `chore/<tarea>`.
- Rebase o merge squash antes de abrir PR para mantener historial claro.

## Pull Requests (PR)

- Un PR debe tener una descripción clara del cambio, motivos y pasos para probar localmente.
- Etiqueta el PR con el tipo (feature, bug, docs, infra).
- Asigna revisores apropiados.
- Incluye referencias a issues si aplica.

## Mensajes de commit

Usar mensajes claros y convencionales. Formato sugerido (imperativo):

- `feat: añadir XYZ` — nueva funcionalidad
- `fix: corregir bug en ABC`
- `docs: actualizar README sobre D`
- `chore: actualizar dependencias`

Incluye en el cuerpo del commit detalles importantes si es necesario.

## Revisión de código — Checklist mínimo (PR)

- [ ] ¿El PR tiene una descripción útil y pasos para reproducir o validar?
- [ ] ¿Los nombres de ramas/commits son claros?
- [ ] ¿Se añadieron pruebas unitarias/integración cuando aplica?
- [ ] ¿Se respetan los principios de Clean Architecture (ver abajo)?
- [ ] ¿Se actualizaron README o documentos relacionados si se añadieron carpetas o APIs?
- [ ] ¿No hay credenciales/secretos en el cambio?

## Estándares de código y Clean Architecture

- Separación en capas: Domain (entidades y contratos), Application (casos de uso), Infrastructure (implementaciones/adapters), UI (presentación).
- Regla de dependencia: las capas internas nunca deben depender de las externas.
- Las interfaces/contratos deben vivir en capas internas (Domain o Contracts compartidos).
- Implementaciones concretas se ubican en Infrastructure y se inyectan mediante DI.
- DTOs para borde de sistema; mapear explícitamente entre Domain y DTO.
- Mantener clases con responsabilidad única y métodos cortos y testeables.

## Tests y CI

- Añade tests unitarios a la librería correspondiente (preferiblemente con cobertura dirigida a la lógica de dominio y casos de uso).
- Executa tests localmente antes de abrir PR.
- El pipeline CI debe correr linting, tests y el script de verificación de READMEs (opcionalmente).

## Requisitos para nuevas carpetas/proyectos

Si creas una nueva carpeta/proyecto dentro de `GamePlatform`:

- Añade `README.md` con:
  - Propósito
  - Descripción de la parte del proyecto
  - Estándares de Clean Architecture aplicables
- Registra la nueva carpeta en `EnumaElishStructure.html`.

## Uso del comprobador automático de README

Se incluye un script PowerShell en `scripts\check_readmes.ps1` que analiza `EnumaElishStructure.html` y valida que exista `README.md` para cada carpeta/subcarpeta declarada.

Ejecutarlo desde la raíz del repositorio (Windows PowerShell):

```powershell
# Desde la carpeta del repositorio
powershell -NoProfile -ExecutionPolicy Bypass -File ".\scripts\check_readmes.ps1" -HtmlPath ".\EnumaElishStructure.html" -Root ".\GamePlatform"
```

El script devuelve código de salida 0 si todos los README están presentes; 1 si faltan archivos.

## Buenas prácticas adicionales

- Mantén PRs pequeños y focalizados.
- Documenta las decisiones arquitectónicas en `Docs/`.
- Evita cambios masivos sin coordinación previa con el equipo.

Gracias por contribuir. Si tienes dudas sobre el proceso, abre un issue o contacta a los mantenedores.
