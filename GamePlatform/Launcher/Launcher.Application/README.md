# Launcher.Application

Propósito

- Implementar los casos de uso y la orquestación de la lógica de negocio específica del lanzador.

Descripción

- Contiene servicios de aplicación, comandos, validaciones y flujos que coordinan Domain e Infrastructure.

Estándares de Clean Architecture

- Depende de `Launcher.Domain` pero no de `Launcher.Infrastructure` o `Launcher.UI`.
- Implementar interfaces definidas en Domain mediante inversion of control (IoC) y DI.
- Mantener clases finas, testables y con responsabilidades únicas.
