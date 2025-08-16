# GamePlatform

Propósito

- Contener los subproyectos que componen la plataforma: Launcher, GameClient, Server.LightAPI, Server.HeavyAPI, Shared, Docs.

Descripción

- Coordinador del desarrollo multi-proyecto: cada subcarpeta es un proyecto independiente con su propio README y responsabilidades.

Estándares de Clean Architecture

- Seguir separación en capas: Domain, Application, Infrastructure, UI.
- Mantener dependencias dirigidas hacia adentro.
- Definir contratos (interfaces) en capas internas y adaptadores en Infrastructure.
- Usar DTOs para comunicación entre capas y proyectos.

Estructura esperada

- Launcher/
- GameClient/
- Server.LightAPI/
- Server.HeavyAPI/
- Shared/
- Docs/
