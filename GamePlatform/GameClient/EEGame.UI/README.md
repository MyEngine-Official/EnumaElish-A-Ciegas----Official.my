# Game.UI

Propósito

- Contener las pantallas, HUD y componentes visuales del cliente de juego.

Descripción

- Renderizado, gestión de escenas y elementos de UX; no incluye lógica de juego profunda.

Estándares de Clean Architecture

- UI debe depender de `Game.Logic` a través de contratos; no contener reglas de negocio.
