using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System;
using System.Linq;

// Clase responsabilidad de Agustín Toya.

namespace ClassLibrary;
/// <summary>
/// Ordenador concreto que ordena en base a la distancia entre el trabajador que ejecuta el comando y cada dueño de las ofertas. Quedando primeras
/// las ofertas con workers que se ubiquen más cerca del empleador.
/// 
/// Cumple con el principio de inversión de dependencias (DIP), ya que esta clase depende de una interfaz
/// (ISort), de la cual depende el comando SearchSortedWorkOfferCommand que es una clase concreta. Al cumplir con DIP
/// también se cumple con LSP puesto que se puede sustituir por cualquier otra clase que también implemente el tipo ISort. 
/// Con esto también podemos decir que se rige bajo Polimorfismo, ya que se puede cambiar el comportamiento de cada método en el subtipo, 
/// estando definida la firma del método en el supertipo. 
/// </summary>
public class SortByDistance : ISort
{
    /// <summary>
    /// Es el nombre que tendrá el ordenardor y que lo diferenciará de otros ordenadores
    /// </summary>
    /// <value>El nombre que se le especifique</value>
    public string Name { get; }



    // Un calculador de distancias entre dos direcciones. Permite que la forma de calcular distancias se determine en
    // tiempo de ejecución: en el código final se asigna un objeto que use una API para buscar distancias; y en los
    // casos de prueba se asigne un objeto que retorne un resultado que puede ser configurado desde el caso de prueba.
    private IDistanceCalculator calculator;

    /// <summary>
    /// El resultado del cálculo de la distancia entre las direcciones ingresadas.
    /// </summary>
    public IDistanceResult Result { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="calculator">Un calculador de distancias.</param>
    public SortByDistance(IDistanceCalculator calculator)
    {
        this.calculator = calculator;
        this.Name = "location";
    }

    /// <summary>
    /// Propiedad que se usa para saber si ocurrió un fallo en el ordanamiento, ya sea que 
    /// alguna dirección no existía, o se haya perdido la conexión y los datos no hubiesen llegado a 
    /// destino de forma correcta. 
    /// </summary>
    /// <value></value>
    public bool Faild { get; private set; }

    /// <summary>
    /// Propiedad que aloja el mensaje de fallo, explicando que fallo
    /// </summary>
    /// <value></value>
    public string MessageFaild { get; private set; }


    /// <summary>
    /// Operación polimorfica especificada en el tipo ISort. Se encarga de reacomodar la lista  
    /// de ofertas de trabajo, ordenandolas bajo el criterio de la ubicación que tiene su dueño. 
    /// Las ofertas que quedan primero son las que tienen un dueño más cercano al employer que ejecuta el comando.
    /// </summary>
    /// <param name="userID"> User</param>
    /// <returns> Retornará un string con todos los datos formateados de lo que ordeno</returns>
    public string Order(long userID)
    {
        // Primero obtengo todas las ofertas, para tomar los workers que tienen ofertas hechas
        //Antes que nada necesitamos la lista de ofertas para así poder hacer una ordenación
        ReadOnlyCollection<WorkOffer> workOffers = Database.Instance.GetAllWorkOffers();
        // Ahora hago una lista de los workers con los workOffers relacionados
        IDictionary<long, List<WorkOffer>> workOffersForWorker = new Dictionary<long, List<WorkOffer>>();
        // Como a la vez que vamos viendo en que orden va el workerID y las ofertas que tiene, 
        // vamos guardando dicha info en un StringBuilder que luego se transformará a string.
        StringBuilder workOffersSortByDistance = new StringBuilder();
        // Para pasar los datos mejor usamos un Array List
        List<ArrayList> data = new List<ArrayList>();

        // Puede ser que al momento de usar este sort no haya workOffers, entonces, 
        // para devolverle un mensaje adecuado al employer, hacemos lo siguiente, si no hay cortamos la ejecución
        // para que no siga y devuelve un mensaje apropiado
        if(workOffers.Count == 0)
        {
            return "No hay ofertas en el sistema como para poder ser ordeandas, debe esperar a que un trabajador postule algún trabajo";
        }
        // Vamos recorriendo y agrupamos al worker con sus ofertas de trabajo
        foreach (WorkOffer offer in workOffers)
        {
            if(!workOffersForWorker.ContainsKey(offer.OwnerWorkerID))
            {
                workOffersForWorker[offer.OwnerWorkerID] = new List<WorkOffer>();
            }
            workOffersForWorker[offer.OwnerWorkerID].Add(offer);
        }
        // El método de calcular toma la dirección escrita en palabras
        // verifica que existan, y luego hace el calculo devolviendo un objeto de tipo IDistanceResult
        // Recupero el objeto asociado a este userID si existe correctamente
        if (!Database.Instance.ExistEmployer(userID))
        {
            throw new FieldAccessException("Ustedes no se como llego aquí, pero no es un employer");
        }
        Employer employerOwnerUserID = Database.Instance.SearchEmployer(userID);
        string formatAddressEmployer = employerOwnerUserID.Location.FormatAddress;

        if (this.calculator == null)
        {
            throw new FieldAccessException("Fallo el sistema por que no existe el calculador");
        }

        // Almacenamos los empleados con su userID y la distancia a la que estan del empleador en parcticular
        IDictionary<long, double> distancesForWorker = new Dictionary<long, double>();
        ReadOnlyCollection<Worker> workers = Database.Instance.GetWorkers();
        foreach (Worker worker in workers)
        {
            // Precondiciones
            // Si esa id no esta en la lista de trabajos, significa que no tiene ofertas a su nombre
            // por ende debe excluirse de esa lista
            if (workOffersForWorker.ContainsKey(worker.UserID))
            {
                string formatAddressWorker = worker.Location.FormatAddress;
                IDistanceResult result = this.calculator.CalculateDistance(formatAddressEmployer, formatAddressWorker);
                if (result.FromExists && result.ToExists && this.calculator != null)
                {

                    distancesForWorker.Add(worker.UserID, result.Distance);
                }
                else
                {
                    // Preguntar que queda mejor, si una excepción o un mensaje de alerta
                    if (!result.FromExists)
                    {
                        this.MessageFaild = "Su dirección o la del worker no existe, lo cual no debería de ser así";
                        this.Faild = true;
                    }
                    else if (!result.ToExists)
                    {
                        this.MessageFaild = "La dirección del worker no existe, lo cual no debería de ser así";
                        this.Faild = true;
                    }
                    else
                    {
                        this.MessageFaild = "Tuvo un fallo interno";
                        this.Faild = true;
                    }
                }

            }
        }
        List<long> usersSortMoreNearForEmployer = new List<long>();
        // Obtenemos la lista de distancias calculadas
        List<double> values = distancesForWorker.Values.ToList();
        // Ordenamos las distancias de menor a mayor
        values.Sort();
        // Ahora tengo que relacionar esas distancias ordenadas con esos worker
        // para hacer eso me conviene tener una lista con los userID e ir revisando 
        // si la distancia esta con ese id como clave-valor. Si es lo eliminamos del dicionario
        // va a ser algo recursivo
        while (distancesForWorker.Count > 0 && values.Count > 0)
        {
            //Obtenemos lista de claves = userIDs
            List<long> userIDs = distancesForWorker.Keys.ToList();
            //Tomamos la primer distancia luego de ser ordenadas
            double lowerDistance = values[0];
            // Para parar la busqueda una vez que se encuentra
            bool stop = false;
            // Para recorrer la lista de las userIDs
            int index = 0;
            // Ahora buscamos al userId relacionado a esa distancia
            while (!stop)
            {

                if (distancesForWorker[userIDs[index]] == lowerDistance)
                {
                    /* // Lo agregamos a la lista
                    usersSortMoreNearForEmployer.Append(userIDs[index]); */
                    long workerOwnerID = userIDs[index];
                    Worker owner = Database.Instance.SearchWorker(userIDs[index]);
                    long workerUserID = owner.UserID;
                    string nameOwner = owner.Name;
                    string lastNameOwner = owner.LastName;
                    //Ahora hay que armar el texto, para lo que necesito el userID, el nombre, el apellido
                    // Todos los datos de la oferta, y la distancia que hay del worker del employer (el que ejecuto el comando)
                    foreach (WorkOffer offer in workOffersForWorker[workerOwnerID])
                    {
                        workOffersSortByDistance.Append($"\n\nIdentificador: {offer.Identify}\nDueño: {nameOwner} {lastNameOwner}, ID del dueño: {workerUserID}. \nDescripción: {offer.Description}\nPrecio: {offer.Price} \n\nEl oferente esta a {lowerDistance} kilométros de usted");
                    }
                    // Ahora quitamos el id y la distancia, para evitar repeticiones
                    distancesForWorker.Remove(userIDs[index]);
                    values.Remove(lowerDistance);
                    // Para salir de esta búsqueda
                    stop = true;
                }
                else
                {
                    // Como no era el caso, incrementamos index
                    index += 1;
                    // Si ese valor llega a ser del tamaño de la listas, cancelamos la búsqueda
                    if (distancesForWorker.Count == index)
                    {
                        stop = true;
                    }
                }
            }

        }
        return workOffersSortByDistance.ToString();
    }

}