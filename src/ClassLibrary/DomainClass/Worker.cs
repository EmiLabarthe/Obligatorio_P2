namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe.

/// <summary>
/// Clase que modela al rol de Trabajador en el bot.
/// La clase Worker cumple con Expert al igual que la clase User, ya que es la encargada de conocer
/// los atributos de sus instancias únicamente. En caso de necesitar saber la ubicación, necesita llamar
/// a la clase Location, ya que es la experta en eso. 
/// Tenemos que el constructor de la clase es el que hace el registro del usuario.
/// Como los usuarios van a ser diferentes del administrador, por que van a tener datos personales
/// que sirven para las funcinalidades del programa, implementamos la clase padre y en la clase hija 
/// le enviamos los datos necesarios para construir el objeto. Utilizamos herencia en función de lograr 
/// una reutilización del código.
/// </summary>
public class Worker : User, IObserver
{

 

    /// <summary>
    /// Constructor de la clase Worker. Le pasa los datos al constructor de la clase padre.
    /// </summary>
    /// <param name="userID">Identificador de telegram</param>
    /// <param name="name">Nombre del trabajador</param>
    /// <param name="lastName">Apellido del trabajador</param>
    /// <param name="phone">Número de celular del trabajador</param>
    /// <param name="address">Dirección del trabajador</param>
    /// <param name="latitude">Coordenada correspondiente a la latitud de la dirección del trabajador</param>
    /// <param name="longitude">Coordenada correspondiente a la longitud de la dirección del trabajador</param>
    public Worker(long userID, string name, string lastName, string phone, string address, double latitude, double longitude)
        : base(userID, name, lastName, phone, address, latitude, longitude)
    {
        // Intencionalmente en blanco
    }

    /// <summary>
    /// Aplicando el patrón Observer establecimos que el tipo Worker debería ser un Subscriptor de las ofertas de trabajo, para así 
    /// en vez de estar pregutando o revisando cada cierto tiempo en que estado esta su oferta, se le avisa cuando esta es dada de baja
    /// por un administrador. Este método se encarga de recibir el mensaje cuando se da la alerta o notificación del lado del objeto observado, y 
    /// crea un objeto de tipo Notification que se le agrega a la lista de notificaciones del trabajador.
    /// </summary>
    /// <param name="message"></param>
    public void Update(string message)
    {
        Notification notification = new Notification(message, UserID, this.UltimateIDNotification + 1, Notification.Reasons.AdminDeleteWorkOffer);
        this.Notifications.Add(notification);
    }
}
