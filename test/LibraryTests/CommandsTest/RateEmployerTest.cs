using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe.

namespace Tests
{
    /// <summary>
    /// Test del comando de calificar un empleador.
    /// </summary>
    [TestFixture]
    public class RateEmployerTest
    {
        const string commandName = "/rateemployer";
        long workerID = 0;
        long employerID = 500;
        long adminID = 1000;
        /// <summary>
        /// User story de calificar a un empleador.
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
        /// Prueba que un employer intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void EmployerTryRateEmployer()
        {
            employerID += 12;
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
        public void AdminTryRateEmployer()
        {
            adminID += 12;
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

        /// <summary>
        /// Prueba que el comando RateEmployer se ejecute correctamente
        /// </summary>
        [Test]
        public void RateEmployer()
        {
            // Probamos a calificar un empleador correctamente.
            workerID +=1;
            employerID +=1;
            long expectedUserID = workerID;

            // Creamos una oferta de trabajo y un empleador a calificar.
            Database.Instance.AddWorker(workerID, "pep", "guardiola", "099999999", "Bali", 17, 17);
            Database.Instance.AddEmployer(employerID, "jose", "marquez", "099999999", "Young", 17, 17);
            Database.Instance.AddCategory("Jardinería");
            List<string> categoriesForTest = new List<string>() { "Jardineria" };
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un empleador. \nIngrese el ID del usuario a calificar.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
            
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, employerID.ToString());
            expectedResponseOfCommand = "Ingrese el ID de la oferta de trabajo que lo vincula.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "Ingrese una calificación del 1 al 10 para el empleador.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "10");
            expectedResponseOfCommand = "Calificación realizada.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando RateWorker se ejecute correctamente con datos erróneos
        /// </summary>
        [Test]
        public void FailedRateEmployer()
        {
            
            // Probamos a calificar un empleador con datos que no sirvan.
            workerID +=1;
            employerID +=1;
            long expectedUserID = workerID;

            // Creamos una oferta de trabajo y un empleador a calificar.
            Database.Instance.AddEmployer(employerID, "pepe", "sanchez", "099999999", "Manantiales", 13, 13);
            Database.Instance.AddCategory("Jardineria");
            Database.Instance.AddWorker(workerID, "pep", "guardiola", "099999999", "Bali", 17, 17);
            List<string> categoriesForTest = new List<string>() { "Jardinería" };
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un empleador. \nIngrese el ID del usuario a calificar.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Mal dato cuando pide ID del empleador.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Perro");
            expectedResponseOfCommand = "Esa ID no corresponde a ningún empleador.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, employerID.ToString());
            expectedResponseOfCommand = "Ingrese el ID de la oferta de trabajo que lo vincula.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Malos datos cuando pide la oferta que vincula.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "kobe bryant");
            expectedResponseOfCommand = "El valor debe ser un número.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "-40");
            expectedResponseOfCommand = "Esa ID no corresponde a ninguna oferta de trabajo.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "Ingrese una calificación del 1 al 10 para el empleador.";
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
        /// Prueba que el comando RateEmployer se ejecute correctamente y se cancele luego de ingresar el comando
        /// </summary>
        [Test]
        public void RateEmployerCancelledInSecondStep()
        {
          
            // Probamos a calificar un empleador correctamente.
            workerID +=1;
            employerID +=1;
            long expectedUserID = workerID;

            // Creamos una oferta de trabajo y un empleador a calificar.
            Database.Instance.AddWorker(workerID, "pep", "guardiola", "099999999", "Bali", 17, 17);
            Database.Instance.AddEmployer(employerID, "jose", "marquez", "099999999", "Young", 17, 17);
            Database.Instance.AddCategory("Jardinería");
            List<string> categoriesForTest = new List<string>() { "Jardineria" };
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un empleador. \nIngrese el ID del usuario a calificar.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Mal dato cuando pide ID de la oferta de trabajo.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando RateEmployer se ejecute correctamente y se cancele en el tercer paso, luego de pedir el ID de la oferta de trabajo.
        /// </summary>
        [Test]
        public void RateEmployerCancelledInThirdStep()
        {
            // Probamos a calificar un empleador correctamente.
            workerID +=1;
            employerID +=1;
            long expectedUserID = workerID;

            // Creamos una oferta de trabajo y un empleador a calificar.
            Database.Instance.AddWorker(workerID, "pep", "guardiola", "099999999", "Bali", 17, 17);
            Database.Instance.AddEmployer(employerID, "jose", "marquez", "099999999", "Young", 17, 17);
            Database.Instance.AddCategory("Jardinería");
            List<string> categoriesForTest = new List<string>() { "Jardineria" };
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un empleador. \nIngrese el ID del usuario a calificar.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, employerID.ToString());
            expectedResponseOfCommand = "Ingrese el ID de la oferta de trabajo que lo vincula.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando RateEmployer se ejecute correctamente y se cancele en el cuarto paso
        /// </summary>
        [Test]
        public void RateEmployerCancelledInFourthStep()
        {
            // Configuración
            workerID +=1;
            employerID +=1;
            long expectedUserID = workerID;

            // Creamos una oferta de trabajo y un empleador a calificar.
            Database.Instance.AddWorker(workerID, "pep", "guardiola", "099999999", "Bali", 17, 17);
            Database.Instance.AddEmployer(employerID, "jose", "marquez", "099999999", "Young", 17, 17);
            Database.Instance.AddCategory("Jardinería");
            List<string> categoriesForTest = new List<string>() { "Jardineria" };
            Database.Instance.AddWorkOffer("cortapasto", "UYU", 200, workerID, categoriesForTest, 4);

            // Test.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ejecutaste el comando para calificar a un empleador. \nIngrese el ID del usuario a calificar.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
            
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, employerID.ToString());
            expectedResponseOfCommand = "Ingrese el ID de la oferta de trabajo que lo vincula.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, Database.Instance.UltimateIDWorkOffer.ToString());
            expectedResponseOfCommand = "Ingrese una calificación del 1 al 10 para el empleador.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }
    }
}