using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exceptions;

using System.Text;
using System;
namespace ClassLibrary;

// Comando responsabilidad de Juan Pablo Amorín y Emiliano Labarthe.

/// <summary>
/// Comando que se ejecuta cuando el usuario inserta /start.
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
public class StartCommand : ICommand
{
    /// <summary>
    /// String que debe coincidir con la entrada del usuario.
    /// </summary>
    /// <value></value>
    public string Name {get;}

    /// <summary>
    /// Construye con el string necesario para asociarse con el InputInterfacer.
    /// </summary>
    public StartCommand()
    {
        this.Name = "/start";
    }

    /// <summary>
    /// Cualquier usuario de Telegram puede usar este comando
    /// </summary>
    /// <returns></returns>
    public bool ProfileCanExecute(long userID)
    {
        // Como es un comando informativo, siempre puede usarse, seas lo que seas
        return true;
    }

    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <value></value>
    public bool InProccess { get; set; } = false; 
    
    /// <summary>
    /// Método cancel necesario por la interfaz. Este comando al ser de un solo paso, no necesita usar cancel.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        // Intencionalmente devuelve un string vacío
        return "";
    }

    /// <summary>
    /// Se encarga de resturar los datos que genero el comando cuando entro el user la vez pasada o en otra oportunidad
    /// Si es la primera vez que entra, lo pone en el diccionario
    /// </summary>
    /// <param dataOfWorker.Name="userID"></param>
    public void RestoreData(long userID)
    {
        // Intencionalmente vacío
    }

    /// <summary>
    /// Devuelve la lista de comandos existentes.
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Devuelve la lista de comandos disponibles que tienes dependiendo de si estas en el sitema o no y que rol tengas</returns>
    public string Execute(long userID, string inputData)
    {
        // Obtenemos la información de si es un worker, un employer, un admin o ninguno de los anteriores
        bool userIsEmployer = Database.Instance.ExistEmployer(userID);
        bool userIsWorker = Database.Instance.ExistWorker(userID);
        bool userIsAdmin = Database.Instance.ExistAdmin(userID);
        bool notStrnger = userIsAdmin || userIsEmployer || userIsWorker;
        // Respuesta del comando
        string response = "";
        // Si es un usuario de telegram no registrado en el sistema
        if(!notStrnger)
        {
             response = "Hola! Me presento, soy tu bot designado para ayudarte a conseguir trabajo o conseguir a la persona que necesites para tus tareas. \n\nPuedes comenzar ejecutando los comandos en el menú de la izquierda o en la siguiente lista. \n/start - Ver lista de comandos\n/registerasemployer - Registrarse como empleador\n/registerasworker - Registrarse como trabajador";
        }
        else if(userIsWorker)
        {
            // Respuesta para los comandos del trabajador
            response = "\nComandos que tiene un trabajador: \n/shownotifications - Mostrar notificaciones\n/showcategories - Mostrar categorías de trabajo\n/createworkoffer - Crear oferta de trabajo\n/rateemployer - Calificar empleador\n/showemployerrating - Mostrar rating del empleado \n/responseaemployer - Te permite responder a un intento de contacto de un trabajador respecto a una de tus ofertas";
        }
        else if(userIsEmployer)
        {
            // Respuesta para los comandos del empleador
            response = "\nComandos que tiene un empleador: \n/showemployeerating - Mostrar rating del trabajador\n/showcategories - Mostrar categorías de trabajo\n/showallworkoffers - Mostrar todas las ofertas de trabajo\n/searchfilteredworkoffers - Mostrar las ofertas de trabajo filtradas\n/searchsortedworkoffers - Mostrar las ofertas de trabajo ordenadas\n/hireemployee - Te permite contactar a un trabajador por una oferta que te interesa\n/rateworker - Calificar trabajador";
        }
        else
        {
            // El último caso que queda es el admin
            response =  "\nComandos de un administrador: \n/createcategory - Crear categoría de trabajo\n/showcategories - Mostrar categorías de trabajoS\n/deleteworkoffer - Eliminar oferta de trabajo";
        }
        return response;
    }
}