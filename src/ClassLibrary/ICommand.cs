namespace ClassLibrary;

// Clase responsabilidad de Agustín Toya.

/// <summary>
/// Intefaz craeda bajo el principio DIP para que el interpretador de entradas no 
/// dependa de una clase concreta y sea obligado a tener una dependencia indeseable. 
/// Al seguir DIP en el marco de la ejecución del programa, yendo más precisamente al interpretador 
/// de entradas (InputInterfacer), este puede hacer uso de cualquier comando que tenga como supertipo el tipo 
/// ICommand, por lo cual se puede sustituir sin probocar cambios colaterales, cumpliendo de esta forma con LSP. 
/// A su vez, al ser una abstracción implementada por varias clases abstractas, sus operaciones terminan siendo 
/// polimorficas, puesto que cada una las implementa de la forma que más les convenga. 
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Es el nombre que tendrá el comando y que lo diferenciará de otros comandos
    /// </summary>
    /// <value>El nombre que se le especifique</value>
    public string Name { get; }

    /// <summary>
    /// Para que de afuera se pueda saber si se esta ejecutando o si ya termino su ejecución, para dejar libre el camino a otro
    /// </summary>
    /// <value>false si esta terminada la ejecución, true si todavía sigue en camino</value>
    public bool InProccess { get; }

    /// <summary>
    /// Operación  que se encarga de ejecutar la acción que le da su razón de ser al comando
    /// </summary>
    /// <param name="userID"> Identificador del usuario que ejecuta el comando</param>
    /// <param name="data"> Conjunto de datos ingresados o recibidos desde la interfaz, es lo que escribió el usuario</param>
    /// <returns> Devuleve o el resultado de una busqueda o filtro, o si se concreto correctamente la acción</returns>
    public string Execute(long userID, string data);

    /// <summary>
    /// Se encarga de verificar que el user de Telegram pueda usar este comando o no,
    /// en base a que es o que rol tiene en nuestro sistema.
    /// </summary>
    /// <param name="userID">Identificador de Telegram del usuario que ejecuto el comando</param>
    /// <returns>True si puede, false si no esta habilitado</returns>
    public bool ProfileCanExecute (long userID);

    /// <summary>
    /// Se restaura la información del user que esta usando el comando 
    /// </summary>
    public void RestoreData(long userID);

    /// <summary>
    /// Cancela la ejecución del comando y retorna un 
    /// mensaje diciendo el nombre del comando y que se cancelo. 
    /// </summary>
    /// <returns>Mensaje con el nombre del comando a modo de confirmación de que se 
    /// cancelo el comando</returns>
    public string Cancel(long userID);

}