using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Exceptions;

namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe y Agustín Toya.

/// <summary>
/// Clase creada para representar a una oferta de trabajo siguiendo el principio de Expert, puesto que 
/// es la experta en todo lo que tiene que ver a los datos de una oferta de trabajo, en este contexto. 
/// </summary>
public class WorkOffer : IObservable

{
    /// <summary>
    /// Identificador dado por la base de datos a la oferta, con tal de hacerla distinta de otras
    /// </summary>
    /// <value>Int correspondiente al identificador</value>
    public int Identify { get; private set; }
    
    /// <summary>
    /// Descripción dada por el worker que la crea
    /// </summary>
    /// <value>String que corresponde a la descripción dada por su creador</value>
    public string Description { get; private set; }

    /// <summary>
    /// Esta propiedad se refiere a la moneda que tendría el precio de la oferta.
    /// </summary>
    /// <value>String que contiene el simbolo o simbolos de la moneda a usar</value>
    public string Currency { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public int Price { get; private set; }
    // Trabajador que publico esta oferta
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public long OwnerWorkerID { get; private set; }
    // Para saber si la oferta sigue publicada o fue dada de baja por un administrador.
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public bool IsPublished { get; private set; }

    /// <summary>
    /// Esta propiedad se refiere a cuanto aproximadamente sería el trabajo o de por cuanto sería tiempo
    /// sería el trabajo.
    /// </summary>
    /// <value>Valor de cuantos días dura el trabajo o servicio en particular</value>
    public int DurationInDays { get; private set; }

    /// <summary>
    /// Guardamos los observadores que puede llegar a tener un objeto de este tipo.
    /// </summary>
    /// <value></value>
    /// 
    public List<IObserver> Observers { get; }
    
    /// <summary>
    /// Almacenamos a que categorías pertenece la oferta 
    /// </summary>
    private List<string> categories;


    /// <summary>
    ///  Constructor encargado de crear la oferta de trabajo
    /// </summary>
    /// <param name="identify">Identificador único dado por la base de datos</param>
    /// <param name="description">Descripción dada por el worker que la crea</param>
    /// <param name="currency">Moneda que tendrá la oferta, habiendo cuatro simolos validos USD, UYU, $U, U$S</param>
    /// <param name="price">Precio que tendrá la oferta</param>
    /// <param name="ownerWorkerID">Identificador del dueño</param>
    /// <param name="categories">Lista de categorías que tendrá la oferta</param>
    /// <param name="durationInDays">Duración del trabajo en días</param>    
    public WorkOffer(int identify, string description, string currency, int price, long ownerWorkerID, List<string> categories, int durationInDays)
    {
        // Monedas validas
        List<string> currenciesValids = new List<string>() { "USD", "UYU", "$U", "U$S" };
        
        // Revisamos las precondiciones
        Check.Precondition(identify > 0, "El indentificador de la oferta debe ser mayor a 0");
        Check.Precondition(!String.IsNullOrWhiteSpace(description), "La descripción no puede ser nula, vacía o ser solo espacios en blanco");
        Check.Precondition(!String.IsNullOrWhiteSpace(currency), "La moneda no puede ser nula, vacía o ser solo espacios en blanco");
        // Una vez que sabemos que no es null, empty o whiteSpace
        string currencyCheck = currency.Trim().ToUpper();
        Check.Precondition(currenciesValids.Contains(currencyCheck), "La moneda tiene que ser una de las siguientes: USD', UYU, $U, U$S");
        Check.Precondition(price > 0, "El precio tiene que ser mayor a 0, no se ofrecen servicios gratuitos en esta plataforma");
        Check.Precondition(ownerWorkerID > 0, "El id del trabajador que creo la oferta nunca sería menor a 1");
        // En el caso de las categorías hay cuatro casos, 1 que la lista venga vacía, 2. que haya una categoría null, 3. que haya una empty, 4 que haya una whitspace
        Check.Precondition(categories.Count > 0, "No pueden haber ofertas sin categorías");
        // Podemos comprobar los ultimos tres casos de forma rápida:
        int lisNottHaveInvalidCategory = 0;
        foreach(string word in categories)
        {
            if(string.IsNullOrWhiteSpace(word))
            {
                lisNottHaveInvalidCategory +=1 ;
            }
        }
        Check.Precondition(lisNottHaveInvalidCategory == 0, "No pueden haber categorías nulas, vacías o que sean espacios en blanco");
        Check.Precondition(durationInDays > 0, "La duración de un trabajo no puede ser menor a un día");

        this.Identify = identify;
        this.Description = description;
        this.Currency = currency;
        this.Price = price;
        this.OwnerWorkerID = ownerWorkerID;
        this.categories = categories;
        this.DurationInDays = durationInDays;
        this.IsPublished = true;
        this.Observers = new List<IObserver>();

        Check.Postcondition(this.Identify == identify, "Ocurrio un error en el sistema y no se pudo cargar el identificador de la oferta");
        Check.Postcondition(this.Description == description, "Ocurrio un error en el sistema y no se pudo cargar la descripción de la oferta");
        Check.Postcondition(this.Currency == currency, "Ocurrio un error en el sistema y no se pudo cargar la moneda que tendrá su oferta");
        Check.Postcondition(this.OwnerWorkerID == ownerWorkerID, "Ocurrio un error en el sistema y no se pudo cargar el identificador de la oferta");
        Check.Postcondition(this.categories == categories, "Ocurrio un error en el sistema y no se pudo cargar la descripción de la oferta");
        Check.Postcondition(this.DurationInDays == durationInDays, "Ocurrio un error en el sistema y no se pudo cargar la moneda que tendrá su oferta");

        Check.Invariant(this.IsPublished == true, "Cambiara cuadno se de de baja, no todavía");
        Check.Invariant(this.Observers != null, "La variable que contiene a la lista de observadores no puede ser null luego de haberse crado este objeto");
    }





    /// <summary>
    /// Agrega un subscriptor a recibir una notificación por el cambio que se espera manejar, 
    /// siguiendo con lo pactado en el patrón Observer
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(IObserver observer)
    {
        this.Observers.Add(observer);
    }

    /// <summary>
    /// Elimina un subscriptor de la lista de los que querían recibir
    /// una notificación por el cambio que se espera manejar. 
    /// Esto se hace siguiendo el patrón Observer
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IObserver observer)
    {
        this.Observers.Remove(observer);
        this.Notify();
    }

    /// <summary>
    /// Método asociados al patrón de diseño Observer. Encargado de notificar al usuario si su oferta fue eliminada.
    /// </summary>
    public void Notify()
    {   
        // Recorre la lista y va haciendoles saber a los subscriptores del cambio en el estado de la oferta
        foreach (IObserver observer in this.Observers)
        {
            observer.Update($"Su oferta {this.Identify} fue eliminada.");
        }
    }
    /// <summary>
    /// Método que elimina una oferta y dado el patrón Observer, notifica a sus subscriptores.
    /// </summary>
    public void Delete()
    {
        this.IsPublished = false;
        this.Notify();
    }

    /// <summary>
    /// Devulve las categorías de la oferta de trabajo, como una lista de solo lectura
    /// </summary>
    /// <returns>Lista de solo lectura</returns>
    public ReadOnlyCollection<string> GetCateogories()
    {
        return this.categories.AsReadOnly();
    }
}