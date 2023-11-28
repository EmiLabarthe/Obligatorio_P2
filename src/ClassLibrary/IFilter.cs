using System.Collections.Generic;
namespace ClassLibrary;

// Clase responsabilidad de Agustín Toya.

/// <summary>
/// Intefaz que usamos para abstraer el concepto de un Filtro. Además lo creamos para que una clase concreta, que es 
/// SearchFilteredWorkOfferCommand, no dependa de un filtro en particular sino de cualquier filtro que quiera usarse. 
/// Por tal motivo cumple con DIP.  A su vez, se cumple con LSP puesto que se en el programa se puede sustituir un ordenar
///  por cualquier otro  que también implemente el tipo IFilter. Con esto también podemos decir que se rige bajo Polimorfismo,
///  ya que en cada clase concreta (subtipo) se implementa un comportamiento especifico de ella, estando la firma
/// definida en la interfaz (supertipo).  
/// 
/// La forma de crearse fue siguiendo el principio de segragación de interfaces (ISP), puesto que en un inicio 
/// teníamos una interfaz que especificaba las operaciones de buscar las ofertas y ordenarlas por ubicación,
/// por puntuación así como de filtrarlas por la categoría. 
/// Por ende, la clase que hiciera uso de esta abstracción iba a termiar dependiendo e implementando 
/// operaciones que no le correspondían usar. Por tal motivo separamos las responsabilidades en filtros y ordenadores, 
/// cada uno con una correspondiente interfaz, IFilter e ISort respectivamente. Siendo esta la resultante para la operación de filtrar.
/// 
/// 
/// </summary>
public interface IFilter
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
    /// Operación polimorfica que va a variar dependiendo de la clase concreta que lo implemente. De todas formas 
    /// se encarga de filtrar las ofertas de trabajo en base al criterio marcado para su existencia y devuelve un texto
    /// con la información filtrada. 
    /// </summary>
    /// <param name="criterion">Valor que se le pasará para poder hacer el filtro, ya sea un nombre, un precio u otra
    /// especie de dato. </param>
    /// <returns> Retornara una lista con los resultados obtenidos de los filtros</returns>
    public string Fiilter(string criterion);
}