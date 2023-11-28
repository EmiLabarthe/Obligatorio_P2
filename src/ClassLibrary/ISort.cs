using System.Collections.Generic;
using System.Collections;
namespace ClassLibrary;

// Clase responsabilidad de Agustín Toya.

/// <summary>
/// Interfaz que representa la abstracción de las diferentes formas de ordenar las ofertas de trabajo.
/// La forma de crearse fue siguiendo el principio de segragación de interfaces (ISP), puesto que en un inicio 
/// teníamos una interfaz que especificaba las operaciones de buscar las ofertas y ordenarlas por ubicación,
/// por puntuación así como de filtrarlas por la categoría. 
/// Por ende, la clase que hiciera uso de esta abstracción iba a termiar dependiendo e implementando 
/// operaciones que no le correspondían usar. Por tal motivo separamos las responsabilidades en filtros y ordenadores, 
/// cada uno con una correspondiente interfaz, IFilter e ISort respectivamente. Siendo esta la resultante para la operación de ordenar.
/// 
/// A su vez, cumple con el principio de inversión de dependencias (DIP), ya que las clases concrtas que ordenen dependerán de esta interfaz
/// mientras que las clases que quieran usar esta funcionalidad de ordenar (los comandos), dependen de esta abstracción y no de una clase concreta en particular.
/// También se cumple con LSP puesto que se en el programa se puede sustituir un ordenar por cualquier otro  que también implemente el tipo ISort. 
/// Con esto también podemos decir que se rige bajo Polimorfismo, ya que en cada clase concreta (subtipo) se implementa un comportamiento especifico de ella, estando la firma
/// definida en la interfaz (supertipo).  
/// </summary>
public interface ISort
{
    /// <summary>
    /// Es el nombre que tendrá el ordenardor y que lo diferenciará de otros ordenadores
    /// </summary>
    /// <value>El nombre que se le especifique</value>
    public string Name { get; }

    /// <summary>
    /// Operación polimorfica que va a variar dependiendo de la clase concreta que lo implemente. De todas formas 
    /// se encarga de ordenar las ofertas de trabajo en base al criterio marcado para su existencia y devuelve un texto
    /// con la información ordeanda. 
    /// </summary>
    /// <param name="userID"> En algunos casos se requiere de la información del solicitante para poder hacer la ordenación, 
    /// como en el caso de la distancia</param>
    /// <returns>Como cada ordenador manejar información diferente, es el ordenador el encargado de armar el mensaje
    ///  con los resultados del proceso de ordenación. Por eso devuelve un string</returns>
    public string Order(long userID);

}