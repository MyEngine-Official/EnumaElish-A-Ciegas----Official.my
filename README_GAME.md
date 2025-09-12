Este videojuego se llama EnumaElish: A Ciegas.

Esta basado en una historia original de los autores, la cual plasmaran en su modo historia. 

El modo online de este juego toma como premisa una escena de la historia:
"Mientras la división trigésimo primera avanzaba por el denso bosque boreal, cada vez más cerca del santuario donde esperaban encontrar el Enuma Elish, un desafortunado encuentro con la Inquisición y el creador de este artefacto los forzó a enfrentarse en un combate decisivo. Tras una larga y agotadora batalla, Rubí, la segunda comandante de la división, observando que la derrota era inminente, decidió arriesgarlo todo. Utilizando su magia ilusoria, logró transportar a todos a un lugar diferente, con la esperanza de que su equipo pudiera aprovechar las tácticas previamente entrenadas en ese entorno.

Sin embargo, algo salió mal. Cuando la bruma se disipó, cada uno de ellos se vio a sí mismo y a sus compañeros como meras siluetas negras, indistinguibles en la penumbra ilusoria. Sus recuerdos se encontraban nublados, y los rostros de sus aliados se habían desvanecido. Solo sabían cuántos eran y que entre ellos se escondían asesinos. Aunque incapaces de reconocerse, comprendieron que la única forma de sobrevivir era encontrar al verdadero enemigo… y eliminarlo a ciegas."

La premisa del modo online es simple, se ubica en uno de los momentos de mas tensión en la trama, y toma dos bandos representados por los jugadores. 
La premisa de "encontrar al verdadero enemigo y eliminarlo a ciegas" es fielmente mostrada al ocultar el rol de cada jugador durante la partida, y no revelarlo sino solo al entrar en la arena con otro jugador (se detallara mas adelante este apartado).

El juego comienza de esta manera. Se muestra una cinemática, narración, o sucesión de imágenes que cuenten la premisa descrita arriba, colocando en contexto a los jugadores. 

Posteriormente, se le dará a los jugadores un rol, el cual no será de dominio publico. 

A partir de este momento, el juego empezara su bucle hasta que haya una ganador, dependiendo de la modalidad jugada: online o torneo, pero las dos con diferencias sutiles que no lo que denominaremos "el ciclo del juego".

El ciclo del juego no es más que los eventos que se repetirán hasta que haya un ganador. En este caso, el ciclo iniciará en la fase diurna, en la cual cada uno de los jugadores entrarán en una especie de mazmorra con enemigos y puntos de guardado, y se les dará un tiempo para avanzar lo mas que puedan.
Hay caminos capaces de desviar a los jugadores hacia la dirección de otro jugador, provocando un combate directo entre ambos y con la posibilidad de reiniciar el punto de guardado de ese jugador o la colaboración entre ambos.
Mientras más avance un jugador, mayor equipamiento podrá obtener para subir sus estadisticas de ataque, defensa, hp, etc...

Al terminarse el tiempo, entrará una fase de discusión en la que los jugadores podrán discutir entre quienes serán de su bando y quienes del bando contrario e idear una estrategia.

Después de la fase de discusión, pasaremos a la fase de votación, en la que los jugadores, de manera obligatoria, pueden votar para eliminar a un jugador. Al recibir las votaciones, el sistema resolverá los votos de la siguiente manera y el siguiente nivel de prioridad:

-Los jugadores que se votaron el uno al otro entraran en la misma arena en su primer combate. Al terminarse dicha arena, seguirán la cadena de resolución de votos.

-Los jugadores que recibieron votos no mutuos entrarán a una arena, y al terminarse dicha arena, seguirán con el siguiente en la resolución de votos.

Los jugadores que se estén enfrentando en la misma arena pueden optar por atacar con ataques genéricos, o con sus ataques propios de su rol, pero con la consecuencia de revelarlo al oponente, aunque este recibirá una penalización para no poder hablar en la siguiente fase de discusión. Pero es inmune a este efecto para segunda fase.

(Los jugadores mantendrán la equipación de la mazmorra, por lo que mientras más avances, mas chances tendrás de ganarle a los demás jugadores).

El propósito de la arena es darle la oportunidad al acusado de invalidar cada voto en su contra, al ganar en la arena contra su votante. Al concluirse la cadena de resolución de votos. El jugador con más votos válidos en su contra, es eliminado y contará como asesinato. Cada jugador con asesinatos que no sean hacia su mismo equipo, recibirá bonificación y ayuda de la inteligencia artificial para ganar dentro de la partida. 

La siguiente fase es la nocturna, en donde cada uno de los jugadores puede hacer uso de sus roles ocultos. Los cuales servirán para colocarle más obstáculos a los demás jugadores en sus respectivas mazmorras y de esa manera retrasarlos. En otros casos, jugadores tendrán el rol o la habilidad de resucitar a uno de sus compañeros caídos.

Este ciclo de repetirá hasta que un jugador complete la mazmorra (obteniendo asi una recompensa proporcional a su logro) y llegue hasta la sala del EnumaElish. En ese momento, todos los jugadores vivos, independientemente de su avance en la mazmorra, pasarán a una arena en la cual todos sus roles serán descubiertos y el bando de cada jugador será visible para todos los demás, los dos bandos lucharán entre si por la posesión del EnumaElish.

Ganará el equipo que quede en pie, reclamando asi el EnumaElish.

Notas:

-En esta ultima batalla los equipos estarán buffeados o nerfeados según que tan lejos llegaron a la mazmorra, o sus integrantes con vida.







------------------------------------------------------------------------

Necesito que revises el motor y arregles incongruencias que encuentres. Necesito que lo que esta implementado actualmente, lo este de forma estable, absolutamente todo. Por ejemplo, la camara en alguna parte supuestamente tiene un constructor con 1 parametro, pero en la clase camara no hay constructores. Necesito que estabilices todo el motor, y hagas un readme de como funciona y otro readme sobre como usarlo, y ejemplos.

Quiero crear un motor de videojuegos robusto utilizando monogame como base. En primera instancia quiero usar el patron ECS (ENTITY-COMPONENT-SYSTEM) para construirlo. Tengo una libreria que hice hace un tiempo basado en monogame con clases como tilemap, animated sprite, del cual quisiera extraer toda su logica, pero en pequeños pedazos, como un lego, ya que el patron ecs asi lo requiere. Nuestro trabajo será tratar de que el motor sea lo mas completo y eficiente posible. Apoyandonos en mi otra libreria para algunas cosas y asi agilizar el proceso. Tu trabajo sera ver que falta para completar la libreria, e incluirlo o editarlo en caso de no ser eficiente o poder ser un ionconveniente a futuro. Por ultimo, deberas crear un archivo README.md en el cual describas con mas detalle que el mismo motor, como funciona este, como funcionan las entidades, componentes, y como funciona todo en conjunto. Ademas, deberas crear una carpeta llamada examples, en donde harás un videojuego de ejemplo de como utilizar el motor para construir un videojuego sencillo pero funcional. El archivo readme.md deberá ser actualizado cada vez que se edite el motor para tenerlo actualizado de todo el funcionamiento de este.

Tu prioridad es analizar el motor, cómo funciona, por qué funciona y su estructura interna y absolutamente todo. Debes leer todos los archivos, armar en tu mente las conexiones, cómo se usa cada clase y cómo se interrelaciona con las demás. Cómo funciona el motor, su estructura interna, todos los detalles de cada clase y cómo se usa, cómo está pensado el motor para usarse, recomendaciones de cómo usarlo y mostrar casos de uso comunes, tienes un readme.md dentro de la carpeta docs el cual te guiara dentro del motor y te dara un resumen de como funciona, pero tambien quiero que lo veas por tu cuenta y leas las clases del motor. Además, harás un segundo **README.md** con un tutorial paso a paso en donde enseñes cómo usar cada elemento del motor. También harás un tercer **README.md** en el cual colocarás todas las mejoras, incongruencias y problemas que encuentres en el motor y que deben solucionarse (solo regístralo en ese README, no corrijas los errores). Te faltan todavia los otros 2 readmes. Recuerda, en uno daras un tutorial de ejemplos de como se usa cada cosa, y como construir un videojuego de ejemplo con el motor. En el otro readme colocaras las cosas a mejorar del motor.

analizar el motor, cómo funciona, por qué funciona y su estructura interna y absolutamente todo. Debes leer todos los archivos, armar en tu mente las conexiones, cómo se usa cada clase y cómo se interrelaciona con las demás. Cómo funciona el motor, su estructura interna, todos los detalles de cada clase y cómo se usa, cómo está pensado el motor para usarse, tienes un readme.md dentro de la carpeta docs el cual te guiara dentro del motor y te dara un resumen de como funciona, pero tambien quiero que lo veas por tu cuenta y leas las clases del motor. Revisa exhaustivamente el motor de videojuegos, analizalo en cada rincon, especialmente sus systems, y redacta en un readme_fix.md todos los errores, inconvenientes e ineficiencias que presenta el motor.