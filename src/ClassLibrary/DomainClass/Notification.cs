using System;
using Exceptions;

namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe y Agustín Toya.

/// <summary>
/// Esta clase viene a representar las notificaciones que recibe un user en su sección o chat. 
/// Se creo cumpliendo con el patrón de Delegación y Composición, por que se separon los datos en 
/// otro objeto cuando pudieron estar en la clase user, terminando esta en ser un componente de user.
/// Además se cumple dos condiciones para este patrón, que la clase User tiene una lista de objetos de este tipo y que
/// no tiene sentido que exista esta clase sin estar ligada a un usuario, al menos en este contexto. 
/// </summary>
public class Notification
{

    /// <summary>
    /// Motivo o asunto de la notificación
    /// </summary>
    /// <value></value>
    public Reasons NotificationReasons {get;}

    /// <summary>
    ///  Representa los posibles motivos por lo que se le notifico algo al user
    /// </summary>
    public enum Reasons
    {
        /// Para cuando se crea una notificación de los que un employer se quiere contactar cono un worker
        EmployerWantContactWorker,
        /// Para cuando el worker le response al employer
        WorkerResponseAnEmployer,
        /// Para cuando el admin le borra una work offer al worker y se le genera una notificación
        AdminDeleteWorkOffer
    }

    /// <summary>
    /// Identificador de la notificación, para hacer que una sea diferente de otra
    /// </summary>
    /// <value></value>
    public int NotificationID { get; private set; }
    
    /// <summary>
    /// Mensaje que se le va a mostrar al user
    /// </summary>
    /// <value></value>
    public string Message { get; private set; }
    
    /// <summary>
    /// Identificador del usuario que le genero una notificación
    /// </summary>
    /// <value></value>
    public long SenderID { get; private set; }
    
    /// <summary>
    /// Fecha de creación o cuando se produjo la notificación
    /// </summary>
    /// <value></value>
    public DateTime CreationDate { get; private set; }

    /// <summary>
    /// Para controlar las notificaciones que estan "abiertas" 
    /// o que todavía no se cerraron
    /// </summary>
    /// <value></value>
    public bool IsOpen {get; private set;}

    /// <summary>
    /// Constructor de la notificación, la forma de que un user se entere de cosas, como de las intenciones de 
    /// comunicarse de un employer (si es que eres un worker), o sobre la respuesta de un worker 
    /// al querer contactarlo si eres un employer. 
    /// </summary>
    /// <param name="message">Texto que se le va a mostar al user</param>
    /// <param name="senderID">UserID del que le genero la notificación</param>
    /// <param name="notificationID">Identificador único para este objeto, así se diferencia de otros objetos del mismo tipo</param>
    /// <param name="reasons">Asunto o motivo de la notificación</param>
    public Notification(string message, long senderID, int notificationID, Reasons reasons)
    {
        // Revisamos las precondiciones
        Check.Precondition(!string.IsNullOrWhiteSpace(message), "El mensaje no puede ser nulo, vacío o ser espacios en blanco");
        Check.Precondition(senderID > 0, "El id del user que envio un mensaje o provoco una notificación, no puede ser menor a 1");
        Check.Precondition(notificationID > 0, "El identificador de las ofertas no puede ser menor a 1");
        bool userExist = Database.Instance.ExistEmployer(senderID) || Database.Instance.ExistWorker(senderID) || Database.Instance.ExistAdmin(senderID);
        Check.Precondition(userExist, "El userID del calificador no corresponde a ningún worker ni a ningún employer");
        this.Message = message;
        this.SenderID = senderID;
        this.CreationDate = new DateTime();
        this.NotificationID = notificationID;
        this.NotificationReasons = reasons;
        // Revisamos que todo haya sucedido como debía
        Check.Postcondition(this.Message == message, "Ocurrio un error en el sistema, no se pudo cargar el mensaje");
        Check.Postcondition(this.SenderID == senderID, "Ocurrio un error en el sistema, no se pudo cargar el indentificador del remitente ");
        Check.Postcondition(this.NotificationID == notificationID, "Ocurrio un error en el sistema, no se pudo cargar el identificador de la notificación");
    }

    /// <summary>
    /// Para mantener encapsulado el cerrar la notificación
    /// </summary>
    public void CloseNotifcation()
    {
        this.IsOpen = false;
    }

    
}