# Launcher.UI

Propósito

- Contener las vistas, componentes Razor y la lógica de presentación del lanzador.

Descripción

- Implementa la interfaz de usuario y consume `Launcher.Application` para ejecutar casos de uso.

Estándares de Clean Architecture

- UI debe depender sólo de Application (a través de interfaces) y modelos de presentación/DTO.
- Evitar lógica de negocio en la UI; delegarla a Application/Domain.

