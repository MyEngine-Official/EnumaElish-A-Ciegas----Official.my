# EnumaElish: A Ciegas

Este repositorio contiene la plataforma de juego "EnumaElish: A Ciegas" y su estructura de proyectos.

Propósito
- Describir la organización general del monorepo y enlazar a las carpetas de los subproyectos.

Descripción
- Contiene proyectos cliente, lanzador y servidores (light/heavy) más código compartido y documentación.

Estándares de Clean Architecture (resumen)
- Seguir la regla de dependencia: las capas internas (Domain) no deben depender de las externas.
- Capas típicas: Domain (Entidades), Application (Casos de uso), Infrastructure (Implementaciones), UI (Presentación).
- Definir interfaces/contratos en capas internas; implementar en Infrastructure.
- Mantener codificación por contrato: repositorios, DTOs y adaptadores bien definidos.

Próximos pasos
- Entrar en cada carpeta (GamePlatform, Launcher, GameClient, Server.*, Shared, Docs) para ver sus README.md específicos.
