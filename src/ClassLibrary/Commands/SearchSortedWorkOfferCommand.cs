using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ucu.Poo.Locations.Client;
using System.Text;
using System;
namespace ClassLibrary;

// Comando responsabilidad de Agustín Toya.

/// <summary>
/// Comando encargado de buscar y devolver todas las ofertas de trabajo que se han hechos filtradas por la cateogoría indicada
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

public class SearchSortedStateWorkOfferCommand : ICommand
{
    /// <summary>
    /// Es el nombre que tendrá el comando y que lo diferenciará de otros comandos
    /// </summary>
    /// <value>El nombre que se le especifique</value>
    public string Name { get; }

    /// <summary>
    /// Propiedad que guía al propio comando en que paso debe estar ahora, y a sabiendas de eso, hace algo.
    /// El valor por defecto es que no inicio el comando, NoStarted
    /// </summary>
    /// <value></value>
    public SearchSortedState State { get; private set; } = SearchSortedState.NoStarted;

    /// <summary>
    /// Propiedad de solo lectura, compartida por el tipo ICommand, para que se sepa 
    /// si el comando se esta ejecutando (en cualquiera de sus pasos),  o si ya termino y 
    /// se puede pasar a otro.
    /// </summary>
    /// <value>true si todavía se esta ejecutando, false si ya termino o no ha empezado</value>/
    public bool InProccess { get; private set; } = false;

    /// <summary>
    /// Se encarga de asignarle un valor a la propiedad del nombre con el fin
    /// de diferenciar entre un comando y otro. También inicializa lo necesario 
    /// para hacer uso de la APILocation
    /// </summary>
    public SearchSortedStateWorkOfferCommand()
    {
        // Cargamos los nombres de los ordenadores posibles
        LocationApiClient client = new LocationApiClient();
        IDistanceCalculator calculator = new DistanceCalculator(client);
        sorters.Add("location", new SortByDistance(calculator));
        sorters.Add("rates", new SortByRate());
        // Cargamos el nombre del comando
        this.Name = "/searchsortedworkoffer";
    }

    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedo por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, SearchSortedState> stateForUser = new Dictionary<long, SearchSortedState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, SerachSortedData> data = new Dictionary<long, SerachSortedData>();

    private SerachSortedData dataOfSearch;

    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        stateForUser[userID] = SearchSortedState.NoStarted;
        this.InProccess = false;
        return $"El comando {Name} se cancelo";
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
        if (stateForUser.ContainsKey(userID))
        {
            this.State = stateForUser[userID];
            this.dataOfSearch = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.InProccess = true;
            this.State = SearchSortedState.NoStarted;
            stateForUser[userID] = SearchSortedState.NoStarted;
            dataOfSearch = new SerachSortedData();
            data[userID] = dataOfSearch;
        }

        // Poscondición
        if (!stateForUser.ContainsKey(userID))
        {
            throw new InvalidOperationException();
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
    /// Ordenadores que se pueden usar para ordenar el resultado querido
    /// </summary>
    /// <returns></returns>
    private IDictionary<string, ISort> sorters = new Dictionary<string, ISort>();

    /// <summary>
    /// Los que hace es buscar entre la lista de ordenadores, para ver si el nombre de 
    /// alguno concide con lo puesto en la entrada. Si concide lo usa y retorna el resultado 
    /// de la ordenación, siendo esto un texto con las ofertas ordenadas o un mensaje de alerta 
    /// si no se pudo realizar la tarea. 
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="inputData"></param>
   /// <returns>Mensaje de respuesta en base a lo que esta pasando en el paso actual</returns>
    public string Execute(long userID, string inputData)
    {
        RestoreData(userID);
        // Lista que contendrá las workOffer ordeandas
        string  workOffersSortedData = "";
        // Obtenemos una lista con los workers para poder brindar el nombre y apellido del 
        // creador además de su UserID. 
        ReadOnlyCollection<Worker> workers = Database.Instance.GetWorkers();
        // Contendor de la información de las ofertas
        StringBuilder result = new StringBuilder();
        List<string> validCriterions = sorters.Keys.ToList();
        string validCriterionsString = string.Join("\n->", validCriterions);
        if (this.State == SearchSortedState.NoStarted)
        {
            dataOfSearch.Response = $"Ingrese el criterio para poder ordenar \nCriterios validos: \n->{validCriterionsString}";
            stateForUser[userID] = SearchSortedState.Started;
        }
        else if (this.State == SearchSortedState.Started)
        {
            // La información que se recibe es la de el nombre o el criterio de ordenación,
            // que esta todo en mínusculas y no tiene tildes
            string sortedName = Format.RemoveAcentMarkToLower(inputData);
            // Ahora buscamos si existe un ordenador con ese nombre
            if (sorters.ContainsKey(sortedName))
            {
                ISort sorter = sorters[sortedName];
                workOffersSortedData = sorter.Order(userID);
                dataOfSearch.Response = workOffersSortedData;
                // Finalizamos el comando usando este metódo
                Cancel(userID);
            }
            else
            {
                dataOfSearch.Response = $"El citerio {inputData.ToLower().Trim()} no concide con ningun criterio vlaido, revise que haya escrito bien";
            }
        }
        else
        {
            dataOfSearch.Response = string.Empty;
        }
        return dataOfSearch.Response;
    }
    

        /// <summary>
        /// Indica los diferentes estados que puede tener el comando SerachSortedWorkOfferCommand.
        /// </summary>
        public enum SearchSortedState
        {
            /// Para cuando no se ha iniciado el comando o se detuvo. 
            /// Si no se había iniciado y ahora InProccess pasa a estar en true, 
            /// se pide el criterio con el cual ordenar las ofertas
            NoStarted,
            /// Inicio el comando, por lo que recoje el criterio
            Started
        }

        /// <summary>
        /// Representa los datos que va obteniendo el comando SearchSortedWorkOfferCommand en los diferentes estados.
        /// </summary>
        public class SerachSortedData
        {
            /// <summary>
            /// La descripción que se ingresó en el estado SerachSortedState.Started.
            /// </summary>
            public string Option { get; set; }

            /// <summary>
            /// Resuesta que se guarda en cada paso que se va haciendo, así si entra otro usuario a ejecutar el comando, no se pierde lo que se 
            /// le había dicho a ese en particular
            /// </summary>
            /// <value></value>
            public string Response { get; set; }


        }
}

