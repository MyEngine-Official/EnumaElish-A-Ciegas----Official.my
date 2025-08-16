# Launcher.Infrastructure

Propósito

- Proveer implementaciones concretas: acceso a APIs, persistencia, adaptadores externos y servicios de red.

Descripción

- Implementa interfaces declaradas en `Launcher.Domain` y ofrece adaptadores para `Launcher.Application`.

Estándares de Clean Architecture

- Puede depender de Domain y Application (solo si es necesario), pero nunca de UI.
- Mantener adaptadores pequeños y testeables; usar patrones Repository, Gateway y Adapter.
