using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace ClassLibrary;

// Comando responsabilidad de Juan Pablo Amorín.

/// <summary>
/// Comando encargado de mostrar la calificación de un tarbajador en espécifico. 
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
public class ShowRateWorkerCommand : ICommand
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
    public ShowRateWorkerState State { get; private set; } = ShowRateWorkerState.NoStarted;

    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedo por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, ShowRateWorkerState> stateForUser = new Dictionary<long, ShowRateWorkerState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, ShowRateWorkerData> data = new Dictionary<long, ShowRateWorkerData>();

    private ShowRateWorkerData dataOfShowRate;

    /// <summary>
    /// Establece el nombre que tendrá el comando
    /// </summary>
    public ShowRateWorkerCommand()
    {
        this.Name = "/showworkerrating";
    }


    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        stateForUser[userID] = ShowRateWorkerState.NoStarted;
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
        // Si el user ya hizo uso de este comando, estará en esta lista, 
        // si no esta, hay que agregarlo
        if (stateForUser.ContainsKey(userID))
        {
            this.State = stateForUser[userID];
            this.dataOfShowRate = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.InProccess = true;
            this.State = ShowRateWorkerState.NoStarted;
            stateForUser[userID] = ShowRateWorkerState.NoStarted;
            dataOfShowRate = new ShowRateWorkerData();
            data[userID] = dataOfShowRate;
        }
    }

    /// <summary>
    /// Controla de que si no es un admin o un employer, no pueda usarlo
    /// </summary>
    /// <returns></returns>
    public bool ProfileCanExecute(long userID)
    {
        // En este caso solo el employer puede hacer uso de este comando
        bool userIsEmployer = Database.Instance.ExistEmployer(userID);
        bool userIsWorker = Database.Instance.ExistWorker(userID);
        bool userIsAdmin = Database.Instance.ExistAdmin(userID);
        // Por si por algún error se agrego en el resto de listas de tipos de usuario
        bool  canExecute = !userIsWorker && (userIsEmployer || userIsAdmin);
        return canExecute;
    }

    /// <summary>
    /// Se encarga de resturar los datos que genero el comando cuando entro el user la vez pasada o en otra oportunidad
    /// Si es la primera vez que entra, lo pone en el diccionario
    /// </summary>
    ///  /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Mensaje de que paso en cada paso de la ejecución del comando</returns>
    public string Execute(long userID, string inputData)
    {

        RestoreData(userID);
        if (this.State == ShowRateWorkerState.NoStarted)
        {
            dataOfShowRate.Response = $"Esta ejecutando el comando de mirar la calificación de un trabajador. Ingrese el id del worker del que quiere consultar su calificación";
            stateForUser[userID] = ShowRateWorkerState.Started;
        }
        else if (this.State == ShowRateWorkerState.Started)
        {
            // Primero tenemos que hacer un parse con lo que nos llega
            long workerIDSpecification = 0;
            try
            {
                workerIDSpecification = Int64.Parse(inputData);
            }
            catch (FormatException)
            {
                return "El formato no es valido, ingrese solo números como identificador del worker, por favor.";
            }
            // Una vez convertido el dato y que no sea frenado por el formato, buscamos si existe algún worker con ese id
            bool existWorker = Database.Instance.ExistWorker(workerIDSpecification);
            if (existWorker)
            {
                // Obtenemos al worker dueño de ese id
                Worker workerShowRate = Database.Instance.SearchWorker(workerIDSpecification);
                double calification = workerShowRate.GetRating();
                // Da de baja el comando para que no quede prendido
                Cancel(userID);
                if(calification != 0)
                {
                    dataOfShowRate.Response = $"La calificación del trabajador es: {calification} en un total de 10 puntos";
                } 
                else
                {
                    dataOfShowRate.Response = "Este worker no ha sido calificado aún.";
                }

            }
            else
            {
                dataOfShowRate.Response = "No se ha encontrado un worker con ese id.";
            }
        }
        data[userID] = dataOfShowRate;
        return dataOfShowRate.Response;
    }

    /// <summary>
    /// Indica los diferentes estados que puede tener el comando.
    /// </summary>
    public enum ShowRateWorkerState
    {
        /// Para cuando no se ha iniciado el comando o se detuvo. 
        /// Si no se había iniciado y ahora InProccess pasa a estar en true, 
        /// se pide el criterio con el id del employer del que quiere ver la calificación
        NoStarted,
        /// Inicio el comando, por lo que recoje el id el worker del que quiere verse la calificación
        Started
    }

    /// <summary>
    /// Representa los datos que va obteniendo el comando ShowRateWorkerCommand en los diferentes estados.
    /// </summary>
    public class ShowRateWorkerData
    {

        /// <summary>
        /// Resuesta que se guarda en cada paso que se va haciendo, así si entra otro usuario a ejecutar el comando, no se pierde lo que se 
        /// le había dicho a ese en particular
        /// </summary>
        /// <value></value>
        public string Response { get; set; }
    }
}













