using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe.

namespace Tests
{
    /// <summary>
    /// Test del comando registar un usuario como un employer
    /// </summary>
    [TestFixture]
    public class RegisterAsEmployerTest
    {
        long workerID = 0;
        long employerID = 500;
        long adminID = 1000;

        /// <summary>
        /// User story de registrarse como employer
        /// </summary>
        /// 
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
        public void EmployerTrRegisterAsEmployer()
        {
            string commandName = "/registerasemployer";

             employerID += 3;
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
        public void WorkerTrRegisterAsEmployer()
        {
            string commandName = "/registerasemployer";
            workerID += 1;
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
        public void AdminTrRegisterAsEmployer()
        {
            adminID += 1;
            string commandName = "/registerasemployer";
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
        public void RegisterAsEmployer()
        {
            // Datos a usar cuando creamos el employer
            const long expectedUserID = 212;
            const string commandName = "/registerasemployer";

            // Chequeo de que las respuestas del comando sean las esperadas según los datos enviados.
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un empleador. \nCuál es su nombre?";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Mbappé");
            expectedResponseOfCommand = "Ingrese su número de celular. solo los números.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "098111333");
            expectedResponseOfCommand = "Ingrese su dirección por favor.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Avenida Italia y Cooper");
            expectedResponseOfCommand = $"Se hizo correctamente el registro, ahora eres un empleador. \nNombre: Kylian Mbappé \nTeléfono: 098111333 \nUbicación: Avenida Italia & Cooper, Montevideo, Uruguay";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == expectedUserID)
                {
                    employerExist = true;
                }
            }
            Assert.IsTrue(employerExist);
        }

        /// <summary>
        /// Prueba a crear un employer pero con datos que fallen.
        /// </summary>
        [Test]
        public void FailedRegisterAsEmployer()
        {
            // UserID a usar
            const long expectedUserID = 723;
            const string commandName = "/registerasemployer";

            // Chequeo de que las respuestas del comando sean las esperadas según los datos enviados.
            // Primer entrada: comando a usar
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "registrarme como contratador");
            string expectedResponseOfCommand = "Ese nombre no coincide con el de un comando";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
            
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un empleador. \nCuál es su nombre?";
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
            expectedResponseOfCommand = "Ingrese su número de celular. solo los números.";
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


            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Avenida Italia y Cooper");
            expectedResponseOfCommand = $"Se hizo correctamente el registro, ahora eres un empleador. \nNombre: Kylian Mbappé \nTeléfono: 098111333 \nUbicación: Avenida Italia & Cooper, Montevideo, Uruguay";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == expectedUserID)
                {
                    employerExist = true;
                }
            }
            Assert.IsTrue(employerExist);
        }

        /// <summary>
        /// Creamos un test que cancele la operación usando la palabra "cancel" en el segundo paso (el primero es ingresar el comando)
        /// </summary>
        [Test]
        public void CancelperationInSecondStep()
        {
            const string commandName = "/registerasemployer";

            const long expectedUserID = 913;
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un empleador. \nCuál es su nombre?";
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
        public void CancelperationInThirdStep()
        {
            const string commandName = "/registerasemployer";

            const long expectedUserID = 913;
            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un empleador. \nCuál es su nombre?";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Espera tercer entrada: apellido
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Creamos un test que cancele la operación usando la palabra "cancel" en el primer paso
        /// </summary>
        [Test]
        public void CancelperationInFourthStep()
        {
            const long expectedUserID = 913;
            const string commandName = "/registerasemployer";

            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un empleador. \nCuál es su nombre?";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Mbappé");
            expectedResponseOfCommand = "Ingrese su número de celular. solo los números.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            // Espera cuarta entrada: número
            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "cancel");
            expectedResponseOfCommand = $"El comando {commandName} se canceló";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);
        }

        /// <summary>
        /// Creamos un test que cancele la operación usando la palabra "cancel" en el primer paso
        /// </summary>
        [Test]
        public void CancelperationInFifthStep()
        {
            const long expectedUserID = 913;
            const string commandName = "/registerasemployer";

            string actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            string expectedResponseOfCommand = "Ahora pasaremos a pedir los datos para crear un empleador. \nCuál es su nombre?";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "kylian");
            expectedResponseOfCommand = "Ingrese su apellido.";
            Assert.AreEqual(expectedResponseOfCommand, actualResponseOfCommand);

            actualResponseOfCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "Mbappé");
            expectedResponseOfCommand = "Ingrese su número de celular. solo los números.";
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