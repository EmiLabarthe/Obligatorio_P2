using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
namespace ClassLibrary;

// Comando responsabilidad de Agustín Toya.

/// <summary>
/// Filtro concreto que excluye las ofertas de trabajo que no tienen la categoría pasada en su método Filter
/// Filtro concreto de las ofertas en base a la categoría de las ofertas de trabajo, quedando solamente las que tengan al menos la categoría especificada.
/// Cumple con el principio de inversión de dependencias (DIP), ya que esta clase depende de una interfaz
/// (IFilter), de la cual depende el comando SearchFilteredWorkOfferCommand que es una clase concreta. Al cumplir con DIP
/// también se cumple con LSP puesto que se puede sustituir por cualquier otra clase que también implemente el tipo IFilter. 
/// Con esto también podemos decir que se rige bajo Polimorfismo, ya que se puede cambiar el comportamiento de cada método en el subtipo, 
/// estando definida la firma del método en el supertipo. 
/// </summary>
public class FilterByCategory : IFilter
{
    /// <summary>
    /// Es el nombre que tendrá el filtro y que lo diferenciará de otros filtros
    /// </summary>
    /// <value>El nombre que se le especifique</value>
    public string Name { get; }

    /// <summary>
    /// Mensaje se que se devuelve por filtro, para que el usuario sepa que información ingresar
    /// </summary>
    /// <value></value>
    public string MessageForFilter {get;}

    /// <summary>
    /// Se encarga de asignarle un valor a la propiedad del nombre con el fin
    /// de diferenciar entre un filtro y otro
    /// </summary>
    public FilterByCategory()
    {
        this.MessageForFilter = "Ahora ingrese una categoría para filtrar las ofertas de trabajo";
        this.Name = "category";
    }

    /// <summary>
    /// Operación polimorfica encargada de filtrar  las ofertas por su categoría (en este caso), excluyendo las que no 
    /// tengan en su lista a la categoría especificada. 
    /// /// </summary>
    /// <param name="nameCategory">Nombre de la categoría que se va a usar </param>
    /// <returns> Retornara un mensjae con los resultados obtenidos de los filtros</returns>
    public string Fiilter(string nameCategory)
    {
        // Para ponerlo en un mismo formato por si lo escribió con mayus y min
        nameCategory = Format.RemoveAcentMarkToUpper(nameCategory);
        // Obtenemos la lista de offers de trabajo
        ReadOnlyCollection<WorkOffer> offers = Database.Instance.GetAllWorkOffers();
        // Antes de pasar por un filtro, controlamos que esta lista no este vacía
        if(offers.Count == 0)
        {
            return "La lista de ofertas de trabajo esta vacía, no hay nada para filtrar.";
        }
        // Revisamos si existe o no la categoría en el sistema
        if(!Database.Instance.GetAllCategories().Contains(nameCategory))
        {
            return "No existe esa categoría en el sistema";
        }
        // Lista para contener las offers Filtered
        List<WorkOffer> offersFiltered = new List<WorkOffer>();
        // Hacemos el filtro
        foreach (WorkOffer offer in offers)
        {
            foreach (string category in offer.GetCateogories())
            {
                if (category == nameCategory)
                {
                    offersFiltered.Add(offer);
                }
            }
        }
        // Lista de trabajos filtrados por la categoría
        StringBuilder worksFilterByCategory = new StringBuilder();
        // Resultado del filtro 
        string result = "";
        // Ahora, en el caso de que no haya ninguna oferta de trabajo con esa categoría, hay que avisarle al user
        if(offersFiltered.Count != 0)
        {
            // Recorremos la lista del principio y vamos agregando los datos a la lista
            foreach (WorkOffer offer in offersFiltered)
            {
                    // Recuperamos el objeto worker dueño de la oferta
                    Worker owner = Database.Instance.SearchWorker(offer.OwnerWorkerID);   
                    long workerUserID = owner.UserID;
                    string nameOwner = owner.Name;
                    string lastNameOwner = owner.LastName;
                    worksFilterByCategory.Append($"\n\nIdentificador: _{offer.Identify}_\nDueño: {nameOwner} {lastNameOwner}, ID del dueño: {workerUserID}. \nDescripción: {offer.Description}\nPrecio: {offer.Price}");
                
            }
            result = worksFilterByCategory.ToString();
        }
        else
        {
            result = "No hay ofertas de trabajo que tengan esa categoría.";
        }

        return result;
    }
}