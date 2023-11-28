using System.Collections.Generic;
using System.Linq;
using Exceptions;

// Clase responsabilidad de Agustín Toya, Juan Pablo Amorín y Emiliano Labarthe

using System;

namespace ClassLibrary;
/// <summary>
/// Clase encargada de recibir lo que el usario ingresa por teclado, interpretarlo y ejecutar un comando en base a lo recibido.
/// También se encarga de contener todos los comandos que pueden usarse. En un futuro se aplicará la restricción por rol o por subtipo del tipo Profile
/// </summary>
public class InputInterfacer
{
    private IDictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
    /// <summary>
    /// Se guarda por usuario el comadno que esta ejecutando
    /// </summary>
    /// <returns></returns>
    private IDictionary<long, ICommand> lastCommandByUser = new Dictionary<long, ICommand>();


    private ICommand comInProcess = null;

    private static InputInterfacer instance;
    /// <summary>
    /// Property para hacer que input interfacer sea un Singleton
    /// </summary>
    /// <value></value>
    public static InputInterfacer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputInterfacer();
            }
            return instance;
        }
    }
    /// <summary>
    /// Constructor del objeto, se encarga de crear instancias de los comandos y almacenarlos, para luego poder usarlos. 
    /// </summary>
    private InputInterfacer()
    {
        // Comandos para usuario generico, sin estar en el sistema
        commands["/start"] = new StartCommand();
        commands["/registerasemployer"] = new RegisterAsEmployerCommand();
        commands["/registerasworker"] = new RegisterAsWorkerCommand();
        commands["/shownotifications"] = new ShowNotificationsCommand();
        commands["/showcategories"] = new ShowCategoriesCommand();
        // Comandos para el worker
        commands["/createworkoffer"] = new CreateWorkOfferCommand();
        commands["/rateemployer"] = new RateEmployerCommand();
        commands["/showemployerrating"] = new ShowRateEmployerCommand();
        commands["/responseaemployer"] = new ResponseAEmployerCommand();
        // commands.Add(new ());
        // Comandos para el employer
        commands["/showemployeerating"] = new ShowRateEmployerCommand();
        commands["/showworkerrating"] = new ShowRateWorkerCommand();
        commands["/showallworkoffers"] = new SearchAllWorkOfferCommand();
        commands["/searchfilteredworkoffer"] = new SearchFilteredWorkOfferCommand();
        commands["/searchsortedworkoffer"] = new SearchSortedStateWorkOfferCommand();
        commands["/hireemployee"] = new ContactWorkerCommand();
        commands["/rateworker"] = new RateWorkerCommand();

        // Comandos para el admin
        commands["/createcategory"] = new CreateCategoryCommand();
        commands["/deleteworkoffer"] = new DeleteWorkOfferCommand();
    }



    /// <summary>
    /// Método encargado de traducir la entrada de datos en un comando, para luego ser ejecutado
    /// </summary>
    /// <param name="userID"> Identificador de quien es el que ejecuto el comando</param>
    /// <param name="inputData">Los datos que llegan por la entrada o la interfaz gráfica</param>
    /// <returns>Devuleve el mensaje de confirmación del comando, el restulado de una busqueda o que no se encontro el comando especificado</returns>
    public string TranslateToCommand(long userID, string inputData)
    {
        string response = "";
        if(String.IsNullOrWhiteSpace(inputData) || userID < 0)
        {
            return "Los datos ingresados no pueden ser vacíos o ser solo espacios en blanco. \nPor favor ingrese información o cancele la ejecución del comando ingresando 'cancel'";
        }
        // Aquí formateamos el texto de entrada para eliminar desperfectos que lo hagan ser diferente
        // del nombre del comando, por ahora usemos el mayus, pero luego se usará el minus
        
        string nameComand = Format.RemoveAcentMarkToLower(inputData);
        // Revisa si el user ingreso la palabra cancel
        if (inputData.Trim().ToLower() == "cancel")
        {
            // Si la ingreso, revisa que haya algún comando asociado a su id vinculado a algún comando,
            // se cancela la ejecución del comando
            if (lastCommandByUser.ContainsKey(userID))
            {
                if (lastCommandByUser[userID] != null)
                {
                    // Se recupera el comando y se cancela
                    comInProcess = lastCommandByUser[userID];
                    response = comInProcess.Cancel(userID);
                    lastCommandByUser[userID] = null;
                }
                else
                {
                    response = "No se esta ejecutando ningún comando como para poder cancelarse";
                }

            }
            else
            {
                response = "Usted no ha ejecutado ningún comando todavía, como para poder cancelar algo";
            }
        }
        else
        {
            if (lastCommandByUser.ContainsKey(userID))
            {
                // Llega acá si el user no ha ejecutado un comando antes
                // Revisa si la data corresponde al nombre de un comando, si es, lo carga para registarlo o usarlo
                if (commands.ContainsKey(nameComand))
                {
                    // Revisa si ya esta en el user en el diccionario con un comando
                    if (lastCommandByUser[userID] != null)
                    {
                        // Revisamos si el comadno que quiere ejecutar es igual al que se esta corriendo
                        if (nameComand == lastCommandByUser[userID].Name)
                        {
                            response = $"El comando {nameComand} ya se esta ejecutando";
                        }
                        else
                        {
                            response = $"Se esta ejecutando el comando {lastCommandByUser[userID].Name}, para cancelar ingrese cancel o ingrese datos diferentes a los nombres de los comandos";
                        }
                    }
                    else
                    {
                        // El espacio es nulo, entonces no hay nada y se puede ejecutar uno nuevo
                        lastCommandByUser[userID] = commands[nameComand];
                        comInProcess = lastCommandByUser[userID];
                        if(comInProcess.ProfileCanExecute(userID))
                        {
                            response = comInProcess.Execute(userID, inputData);
                            FinishedCommandByUser(userID);
                        }
                        else
                        {
                            return "El tipo de perfil que tienes no tiene permito usar este comando";
                        }
                    }
                }
                else
                {
                    // Como el dato no coincide con ningún comando, habrá que ver si hay alguno que se este ejecutando ahora
                    if (lastCommandByUser[userID] != null)
                    {
                        comInProcess = lastCommandByUser[userID];
                        if(comInProcess.ProfileCanExecute(userID))
                        {
                            response = comInProcess.Execute(userID, inputData);
                            FinishedCommandByUser(userID);
                        }
                        else
                        {
                            return "El tipo de perfil que tienes no tiene permito usar este comando";
                        }
                    }
                    else
                    {
                        response = "Ese nombre no coincide con el de un comando y no hay ninguno ejecutandose";
                    }
                }
            }
            else
            {
                // Acá llega si nunca ejecuto un comando, entonces es aquí donde verificamos si puede usar ese comando que 
                // quiere usar ahora
                lastCommandByUser[userID] = null;
                if (commands.ContainsKey(nameComand))
                {
                    lastCommandByUser[userID] = commands[nameComand];
                    comInProcess = lastCommandByUser[userID];
                    if(comInProcess.ProfileCanExecute(userID))
                    {
                        response = comInProcess.Execute(userID, inputData);
                        FinishedCommandByUser(userID);
                    }
                    else
                    {
                        return "El tipo de perfil que tienes no tiene permito usar este comando";
                    }
                }
                else
                {
                    response = "Ese nombre no coincide con el de un comando";
                }
            }
        }






        return response;
    }

    private void FinishedCommandByUser(long userID)
    {
        if (!comInProcess.InProccess)
        {
            lastCommandByUser[userID] = null;
        }
    }
}
