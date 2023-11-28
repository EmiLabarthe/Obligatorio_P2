using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exceptions;

using System.Text;
using System;
namespace ClassLibrary;

// Comando responsabilidad de Juan Pablo Amorín.

/// <summary>
/// Comando encargado de hacer el contacto entre un empleador y un empleado,
/// haciendo referencia a que quiere contratarlo para un servicio en particular
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
public class ShowCategoriesCommand : ICommand
{
    /// <summary>
    /// Nombre del comando
    /// </summary>
    /// <value></value>
    public string Name {get;}

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
    public ShowCategoriesCommand()
    {
        this.Name = "/showcategories";
    }
    /// <summary>
    /// Cancela la ejecución del comando
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID)
    {
        this.InProccess = false;
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
    /// Devuelve la lista de categorías existentes en el sistema.
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="inputData"> Los datos que tiene que llegarle al comando</param>
    /// <returns> Mensaje de que paso en cada paso de la ejecución del comando</returns>
    public string Execute(long userID, string inputData)
    {
        this.InProccess = true;
        StringBuilder result = new StringBuilder();
        // Primero revisamos si hay categorías o la lista esta vacía
        if(Database.Instance.GetAllCategories().Count != 0)
        {
            result.Append("Las categorías existentes son:\n");
            foreach (string category in Database.Instance.GetAllCategories())
            {
                result.Append($"{category}\n");
            }
            Cancel(userID);
            return result.ToString(); 
        }
        else
        {
            Cancel(userID);
            return "En la base de datos no hay categorías creadas";
        }
    
    }
 
}