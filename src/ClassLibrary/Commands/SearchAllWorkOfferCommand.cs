using System.Collections.ObjectModel;
using System.Text;
using System;
namespace ClassLibrary;

// Comando responsabilidad de Agustín Toya.

/// <summary>
/// Comando encargado de buscar y devolver todas las ofertas de trabajo que se han hechos
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
public class SearchAllWorkOfferCommand : ICommand
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
    public SearchAllWorkOfferCommand()
    {

        this.Name = "/showallworkoffers";
    }
    
    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        this.InProccess = false;
        // Como es escribirse y ya funciona, no hay paso en el que se pueda cancelar
        return $"El comando {Name} se canceló";
    }

    /// <summary>
    /// Prueba de que si no es admin o worker o employer, no puede usarlo
    /// </summary>
    /// <returns></returns>
    public bool ProfileCanExecute(long userID)
    {
        // En este caso solo el employer puede hacer uso de este comando
        bool userIsEmployer = Database.Instance.ExistEmployer(userID);
        bool userIsWorker = Database.Instance.ExistWorker(userID);
        bool userIsAdmin = Database.Instance.ExistAdmin(userID);
        // Por si por algún error se agrego en el resto de listas de tipos de usuario
        bool  canExecute = userIsEmployer || userIsWorker || userIsAdmin;
        return canExecute;

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
    /// Método que se encarga de ejecutar el comportamiento de este comando. En este caso
    /// se encarga de devolver todas las ofertas de trabajo que hay en el sistema y que estan Published
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns>Mensaje de respuesta en base a lo que esta pasando en el paso actual</returns>
    public string Execute(long userID, string inputData)
    {
        InProccess = true;
        // Obtenemos la lista de ofertas de trabajo
        ReadOnlyCollection<WorkOffer> offers = Database.Instance.GetAllWorkOffers();
        // Si esa lista no esta vacía, deveríamos de avisarle al suer
        if(offers.Count == 0)
        {
            return "No hay ofertas en el sistema en este momento, por favor espera a que un worker ponga su oferta a disposición.";
        }
        // Contenedor de la información recolectada
        StringBuilder result = new StringBuilder();
        // Obtenemos una lista con los workers para poder brindar el nombre y apellido del 
        // creador además de su UserID. 
        ReadOnlyCollection<Worker> workers = Database.Instance.GetWorkers();

        // Recorremos la lista del principio y vamos agregando los datos a la lista
        // si es que la oferta esta puesta y no fue "eliminada"
        foreach (WorkOffer offer in offers)
        {
            if(offer.IsPublished)
            {
                Worker workerOwner = Database.Instance.SearchWorker(offer.OwnerWorkerID);
                long workerUserID = workerOwner.UserID;;
                string nameOwner = workerOwner.Name;
                string lastNameOwner = workerOwner.LastName;
                lastNameOwner = workerOwner.LastName;
                result.Append($"\n\nIdentificador: {offer.Identify}\nDueño: {nameOwner} {lastNameOwner}, ID del dueño: {workerUserID}. \nDescripción: {offer.Description}\nPrecio: {offer.Price}");
            }
        }
        // Da de baja el comando para que no quede prendido
        Cancel(userID);
        return result.ToString();
    }
}