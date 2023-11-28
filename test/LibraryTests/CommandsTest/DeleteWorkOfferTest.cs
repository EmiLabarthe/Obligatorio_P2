using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe.

namespace Tests
{   
    /// <summary>
    /// Clase que se encarga de probar el comando eliminar WorkOffer
    /// </summary>
    [TestFixture]
    public class DeleteWorkOfferTest
    {

        long workerID = 0;
        long employerID = 500;
        long adminID = 1000;
        /// <summary>
        /// User story de eliminar work offer.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Limpiamos las listas por las dudas
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
                employerID += userIDBiggest + 1 - 500;
            }
            if (cantAdmins != 0)
            {
                long userIDBiggest = Database.Instance.GetAdmins()[cantAdmins - 1].UserID;
                adminID += userIDBiggest + 1 - 500;
            }
        }

         /// <summary>
        /// Prueba que un worker intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void WorkerTryDeleteWorkOffer()
        {
            workerID += 1;
            string commandName = "/deleteworkoffer";

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
        /// Prueba que un employer intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void EmployerTryDeleteWorkOffer()
        {
            employerID +=2;
            string commandName = "/deleteworkoffer";
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
        /// Prueba que el comando de eliminar una oferta de trabajo responda como corresponde.
        /// </summary>
        [Test]
        public void DeleteWorkOffer()
        {
            
            Database.Instance.ClearWorkOffers();
            Database.Instance.ClearCategories();
            adminID +=1;
            workerID += 1;
            long expectedUserID = adminID;
            const string commandName = "/deleteworkoffer";
            

            // Creamos una oferta de trabajo a eliminar y un trabajador.
            Database.Instance.AddWorker(workerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddAdmin(expectedUserID);
            Database.Instance.AddCategory("Jardinería");
            List<string> categoriesForTest = new List<string>() {"Jardinería"};
            int identify = Database.Instance.UltimateIDWorkOffer +1;
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Usted seleccionó eliminar una oferta de trabajo. \nIngrese el ID de la oferta que desea eliminar.";
            Assert.AreEqual(expectedResponseOfCommand,actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "La oferta de trabajo se ha eliminado con éxito.";
            Assert.AreEqual(expectedResponseOfCommand,actualResponseOfCommand);

            // Además hay que comprobar que le haya llegado la notificación al worker
            string expectedNotification = $"Su oferta {identify} fue eliminada.";
            // Obtenemos el worker 
            Worker workerLostWorkOffer = Database.Instance.SearchWorker(workerID);
            ReadOnlyCollection<Notification> notificationsForWorker = workerLostWorkOffer.GetNotifications();
            // Recorremos las notificaciones
            bool notificationSuccessed = false;
            foreach(Notification not in notificationsForWorker)
            {
                if(not.Message == expectedNotification && not.NotificationReasons == Notification.Reasons.AdminDeleteWorkOffer)
                {
                    notificationSuccessed = true;
                }
            }
            Assert.IsTrue(notificationSuccessed);
        }

        /// <summary>
        /// Prueba que el comando de eliminar una oferta de trabajo responda como corresponde cuando se ingresan mal los datos.
        /// </summary>
        [Test]
        public void DeleteFailedWorkOffer()
        {
            
            Database.Instance.ClearWorkOffers();
            Database.Instance.ClearCategories();
            adminID +=1;
            workerID += 1;
            long expectedUserID = adminID;
            const string commandName = "/deleteworkoffer";
            

            // Creamos una oferta de trabajo a eliminar y un trabajador.
            Database.Instance.AddWorker(workerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddCategory("Jardineria");
            Database.Instance.AddAdmin(expectedUserID);
            List<string> categoriesForTest = new List<string>() {"Jardinería"};
            int identify = Database.Instance.UltimateIDWorkOffer +1;
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Usted seleccionó eliminar una oferta de trabajo. \nIngrese el ID de la oferta que desea eliminar.";
            Assert.AreEqual(expectedResponseOfCommand,actualResponseOfCommand);
            
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "99990");
            expectedResponseOfCommand = "Esa ID no corresponde a ninguna oferta de trabajo";
            Assert.AreEqual(expectedResponseOfCommand,actualResponseOfCommand);
        
            
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "La oferta de trabajo se ha eliminado con éxito.";
            Assert.AreEqual(expectedResponseOfCommand,actualResponseOfCommand);

            // Además hay que comprobar que le haya llegado la notificación al worker
            string expectedNotification = $"Su oferta {identify} fue eliminada.";
            // Obtenemos el worker 
            Worker workerLostWorkOffer = Database.Instance.SearchWorker(workerID);
            ReadOnlyCollection<Notification> notificationsForWorker = workerLostWorkOffer.GetNotifications();
            // Recorremos las notificaciones
            bool notificationSuccessed = false;
            foreach(Notification not in notificationsForWorker)
            {
                if(not.Message == expectedNotification && not.NotificationReasons == Notification.Reasons.AdminDeleteWorkOffer)
                {
                    notificationSuccessed = true;
                }
            }
            Assert.IsTrue(notificationSuccessed);
        }

        /// <summary>
        /// Prueba que el comando de eliminar una oferta de trabajo responda como corresponde cuando se cancela.
        /// </summary>
        [Test]
        public void DeleteWorkOfferCancelled()
        {
            
            Database.Instance.ClearWorkOffers();
            adminID +=1;
            workerID += 1;
            long expectedUserID = adminID;
            const string commandName = "/deleteworkoffer";
            

            // Creamos una oferta de trabajo a eliminar y un trabajador.
            Database.Instance.AddWorker(workerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddAdmin(expectedUserID);
            Database.Instance.AddCategory("Jardineria");
            List<string> categoriesForTest = new List<string>() {"Jardinería"};
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Usted seleccionó eliminar una oferta de trabajo. \nIngrese el ID de la oferta que desea eliminar.";
            Assert.AreEqual(expectedResponseOfCommand,actualResponseOfCommand);
            
            // Cancelamos.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand,actualResponseOfCommand);
        }
    }
}