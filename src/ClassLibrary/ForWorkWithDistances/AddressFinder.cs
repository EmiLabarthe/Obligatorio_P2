using Ucu.Poo.Locations.Client;
using Nito.AsyncEx;

// Clase responsabilidad de Agustín Toya.

namespace ClassLibrary
{
    /// <summary>
    /// Un buscador de direcciones concreto que usa una API de localización.
    /// Cumple con el principio de inversión de dependencias, por que esta clase depende de una abstracción 
    /// y las clases que la usan, definen el tipo como IAddressFinder, cumpliendo a su vez con LSP, puesto que en tiempo
    /// de ejecución se puede definir que buscar contreto usar. A su vez, cumple con Polimorfismo, ya que cada buscador
    /// implementará los métodos como más le resulte conveniente.
    /// </summary>
    public class AddressFinder : IAddressFinder
    {
        private LocationApiClient client;

        /// <summary>
        /// Inicializa una nueva instancia de AddressFinder.
        /// </summary>
        /// <param name="client">El cliente de la API de localización.</param>
        public AddressFinder(LocationApiClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Determina si existe una dirección.
        /// </summary>
        /// <param name="address">La dirección a buscar.</param>
        /// <returns>Una instancia de AddressResult con el resultado de la búsqueda, que incluye si la dirección se
        /// encontró o no, y si se encontró, la latitud y la longitud de la dirección.</returns>
        public IAddressResult GetLocation(string address)
        {
            Location location = AsyncContext.Run(() => this.client.GetLocationAsync(address));
            AddressResult result = new AddressResult(location);

            return result;
        }
    }

    /// <summary>
    /// Una implementación concreta del resutlado de buscar una dirección. Además de las propiedades definidas en
    /// IAddressResult esta clase agrega una propiedad Location para acceder a las coordenadas de la dirección buscada.
    /// Cumple con DIP ya que depende de una abstracción y las clases que la usen dependerán de la abstracción y no de ella.
    /// Polirfismo se aplica al tener que cada clase concreta implementa de la forma que más le sirva cada método definido en 
    /// el tipo IAddressResult. 
    /// </summary>
    public class AddressResult : IAddressResult
    {   
          /// <summary>
        /// Propiedad que indica si se encontró algún lugar o si no se hayo nada
        /// </summary>
        /// <value></value>
        public bool Found
        {
            get
            {
                // Para poder controlar el caso de que no encuentre realmente las cosas
                bool successful = this.Location.Found && this.Location.AddresLine != null;
                return successful;
            }
        }

        /// <summary>
        /// Propiedad correspondiente al dato latitud que devuelve la búsqueda
        /// </summary>
        /// <value></value>
        public double Latitude
        {
            get
            {
                return this.Location.Latitude;
            }
        }

        /// <summary>
        /// Propiedad correspondiente al dato longitud que devuelve la búsqueda
        /// </summary>
        /// <value></value>
        public double Longitude
        {
            get
            {
                return this.Location.Longitude;
            }
        }
        
        /// <summary>
        /// Guarda una instancia de la clase Location con los datos proporicionados por la API
        /// </summary>
        /// <value></value>
        public Location Location { get; }
        /// <summary>
        /// Se encarga de devolver la dirección  completa.
        /// </summary>
        /// <value></value>
        public string FormatAddress
        {
            get
            {
               return  this.Location.FormattedAddress;
            } 
        }

        /// <summary>
        /// Constructor de la clase 
        /// </summary>
        /// <param name="location"></param>
        public AddressResult(Location location)
        {
            this.Location = location;
        }
      
    }
}