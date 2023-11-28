using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System;
using Exceptions;
using System.Linq;
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
public class SearchFilteredWorkOfferCommand : ICommand
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

    private IDictionary<string, IFilter> filters = new Dictionary<string, IFilter>();

    /// <summary>
    /// Se encarga de asignarle un valor a la propiedad del nombre con el fin
    /// de diferenciar entre un comando y otro
    /// </summary>
    public SearchFilteredWorkOfferCommand()
    {

        this.Name = "/searchfilteredworkoffer";
        filters["category"] = new FilterByCategory();
    } 

    /// <summary>
    /// Propiedad que guía al propio comando en que paso debe estar ahora, y a sabiendas de eso, hace algo.
    /// El valor por defecto es que no inicio el comando, NoStarted
    /// </summary>
    /// <value></value>
    public SearchFilteredState State { get; private set; } = SearchFilteredState.NoStarted;

    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedo por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, SearchFilteredState> stateForUser = new Dictionary<long, SearchFilteredState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, SearchFilteredData> data = new Dictionary<long, SearchFilteredData>();

    private SearchFilteredData dataOfSearch;


    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        stateForUser[userID] = SearchFilteredState.NoStarted;
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
            this.dataOfSearch = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.InProccess = true;
            this.State = SearchFilteredState.NoStarted;
            stateForUser[userID] = SearchFilteredState.NoStarted;
            dataOfSearch = new SearchFilteredData();
            data[userID] = dataOfSearch;
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
    /// Operación polimorfica que se encarga de devolver todas las ofertas de trabajo siendo filtradas por cierto criterio
    /// que se le solicita al usuario. 
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns>Mensaje de respuesta en base a lo que esta pasando en el paso actual</returns>
    public string Execute(long userID, string inputData)
    {

        RestoreData(userID);
        if(this.State == SearchFilteredState.NoStarted)
        {  
            List<string> validCriterions = filters.Keys.ToList();
            string validCriterionsString = string.Join("\n->", validCriterions);
            dataOfSearch.Response = $"Esta ejecutando el comando de filtrar ofertas de trabajo. Ingrese uno de los siguientes criterios validos: \n->{validCriterionsString}";
            stateForUser[userID] = SearchFilteredState.Started;
        }
        else if(this.State == SearchFilteredState.Started)
        {
            // Primero formateamos la entrada para estar al mismo estilo de los nombres de los filtros
            string nameOfFilter = Format.RemoveAcentMarkToLower(inputData);
            // Revisamos que exista un filtro con ese nombre
            if(filters.ContainsKey(nameOfFilter))
            {
                dataOfSearch.FilterAplicated = filters[nameOfFilter];;
                stateForUser[userID] = SearchFilteredState.CriterionPromt;
                dataOfSearch.Response = $"Perfecto, ahora se usará ese filtro. {dataOfSearch.FilterAplicated.MessageForFilter}";
            }
            else
            {
                dataOfSearch.Response = "No se ha encontrado ningún filtro que concida con el que especifico";
            }
        }
        else if(this.State == SearchFilteredState.CriterionPromt)
        {
            // Ahora, usamos el filtro
            string resultFilter = dataOfSearch.FilterAplicated.Fiilter(inputData);
            if(resultFilter == "La lista de ofertas de trabajo esta vacía, no hay nada para filtrar" || resultFilter != "No existe esa categoría en el sistema")
            {
                // Finalizamos el comando usando este metódo
                Cancel(userID);
            }
            dataOfSearch.Response = resultFilter;
        }
        return dataOfSearch.Response;
    }

        /// <summary>
        /// Diferentes estados por los que puede pasar este comando
        /// </summary>
        public enum SearchFilteredState
        {
            /// Para cuando no se ha iniciado el comando o se detuvo. 
            /// Si no se había iniciado y ahora InProccess pasa a estar en true, 
            /// se pide el criterio con el cual filtrar las ofertas
            NoStarted,
            /// Inicio el comando, por lo que pide el criterio para filtrar
            Started,
            /// Criterio que se le pasa al filtro
            CriterionPromt
        }

        /// <summary>
        /// Representa los datos que va obteniendo el comando SearchFilteredWorkOfferCommand en los diferentes estados.
        /// </summary>
        public class SearchFilteredData
        {
            /// <summary>
            /// Guardo el filtro que va a user en base a lo que ingresó en el estado SearchFilteredState.Started.
            /// </summary>
            public IFilter FilterAplicated { get; set; }

            /// <summary>
            /// Resuesta que se guarda en cada paso que se va haciendo, así si entra otro usuario a ejecutar el comando, no se pierde lo que se 
            /// le había dicho a ese en particular
            /// </summary>
            /// <value></value>
            public string Response { get; set; }
        }
}