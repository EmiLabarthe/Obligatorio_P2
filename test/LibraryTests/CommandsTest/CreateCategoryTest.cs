using System.Collections.ObjectModel;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe.

namespace Tests
{
    /// <summary>
    /// Clase encargada de probar la funcionalidad de crear una categoría
    /// </summary>
    [TestFixture]
    public class ConstructCategoryTest
    {

        private long workerID = 0;
        private long employerID = 500;
        private long adminID = 1000;
        /// <summary>
        /// User story de crear una category.
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
        public void WorkerTryCreateCategory()
        {
            workerID += 1;
            string commandName = "/createcategory";

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
        public void EmployerTryCreateCategory()
        {
            employerID += 129;
            string commandName = "/createcategory";
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
        /// Prueba que el comando de crear categoría funcione.
        /// </summary>
        [Test]
        public void CreateCategory()
        {
            adminID +=1;
            long expectedUserID = adminID;
            Database.Instance.AddAdmin(adminID);
            const string commandName = "/createcategory";

            // Chequeo de que las respuestas del comando sean las esperadas según los datos enviados.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Usted seleccionó crear una categoría. \nIngrese el nombre de la categoría que desea crear.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Jardinería");
            expectedResponseOfCommand = "La categoría se ha creado con éxito.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando de crear categoría funcione cuando se le ingresan mal los datos.
        /// </summary>
        [Test]
        public void CreateCategoryFailed()
        {
            adminID +=1;
            long expectedUserID = adminID;
            Database.Instance.AddAdmin(adminID);
            const string commandName = "/createcategory";

            // Chequeo de que las respuestas del comando sean las esperadas según los datos enviados.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Usted seleccionó crear una categoría. \nIngrese el nombre de la categoría que desea crear.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Dato vacío.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "    ");
            expectedResponseOfCommand = "Los datos ingresados no pueden ser vacíos o ser solo espacios en blanco. \nPor favor ingrese información o cancele la ejecución del comando ingresando 'cancel'";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Jardinería");
            expectedResponseOfCommand = "La categoría se ha creado con éxito.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Prueba que el comando de crear categoría funcione como se espera cuando se cancela.
        /// </summary>
        [Test]
        public void CreateCategoryCancelled()
        {
            adminID +=1;
            long expectedUserID = adminID;
            Database.Instance.AddAdmin(adminID);
            const string commandName = "/createcategory";

            // Chequeo de que las respuestas del comando sean las esperadas según los datos enviados.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Usted seleccionó crear una categoría. \nIngrese el nombre de la categoría que desea crear.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Cancelamos.
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }
    }
}