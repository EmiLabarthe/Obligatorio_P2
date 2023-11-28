using Exceptions;
namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe.

/// <summary>
/// Clase que se encarga de conocer un dato común entre todos los usuarios.
/// Esta clase cumple con el principio Expert pues es la experta en conocer el identificador de usuario proveído por Telegram
/// </summary>
public class Profile
{

    /// <summary>
    ///  Propiedad que representa al ID de los usuarios que llegan al bot
    /// </summary>
    /// <value> Será dado por el resto de componentes del bot</value>
    public long UserID { get; private set; }

    /// <summary>
    /// Hacemos que el constructor sea protected porque no queremos que 
    /// se creen instancias de este objeto. 
    /// </summary>
    /// <param name="userID"> ID de la plataforma que usa nuestro bot</param>
    protected Profile(long userID)
    {
        Check.Precondition(userID > 0, "El userID no puede ser nunca menor o igual a cero");
        this.UserID = userID;
        Check.Postcondition(this.UserID == userID, "Ocurrio un error en el sistema y no se pudo cargar la información");
        
    }
}
