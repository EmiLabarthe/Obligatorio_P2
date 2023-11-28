using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using System;
using ClassLibrary;

// Clase responsabilidad de Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Clase que se encarga de probar la funcionalidad del comando de mostrar el las notificaciones que un usuario tiene.
    /// </summary>
    [TestFixture]

    public class ShowNotificationsCommandTest
    {

        private long workerID = 0;
        private long employerID = 500;
        private long adminID = 1000;

        /// <summary>
        /// User story de mostrar las notificaciones de los user: employer y worker
        /// </summary>
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
        /// Prueba que un admin intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void AdminTryRateAWorker()
        {
            adminID += 1;
            const string commandName = "/shownotifications";
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }



        /// <summary>
        /// Prueba que el comando devuelva todas las notificaciones que hay que hay.
        /// </summary>
        /// 
        [Test]
        public void ShowNotificationsWorker()
        {
            //Configuración

            //Creamos un worker que va a recibir notificaciones
            workerID += 1;
            const string nameWorker = "Jim";
            const string lastNameWorker = "Morrison";
            const string phoneWorker = "098475636";
            const string addressWorker = "Florida";
            const double latitudeWorker = 62;
            const double longitudeWorker = 34;
            Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);

            //Creamos el employer que va a mandar la notificacion.
            employerID += 3;
            const string nameEmployer = "Robbie";
            const string lastNameEmployer = "Krieger";
            const string phoneEmployer = "098475636";
            const string addressEmployer = "Florida";
            const double latitudeEmployer = 62;
            const double longitudeEmployer = 34;
            Database.Instance.AddEmployer(employerID, nameEmployer, lastNameEmployer, phoneEmployer, addressEmployer, latitudeEmployer, longitudeEmployer);

            //Creamos las notificaciones del worker.

            Worker worker = Database.Instance.SearchWorker(workerID);
            string message1 = "Hola!";
            worker.AddNotification(message1, employerID, Notification.Reasons.EmployerWantContactWorker);
            string message2 = "Te borre la oferta de trabajo por ser muy cara";
            worker.AddNotification(message2, workerID, Notification.Reasons.AdminDeleteWorkOffer);
            string message3 = "right on";
            worker.AddNotification(message3, employerID, Notification.Reasons.EmployerWantContactWorker);
            string message4 = "Tu oferta de empleo fue eliminada por que no la consideramos adecuada";
            worker.AddNotification(message4, workerID, Notification.Reasons.AdminDeleteWorkOffer);

            const string commandName = "/shownotifications";
            // Ejecutamos el comando
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(workerID, commandName);


            //Chequeamos que el mensaje devuelto traiga las notificaciones que le agregamos al worker.
            bool cond1 = responseCommand.Contains($"Asunto: {Notification.Reasons.EmployerWantContactWorker}. Viene de: {employerID}. Mensaje: {message1}.");
            bool cond2 = responseCommand.Contains($"Asunto: {Notification.Reasons.AdminDeleteWorkOffer}. Viene de: {workerID}. Mensaje: {message2}.");
            bool cond3 = responseCommand.Contains($"Asunto: {Notification.Reasons.EmployerWantContactWorker}. Viene de: {employerID}. Mensaje: {message3}.");
            bool cond4 = responseCommand.Contains($"Asunto: {Notification.Reasons.AdminDeleteWorkOffer}. Viene de: {workerID}. Mensaje: {message4}.");
            Assert.True(cond1 && cond2 && cond3 && cond4);
        }

        /// <summary>   
        ///  Se prueba el caso en el que se busca en las notificaciones del 
        /// </summary>
        /// <value></value>
        [Test]
        public void ShowNotificationsWorkerButWorkerNotHaveNotifications()
        {
            //Creamos un worker que va a recibir notificaciones
            workerID += 1;
            const string nameWorker = "Jim";
            const string lastNameWorker = "Morrison";
            const string phoneWorker = "098475636";
            const string addressWorker = "Florida";
            const double latitudeWorker = 62;
            const double longitudeWorker = 34;
            Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);

            const string commandName = "/shownotifications";
            const string expectedResponseWhenNotHaveNotifications = "No tiene notificaciones en su buzón, en este momento";
            // Ejecutamos el comando
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(workerID, commandName);
            Assert.AreEqual(responseCommand, expectedResponseWhenNotHaveNotifications);
        }

        /// <summary>
        /// Prueba que el comando devuelva todas las notificaciones que tiene
        /// </summary>
        /// 
        [Test]
        public void ShowNotificationsEmployer()
        {
            //Configuración

            // Creamos un worker que va a "responder" el contacto del employer, 
            // crandole una notificación al employer. En realidad no se va a dar un contacto, 
            // sino que se va a simular que le respondió. 
            workerID += 1;
            const string nameWorker = "Jim";
            const string lastNameWorker = "Morrison";
            const string phoneWorker = "098475636";
            const string addressWorker = "Florida";
            const double latitudeWorker = 62;
            const double longitudeWorker = 34;
            Database.Instance.AddWorker(workerID, nameWorker, lastNameWorker, phoneWorker, addressWorker, latitudeWorker, longitudeWorker);

            //Creamos el employer que va a mandar la notificacion.
            employerID += 2;
            const string nameEmployer = "Robbie";
            const string lastNameEmployer = "Krieger";
            const string phoneEmployer = "098475636";
            const string addressEmployer = "Florida";
            const double latitudeEmployer = 62;
            const double longitudeEmployer = 34;
            Database.Instance.AddEmployer(employerID, nameEmployer, lastNameEmployer, phoneEmployer, addressEmployer, latitudeEmployer, longitudeEmployer);

            //Creamos las notificaciones del worker.

            Employer empoloyer = Database.Instance.SearchEmployer(employerID);
            string message1 = "No gracias, no quiero trabajar para ti";
            empoloyer.AddNotification(message1, workerID, Notification.Reasons.WorkerResponseAnEmployer);
            string message2 = "Si claro, me interesa que me contactes mi número es xxx xxx xxx";
            empoloyer.AddNotification(message2, workerID, Notification.Reasons.WorkerResponseAnEmployer);
            string message3 = "Disculpa, pero ahora no puedo atender llamados";
            empoloyer.AddNotification(message3, workerID, Notification.Reasons.WorkerResponseAnEmployer);
            string message4 = "Gracias por interesarte en mi oferta, puedes contactarme a xxx xxx xxx";
            empoloyer.AddNotification(message4, workerID, Notification.Reasons.WorkerResponseAnEmployer);


            const string commandName = "/shownotifications";

            // Ejecutamos el comando
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);


            //Chequeamos que el mensaje devuelto traiga las notificaciones que le agregamos al worker.
            //Chequeamos que el mensaje devuelto traiga las notificaciones que le agregamos al worker.
            bool cond1 = responseCommand.Contains($"Asunto: {Notification.Reasons.WorkerResponseAnEmployer}. Viene de: {workerID}. Mensaje: {message1}.");
            bool cond2 = responseCommand.Contains($"Asunto: {Notification.Reasons.WorkerResponseAnEmployer}. Viene de: {workerID}. Mensaje: {message2}.");
            bool cond3 = responseCommand.Contains($"Asunto: {Notification.Reasons.WorkerResponseAnEmployer}. Viene de: {workerID}. Mensaje: {message3}.");
            bool cond4 = responseCommand.Contains($"Asunto: {Notification.Reasons.WorkerResponseAnEmployer}. Viene de: {workerID}. Mensaje: {message4}.");
            Assert.True(cond1 && cond2 && cond3 && cond4);
        }

        /// <summary>   
        /// Se prueba el caso en el que se busca en las notificaciones del employer, pero 
        /// este no tiene notificaciones
        /// </summary>
        /// <value></value>
        [Test]
        public void ShowNotificationsEmployerButEmployerNotHaveNotifications()
        {
            //Creamos un employer que va a recibir notificaciones
            employerID += 1;
            const string nameEmployer = "Jim";
            const string lastNameEmployer = "Morrison";
            const string phoneEmployer = "098475636";
            const string addressEmployer = "Florida";
            const double latitudeEmployer = 62;
            const double longitudeEmployer = 34;
            Database.Instance.AddEmployer(employerID, nameEmployer, lastNameEmployer, phoneEmployer, addressEmployer, latitudeEmployer, longitudeEmployer);

            const string commandName = "/shownotifications";
            const string expectedResponseWhenNotHaveNotifications = "No tiene notificaciones en su buzón, en este momento";
            // Ejecutamos el comando
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(employerID, commandName);
            Assert.AreEqual(responseCommand, expectedResponseWhenNotHaveNotifications);
        }
    }
}