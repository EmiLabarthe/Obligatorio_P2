using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System;
using System.Runtime;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe.

namespace Tests
{
    /// <summary>
    /// Test del comando de calificar un trabajador.
    /// </summary>
    [TestFixture]
    public class RateWorkerTest
    {
        const string commandName = "/rateworker";

        long workerID = 0;
        long employerID = 500;
        long adminID = 1000;
        
        /// <summary>
        /// User story de calificar un trabajador.
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
        public void WorkerTryRateAWorker()
        {
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
        /// Prueba que un admin intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void AdminTryRateAWorker()
        {
            adminID += 1;
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

        /// <summary>
        /// Prueba que el comando RateWorker se ejecute correctamente
        /// </summary>
        [Test]
        public void RateWorker()
        {
            
            // Probamos a calificar un empleador correctamente.
            workerID += 1;
            employerID +=1;
            long expectedUserID = employerID;
            

            // Creamos una oferta de trabajo y un trabajador a calificar.
            Database.Instance.AddWorker(workerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddEmployer(expectedUserID, "Jose", "Lizo", "099999999", "Manantiales", 13, 13);

            Database.Instance.AddCategory("Jardineria");
            List<string> categoriesForTest = new List<string>() {"Jardinería"};
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un trabajador. \nIngrese el ID de la oferta de trabajo del calificado";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "Ingrese la calificación del 1 al 10.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "10");
            expectedResponseOfCommand = "Calificación realizada.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando RateWorker se ejecute correctamente con datos erróneos
        /// </summary>
        [Test]
        public void FailedRateWorker()
        {
            
            // Probamos a calificar un empleador con datos que no sirvan.
            workerID += 1;
            employerID +=1;
            long expectedUserID = employerID;
            

            // Creamos una oferta de trabajo y un trabajador a calificar.
            Database.Instance.AddWorker(workerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddEmployer(expectedUserID, "Jose", "Lizo", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddCategory("Jardineria");
            List<string> categoriesForTest = new List<string>() {"Jardinería"};
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un trabajador. \nIngrese el ID de la oferta de trabajo del calificado";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Mal dato cuando pide ID de la oferta de trabajo.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Perro");
            expectedResponseOfCommand = "El valor debe ser un número.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "Ingrese la calificación del 1 al 10.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Dato que falle al calificar
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "100000");
            expectedResponseOfCommand = "Debe ingresar un número del 1 al 10.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "10");
            expectedResponseOfCommand = "Calificación realizada.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando RateWorker se ejecute correctamente y se cancele luego de ingresar el comando
        /// </summary>
        [Test]
        public void RateWorkerCancelledInSecondStep()
        {          
            workerID += 1;
            employerID +=1;
            long expectedUserID = employerID;

            // Creamos una oferta de trabajo y un trabajador a calificar.
            Database.Instance.AddWorker(workerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddEmployer(expectedUserID, "Jose", "Listorti", "099999999", "Manantiales", 13, 13);

            Database.Instance.AddCategory("Jardineria");
            List<string> categoriesForTest = new List<string>() {"Jardinería"};
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un trabajador. \nIngrese el ID de la oferta de trabajo del calificado";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Mal dato cuando pide ID de la oferta de trabajo.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando RateWorker se ejecute correctamente y se cancele en el tercer paso, luego de pedir el ID de la oferta de trabajo.
        /// </summary>
        [Test]
        public void RateWorkerCancelledInThirdStep()
        {
            workerID += 1;
            employerID +=1;
            long expectedUserID = employerID;

            // Creamos una oferta de trabajo y un trabajador a calificar.
            Database.Instance.AddWorker(workerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddEmployer(expectedUserID, "Jose", "Listorti", "093999999", "Manantiales", 13, 13);

            Database.Instance.AddCategory("Jardineria");
            List<string> categoriesForTest = new List<string>() {"Jardinería"};
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un trabajador. \nIngrese el ID de la oferta de trabajo del calificado";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "Ingrese la calificación del 1 al 10.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }
    }
}