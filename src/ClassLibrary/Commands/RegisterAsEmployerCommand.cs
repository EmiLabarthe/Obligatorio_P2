using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Ucu.Poo.Locations.Client;

namespace ClassLibrary;

// Comando responsabilidad de Emiliano Labarthe y Agustín Toya.

/// <summary>
/// Comando necesario que realiza la gestión para que un usuario se registre como empleador.
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
public class RegisterAsEmployerCommand : ICommand
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
    /// También inicializa lo necesario para hacer uso de la APILocation
    /// </summary>
    public RegisterAsEmployerCommand()
    {
        this.Name = "/registerasemployer";
        LocationApiClient client = new LocationApiClient();
        this.finder = new AddressFinder(client);
    }

    /// <summary>
    /// Propiedad que guía al propio comando en que paso debe estar ahora, y a sabiendas de eso, hace algo.
    /// El valor por defecto es que no inicio el comando, NoStarted
    /// </summary>
    /// <value></value>
    public RegisterAsEmployerState State { get; private set; } = RegisterAsEmployerState.NoStarted;

  

    /// <summary>
    /// Diccionario que guarda el userID de cada usuario que ha ejecutado el comando y le guarda el estado en el que 
    /// quedo por última vez, así no se pisa entre la entrada de uno y la otra.
    /// </summary>
    /// <returns></returns>
    private Dictionary<long, RegisterAsEmployerState> stateForUser = new Dictionary<long, RegisterAsEmployerState>();

    /// <summary>
    /// Los datos  para un usuario que va obteniendo el comando en los diferentes estados.
    /// </summary>
    private Dictionary<long, EmployerData> data = new Dictionary<long, EmployerData>();
    
    private EmployerData dataOfEmployer;

    // Un buscador de direcciones. Permite que la forma de encontrar una dirección se determine en tiempo de
    // ejecución: en el código final se asigna un objeto que use una API para buscar direcciones; y en los casos de
    // prueba se asigne un objeto que retorne un resultado que puede ser configurado desde el caso de prueba.
    private IAddressFinder finder;

     /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        this.State = RegisterAsEmployerState.NoStarted;
        stateForUser[userID] = RegisterAsEmployerState.NoStarted;
        this.InProccess = false;
        return $"El comando {Name} se canceló";
    }

    /// <summary>
    /// Se encarga de resturar los datos que genero el comando cuando entro el user la vez pasada o en otra oportunidad
    /// Si es la primera vez que entra, lo pone en el diccionario
    /// </summary>
    /// <param dataOfEmployer.Name="userID"></param>
    public void RestoreData(long userID)
    {
        // Si el user ya hizo uso de este comando, estará en esta lista, 
        // si no esta, hay que agregarlo
        if(stateForUser.ContainsKey(userID))
        {
            this.State  = stateForUser[userID];
            this.dataOfEmployer  = data[userID];
            this.InProccess = true;
        }
        else
        {
            this.State = RegisterAsEmployerState.NoStarted;
            stateForUser[userID] = RegisterAsEmployerState.NoStarted;
            dataOfEmployer = new EmployerData();
            data[userID] = dataOfEmployer;
            this.InProccess = true;
        }

    }

    /// <summary>
    /// Revisa si no es un user del sistema debe poder usarlo
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
        bool  canExecute = !userIsEmployer && !userIsWorker && !userIsAdmin;
        return canExecute;
    }
    

    /// <summary>
    /// Operación polimorifica que en este caso se encarga de ir tomando los datos necesarios 
    /// para registrar/crear un user como employer. Si los datos están mal lo obliga a ingresarlos bien
    /// o a cancelar el comando.
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Mensaje de que paso en cada paso de la ejecución del comando</returns>
    public string Execute(long userID, string inputData)
    {
        RestoreData(userID);
        inputData = inputData.Trim();
        if (this.State == RegisterAsEmployerState.NoStarted)
        {
         dataOfEmployer.Response = "Ahora pasaremos a pedir los datos para crear un empleador. \nCuál es su nombre?";
         stateForUser[userID] = RegisterAsEmployerState.Started;
        }
        else if (this.State == RegisterAsEmployerState.Started)
        {
            //Validamos que el nombre este correcto
            if (!string.IsNullOrWhiteSpace(inputData))
            {
                dataOfEmployer.Name = inputData;
                stateForUser[userID] = RegisterAsEmployerState.LastNamePromt;
                dataOfEmployer.Response = "Ingrese su apellido.";
            }
            else
            {
                dataOfEmployer.Response = "Su nombre no puede estar vacío.";
            }
        }
        else if (this.State == RegisterAsEmployerState.LastNamePromt)
        {
            //Validamos que el apellido este correcto
            if (!string.IsNullOrWhiteSpace(inputData))
            {
                dataOfEmployer.LastName = inputData;
                stateForUser[userID] = RegisterAsEmployerState.PhonePromt;
                dataOfEmployer.Response = "Ingrese su número de celular. solo los números.";
            }
            else
            {
                dataOfEmployer.Response = "Su apellido no puede estar vacío.";
            }
        }
        else if (this.State == RegisterAsEmployerState.PhonePromt)
        {
            
            Regex regex = new Regex("^09[1-9]{1}[0-9]{6}$");
            Match match = regex.Match(inputData);
            //Validamos que el número esté correcto
            if (inputData.Length == 9)
            {
                if(match.Success)
                {
                    dataOfEmployer.Phone = inputData;
                    stateForUser[userID] = RegisterAsEmployerState.FormatAddressLocation;
                    dataOfEmployer.Response = "Ingrese su dirección por favor.";
                }
                else
                {
                    dataOfEmployer.Response = "El celular que ingreso no tiene un formato valido. El formato valido es: 09[1-9]xxxxxx. Cambiando las x por los números correspondientes";
                }
            }
            else
            {
                dataOfEmployer.Response = "Su número de celular debe tener 9 dígitos.";
            }
        }
        else if (this.State == RegisterAsEmployerState.FormatAddressLocation)
        {
            //Validamos que el nombre de la ubicación esté correcto
            if (!string.IsNullOrWhiteSpace(inputData))
            {
                dataOfEmployer.Address = inputData;
                // Ahora revisa si la ubicación es valida
                this.dataOfEmployer.Result = this.finder.GetLocation(this.dataOfEmployer.Address);
                // Hay un tema con la API, a pesar de no encontrar una dirección valida, como deja la ciudad
                // el departamento y el país por defecto, siempre haya algo. Entonces, si no puede formar el 
                // Addres o FormatAddres, es que no la encontro

                if (this.dataOfEmployer.Result.Found)
                {
                    // Si encuentra la dirección pasa nuevamente al estado Initial
                    Database.Instance.AddEmployer(userID, dataOfEmployer.Name, dataOfEmployer.LastName, dataOfEmployer.Phone, dataOfEmployer.FormattedAddress, dataOfEmployer.Result.Latitude, dataOfEmployer.Result.Longitude);
                    // Finalizamos el comando usando este metódo
                    Cancel(userID);
                    // Esto me parece que debería ir con Check
                    if(Database.Instance.ExistEmployer(userID))
                    {
                        dataOfEmployer.Response = $"Se hizo correctamente el registro, ahora eres un empleador. \nNombre: {dataOfEmployer.Name} {dataOfEmployer.LastName} \nTeléfono: {dataOfEmployer.Phone} \nUbicación: {dataOfEmployer.FormattedAddress}";
                    }
                    else
                    {
                        dataOfEmployer.Response = "No se pudo completar la tarea por un error del sistema";
                    }
                }
                else
                {
                    // Si no encuentra la dirección se la pide de nuevo y queda en el estado AddressPrompt
                    dataOfEmployer.Response = "No encuentro la dirección. Decime qué dirección querés buscar por favor";
                }
            }
            else
            {
                dataOfEmployer.Response = "Su ubicación debe ser una palabra o palabras válidas.";
            }
        }
       
        else
        {
            dataOfEmployer.Response = "Se canceló la ejecución";
        }
        data[userID] = dataOfEmployer;
        return dataOfEmployer.Response;
    }

    /// <summary>
    /// Determina los estados que tiene el comando.
    /// </summary>
    public enum RegisterAsEmployerState
    {
        /// Para cuando no se ha iniciado el comando o se detuvo. 
        /// Si no se había iniciado y ahora InProccess pasa a estar en true, 
        /// se pide el nombre del usuario a registrarse. 
        NoStarted,
        /// Cuando inicia el comando y recoje el dato del nombre pedido en el paso anterior
        Started,
        /// Cuando el comando recoje el apellido
        LastNamePromt,
        /// Cuando el comando recoje el teléfono
        PhonePromt,
        /// Cuando el comando recoje el nombre de la ubicación
        FormatAddressLocation

    }

       /// <summary>
        /// Representa los datos que va obteniendo el comando RegisterAsEmployerCommand en los diferentes estados.
        /// </summary>
        public class EmployerData
        {
            /// <summary>
            /// El nombre que se ingresó en el estado RegisterAsEmployerState.Started.
            /// </summary>
            public string Name  {get; set;}
            
            /// <summary>
            /// El apellido que se ingresó en el estado RegisterAsEmployerState.LastNameProm.
            /// /// </summary>
            public string LastName  {get; set;}

            /// <summary>
            /// El número que se ingresó en el estado RegisterAsEmployerState.PhonePromt.
            /// /// </summary>
            public string Phone  {get; set;}

            /// <summary>
            /// Resultado de la busqueda, en objetos de tipo
            ///  IAddressREsult se guerda la información de la ubicación,
            /// así como si fue encontrada o no. 
            /// </summary>
            /// <value></value>
            public IAddressResult Result {get; set;}
         
            /// <summary>
            /// La ubicación que se ingresó en el estado RegisterAsEmployerState.FormatAddressLocation.
            /// /// </summary>
            public string Address  {get; set;}

            /// <summary>
            /// Dirección formateada que es uno de los datos que devuelve el buscador de direcciones
            /// </summary>
            /// <value></value>
            public string FormattedAddress
            {
                get
                {
                    string address = "";
                    if(Result == null)
                    {
                        address = "No hay una dirección asignada";
                    }
                    else
                    {
                        address = Result.FormatAddress;
                    }
                    return address;
                }
            }

            
            /// <summary>
            /// Resuesta que se guarda en cada paso que se va haciendo, así si entra otro usuario a ejecutar el comando, no se pierde lo que se 
            /// le había dicho a ese en particular
            /// </summary>
            /// <value></value>
            public string Response {get; set;}

            
        }
}