using Exceptions;
namespace ClassLibrary;

// Clase responsabilidad de Agustín Toya.

/// <summary>
/// Clase encargada por Expert de conocer los datos de la ubicación del usuario.
/// Por otro lado es un componente de la clase User, ya que no hay motivos para que esta información exista sin 
/// estar ligada a un usuario, al menos en este contexto.
/// </summary>
public class LocationUser
{
    /// <summary>
    /// Dirección con un formato, como Av. 8 de Octubre 2121
    /// </summary>
    /// <value></value>
    public string FormatAddress { get; set; }

    /// <summary>
    /// Coordenada de la latitud de la ubicación del usuario
    /// </summary>
    /// <value></value>
    public double Latitude { get; set; }

    /// <summary>
    /// Coordenada de la longitud de la ubicación del usuario
    /// </summary>
    /// <value></value>
    public double Longitude { get; set; }

    /// <summary>
    /// Crea la locación o ubicación para poder saber donde esta ese usuario en un mapa,
    /// para así poder tomar decisiones en base a cuan lejos esta el trabajador del empleador. 
    /// </summary>
    /// <param name="formatAddress"></param>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    public LocationUser(string formatAddress, double latitude, double longitude)
    {
        // Precondiciones
        Check.Precondition(!string.IsNullOrWhiteSpace(formatAddress), "Su dirección no puede ser nula, estar vacia o ser espacios en blanco");
        Check.Precondition(latitude >= -90 && latitude <= 90, "La latitud no puede ser menor a -90 ni ser mayor a 90");
        Check.Precondition(longitude >= -180 && longitude <= 180, "La longitud no puede ser menor a -180 ni mayor a 180");
        this.FormatAddress = formatAddress;
        this.Latitude = latitude;
        this.Longitude = longitude;
        Check.Postcondition(FormatAddress == formatAddress, "Ocurrio un error en el sistema y no se pudo cargar la dirección");
        Check.Postcondition(Latitude == latitude, "Ocurrio un error en el sistema y no se pudo cargar la latitud de su ubiación");
        Check.Postcondition(Longitude == longitude, "Ocurrio un error en el sistema y no se pudo cargar la longitud de su ubicación");

        // Cosas que no pueden cambiar
        Check.Invariant(latitude >= -90 && latitude <= 90, "La latitud no puede ser menor a -90 ni ser mayor a 90");
        Check.Invariant(longitude >= -180 && longitude <= 180, "La longitud no puede ser menor a -180 ni mayor a 180");
    }
}
