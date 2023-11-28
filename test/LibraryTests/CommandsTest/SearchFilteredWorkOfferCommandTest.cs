using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using ClassLibrary;
using NUnit.Framework;

// Clase responsabilidad de Agustín Toya.

namespace Tests;
/// <summary>
/// Test que prueba la función del comando encargado de buscar y devolver todas las ofertas de trabajo que se han hechos filtradas por la cateogoría indicada
/// </summary>
[TestFixture]
public class SearchFilteredWorkOfferCommandTest
{
    private long workerID = 0;
    private long employerID = 500;
    private long adminID = 1000;

    const string nameUser = "Jose";
    const string lastNameUser = "Lamas";
    const string phoneUser = "094312866";
    const string addressUser = "La Teja";
    const double latitudeUser = -34.0832;
    const double longitudeUser = -36.0832;

    int ID = 0;

    int ID2 = 0;

    int ID3 = 0;

    /// <summary>
    /// Hacemos configuraciones generales para poder usar en los test
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        int cantWorkers = Database.Instance.GetWorkers().Count;
        int cantEmployers = Database.Instance.GetEmployers().Count;
        int cantAdmins = Database.Instance.GetAdmins().Count;
        if (cantWorkers != 0)
        {
            long workerIDBiggest = Database.Instance.GetWorkers()[cantWorkers - 1].UserID;
            workerID += workerIDBiggest + 1;
        }
        if (cantEmployers != 0)
        {
            long employerIDBiggest = Database.Instance.GetEmployers()[cantEmployers - 1].UserID;
            employerID += employerIDBiggest + 1 - 500;
        }
        if (cantAdmins != 0)
        {
            long adminBiggestUserID = Database.Instance.GetAdmins()[cantAdmins - 1].UserID;
            adminID += adminBiggestUserID + 1 - 500;
        }

        employerID +=1;
        workerID +=1;
        long employerTestID = employerID;
        long workerTestID = workerID;
        long ownerWorkOffer = workerTestID;

        Database.Instance.AddWorker(workerTestID, nameUser, lastNameUser, phoneUser, addressUser, latitudeUser, longitudeUser);
        Database.Instance.AddEmployer(employerID, nameUser, lastNameUser, phoneUser, addressUser, latitudeUser, longitudeUser);
        
        const string description = "Una oferta de trabajo";
        const string moneda = "UYU";
        const int price = 2000;
        Database.Instance.AddCategory("Traslados");
        Database.Instance.AddCategory("Fletes");
        const int durationWorkOffer = 20;
        List<string> catogories = new List<string>() { "Traslados", "Fletes" };
        // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
        // me pareció coherente que el ID lo configure en base a ese criterio
        ReadOnlyCollection<WorkOffer> offers = Database.Instance.GetAllWorkOffers();
        ID = Database.Instance.UltimateIDWorkOffer + 1;
        Database.Instance.AddWorkOffer(description, moneda, price, ownerWorkOffer, catogories, durationWorkOffer);
        // Oferta 2
        const string description2 = "SEgunda oferta de trabajo";
        const string moneda2 = "USD";
        const int price2 = 100;
        Database.Instance.AddCategory("GASTRONOMIA");
        const int durationWorkOffer2 = 1;
        List<string> catogories2 = new List<string>() { "GASTRONOMIA" };
        // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
        // me pareció coherente que el ID lo configure en base a ese criterio
        ReadOnlyCollection<WorkOffer> offers2 = Database.Instance.GetAllWorkOffers();
        ID2 = Database.Instance.UltimateIDWorkOffer + 1;
        Database.Instance.AddWorkOffer(description2, moneda2, price2, ownerWorkOffer, catogories2, durationWorkOffer2);
        // Oferta 3
        const string description3 = "Tercera oferta de trabajo";
        const string moneda3 = "USD";
        const int price3 = 100;
        const int durationWorkOffer3 = 20;
        List<string> catogories3 = new List<string>() { "GASTRONOMIA" };
        // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
        // me pareció coherente que el ID lo configure en base a ese criterio
        ID3 = Database.Instance.UltimateIDWorkOffer + 1;
        Database.Instance.AddWorkOffer(description3, moneda3, price3, ownerWorkOffer, catogories3, durationWorkOffer3);
    }

        /// <summary>
        /// Prueba que un worker intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void WorkerTrySearchFiltered()
        {
            workerID += 1;
            string commandName = "/searchfilteredworkoffer";
            // Datos del worker
            const string name = "José";
            const string lastName = "Rums";
            const string phone = "095180399";
            const string address = "Cordón";
            const double latitude = -34.99659313;
            const double longitude = -50.5730071;
            // Agrego el worker
            Database.Instance.AddWorker(workerID, name, lastName, phone, address, latitude, longitude);
            
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(workerID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

        /// <summary>
        /// Prueba que un admin intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void AdminTrySearchFiltered()
        {
            adminID += 312129;
            string commandName = "/searchfilteredworkoffer";
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

    /// <summary>
    /// Prueba que el comando funcione cuando quiere filtar por categoría
    /// </summary>
    [Test]
    public void FilterdByCategoryTest()
    {
        //Creamos tres ofertas de trabajo
        // Configuración
        // Tenemos que crear al worker que es dueño de las ofertas y el employer que hace uso de este comando
        long expectedUserID = employerID;
       

        // Mensajes que devulve el comando al ejecutarlo
        // Respuesta cuando se ejecuta en un principio
        const string messageStartCommand = $"Esta ejecutando el comando de filtrar ofertas de trabajo. Ingrese uno de los siguientes criterios validos: \n->category";
        // Respuesta cuando se mete el nombre del filtro
        const string messageResponsePromtFilter = "Perfecto, ahora se usará ese filtro. Ahora ingrese una categoría para filtrar las ofertas de trabajo";

        //Comportamiento
        const string expectedCommand = "/searchfilteredworkoffer";
        // Ingresamos el nombre del comando para empezar a usarlo
        string responseCommand =  InputInterfacer.Instance.TranslateToCommand(expectedUserID, expectedCommand);
        Assert.AreEqual(responseCommand, messageStartCommand);

        // Ingresamos el filtro que queremos usar, category
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "category");
        Assert.AreEqual(responseCommand, messageResponsePromtFilter);

        // Ingresamos la categoría que queremos buscar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "gastronomía");

        // Ahora pasamos a revisar si en la respuesta estan los workOffer ID
        string result = responseCommand;

        // Ahora verificamos que la cadena devuelta contega los identify de las dos ofertas con 
        // categoría GASTRONOMIA
        bool foundID1 = result.Contains($"_{ID}_");
        bool foundID2 = result.Contains($"_{ID2}_");
        bool foundID3 = (result.Contains($"_{ID3}_"));
        bool sucess = !foundID1 && foundID2 && foundID3;
        Assert.IsTrue(sucess); 
    }

      /// <summary>
    /// Prueba que el comando funcione cuando quiere filtar por categoría
    /// </summary>
    [Test]
    public void FilterdByCategoryWithAProblems()
    {
        //Creamos tres ofertas de trabajo
        // Configuración
        // Tenemos que crear al worker que es dueño de las ofertas y el employer que hace uso de este comando
        long expectedUserID = employerID;
       

        // Mensajes que devulve el comando al ejecutarlo
        // Respuesta cuando se ejecuta en un principio
        const string messageStartCommand = $"Esta ejecutando el comando de filtrar ofertas de trabajo. Ingrese uno de los siguientes criterios validos: \n->category";
        // Respuesta cuando se mete el nombre del filtro
        const string messageResponsePromtFilterNotFound = "No se ha encontrado ningún filtro que concida con el que especifico";
        // Respuesta cuando se mete el nombre del filtro
        const string messageResponsePromtFilter = "Perfecto, ahora se usará ese filtro. Ahora ingrese una categoría para filtrar las ofertas de trabajo";
        // Respuesta cuando no existe la categoría
        const string messageResponsePromtNoExistCategory = "No existe esa categoría en el sistema";

        //Comportamiento
        const string expectedCommand = "/searchfilteredworkoffer";
        // Ingresamos el nombre del comando para empezar a usarlo
        string responseCommand =  InputInterfacer.Instance.TranslateToCommand(expectedUserID, expectedCommand);
        Assert.AreEqual(responseCommand, messageStartCommand);

        // Ingresamos el filtro que queremos usar pero no existe
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "No existe filtro");
        Assert.AreEqual(messageResponsePromtFilterNotFound, responseCommand);

        // Ingresamos el nombre del filtro eque es valido
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "category");
        Assert.AreEqual(responseCommand, messageResponsePromtFilter);

        // Ingresamos la categoría que queremos buscar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "No existe categoría");
        Assert.AreEqual(messageResponsePromtNoExistCategory, responseCommand);
        
        // Ingresamos la categoría que queremos buscar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "gastronomía");

        // Ahora pasamos a revisar si en la respuesta estan los workOffer ID
        string result = responseCommand;

        // Ahora verificamos que la cadena devuelta contega los identify de las dos ofertas con 
        // categoría GASTRONOMIA
        bool foundID1 = result.Contains($"_{ID}_");
        bool foundID2 = result.Contains($"_{ID2}_");
        bool foundID3 = (result.Contains($"_{ID3}_"));
        bool sucess = !foundID1 && foundID2 && foundID3;
        Assert.IsTrue(sucess); 
    }

    


    /// <summary>
    /// Prueba que el comando funcione cuando quiere filtar por categoría pero no hay una lista de ofertas
    /// </summary>
    [Test]
    public void FilterdByCategoryButNoExistWorkOffers()
    {
        Database.Instance.ClearWorkOffers();
        //Creamos tres ofertas de trabajo
        // Configuración
        // Tenemos que crear al worker que es dueño de las ofertas y el employer que hace uso de este comando
        long expectedUserID = employerID;
       

        // Mensajes que devulve el comando al ejecutarlo
        // Respuesta cuando se ejecuta en un principio
        const string messageStartCommand = $"Esta ejecutando el comando de filtrar ofertas de trabajo. Ingrese uno de los siguientes criterios validos: \n->category";
        // Respuesta cuando se mete el nombre del filtro
        // Respuesta cuando se mete el nombre del filtro
        const string messageResponsePromtFilter = "Perfecto, ahora se usará ese filtro. Ahora ingrese una categoría para filtrar las ofertas de trabajo";
        // Respuesta cuando no existe la categoría
        const string messageResponsePromtListIsEmtpy = "La lista de ofertas de trabajo esta vacía, no hay nada para filtrar.";

        //Comportamiento
        const string expectedCommand = "/searchfilteredworkoffer";
        // Ingresamos el nombre del comando para empezar a usarlo
        string responseCommand =  InputInterfacer.Instance.TranslateToCommand(expectedUserID, expectedCommand);
        Assert.AreEqual(responseCommand, messageStartCommand);


        // Ingresamos el nombre del filtro eque es valido
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "category");
        Assert.AreEqual(responseCommand, messageResponsePromtFilter);
        
        // Ingresamos la categoría que queremos buscar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "gastronomía");
        Assert.AreEqual(messageResponsePromtListIsEmtpy, responseCommand);
    }

    /// <summary>
    /// Prueba que el comando funcione cuando quiere filtar por categoría pero no hay una lista de ofertas
    /// </summary>
    [Test]
    public void FilterdByCategoryCancelInFirstSetp()
    {
        //Creamos tres ofertas de trabajo
        // Configuración
        // Tenemos que crear al worker que es dueño de las ofertas y el employer que hace uso de este comando
        long expectedUserID = employerID;
        long ownerWorkOffer = workerID;
        // Creamos otra oferta, la cuarta en este test
        const string description4 = "Cuarta oferta de trabajo";
        const string moneda4 = "USD";
        const int price4 = 100;
        const int durationWorkOffer4 = 20;
        List<string> catogories4 = new List<string>() { "GASTRONOMIA" };
        // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
        // me pareció coherente que el ID lo configure en base a ese criterio
        ID3 = Database.Instance.UltimateIDWorkOffer + 1;
        Database.Instance.AddWorkOffer(description4, moneda4, price4, ownerWorkOffer, catogories4, durationWorkOffer4);
       
        // Ingresa la opción cancelar
        const string cancelInput = "cancel";

        // Mensajes que devulve el comando al ejecutarlo
        // Respuesta cuando se ejecuta en un principio
        const string messageStartCommand = $"Esta ejecutando el comando de filtrar ofertas de trabajo. Ingrese uno de los siguientes criterios validos: \n->category";
        // Mensaje que devuelve cuando le da a cancel
        const string cancelResponse = $"El comando /searchfilteredworkoffer se canceló";

        //Comportamiento
        const string expectedCommand = "/searchfilteredworkoffer";
        // Ingresamos el nombre del comando para empezar a usarlo
        string responseCommand =  InputInterfacer.Instance.TranslateToCommand(expectedUserID, expectedCommand);
        Assert.AreEqual(responseCommand, messageStartCommand);
        
        // Ingresamos el cancelar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, cancelInput);
        Assert.AreEqual(cancelResponse, responseCommand);
    }

    /// <summary>
    /// Prueba que el comando funcione cuando quiere filtar por categoría pero no hay una lista de ofertas
    /// </summary>
    [Test]
    public void FilterdByCategoryCancelInSecondStep()
    {
        //Creamos tres ofertas de trabajo
        // Configuración
        // Tenemos que crear al worker que es dueño de las ofertas y el employer que hace uso de este comando
        long expectedUserID = employerID;
        long ownerWorkOffer = workerID;
        // Creamos otra oferta, la cuarta en este test
        const string description4 = "Cuarta oferta de trabajo";
        const string moneda4 = "USD";
        const int price4 = 100;
        const int durationWorkOffer4 = 20;
        List<string> catogories4 = new List<string>() { "GASTRONOMIA" };
        // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
        // me pareció coherente que el ID lo configure en base a ese criterio
        ID3 = Database.Instance.UltimateIDWorkOffer + 1;
        Database.Instance.AddWorkOffer(description4, moneda4, price4, ownerWorkOffer, catogories4, durationWorkOffer4);
       
        // Ingresa la opción cancelar
        const string cancelInput = "cancel";

        // Mensajes que devulve el comando al ejecutarlo
        // Respuesta cuando se ejecuta en un principio
        const string messageStartCommand = $"Esta ejecutando el comando de filtrar ofertas de trabajo. Ingrese uno de los siguientes criterios validos: \n->category";
        // Respuesta cuando se mete el nombre del filtro
        // Respuesta cuando se mete el nombre del filtro
        const string messageResponsePromtFilter = "Perfecto, ahora se usará ese filtro. Ahora ingrese una categoría para filtrar las ofertas de trabajo";
        // Mensaje que devuelve cuando le da a cancel
        const string cancelResponse = $"El comando /searchfilteredworkoffer se canceló";

        //Comportamiento
        const string expectedCommand = "/searchfilteredworkoffer";
        // Ingresamos el nombre del comando para empezar a usarlo
        string responseCommand =  InputInterfacer.Instance.TranslateToCommand(expectedUserID, expectedCommand);
        Assert.AreEqual(responseCommand, messageStartCommand);


        // Ingresamos el nombre del filtro eque es valido
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "category");
        Assert.AreEqual(responseCommand, messageResponsePromtFilter);
        
        // Ingresamos la categoría que queremos buscar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, cancelInput);
        Assert.AreEqual(cancelResponse, responseCommand);
    }   



    
}