using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exceptions;

using System.Text;
using System;
namespace ClassLibrary;

// Responsabilidad de Agustín Toya.

/// <summary>
/// Comando encargado de hacer el contacto entre un empleador y un empleado,
/// haciendo referencia a que quiere contratarlo para un servicio en particular.
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
/// </summary>
public class ContactWorkerCommand : ICommand
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
    /// Propiedad que guía al propio comando en que paso debe estar ahora, y a sabiendas de eso, hace algo.
    /// El valor por defecto es que no inicio el comando, NoStarted
    /// </summary>
    /// <value></value>
    public ContactWorkerState State { get; private set; } = ContactWorkerState.NoStarted;

  

    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedo por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, ContactWorkerState> stateForUser = new Dictionary<long, ContactWorkerState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, ContactWorkerData> data = new Dictionary<long, ContactWorkerData>();
    
    private ContactWorkerData dataSendMessage;

    /// <summary>
    /// Se encarga de asignarle un valor a la propiedad del nombre con el fin
    /// de diferenciar entre un comando y otro.
    /// </summary>
    public ContactWorkerCommand()
    {

        this.Name = "/hireemployee";
    }

    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        this.State = ContactWorkerState.NoStarted;
        stateForUser[userID] = ContactWorkerState.NoStarted;
        this.InProccess = false;
        return $"El comando {Name} se canceló";
    }

    /// <summary>
    /// Se encarga de resturar los datos que genero el comando cuando entro el user la vez pasada o en otra oportunidad
    /// Si es la primera vez que entra, lo pone en el diccionario
    /// </summary>
    /// <param name="userID">ID del usuario de Telegram</param>
    public void RestoreData(long userID)
    {
        // Si el user ya hizo uso de este comando, estará en esta lista, 
        // si no esta, hay que agregarlo
        if(stateForUser.ContainsKey(userID))
        {
            this.State  = stateForUser[userID];
            this.dataSendMessage  = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.InProccess = true;
            this.State = ContactWorkerState.NoStarted;
            stateForUser[userID] = ContactWorkerState.NoStarted;
            dataSendMessage = new ContactWorkerData();
            data[userID] = dataSendMessage;
        }

    }

    /// <summary>
    /// Revisa si es un employer, si es employer y no otro puede ejecutar este comando
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
        bool  canExecute = userIsEmployer && !userIsWorker && !userIsAdmin;
        return canExecute;
        
    }

    /// <summary>
    /// Este método permite que el employer envie un "mensaje", para así presentarse ante un worker, esto con la intención de 
    /// contratarlo para un servico en particular. Por eso es que se toma en cuenta la workOffer, ya que el employer quiere contactar
    /// al worker por esa oferta.
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Mensaje de si se ha podido enviarle su mensaje o solicitud de contacto al worker</returns>
    public string Execute(long userID, string inputData)
    {
        RestoreData(userID);
        if (this.State == ContactWorkerState.NoStarted)
        {
         dataSendMessage.Response = "Esta ejecutando el comando de contactar a un trabajador. Ingrese el identificador de la oferta por la que esta interesado y quiere contactarlo.";
         stateForUser[userID] = ContactWorkerState.Started;

        }
        else if (State == ContactWorkerState.Started)
        {
            if (!String.IsNullOrWhiteSpace(inputData))
            {
                // Tratamos de convertir el dato del identify del workOffer a int, si falla controlamos la excepción.
                try
                {
                    int WorkOfferIDCheck = Int32.Parse(inputData);
                    // Buscamos si existe una oferta de trabajo con ese identificador
                    bool workOfferExist = Database.Instance.ExistWorkOffer(WorkOfferIDCheck);
                   

                    if (workOfferExist)
                    {
                        // Como existe la oferta, ahora recuperamos el id del worker relacionado, comprobamos que exista y le mandamos la notificación
                        WorkOffer offer = Database.Instance.SearchWorkOffer(WorkOfferIDCheck);
                        long workerOwnerID = offer.OwnerWorkerID;
                        bool existWorker = Database.Instance.ExistWorker(workerOwnerID);
                        Console.WriteLine(existWorker);

                        // Como no puede haber una workoffer con un identificador de un worker que no exista, si sucede, se debe parar
                        Check.Precondition(existWorker, "No puede se que exista una oferta con un dueño que no esta en el sistema");
                        
                        // El worker dueño existe, entonces reuperamos el objeto
                        Worker owner = Database.Instance.SearchWorker(workerOwnerID);
                        string message = $"Estimado o estimada, espero tenga un buen día. Me interesa contactarme con usted para poder acordar la contratación de su servicio de: {offer.Description} con identificador {offer.Identify}";
                        owner.AddNotification(message, userID, Notification.Reasons.EmployerWantContactWorker);
                        dataSendMessage.Response = $"Se ha contactado al worker {owner.Name} {owner.LastName}";
                        // Finalizamos el comando usando este metódo
                        Cancel(userID);

                    }
                    else
                    {
                        dataSendMessage.Response = "El identificador no es valido, no existe una oferta con ese identificador";
                    }

                }
                catch (FormatException)
                {
                    dataSendMessage.Response = "Como identificador de la oferta de trabajo solo ingrese números enteros mayores a 0";
                }
            }
        }
        data[userID] = dataSendMessage;
        return dataSendMessage.Response;
    }
    /// <summary>
    /// Lista de los estados en los que puede estar este comando. La información se pide
    /// en el estado anterior, es decir, empieza a pedir información al usuario cuando esta en NoStarted
    ///  pero con el InProcess en true, y así sucesivamente. 
    /// </summary>
    public enum ContactWorkerState
    {
        /// Recoje y procesa el supuesto identify de la workoffer que le interesa. En caso no ser correcta
        /// le pide que la ingrese de nuevo
        Started,

        /// Se da por terminado el comando o cuando no ha arrancado. En caso de no haber iniciado
        /// se pide el id de la oferta de trabajo que tiene el dueño que se quiere contactar. 
        NoStarted

    }
        /// <summary>
        /// Representa los datos que va obteniendo el comando ContactWorkerCommand en los diferentes estados.
        /// </summary>
        public class ContactWorkerData
        {
       

            /// <summary>
            /// El precio que se ingresó en el estado ContactWorkerState.PricePromt.
            /// </summary>
            public int WorkOfferID  {get; set;}
            
            /// <summary>
            /// Resuesta que se guarda en cada paso que se va haciendo, así si entra otro usuario a ejecutar el comando, no se pierde lo que se 
            /// le había dicho a ese en particular
            /// </summary>
            /// <value></value>
            public string Response {get; set;}

            
        }
}