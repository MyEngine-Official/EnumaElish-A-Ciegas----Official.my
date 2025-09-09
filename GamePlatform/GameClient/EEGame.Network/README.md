# Game.Network

Propósito

- Implementar clientes de red, protocolos de comunicación y adaptadores para SignalR/HTTP.

Descripción

- Contiene implementaciones de conexión, mensajes y manejo de latencia/reconexión.

Estándares de Clean Architecture

- Exponer interfaces para que `Game.Logic` las consuma; aislar dependencias de librerías externas.
