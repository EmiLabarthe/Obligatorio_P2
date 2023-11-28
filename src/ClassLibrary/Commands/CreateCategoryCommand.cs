using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace ClassLibrary;

// Comando responsabilidad de Emiliano Labarthe.

/// <summary>
/// Comando encargado de crear una categoría.
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
public class CreateCategoryCommand : ICommand
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
    public CreateCategoryState State { get; private set; } = CreateCategoryState.NoStarted;

    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedo por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, CreateCategoryState> stateForUser = new Dictionary<long, CreateCategoryState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, CategoryData> data = new Dictionary<long, CategoryData>();
    
    private CategoryData dataOfCategory;


    
    /// <summary>
    /// Propiedad de solo lectura, compartida por el tipo ICommand, para que se sepa 
    /// si el comando se esta ejecutando (en cualquiera de sus pasos),  o si ya termino y 
    /// se puede pasar a otro.
    /// </summary>
    /// <value>true si todavía se esta ejecutando, false si ya termino o no ha empezado</value>/
    public bool InProccess { get; private set; } = false;



    /// <summary>
    /// Se encarga de asignarle un valor a la propiedad del nombre con el fin
    /// de diferenciar entre un comando y otro.
    /// </summary>
    public CreateCategoryCommand()
    {

        this.Name = "/createcategory";
    }

    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        stateForUser[userID] = CreateCategoryState.NoStarted;
        this.State = CreateCategoryState.NoStarted;
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
            this.dataOfCategory  = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.InProccess = true;
            this.State = CreateCategoryState.NoStarted;
            stateForUser[userID] = CreateCategoryState.NoStarted;
            dataOfCategory = new CategoryData();
            data[userID] = dataOfCategory;
        }

        // Poscondición
        if (!stateForUser.ContainsKey(userID))
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Revisa si es un admin, si es admin y no otro puede ejecutar este comando
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
        bool  canExecute = !userIsEmployer && !userIsWorker && userIsAdmin;
        return canExecute;
    }

    /// <summary>
    /// Operación polimorfica que le solicita al usuario (admin), que ingrese el nombre de la categoría que quiere crear,
    /// de tener un formato adecuado lo crea y le devuelve un mensaje de confirmación. En caso de que no le vuelve a pedir la 
    /// información. 
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Mensaje de que paso en cada paso de la ejecución del comando</returns>
    public string Execute(long userID, string inputData)
    {
        RestoreData(userID);
        if (this.State == CreateCategoryState.NoStarted)
        {
         dataOfCategory.Response = "Usted seleccionó crear una categoría. \nIngrese el nombre de la categoría que desea crear.";
         stateForUser[userID] = CreateCategoryState.Started;

        }
        else if (this.State == CreateCategoryState.Started)
        {
            //Validamos que el nombre este correcto
            if (!string.IsNullOrWhiteSpace(inputData))
            {
                dataOfCategory.Name = inputData;
                // Finalizamos el comando usando este metódo
                Cancel(userID);
                Database.Instance.AddCategory(dataOfCategory.Name);
                dataOfCategory.Response = "La categoría se ha creado con éxito.";
            }
            else
            {
                dataOfCategory.Response = "El nombre de la categoría no puede estar vacío.";
            }
        }
        else
        {
            dataOfCategory.Response = "Se canceló la ejecución";
        }
        // Ahora antes de que salga, guardamos el objeto de data para que no se pierda
        data[userID] = dataOfCategory;
        return dataOfCategory.Response;
    }

    /// <summary>
    /// Lista de los estados en los que puede estar este comando. La información se pide
    /// en el estado anterior, es decir, empieza a pedir información al usuario cuando esta en NoStarted
    ///  pero con el InProcess en true, y así sucesivamente. 
    /// </summary>
    public enum CreateCategoryState
    {
        /// Para cuando no se ha iniciado el comando o se detuvo. Cuando no ha iniciado y pasa a estar InProcess en tru
        ///pide el nombre de la categoría a crear.
        NoStarted,
        /// Recoje el nombre de la catogría pedido al inicio cuando "prende" el comando
        Started,
    }

        /// <summary>
        /// Representa los datos que va obteniendo el comando CreateCategoryCommand en los diferentes estados.
        /// </summary>
        public class CategoryData
        {
            /// <summary>
            /// El nombre que se ingresó en la primer entrada del comando.
            /// </summary>
            public string Name {get; set;}

            /// <summary>
            /// La respuesta a dar al usuario.
            /// </summary>
            public string Response {get; set;}
            
        }
}
