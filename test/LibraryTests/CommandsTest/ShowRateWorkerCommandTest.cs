using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using System;
using ClassLibrary;

// Clase responsabilidad de Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Clase que se encarga de probar la funcionalidad del comando de mostrar el rating de un worker.
    /// </summary>
    [TestFixture]

    public class ShowRateWorkerCommandTest
    {
        private long employerID = 500;
        private long workerID = 0;

        
        /// <summary>
        /// User story de crear worker.
        /// </summary>
        /// 
        [SetUp]
        public void Setup()
        {
            Database.Instance.ClearWorkOffers();
            Database.Instance.ClearEmployers();
            Database.Instance.ClearWorkers();
            int cantWorkers = Database.Instance.GetWorkers().Count;
            int cantEmployers = Database.Instance.GetEmployers().Count;
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


            //Creamos el worker que va a calificar a nuestro worker.
            employerID += 1;
            const string nameEmployer = "Junior";
            const string lastNameEmployer = "Gong";
            const string phoneEmployer = "094482563";
            const string addressEmployer = "South Carolina";
            const double latitudeEmployer = 5;
            const double longitudeEmployer = 4;
            Database.Instance.AddEmployer(employerID, nameEmployer, lastNameEmployer, phoneEmployer, addressEmployer, latitudeEmployer, longitudeEmployer);
            
            
        }

        /// <summary>
        /// Prueba que un worker intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void WorkerTryRateAWorker()
        {
            const string commandName = "/showworkerrating";
            workerID += 1;
            // Datos del employer
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
        /// Se prueba que devuelva el rate del worker correctamente
        /// </summary>
        [Test]
        public void ShowRateWorker()
        {
            //Configuración
            //Creamos un worker que vamos a querer ver el rating más adelante.
            workerID += 1;
            const string nameWorker = "Bruce";
            const string lastNameWorker = "Atrey";
            const string phoneWorker = "092432536";
            const string addressWorker = "Kingston";
            const double latitudeWorker = 25;
            const double longitudeWorker = 3;
            Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);
            Worker worker = Database.Instance.SearchWorker(workerID);

            
            //Creamos la work offer que los vincula.
            ReadOnlyCollection<string> catogories = Database.Instance.GetAllCategories();
            Database.Instance.AddCategory("FONTANERIA");

            const string descriptionWorkOffer = "Un trabajo de fontanería, para hogares";
            const string currencyWorkOffer = "UYU";
            const int price = 1500;
            long ownerID = workerID;
            const string category1 = "FONTANERIA";
            List<string> categoriesInput = new List<string>() {category1};
            const int durationInDays = 3;
            int workOfferID = Database.Instance.UltimateIDWorkOffer + 1; 
            Database.Instance.AddWorkOffer(descriptionWorkOffer, currencyWorkOffer, price, ownerID, categoriesInput, durationInDays);

            // Creamos otra oferta de trabajo que le adjudicamos de dueño a ese worker, pero le cambiamos la descripción
            const string description2 = "Arreglo caños en alturas, especialmente edificios de 10 pisos. Primer caso";
            int secondWorkOfferID = Database.Instance.UltimateIDWorkOffer +1;
            Database.Instance.AddWorkOffer(description2, currencyWorkOffer, price, ownerID, categoriesInput, durationInDays);

            
            // Calificación esperada
            double expectedCalification = 6;
            worker.AddRating(employerID,durationInDays,workOfferID);
            worker.AddRating(employerID,durationInDays,secondWorkOfferID);
            worker.ReciveCalification(7, employerID, workOfferID, true);
            worker.ReciveCalification(6, employerID, secondWorkOfferID, true);

            // Nombre del comando
            const string commandName = "/showworkerrating";
            // Mensaje que se supone que devuelve cuando arranca el comando
            const string messageStartCommand = $"Esta ejecutando el comando de mirar la calificación de un trabajador. Ingrese el id del worker del que quiere consultar su calificación";
            
            // Comportamiento
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
            Assert.AreEqual(messageStartCommand, responseCommand);

            // Ahora ingresamos el id del worker
            responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, workerID.ToString());

            // Comprobación, ahora revisamos que devuelva lo esperado
            string expectedMessage = $"La calificación del trabajador es: {expectedCalification} en un total de 10 puntos";
            Assert.AreEqual(expectedMessage, responseCommand);
        }


        /// <summary>
        /// La versión de uso del comando en el cual el usuario comete errores en el paso de ingrear el id del worker
        /// pero finalmente logra hacerlo bien y ver la calificación.
        /// </summary>
        [Test]
        public void ShowRateWorkerWithProblems()
        {
            //Configuración         
            //Creamos un worker que vamos a querer ver el rating más adelante.
            workerID += 1;
            const string nameWorker = "Bruce";
            const string lastNameWorker = "Atrey";
            const string phoneWorker = "092432536";
            const string addressWorker = "Kingston";
            const double latitudeWorker = 25;
            const double longitudeWorker = 3;
            Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);   
            Worker worker = Database.Instance.SearchWorker(workerID);
            
            //Creamos la work offer que los vincula.
            ReadOnlyCollection<string> catogories = Database.Instance.GetAllCategories();
            Database.Instance.AddCategory("FONTANERIA");

            const string descriptionWorkOffer = "Segundo trabajo de fontanería";
            const string currencyWorkOffer = "UYU";
            const int price = 1500;
            long ownerID = workerID;
            const string category1 = "FONTANERIA";
            List<string> categoriesInput = new List<string>() {category1};
            const int durationInDays = 3;
            int workOfferID = Database.Instance.UltimateIDWorkOffer + 1; 
            Database.Instance.AddWorkOffer(descriptionWorkOffer, currencyWorkOffer, price, ownerID, categoriesInput, durationInDays);

            // Creamos otra oferta de trabajo que le adjudicamos de dueño a ese worker, pero le cambiamos la descripción
            const string description2 = "Arreglo caños en alturas, especialmente edificios de 10 pisos. Segundo caso";
            int secondWorkOfferID = Database.Instance.UltimateIDWorkOffer +1;
            Database.Instance.AddWorkOffer(description2, currencyWorkOffer, price, ownerID, categoriesInput, durationInDays);

            
            // Calificación esperada
            double expectedCalification = 6;
            worker.AddRating(employerID,durationInDays,workOfferID);
            worker.AddRating(employerID,durationInDays,secondWorkOfferID);
            worker.ReciveCalification(7, employerID, workOfferID, true);
            worker.ReciveCalification(6, employerID, secondWorkOfferID, true);

            // Nombre del comando
            const string commandName = "/showworkerrating";
            // Mensaje que se supone que devuelve cuando arranca el comando
            const string messageStartCommand = $"Esta ejecutando el comando de mirar la calificación de un trabajador. Ingrese el id del worker del que quiere consultar su calificación";
            // Mensaje que debe devolver cuando ingresa un dato que no es numerico
            const string responseWhenDataNotIsLong = "El formato no es valido, ingrese solo números como identificador del worker, por favor.";
            // Mensaje que devuelve cuando no encuentra el id en la lista de employers
            const string responseNotExistEmployer = "No se ha encontrado un worker con ese id.";
            long notWorker = employerID;

            // Comportamiento
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
            Assert.AreEqual(messageStartCommand, responseCommand);

            // Ingresamos un dato que no es numérico
            responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, "No es un número");
            Assert.AreEqual(responseWhenDataNotIsLong, responseCommand);

            // Ahora ingresamos un id de alguien que no existe
            responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, notWorker.ToString());
            Assert.AreEqual(responseNotExistEmployer, responseCommand);

            // Ahora ingresamos el id del worker
            responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, workerID.ToString());
            // Comprobación, ahora revisamos que devuelva lo esperado
            string expectedMessage = $"La calificación del trabajador es: {expectedCalification} en un total de 10 puntos";
            Assert.AreEqual(expectedMessage, responseCommand);

        }

        /// <summary>
        /// Se encarga de probar que devuelva un mensaje como que el worker nunca fue calificado
        /// </summary>
        [Test]
        public void ShowRateButWorkerNeverWasCalificated()
        {
            //Creamos un worker que vamos a querer ver el rating más adelante.
            workerID += 1;
            const string nameWorker = "Bruce";
            const string lastNameWorker = "Atrey";
            const string phoneWorker = "092432536";
            const string addressWorker = "Kingston";
            const double latitudeWorker = 25;
            const double longitudeWorker = 3;
            Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);

            // Nombre del comando
            const string commandName = "/showworkerrating";
            // Respuestas
            // Mensaje que se supone que devuelve cuando arranca el comando
            const string messageStartCommand = $"Esta ejecutando el comando de mirar la calificación de un trabajador. Ingrese el id del worker del que quiere consultar su calificación";
            // Mensaje que debe devolver cuando ingresa un dato que no es numerico
            const string responseWhenEmployerNoCalificated = "Este worker no ha sido calificado aún.";

            // Comportamiento
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
            Assert.AreEqual(messageStartCommand, responseCommand);

            // Ahora ingresamos el id del worker que no ha sido calificado
            responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, workerID.ToString());
            Assert.AreEqual(responseWhenEmployerNoCalificated, responseCommand);
        }

        /// <summary>
        /// Se encarga de probar que se cancele cuando se le da cancel en el único paso que tiene
        /// </summary>
        [Test]
        public void ShowRateCancel()
        {
            //Creamos un worker que vamos a querer ver el rating más adelante.
            workerID += 1;
            const string nameWorker = "Bruce";
            const string lastNameWorker = "Atrey";
            const string phoneWorker = "092432536";
            const string addressWorker = "Kingston";
            const double latitudeWorker = 25;
            const double longitudeWorker = 3;
            Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);

            // Nombre del comando
            const string commandName = "/showworkerrating";
            // Respuestas
            // Mensaje que se supone que devuelve cuando arranca el comando
            const string messageStartCommand = $"Esta ejecutando el comando de mirar la calificación de un trabajador. Ingrese el id del worker del que quiere consultar su calificación";
            // Mensaje que debe devolver cuando ingresa un dato que no es numerico
            const string responseCancelCommand = $"El comando /showworkerrating se canceló";

            // Entrada para cancelar
            const string cancelInput = "cancel";

            // Comportamiento
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
            Assert.AreEqual(messageStartCommand, responseCommand);

            // Ahora ingresamos el id del worker que no ha sido calificado
            responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, cancelInput);
            Assert.AreEqual(responseCancelCommand, responseCommand);
        }
        

    }
}