using System.Collections.Generic;
using System.Collections;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

// Clase responsabilidad de Agustín Toya.

namespace ClassLibrary;
/// <summary>
/// Ordenador concreto de las ofertas que ordena en base a la calificación del worker, quedando primero el de mejor calificación.
/// Cumple con el principio de inversión de dependencias (DIP), ya que esta clase depende de una interfaz
/// (ISort), de la cual depende el comando SearchSortedWorkOfferCommand que es una clase concreta. Al cumplir con DIP
/// también se cumple con LSP puesto que se puede sustituir por cualquier otra clase que también implemente el tipo ISort. 
/// Con esto también podemos decir que se rige bajo Polimorfismo, ya que se puede cambiar el comportamiento de cada método en el subtipo, 
/// estando definida la firma del método en el supertipo. 
/// </summary>
public class SortByRate : ISort
{

    /// <summary>
    /// Es el nombre que tendrá el ordenardor y que lo diferenciará de otros ordenadores
    /// </summary>
    /// <value>El nombre que se le especifique</value>
    public string Name { get; }


    /// <summary>
    /// Se encarga de cargar el nombre del ordenador
    /// </summary>
    public SortByRate()
    {
        this.Name = "rates";
    }


    /// <summary>
    /// Operación polimorfica especificada en el tipo ISort. Se encarga de reacomodar la lista de 
    /// ofertas de trabajo, ordenandolas bajo el criterio de la calificación que tiene su dueño. 
    /// Las que tengan un dueño con calificación alta irán primero, quedando para lo último las que tengan dueños
    /// con una calificación mala o no tan buena en comparación con el resto. 
    /// </summary>
    /// <param name="userID"> User</param>
    /// <returns>Retorna un string con los datos de las ofertas que ordeno</returns>
    public string Order(long userID)
    {

        //Antes que nada necesitamos la lista de ofertas para así poder hacer una ordenación
        ReadOnlyCollection<WorkOffer> workOffers = Database.Instance.GetAllWorkOffers();
        // Para almacenar la información de las ofertas ordenadas 
        StringBuilder workOffersSortedByRate = new StringBuilder();
        // Diccionario para guardar al trabajador y sus ofertas
        IDictionary<long, List<WorkOffer>> workOffersForWorker = new Dictionary<long, List<WorkOffer>>();
        // Para almancenar al empleado en base a la puntuación
        List<double> ratings = new List<double>();
        // Como en el caso del sort por distancia, vamos a tener un diccionario con id -> rating
        IDictionary<long, double> workersWithRating = new Dictionary<long, double>();

        // En caso de que se use el sort estando la lista de ofertas vacía, cortamos antes que siga,
        // retornando un mensaje adecuado.
        if(workOffers.Count == 0)
        {
            return "No hay ofertas en el sistema como para poder ser ordeandas, debe esperar a que un trabajador postule algún trabajo";
        }

        // La rellenamos
        foreach(WorkOffer offer in workOffers)
        {
            if(!workOffersForWorker.ContainsKey(offer.OwnerWorkerID))
            {
                workOffersForWorker[offer.OwnerWorkerID] = new List<WorkOffer>();
                Worker workerOwner = Database.Instance.SearchWorker(offer.OwnerWorkerID);
                workersWithRating[workerOwner.UserID] =  workerOwner.GetRating();
            }
            workOffersForWorker[offer.OwnerWorkerID].Add(offer);
            
        }
        
        //Ordeno la lista de menor a mayor con un Sort, y luego lo inverto con Reverse
        ratings = workersWithRating.Values.ToList();
        ratings.Sort();
        ratings.Reverse();
        // Ahora busco los workers que tengan esos rates para poder obtener su userID
        while(workersWithRating.Count > 0 && ratings.Count > 0)
        {
            // Lista con los userID que quedan en el diccionario
            List<long> userIDs = workersWithRating.Keys.ToList();
            // Para parar la busqueda una vez que se encuentra
            bool stop = false;
            // Para recorrer la lista de los ratings
            int index = 0;
            // Ahora buscamos al userId relacionado a esa distancia
            while (!stop)
            {
                double biggestRating = ratings[0];
                if (workersWithRating[userIDs[index]] == biggestRating)
                {
                    long workerOwnerID = userIDs[index];
                    Worker owner = Database.Instance.SearchWorker(workerOwnerID);
                    long workerUserID = owner.UserID;
                    string nameOwner = owner.Name;
                    string lastNameOwner = owner.LastName;
                    //Ahora hay que armar el texto, para lo que necesito el userID, el nombre, el apellido
                    // Todos los datos de la oferta, y la puntuación del worker
                    foreach (WorkOffer offer in workOffersForWorker[workerOwnerID])
                    {
                        workOffersSortedByRate.Append($"\n\nIdentificador: {offer.Identify}\nDueño: {nameOwner} {lastNameOwner}, ID del dueño: {workerUserID}. \nDescripción: {offer.Description}\nPrecio: {offer.Price} \n\nEl oferente tiene una calificación de {biggestRating} en un total de 10 puntos");
                    }
                    // Ahora quitamos el id y el objeto Worker, para evitar repeticiones
                    workersWithRating.Remove(userIDs[index]);
                    ratings.RemoveAt(0);
                    // Para salir de esta búsqueda
                    stop = true;
                }
                else
                {
                    // Como no era el caso, incrementamos index
                    index += 1;
                    // Si ese valor llega a ser del tamaño de la listas, cancelamos la búsqueda
                    if (workersWithRating.Count == index)
                    {
                        stop = true;
                    }
                }
            }
        }
        return workOffersSortedByRate.ToString();
    }
}