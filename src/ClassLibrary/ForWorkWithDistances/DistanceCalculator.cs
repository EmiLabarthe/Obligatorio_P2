using Ucu.Poo.Locations.Client;
using Nito.AsyncEx;

// Clase responsabilidad de Agustín Toya.

namespace ClassLibrary
{
    /// <summary>
    /// Un calculador de distancias concreto que utiliza una API de localización para calcular la distancia entre dos
    /// direcciones. Cumple con DIP ya que depende de una abstracción y no de una clase concreta, además cuando se haga uso, primero 
    /// se va definir el tipo de la variable como IDistanceCalculator, pudiendo luego usarse cualquiera que se requiera mientras corre el 
    /// programa. Esto último a su ve permite que se cumpla con LSP. 
    /// </summary>
    public class DistanceCalculator : IDistanceCalculator
    {
        private LocationApiClient client;

        /// <summary>
        /// Constructor de la clase 
        /// </summary>
        /// <param name="client">Instancia de la clase LocationApiCliente</param>

        public DistanceCalculator(LocationApiClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Se encarga de calcular la disntacia y devolver un objeto IDistanceResult, que tiene toda la información sobre 
        /// el cálculo
        /// </summary>
        /// <param name="fromAddress">Dirección de inicio o punto de partida</param>
        /// <param name="toAddress">Dirección de fin o punto de finalización de un camino</param>
        /// <returns></returns>
        public IDistanceResult CalculateDistance(string fromAddress, string toAddress)
        {
            // La API de localización ofrece un método para obtener la distancia entre dos direcciones; aquí se buscan
            // primero las coordenadas de las direcciones; esto permite determinar cuál de las dos direcciones no existe.
            //
            // Los métodos de la API de localización son async. Como el resultado de las dos primeras llamadas se utiliza
            // en la tercera, los métodos deben ser invocados en forma sincrónica. Por eso el uso de AsyncContext.
            Location fromLocation = AsyncContext. Run(() => client.GetLocationAsync(fromAddress));
            Location toLocation = AsyncContext. Run(() => client.GetLocationAsync(toAddress));
            Distance distance = AsyncContext. Run(() => client.GetDistanceAsync(fromLocation, toLocation));

            DistanceResult result = new DistanceResult(fromLocation, toLocation, distance.TravelDistance, distance.TravelDuration);

            return result;
        }
    }

    /// <summary>
    /// Una implementación concreta del resutlado de calcular distancias. Además de las propiedades definidas en
    /// IDistanceResult esta clase agrega propiedades para acceder a las coordenadas de las direcciones buscadas.
    /// </summary>
    public class DistanceResult : IDistanceResult
    {
        private Location from;
        private Location to;
        private double distance;
        private double time;

        
        /// <summary>
        /// Propiedad que almacena si existe la dirección de origen
        /// </summary>
        /// <value>True si existe, false si no existe</value>
        public bool FromExists
        {
            get
            {
                return this.from.Found;
            }
        }

        /// <summary>
        /// Propiedad que almacena si existe la dirección de destino
        /// </summary>
        /// <value>True si existe, false si no existe</value>
        public bool ToExists
        {
            get
            {
                return this.to.Found;
            }
        }

        /// <summary>
        /// Propiedad que almacena el resultado del cálculo de distancia
        /// </summary>
        /// <value>True si existe, false si no existe</value>
        public double Distance
        {
            get
            {
                return this.distance;
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de DistanceResult a partir de dos coordenadas, la distancia y el tiempo
        /// entre ellas.
        /// </summary>
        /// <param name="from">Las coordenadas de origen.</param>
        /// <param name="to">Las coordenadas de destino.</param>
        /// <param name="distance">La distancia entre las coordenadas.</param>
        /// <param name="time">El tiempo que se demora en llegar del origen al destino.</param>
        public DistanceResult(Location from, Location to, double distance, double time)
        {
            this.from = from;
            this.to = to;
            this.distance = distance;
            this.time = time;
        }
    }
}