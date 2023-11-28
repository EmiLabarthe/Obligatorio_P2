using System.Collections.ObjectModel;
using System.Collections.Generic;
using ClassLibrary;
using NUnit.Framework;

// Clase responsabilidad de Juan Pablo Amorín y Agustín Toya.

namespace Tests;
/// <summary>
/// Test que prueba la función del comando encargado de buscar y devolver todas las categorías.
/// </summary>
/// 

[TestFixture]

public class ShowCategoriesInSystemCommandTest
{
    private long workerID = 0;
    private long employerID = 500;
    private long adminID = 1000;
    
    /// <summary>
    /// Se configuran cosas para probar en este test, como la limpieza de listas por si hay información repetida
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        int cantWorkers = Database.Instance.GetWorkers().Count;
        int cantEmployers = Database.Instance.GetEmployers().Count;
        int catnAdmins = Database.Instance.GetAdmins().Count;
        if (cantWorkers != 0)
        {
            long workerIDBiggest = Database.Instance.GetWorkers()[cantWorkers - 1].UserID;
            workerID += workerIDBiggest + 1;
            
        }
        if (cantEmployers != 0)
        {
            long employerIDBiggest = Database.Instance.GetEmployers()[cantEmployers - 1].UserID;
            this.employerID += employerIDBiggest + 1;
        }
        if (catnAdmins != 0)
        {
            long adminIDBiggest = Database.Instance.GetAdmins()[catnAdmins - 1].UserID;
            this.adminID += adminIDBiggest + 1;
        }
    }

    /// <summary>
    /// Caso de prueba de cuando un user  de telegram cualquiera que no es un usuario del sistema 
    /// intenta usar el comando
    /// </summary>
    [Test]
    public void SearchWorkOfferWhenYouDontExistInSystem()
    {
        long userStrangerID = adminID* 4;
        // Nombre del comando a ejecutar
        const string commandName = "/showallworkoffers";

        // Comportamiento
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(userStrangerID, commandName);
        const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";
        // Probamos que nos diga que no podemos usar el comando
        Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
    }
    /// <summary>
    /// Prueba que el comando devuelva todas las categorías que hay. Habiendo categorías para cualquiera de 
    /// los tres tipos de perfiles que tenemos hoy
    /// </summary>
    [Test]
    public void ShowCategoriesTest()
    {
        //Configuración
        // Cramos un worker, un admin y un employer, por que los tres pueden usar este comanndo, y probamos con los tres
        // Admin
        adminID +=1;
        workerID +=1;
        employerID +=1;

        const string nameWorker = "Pepe";
        const string lastNameWorker = "Martes";
        const string phoneWorker = "094317098";
        const string addressWorker = "Av. Italia 431";
        const double latitudeWorker = -34.7612;
        const double longitudeWorker = -34.7012;
        Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);

        const string nameEmployer = "Marta";
        const string lastNameEmployer = "Sanchez";
        const string phoneEmployer = "094317093";
        const string addressEmployer = "Av. Italia 651";
        const double latitudeEmployer = -34.7612;
        const double longitudeEmployer = -34.7012;
        Database.Instance.AddEmployer(employerID, nameEmployer, lastNameEmployer, phoneEmployer, addressEmployer, latitudeEmployer, longitudeEmployer);

        Database.Instance.AddAdmin(adminID);

        //Creamos 4 categorías
        string cat1 = "FONTANERIA";
        string cat2 = "ELECTRICISTA";
        string cat3 = "CONSTRUCCION";
        string cat4 = "MECANICA";
        Database.Instance.AddCategory(cat1);
        Database.Instance.AddCategory(cat2);
        Database.Instance.AddCategory(cat3);
        Database.Instance.AddCategory(cat4);
        // Nombre del comando
        const string commandName = "/showcategories";
        // Primero probamos con el admin
        //Comportamiento
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
        //Nos fijamos de que el comando devuelva las 4 categorias creadas anteriormente, pueden haber más, pero esas deben estar
        bool result1 = responseCommand.Contains(cat1);
        bool result2 = responseCommand.Contains(cat2);
        bool result3 = responseCommand.Contains(cat3);
        bool result4 = responseCommand.Contains(cat4);
        Assert.IsTrue(result1 && result2 && result3 && result4);

        // Ahora ejecutamos como si lo hiciera el worker
        responseCommand = InputInterfacer.Instance.TranslateToCommand(workerID, commandName);
        // Comprobamos de que también le aparezcan
        result1 = responseCommand.Contains(cat1);
        result2 = responseCommand.Contains(cat2);
        result3 = responseCommand.Contains(cat3);
        result4 = responseCommand.Contains(cat4);
        Assert.IsTrue(result1 && result2 && result3 && result4);

        // Ahora ejecutamos como si lo hiciera el employer
        responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
        // Comprobamos de que también le aparezcan
        result1 = responseCommand.Contains(cat1);
        result2 = responseCommand.Contains(cat2);
        result3 = responseCommand.Contains(cat3);
        result4 = responseCommand.Contains(cat4);
        Assert.IsTrue(result1 && result2 && result3 && result4);
    }


    /// <summary>
    /// Prueba que el comando devuelve un mensaje de advertencia al que ingresa el comando ya que esta vacía la lista. El mensaje 
    /// te lo devuelve si estas registrado en el sistema como uno de nuestros profiles,
    /// </summary>
    [Test]
    public void ShowCategoriesTestButNoExistCategories()
    {
        //Configuración
        // Limpiamos la lista de categorías
        Database.Instance.ClearCategories();

        // Cramos un worker, un admin y un employer, por que los tres pueden usar este comanndo, y probamos con los tres
        // Admin
        adminID +=1;
        workerID +=1;
        employerID +=1;
        const string nameWorker = "Pepe";
        const string lastNameWorker = "Martes";
        const string phoneWorker = "094317098";
        const string addressWorker = "Av. Italia 431";
        const double latitudeWorker = -34.7612;
        const double longitudeWorker = -34.7012;
        Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);

        const string nameEmployer = "Marta";
        const string lastNameEmployer = "Sanchez";
        const string phoneEmployer = "094317093";
        const string addressEmployer = "Av. Italia 651";
        const double latitudeEmployer = -34.7612;
        const double longitudeEmployer = -34.7012;
        Database.Instance.AddEmployer(employerID, nameEmployer, lastNameEmployer, phoneEmployer, addressEmployer, latitudeEmployer, longitudeEmployer);

        Database.Instance.AddAdmin(adminID);
        // Nombre del comando
        const string commandName = "/showcategories";
        // Mensaje que debería de devolver cuando no hay categorías a mostrar
        const string responseWhenNoExistCategories = "En la base de datos no hay categorías creadas";
        // Primero probamos con el admin
        //Comportamiento
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
        // Revisamos que devuelva el mensaje esperado
        Assert.AreEqual(responseWhenNoExistCategories, responseCommand);

        // Ahora ejecutamos como si lo hiciera el worker
        responseCommand = InputInterfacer.Instance.TranslateToCommand(workerID, commandName);
        // Revisamos que devuelva el mensaje esperado
        Assert.AreEqual(responseWhenNoExistCategories, responseCommand);


        // Ahora ejecutamos como si lo hiciera el employer
        responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
        // Revisamos que devuelva el mensaje esperado
        Assert.AreEqual(responseWhenNoExistCategories, responseCommand);
    }
}