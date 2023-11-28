namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe.

/// <summary>
/// Clase que modela al rol de Adminstrador en el bot.
/// Se aplica herencia como técnica de reutilización de código en este caso, ya que 
/// la propiedad de userID esta en la clase padre.
/// </summary>
public class Admin : Profile
{
    /// <summary>
    /// Se crea al Admin con el ID como dato único que posee
    /// </summary>
    /// <param name="userID"> ID de la plataforma que usa nuestro bot</param>
    /// <returns>Devulve una instancia de la clase Admin</returns>
    public Admin(long userID) : base(userID)
    {
        // Intencionalmente vacío
    }
}
