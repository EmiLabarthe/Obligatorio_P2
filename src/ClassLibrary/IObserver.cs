namespace ClassLibrary;

// Clase responsabilidad de Agustín Toya.

/// <summary>
/// Interfaz que aplica el patrón Observer, siendo esta interfaz la que implementan 
/// los observadores, en este caso los workers. 
/// </summary>
public interface IObserver
{
    /// <summary>
    /// Método por el cual se le da parte al observador de que sucedio un cambio en 
    /// el observable
    /// </summary>
    /// <param name="message">message que recibe el usuario</param>
    public void Update(string message);
}