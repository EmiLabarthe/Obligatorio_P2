using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime;

// Comando responsabilidad de Emiliano Labarthe y Agustín Toya.

namespace ClassLibrary;

/// <summary>
/// Comando que realiza la gestión para que un trabajador califique a un empleador.
/// 
///  Sigue el principio de inversión de dependencias puesto que depende de una abstracción (ICommand), 
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
public class RateEmployerCommand : ICommand
{
    /// <summary>
    /// Nombre del comando, el que debe coincidir con la entrada que el usuario inserta al bot.
    /// </summary>
    /// <value></value>
    public string Name { get; }

    /// <summary>
    /// Propiedad de solo lectura, compartida por el tipo ICommand, para que se sepa 
    /// si el comando se esta ejecutando (en cualquiera de sus pasos),  o si ya termino y 
    /// se puede pasar a otro.
    /// </summary>
    /// <value>true si todavía se esta ejecutando, false si ya termino o no ha empezado</value>/
    public bool InProccess { get; private set; } = false;

    /// <summary>
    /// Constructor que determina el nombre del comando.
    /// </summary>
    public RateEmployerCommand()
    {
        this.Name = "/rateemployer";
    }

    /// <summary>
    /// Propiedad que guía al propio comando en que paso debe estar ahora, y a sabiendas de eso, hace algo.
    /// El valor por defecto es que no inicio el comando, NoStarted
    /// </summary>
    /// <value></value>
    public RateEmployerState State { get; private set; } = RateEmployerState.NoStarted;



    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedó por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, RateEmployerState> stateForUser = new Dictionary<long, RateEmployerState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, RateData> data = new Dictionary<long, RateData>();

    private RateData ratedInstance;


    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        stateForUser[userID] = RateEmployerState.NoStarted;
        this.State = RateEmployerState.NoStarted;
        this.InProccess = false;
        return $"El comando {Name} se canceló";
    }

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
            this.ratedInstance = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.State = RateEmployerState.NoStarted;
            stateForUser[userID] = RateEmployerState.NoStarted;
            ratedInstance = new RateData();
            data[userID] = ratedInstance;
            this.InProccess = true;
        }

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
    /// Operación polimorfica que le solicita al usuario (worker), que ingrese el identificador del employer
    /// al que se quiere calificar.
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Mensaje de que paso en cada paso de la ejecución del comando</returns>
    public string Execute(long userID, string inputData)
    {
        RestoreData(userID);
        int inputDataInt = 0;

        if (this.State == RateEmployerState.NoStarted)
        {
            ratedInstance.Response = "Ejecutaste el comando para calificar a un empleador. \nIngrese el ID del usuario a calificar.";
            stateForUser[userID] = RateEmployerState.Started;
        }
        else if (this.State == RateEmployerState.Started)
        {
            //Validamos que exista el empleador con ese ID
            long inputDataLong = 0;
            //Para usar el exist hay que convertir el string en long
            try
            {
                inputDataLong = Int64.Parse(inputData.Trim());
            }
            catch (FormatException)
            {
                ratedInstance.Response = "El dato ingresado deber ser un valor númerico entero";
            }
            if (Database.Instance.ExistEmployer(inputDataLong))
            {
                ratedInstance.CalificatedEmployer = Database.Instance.SearchEmployer(inputDataLong);
                stateForUser[userID] = RateEmployerState.WorkOfferPromt;
                ratedInstance.Response = "Ingrese el ID de la oferta de trabajo que lo vincula.";
            }
            else
            {
                ratedInstance.Response = "Esa ID no corresponde a ningún empleador.";
            }
        }
        else if (this.State == RateEmployerState.WorkOfferPromt)
        {
            // Verificamos que exista la oferta de trabajo
            try
            {
                inputDataInt = Int32.Parse(inputData);
            }
            catch (FormatException)
            {
                return "El valor debe ser un número.";
            }
            if (Database.Instance.ExistWorkOffer(inputDataInt))
            {
                ratedInstance.WorkOfferBased = Database.Instance.SearchWorkOffer(inputDataInt);
                stateForUser[userID] = RateEmployerState.RatePromt;
                ratedInstance.Response = "Ingrese una calificación del 1 al 10 para el empleador.";
            }
            else
            {
                return "Esa ID no corresponde a ninguna oferta de trabajo.";
            }
        }
        else if (this.State == RateEmployerState.RatePromt)
        {
            // Verificamos que la calificación sea del 1 al 10.
            try
            {
                inputDataInt = Int32.Parse(inputData);
            }
            catch (FormatException)
            {
                return "El valor debe ser un número.";
            }
            if (inputDataInt >= 1 && inputDataInt <= 10)
            {
                // Si se valida, el trabajador pasa a ser calificado.
                ratedInstance.Rate = inputDataInt;
                ratedInstance.CalificatedEmployer.AddRating(userID, ratedInstance.WorkOfferBased.DurationInDays, ratedInstance.WorkOfferBased.Identify);
                ratedInstance.CalificatedEmployer.ReciveCalification(inputDataInt, userID, ratedInstance.WorkOfferBased.Identify, true);
                // Finalizamos el comando usando este metódo
                Cancel(userID);
                ratedInstance.Response = "Calificación realizada.";
            }
            else
            {
                ratedInstance.Response = "Debe ingresar un número del 1 al 10.";
            }
        }
        else
        {
            ratedInstance.Response = "Se canceló la ejecución";
        }
        data[userID] = ratedInstance;
        return ratedInstance.Response;
    }
    /// <summary>
    /// Estados que puede tener el comando.
    /// </summary>
    public enum RateEmployerState
    {
        /// Para cuando no se ha iniciado el comando o se detuvo. 
        /// Si no se había iniciado y ahora InProccess pasa a estar en true, se pide el
        /// identificador del employer
        NoStarted,
        /// Cuando inicia el comando, recoje el id del que se quiere calificar, pedido en el paso anterior.
        Started,
        /// Pide Work Offer que relaciona al empleador y trabajador.
        WorkOfferPromt,
        /// Pasa a pedir la calificación a enviar.
        RatePromt
    }

    /// <summary>
    /// Representa los datos que va obteniendo el comando en los diferentes estados.
    /// </summary>
    public class RateData
    {
        /// <summary>
        /// El empleador calificado.
        /// </summary>
        public Employer CalificatedEmployer { get; set; }

        /// <summary>
        /// La calificación a ingresar al empleador.
        /// </summary>
        public int Rate { get; set; }

        /// <summary>
        /// En qué WorkOffer se basa para calificar.
        /// </summary>
        /// <value></value>
        public WorkOffer WorkOfferBased { get; set; }

        /// <summary>
        /// La respuesta a devolver al usuario.
        /// </summary>
        /// <value></value>
        public string Response { get; set; }
    }
}