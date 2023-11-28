using System.Collections.Generic;

namespace ClassLibrary;

// Clase responsabilidad de Agustín Toya.

/// <summary>
///  Interfaz que aplica el patrón Observer. En este caso es la parte
/// del observable, de lo que se observa. siendo en nuestro caso las ofertas de trabajo, 
/// y la propiedad que dispara la notificación de que sucedio algo es la de IsPublished, haciendo 
/// referencia si se dio de baja una oferta o no. 
/// </summary>
public interface IObservable
{
    /// <summary>
    /// Guardamos los observadores que puede llegar a tener un objeto de este tipo
    /// </summary>
    /// <value></value>
    public List<IObserver> Observers { get; }

    /// <summary>
    /// Agrega un subscriptor a recibir una notificación por el cambio que se espera manejar.
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(IObserver observer);

    /// <summary>
    /// Elimina un subscriptor de la lista de los que querían recibir
    /// una notificación por el cambio que se espera manejar. 
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IObserver observer);

    /// <summary>
    /// Método encargado de revisar que subscriptores hay y notificarlos sobre el cambio previsto
    /// </summary>
    public void Notify();
}