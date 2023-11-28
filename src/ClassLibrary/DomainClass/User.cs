
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Text.RegularExpressions;
using Exceptions;

namespace ClassLibrary;

// Clase responsabilidad de Emiliano Labarthe y Agustín Toya.

/// <summary>
/// Esta clase cumple con el principio SRP, ya que su única responsabilidad es conocer los atributos de un usuario, teniendo las responsabilidades de conocer 
/// la puntuación, la ubicación, y lo necesario de una notificación a otras clases, para así no violar dicho principio.
/// También cumple con Expert, ya que se ocupa de saber las características que le toca y operar con ellas únicamente. 
/// Por ejemplo, si necesitara saber la ubicación de un usuario, tiene que llamar a la clase Location, ya que ella no es la experta en eso.
/// También cumple con el patrón Creator, ya que al recibir todos los datos de la ubicación, los datos de una notificación y una calificación, es capaz 
/// de crear objetos de este tipo. Además cumple con la condición de tener un objeto de ese tipo (LocationUser), así como con la condición de contener a
/// instancias de las clases que terminan siendo componentes. 
/// </summary>
public class User : Profile
{
    /// <summary>
    /// Nombre del usuario
    /// </summary>
    /// <value>String que se le especifique como nombre</value>
    public string Name { get; private set; }
    /// <summary>
    /// Apellido del usuario
    /// </summary>
    /// <value>String que se le especifique como apellido</value>
    public string LastName { get; private set; }
    /// <summary>
    /// Número de celular del usuario
    /// </summary>
    /// <value>String que se le especifique como número de celular</value>
    public string Phone { get; private set; }
    /// <summary>
    /// Objeto que guarda los datos de la ubicación del usuario
    /// </summary>
    /// <value>Instancia de la clase LocationUser</value>
    public LocationUser Location { get; private set; }
    /// <summary>
    /// Lista de calificaciones del usuario
    /// </summary>
    public List<Rating> Ratings { get;  }
    /// <summary>
    /// Lista de notificaciones del usuario
    /// </summary>
    /// <value></value>
    public List<Notification> Notifications { get; }

    /// <summary>
    /// Al igual que con las workOffer para tener un id para identificar una notificación de otra.
    /// </summary>
    /// <value></value>
    public int UltimateIDNotification { get; private set; }

    /// <summary>
    /// Tenemos que el constructor de la clase es el que hacae el registro del usuario.
    /// Como los usuarios van a ser diferentes del administrador, por que van a tener datos personales
    /// que sirven para las funcinalidades del programa, implementamos la clase padre y en la clase hija 
    /// le enviamos los datos necesarios para construir el objeto. 
    /// </summary>
    /// <param name="userID">Identificador de telegram</param>
    /// <param name="name">Nombre del usuario</param>
    /// <param name="lastName">Apellido del usuario</param>
    /// <param name="phone">Número de celular del usuario</param>
    /// <param name="formatAddress">Dirección del usuario</param>
    /// <param name="latitude">Coordenada correspondiente a la latitud de la dirección del usuario</param>
    /// <param name="longitude">Coordenada correspondiente a la longitud de la dirección del usuario</param>
    protected User(long userID, string name, string lastName, string phone, string formatAddress, double latitude, double longitude)
        : base(userID)
    {
        Check.Precondition(!String.IsNullOrWhiteSpace(name), "El nombre no puede ser nulo, estar vació o ser espacios en blanco");
        Check.Precondition(!String.IsNullOrWhiteSpace(lastName), "El apellido no puede ser nulo, estar vació o ser espacios en blanco");
        Check.Precondition(!String.IsNullOrWhiteSpace(phone), "El celular no puede ser nulo, estar vació o ser espacios en blanco");
        Regex regex = new Regex("^09[1-9]{1}[0-9]{6}$");
        Match match = regex.Match(phone);
        Check.Precondition(match.Success, "El celular que ingreso no tiene un formato valido. El formato valido es: 09[1-9]xxxxxx. Cambiando las x por los números correspondientes");
        Check.Precondition(!String.IsNullOrWhiteSpace(formatAddress), "Su dirección no puede ser nula, estar vacia o ser espacios en blanco");
        Check.Precondition(latitude >= -90 && latitude <= 90, "La latitud no puede ser menor a -90 ni ser mayor a 90");
        Check.Precondition(longitude >= -180 && longitude <= 180, "La longitud no puede ser menor a -180 ni mayor a 180");

        //Asignamos las propiedades que deben ser diferentes de vacías o nulas
        this.Name = name;
        this.LastName = lastName;
        this.Phone = phone;
        // Patrón creator
        this.Location = new LocationUser(formatAddress, latitude, longitude);
        Notifications = new List<Notification>();
        Ratings = new List<Rating>();
        UltimateIDNotification = 0;

        Check.Postcondition(this.Name == name, "Ocurrio un error en el sistema y no se pudo cargar el nombre");
        Check.Postcondition(this.LastName == lastName, "Ocurrio un error en el sistema y no se pudo cargar el apellido");
        Check.Postcondition(this.Phone == phone, "Ocurrio un error en el sistema y no se pudo cargar el número de celular");
        Check.Postcondition(this.Location.FormatAddress == formatAddress, "Ocurrio un error en el sistema y no se pudo cargar la dirección");
        Check.Postcondition(this.Location.Latitude == latitude, "Ocurrio un error en el sistema y no se pudo cargar la latitud de su ubiación");
        Check.Postcondition(this.Location.Longitude == longitude, "Ocurrio un error en el sistema y no se pudo cargar la longitud de su ubicación");
        
        Check.Invariant(this.Location != null, "Ocurrio un error en el sistema y no se pudo crear el objeto ubicación");
        Check.Invariant(this.Notifications != null, "Ocurrio un error en el sistema y no se pudo crear la lista de notificación");
        Check.Invariant(this.Notifications != null, "Ocurrio un error en el sistema y no se pudo crear la lista de calificaciones");
    }

    /// <summary>
    /// Este método se encarga de crear una notificación (siguiendo con el patrón Creator), y de añadirla a la lista de notificaciones.
    /// Decimos que cumple porque al tener una lista de instancias de esta clase, le corresponde recibir los datos necesarios para crear 
    /// instancias de dicha clase. 
    /// </summary>
    /// <param name="message">Mensaje que recibe el user</param>
    /// <param name="senderID">Identificador del que le genero la notificación</param>
    /// <param name="reasons">Asunto de la notificación</param>
    public void AddNotification(string message, long senderID, Notification.Reasons reasons)
    {
        Notification newNotfication = new Notification(message, senderID, UltimateIDNotification + 1, reasons);
        UltimateIDNotification += 1;
        this.Notifications.Add(newNotfication);
    }

    /// <summary>
    /// Se encarga de dar por cerrada la notificación en cuestión, así no le aparece más al user
    /// </summary>
    /// <param name="notificationID"></param>
    public void CloseNotifcation(int notificationID)
    {
        bool stop = false;
        int index = 0;
        while(!stop && index != Notifications.Count)
        {
            Notification not = Notifications[index];
            if(not.NotificationID == notificationID)
            {
                not.CloseNotifcation();
                stop = true;
            }
            index +=1;
        }
    }

    /// <summary>
    /// Se encarga de recibir la calificación que le otorga otro user.
    /// </summary>
    /// <param name="rate">puntaje</param>
    /// <param name="calificatorID">puntuado</param>
    /// <param name="workOfferID">la workoffer por la que se puntúa</param>
    /// <param name="simulateDate">Se usara para los test, para poder simular que estas en un día habil para calificar</param>
    /// <returns>Devuleve un mensaje de confirmación</returns>
    public string ReciveCalification(int rate, long calificatorID, int workOfferID, bool simulateDate)
    {
        foreach (Rating rating in Ratings)
        {
            if (rating.WorkOfferID == workOfferID && rating.CalificatorID == calificatorID)
            {
                rating.UpdateRate(rate,simulateDate);
                return "Se ha calificado correctamente";
            }
        }
        return "No se ha podido calificar";
    }

    /// <summary>
    /// Método que devuelve la lista de notificaciones del usuario pero con la restricción de
    /// que son de solo lectura, preservando la integridad de dichos datos.
    /// </summary>
    /// <returns> Convierte el stringbuilder en string </returns>
    public ReadOnlyCollection<Notification> GetNotifications()
    {
        return Notifications.AsReadOnly();
    }

    /// <summary>
    /// En este método, recibe como parámetro lo necesario para crear un nuevo objeto de la clase Rating.
    /// Por patrón Creator, debido a que es el encargado a almacenarlo, este objeto crea la Rating.
    /// </summary>
    /// <param name="calificatorID">Quien calificó</param>
    /// <param name="durationWorkOffer">Duración de la oferta</param>
    /// <param name="workOfferID">Identificador de sobre que oferta se esta haciendo la calificación</param>
    public void AddRating(long calificatorID, int durationWorkOffer, int workOfferID)
    {
        Rating rating = new Rating(calificatorID, durationWorkOffer, workOfferID);
        this.Ratings.Add(rating);
    }

    /// <summary>
    /// Devuelve el resultado de la cuenta de promediar las calificaciones del Usuario
    /// </summary>
    /// <returns></returns>
    public double GetRating()
    {
        int sumRatings = 0;
        double resultRating = 0;
        int totalRatesValid = 0;
        foreach (Rating r in Ratings)
        {
            if (r.Rate != 0)
            {
                totalRatesValid += 1;
                sumRatings = sumRatings + r.Rate;
            }
        }
        if(totalRatesValid != 0)
        { 
            resultRating = (sumRatings / totalRatesValid);
        }
        return resultRating;
    }
}
