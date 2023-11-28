using System.Threading.Tasks;

// Clase responsabilidad de Agustín Toya.

namespace ClassLibrary
{
    /// <summary>
    /// Una interfaz define una abstracción para un buscador de direcciones genérico.
    /// </summary>
    /// <remarks>
    /// Esta interfaz fue creada siguiendo el principio de inversión de dependencias para evitar que los comandos
    /// concretos dependan de buscadores de direcciones concretos; en su lugar los comandos concretos dependen de esta
    /// abstracción.
    /// Entre otras cosas est permite cambiar el buscador de direcciones en tiempo de ejecución, para utilizar uno en
    /// los casos de prueba que retorna resultados conocidos para direcciones conocidas, y otro en la versión final para
    /// buscar usando una API de localizaciones.
    /// </remarks>
    public interface IAddressFinder
    {
        /// <summary>
        /// Determina si existe una dirección.
        /// </summary>
        /// <param name="address">La dirección a buscar.</param>
        /// <returns>Un objeto de una clase que implemente la interfaz IAddressResult con el resultado de la búsqueda, que
        /// incluye si la dirección se encontró o no, y si se encontró, la latitud y la longitud de la dirección.</returns>
        IAddressResult GetLocation(string address);
    }

    /// <summary>
    /// Interfaz que representa la abstracción de los resultados de la búsqueda. Cumple con DIP ya que no se va a depender de una clase concreta
    /// sino de esta abstracción que será implementada por al menos una clase concreta. Al poder sustituirse en tiempo de ejecución
    /// por cualquier clase concreta que implemente el tipo también cumple con LSP.
    /// </summary>
    public interface IAddressResult
    {
        /// <summary>
        /// Indica si se encontró o no la dirección. En ese caso son válidos los demás valores. En caso contrario los
        /// demás valores son indeterminados.
        /// </summary>
        /// <value>true si se encontró la dirección; false en caso contrario.</value>
        bool Found { get; }

        /// <summary>
        /// La latitud de la dirección.
        /// </summary>
        /// <value>El valor de la latitud en formato decimal.</value>
        double Latitude { get; }

        /// <summary>
        /// La longitud de la dirección.
        /// </summary>
        /// <value>El valor de la longitud en formato decimal.</value>
        double Longitude { get; }
        /// <summary>
        /// Dirección con formato normal, como por ejemplo Av. 8 de Octubre 2012
        /// </summary>
        /// <value></value>
        string FormatAddress {get; }
    }
}