using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using NUnit.Framework;
using ClassLibrary;

// Clase responsabilidad de Agustín Toya.

namespace Tests
{   /// <summary>
    /// Clase que se encarga de probar el comando de /hireemployee
    /// </summary>
    [TestFixture]
    public class ContactWorkerTest
    {
        private long workerID = 0;
        private long employerID = 500;
        private long adminID = 1000;

        // Primero creamos al woker que queremos contactar 
        const string name = "José";
        const string lastName = "Rums";
        const string phone = "095180399";
        const string address = "Cordón";
        const double latitude = -34.99659313;
        const double longitude = -50.5730071;

        // Creamos a un employer para garantizar de que exista
        const string employerName = "Manolo";
        const string employerLastName = "Fierro";
        const string employerPhone = "095180399";
        const string employeraddress = "Cordón";
        const double employerLatitude = -34.99659313;
        const double employerLongitude = -50.5730071;
        //Creamos una oferta de trabajo para que la tenga el worker si o si
        const string categoryName = "Traslados";

        const string description = "Llevar al areopuerto";
        const string moneda = "USD";
        const int price = 50;
        long ownerID = 0;
        const int durationWorkOffer = 20;
        string expectedMessageReciveWorker = $"";



        /// <summary>
        /// User story que simula el contacto de un empleador a un trabajador.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            int cantWorkers = Database.Instance.GetWorkers().Count;
            int cantEmployers = Database.Instance.GetEmployers().Count;
            int cantAdmins = Database.Instance.GetAdmins().Count;
            if (cantWorkers != 0)
            {
                long userIDBiggest = Database.Instance.GetWorkers()[cantWorkers - 1].UserID;
                workerID += userIDBiggest + 1;
                ownerID = workerID;
            }
            if (cantEmployers != 0)
            {
                long userIDBiggest = Database.Instance.GetEmployers()[cantEmployers - 1].UserID;
                employerID += userIDBiggest + 1 - 500;
            }
            if (cantAdmins != 0)
            {
                long userIDBiggest = Database.Instance.GetAdmins()[cantAdmins - 1].UserID;
                adminID += userIDBiggest + 1 - 500;
            }
          
            Database.Instance.AddCategory(categoryName);
        }

        /// <summary>
        /// Prueba que un worker intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void WorkerTryContact()
        {
            workerID += 1;
            string commandName = "/hireemployee";
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
        public void AdminTryContact()
        {
            adminID +=1;
            string commandName = "/hireemployee";
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }


        /// <summary>
        /// Prueba de que al empleado le llegue la notificación cuando el empleador quiere contactarlo
        /// </summary>
        [Test]
        public void ContactSucceeded()
        {
            // Configuración
            //Creamos a un worker
            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;

            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long expectedUserID = employerID;
            string commandName = "/hireemployee";

            List<string> catogories = new List<string>() { "Traslados" };
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(expectedUserID, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            expectedMessageReciveWorker = $"Estimado o estimada, espero tenga un buen día. Me interesa contactarme con usted para poder acordar la contratación de su servicio de: {description} con identificador {expectedWorkOfferID}";
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);


            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando de contactar a un trabajador. Ingrese el identificador de la oferta por la que esta interesado y quiere contactarlo.";
        
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Se ha contactado al worker {name} {lastName}";
            // Fin de respuestas de cuando todo esta bien

            //Comportamiento
            // Ejecutamos comportamiento y comprobaciones
            // va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);

            // Se envía el identificador de la oferta
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, expectedWorkOfferID.ToString());
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);


            bool contactSucssed = false;
            expectedNotificationID = workerCreated.UltimateIDNotification;
            foreach (Notification not in workerCreated.GetNotifications())
            {
                if (not.Message == expectedMessageReciveWorker && not.SenderID== employerID && expectedNotificationID == not.NotificationID)
                {
                    contactSucssed = true;
                }
            }



            Assert.IsTrue(contactSucssed);
        }

        /// <summary>
        /// Prueba de que al empleado le llegue la notificación cuando el empleador quiere contactarlo
        /// </summary>
        [Test]
        public void ContactSucceededWithAProblems()
        {
            // Configuración
            //Creamos a un worker
            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;
            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long expectedUserID = employerID;
            string commandName = "/hireemployee";

            List<string> catogories = new List<string>() { "Traslados" };
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(employerID, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);
           
            // Mensaje que se espera que reciba el worker
            expectedMessageReciveWorker = $"Estimado o estimada, espero tenga un buen día. Me interesa contactarme con usted para poder acordar la contratación de su servicio de: {description} con identificador {expectedWorkOfferID}";


            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando de contactar a un trabajador. Ingrese el identificador de la oferta por la que esta interesado y quiere contactarlo.";
           
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Se ha contactado al worker {name} {lastName}";
            // Fin de respuestas de cuando todo esta bien

            // Respuestas cuando la información esta mal
            // Respuesta que se espera cuando la work offer indicada por id no existe en la lista de work offers
            const string expectedResponseWorkOfferNoExist = "El identificador no es valido, no existe una oferta con ese identificador";
            // Respuesta esperada cuando no se ingresa un número entero como identificador
            const string expectedResponseIdentifyNotInt = "Como identificador de la oferta de trabajo solo ingrese números enteros mayores a 0";


            //Comportamiento
            // Ejecutamos comportamiento y comprobaciones, pero primero hacemos los pasos mal y luego los bien
            // para así comprobar que se den las respuestas que tendrían que darse ante casos de datos incorrectos

            // va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió cuando se ejecuta por primera vez
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Ingresamos mal el ID de la workOffer, por lo cual nos debería de decir que no existe tal oferta, como no existe un -90 
            // por que son mayores a 0, no existirá ninguna así.
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "-90");
            // Comprobamos que de lo esperado
            Assert.AreEqual(expectedResponseWorkOfferNoExist, responseCommand);
            // Ingreamos un identificador que no sea un número, lo cual debería pedirnos que ingresemos solo números, veamos
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Cortar pasto");
            // Comprobamos que de lo esperado
            Assert.AreEqual(expectedResponseIdentifyNotInt, responseCommand);

            //Ahora ingresamos el dato bien
             responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, expectedWorkOfferID.ToString());
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);

            bool contactSucssed = false;
            foreach (Notification not in workerCreated.GetNotifications())
            {
                if (not.Message == expectedMessageReciveWorker && not.SenderID== expectedUserID && expectedNotificationID == not.NotificationID)
                {
                    contactSucssed = true;
                }
            }
            Assert.IsTrue(contactSucssed);
        }

        /// <summary>
        /// Probamos que el comando pueda cancelarse cuando se pide
        /// el identificador de la oferta
        /// </summary>
        [Test]
        public void ConctacWorkerCancel()
        {
            employerID += 1;
            long expectedUserID = employerID;
            Database.Instance.AddEmployer(employerID, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            string commandName = "/hireemployee";

            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Esta ejecutando el comando de contactar a un trabajador. Ingrese el identificador de la oferta por la que esta interesado y quiere contactarlo.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Mal dato cuando pide ID de la oferta de trabajo.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }
    }
} 