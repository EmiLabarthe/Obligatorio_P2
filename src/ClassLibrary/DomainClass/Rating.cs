using System;
using Exceptions;

namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe y Agustín Toya.

/// <summary>
/// Esta clase se diseño pensando en Composición y Delegación, ya que debiamos de tener varias calificaciones 
/// para poder hacer el promedio, pero si todo eso quedaba cargado en la clase User, sumando el hecho de contar con notificaciones
///  y un objeto LocationUser, le daríamos más de una razón para cambiar (si cambiese la forma de notificar y la de calificar por ejemplo). 
/// Así que se cumple con SRP y también se cumple con Expert, ya que para saber la información de una calificación, 
/// deben entrar a la clase Rating, que es la experta en el área.
/// </summary>
public class Rating
{   
    /// <summary>
    /// Valor númerico de una calificación
    /// </summary>
    /// <value></value>
    public int Rate { get; private set; }

    /// <summary>
    /// Marca el tiempo momento en el que un worker puede puntuar al employer y viceversa, una vez que 
    /// se ven relacionados por un contacto afirmativo.
    /// </summary>
    /// <value></value>
    public DateTime StartCountAMonth { get; private set; }

    /// <summary>
    /// Identificador de Telegram del usuario que califica o que crea esta calificación
    /// </summary>
    /// <value></value>
    public long CalificatorID { get; private set; }
    /// <summary>
    /// Como las calificaciones se hacen sobre una oferta de trabajo, guardamos el identify de la oferta para 
    /// poder diferenciar esta de otra calificación, en la que estén el mismo employer contratante y al worker dueño
    /// </summary>
    /// <value></value>
    public int WorkOfferID { get; private set; }
    
    /// <summary>
    /// Constructor de la calificación, crea e objeto con los datos necesarios para luego hacer el proceso de calificación
    /// </summary>
    /// <param name="calificatorID">Id del user que califica</param>
    /// <param name="durationWorkOffer">Duración de la oferta para hacer el cálculo de cuando arranca a contar el mes</param>
    /// <param name="workOfferID">Identificador de la oferta para poder saber sobre que trabajo se esta calificando al user en cuestión</param>
    public Rating(long calificatorID, int durationWorkOffer, int workOfferID)
    {
        // Precondiciones
        Check.Precondition(calificatorID > 0, "El userID del calificador no puede ser menor a 1 nunca");
        // Por si no esta mal usar a la base de datos para revisar si existen esos identificadores
        bool userExist = Database.Instance.ExistEmployer(calificatorID) || Database.Instance.ExistWorker(calificatorID);
        Check.Precondition(userExist, "El userID del calificador no corresponde a ningún worker ni a ningún employer");
        Check.Precondition(durationWorkOffer > 0, "La duración del trabajo no puede ser nunca menor a 1");

        // Por si no esta mal usar a la base de datos para revisar si existen esos identificadores
        Check.Precondition(workOfferID > 0, "El identificador de la oferta de trabajo no puede ser menor a 1 nunca");
        Check.Precondition(Database.Instance.ExistWorkOffer(workOfferID), "No existe una oferta de trabajo con este identificador");
        // Fin de revisión con la base de datos

        
        this.CalificatorID = calificatorID;
        this.WorkOfferID = workOfferID;
        // Devuleve mm/dd/yyyy
        StartCountAMonth = DateTime.Today;
        // Ahora al día de la fecha le agregamos la duracción de la oferta de trabajo 
        // para así tener idea de cuando empezaría a correr el mes que tienen de plazo
        // tanto el empleador como el empleado para calificarse mutamente
        StartCountAMonth = StartCountAMonth.AddDays(durationWorkOffer);
        this.Rate = 0;

        // Revisamos las postcondiciones
        Check.Postcondition(this.CalificatorID == calificatorID, "Ocurrio un error en el sistema y no se pudo cargar el id del calificador");
        Check.Postcondition(this.WorkOfferID == workOfferID, "Ocurrio un error en el sistema y no se pudo cargar el identificador de la oferta");
        Check.Postcondition(this.StartCountAMonth == DateTime.Today.AddDays(durationWorkOffer), "Ocurrio un error en el sistema y no se pudo establcer la fecha de fin del período de calificación");
    }

    /// <summary>
    /// Este método se encarga de realizar el proceso de calificación, si es un período de tiempo válido permite que se cambie ese valor 
    /// de puntuación que por defecto se carga en 0. 
    /// </summary>
    /// <param name="newRate">Puntuación que se va a otorgar</param>
    /// <param name="simulateDate">Bandera que sirve para simular que ya estamos en un momento valido para calificar; solo se usa en los test</param>
    public void UpdateRate(int newRate, bool simulateDate)
    {

        // Como una resta entre dos numeros da un TimeSpan, y este tiene de días para abajo (hh:mm:ss)
        // y como hay meses de 30, otros de 31 y 28 y cuando es bisiesto 29. POr lo cual, para ser más preciosos
        // aplicamos una forma un tanto diferente de saber si paso un mes o no.
        // Primero a la fecha de referencia, de cuando inicio el contrato, le sumamos un mes, para controlar que haya pasado un mes
        DateTime dateReference = StartCountAMonth.AddMonths(1);
        // Revisamos si la comparación entre la fecha de referencia y la actual son iguales, la actual es mayor  o menor. 
        int dateResult = DateTime.Compare(DateTime.Today, dateReference);
        //Ahora necesitamos saber si la fecha actual es mayor a la que termino el contrato
        int dateAfeterContractEnd = DateTime.Compare( DateTime.Today, StartCountAMonth);
        /* 
        - Menor que 0 si la primera fecha es menor que la segunda.
        - 0 si ambas fechas son iguales
        - Mayor que 0 si la primera fecha es mayor que la segunda.
        */
        // Para los test no podemos esperar a que se pueda calificar, por lo cual ponemos una propiedad que nos permite 
        // simular que la fecha es valida como para poder calificar al user
        if (simulateDate)
        {
            dateAfeterContractEnd = 1;
        }
        if (dateResult <= 0 && dateAfeterContractEnd > 0)
        {
            if (newRate > 0 && newRate <= 10)
            {
                this.Rate = newRate;
            }
        }
    }
}
