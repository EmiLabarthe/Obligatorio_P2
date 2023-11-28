using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace ClassLibrary;

// Comando responsabilidad de Agustín Toya y Emiliano Labarthe

/// <summary>
/// Comando encargado de crear una oferta de trabajo con los datos que se le van pasando. 
/// También controla si sos uno de los user habilitados para hacer uso de este comando, como el resto de clases concretas
/// que implementan el tipo ICommand.
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
public class CreateWorkOfferCommand : ICommand
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
    public CreateWorkOfferState State { get; private set; } = CreateWorkOfferState.NoStarted;

  
    // Lista de monedas disponibles 
    private List<string> currencies = new List<string>() { "USD", "UYU", "$U", "U$S" };

    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedo por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, CreateWorkOfferState> stateForUser = new Dictionary<long, CreateWorkOfferState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, WorkOfferData> data = new Dictionary<long, WorkOfferData>();
    
    private WorkOfferData dataOfWorkOffer;


    
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
    public CreateWorkOfferCommand()
    {

        this.Name = "/createworkoffer";
    }

    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        dataOfWorkOffer.Categories.Clear();
        stateForUser[userID] = CreateWorkOfferState.NoStarted;
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
            this.dataOfWorkOffer  = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.InProccess = true;
            this.State = CreateWorkOfferState.NoStarted;
            stateForUser[userID] = CreateWorkOfferState.NoStarted;
            dataOfWorkOffer = new WorkOfferData();
            data[userID] = dataOfWorkOffer;
        }

        // Poscondición
        if (!stateForUser.ContainsKey(userID))
        {
            throw new InvalidOperationException();
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
    /// Operación polimorfica que le solicita al usuario (worker), que ingrese los diferentes datos necesarios para crear una oferta de trabajo.
    /// Si estan bien esos datos desde un principio, cambia al siguiente paso, de no ser así, permanece ahí y vuelve a pedir la inforamción que 
    /// se ingresó mal. Finalmente, si esta todo bien y no se cancelo antes, crea una oferta de trabajo, con la información especificada. 
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Mensaje de que paso en cada paso de la ejecución del comando</returns>
    public string Execute(long userID, string inputData)
    {
        RestoreData(userID);
        if (this.State == CreateWorkOfferState.NoStarted)
        {
         dataOfWorkOffer.Response = "Ingrese la descripción de la oferta de trabajo";
         stateForUser[userID] = CreateWorkOfferState.Started;

        }
        else if (this.State == CreateWorkOfferState.Started)
        {
            //Validamos que la descripción este correcta
            if (!string.IsNullOrWhiteSpace(inputData) && inputData.Length > 6)
            {
                dataOfWorkOffer.Description = inputData;
                stateForUser[userID] = CreateWorkOfferState.CurrencyPromt;
                dataOfWorkOffer.Response = "Ingrese el simbolo de la moneda que quiere usar. \nLos valores validos pueden ser  USD, UYU, $U y U$S";
            }
            else
            {
                dataOfWorkOffer.Response = "La descripción no puede estar vacía y debe ser mayor a 6 letras.";
            }
        }
        else if (this.State == CreateWorkOfferState.CurrencyPromt)
        {
            // A reciveData la ponemos en mayus por las dudas y le quitamos el espacio en blanco del principio y el final
            inputData = inputData.ToUpper().Trim();
            bool currencyValid = currencies.Contains(inputData);
            if (currencyValid)
            {
                dataOfWorkOffer.Currency = inputData;
                stateForUser[userID] = CreateWorkOfferState.PricePromt;
                dataOfWorkOffer.Response = "Ingrese el precio que tendrá la oferta";
            }
            else
            {
                dataOfWorkOffer.Response = "La moneda especificada no esta dentro de las validas, las expresioens validas son: USD, UYU, $U y U$S";
            }
        }
        else if (this.State == CreateWorkOfferState.PricePromt)
        {
            // Le quietamos los espacios que pueda tener 
            inputData = inputData.Trim();
            try
            {

                int priceParse = Int32.Parse(inputData);
                if (priceParse > 0)
                {
                    stateForUser[userID] = CreateWorkOfferState.DurationInDaysPromt;
                    dataOfWorkOffer.Price = priceParse;
                    dataOfWorkOffer.Response = "Ingrese la duración en días que tendrá la oferta, si se trata de horas ponga un día";
                }
                else
                {
                    dataOfWorkOffer.Response = "Ingrese un precio mayor a 0, sus servicios no pueden ser gratis, por ahora";
                }

            }
            catch (FormatException)
            {
                dataOfWorkOffer.Response = "El precio debe ser un valor númerico entero";
            }
        }
        else if (this.State == CreateWorkOfferState.DurationInDaysPromt)
        {
            // Le quietamos los espacios que pueda tener 
            inputData = inputData.Trim();
            try
            {
                int days = Int32.Parse(inputData);
                if (days > 0)
                {
                    dataOfWorkOffer.DurationInDays = days;
                    stateForUser[userID] = CreateWorkOfferState.CategoriesPromt;
                    dataOfWorkOffer.Response = "Ingrese una categoría que tendrá esta oferta, para dejar de ingresar categorías ingrese el comando 'done'";
                }
                else
                {
                    dataOfWorkOffer.Response = "La duración no puede ser menor a 0, si quiere referirse a un trabajo que se hace en un día, ponga 1";
                }
            }
            catch (FormatException)
            {
                dataOfWorkOffer.Response = "La duración del trabajo debe ser un valor númerico entero";
            }
        }
        else if (this.State == CreateWorkOfferState.CategoriesPromt)
        {
            if (!string.IsNullOrWhiteSpace(inputData))
            {

                if (inputData.ToLower() != "done")
                {
                    // Obtenemos la lista de posibles categorías 
                    ReadOnlyCollection<string> posibleCategory = Database.Instance.GetAllCategories();
                    string formatedCategory = Format.RemoveAcentMarkToUpper(inputData.Trim());
                    // Validmos que exista en la lista de categorías del sistema

                    if (!posibleCategory.Contains(formatedCategory))
                    {
                        dataOfWorkOffer.Response = $"La cateogoría {inputData} no existe. Revise que el nombre ese bien puesto o si existe esa categoría realmente";
                    }
                    else
                    {
                        dataOfWorkOffer.Categories.Add(inputData);
                        dataOfWorkOffer.Response = "Ingrese otra cateogoría, recuerde, 'done' para dejar de ingresar categorías";
                    }
                }
                else
                {
                    if (dataOfWorkOffer.Categories.Count > 0)
                    {
                        // Ya se valido que todas las categorías especificadas o la categoría especificada esta en el sistema
                        // ahora agrega la workOffer
                        List<string>  categoriesOfWorkOffer = dataOfWorkOffer.Categories.ToList();
                        string categoriesReturns = string.Join(", ", categoriesOfWorkOffer);
                        Database.Instance.AddWorkOffer(dataOfWorkOffer.Description, dataOfWorkOffer.Currency, dataOfWorkOffer.Price, userID, dataOfWorkOffer.Categories, dataOfWorkOffer.DurationInDays);
                        dataOfWorkOffer.Response = $"Se ha creado correctamente la oferta de trabajo. \nDescripción: {dataOfWorkOffer.Description} \nPrecio: {dataOfWorkOffer.Price} {dataOfWorkOffer.Currency} \nCategoría(s): {categoriesReturns} \nDuración: {dataOfWorkOffer.DurationInDays}";
                        // Finalizamos el comando usando este metódo
                        Cancel(userID);

                    }
                    else
                    {
                        dataOfWorkOffer.Response = "No ha ingresado ninguna categoría, ingrese una por favor";
                    }
                }
            }
            else
            {
                dataOfWorkOffer.Response = "Por favor ingrese información o cancele la operación del comando con 'cancel'";
            }
        }
        else
        {
            dataOfWorkOffer.Response = "Se canceló la ejecución";
        }
        // Ahora antes de que salga, guardamos el objeto de data para que no se pierda
        data[userID] = dataOfWorkOffer;
        return dataOfWorkOffer.Response;
    }

    /// <summary>
    /// Lista de los estados en los que puede estar este comando. La información se pide
    /// en el estado anterior, es decir, empieza a pedir información al usuario cuando esta en NoStarted
    ///  pero con el InProcess en true, y así sucesivamente. 
    /// </summary>
    public enum CreateWorkOfferState
    {
        /// Para cuando no se ha iniciado el comando o se detuvo. Si no se ha iniciado y se pasa el InProcess a true,
        /// se le pide el primer dato, la descripción de la oferta
        NoStarted,
        /// Cuando se inicio el comando, y recoje el dato pedido cuando paso a estar "prendido": la despcrpción.
        Started,
        /// Cuando el comando recoje la moneda a usar
        CurrencyPromt,
        /// Cuando el comando recoje el precio
        PricePromt,
        /// Cuando el comando recoje la o las categorías
        CategoriesPromt,
        /// Cuando el comando recoje la cantidad de días que dura la oferta
        DurationInDaysPromt,
    }

        /// <summary>
        /// Representa los datos que va obteniendo el comando CreateWorkOfferCommand en los diferentes estados.
        /// </summary>
        public class WorkOfferData
        {
            /// <summary>
            /// La descripción que se ingresó en el estado CreateWorkOfferState.Started.
            /// </summary>
            public string Description  {get; set;}
            
            /// <summary>
            /// La moneda que se ingresó en el estado CreateWorkOfferState.CurrencyPromt.
            /// </summary>
            public string Currency  {get; set;}

            /// <summary>
            /// El precio que se ingresó en el estado CreateWorkOfferState.PricePromt.
            /// </summary>
            public int Price  {get; set;}
            
            /// <summary>
            /// Las categorías ingresadas en el estado CreateWorkOfferState.CategoriesPromt.
            /// </summary>
            public List<string> Categories = new List<string>();
            
            /// <summary>
            /// La duración del trabajo que se ingresó en el estado CreateWorkOfferState.DurationInDaysPromt.
            /// </summary>
            public int DurationInDays  {get; set;}
            
            /// <summary>
            /// Resuesta que se guarda en cada paso que se va haciendo, así si entra otro usuario a ejecutar el comando, no se pierde lo que se 
            /// le había dicho a ese en particular
            /// </summary>
            /// <value></value>
            public string Response {get; set;}

            
        }
}

