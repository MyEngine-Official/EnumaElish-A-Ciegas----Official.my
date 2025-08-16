# GameClient

Propósito

- Contener el cliente de juego principal (proyecto MonoGame) que implementa la lógica, UI de juego y la red.

Descripción

- Se divide por responsabilidades: UI del juego, lógica de juego, networking y modelos de dominio del juego.

Estándares de Clean Architecture

- Separar Game.Domain (modelos), Game.Logic (reglas y sistemas), Game.Network (comunicación) y Game.UI (renders y HUD).
- Mantener sistemas independientes y testables; definir contratos para servicios de red y persistencia.
