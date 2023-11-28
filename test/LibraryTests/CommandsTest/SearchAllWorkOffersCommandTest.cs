using System.Collections.Generic;
using ClassLibrary;
using System;
using NUnit.Framework;

// Clase responsabilidad de Agustín Toya.

namespace Tests;
/// <summary>
/// Test que prueba la función del comando de mostrar todas las work offers.
/// </summary>

[TestFixture]
public class SearchAllWorkOffersCommandTest
{

    private long workerID = 0;
    private long employerID = 500;
    private long adminID = 1000;
    /// <summary>
    /// User story de buscar todas las ofertas de trabajo
    /// </summary>
    /// 
    [SetUp]
    public void Setup()
    {
        Database.Instance.ClearWorkOffers();
        int cantWorkers = Database.Instance.GetWorkers().Count;
        int cantEmployers = Database.Instance.GetEmployers().Count;
        int cantAdmins = Database.Instance.GetAdmins().Count;
        if (cantWorkers != 0)
        {
            long userIDBiggest = Database.Instance.GetWorkers()[cantWorkers - 1].UserID;
            workerID += userIDBiggest + 1;
        }
        if (cantEmployers != 0)
        {
            long userIDBiggest = Database.Instance.GetEmployers()[cantEmployers - 1].UserID;
            employerID += userIDBiggest + 1 +500;
        }
        if (cantAdmins != 0)
        {
            long userIDBiggest = Database.Instance.GetAdmins()[cantAdmins - 1].UserID;
            adminID += userIDBiggest + 1 - 500;
        }
    }

    /// <summary>
    /// Caso de prueba de cuando un user  de telegram cualquiera que no es un usuario del sistema 
    /// intenta usar el comando
    /// </summary>
    [Test]
    public void SearchWorkOfferWhenYouDontExistInSystem()
    {
        long userStrangerID = 102123123;
        // Nombre del comando a ejecutar
        const string commandName = "/showallworkoffers";

        // Comportamiento
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(userStrangerID, commandName);
        const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";
        // Probamos que nos diga que no podemos usar el comando
        Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
    }

    /// <summary>
    /// Prueba que el comando devuelva todas las work offers que hay que tiene el published en tru.
    /// </summary>
    [Test]
    public void SearchWorkOffers()
    {
        //Configuración

        //Creamos los workers que van a ser dueños de las work offers.

        workerID += 1;
        long workerID1 = workerID;
        const string name1 = "Pablo";
        const string lastName1 = "Varela";
        const string phone1 = "099539385";
        const string address1 = "Malvín";
        const double latitude1 = 5;
        const double longitude1 = 2;

        Database.Instance.AddWorker(workerID,name1,lastName1,phone1,address1,latitude1,longitude1);

        workerID += 1;
        long workerID2 = workerID;
        const string name2 = "Maite";
        const string lastName2 = "Gutiérrez";
        const string phone2 = "094253163";
        const string address2 = "Solymar";
        const double latitude2 = 26;
        const double longitude2 = 1;

        Database.Instance.AddWorker(workerID,name2,lastName2,phone2,address2,latitude2,longitude2);

        // El que puede ejcutar el comando sería un employer por lo que vamos a crear uno de prueba
        employerID += 3;
        const string name3 = "Lisa";
        const string lastName3 = "Mayer";
        const string phone3 = "094253163";
        const string address3 = "Solymar";
        const double latitude3 = 26;
        const double longitude3 = 1;

        Database.Instance.AddEmployer(employerID,name3,lastName3,phone3,address3,latitude3,longitude3);

        //Creamos las categorías.

        Database.Instance.AddCategory("ELECTRICISTA");
        Database.Instance.AddCategory("JARDINERIA");
        Database.Instance.AddCategory("CARPINTERIA");

        //Creamos las work offers.

        const string description1 = "Oferta de trabajo numero 1";
        const string moneda1 = "UYU";
        const int price1 = 2000;
        long ownerID1 = workerID1;
        List<string> categories1 = new List<string>() { "ELECTRICISTA" };
        const int durationWorkOffer1 = 5;
        int identify1 = Database.Instance.UltimateIDWorkOffer +1;

        Database.Instance.AddWorkOffer(description1,moneda1,price1,ownerID1,categories1,durationWorkOffer1);
        
        const string description2 = "Oferta de trabajo numero 2";
        const string moneda2 = "UYU";
        const int price2 = 800;
        long ownerID2 = workerID1;
        List<string> categories2 = new List<string>() { "JARDINERIA" };
        const int durationWorkOffer2 = 3;
        int identify2 = Database.Instance.UltimateIDWorkOffer +1;
        
        Database.Instance.AddWorkOffer(description2,moneda2,price2,ownerID2,categories2,durationWorkOffer2);
        
        const string description3 = "Oferta de trabajo numero 3";
        const string moneda3 = "UYU";
        const int price3 = 1100;
        long ownerID3 = workerID2;
        List<string> categories3 = new List<string>() { "JARDINERIA" };
        const int durationWorkOffer3 = 3;
        int identify3 = Database.Instance.UltimateIDWorkOffer +1;
        
        Database.Instance.AddWorkOffer(description3,moneda3,price3,ownerID3,categories3,durationWorkOffer3);
        
        const string description4 = "Oferta de trabajo numero 4";
        const string moneda4 = "UYU";
        const int price4 = 2300;
        long ownerID4 = workerID2;
        List<string> categories4 = new List<string>() { "CARPINTERIA" };
        const int durationWorkOffer4 = 15;
        int identify4 = Database.Instance.UltimateIDWorkOffer +1;
        
        Database.Instance.AddWorkOffer(description4,moneda4,price4,ownerID4,categories4,durationWorkOffer4);
        // Para demostrar que solo muestra las ofertas que estan en published = true, damos de baja a la tercer oferta
        WorkOffer offerWillDelete  = Database.Instance.SearchWorkOffer(identify3);
        offerWillDelete.Delete();

        // Nombre del comando a ejecutar
        const string commandName = "/showallworkoffers";

        // Comportamiento
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
        
        // Ahora revisamos que el mensaje que devuelve contine la info que le ingresamos, si ya tiene 
        // los id que recuperamos y la descripción, se puede dar por valido
        string workOfferID1String = $"Identificador: {identify1}";
        string workOfferID2String = $"Identificador: {identify2}";
        string workOfferID3String = $"Identificador: {identify3}";
        string workOfferID4String = $"Identificador: {identify4}";
        // Ahora las descripciones
        bool existWorkOffer1 = responseCommand.Contains(workOfferID1String) && responseCommand.Contains(description1);
        bool existWorkOffer2 = responseCommand.Contains(workOfferID2String) && responseCommand.Contains(description2);
        bool existWorkOffer3 = responseCommand.Contains(workOfferID3String) && responseCommand.Contains(description3);
        bool existWorkOffer4 = responseCommand.Contains(workOfferID4String) && responseCommand.Contains(description4);
        
        Assert.True(existWorkOffer1 && existWorkOffer2 && !existWorkOffer3 && existWorkOffer4);
    }

    /// <summary>
    /// Test que prueba a ver que responde cuando no hay ninguna oferta puesta en el sistema como para responder
    /// </summary>
    [Test]
    public void ShowAllWorkOffersButListIsEmpty()
    {
        // Configuración
        // Limpiamos la lista de ofertas para asegurarnos que no hay nada
        Database.Instance.ClearWorkOffers();
        // El que puede ejcutar el comando sería un employer por lo que vamos a crear uno de prueba
        employerID += 12;
        const string name3 = "Lisa";
        const string lastName3 = "Mayer";
        const string phone3 = "094253163";
        const string address3 = "Solymar";
        const double latitude3 = 26;
        const double longitude3 = 1;

        Database.Instance.AddEmployer(employerID,name3,lastName3,phone3,address3,latitude3,longitude3);

        const string expectedResponseCommand = "No hay ofertas en el sistema en este momento, por favor espera a que un worker ponga su oferta a disposición.";
        // Nombre del comando a ejecutar
        const string commandName = "/showallworkoffers";

        // Comportamiento
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);

        // Comprobación
        Assert.AreEqual(responseCommand, expectedResponseCommand);
    }
}