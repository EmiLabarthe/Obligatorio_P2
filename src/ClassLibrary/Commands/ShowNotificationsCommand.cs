using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using Exceptions;

namespace ClassLibrary;

// Comando responsabilidad de Agustín Toya.
/// <summary>
/// Comando encargado de mostrar las notificaciones de un worker o un employer, dependidendo de cual de los dos esta buscando ver sus 
/// notificacioens. 
/// 
/// Sigue el principio de inversión de dependencias puesto que depende de una abstracción (ICommand), 
/// al igual que la clase concreta que termina haciendo uso de esta, por ende evitamos dependencias indeseables. 
/// Al aplicar DIP, podemos cambiar en tiempo de ejecución el comando a usar (como sucede realmente), por lo cual podemos
/// decir que se cumple con LSP. Esto a su vez nos lleva a decir que como se esta implementando una intarfaz, implementando 
/// sus operaciones pero al modo que necesite cada comando, estas operaciones terminan siendo polimorficas. 
/// 
/// Por último, esta clase cumple con OCP, el principio de abierto y cerrado, por que se define un tipo abstracto, la interfaz
/// ICommand, con sus operaciones y propiedades, pero en la práctica esta clase incorpora otras responsabilidad que originalmente
/// no estan definidas en el contrato del tipo (abierto a la extensión), pero sin modificar lo que ya estaba hecho (cerrado a la modificación).
/// /// Sigue el principio de inversión de dependencias puesto que depende de una abstracción (ICommand), 
/// al igual que la clase concreta que termina haciendo uso de esta, por ende evitamos dependencias indeseables. 
/// Al aplicar DIP, podemos cambiar en tiempo de ejecución el comando a usar (como sucede realmente), por lo cual podemos
/// decir que se cumple con LSP. Esto a su vez nos lleva a decir que como se esta implementando una intarfaz, implementando 
/// sus operaciones pero al modo que necesite cada comando, estas operaciones terminan siendo polimorficas. 
/// 
/// Por último, esta clase cumple con OCP, el principio de abierto y cerrado, por que se define un tipo abstracto, la interfaz
/// ICommand, con sus operaciones y propiedades, pero en la práctica esta clase incorpora otras responsabilidad que originalmente
/// no estan definidas en el contrato del tipo (abierto a la extensión), pero sin modificar lo que ya estaba hecho (cerrado a la modificación).
/// </summary>
public class ShowNotificationsCommand : ICommand
{
    /// <summary>
    /// Nombre del comando en particular
    /// </summary>
    /// <value>String que se le especifique</value>
    public string Name { get; }
    /// <summary>
    /// Propiedad de solo lectura, compartida por el tipo ICommand, para que se sepa 
    /// si el comando se esta ejecutando (en cualquiera de sus pasos),  o si ya termino y 
    /// se puede pasar a otro.
    /// </summary>
    /// <value>true si todavía se esta ejecutando, false si ya termino o no ha empezado</value>/
    public bool InProccess { get; private set; } = false;

    /// <summary>
    /// Se encarga de cargarle el nombre a la propiedad Name
    /// </summary>
    public ShowNotificationsCommand()
    {

        this.Name = "/shownotifications";
    }
   

    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        this.InProccess = false;   
        return $"El comando {Name} se canceló";
    }

     /// <summary>
    /// Se encarga de resturar los datos que genero el comando cuando entro el user la vez pasada o en otra oportunidad
    /// Si es la primera vez que entra, lo pone en el diccionario
    /// </summary>
    /// <param dataOfWorker.Name="userID"></param>
    public void RestoreData(long userID)
    {
        // Intencionalmente en blanco
    }
    
    /// <summary>
    /// Controla de que si no es un user: worker o employer, no puede usarlo
    /// </summary>
    /// <returns></returns>
    public bool ProfileCanExecute(long userID)
    {
        // En este caso solo el employer puede hacer uso de este comando
        bool userIsEmployer = Database.Instance.ExistEmployer(userID);
        bool userIsWorker = Database.Instance.ExistWorker(userID);
        bool userIsAdmin = Database.Instance.ExistAdmin(userID);
        // Por si por algún error se agrego en el resto de listas de tipos de usuario
        bool  canExecute = userIsEmployer || userIsWorker;
        return canExecute;

    }
    /// <summary>
    /// Esta versión o implementación de Execute lo que hace es devolver la lista de notificaciones 
    /// que tiene el user que esta escribiendo. Si no es un worker o un employer, no le va a dejar ejecutarlo.
    /// Si no tiene notificaciones le dirá que no las tiene en vez de no mostrar nada. 
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Devuelve la lista de notificaciones que tiene o un mensaje indicando por que no puede hacerlo</returns>
    public string Execute(long userID, string inputData)
    {
        
        StringBuilder result = new StringBuilder();
        bool existEmployer = Database.Instance.ExistEmployer(userID);
        bool existWorker = Database.Instance.ExistWorker(userID);
        // Finalizamos el comando usando este metódo
        Cancel(userID);
        Check.Precondition(existEmployer || existWorker, "Los admin no tienen notificaciones como para poder solicitar ver las suyas");
        if(existWorker)
        {
            Worker worker = Database.Instance.SearchWorker(userID);
            foreach(Notification not in worker.GetNotifications())
            {   
                result.Append($"Asunto: {not.NotificationReasons}. Viene de: {not.SenderID}. Mensaje: {not.Message}. Fecha de cración: {not.CreationDate}");
            }
        }
        else
        {
            Employer employer = Database.Instance.SearchEmployer(userID);
            foreach(Notification not in employer.GetNotifications())
            {  
                result.Append($"Asunto: {not.NotificationReasons}. Viene de: {not.SenderID}. Mensaje: {not.Message}. Fecha de cración: {not.CreationDate}");
            }
           
        }
        if(result.Length == 0)
        {
            return ("No tiene notificaciones en su buzón, en este momento");
        }
        else
        {
            return result.ToString();
        }
    }
}