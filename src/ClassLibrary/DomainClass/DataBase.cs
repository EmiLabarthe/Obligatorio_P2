using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Exceptions;
namespace ClassLibrary;

// Clase responsabilidad de Agustín Toya.

/// <summary>
/// Clase que se encargará de contener los datos del programa, siendo estos las ofertas de servicios,
/// las categorías, los trabajadores, los administradores y los empleadores. 
/// Para diseñarla se uso el principio Expert, ya que consideramos que la clase que alojara objetos era la más adecuada para 
/// devolver la información sobre los objetos, así como para crearlos, quitarlos de la lista, decir si existe uno en base a su identificador,
/// o devolver un objeto en particular al especificarse una información.
/// 
/// Por otro lado, esta clase es un Singleton para así poder tener una única instancia de los datos mientras se ejecuta el programa, 
/// de lo contrario podría darse que existierán dos instancias con datos diferentes al mismo tiempo. 
/// </summary>
public class Database
{
    /// <summary>
    /// Variable privada que contiene la instancia del objeto, siguiendo con la forma de hacer un Singleton
    /// </summary>
    private static Database instance;

    /// <summary>
    ///  Para saber cual es la última id de work offer. Cuando se agrega una oferta se incrementa en uno, y ese termina siendo el identify de esa oferta
    /// </summary>
    public int UltimateIDWorkOffer { get; private set; }

    /// <summary>
    /// Property que se encarga de devolver la instancia de Database si existe, 
    /// si no existe la crea y la devuelve. De esta manera siempre se accede a la misma instancia 
    /// </summary>
    public static Database Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Database();
            }
            return instance;
        }
    }

    /// <summary>
    /// Decidimos que las categorias serían un nombre, por ende tenemos una lista de strings
    /// donde serán agregados y consultados los nombres de la categorias durante el programa.
    /// 
    /// No se usa un IList porque luego el método GetAllCategories devuelve una lista de solo
    /// lectura, usando el método AsReadOnly que tiene el tipo List. Esto se hace para mantener
    /// la encapsualción y la estabilidad de los datos, evitando que se cambien datos de los objetos
    /// (listas), que se pasan.
    /// </summary>
    /// <returns></returns>
    private List<string> categories = new List<string>();

    /// <summary>
    /// Lista que contendrá las ofertas de trabajo que se van creando en el sistema.
    /// No se usa un IList porque luego el método GetAllWorkOffers devuelve una lista de solo
    /// lectura, usando el método AsReadOnly que tiene el tipo List. Esto se hace para mantener
    /// la encapsualción y la estabilidad de los datos, evitando que se cambien datos de los objetos
    /// (listas), que se pasan.
    /// </summary>
    /// <returns></returns>
    private List<WorkOffer> workOffers = new List<WorkOffer>();

    /// <summary>
    /// Lista que contiene a todos los trabajadores que estaran en al aplicación.
    /// No se usa un IList porque luego el método GetWorkers devuelve una lista de solo
    /// lectura, usando el método AsReadOnly que tiene el tipo List. Esto se hace para mantener
    /// la encapsualción y la estabilidad de los datos, evitando que se cambien datos de los objetos
    /// (listas), que se pasan.
    /// </summary>
    /// <returns></returns>
    private List<Worker> workers = new List<Worker>();

    /// <summary>
    /// Lista que contiene a todos los empleadores que estaran en al aplicación.
    /// No se usa un IList porque luego el método GetEmployers devuelve una lista de solo
    /// lectura, usando el método AsReadOnly que tiene el tipo List. Esto se hace para mantener
    /// la encapsualción y la estabilidad de los datos, evitando que se cambien datos de los objetos
    /// (listas), que se pasan.
    /// </summary>
    /// <returns></returns>
    private List<Employer> employers = new List<Employer>();

    /// <summary>
    /// Lista que contiene a todos los administradores que estaran en al aplicación.
    /// No se usa un IList porque luego el método GetAdmins devuelve una lista de solo
    /// lectura, usando el método AsReadOnly que tiene el tipo List. Esto se hace para mantener
    /// la encapsualción y la estabilidad de los datos, evitando que se cambien datos de los objetos
    /// (listas), que se pasan.
    /// </summary>
    /// <returns></returns>
    private List<Admin> admins = new List<Admin>();

    /// <summary>
    /// Constructor privado para implementar el patrón Singleton
    /// </summary>
    private Database()
    {
        UltimateIDWorkOffer = 0;
    }

    /// <summary>
    /// Método que se encarga de crear una oferta de trabajo aplicando el patrón Creator, ya que que se le pasa
    /// todos los datos necesarios para construir el objeto. A su vez, se puede aplicar creator por que la clase Database
    /// contiene objetos de este tipo en una lista. 
    /// </summary>
    /// <param name="description"> Descripción de la oferta; los datos que quiera ingrear el worker para presentar su trabajo</param>
    /// <param name="currency"> Moneda que tendrá la oferta de trabajo</param>
    /// <param name="price"> Cuanto dinero vale el trabajo</param>
    /// <param name="ownerID"> Identificador del usuario que creo la oferta de trabajo</param>
    /// <param name="categoriesInput"> Lista de categorías en las que esta publicada la oferta</param>
    /// <param name="durationInDays"> Duración en días del trabajo en particular</param>
    public void AddWorkOffer(string description, string currency, int price, long ownerID, IList<string> categoriesInput, int durationInDays)
    {
        List<string> catsFormated = new List<string>();
        List<string> currenciesValids = new List<string>() { "USD", "UYU", "$U", "U$S" };
        // Hay que formatear lo que llega por las dudas
        string currencyFormated = Format.RemoveAcentMarkToUpper(currency);
        // Primero revisa que los datos que son string no esten vacíos, nulos o en espacios en blanco
        if (!string.IsNullOrWhiteSpace(description) && !string.IsNullOrWhiteSpace(currency) && ownerID > 0 && currenciesValids.Contains(currencyFormated))
        {
            //Revisa que el ownerID sea un worker
            bool workerExist = ExistWorker(ownerID);
            if (workerExist)
            {
                // Revisa que los valores númericos esten correctos y que la lista no este vacía
                if (price > 0 && durationInDays > 0 && categoriesInput.Count > 0)
                {
                    //Ahora revisa si las categorías que llegan estan en la lista de categorías que tiene la base de datos
                    bool categoriesValid = true;

                    foreach (string cat in categoriesInput)
                    {
                        categoriesValid &= categories.Contains(Format.RemoveAcentMarkToUpper(cat));
                        if(categoriesValid)
                        {
                            catsFormated.Add(Format.RemoveAcentMarkToUpper(cat));
                        }
                    }
                    // Se busca si hay alguna oferta que tenga los mismos datos, por si el mismo worker ingreso exactamente dos veces 
                    // los mismos datos
                    int counterOffersMatch = MatachWorkOffer(description, currency, price, ownerID ,  categories, durationInDays);
                    if (categoriesValid && counterOffersMatch == 0)
                    {
                        WorkOffer nuevaOferta = new WorkOffer(UltimateIDWorkOffer + 1, description, currency, price, ownerID, catsFormated, durationInDays);
                        Worker owner = this.SearchWorker(ownerID);
                        // Agregamos al worker propietario como parte de la lista de observadores
                        nuevaOferta.AddObserver(owner);
                        // Incrementamos el identificador de work offers
                        UltimateIDWorkOffer += 1;
                        // Agregamos la work offer
                        workOffers.Add(nuevaOferta);
                    }
                }
            }

        }

    }
    /// <summary>
    /// Devuleve los workOffers para que puedan usarla los comandos, pero siendo solo de lectura
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<WorkOffer> GetAllWorkOffers()
    {
        return workOffers.AsReadOnly();
    }

    /// <summary>
    /// Método encargado de crear nuevas categorías y agregarlas al contendor, en este caso una lista. En el caso de recibir 
    /// nombres en un formato no valido (minúsculas y con tildes), se cambia al formato correcto para esta información (mayúsculas y sin tildes). 
    /// </summary>
    /// <param name="categoryName"> nombre de la categoría o categoría en si, que se va a agregar</param>
    public void AddCategory(string categoryName)
    {
        // Se controla que no llegue información erronea, levantando una excepción puesto que nunca debería de pasar
        Check.Precondition(!string.IsNullOrWhiteSpace(categoryName), "La categoría nunca puede ser nula, vacía o ser espacios en blanco");
        string formatedcategoryName = Format.RemoveAcentMarkToUpper(categoryName);
        // Ahora hay que revisar si la categoría, con el formateo aplicado, esta en la lista o no
        if (!categories.Contains(formatedcategoryName))
        {
            categories.Add(formatedcategoryName.Trim());

        }
    }

    /// <summary>
    /// Devuleve las categorias para que puedan usarla los comandos, pero son de solo lectura
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<string> GetAllCategories()
    {
        return categories.AsReadOnly();
    }

    /// <summary>
    /// Se encarga de dar de baja una oferta de trabajo
    /// Si existe alguna oferta con ese identificador procederá a "eliminarla"
    /// </summary>
    /// <param name="identify"> Identificador de la oferta de trabajo</param>
    public void DeleteWorkOffer(int identify)
    {
        bool existWorkOffer = ExistWorkOffer(identify);
        if (existWorkOffer)
        {
            WorkOffer offer = SearchWorkOffer(identify);
            offer.Delete();
        }
    }

    /// <summary>
    /// Se encarga de crear y agregar el trabajador a la lista de trabajadores. 
    /// En base al patrón Creator es que le pasamos los datos para crear el objeto, esto debido
    /// a que cumple con la condición de que la clase Database agrega objetos de tipo Worker.
    /// </summary>
    /// <param name="userID"> ID de la plataforma que le llega al user</param>
    /// <param name="name"> Nombre del trabajador</param>
    /// <param name="lastName"> Apellido del trabajador</param>
    /// <param name="phone"> Celular del trabajador</param>
    /// <param name="address"> Dirección del worker</param>
    /// <param name="latitude"> Latitud de la ubicación del worker</param>
    /// <param name="longitude"> Longitud de la ubicación del worker</param>
    public void AddWorker(long userID, string name, string lastName, string phone, string address, double latitude, double longitude)
    {
        if (!String.IsNullOrWhiteSpace(name) && (!String.IsNullOrWhiteSpace(lastName)) && !String.IsNullOrWhiteSpace(phone))
        {
            //Revisamos que no exista esa userID en la lista de workers, employes o admins
            bool alreadyExistProfile = ExistAdmin(userID) || ExistWorker(userID) || ExistEmployer(userID);

            // Ahora revisa que el número de telefono tenga el formato adecuado al hacer uso de una expresión regular
            Regex regex = new Regex("^09[1-9]{1}[0-9]{6}$");
            Match validPhone = regex.Match(phone);
            if (validPhone.Success && !alreadyExistProfile)
            {
                Worker worker = new Worker(userID, name, lastName, phone, address, latitude, longitude);
                workers.Add(worker);
            }


        }
    }



    /// <summary>
    /// Se encarga de crear y agregar el empleador a la lista de empledores. 
    /// En base a creator es que le pasamos los datos para crear el objeto, esto debido
    /// a que cumple con la condición de que la clase Database agrega objetos de tipo Employer
    /// </summary>
    /// <param name="userID"> ID de la plataforma que le llega al user</param>
    /// <param name="name"> Nombre del empleador</param>
    /// <param name="lastName"> Apellido del empleador</param>
    /// <param name="phone"> Celular del empleador</param>
    /// <param name="address"> Dirección del employer</param>
    /// <param name="latitude"> Latitud de la ubicación del employer</param>
    /// <param name="longitude"> Longitud de la ubicación del employer</param>
    public void AddEmployer(long userID, string name, string lastName, string phone, string address, double latitude, double longitude)
    {
        if (!String.IsNullOrWhiteSpace(name) && (!String.IsNullOrWhiteSpace(lastName)) && !String.IsNullOrWhiteSpace(phone) && phone.Length == 9)
        {
            //Revisamos que no exista esa userID en la lista de workers, employes o admins
            bool alreadyExistProfile = ExistAdmin(userID) || ExistWorker(userID) || ExistEmployer(userID);

            // Ahora revisa que el número de telefono tenga el formato adecuado al hacer uso de una expresión regular
            Regex regex = new Regex("^09[1-9]{1}[0-9]{6}$");
            Match validPhone = regex.Match(phone);
            if (validPhone.Success && !alreadyExistProfile)
            {
                Employer employer = new Employer(userID, name, lastName, phone, address, latitude, longitude);
                employers.Add(employer);
            }
        }
    }


    /// <summary>
    /// Se encarga de crear y agregar el admin a la lista de administradores. 
    /// En base a Creator es que le pasamos los datos para crear el objeto, esto debido
    /// a que cumple con la condición de que la clase Database agrega objetos de tipo Admin
    /// </summary>

    public void AddAdmin(long userID)
    {
        //Revisamos que no exista esa userID en la lista de workers, employes o admins
        bool alreadyExistProfile = ExistAdmin(userID) || ExistWorker(userID) || ExistEmployer(userID);
        if (!alreadyExistProfile)
        {
            Admin admin = new Admin(userID);
            admins.Add(admin);
        }

    }

    /// <summary>
    /// Se encarga de devolver los trabajadores de forma que pueda userse la información, pero 
    /// sin tocarla en la lista.
    /// </summary>
    /// <returns>Devulve la lista de trabajadores que contiene Database</returns>
    public ReadOnlyCollection<Worker> GetWorkers()
    {
        return workers.AsReadOnly();
    }

    /// <summary>
    /// Se encarga de devolver los empleadores de forma que pueda userse la información, pero 
    /// sin tocarla en la lista
    /// </summary>
    /// <returns>Devulve la lista de empleadores que contiene Database</returns>
    public ReadOnlyCollection<Employer> GetEmployers()
    {
        return employers.AsReadOnly();
    }

    /// <summary>
    /// Se encarga de devolver los administradores de forma que pueda userse la información, pero 
    /// sin tocarla en la lista.
    /// </summary>
    /// <returns>Devulve la lista de administradores que contiene Database pero como una lista de solo lectura</returns>
    public ReadOnlyCollection<Admin> GetAdmins()
    {
        return admins.AsReadOnly();
    }

    // Limpiadores de listas para los tests

    /// <summary>
    /// Limpia la lista de los workers, pero solo se usa en los test
    /// </summary>
    public void ClearWorkers()
    {
        workers.Clear();
    }

    /// <summary>
    /// Limpia la lista de los employers, pero solo se usa en los test
    /// </summary>
    public void ClearEmployers()
    {
        employers.Clear();
    }

    /// <summary>
    /// Limpia la lista de los employers, pero solo se usa en los test
    /// </summary>
    public void ClearAdmins()
    {
        admins.Clear();
    }

    /// <summary>
    /// Limpia la lista de los categorías, pero solo se usa en los test
    /// </summary>
    public void ClearCategories()
    {
        categories.Clear();
    }

    /// <summary>
    /// Limpia la lista de los ofertas de trabajo, se usa en los test
    /// </summary>
    public void ClearWorkOffers()
    {
        workOffers.Clear();
    }

    // Para comprobar si existe el Administrador, Trabajador, Empleador u oferta de trabajo al pasarse su identificador único

    /// <summary>
    /// Busca si hay algún admin que concida con ese identificador en el sistema
    /// </summary>
    /// <param name="userID">identificador del admin que se busca obtener</param>
    /// <returns>Retorna true si encuentra alguna coicidencia con el identificador
    ///  pasado, devuelve false si no encuentra coicidencia</returns>
    public bool ExistAdmin(long userID)
    {
        foreach (Admin item in admins)
        {
            if (item.UserID == userID)
            {
                return true;
            }
        }
        return false;

    }

    /// <summary>
    /// Busca si hay algún worker que concida con ese identificador
    /// </summary>
    /// <param name="userID">identificador del worker que se busca obtener</param>
    /// <returns>Retorna true si encuentra alguna coicidencia con el identificador
    ///  pasado, devuelve false si no encuentra coicidencia</returns>
    public bool ExistWorker(long userID)
    {
        foreach (Worker item in workers)
        {
            if (item.UserID == userID)
            {
                return true;
            }
        }
        return false;

    }

    /// <summary>
    /// Busca si hay algún employer que concida con ese identificador
    /// </summary>
    /// <param name="userID">identificador del employer que se busca obtener</param>
    /// <returns>Retorna true si encuentra alguna coicidencia con el identificador
    ///  pasado, devuelve false si no encuentra coicidencia</returns>
    public bool ExistEmployer(long userID)
    {
        foreach (Employer item in employers)
        {
            if (item.UserID == userID)
            {
                return true;
            }
        }
        return false;

    }
    
    /// <summary>
    /// Se encarga de revisar si existe una oferta de trabajo con ese identificador
    /// </summary>
    /// <param name="identify"></param>
    /// <returns>True si existe una workOffer con ese identificador o false si no existe</returns>
    public bool ExistWorkOffer(int identify)
    {
        foreach (WorkOffer offer in workOffers)
        {
            if (offer.Identify == identify)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Se encarga de revisar si existe una coicidencia para una oferta de trabajo que sea igual, así 
    /// sabremos si ya esta en el sistema o si no existe tal oferta. Esto debido a que al ser el identify un autoincrementable, 
    /// puede ocurrír que se ingrese dos veces la misma información
    /// </summary>
    /// <param name="description">Descripción de la oferta a buscar</param>
    /// <param name="currency">Moneda de la oferta a buscar</param>
    /// <param name="price">Precio de la oferta</param>
    /// <param name="ownerID">Identificador del dueño</param>
    /// <param name="inputCategories">Categorías que tiene la oferta a buscar</param>
    /// <param name="durationInDays">Duración en días de la oferta</param>
    /// <returns></returns>
    public int MatachWorkOffer(string description,  string currency, int price, long ownerID, List<string> inputCategories,  int durationInDays)
    {
        // Para saber si la lista de categorías pasadas es igual a la que tiene la oferta que se 
        // esta investigando, hay que recorrer ambas listas e ir comparando miembro a miembro
        bool categoriesExist = true;
        int cantWorkOffersMatch = 0;
        foreach (WorkOffer offer in Database.Instance.GetAllWorkOffers())
        {
            categoriesExist = true;
            // Si la lista de categoría que llega tiene la misma cantidad que la de la oferta en cuestión, podría ser que tengan lo mismo
            // por ende pasa a comparar miembro a miembro
            if (offer.GetCateogories().Count == inputCategories.Count)
            {
                for (int j = 0; j < inputCategories.Count; j++)
                {
                    string formatedcategoryName = Format.RemoveAcentMarkToUpper(inputCategories[j].Trim());
                    if (formatedcategoryName == offer.GetCateogories()[j])
                    {
                        categoriesExist &= true;
                    }
                    else
                    {
                        categoriesExist &= false;
                    }
                }
            }
            // Comparamos que todos los datos sean iguales, si es así aumentamos el contador de coincidencias. 
            if (offer.Currency == currency && offer.Price == price && offer.Description == description && categoriesExist && offer.OwnerWorkerID == ownerID && offer.DurationInDays == durationInDays)
            {
                cantWorkOffersMatch += 1;
            }
        }
        return cantWorkOffersMatch;
    }

    // Fin de la parte de los comprobadores

    // Parte de los buscadores de un objeto en base a su identificador único

    /// <summary>
    /// Se encarga de devolver el objeto que este en la lista de ofertas de trabajo si es que hay alguno que 
    /// tenga el mismo identify que el que se recibe por parametros. 
    /// </summary>
    /// <param name="identify"></param>
    /// <returns>El objeto workOffer si es que hay uno, o null si no econtró nada</returns>
    public WorkOffer SearchWorkOffer(int identify)
    {
        // Aprovecha el método para saber si existe la oferta y no hacer un foreach, gastando recursos de computo en vano
        if (ExistWorkOffer(identify))
        {
            foreach (WorkOffer offer in workOffers)
            {
                if (offer.Identify == identify)
                {
                    return offer;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Se encarga de devolver el objeto worker que corresponde al userID pasado
    /// </summary>
    /// <param name="workerID"></param>
    /// <returns>Devuelve el objeto si existe un worker con ese userID, si no existe devuelve null</returns>
    public Worker SearchWorker(long workerID)
    {
        // Aprovecha el método para saber si existe el trabajdor y no hacer un foreach, gastando recursos de computo en vano
        if (ExistWorker(workerID))
        {
            foreach (Worker worker in workers)
            {
                if (worker.UserID == workerID)
                {
                    return worker;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Se encarga de devolver el objeto employer que corresponde al userID pasado
    /// </summary>
    /// <param name="employerID"></param>
    /// <returns>Devuelve el objeto si existe un employer con ese userID, si no existe devuelve null</returns>
    public Employer SearchEmployer(long employerID)
    {
        // Aprovecha el método para saber si existe el empleador y no hacer un foreach, gastando recursos de computo en vano
        if (ExistEmployer(employerID))
        {
            foreach (Employer employer in employers)
            {
                if (employer.UserID == employerID)
                {
                    return employer;
                }
            }
        }
        return null;
    }
}
