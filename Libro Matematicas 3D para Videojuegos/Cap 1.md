# Introducción

Lo primero es lo primero, pero no necesariamente en ese orden.

— Doctor Who de Meglos (1980)

## Quién debería leer este libro

Este libro trata sobre las matemáticas 3D, la geometría y el álgebra del espacio 3D. Está diseñado para enseñarte cómo describir objetos y sus posiciones, orientaciones y trayectorias en 3D utilizando las matemáticas. Este no es un libro sobre gráficos por computadora, simulación o incluso geometría computacional, aunque si planeas estudiar esos temas, definitivamente necesitarás la información aquí.

Este no es solo un libro para programadores de videojuegos. Damos por sentado que la mayoría de nuestros lectores aprenden con el propósito de programar videojuegos, pero esperamos un público más amplio y hemos diseñado el libro pensando en un público diverso. Si eres programador o te interesa aprender a crear videojuegos, ¡bienvenido! Si no cumples ninguno de estos criterios, aquí encontrarás mucho para ti. Nos hemos esforzado al máximo para que el libro sea útil para diseñadores y artistas técnicos. Aunque incluye varios fragmentos de código, esperamos que sean fáciles de leer incluso para quienes no son programadores. Y lo más importante, aunque siempre es necesario comprender los conceptos que lo rodean para comprender el código, nunca ocurre lo contrario. Usamos ejemplos de código para ilustrar cómo se pueden implementar las ideas en un ordenador, no para explicar las ideas en sí.

El título de este libro indica que está dirigido al desarrollo de videojuegos, pero gran parte del material que cubrimos es aplicable fuera del ámbito de los videojuegos. Prácticamente cualquier persona que desee simular, renderizar o comprender un mundo tridimensional encontrará este libro útil. Si bien intentamos ofrecer ejemplos motivadores del mundo del desarrollo de videojuegos, dado que es nuestra área de especialización y también nuestro público objetivo principal, no te quedarás fuera si el último juego que completaste fue Space Quest. Si te interesan temas más "adultos" que los videojuegos, ten por seguro que este libro no está lleno de ejemplos específicos de videojuegos sobre disparos a la cabeza, extremidades amputadas o cómo lograr que el chorro de sangre se vea perfecto.

## ¿Por qué deberías leer este libro?

Este libro tiene muchas características únicas, incluido su tema, enfoque, autores y estilo de escritura.

**Tema único.** Este libro llena un vacío dejado por otros libros sobre gráficos, álgebra lineal, simulación y programación. Es un libro introductorio, lo que significa que hemos centrado nuestros esfuerzos en brindar una cobertura exhaustiva de conceptos fundamentales de 3D, temas que normalmente se tratan superficialmente en unas pocas páginas o se relegan a un apéndice en otros libros (porque, después de todo, ya se sabe todo esto). ¡Hemos descubierto que estos mismos temas suelen ser los puntos de fricción para los principiantes! En cierto modo, este libro es la imagen especular de unir libros sobre gráficos, física y curvas. Mientras que esa conglomeración mítica comenzaría con una breve descripción general de los fundamentos matemáticos, seguida de una cobertura a fondo del área de aplicación, nosotros comenzamos con una cobertura exhaustiva de los fundamentos matemáticos y luego ofrecemos descripciones generales concisas y de alto nivel de las áreas de aplicación.

Este libro intenta ofrecer una vía de acceso ágil para principiantes, pero eso no significa que estemos atrapados en el carril lento para siempre. Contiene abundante material que tradicionalmente se considera "avanzado" y se enseña en cursos de nivel superior o de posgrado. En realidad, estos temas son más especializados que complejos, y recientemente se han convertido en prerrequisitos importantes que deben enseñarse con anterioridad, lo que en parte ha impulsado la demanda de un libro como este.

**Enfoque único.** Todos los autores creen encontrar el equilibrio perfecto entre la pedantería y la locuacidad para llegar mejor a su público, y nosotros no somos la excepción. Reconocemos, sin embargo, que quienes no estén de acuerdo con esta entusiasta autoevaluación, en su mayoría, encontrarán este libro demasiado informal (véase la entrada del índice para "alerta de rigor"). Nos hemos centrado en explicaciones perspicaces y en la intuición, y en ocasiones lo hemos hecho a costa del rigor. Nuestro objetivo es simplificar, pero no simplificar en exceso. Guiamos a los lectores a la meta a través de un camino que evita trolls y dragones, así que ¿por qué empezar el viaje señalándolos todos antes de siquiera haber dicho cuál es nuestro destino o por qué vamos allí? Sin embargo, como sabemos que los lectores eventualmente recorrerán el camino por su cuenta, una vez alcanzado nuestro objetivo, les señalaremos dónde residen los peligros. Sin embargo, a veces es posible que debamos dejar la tarea de eliminar ciertos trolls a otra fuente, especialmente si esperamos que su camino habitual no los acerque al peligro. Quienes tengan intención de visitar ese terreno con frecuencia deberían consultar con un lugareño para obtener información más detallada. Esto no significa que pensemos que el rigor no sea importante; simplemente creemos que es más fácil lograrlo una vez que se ha establecido una intuición sobre el panorama general, en lugar de sobrecargar cada discusión con definiciones y axiomas necesarios para abordar los casos extremos. Francamente, hoy en día un lector puede buscar presentaciones concisas y formales. gratis en wikipedia.org o Wolfram MathWorld ( [mathworld.wolfram.com](https://www.google.com/url?sa=E&q=http%3A%2F%2Fmathworld.wolfram.com%2F) ), por lo que no creemos que ningún libro ofrezca mucho que valga la pena pagar si se centra excesivamente en definiciones, axiomas, pruebas y casos extremos, especialmente para material introductorio dirigido principalmente a ingenieros.

**Autores únicos.** Nuestra experiencia combinada combina autoridad académica con consejos prácticos de primera mano. Fletcher Dunn cuenta con 15 años de experiencia profesional en programación de videojuegos, con alrededor de una docena de títulos en su haber en diversas plataformas. Trabajó en Terminal Reality en Dallas, donde, como programador principal, fue uno de los arquitectos del motor Infernal y programador principal de BloodRayne. Fue director técnico de The Walt Disney Company en Wideload Games en Chicago y programador principal de Disney Guilty Party, Juego Familiar del Año de IGN en el E3 2010. Actualmente trabaja para Valve Software en Bellevue, Washington. Pero su mayor logro, sin duda, es ser el homónimo del cabo Dunn de Call of Duty: Modern Warfare 2.

El Dr. Ian Parberry cuenta con más de un cuarto de siglo de experiencia en investigación y docencia académica. Este es su sexto libro, el tercero sobre programación de videojuegos. Actualmente es profesor titular del Departamento de Ciencias de la Computación e Ingeniería de la Universidad del Norte de Texas. Es reconocido nacionalmente como uno de los pioneros de la programación de videojuegos en la educación superior y ha impartido clases de programación de videojuegos en la Universidad del Norte de Texas de forma continua desde 1993.

**Estilo de escritura único.** Esperamos que disfrutes de la lectura de este libro de matemáticas (¿cómo?) por dos razones. La más importante es que queremos que aprendas de él, y que aprender algo que te interese sea divertido. En segundo lugar, queremos que disfrutes de la lectura de este libro como si leyeras una obra literaria. No nos hacemos ilusiones de que estemos al nivel de Mark Twain, ni de que este libro esté destinado a convertirse en un clásico como, por ejemplo, La Guía del Autoestopista Galáctico. Pero siempre se pueden tener aspiraciones. Sinceramente, solo somos un poco tontos. Al mismo tiempo, ningún estilo de escritura debería interferir con la prioridad principal: comunicar con claridad el conocimiento matemático sobre los videojuegos.

## Lo que deberías saber antes de leer este libro

Hemos intentado que el libro sea accesible al público más amplio posible; sin embargo, ningún libro puede remontarse hasta los primeros principios. Esperamos que el lector adquiera las siguientes habilidades matemáticas básicas:

- Manipular expresiones algebraicas, fracciones y leyes algebraicas básicas como las leyes asociativas y distributivas y la ecuación cuadrática.
    
- Entender qué son las variables, qué es una función, cómo graficar una función, etc.
    
- Algunos conceptos básicos de geometría euclidiana 2D, como qué es un punto, qué es una línea, qué significa que las líneas sean paralelas y perpendiculares, etc. Se utilizan algunas fórmulas básicas para el área y la circunferencia en algunos lugares. No importa si las has olvidado temporalmente; con suerte las reconocerás cuando las veas.
    
- Es recomendable tener cierta experiencia previa con trigonometría. Al principio del libro, ofrecemos un breve repaso de trigonometría, pero no se presenta con el mismo ritmo de explicación que en otras partes.
    
- Los lectores con cierta experiencia previa en cálculo tendrán ventaja, pero en este libro hemos limitado el uso del cálculo a principios muy básicos, que intentaremos enseñar en el Capítulo 11 para quienes no tengan esta formación. Solo se necesitan los conceptos más avanzados y las leyes fundamentales.
    

Algunos conocimientos de programación son útiles, pero no obligatorios. En varios lugares, ofrecemos breves fragmentos de código para mostrar cómo las ideas discutidas se traducen en código. (Además, ciertos procedimientos son más fáciles de explicar en código). Estos fragmentos son extremadamente básicos, están bien comentados y solo requieren un conocimiento básico de la sintaxis del lenguaje C (que se ha copiado a varios otros lenguajes). La mayoría de los artistas técnicos o diseñadores de niveles deberían poder interpretar estos fragmentos con facilidad.

## Descripción general

- [Capítulo 1](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C001.xhtml) introduce algunos fundamentos necesarios para el resto del libro y que probablemente ya conozcas. Revisa el sistema de coordenadas cartesianas en 2D y 3D y explica cómo usarlo para localizar puntos en el espacio. También incluye un repaso rápido de trigonometría y notación de suma.
    
- [Capítulo 2](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C002.xhtml) presenta los vectores desde una perspectiva matemática y geométrica e investiga la importante relación entre puntos y vectores. También analiza diversas operaciones con vectores, cómo realizarlas, qué significan geométricamente y en qué situaciones pueden ser útiles.
    
- [Capítulo 3](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C003.xhtml) analiza ejemplos de espacios de coordenadas y su anidación jerárquica. También introduce los conceptos centrales de vectores base y transformaciones en espacios de coordenadas.
    
- [Capítulo 4](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C004.xhtml) presenta las matrices desde una perspectiva matemática y geométrica y muestra cómo las matrices son una notación compacta para las matemáticas detrás de las transformaciones lineales.
    
- [Capítulo 5](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C005.xhtml) analiza en detalle los diferentes tipos de transformaciones lineales y sus matrices correspondientes. También analiza diversas maneras de clasificar las transformaciones.
    
- [Capítulo 6](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C006.xhtml) cubre algunas propiedades más interesantes y útiles de las matrices, como las transformaciones afines y la proyección en perspectiva, y explica el propósito y el funcionamiento de los vectores y matrices de cuatro dimensiones dentro de un mundo tridimensional.
    
- [En el Capítulo 7](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C007.xhtml) se analiza cómo utilizar coordenadas polares en 2D y 3D, por qué es útil hacerlo y cómo convertir entre representaciones polares y cartesianas.
    
- [Capítulo 8](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C008.xhtml) analiza diferentes técnicas para representar la orientación y el desplazamiento angular en 3D: ángulos de Euler, matrices de rotación, funciones exponenciales y cuaterniones. Para cada método, se explica su funcionamiento, se presentan sus ventajas y desventajas y cuándo se recomienda su uso. También se muestra cómo realizar conversiones entre diferentes representaciones.
    
- [Capítulo 9](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C009.xhtml) examina una serie de primitivos geométricos de uso común y analiza cómo representarlos y manipularlos matemáticamente.
    
- [Capítulo 10](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C010.xhtml) es una lección rápida sobre gráficos, que aborda algunos temas teóricos y prácticos modernos. Primero, presenta una visión general del funcionamiento de los gráficos, que culmina en la ecuación de renderizado. A continuación, el capítulo aborda algunos temas teóricos de naturaleza matemática. A continuación, aborda dos temas contemporáneos que suelen presentar dificultades matemáticas y que deberían ser de especial interés para el lector: la animación esquelética y el mapeo de relieve. Finalmente, el capítulo presenta una visión general del flujo de trabajo de gráficos en tiempo real, demostrando cómo se implementan las teorías de la primera mitad del capítulo en el contexto del hardware de renderizado actual.
    
- [Capítulo 11](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C011.xhtml) concentra dos temas bastante extensos en uno solo. Intercala los temas más avanzados del cálculo del primer semestre con un análisis de la cinemática de cuerpos rígidos: cómo describir y analizar el movimiento de un cuerpo rígido sin comprender necesariamente su causa ni preocuparse por la orientación o la rotación.
    
- [Capítulo 12](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C012.xhtml) continúa el análisis de la mecánica de cuerpos rígidos. Comienza con una explicación resumida de la mecánica clásica, incluyendo las leyes del movimiento de Newton y conceptos básicos como la inercia, la masa, la fuerza y ​​el momento. Revisa algunas leyes básicas de fuerza, como la gravedad, los resortes y la fricción. El capítulo también considera las analogías rotacionales de todas las ideas lineales discutidas hasta este punto. Se presta especial atención al importante tema de las colisiones. El capítulo finaliza con un análisis de los problemas que surgen al utilizar una computadora para simular cuerpos rígidos.
    
- [Capítulo 13](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C013.xhtml) explica las curvas paramétricas en 3D. La primera mitad del capítulo explica cómo se representa una curva relativamente corta en algunas formas comunes e importantes: monomio, Bézier y Hermite. La segunda mitad se centra en la unión de estas piezas más cortas para formar una curva más larga, denominada spline. Para comprender cada sistema, el capítulo considera los controles que el sistema presenta al diseñador de curvas, cómo recrear la descripción de una curva realizada por el diseñador, y cómo estos controles pueden utilizarse para construir una curva con propiedades específicas.
    
- [Capítulo 14](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C014.xhtml) inspira al lector a buscar la grandeza en los videojuegos.
    
- [Apéndice A](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C015.xhtml) es una colección de pruebas útiles que se pueden realizar en primitivas geométricas. Pretendemos que sea una referencia útil, pero también puede resultar interesante de explorar.
    
- [Apéndice B](https://www.google.com/url?sa=E&q=https%3A%2F%2Flearning-oreilly-com.ezproxy.unapec.edu.do%2Flibrary%2Fview%2F3d-math-primer%2F9781439869819%2FK13210_C016.xhtml) tiene todas las respuestas.
    

## ¿Encontraste un error en este libro?

Calculamos las probabilidades de que pudiéramos escribir un libro de matemáticas de más de 800 páginas sin errores. El resultado fue un número negativo, que sabemos que no puede ser correcto, pero probablemente esté bastante cerca. Si encuentras un error en este libro, visita gamemath.com. Lo más probable es que el error ya esté listado en las erratas, en cuyo caso te pedimos nuestras más sinceras disculpas. De lo contrario, envíanos un correo electrónico, y tendrás (además de nuestro profundo agradecimiento) fama eterna a través del crédito en las erratas por ser el primero en encontrar el error.

Cuidado. No queremos aprender de esto.

— Bill Watterson (1958–) de Calvin y Hobbes

---

Notas al pie:  
Bueno, puede que te pierdas algunas bromas, como esa. Lo siento.  
Por eso hemos puesto la mayoría de las bromas y la información inútil en notas al pie como esta. De alguna manera, sentimos que podríamos salirnos con la nuestra de esa manera.  
A los ejercicios, eso es.