# Proyecto Programación II 
## Agustín Toya, Emiliano Labarthe y Juan Pablo Amorín
-----------------------------------------------------------
## Descripción

¿Cómo podemos nosotros, estudiantes del curso de Programación II ayudar a las personas a encontrar trabajo?

La respuesta a este problema es un chatbot. El chatbot es una manera sencilla e intuitiva de vincular al usuario con el programa, 
para facilitar el uso e implementación del mismo.
 
El programa consta de 3 tipos de usuarios:

-Administrador

-Empleador

-Trabajador


Los administradores pueden crear categorías laborales y dar de baja a ofertas de trabajo.

Los empleadores pueden contratar trabajadores, puntuar al trabajador, ver la puntuación del trabajador y buscar todas las ofertas de trabajo, ofertas de trabajo
con filtro y ofertas de trabajo según el puntaje de su trabajador.

Los trabajadores pueden crear oferta de trabajo, puntuar al empleador, ver la puntuación del empleador y escribirle al empleador.

Todas las personas que usen el programa pueden registrarse como empleador, registrarse como empleador y mostrar las notificaciones que tenés.

Para usar el bot, una vez esté ejecutándose el programa, hay que escribirle por telegram al bot, usando "/" para mostrar los comandos que hay disponibles.
Cada comando tiene una breve descripción de su funcionamiento.
Una vez seleccionado uno de los comandos disponibles el bot le dará las instrucciones a seguir para lograr completar la acción correctamente.

### Funcionamiento del bot / Historias de usuario
https://www.youtube.com/watch?v=AwZdz0c0cfQ&ab_channel=Panther139

# Patrones y principios usados

## Observer: 
A la hora de enviar notificaciones al trabajador cuando la oferta de trabajo es eliminada, se usa patrón observer.
El trabajador se subscribe a la lista de observadores de la oferta de trabajo. Por ende, al eliminar una oferta de trabajo, se notifica al trabajador subscripto.

## Creator:
Cuando hablamos de creator, hablamos de un patrón utilizado frecuentemente en nuestro proyecto. Uno de nuestros usos más claros del mismo es en la clase Database. Esta clase es un singleton que se encarga de guardar objetos en listas. Al ser una clase encargada de almacenar, alberga todas las instancias de usuarios y ofertas usadas en el bot.
Al almacenar estas instancias, es a la vez la encargada de crearlas, ya que el patrón creator estipula que una clase que alberga un objeto, puede ser la encargada de crearlo.
También usamos creator en el caso de la clase LocationUser. Los objetos de esta clase son almacenados en los objetos de la clase User y sus herederos, por ende, User se encarga de crear a LocationUser.

## Polymorphism:
En el caso de polimorfismo, sabemos que el problema a solucionar consiste en que hay operaciones de comportamientos relacionados, pero implementadas por distintos tipos. Para solucionar esto, el patrón busca convertir una operación en polimórfica, es decir, que puedan implementarlas distintos objetos de distintos tipos, dada una clase padre o interfaz.
Un caso de polimorfismo en nuestro proyecto es ISort o IFilter y las clases que los implementan. Las clases de bajo nivel se encargan de realizar los comportamientos que sus interfaces declaran, pero a su manera y siendo distintos tipos.

## Expert:
Expert fue uno de los principales patrones dados en el curso, y sin lugar a dudas un patrón muy presente a la hora de desarrollar nuestro software. Este patrón dicta que a la hora de asignar una responsabilidad a una de las clases, se le debe asignar a la clase experta en la información necesaria. Es decir, a la que debe conocer todos los datos que se necesitan para lograr esa responsabilidad.
Una clara ilustración de este patrón es el caso de Database con sus métodos de "Get". Por ejemplo, en el caso de "GetWorkers", tenemos un método que busca devolver una lista de solo lectura de todos los trabajadores albergados en Database. Como ya sabemos, las responsabilidades deben ser ejecutadas por el experto en información, que en este caso es Database, ya que conoce la lista de todos los trabajadores almacenados.

## Composición y delegación:
Bajo el concepto de composición, entendemos que una clase componente compone a una clase compuesta. Como menciona en la lectura del tema, las instancias de la clase componente no existirían sin las instancias de la clase compuesta.
Un caso así en nuestro proyecto, es el de LocationUser y User. User (tanto sus herederos como worker o employer), está compuesto por instancias de LocationUser (para guardar la ubicación), y de Notification (instancias de Notification que alberga en su lista de notificaciones). Estas instancias no existirían si no existiese User, Worker o Employer.

Pasando a delegación, sabemos que es cuando se llama al método de una clase, pero esa clase, a la vez llama a otra para delegarle el comportamiento deseado. El caso de nuestro proyecto para ejemplificar este concepto, es tenemos el caso en el que se recibe la calificación, pero en vez del usar actualizar una propiedad lo hace la clase Rating a la que se le delega esta operación. 

## SRP:
El principio SRP es un principio utilizado frecuentemente en nuestro programa. Este principio consta en que cada clase debe encargarse de una sola funcionalidad del programa (single responsibility), por ende, solo tener una razón de cambio, que debe ser satisfacer esa necesidad. Al cumplir con SRP, todos sus métodos y atributos deben enlazarse con su responsabilidad. 
Una clara ilustración de una clase que cumple con SRP es LocationUser. Esta clase tiene la función de albergar los datos de los que se conforma una ubicación, y esta es su razón de existencia y la responsabilidad que debe cumplir. En caso de querer cambiar la clase LocationUser, su razón de cambio debe estar vinculada a albergar la ubicación de un usuario.

## OCP:
En cuanto a OCP, contamos con una instancia clara del mismo. Este principio se ve en ICommand y las clases que implementan dicha interfaz.
Los comandos son implementaciones de ICommand, por ende implementan sus métodos y atributos, los cuales están cerrados a la modificación, ya que funcionan correctamente y no es necesario cambiarlos. A su vez, están abiertos a la agregación, ya que se le pueden implementar nuevos métodos sin afectarlos.

## LSP:
Liskov Substitution Principle es un principio que consiste en que si un objeto es un subtipo del supertipo que se necesita para la operación, se podría usar cualquiera de los dos en la operación, sin tener efectos colaterales en el programa.
Un ejemplo de esto en nuestro software es en el caso de los comandos. Todos nuestros comandos son de tipo ICommand, por ende, al usar un comando, se podría sustituir por cualquier objeto del tipo ICommand, sin causar efectos colaterales en el código.

## ISP:
El Principio de Segragación de Interfaces (ISP), establece que una clase concreta no tiene por que depender de una interfaz de la cual solo usa ciertas operaciones cuadno el tipo define más. Por ende, para evitar que la clase tenga que implementar operaciones que no va a usar la interfaz se divide en otras que reunen a las operaciones que si se van a usar plenamente por clases concretas. 

En nuestro caso, aplicamos este principio al crear las interfaces IFiltere ISort, puesto que en un inicio 
teníamos una interfaz que especificaba las operaciones de buscar las ofertas ordenandolas por ubicación,
por puntuación así como filtrandolas por la categoría. Por ende, la clase que hiciera uso de esta abstracción iba a termiar dependiendo e implementando operaciones que no le correspondían usar. Por tal motivo separamos las responsabilidades en filtros y ordenadores,  cada uno en las abstracciones mencionadas al inicio.
 

## DIP:
En el caso de DIP, lo vemos reflejado en la lista que almacena los comandos en InputInterfacer. Esta lista guarda instancias del tipo ICommand. Esto quiere decir, tal como dice el principio de DIP, que las clases de alto nivel dependen de clases de alto nivel o abstracciones (interfaces como ICommand), dejando los detalles a clases de bajo nivel como cada comando en particular.

