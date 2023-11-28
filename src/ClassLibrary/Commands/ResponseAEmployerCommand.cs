using System.Collections.Generic;
using Exceptions;
using System.Text;
using System.Collections.ObjectModel;
using System;
namespace ClassLibrary;

// Comando de Agustín Toya y Emiliano Labarthe.

/// <summary>
/// Comando encargado responder a la solicitud de contacto del employer
/// haciendo referencia a que quiere contratarlo para un servicio en particular
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
public class ResponseAEmployerCommand : ICommand
{
    /// <summary>
    /// Es el nombre que tendrá el comando y que lo diferenciará de otros comandos
    /// </summary>
    /// <value>El nombre que se le especifique</value>
    public string Name { get; }

    /// <summary>
    /// Propiedad de solo lectura, compartida por el tipo ICommand, para que se sepa 
    /// si el comando se esta ejecutando (en cualquiera de sus pasos),  o si ya termino y 
    /// se puede pasar a otro.
    /// </summary>
    /// <value>true si todavía se esta ejecutando, false si ya termino o no ha empezado</value>/
    public bool InProccess { get; private set; } = false;

    /// <summary>
    /// Se encarga de asignarle un valor a la propiedad del nombre con el fin
    /// de diferenciar entre un comando y otro
    /// </summary>
    public ResponseAEmployerCommand()
    {
        Name = "/responseaemployer";
    }

    /// <summary>
    /// Propiedad que guía al propio comando en que paso debe estar ahora, y a sabiendas de eso, hace algo.
    /// El valor por defecto es que no inicio el comando, NoStarted
    /// </summary>
    /// <value></value>
    public ReponseEmployerState State { get; private set; } = ReponseEmployerState.NoStarted;



    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedó por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, ReponseEmployerState> stateForUser = new Dictionary<long, ReponseEmployerState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, ReponseEmployerData> data = new Dictionary<long, ReponseEmployerData>();

    private ReponseEmployerData responseData;
    /// <summary>
    /// Se encarga de resturar los datos que genero el comando cuando entro el user la vez pasada o en otra oportunidad
    /// Si es la primera vez que entra, lo pone en el diccionario
    /// </summary>
    public void RestoreData(long userID)
    {
        // Si el user ya hizo uso de este comando, estará en esta lista, 
        // si no está, hay que agregarlo
        if (stateForUser.ContainsKey(userID))
        {
            this.State = stateForUser[userID];
            this.responseData = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.State = ReponseEmployerState.NoStarted;
            stateForUser[userID] = ReponseEmployerState.NoStarted;
            responseData = new ReponseEmployerData();
            data[userID] = responseData;
            this.InProccess = true;
        }

    }

    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        stateForUser[userID] = ReponseEmployerState.NoStarted;
        this.State = ReponseEmployerState.NoStarted;
        this.InProccess = false;
        return $"El comando {Name} se canceló";
    }

     /// <summary>
    /// Revisa si es un worker, si es worker y no otro puede ejecutar este comando
    /// </summary>
      /// <param name="userID">Identificador de Telegram del usuario que ejecuto el comando</param>
    /// <returns>True si puede, false si no esta habilitado</returns>
    public bool ProfileCanExecute (long userID)
    {
        // En este caso solo el employer puede hacer uso de este comando
        bool userIsEmployer = Database.Instance.ExistEmployer(userID);
        bool userIsWorker = Database.Instance.ExistWorker(userID);
        bool userIsAdmin = Database.Instance.ExistAdmin(userID);
        // Por si por algún error se agrego en el resto de listas de tipos de usuario
        bool  canExecute = !userIsEmployer && userIsWorker && !userIsAdmin;
        return canExecute;
    }

    /// <summary>
    /// Este método permite que el worker envie un "respuesta", a la solicitud, pasando los datos de contacto en caso 
    /// de aceptar, enviando un negativo en caso de no aceptar. 
    /// </summary>
    /// <param name="userID"> identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al mensaje</param>
    /// <returns> Mensaje de que paso en cada paso de la ejecución del comando</returns>
    public string Execute(long userID, string inputData)
    {   
   
        RestoreData(userID);
         // Primero hay que revisar que no sea nulo o vacío, pero acá nunca tendría que llegar nulo o vacío
        Check.Precondition(!string.IsNullOrWhiteSpace(inputData), "La información no puede llegar vacía o nula");
        if(this.State == ReponseEmployerState.NoStarted)
        {
            responseData.Response = "Esta ejecutando el comando responder a un empleador. Ahora ingrese el id de la notificación del contacto, para poder responder";
            stateForUser[userID] = ReponseEmployerState.Started;
        }
        else if (this.State == ReponseEmployerState.Started)
        {
            //Antes que nada, tenemos que convertir el tipo de dato en un int
            int inputDataInt = 0;
            try
            {
                inputDataInt = Int32.Parse(inputData);
            }
            catch(FormatException)
            {
                return "Ingrese números enteros para la próxima ocasión, por favor";
            }
            // Ahora tenemos que validar que ese ID sea valido entre las notificaciones
            // Como se supone que no debería llegar acá ningún otro tipo de profile, lo tomamos como una precondición
            Check.Precondition(Database.Instance.ExistWorker(userID), "Este usuario o no existe en el sistema o no es un worker");
            // Recuperamos al worker que ejecuta este comando
            Worker worker = Database.Instance.SearchWorker(userID);
            ReadOnlyCollection<Notification> notificationsForWorker = worker.GetNotifications();
            // Para detener la búsqueda cuando se encuentre y no hacer que se busque por toda la lista
            bool stop = false;
            int index = 0;
            bool noExistNotification = true;
            bool reasonNotEmployerWantContactWorker = false;
            while (!stop && index != notificationsForWorker.Count)
            {   
                Notification not = notificationsForWorker[index];
                noExistNotification = not.NotificationID != inputDataInt;
                reasonNotEmployerWantContactWorker = not.NotificationReasons != Notification.Reasons.EmployerWantContactWorker;
                if(!noExistNotification && !reasonNotEmployerWantContactWorker)
                {
                    responseData.Response = "Ingrese 'Yes, send' para que le enviemos a este employer su información de contacto o 'No, send' en caso contrario";
                    responseData.EmployerResponded = Database.Instance.SearchEmployer(not.SenderID);
                    responseData.NotificationToReply = not;
                    stateForUser[userID] = ReponseEmployerState.ConfirmOrCancelContactPromt;
                    stop = true;
                    noExistNotification = false;
                    reasonNotEmployerWantContactWorker = false;
                }
                index +=1;
            }
            // Por si sale del while sin haber encontrado la notificación
            if(noExistNotification)
            {
                responseData.Response = "No se ha encontrado una notificación con ese id";
            }
            else if(reasonNotEmployerWantContactWorker)
            {
                responseData.Response = "La notificación no es de un employer queriendo contactarse";
            }

        }
        else if(this.State == ReponseEmployerState.ConfirmOrCancelContactPromt)
        {
            // Luego formateamos lo que recibimos, quitando espacios y poniendo el texto en minúsculas
            string decisionWorker = inputData.Trim().ToLower();
            // Obtenemos al objeto del user que esta escribiendo
            Worker worker = Database.Instance.SearchWorker(userID);

            // Ahora revisamos si es yes or no
            if(decisionWorker == "yes, send")
            {
                // Ahora notificamos al employer
                string message = $"El worker {worker.Name} {worker.LastName} de identificador {worker.UserID} ha aceptado ser contactado. Su información de contacto es {worker.Phone}";
                responseData.EmployerResponded.AddNotification(message, userID, Notification.Reasons.WorkerResponseAnEmployer);
                // Finalizamos el comando usando este metódo
                Cancel(userID);
                responseData.Response = "Ya se le envio su información de contacto para que el employer se contacte con usted";
                // Ahora cerramos automaticamente la notifiación de contacto por que ya no tiene sentido que la tenga
                responseData.NotificationToReply.CloseNotifcation();
            }
            else if(decisionWorker == "no, send")
            {
                string message = $"El worker {worker.Name} {worker.LastName} de identificador {worker.UserID} no ha aceptado ser contactado. Lo sentimos mucho";
                responseData.EmployerResponded.AddNotification(message, userID, Notification.Reasons.WorkerResponseAnEmployer);
                // Finalizamos el comando usando este metódo
                Cancel(userID);
                responseData.Response = "Ya se le envió la notifiación al employer de que no quiere ser contactado por él para hablar sobre la oferta en cuestión";
                responseData.NotificationToReply.CloseNotifcation();
            }
            else
            {
                responseData.Response = "El texto que introdujo no corresponde con ninguna opción valida en este paso, recuerde: 'yes send' para enviar su información de contacto, 'no send' en caso contrario";
            }
            
        }
        data[userID] = responseData;
        return responseData.Response;
    }


    /// <summary>
    /// Estados que puede tener el comando
    /// </summary>
    public enum ReponseEmployerState
    {
        /// Para cuando no se ha iniciado el comando o se detuvo. 
        /// Si no se había iniciado y ahora InProccess pasa a estar en true, 
        /// se pide el identificador de la notificación de contacto, para responder 
        /// esa notificación
        NoStarted,
        /// Cuando inicia el comando pide el ID de la oferta de trabajo
        Started,
        /// Paso en el que se pide su decisión, si es no o si
        ConfirmOrCancelContactPromt
    }

    /// <summary>
    /// Representa los datos que va obteniendo el comando en los diferentes estados.
    /// </summary>
    public class ReponseEmployerData
    {
    

        /// <summary>
        /// El objeto employer al que se le quiere enviar la notificación
        /// </summary>
        /// <value></value>
        public Employer EmployerResponded {get; set;}

        /// <summary>
        ///  Objeto que representa a la notifiación que va a responder el user
        /// </summary>
        /// <value></value>
        public Notification NotificationToReply  {get; set;}

        /// <summary>
        /// La respuesta a devolver al usuario.
        /// </summary>
        /// <value></value>
        public string Response { get; set; }
    }
}
