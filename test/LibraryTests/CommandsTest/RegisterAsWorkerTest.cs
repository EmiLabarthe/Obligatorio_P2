using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe

namespace Tests
{
    /// <summary>
    /// Test del comando registar un usuario como un worker
    /// </summary>
    [TestFixture]
    public class RegisterAsWorkerTest
    {
        const string commandName = "/registerasworker";
        long workerID = 0;
        long employerID = 500;
        long adminID = 1000;
        int cantWorkers = Database.Instance.GetWorkers().Count;
        int cantEmployers = Database.Instance.GetEmployers().Count;
        int cantAdmins = Database.Instance.GetAdmins().Count;



        /// <summary>
        /// User story de registrarse como worker
        /// </summary>
        public void Setup()
        {
            long workerID = 0;
            long employerID = 500;
            long adminID = 1000;
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
        public void EmployerTrRegisterAsWorker()
        {

            employerID += 1;
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
        /// Prueba que un employer intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void WorkerTrRegisterAsWorker()
        {
            workerID += 12;
            // Datos del employer
            const string name = "José";
            const string lastName = "Rums";
            const string phone = "095180399";
            const string address = "Cordón";
            const double latitude = -34.99659313;
            const double longitude = -50.5730071;

            // Agrego el employer
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
        public void AdminTrRegisterAsWorker()
        {
            adminID += 2;
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
            };
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

        /// <summary>
        /// Prueba que se ejecute correctamente el comando de registrarse como trabajador.
        /// </summary>
        [Test]
        public void RegisterAsWorker()
        {
            // Datos a esperar cuando creamos el worker
            const long expectedUserID = 111;

            // Chequeo de que las respuestas del comando sean las esperadas según los datos enviados.
            // Primer entrada: comando a usar
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un trabajador. \nIngrese el nombre del trabajador";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Segunda entrada: nombre
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Tercer entrada: apellido
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Mbappé");
            expectedResponseOfCommand = "Ingrese su número de celular.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            //Cuarta entrada: número
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "098111333");
            expectedResponseOfCommand = "Ingrese su dirección por favor.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Intercambiador Belloni");
            expectedResponseOfCommand = $"Se hizo correctamente el registro, ahora eres un trabajador. \nNombre: Kylian Mbappé \nTeléfono: 098111333 \nUbicación: Avenida José Belloni, Montevideo";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            bool workerExist = false;
            foreach (Worker worker in Database.Instance.GetWorkers())
            {
                if (worker.UserID == expectedUserID)
                {
                    workerExist = true;
                }
            }
            Assert.IsTrue(workerExist);
        }

        /// <summary>
        /// Prueba a crear un worker pero con datos que fallen.
        /// </summary>
        [Test]
        public void FailedRegisterAsWorker()
        {
            // UserID a usar
            const long expectedUserID = 722;

            // Chequeo de que las respuestas del comando sean las esperadas según los datos enviados.
            // Primer entrada: comando a usar
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "registrarme como laburante");
            string expectedResponseOfCommand = "Ese nombre no coincide con el de un comando";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un trabajador. \nIngrese el nombre del trabajador";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Segunda entrada: nombre
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "    ");
            expectedResponseOfCommand = "Los datos ingresados no pueden ser vacíos o ser solo espacios en blanco. \nPor favor ingrese información o cancele la ejecución del comando ingresando 'cancel'";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Tercer entrada: apellido
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "    ");
            expectedResponseOfCommand = "Los datos ingresados no pueden ser vacíos o ser solo espacios en blanco. \nPor favor ingrese información o cancele la ejecución del comando ingresando 'cancel'";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Mbappé");
            expectedResponseOfCommand = "Ingrese su número de celular.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            //Cuarta entrada: número
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "  4");
            expectedResponseOfCommand = "Su número de celular debe tener 9 dígitos.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "090123432");
            expectedResponseOfCommand = "El celular que ingreso no tiene un formato valido. El formato valido es: 09[1-9]xxxxxx. Cambiando las x por los números correspondientes";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "098111333");
            expectedResponseOfCommand = "Ingrese su dirección por favor.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Quinta entrada: ubicación
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "  ");
            expectedResponseOfCommand = "Los datos ingresados no pueden ser vacíos o ser solo espacios en blanco. \nPor favor ingrese información o cancele la ejecución del comando ingresando 'cancel'";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Repetimos la quinta entrada con otro dato erroneo
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "?");
            expectedResponseOfCommand = "No encuentro la dirección. Decime qué dirección querés buscar por favor";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Intercambiador Belloni");
            expectedResponseOfCommand = $"Se hizo correctamente el registro, ahora eres un trabajador. \nNombre: Kylian Mbappé \nTeléfono: 098111333 \nUbicación: Avenida José Belloni, Montevideo";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            bool workerExist = false;
            foreach (Worker worker in Database.Instance.GetWorkers())
            {
                if (worker.UserID == expectedUserID)
                {
                    workerExist = true;
                }
            }
            Assert.IsTrue(workerExist);
        }

        /// <summary>
        /// Creamos un test que cancele la operación usando la palabra "cancel" en el segundo paso (el primero es ingresar el comando)
        /// </summary>
        [Test]
        public void cancelóperationInSecondStep()
        {
            const long expectedUserID = 912;
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un trabajador. \nIngrese el nombre del trabajador";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Espera segunda entrada: nombre
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Creamos un test que cancele la operación usando la palabra "cancel" en el tercer paso
        /// </summary>
        [Test]
        public void cancelóperationInThirdStep()
        {
            const long expectedUserID = 912;
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un trabajador. \nIngrese el nombre del trabajador";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Espera tercer entrada: apellido
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Creamos un test que cancele la operación usando la palabra "cancel" en el cuarto paso
        /// </summary>
        [Test]
        public void cancelóperationInFourthStep()
        {
            const long expectedUserID = 912;
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un trabajador. \nIngrese el nombre del trabajador";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Mbappé");
            expectedResponseOfCommand = "Ingrese su número de celular.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Espera cuarta entrada: número
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Creamos un test que cancele la operación usando la palabra "cancel" en el quinto paso
        /// </summary>
        [Test]
        public void cancelóperationInFifthStep()
        {
            const long expectedUserID = 912;
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un trabajador. \nIngrese el nombre del trabajador";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Mbappé");
            expectedResponseOfCommand = "Ingrese su número de celular.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "098111333");
            expectedResponseOfCommand = "Ingrese su dirección por favor.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Espera quinta entrada: nombre de ubicación
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

    }
}