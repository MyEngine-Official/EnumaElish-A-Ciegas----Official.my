# Launcher

Propósito

- Proyecto MAUI Blazor Hybrid que actúa como lanzador y centro de administración para clientes y actualizaciones.

Descripción

- Contiene la UI (Razor), lógica de aplicación, infraestructura para comunicación con APIs y el dominio con entidades y reglas.

Estándares de Clean Architecture

- Capas: Launcher.Domain (entidades, interfaces), Launcher.Application (casos de uso), Launcher.Infrastructure (implementaciones), Launcher.UI (presentación).
- Las interfaces deben vivir en `Launcher.Domain`.
- Evitar referencias de capas exteriores hacia interiores; las pruebas y los contratos deben importar solo capas internas.
- DTOs y mapeos claros entre capas.
