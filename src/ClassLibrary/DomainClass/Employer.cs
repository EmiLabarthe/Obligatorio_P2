namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe.

/// <summary>
/// Clase que modela al rol de Empleador en el bot.
/// La clase employer cumple con Expert al igual que la clase User, ya que es la encargada de conocer
/// los atributos de sus instancias únicamente. En caso de necesitar saber la ubicación, necesita llamar
/// a la clase Location, ya que es la experta en eso.
/// Tenemos que el constructor de la clase es el que hace el registro del usuario.
/// Como los usuarios van a ser diferentes del administrador, por que van a tener datos personales
/// que sirven para las funcinalidades del programa, implementamos la clase padre y en la clase hija 
/// le enviamos los datos necesarios para construir el objeto. Utilizamos herencia en función de lograr 
/// una reutilización del código.
/// </summary>
public class Employer : User
{
    /// <summary>
    /// Constructor de la clase Employer. Le pasa los datos al constructor de la clase padre.
    /// </summary>
    /// <param name="userID">Identificador de telegram</param>
    /// <param name="name">Nombre del empleador</param>
    /// <param name="lastName">Apellido del empleador</param>
    /// <param name="phone">Número de celular del empleador</param>
    /// <param name="address">Dirección del empleador</param>
    /// <param name="latitude">Coordenada correspondiente a la latitud de la dirección del empleador</param>
    /// <param name="longitude">Coordenada correspondiente a la longitud de la dirección del empleador</param>
    public Employer(long userID, string name, string lastName, string phone, string address, double latitude, double longitude)
        : base(userID, name, lastName, phone, address, latitude, longitude)
    {
    }
}