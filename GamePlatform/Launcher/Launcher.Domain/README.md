# Launcher.Domain

Propósito

- Contener las entidades de negocio, value objects y las interfaces (contratos) que definen la lógica del dominio del lanzador.

Descripción

- Núcleo del proyecto: modelos puros sin dependencias de infraestructura ni UI.

Estándares de Clean Architecture

- No debe depender de otras capas.
- Definir repositorios y servicios como interfaces aquí.
- Mantener invariantes del dominio y lógica de validación centrada en entidades.
