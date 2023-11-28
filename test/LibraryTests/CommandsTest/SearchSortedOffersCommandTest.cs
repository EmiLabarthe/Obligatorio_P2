using System.Collections.Generic;
using ClassLibrary;
using NUnit.Framework;
using System;

// Clase responsabilidad de Agustín Toya.

namespace Tests;

/// <summary>
/// Test que prueba la función del comando de mostrar todas las work offers.
/// </summary>
[TestFixture]
public class SearchSortedOfferCommand
{
    private long workerID = 0;
    private long employerID = 500;
    private long adminID = 1000;


    /// <summary>
    /// User story de registrarse como employer
    /// </summary>
    /// 
    [SetUp]
    public void Setup()
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
        /// Prueba que un worker intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void WorkerTrySearchSorted()
        {
            workerID += 1;
            string commandName = "/searchsortedworkoffer";
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
        public void AdminTrySearchSorted()
        {
            adminID += 12;
            string commandName = "/searchsortedworkoffer";
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

    /// <summary>
    /// Probamos el caso de ordenar ofertas por la ubicación del dueño, cuando la lista no esta vacía.
    /// </summary>
    //Test]
    public void SearchSortedWorkOffersByDistance()
    {
        //Configuración
        // Creamos al employer que se supone que sería el que esta ejecutando este comando
        long employerIDTest = employerID + 1;
        employerID+=1;
        const string name = "Tomas";
        const string lastName = "Aquino";
        const string phone = "099139385";
        const string address = "Estero Bellaco 2771";
        const double latitude = -34.8880311715963;   
        const double longitude = -56.15881920239052;

        Database.Instance.AddEmployer(employerIDTest, name, lastName, phone, address, latitude, longitude);

        //Creamos los workers que van a ser dueños de las work offers.
        long workerID1 = workerID + 1;
        workerID+=1;
        const string name1 = "Pablo";
        const string lastName1 = "Varela";
        const string phone1 = "099539385";
        const string address1 = "Malvín";
        const double latitude1 = -34.89360791777325;  
        const double longitude1 = -56.10992014308215;

        Database.Instance.AddWorker(workerID1, name1, lastName1, phone1, address1, latitude1, longitude1);
        
        long workerID2 = workerID +1;
        workerID+=1;
        const string name2 = "Maite";
        const string lastName2 = "Gutiérrez";
        const string phone2 = "094253163";
        const string address2 = "Av. Giannattasio, 15600 Ciudad de la Costa, Departamento de Canelones";
        const double latitude2 = -34.825208;   
        const double longitude2 = -55.964197;

        Database.Instance.AddWorker(workerID2, name2, lastName2, phone2, address2, latitude2, longitude2);
        
        long workerID3 = workerID +1;
        workerID +=1;
        const string name3 = "Lucas";
        const string lastName3 = "Delgado";
        const string phone3 = "094253163";
        const string address3 = "Dr. Carlos María Ramiréz 498";
        const double latitude3 = -34.860661414303834; 
        const double longitude3 =  -56.23004691773492;

        Database.Instance.AddWorker(workerID3, name3, lastName3, phone3, address3, latitude3, longitude3);

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

        Database.Instance.AddWorkOffer(description1, moneda1, price1, ownerID1, categories1, durationWorkOffer1);

        const string description2 = "Oferta de trabajo numero 2";
        const string moneda2 = "UYU";
        const int price2 = 800;
        long ownerID2 = workerID2;
        List<string> categories2 = new List<string>() { "JARDINERIA" };
        const int durationWorkOffer2 = 3;
        int workOfferID2 = Database.Instance.UltimateIDWorkOffer +1;
        Database.Instance.AddWorkOffer(description2, moneda2, price2, ownerID2, categories2, durationWorkOffer2);
        
        const string description3 = "Oferta de trabajo numero 3";
        const string moneda3 = "UYU";
        const int price3 = 1100;
        long ownerID3 = workerID2;
        List<string> categories3 = new List<string>() { "JARDINERIA" };
        const int durationWorkOffer3 = 3;
        int workOfferID3 = Database.Instance.UltimateIDWorkOffer +1;
        Database.Instance.AddWorkOffer(description3, moneda3, price3, ownerID3, categories3, durationWorkOffer3);


        const string description4 = "Oferta de trabajo numero 4";
        const string moneda4 = "UYU";
        const int price4 = 2300;
        long ownerID4 = workerID3;
        List<string> categories4 = new List<string>() { "CARPINTERIA" };
        const int durationWorkOffer4 = 15;

        Database.Instance.AddWorkOffer(description4, moneda4, price4, ownerID4, categories4, durationWorkOffer4);
        const string validCriterionsString = "\n->location\n->rates";
        // Respuesta esperada de ejecutar el comando en el primer estado o paso
        const string responseStart = $"Ingrese el criterio para poder ordenar \nCriterios validos: {validCriterionsString}";
        const string nameCommand = "/searchsortedworkoffer";
        const string criterion = "location";

        // Ahora toca ejecutar el comando de buscar ordenadamente, revisamos que entre en el cada paso
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, nameCommand);
        //Assert.AreEqual(responseStart, responseCommand);

        // Ahora ingresamos el criterio de ubicación, y la respuesta si todo fue bien, contendrá un string con toda la información
        // de las ofertas ordandas.
        responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, criterion);

        // Ahora hay que revisar que aparezcan en orden, el que esta más cerca por los datos que introducimos es el 
        // de Malvin, el siguiente es el de Carlos María Ramirez y el más lejano es el de Solymar.
        // De la mano de IndexOf podemos sacar la posición en la cadena de caracteres, la que este más cerca aparecerá primero
        // por lo que el que tenga el valor de índice más pequeño es el que esta más cerca
        string formatWorkerID1 = $"ID del dueño: {workerID1}";
        string formatWorkerID2 = $"ID del dueño: {workerID2}";
        string formatWorkerID3 = $"ID del dueño: {workerID3}";
        int positionMoreNear = responseCommand.IndexOf(formatWorkerID1);
        int positionLessNear = responseCommand.IndexOf(formatWorkerID2);
        int positionInTheMiddle = responseCommand.IndexOf(formatWorkerID3);
      


        // Hacemos las comprobaciones
        bool moreNearTrue = positionMoreNear < positionInTheMiddle && positionMoreNear < positionLessNear;
        bool inTheMiddelTrue = positionMoreNear < positionInTheMiddle && positionInTheMiddle < positionLessNear;
        bool lessNearTrue = positionMoreNear < positionLessNear && positionInTheMiddle < positionLessNear;

        bool successed = moreNearTrue && inTheMiddelTrue && lessNearTrue;
        //Assert.IsTrue(successed);
    }

    /// <summary>
    /// Ahora probamos que hace el comando cuando el employer quiere ordenar las ofertas por ubicación, 
    /// pero la lista que no tiene elementos.
    /// </summary>
    [Test]
    public void SearchSortedByLocationInAnEmptyList()
    {
        // Creamos el employer que ejecutaría el comando por las dudas
        employerID+=1;
        long employerIDTest = employerID;
        const string name = "Tomas";
        const string lastName = "Aquino";
        const string phone = "099139385";
        const string address = "Estero Bellaco 2771";
        const double latitude = -34.8880311715963;   
        const double longitude = -56.15881920239052;
        Database.Instance.AddEmployer(employerIDTest, name, lastName, phone, address, latitude, longitude);

        // En este caso hay que optar por el plan B para hacer pruebas unitarias, hay que vaciar la lista de workOffers
        // Sino no hay otra forma de que le diga que la lista esta vacía.
        Database.Instance.ClearWorkOffers();
        const string validCriterionsString = "\n->location\n->rates";
        // Respuesta esperada de ejecutar el comando en el primer estado o paso
        const string responseStart = $"Ingrese el criterio para poder ordenar \nCriterios validos: {validCriterionsString}";
        const string responseWhenNotFoundWorkOffers = "No hay ofertas en el sistema como para poder ser ordeandas, debe esperar a que un trabajador postule algún trabajo";
        const string nameCommand = "/searchsortedworkoffer";
        const string criterion = "location";

        // Ahora toca ejecutar el comando de buscar ordenadamente, revisamos que entre en el cada paso
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, nameCommand);
        Assert.AreEqual(responseStart, responseCommand);

        // Ahora ingresamos el criterio de ubicación, y como no hay ofertas a ordenar tendría que decir algo como que no hay nadad para ordenar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, criterion);
        Assert.AreEqual(responseWhenNotFoundWorkOffers, responseCommand);
    }

    /// <summary>
    /// Probamos el caso de ordenar ofertas por la calificación del dueño, cuando la lista no esta vacía.
    /// </summary>
    [Test]
    public void SearchSortedWorkOffersByRate()
    {
        //Configuración
        // Creamos al employer que se supone que sería el que esta ejecutando este comando
        long employerIDTest = employerID + 1;
        employerID+=1;
        const string name = "Tomas";
        const string lastName = "Aquino";
        const string phone = "099139385";
        const string address = "Estero Bellaco 2771";
        const double latitude = -34.8880311715963;   
        const double longitude = -56.15881920239052;
        Database.Instance.AddEmployer(employerIDTest, name, lastName, phone, address, latitude, longitude);

        //Creamos los workers que van a ser dueños de las work offers.
        long workerID1 = workerID + 1;
        workerID+=1;
        const string name1 = "Pablo";
        const string lastName1 = "Varela";
        const string phone1 = "099539385";
        const string address1 = "Malvín";
        const double latitude1 = -34.89360791777325;  
        const double longitude1 = -56.10992014308215;

        Database.Instance.AddWorker(workerID1, name1, lastName1, phone1, address1, latitude1, longitude1);
        
        long workerID2 = workerID +1;
        workerID+=1;
        const string name2 = "Maite";
        const string lastName2 = "Gutiérrez";
        const string phone2 = "094253163";
        const string address2 = "Av. Giannattasio, 15600 Ciudad de la Costa, Departamento de Canelones";
        const double latitude2 = -34.825208;   
        const double longitude2 = -55.964197;

        Database.Instance.AddWorker(workerID2, name2, lastName2, phone2, address2, latitude2, longitude2);
        
        long workerID3 = workerID +1;
        workerID +=1;
        const string name3 = "Lucas";
        const string lastName3 = "Delgado";
        const string phone3 = "094253163";
        const string address3 = "Dr. Carlos María Ramiréz 498";
        const double latitude3 = -34.860661414303834; 
        const double longitude3 =  -56.23004691773492;

        Database.Instance.AddWorker(workerID3, name3, lastName3, phone3, address3, latitude3, longitude3);

        //Creamos las categorías.

        Database.Instance.AddCategory("ELECTRICISTA");
        Database.Instance.AddCategory("JARDINERIA");
        Database.Instance.AddCategory("CARPINTERIA");

        //Creamos las work offers.
        int workOfferID1 = Database.Instance.UltimateIDWorkOffer + 1;
        const string description1 = "Oferta de trabajo numero 1";
        const string moneda1 = "UYU";
        const int price1 = 2000;
        long ownerID1 = workerID1;
        List<string> categories1 = new List<string>() { "ELECTRICISTA" };
        const int durationWorkOffer1 = 5;

        Database.Instance.AddWorkOffer(description1, moneda1, price1, ownerID1, categories1, durationWorkOffer1);

        int workOfferID2 = Database.Instance.UltimateIDWorkOffer + 1;
        const string description2 = "Oferta de trabajo numero 2";
        const string moneda2 = "UYU";
        const int price2 = 800;
        long ownerID2 = workerID2;
        List<string> categories2 = new List<string>() { "JARDINERIA" };
        const int durationWorkOffer2 = 3;

        Database.Instance.AddWorkOffer(description2, moneda2, price2, ownerID2, categories2, durationWorkOffer2);

        int workOfferID3 = Database.Instance.UltimateIDWorkOffer + 1;
        const string description3 = "Oferta de trabajo numero 3";
        const string moneda3 = "UYU";
        const int price3 = 1100;
        long ownerID3 = workerID2;
        List<string> categories3 = new List<string>() { "JARDINERIA" };
        const int durationWorkOffer3 = 3;

        Database.Instance.AddWorkOffer(description3, moneda3, price3, ownerID3, categories3, durationWorkOffer3);
        
        int workOfferID4 = Database.Instance.UltimateIDWorkOffer + 1;
        const string description4 = "Oferta de trabajo numero 4";
        const string moneda4 = "UYU";
        const int price4 = 2300;
        long ownerID4 = workerID3;
        List<string> categories4 = new List<string>() { "CARPINTERIA" };
        const int durationWorkOffer4 = 15;

        Database.Instance.AddWorkOffer(description4, moneda4, price4, ownerID4, categories4, durationWorkOffer4);

        // Una vez creados los workers, el employer y las ofertas, calificamos en base a la oferta
        // Primero recuperamos los objetos correspondientes
        Worker worker1 = Database.Instance.SearchWorker(workerID1);
        Worker worker2 = Database.Instance.SearchWorker(workerID2);
        Worker worker3 = Database.Instance.SearchWorker(workerID3);
        // Ahora agregamos las calificaciones pero no tienen puntaje todavía
        worker1.AddRating(employerIDTest, durationWorkOffer1, workOfferID1);
        worker2.AddRating(employerIDTest, durationWorkOffer2, workOfferID2);
        worker2.AddRating(employerIDTest, durationWorkOffer3, workOfferID3);
        worker3.AddRating(employerIDTest, durationWorkOffer4, workOfferID4);
        // Ahora calificamos
        worker1.ReciveCalification(10,employerIDTest, workOfferID1,true);
        worker2.ReciveCalification(10,employerIDTest, workOfferID2,true);
        worker2.ReciveCalification(2, employerIDTest, workOfferID3,true);
        worker3.ReciveCalification(4, employerIDTest, workOfferID4,true);        

        const string validCriterionsString = "\n->location\n->rates";
        // Respuesta esperada de ejecutar el comando en el primer estado o paso
        const string responseStart = $"Ingrese el criterio para poder ordenar \nCriterios validos: {validCriterionsString}";
        const string nameCommand = "/searchsortedworkoffer";
        const string criterion = "rates";



        // Ahora toca ejecutar el comando de buscar ordenadamente, revisamos que entre en el cada paso
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, nameCommand);
        Assert.AreEqual(responseStart, responseCommand);

        // Ahora ingresamos el criterio de ubicación, y la respuesta si todo fue bien, contendrá un string con toda la información
        // de las ofertas ordandas.
        responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, criterion);

        // Ahora hay que revisar que aparezcan en orden, el que esta más cerca por los datos que introducimos es el 
        // de Malvin, el siguiente es el de Carlos María Ramirez y el más lejano es el de Solymar.
        // De la mano de IndexOf podemos sacar la posición en la cadena de caracteres, la que este más cerca aparecerá primero
        // por lo que el que tenga el valor de índice más pequeño es el que esta más cerca
        string formatWorkerID1 = $"ID del dueño: {workerID1}";
        string formatWorkerID2 = $"ID del dueño: {workerID2}";
        string formatWorkerID3 = $"ID del dueño: {workerID3}";

        // El que tenga mejor calificación debe salilr primero, por lo que tendrá una posición más pequeña en el string
        int positionMoreNear = responseCommand.IndexOf(formatWorkerID1);
        // El de menor putaje irá último y tendrá una posición alta
        int positionLessNear = responseCommand.IndexOf(formatWorkerID3);
        // El de puntaje en el medio, tendrá también una posición en el medio
        int positionInTheMiddle = responseCommand.IndexOf(formatWorkerID2);


        // Hacemos las comprobaciones
        bool moreNearTrue = positionMoreNear < positionInTheMiddle && positionMoreNear < positionLessNear;
        bool inTheMiddelTrue = positionMoreNear < positionInTheMiddle && positionInTheMiddle < positionLessNear;
        bool lessNearTrue = positionMoreNear < positionLessNear && positionInTheMiddle < positionLessNear;

        bool successed = moreNearTrue && inTheMiddelTrue && lessNearTrue;
        Assert.IsTrue(successed);
    }

    /// <summary>
    /// Ahora probamos que hace el comando cuando el employer quiere ordenar las ofertas por calificación, 
    /// pero la lista que no tiene elementos.
    /// </summary>
    [Test]
    public void SearchSortedByRateInAnEmptyList()
    {
        // Creamos el employer que ejecutaría el comando por las dudas
        employerID+=1;
        long employerIDTest = employerID;
        const string name = "Tomas";
        const string lastName = "Aquino";
        const string phone = "099139385";
        const string address = "Estero Bellaco 2771";
        const double latitude = -34.8880311715963;   
        const double longitude = -56.15881920239052;
        Database.Instance.AddEmployer(employerIDTest, name, lastName, phone, address, latitude, longitude);

        // En este caso hay que optar por el plan B para hacer pruebas unitarias, hay que vaciar la lista de workOffers
        // Sino no hay otra forma de que le diga que la lista esta vacía.
        Database.Instance.ClearWorkOffers();
        const string validCriterionsString = "\n->location\n->rates";
        // Respuesta esperada de ejecutar el comando en el primer estado o paso
        const string responseStart = $"Ingrese el criterio para poder ordenar \nCriterios validos: {validCriterionsString}";
        const string responseWhenNotFoundWorkOffers = "No hay ofertas en el sistema como para poder ser ordeandas, debe esperar a que un trabajador postule algún trabajo";
        const string nameCommand = "/searchsortedworkoffer";
        const string criterion = "rates";

        // Ahora toca ejecutar el comando de buscar ordenadamente, revisamos que entre en el cada paso
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, nameCommand);
        Assert.AreEqual(responseStart, responseCommand);

        // Ahora ingresamos el criterio de ubicación, y como no hay ofertas a ordenar tendría que decir algo como que no hay nadad para ordenar
        responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, criterion);
        Assert.AreEqual(responseWhenNotFoundWorkOffers, responseCommand);
    }

    /// <summary>
    /// Se cancela la ejecución del comando cuando le pide ingresar el criterio de ordenación
    /// </summary>
    [Test]
    public void SerachSortedCancelCommand()
    {
        // Creamos el employer que ejecutaría el comando por las dudas
        employerID+=1;
        long employerIDTest = employerID;
        const string name = "Tomas";
        const string lastName = "Aquino";
        const string phone = "099139385";
        const string address = "Estero Bellaco 2771";
        const double latitude = -34.8880311715963;   
        const double longitude = -56.15881920239052;
        Database.Instance.AddEmployer(employerIDTest, name, lastName, phone, address, latitude, longitude);

        
        const string validCriterionsString = "\n->location\n->rates";
        // Respuesta esperada de ejecutar el comando en el primer estado o paso
        const string responseStart = $"Ingrese el criterio para poder ordenar \nCriterios validos: {validCriterionsString}";
        const string nameCommand = "/searchsortedworkoffer";
        const string expectedResponseCancelCommand = $"El comando {nameCommand} se cancelo";

        // Ahora toca ejecutar el comando de buscar ordenadamente, revisamos que entre en el cada paso
        string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, nameCommand);
        Assert.AreEqual(responseStart, responseCommand);

        // Ahora que nos pidio el criterio de ordenación, le ingresamos cancelar y debería de decir que se cancelo
        responseCommand = InputInterfacer.Instance.TranslateToCommand(employerIDTest, "Cancel");
        Assert.AreEqual(responseCommand, expectedResponseCancelCommand);
    }



}