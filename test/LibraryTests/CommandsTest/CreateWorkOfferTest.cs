using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using System;
using ClassLibrary;

// Test responsabilidad de Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Clase que se encarga de probar la funcionalidad del comando de crear ofertas de trabajo
    /// </summary>
    [TestFixture]
    public class CreateWorkOfferTest
    {


        private long workerID = 0;

        private long ownerID = 0;
        private long employerID = 500;
        private long adminID = 1000;

        /// <summary>
        /// User story de crear work offer.
        /// </summary>
        /// 
        [SetUp]
        public void Setup()
        {
            Database.Instance.ClearWorkOffers();
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

        }


        /// <summary>
        /// Prueba que un employer intente usar el comando y no pueda hacerlo
        /// </summary>
        [Test]
        public void EmployerTryCreateWorkOffer()
        {
            employerID += 12;
            string commandName = "/createworkoffer";
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
        public void AdminTryCreateWorkOffer()
        {
            adminID += 212;
            string commandName = "/createworkoffer";
            // Agrego el admin
            Database.Instance.AddAdmin(adminID);
            const string responseWhenNotCanExecuteCommand = "El tipo de perfil que tienes no tiene permito usar este comando";

            // Probamos que devuelva eso en particular
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(adminID, commandName);
            Assert.AreEqual(responseWhenNotCanExecuteCommand, responseCommand);
        }

        /// <summary>
        /// Prueba que se cree y agregue una oferta de trabajo.
        /// </summary>
        [Test]
        public void CreateWorkOfferWhithoutProblems()
        {
            // Configuración

            //Cramos al usuario worker que sería dueño de esta oferta
            workerID += 1;
            long workerOwnerID = workerID;
            // Introducimos los erroneos y no se debería de crear el worker
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);

            // Creamos las categoría
            // Las categorías ya se agregan en mayúsculas, por que el comando de crear es el que se encarga
            // de hacer que esten bajo un formato bien, y no la clase Database. Ahí entraría lo de precondition
            // con exceptions pero no se si esta del todo bien
            ReadOnlyCollection<string> catogories = Database.Instance.GetAllCategories();
            Database.Instance.AddCategory("JARDINERIA");
            Database.Instance.AddCategory("PODAS");

            //Cargo los datos para la work offer
            const string description = "Cortapasto";
            const string currency = "UYU";
            const string price = "200";
            const string category1 = "JARDINERIA";
            const string category2 = "PODAS";
            int expectedworkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            const string durationInDays = "4";
            const string commandName = "/createworkoffer";;
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Ingrese la descripción de la oferta de trabajo";
            // Respuesta esperada cuando comienza a ejecutarse el comando, pidiendo la descripción
            const string expectedStarExecuteCommand = "Ingrese el simbolo de la moneda que quiere usar. \nLos valores validos pueden ser  USD, UYU, $U y U$S";
            // Respuesta esperada cuando termina de ingresar la moneda
            const string expectedCurrencyPromtResponse = "Ingrese el precio que tendrá la oferta";
            // Respuesta esperada cuando termina de ingresar el precio
            const string expectedPricePromtResponse = "Ingrese la duración en días que tendrá la oferta, si se trata de horas ponga un día";
            // Respuesta esperada cuando termina de ingresar la cantidad de días
            const string expectedDurationInDaysPromtResponse = "Ingrese una categoría que tendrá esta oferta, para dejar de ingresar categorías ingrese el comando 'done'";
            // Repuesta esperada cuando ingresa una o más categorías
            const string expectedCateogoryPromtResponse = "Ingrese otra cateogoría, recuerde, 'done' para dejar de ingresar categorías";
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Se ha creado correctamente la oferta de trabajo. \nDescripción: {description} \nPrecio: {price} {currency} \nCategoría(s): {category1}, {category2} \nDuración: {durationInDays}";

            // Ejecutamos comportamiento y comprobaciones
            //Le decimos que vamos a usar ese comando, por lo cual lo inicia, pero para saber que se 
            // va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió

            string responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Ahora que ya estamos tratando con ese comando en particular, le pasamos el primer dato 
            // que pide, la descripción. Y alojamos la respuesta en la variable responseCommand
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);
            // Ahora pidio la moneda, así que hay que ingresar la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, currency);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCurrencyPromtResponse, responseCommand);
            // Ahora pidio el precio, así que hay que ingresar el precio
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, price);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedPricePromtResponse, responseCommand);
            // Ahora pidio la cantidad de días que dura la oferta
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, durationInDays);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedDurationInDaysPromtResponse, responseCommand);
            // Ahora pidio que ingresen categorías, vamos a hacerlo dos veces para aseguranos que puede con más de una
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, category1);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCateogoryPromtResponse, responseCommand);
            // Segunda categoría
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, category2);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCateogoryPromtResponse, responseCommand);
            // Ingresamos done para esperar que termine de ingresar categorías
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, "done");


            // Comprobamos que devuleva el texto de que se creo correctamente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);

            // Compruebo que la oferta se haya almacenado en la database.

            bool workOfferExist = Database.Instance.ExistWorkOffer(expectedworkOfferID);
            Assert.IsTrue(workOfferExist);
        }

        /// <summary>
        /// Probamos el crear una oferta con todos los datos mal en un incio y luego los modficiamos para que esten bien.
        /// Cuando se ingresan mal debería de retornar un mensaje y cuando se ingresan bien otro.
        /// </summary>
        [Test]
        public void CreateWorkOfferWithProblems()
        {


            workerID += 1;
            long expectedUserID = workerID;
            //Cramos al usuario worker que sería dueño de esta oferta
            long workerOwnerID = expectedUserID;
            // Introducimos los erroneos y no se debería de crear el worker
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);

            // Creamos las categoría

            ReadOnlyCollection<string> catogories = Database.Instance.GetAllCategories();
            Database.Instance.AddCategory("JARDINERIA");
            Database.Instance.AddCategory("PODAS");
            const string commandName = "/createworkoffer";
            int expectedworkOfferID = Database.Instance.UltimateIDWorkOffer + 1;

            //Datos correctos para la work offer
            const string description = "Cortapasto";
            const string currency = "UYU";
            const string price = "200";
            const string category1 = "JARDINERIA";
            const string category2 = "PODAS";
            const string durationInDays = "4";

            // Datos con foramto incorrecto para la work offer
            const string wrongDescription = ".";
            const string wrongCurrency = "YJP";
            const string wrongPrice = "-2000";
            const string wrongPrice2 = "Hola";
            const string wrongCategory1 = "";
            const string wrongCategory2 = "No esta en la lista, hoy es jueves";
            const string wrongDurationInDays = "-90";
            const string wrongDurationInDays2 = "4 días";


            // Respuestas de cuando todo esta bien
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Ingrese la descripción de la oferta de trabajo";
            // Respuesta esperada cuando comienza a ejecutarse el comando, pidiendo la descripción
            const string expectedStarExecuteCommand = "Ingrese el simbolo de la moneda que quiere usar. \nLos valores validos pueden ser  USD, UYU, $U y U$S";
            // Respuesta esperada cuando termina de ingresar la moneda
            const string expectedCurrencyPromtResponse = "Ingrese el precio que tendrá la oferta";
            // Respuesta esperada cuando termina de ingresar el precio
            const string expectedPricePromtResponse = "Ingrese la duración en días que tendrá la oferta, si se trata de horas ponga un día";
            // Respuesta esperada cuando termina de ingresar la cantidad de días
            const string expectedDurationInDaysPromtResponse = "Ingrese una categoría que tendrá esta oferta, para dejar de ingresar categorías ingrese el comando 'done'";
            // Repuesta esperada cuando ingresa una o más categorías
            const string expectedCateogoryPromtResponse = "Ingrese otra cateogoría, recuerde, 'done' para dejar de ingresar categorías";
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedExecuteResponse = $"Se ha creado correctamente la oferta de trabajo. \nDescripción: {description} \nPrecio: {price} {currency} \nCategoría(s): {category1}, {category2} \nDuración: {durationInDays}";
            // Fin de respuestas de cuando todo esta bien

            // Respuestas que debe dar cuando las cosas no estan bien
            // Vale aclarar que cualquier dato vació o con espacios en blanco, será frenado en el InputInterfacer.Instance interfacer
            // Respuesta esperada cuando comienza a ejecutarse el comando, pidiendo la descripción
            const string expectedStarExecuteFaildCommand = "La descripción no puede estar vacía y debe ser mayor a 6 letras.";
            // Respuesta esperada cuando termina de ingresar la moneda
            const string expectedCurrencyPromtFaildResponse = "La moneda especificada no esta dentro de las validas, las expresioens validas son: USD, UYU, $U y U$S";
            // Respuesta esperada cuando termina de ingresar el precio
            const string expectedPricePromtFaildResponse1 = "Ingrese un precio mayor a 0, sus servicios no pueden ser gratis, por ahora";
            // Respuesta esperada cuando termina de ingresar el precio
            const string expectedPricePromtFaildResponse2 = "El precio debe ser un valor númerico entero";
            // Respuesta esperada cuando termina de ingresar la cantidad de días
            const string expectedDurationInDaysPromtFaildResponse1 = "La duración no puede ser menor a 0, si quiere referirse a un trabajo que se hace en un día, ponga 1";
            const string expectedDurationInDaysPromtFaildResponse2 = "La duración del trabajo debe ser un valor númerico entero";
            // Repuesta esperada cuando ingresa una o más categorías
            const string expectedCateogoryPromtFaildResponse1 = "Los datos ingresados no pueden ser vacíos o ser solo espacios en blanco. \nPor favor ingrese información o cancele la ejecución del comando ingresando 'cancel'";
            // Ahora con la segunda categoría que esta mal

            string expectedCateogoryPromtFaildResponse2 = $"La cateogoría {wrongCategory2.Trim()} no existe. Revise que el nombre ese bien puesto o si existe esa categoría realmente";
            // Mensaje de cuando  ingresa 'done' sin haber ninguna categoría valida
            string expectedResponseCategoriesCountIsCero = "No ha ingresado ninguna categoría, ingrese una por favor";
            // Fin de respuestas que debe dar cuando las cosas no estan bien

            // Primero van a fallar en el sentido de salir mal, luego bien

            //Le decimos que vamos a usar ese comando, por lo cual lo inicia, pero para saber que se 
            // va a hacer ese comando, comprobamos el texto que debería de devolver con el que devolvió
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, commandName);
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Ahora que ya estamos tratando con ese comando en particular, le pasamos el primer dato 
            // que pide, la descripción. Y alojamos la respuesta en la variable responseCommand. 
            // Pero esta vez pasamos primero el dato con formato equivocado
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongDescription);
            //Comprobamos que el dato ingresado estuvo mal y retorne el mensaje correspondiente
            Assert.AreEqual(expectedStarExecuteFaildCommand, responseCommand);
            // Ahora lo hacemos bien
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);

            // Ahora hacemso lo mismo con la moneda, así que hay que ingresar la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongCurrency);
            // Comprobamos que devuleva el texto que le corresponde al haber metido mal la data
            Assert.AreEqual(expectedCurrencyPromtFaildResponse, responseCommand);
            // Ahora ingresamos bien la información
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, currency);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCurrencyPromtResponse, responseCommand);

            // Ahora pidio el precio, así que hay que ingresar el precio, pero se lo ponemos mal, pasando un dato que es menor a 1, para revisar que devuleva lo que debe devolver
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongPrice);
            // Comprobamos que devuleva el texto de que se dio un error y cual es ese error en la ejecución del comando
            Assert.AreEqual(expectedPricePromtFaildResponse1, responseCommand);
            // Volvemos a ingresarlo mal, esperando que nos pida que ingresemos un valor entero
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongPrice2);
            // Comprobamos que devuleva el texto de que se dio un error y cual es ese error en la ejecución del comando
            Assert.AreEqual(expectedPricePromtFaildResponse2, responseCommand);
            // Ahora se lo ingresamos bien y vemos que pasa
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, price);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedPricePromtResponse, responseCommand);

            // Ahora pide la cantidad de días que dura la oferta, pero vamos a ingresar dos veces mal los datos, para 
            //recoger que devulva el mensaje esperado en ese caso
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongDurationInDays);
            // Comprobamos que devuleva el texto de que fallo y que fallo por lo que pensabamos. Por que los días son negativos
            Assert.AreEqual(expectedDurationInDaysPromtFaildResponse1, responseCommand);
            // Caso en el que le ingresamos letras además de números, por ende nos dirá que ingresemos números enteros
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongDurationInDays2);
            // Comprobamos que devuleva el texto de que fallo y que fallo por lo que pensabamos. Por que no ingreso solamente numeros
            Assert.AreEqual(expectedDurationInDaysPromtFaildResponse2, responseCommand);
            // El caso en el que esta todo bien
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, durationInDays);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedDurationInDaysPromtResponse, responseCommand);

            // Ahora le pasamos dos categorías invalidas, una que no esta vacía, por ende se frena en el InputInterfacer.Instance interfacer.
            // La otra es un nombre que no existe en la lista, por ende devolverá como que no la encuentra
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongCategory1);
            // Comprobamos que devuelva como que no había texto y que ingrse información.
            Assert.AreEqual(expectedCateogoryPromtFaildResponse1, responseCommand);
            // Ahora el caso de que no exista una categoría con ese nombre
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, wrongCategory2);
            // Comprobamos que devuelva como que no había una categoría con ese nombre, que revise bien los datos ingresados
            Assert.AreEqual(expectedCateogoryPromtFaildResponse2, responseCommand);
            // Probamso que ingresa "done" sin haber puesto categorías
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "done");
            Assert.AreEqual(expectedResponseCategoriesCountIsCero, responseCommand);

            // Ahora ingresamos bien los datos y comprobamos que sigan funcionando bien
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, category1);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCateogoryPromtResponse, responseCommand);
            // Segunda categoría
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, category2);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCateogoryPromtResponse, responseCommand);
            // Ingresamos done para esperar que termine de ingresar categorías
            responseCommand = InputInterfacer.Instance.TranslateToCommand(expectedUserID, "done");
            // Comprobamos que devuleva el texto de que se creo correctamente
            Assert.AreEqual(expectedExecuteResponse, responseCommand);


            // Compruebo que la oferta se haya almacenado en la database.

            bool workOfferExist = Database.Instance.ExistWorkOffer(expectedworkOfferID);

            Assert.IsTrue(workOfferExist);
        }

         /// <summary>
        /// Prueba que se cree y agregue una oferta de trabajo.
        /// </summary>
        [Test]
        public void CancelCreateWorkOfferForStep()
        {
            // Configuración

            //Cramos al usuario worker que sería dueño de esta oferta
            workerID += 1;
            long workerOwnerID = workerID;
            // Introducimos los erroneos y no se debería de crear el worker
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);

            // Creamos las categoría
            // Las categorías ya se agregan en mayúsculas, por que el comando de crear es el que se encarga
            // de hacer que esten bajo un formato bien, y no la clase Database. Ahí entraría lo de precondition
            // con exceptions pero no se si esta del todo bien
            ReadOnlyCollection<string> catogories = Database.Instance.GetAllCategories();
            Database.Instance.AddCategory("JARDINERIA");
            Database.Instance.AddCategory("PODAS");

            //Cargo los datos para la work offer
            const string description = "Cortapasto";
            const string currency = "UYU";
            const string price = "200";
            const string category1 = "JARDINERIA";
            const string category2 = "PODAS";
            int expectedworkOfferID = Database.Instance.UltimateIDWorkOffer + 1;
            const string durationInDays = "4";
            const string commandName = "/createworkoffer";
            // Respuesta esperada de cuando inicia el comando, pidiendo ya de una la descripción
            const string expectedStartCommandResponse = "Ingrese la descripción de la oferta de trabajo";
            // Respuesta esperada cuando comienza a ejecutarse el comando, pidiendo la descripción
            const string expectedStarExecuteCommand = "Ingrese el simbolo de la moneda que quiere usar. \nLos valores validos pueden ser  USD, UYU, $U y U$S";
            // Respuesta esperada cuando termina de ingresar la moneda
            const string expectedCurrencyPromtResponse = "Ingrese el precio que tendrá la oferta";
            // Respuesta esperada cuando termina de ingresar el precio
            const string expectedPricePromtResponse = "Ingrese la duración en días que tendrá la oferta, si se trata de horas ponga un día";
            // Respuesta esperada cuando termina de ingresar la cantidad de días
            const string expectedDurationInDaysPromtResponse = "Ingrese una categoría que tendrá esta oferta, para dejar de ingresar categorías ingrese el comando 'done'";
            // Repuesta esperada cuando ingresa una o más categorías
            const string expectedCateogoryPromtResponse = "Ingrese otra cateogoría, recuerde, 'done' para dejar de ingresar categorías";
            // Respuesta que se espera cuando termina de ingresar las categorías
            const string expectedCancelResponse = $"El comando {commandName} se canceló";
            // Texto que ingresa para cancelar la ejecución del comando
            const string cancelInput = "cancel";

         
            // Vamos haciendo de a pasos, cancelamos y volvemos a ejecutar para llegar a ese paso

            // Arranca a usar el comando, pasa al estado Started
            string responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            // Comprobamos que haya ingresado bien a ese estado
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Nos pidio el nombre, y ahora le decimos cancel
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);

            // Ahora hacemos que llegue al paso moneda y cancela allí
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            // Comprobamos que haya ingresado bien a ese estado
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Pasa al paso de la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);
            // Ahora pidio la moneda y le ingresamos cancel, debería decir que el comando se canceló
             responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);

            // Ahora lo hacemos ingresar en precio y cancelamos ahí
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            // Comprobamos que haya ingresado bien a ese estado
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Pasa al paso de la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);
            // Ahora pidio la moneda, así que hay que ingresar la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, currency);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCurrencyPromtResponse, responseCommand);
            // Ahora pidio el precio y le ingresamos cancel, debería decir que el comando se canceló
             responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);

            // Ahora lo hacemos llegar al paso de duración de trabajo
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            // Comprobamos que haya ingresado bien a ese estado
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Pasa al paso de la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);
            // Ahora pidio la moneda, así que hay que ingresar la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, currency);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCurrencyPromtResponse, responseCommand);
            // Ahora pidio el precio, así que hay que ingresar el precio
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, price);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedPricePromtResponse, responseCommand);
            // Ahora pidio la duración del trabajo y le ingresamos cancel, debería decir que el comando se canceló
             responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);

            // Ahora pide ingresar las categorías, le da cancel apenas entrar
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            // Comprobamos que haya ingresado bien a ese estado
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Pasa al paso de la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);
            // Ahora pidio la moneda, así que hay que ingresar la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, currency);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCurrencyPromtResponse, responseCommand);
            // Ahora pidio el precio, así que hay que ingresar el precio
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, price);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedPricePromtResponse, responseCommand);
            // Ahora pidio la cantidad de días que dura la oferta
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, durationInDays);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedDurationInDaysPromtResponse, responseCommand);
            // Ahora pidio la duración del trabajo y le ingresamos cancel, debería decir que el comando se canceló
             responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);

            // Ahora pide ingresar las categorías, le da cancel luego de ingresar una categoría
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            // Comprobamos que haya ingresado bien a ese estado
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Pasa al paso de la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);
            // Ahora pidio la moneda, así que hay que ingresar la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, currency);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCurrencyPromtResponse, responseCommand);
            // Ahora pidio el precio, así que hay que ingresar el precio
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, price);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedPricePromtResponse, responseCommand);
            // Ahora pidio la cantidad de días que dura la oferta
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, durationInDays);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedDurationInDaysPromtResponse, responseCommand);
            // Ahora pidio que ingresen categorías, vamos a hacerlo dos veces para aseguranos que puede con más de una
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, category1);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCateogoryPromtResponse, responseCommand);
            // Ahora pidie otra caegoría y  le ingresamos cancel, debería decir que el comando se canceló
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);
            
            // Ahora pide ingresar las categorías, le da cancel luego de ingresar una la segunda categoría
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, commandName);
            // Comprobamos que haya ingresado bien a ese estado
            Assert.AreEqual(expectedStartCommandResponse, responseCommand);
            // Pasa al paso de la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, description);
            //Comprobamos que el dato ingresado estuvo bien
            Assert.AreEqual(expectedStarExecuteCommand, responseCommand);
            // Ahora pidio la moneda, así que hay que ingresar la moneda
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, currency);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCurrencyPromtResponse, responseCommand);
            // Ahora pidio el precio, así que hay que ingresar el precio
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, price);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedPricePromtResponse, responseCommand);
            // Ahora pidio la cantidad de días que dura la oferta
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, durationInDays);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedDurationInDaysPromtResponse, responseCommand);
            // Ahora pidio que ingresen categorías, vamos a hacerlo dos veces para aseguranos que puede con más de una
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, category1);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCateogoryPromtResponse, responseCommand);
            // Segunda categoría
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, category2);
            // Comprobamos que devuleva el texto del siguiente paso, para saber que todo salio bien con este
            Assert.AreEqual(expectedCateogoryPromtResponse, responseCommand);
            // Ahora pidie otra caegoría y  le ingresamos cancel, debería decir que el comando se canceló
            responseCommand = InputInterfacer.Instance.TranslateToCommand(workerOwnerID, cancelInput);
            // Probamos que haya devuelto el mensaje de cancelación
            Assert.AreEqual(responseCommand, expectedCancelResponse);
            
            // Ahora ya no hay más pasos en los cuales se pueda cancelar, por ende acá termina el test de probar 
            // que se ejecute el cancel en cada paso. 
        }

    }
}