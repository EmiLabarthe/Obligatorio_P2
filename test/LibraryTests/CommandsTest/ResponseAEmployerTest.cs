using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using NUnit.Framework;
using ClassLibrary;

// Clase responsabilidad de Agustín Toya.

namespace Tests;
    
    /// <summary>
    /// Test en el que probamos el comando de responder a un intento de contacto de un employer a un worker, siendo el worker el que 
    /// hace uso del comando. 
    /// </summary>
    [TestFixture]
    public class ResponseAnEmployerTest
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
                employerID += userIDBiggest + 1 +500;
            }
            if (cantAdmins != 0)
            {
                long userIDBiggest = Database.Instance.GetAdmins()[cantAdmins - 1].UserID;
                adminID += userIDBiggest + 1 - 1000;
            }
          
            Database.Instance.AddCategory(categoryName);
        }

          /// <summary>
        /// Prueba que un employer intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void EmployerTryResponseAEmployer()
        {
            employerID += 1;
            string commandName = "/responseaemployer";
            // Datos del employer
            const string name = "José";
            const string lastName = "Rums";
            const string phone = "095180399";
            const string address = "Cordón";
            const double latitude = -34.99659313;
            const double longitude = -50.5730071;
            
            // Agrego el employer
            Database.Instance.AddEmployer(employerID, name, lastName, phone, address, latitude, longitude);

            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

        /// <summary>
        /// Prueba que un admin intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void AdminTryResponseAEmployer()
        {
            adminID += 1;
            string commandName = "/responseaemployer";
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }


        /// <summary>
        /// Caso en el que se ingresa toda la información bien en la primera instancia, además de que 
        /// ingresa yes, send, queriendo que el bot le pase su info de contacto al employers
        /// </summary>
        [Test]
        public void ResponseAnEmployerSuccessedResponseYes()
        {
            // Configuración
            //Creamos a un worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;

            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long employerContactAWorker = employerID;
            string commandName = "/responseaemployer";
            long userID = workerOwnerID;

            List<string> catogories = new List<string>() { "Traslados" };
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(employerContactAWorker, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);
            // Ahora queda que el employer intente contactarse con el worker para así poder hacer una respuesta
            Worker workerReciveNotification = Database.Instance.SearchWorker(workerOwnerID);
            string message = $"Quiero contactarme con usted para contrarle por su oferta de {description}";
            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;
            
            // Agregamos la notificación
            workerReciveNotification.AddNotification(message, employerContactAWorker, Notification.Reasons.EmployerWantContactWorker);

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando responder a un empleador. Ahora ingrese el id de la notificación del contacto, para poder responder";
           
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedConfirmOrCancelContacResponse = "Ingrese 'Yes, send' para que le enviemos a este employer su información de contacto o 'No, send' en caso contrario";
        
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Ya se le envio su información de contacto para que el employer se contacte con usted";

            string expectedMessageNotification = $"El worker {name} {lastName} de identificador {workerOwnerID} ha aceptado ser contactado. Su información de contacto es {phone}";
            // Fin de respuestas de cuando todo esta bien

            //Comportamiento
            // Ejecutamos comportamiento y comprobaciones
            // Va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);

            // Le ingresamos el identificador de la notificación
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, expectedNotificationID.ToString());
            // Ahora comprobamos que devuleva lo que corresponda a la hora de ingresar el id de la notificación
            Assert.AreEqual(expectedConfirmOrCancelContacResponse, responseCommand);

            // Se envía yes, send, y se comprueba que devuelva lo necesario
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "Yes, send");
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);


            bool contactSucssed = false;
            Employer employerCreated = Database.Instance.SearchEmployer(employerContactAWorker);

            expectedNotificationID = employerCreated.UltimateIDNotification;
            foreach (Notification not in employerCreated.GetNotifications())
            {
                if (not.Message == expectedMessageNotification && not.SenderID== workerOwnerID && expectedNotificationID == not.NotificationID)
                {
                    contactSucssed = true;
                }
            }
            Assert.IsTrue(contactSucssed);
        }

        /// <summary>
        /// Caso en el que se ingresa toda la información bien en la primera instancia, además de que 
        /// ingresa no, send, impidiendo que el bot le envíe su info de contaco al employer
        /// </summary>
        [Test]
        public void ResponseAnEmployerSuccessedResponseNo()
        {
            // Configuración
            //Creamos a un worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;

            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long employerContactAWorker = employerID;
            string commandName = "/responseaemployer";
            long userID = workerOwnerID;

            List<string> catogories = new List<string>() { "Traslados" };
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(employerContactAWorker, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);
            // Ahora queda que el employer intente contactarse con el worker para así poder hacer una respuesta
            Worker workerReciveNotification = Database.Instance.SearchWorker(workerOwnerID);
            string message = $"Quiero contactarme con usted para contrarle por su oferta de {description}";
            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;
            
            // Agregamos la notificación
            workerReciveNotification.AddNotification(message, employerContactAWorker, Notification.Reasons.EmployerWantContactWorker);

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando responder a un empleador. Ahora ingrese el id de la notificación del contacto, para poder responder";
           
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedConfirmOrCancelContacResponse = "Ingrese 'Yes, send' para que le enviemos a este employer su información de contacto o 'No, send' en caso contrario";
        
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Ya se le envió la notifiación al employer de que no quiere ser contactado por él para hablar sobre la oferta en cuestión";

            string expectedMessageNotification = $"El worker {name} {lastName} de identificador {workerOwnerID} no ha aceptado ser contactado. Lo sentimos mucho";
            // Fin de respuestas de cuando todo esta bien

            //Comportamiento
            // Ejecutamos comportamiento y comprobaciones
            // Va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);

            // Le ingresamos el identificador de la notificación
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, expectedNotificationID.ToString());
            // Ahora comprobamos que devuleva lo que corresponda a la hora de ingresar el id de la notificación
            Assert.AreEqual(expectedConfirmOrCancelContacResponse, responseCommand);

            // Se envía yes, send, y se comprueba que devuelva lo necesario
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "No, send");
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);


            bool contactSucssed = false;
            Employer employerCreated = Database.Instance.SearchEmployer(employerContactAWorker);

            expectedNotificationID = employerCreated.UltimateIDNotification;
            foreach (Notification not in employerCreated.GetNotifications())
            {
                if (not.Message == expectedMessageNotification && not.SenderID== workerOwnerID && expectedNotificationID == not.NotificationID)
                {
                    contactSucssed = true;
                }
            }
            Assert.IsTrue(contactSucssed);
        }

        /// <summary>
        /// Ingresa la información bien, luego mal, luego bien y así hasta que se completa con el caso de 
        /// respuesta Yes, send
        /// </summary>
        [Test]
        public void ResponseAnEmployerSuccessedWithProblemsResponseYes()
        {
            // Configuración
            //Creamos a un worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;
            adminID+=1;

            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long employerContactAWorker = employerID;
            string commandName = "/responseaemployer";
            long userID = workerOwnerID;
            long adminSimuleteDeleteOffer = adminID;

            List<string> catogories = new List<string>() {"Traslados"};
            // Agrego el admin 
            Database.Instance.AddAdmin(adminID);
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(employerContactAWorker, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);
            // Ahora queda que el employer intente contactarse con el worker para así poder hacer una respuesta
            Worker workerReciveNotification = Database.Instance.SearchWorker(workerOwnerID);
            string message = $"Quiero contactarme con usted para contrarle por su oferta de {description}";
            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;
            int strangerNotificationID  = -1212;
            
            // Agregamos la notificación
            workerReciveNotification.AddNotification(message, employerContactAWorker, Notification.Reasons.EmployerWantContactWorker);
            int notificationIDDeleteWorOffer = workerCreated.UltimateIDNotification + 1;
            workerReciveNotification.AddNotification("Borre tu oferta de trabajo por ser inadecuada", employerContactAWorker, Notification.Reasons.AdminDeleteWorkOffer);

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando responder a un empleador. Ahora ingrese el id de la notificación del contacto, para poder responder";
           
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedConfirmOrCancelContacResponse = "Ingrese 'Yes, send' para que le enviemos a este employer su información de contacto o 'No, send' en caso contrario";
        
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Ya se le envio su información de contacto para que el employer se contacte con usted";

            string expectedMessageNotification = $"El worker {name} {lastName} de identificador {workerOwnerID} ha aceptado ser contactado. Su información de contacto es {phone}";
            // Fin de respuestas de cuando todo esta bien

            // Respuestas esperadas cuando se introducen datos 
            // Ingresa un id de una notificación que no es entero
            const string expectedNotificationIDNotInt = "Ingrese números enteros para la próxima ocasión, por favor"; 
            // Ingresa un id de una notificación que no corresponde a una notificación
            const string expectedNotificationIDStranger = "No se ha encontrado una notificación con ese id";
            // Ingresea una id de una notificación que no corresponde a un notificación de contacto de um empleador
            const string expectedNotificationNotEmployerContact = "La notificación no es de un employer queriendo contactarse";
            // Ingresa una opción que no corresponde a 'Yes, send' o a 'No, send'
            const string expectedDecisionNotValid = "El texto que introdujo no corresponde con ninguna opción valida en este paso, recuerde: 'yes send' para enviar su información de contacto, 'no send' en caso contrario";
            // Fin de respuestas esperadas cuando se introducen datos erroneos



            //Comportamiento
            // Ejecutamos comportamiento y comprobaciones
            // Va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);

            // Ingresamos un identificador que no es un entero
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "¿Funcionará?");
            // Comprobamos que responda que se debe ingresar un entero
            Assert.AreEqual(expectedNotificationIDNotInt, responseCommand);

            // Ingresamos un identificador que no corresponde a una notificación de su lista
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, strangerNotificationID.ToString());
            // Comprobamos que responda que la notificación no corresponde al contacto de un employer
            Assert.AreEqual(expectedNotificationIDStranger, responseCommand);

            // Ingresamos un identificador que no corresponde a una notificación de contacto de un employer
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, notificationIDDeleteWorOffer.ToString());
            // Comprobamos que responda que la notificación no corresponde al contacto de un employer
            Assert.AreEqual(expectedNotificationNotEmployerContact, responseCommand);

            // Le ingresamos el identificador de la notificación correcto, para hacerlo pasar al paso siguiente
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, expectedNotificationID.ToString());
            // Ahora comprobamos que devuleva lo que corresponda a la hora de ingresar el id de la notificación
            Assert.AreEqual(expectedConfirmOrCancelContacResponse, responseCommand);

            // Se cualquier cosa diferente de Yes, send
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "Si dale");
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedDecisionNotValid, responseCommand);

            // Se envía yes, send, y se comprueba que devuelva lo necesario
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "Yes, send");
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);

            bool contactSucssed = false;
            Employer employerCreated = Database.Instance.SearchEmployer(employerContactAWorker);

            expectedNotificationID = employerCreated.UltimateIDNotification;
            foreach (Notification not in employerCreated.GetNotifications())
            {
                if (not.Message == expectedMessageNotification && not.SenderID== workerOwnerID && expectedNotificationID == not.NotificationID)
                {
                    contactSucssed = true;
                }
            }
            Assert.IsTrue(contactSucssed);
        }


        /// <summary>
        /// Ingresa la información bien, luego mal, luego bien y así hasta que se completa con el caso de 
        /// respuesta No, send
        /// </summary>
        [Test]
        public void ResponseAnEmployerSuccessedWithProblemsResponseNo()
        {
            // Configuración
            //Creamos a un worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;
            adminID+=1;

            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long employerContactAWorker = employerID;
            string commandName = "/responseaemployer";
            long userID = workerOwnerID;
            long adminSimuleteDeleteOffer = adminID;

            List<string> catogories = new List<string>() {"Traslados"};
            // Agrego el admin 
            Database.Instance.AddAdmin(adminID);
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(employerContactAWorker, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);
            // Ahora queda que el employer intente contactarse con el worker para así poder hacer una respuesta
            Worker workerReciveNotification = Database.Instance.SearchWorker(workerOwnerID);
            string message = $"Quiero contactarme con usted para contrarle por su oferta de {description}";
            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;
            int strangerNotificationID  = -1212;
            
            // Agregamos la notificación
            workerReciveNotification.AddNotification(message, employerContactAWorker, Notification.Reasons.EmployerWantContactWorker);
            int notificationIDDeleteWorOffer = workerCreated.UltimateIDNotification + 1;
            workerReciveNotification.AddNotification("Borre tu oferta de trabajo por ser inadecuada", employerContactAWorker, Notification.Reasons.AdminDeleteWorkOffer);

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando responder a un empleador. Ahora ingrese el id de la notificación del contacto, para poder responder";
           
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedConfirmOrCancelContacResponse = "Ingrese 'Yes, send' para que le enviemos a este employer su información de contacto o 'No, send' en caso contrario";
        
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Ya se le envió la notifiación al employer de que no quiere ser contactado por él para hablar sobre la oferta en cuestión";

            string expectedMessageNotification = $"El worker {name} {lastName} de identificador {workerOwnerID} no ha aceptado ser contactado. Lo sentimos mucho";
            // Fin de respuestas de cuando todo esta bien

            // Respuestas esperadas cuando se introducen datos 
            // Ingresa un id de una notificación que no es entero
            const string expectedNotificationIDNotInt = "Ingrese números enteros para la próxima ocasión, por favor"; 
            // Ingresa un id de una notificación que no corresponde a una notificación
            const string expectedNotificationIDStranger = "No se ha encontrado una notificación con ese id";
            // Ingresea una id de una notificación que no corresponde a un notificación de contacto de um empleador
            const string expectedNotificationNotEmployerContact = "La notificación no es de un employer queriendo contactarse";
            // Ingresa una opción que no corresponde a 'Yes, send' o a 'No, send'
            const string expectedDecisionNotValid = "El texto que introdujo no corresponde con ninguna opción valida en este paso, recuerde: 'yes send' para enviar su información de contacto, 'no send' en caso contrario";
            // Fin de respuestas esperadas cuando se introducen datos erroneos



            //Comportamiento
            // Ejecutamos comportamiento y comprobaciones
            // Va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);

            // Ingresamos un identificador que no es un entero
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "¿Funcionará?");
            // Comprobamos que responda que se debe ingresar un entero
            Assert.AreEqual(expectedNotificationIDNotInt, responseCommand);

            // Ingresamos un identificador que no corresponde a una notificación de su lista
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, strangerNotificationID.ToString());
            // Comprobamos que responda que la notificación no corresponde al contacto de un employer
            Assert.AreEqual(expectedNotificationIDStranger, responseCommand);

            // Ingresamos un identificador que no corresponde a una notificación de contacto de un employer
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, notificationIDDeleteWorOffer.ToString());
            // Comprobamos que responda que la notificación no corresponde al contacto de un employer
            Assert.AreEqual(expectedNotificationNotEmployerContact, responseCommand);

            // Le ingresamos el identificador de la notificación correcto, para hacerlo pasar al paso siguiente
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, expectedNotificationID.ToString());
            // Ahora comprobamos que devuleva lo que corresponda a la hora de ingresar el id de la notificación
            Assert.AreEqual(expectedConfirmOrCancelContacResponse, responseCommand);

            // Se cualquier cosa diferente de Yes, send
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "No gracias, a ese sujeto no");
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedDecisionNotValid, responseCommand);

            // Se envía yes, send, y se comprueba que devuelva lo necesario
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, "No, send");
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);

            bool contactSucssed = false;
            Employer employerCreated = Database.Instance.SearchEmployer(employerContactAWorker);

            expectedNotificationID = employerCreated.UltimateIDNotification;
            foreach (Notification not in employerCreated.GetNotifications())
            {
                if (not.Message == expectedMessageNotification && not.SenderID== workerOwnerID && expectedNotificationID == not.NotificationID)
                {
                    contactSucssed = true;
                }
            }
            Assert.IsTrue(contactSucssed);
        }


        /// <summary>
        /// Ingresamos el nombre del comando, lo usamos y le damos cancelar en el primer paso.
        /// Probamos que devuelve el mensaje esperado confirmando la cancelación. 
        /// </summary>
        [Test]
        public void ResponseAnEmployerCancelInFirstStep()
        {
            // Configuración
            //Creamos a un worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;

            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long employerContactAWorker = employerID;
            string commandName = "/responseaemployer";
            long userID = workerOwnerID;

            List<string> catogories = new List<string>() { "Traslados" };
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(employerContactAWorker, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);
            // Ahora queda que el employer intente contactarse con el worker para así poder hacer una respuesta
            Worker workerReciveNotification = Database.Instance.SearchWorker(workerOwnerID);
            string message = $"Quiero contactarme con usted para contrarle por su oferta de {description}";
            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;

            string expectedCancelResponse = $"El comando {commandName} se canceló";
            // Texto que ingresa para cancelar la ejecución del comando
            const string cancelInput = "cancel";
            
            // Agregamos la notificación
            workerReciveNotification.AddNotification(message, employerContactAWorker, Notification.Reasons.EmployerWantContactWorker);

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando responder a un empleador. Ahora ingrese el id de la notificación del contacto, para poder responder";
           
            // Ejecutamos comportamiento y comprobaciones
            // Va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);

            // Ahora cancelo cuando me pide ingresar el id de la notificación
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);

            
        }

        /// <summary>
        /// Ingresamos el nombre del comando, lo usamos, le ingreasamos una id validad de notificación
        ///  y le damos cancelar en el segundo paso.
        /// Probamos que devuelve el mensaje esperado confirmando la cancelación. 
        /// </summary>
        [Test]
        public void ResponseAnEmployerCancelInSecondtStep()
        {
            // Configuración
            //Creamos a un worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            employerID += 1;

            long workerOwnerID = workerID;
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            long employerContactAWorker = employerID;
            string commandName = "/responseaemployer";
            long userID = workerOwnerID;

            List<string> catogories = new List<string>() { "Traslados" };
            // Agrego el worker
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Agrego el employer
            Database.Instance.AddEmployer(employerContactAWorker, employerName, employerLastName, employerPhone, employeraddress, employerLatitude, employerLongitude);
            // Agregamos la oferta de trabajo
            Database.Instance.AddWorkOffer(description, moneda, price, workerOwnerID, catogories, durationWorkOffer);
            // Ahora queda que el employer intente contactarse con el worker para así poder hacer una respuesta
            Worker workerReciveNotification = Database.Instance.SearchWorker(workerOwnerID);
            string message = $"Quiero contactarme con usted para contrarle por su oferta de {description}";
            // Obtenego al worker que cree recién para preguntar sobre la ultima id de notificación
            Worker workerCreated = Database.Instance.SearchWorker(workerOwnerID);
            // Ahora guardamos la ID que tendría que tener la notificación si es que se creo
            int expectedNotificationID = workerCreated.UltimateIDNotification + 1;
            
            // Texto que ingresa para cancelar la ejecución del comando
            const string cancelInput = "cancel";
            

            // Agregamos la notificación
            workerReciveNotification.AddNotification(message, employerContactAWorker, Notification.Reasons.EmployerWantContactWorker);

            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Esta ejecutando el comando responder a un empleador. Ahora ingrese el id de la notificación del contacto, para poder responder";
            // Respuesta esperada cuando se le da cancel
            string expectedCancelResponse = $"El comando {commandName} se canceló";
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedConfirmOrCancelContacResponse = "Ingrese 'Yes, send' para que le enviemos a este employer su información de contacto o 'No, send' en caso contrario";
        
            // Ejecutamos comportamiento y comprobaciones
            // Va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);

            // Le ingresamos el identificador de la notificación
            responseCommand = InputInterfacer.Instance.TranslateToCommand(userID, expectedNotificationID.ToString());
            // Ahora comprobamos que devuleva lo que corresponda a la hora de ingresar el id de la notificación
            Assert.AreEqual(expectedConfirmOrCancelContacResponse, responseCommand);

            // Ahora cancelo cuando me pide ingresar el id de la notificación
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);

        }
    }