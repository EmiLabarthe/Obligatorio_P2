using System.Collections.Generic;
using NUnit.Framework;
using System;
using ClassLibrary;
using Exceptions;
using System.Collections.ObjectModel;

// Este test es responsabilidad de Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Prueba de la clase <see cref="Database"/>.
    /// </summary>
    [TestFixture]
    public class DatabaseTests
    {

        private long workerID = 0;
        private long employerID = 500;
        private long adminID = 1000;

        //Datos genericos sobre las workoffers
        const string description = "Una oferta de trabajo";
        const string currency = "UYU";
        const int price = 2000;
        long ownerID = 0;

        const int durationWorkOffer = 20;
        List<string> categories = new List<string>() { "Traslados", "Fletes" };

        // Datos para crear un user
        const string name = "Agustin";
        const string lastName = "Toya";
        const string phone = "095185397";
        const string address = "Cordón";
        const double latitude = -34.9999313;
        const double longitude = -50.5731;

        /// <summary>
        /// Crea la base de datos a probar
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // La jerarquía de id en las listas de users 
            // para no chocar es de, primero los admin, luego los employeres y 
            // luego los workers, por ende las id más grandes van a ser la de los 
            // workers en los casos de prueba
            //Recojemos el ultimo id que se metio en la lista de workers
            int cantWorkers = Database.Instance.GetWorkers().Count;
            int cantEmployers = Database.Instance.GetEmployers().Count;
            int cantAdmins = Database.Instance.GetAdmins().Count;
            if (cantWorkers != 0)
            {
                long workerIDBiggest = Database.Instance.GetWorkers()[cantWorkers - 1].UserID;
                workerID += workerIDBiggest + 1;
                ownerID = workerID;
            }
            if (cantEmployers != 0)
            {
                long employerIDBiggest = Database.Instance.GetEmployers()[cantEmployers - 1].UserID;
                employerID += employerIDBiggest + 1 - 500;
            }
            if (cantAdmins != 0)
            {
                long userIDBiggest = Database.Instance.GetAdmins()[cantAdmins - 1].UserID;
                adminID += userIDBiggest + 1 - 1000;
            }
            Database.Instance.AddCategory("Traslados");
            Database.Instance.AddCategory("Fletes");


        }


        /// <summary>
        /// Prueba que se cree y agruge una oferta de trabajo.
        /// </summary>
        [Test]
        public void AddWorkOffer()
        {
            // Configuración
            workerID += 1;
            long workerOwnerID = workerID;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Como es el caso en el que esta todo bien, basta con los datos genericos cargados al inicio
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, workerOwnerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(description,  currency, price, workerOwnerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 1);

        }

        /// <summary>
        /// Prueba que no se cree una oferta de trabajo por que la descripción es nula
        /// </summary>
        [Test]
        public void AddWorkOfferDescriptionNull()
        {
            // Configuración
            // Todos los datos salvo la descripción siguien iguales, ya que es lo que se quiere probar
            string descriptionNull = null;
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(descriptionNull, currency, price, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(descriptionNull,  currency, price, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);

        }

        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la descripción es empty 
        /// </summary>
        [Test]
        public void AddWorkOfferDescriptionEmpty()
        {
            // Configuración
            // Todos los datos salvo la descripción siguien iguales, ya que es lo que se quiere probar
            string descriptionEmtpy = "";
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(descriptionEmtpy, currency, price, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(descriptionEmtpy,  currency, price, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);
        }


        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la descripción no tiene nada, es espacios en blanco 
        /// </summary>
        [Test]
        public void AddWorkOfferDescriptionWhiteSpace()
        {
            // Configuración
            // Todos los datos salvo la descripción siguien iguales, ya que es lo que se quiere probar
            string descriptionWhiteSpace = "   ";
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(descriptionWhiteSpace, currency, price, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(descriptionWhiteSpace,  currency, price, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);
        }


        /// <summary>
        /// Prueba que no se cree una oferta de trabajo por que la moneda es nula
        /// </summary>
        [Test]
        public void AddWorkOfferCurrencyNull()
        {
            // Configuración
            // Todos los datos salvo la moneda siguien iguales, ya que es lo que se quiere probar
            string currencyNull = null;
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currencyNull, price, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(description,  currencyNull, price, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);

        }

        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la moneda es empty 
        /// </summary>
        [Test]
        public void AddWorkOfferCurrencyEmpty()
        {
            // Configuración
            // Todos los datos salvo la moneda siguien iguales, ya que es lo que se quiere probar
            string currencyEmpty = "";
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currencyEmpty, price, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(description,  currencyEmpty, price, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);
        }


        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la moneda no tiene nada, contiene solo espacios en blanco 
        /// </summary>
        [Test]
        public void AddWorkOfferCurrencyWhiteSpace()
        {
            // Configuración
            // Todos los datos salvo la moneda siguien iguales, ya que es lo que se quiere probar
            string currencyWhiteSpace = "   ";
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currencyWhiteSpace, price, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(description,  currencyWhiteSpace, price, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);
        }

        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la moneda indicada no es la correcta. Es texto, 
        /// pero no del esperado por el método
        /// </summary>
        [Test]
        public void AddWorkOfferWrongCurrency()
        {
            // Configuración
            // Todos los datos salvo la moneda siguien iguales, ya que es lo que se quiere probar
            string wrongCurrency = "JPY";
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, wrongCurrency, price, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(description,  wrongCurrency, price, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);
        }


        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que el precio indicado no 
        /// es el correcto por que es menor a 0
        /// </summary>
        [Test]
        public void AddWorkOfferLessThantCeroPrice()
        {
            // Configuración
            // Todos los datos salvo el precio siguien iguales, ya que es lo que se quiere probar
            int wrongPrice = -2123;
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, wrongPrice, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(description,  currency, wrongPrice, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);
        }

        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que el precio indicado no 
        /// es el correcto por que es menor a 0
        /// </summary>
        [Test]
        public void AddWorkOfferEqualsCeroPrice()
        {
            // Configuración
            // Todos los datos salvo el precio siguien iguales, ya que es lo que se quiere probar
            int wrongPrice = 0;
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, wrongPrice, ownerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = (Database.Instance.MatachWorkOffer(description,  currency, wrongPrice, ownerID,  categories , durationWorkOffer));
            Assert.IsTrue(matchs == 0);
        }

        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la duracción del trabajo
        ///  es menor a 0, lo cual no es valido
        /// </summary>
        [Test]
        public void AddWorkOfferLessThantCeroDuration()
        {
            // Configuración
            // Todos los datos salvo la duración siguien iguales, ya que es lo que se quiere probar
            int wrongDuration = -23;
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, ownerID, categories, wrongDuration);
            // Comprobación
            int matchs = Database.Instance.MatachWorkOffer(description,  currency, price, ownerID,  categories , wrongDuration);
            Assert.IsTrue(matchs == 0);
        }

        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la duración del trabajo  no 
        /// es la correcta por que es igual a 0
        /// </summary>
        [Test]
        public void AddWorkOfferEqualsCeroDuration()
        {
            // Configuración
            // Todos los datos salvo la duración siguien iguales, ya que es lo que se quiere probar
            int wrongDuration = 0;
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, ownerID, categories, wrongDuration);
            // Comprobación
            int matchs = Database.Instance.MatachWorkOffer(description,  currency, price, ownerID,  categories , wrongDuration);
            Assert.IsTrue(matchs == 0);
        }

        // Ahora hacemos los casos para las categorías
        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que la categoría indicada no existe 
        /// es la correcta por que es igual a 0
        /// </summary>
        [Test]
        public void AddWorkOfferNoExistCategory()
        {
            // Configuración
            // Todos los datos salvo la categoría siguien iguales, ya que es lo que se quiere probar
            List<string> noExistCategory = new List<string>() { "Sastrería" };
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, ownerID, noExistCategory, durationWorkOffer);
            // Comprobación
            bool exist = Database.Instance.ExistWorkOffer(ID);
            Assert.IsFalse(exist);
            int countMatchs = Database.Instance.MatachWorkOffer(description,  currency, price, ownerID,  noExistCategory , durationWorkOffer);
            bool noExist = countMatchs == 0;
            Assert.True(noExist);
        }


        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que en sí no se indica ninguna categoría, se pasa una 
        /// lista vacía. 
        /// </summary>
        [Test]
        public void AddWorkOfferEmptyListCategories()
        {
            // Configuración
            // Todos los datos salvo la categoría siguien iguales, ya que es lo que se quiere probar
            List<string> emptyCategoryList = new List<string>();
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, ownerID, emptyCategoryList, durationWorkOffer);
            // Comprobación
            bool exist = Database.Instance.ExistWorkOffer(ID);
            Assert.IsFalse(exist);
            int countMatchs = Database.Instance.MatachWorkOffer(description,  currency, price, ownerID,  emptyCategoryList , durationWorkOffer);
            bool noExist = countMatchs == 0;
            Assert.True(noExist);
        }
        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que en
        ///  sí se indica una categoría en blanco, empty.
        /// </summary>
        [Test]
        public void AddWorkOfferEmptyCategory()
        {
            // Configuración
            // Todos los datos salvo la categoría siguien iguales, ya que es lo que se quiere probar
            List<string> emptyCategory = new List<string>() { "" };
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, ownerID, emptyCategory, durationWorkOffer);
            // Comprobación
            bool exist = Database.Instance.ExistWorkOffer(ID);
            Assert.IsFalse(exist);
            int countMatchs = Database.Instance.MatachWorkOffer(description, currency, price, ownerID, emptyCategory, durationWorkOffer);
            bool noExist = countMatchs == 0;
            Assert.True(noExist);
        }



        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que en sí se indica una categoría
        ///  que solo tiene espacios en blanco
        /// </summary>
        [Test]
        public void AddWorkOfferWhiteSpaceCategory()
        {
            // Configuración
            // Todos los datos salvo la categoría siguien iguales, ya que es lo que se quiere probar
            List<string> whiteSpaceCategory = new List<string>() { "  " };
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, ownerID, whiteSpaceCategory, durationWorkOffer);
            // Comprobación
            bool exist = Database.Instance.ExistWorkOffer(ID);
            Assert.IsFalse(exist);
            int countMatchs = Database.Instance.MatachWorkOffer(description, currency, price, ownerID, whiteSpaceCategory, durationWorkOffer);
            bool noExist = countMatchs == 0;
            Assert.True(noExist);
        }

        /// <summary>
        /// Prueba que no se cree la oferta de trabajo por que en sí se indica una categoría
        ///  que solo tiene espacios en blanco
        /// </summary>
        [Test]
        public void AddWorkOffeNullCategory()
        {
            // Configuración
            // Todos los datos salvo la categoría siguien iguales, ya que es lo que se quiere probar
            List<string> nullCategory = new List<string>() { null };
            // Como la base de datos es un singleton y el id varia en base al tamaño de la lista
            // me pareció coherente que el ID lo configure en base a ese criterio
            int ID = Database.Instance.UltimateIDWorkOffer + 1;
            // Comportamiento
            Database.Instance.AddWorkOffer(description, currency, price, ownerID, nullCategory, durationWorkOffer);
            // Comprobación
            bool exist = Database.Instance.ExistWorkOffer(ID);
            Assert.IsFalse(exist);
     
        }
        /// <summary>
        /// Caso de prueba donde se busca ingresar dos veces la misma oferta, con exactamente los mismos datos
        /// haciendo un duplicado, por ende no se pued permitir, y solo debe alojar una.
        /// </summary>
        [Test]
        public void WorkOfferDuplicate()
        {
            //Configuración
            // Incrementamos la id para independizarnos del resto
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
            Database.Instance.AddWorkOffer(description, currency, price, workerOwnerID, categories, durationWorkOffer);
            Database.Instance.AddWorkOffer(description, currency, price, workerOwnerID, categories, durationWorkOffer);
            // Comprobación
            int matchs = Database.Instance.MatachWorkOffer(description, currency, price, workerOwnerID, categories, durationWorkOffer);
            Assert.IsTrue(matchs == 1);
        }

        /// <summary>
        /// Test donde se prueba que no se cree un workOffer con un identificador de usuario invalido
        /// </summary>
        [Test]
        public void WorkOfferOwnerIDIsNotValid()
        {
            //Configuración
            // Probamos con 0 y -1
            long ownerIDNoUserNoWorker = 0;
            long ownerIDNoUserNoWorker2 = -1;
            Database.Instance.AddWorkOffer(description, currency, price, ownerIDNoUserNoWorker, categories, durationWorkOffer);
            Database.Instance.AddWorkOffer(description, currency, price, ownerIDNoUserNoWorker2, categories, durationWorkOffer);
            // Comprobación
            int matchs = Database.Instance.MatachWorkOffer(description, currency, price, ownerIDNoUserNoWorker, categories, durationWorkOffer);
            Assert.IsTrue(matchs == 0);
            int matchs2 = Database.Instance.MatachWorkOffer(description, currency, price, ownerIDNoUserNoWorker2, categories, durationWorkOffer);
            Assert.IsTrue(matchs2 == 0);
        }      

        /// <summary>
        /// Test donde se prueba que no se cree un workOffer con un identificador de usuario que no corresponde
        /// a un worker, sino a un admin
        /// </summary>
        [Test]
        public void WorkOfferOwnerIDNoIsWorkerIDIsOfAdmin()
        {
            //Configuración
            long ownerIDNoIsWorker = adminID; ;
            Database.Instance.AddWorkOffer(description, currency, price, ownerIDNoIsWorker, categories, durationWorkOffer);
            // Comprobación
            int matchs = Database.Instance.MatachWorkOffer(description, currency, price, ownerIDNoIsWorker, categories, durationWorkOffer);
            Console.WriteLine(matchs);
            Assert.IsTrue(matchs == 0);
        }

        /// <summary>
        /// Test donde se prueba que no se cree un workOffer con un identificador de usuario que no corresponde
        /// a un worker, sino a un admin
        /// </summary>
        [Test]
        public void WorkOfferOwnerIDNoIsWorkerIDIsOfEmployer()
        {
            //Configuración
            long ownerIDNoIsWorker = employerID;
            Database.Instance.AddWorkOffer(description, currency, price, ownerIDNoIsWorker, categories, durationWorkOffer);
            // Comprobación
            int matchs = Database.Instance.MatachWorkOffer(description, currency, price, ownerIDNoIsWorker, categories, durationWorkOffer);
            Console.WriteLine(matchs);
            Assert.IsTrue(matchs == 0);
        }

        // Ahora viene la parte de borrar las work offers

        /// <summary>
        /// Se pasa un id de work offer que no existe, por que esta fuera del rango de 
        /// 0 hasta UltimateWorkOfferID
        /// </summary>
        [Test]
        public void DeleteWorkOfferExist()
        {
            //Configuración
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
           long workerOwnerID = workerID;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);

            //Craeamos una workOffer para asegurarnos que hay una oferta
            int identify = Database.Instance.UltimateIDWorkOffer + 1;
            string newDescription = "Nueva descripción, Turismo";
            Database.Instance.AddWorkOffer(newDescription, currency, price, workerOwnerID, categories, durationWorkOffer);
            // Comportamiento
            Database.Instance.DeleteWorkOffer(identify);
            // Comprobamos
            // Buscamos la oferta de trabajo, a ver si existe, lo cual no debería de existir por que 
            // no debería de haber una oferta con ese identificador
            bool exist = Database.Instance.ExistWorkOffer(identify);
            bool isDelete = true;
            if (exist)
            {
                WorkOffer offer = Database.Instance.SearchWorkOffer(identify);
                if (offer.IsPublished == false)
                {
                    isDelete = true;
                }
            }
            Assert.IsTrue(isDelete);
        }

        /// <summary>
        /// Se pasa un id de work offer que no existe, por que esta fuera del rango de 
        /// 0 hasta UltimateWorkOfferID
        /// </summary>
        [Test]
        public void DeleteWorkOfferNoExist()
        {
            //Configuración
            const int notExistWorkOfferID = -12;
            // Comportamiento
            Database.Instance.DeleteWorkOffer(notExistWorkOfferID);
            // Comprobamos
            // Buscamos la oferta de trabajo, a ver si existe, lo cual no debería de existir por que 
            // no debería de haber una oferta con ese identificador
            bool exist = Database.Instance.ExistWorkOffer(notExistWorkOfferID);
            Assert.IsFalse(exist);
        }
        // Las categorías deben quedar en mayus, por ende a la hora de agregar una, 
        // si no esta en mayus debe ponerse en amyus, si tiene tilde se le debe de sacar 
        // para evitar problemas de formato más adelante

        /// <summary>
        /// Prueba que se cree y agruge una categoría sin importar si esta en mayus o en min en alguna parte
        /// </summary>
        [Test]
        public void AddCategory()
        {
            // Configuración
            //Limpiamos la lista de ofertas por las dudas
            Database.Instance.ClearCategories();
            const string name = "Confitería";
            const string expectedName = "CONFITERIA";
            // Comportamiento
            Database.Instance.AddCategory(name);
            // Comprobación
            ReadOnlyCollection<string> categories = Database.Instance.GetAllCategories();
            Assert.IsTrue(categories.Contains(expectedName));

        }

        /// <summary>
        /// Prueba que no se cree una categoría con un nombre en blanco
        /// </summary>
        [Test]
        public void AddCategoryEmptyName()
        {
            // Configuración
            const string name = "";
            string messageResponseNullEmptyOrWhiteSpace = "La categoría nunca puede ser nula, vacía o ser espacios en blanco";

            // Comportamiento y comprobación, como la información nunca puede llegar vacía ahí, levantamos una excepción
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate { Database.Instance.AddCategory(name); });
            
            

        }

        /// <summary>
        /// Prueba que no se cree una categoría con un nombre null
        /// </summary>
        [Test]
        public void AddCategoryNullName()
        {
            // Configuración
            const string name = null;
            string messageResponseNullEmptyOrWhiteSpace = "La categoría nunca puede ser nula, vacía o ser espacios en blanco";
            
            // Comportamiento y comprobación, como la información nunca puede llegar vacía ahí, levantamos una excepción
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate { Database.Instance.AddCategory(name); });

        }

        /// <summary>
        /// Prueba de que no se creen dos categorías con el mismo nombre
        /// </summary>
        [Test]
        public void AddCategoryDuplicate()
        {
            // Configuración
            //Limpiamos la lista de ofertas por las dudas
            Database.Instance.ClearCategories();
            const string name = "Bartender";
            const string expectedName = "BARTENDER";
            // Comportamiento
            Database.Instance.AddCategory(name);
            // Ingresamos por segunda vez
            Database.Instance.AddCategory(name);
            // Comprobación
            ReadOnlyCollection<string> categories = Database.Instance.GetAllCategories();
            // Ahora debería haber solo una categoría con ese nombre todo en mayus
            int counterMatch = 0;
            foreach (string item in categories)
            {
                if (item == expectedName)
                {
                    counterMatch += 1;
                }
            }
            Assert.IsTrue(counterMatch == 1);

        }



        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker
        /// </summary>
        [Test]
        public void AddWorker()
        {
            // Configuración
            // Datos
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsTrue(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el nombre 
        /// del user esta vacío
        /// </summary>
        [Test]
        public void AddWorkerFailddNameEmpty()
        {
            // Configuración
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string nameEmpty = "";
            //Comportamiento
            Database.Instance.AddWorker(userID, nameEmpty, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el nombre 
        /// del user son espacios vacíos
        /// </summary>
        [Test]
        public void AddWorkerFailddNameWhiteSpace()
        {
            // Configuración
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            // Datos
            // Introducimos los erroneos y no se debería de crear el worker
            const string whiteSpaceName = "    ";
            //Comportamiento
            Database.Instance.AddWorker(userID, whiteSpaceName, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el nombre 
        /// del user esta nullo
        /// </summary>
        [Test]
        public void AddWorkerFailddNameNull()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string nameNull = null;
            //Comportamiento
            Database.Instance.AddWorker(userID, nameNull, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el apellido 
        /// del user esta vacío
        /// </summary>
        [Test]
        public void AddWorkerFaildLastNameEmpty()
        {
            // Configuración

            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string lastNameEmtpy = "";
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastNameEmtpy, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el apellido 
        /// del user esta solo con espacios en blanco.
        /// </summary>
        [Test]
        public void AddWorkerFaildLastNameWhiteSpace()
        {
            // Configuración

            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string lastNameWhiteSpace = "    ";
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastNameWhiteSpace, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);

            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el apellido 
        /// del user esta nulo
        /// </summary>
        [Test]
        public void AddWorkerFaildLastNameNull()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string lastNameNull = null;
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastNameNull, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);

            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el 
        /// phone esta vacío
        /// </summary>
        [Test]
        public void AddWorkerFaildPhoneEmpty()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string phoneEmpty = "";
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastName, phoneEmpty, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);

            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el phone 
        /// del user son espacios en blanco
        /// </summary>
        [Test]
        public void AddWorkerFaildPhonWhiteSpace()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string phoneWhiteSpace = "    ";
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastName, phoneWhiteSpace, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker, cuando debería de fallar por que el phone 
        /// del user son espacios en blanco
        /// </summary>
        [Test]
        public void AddWorkerFaildPhoneNull()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el worker
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            const string phoneNull = null;
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastName, phoneNull, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un worker dos veces. 
        /// Haciendo que se agrege la primera vez, pero que no se cree otro con los mismos datos
        /// </summary>
        [Test]
        public void AddWorkerDuplicate()
        {
            // Configuración
            // Incrementamos la id para independizarnos del resto
            workerID += 1;
            long userID = workerID;
            // Introducimos los erroneos y no se debería de crear el worker
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddWorker(userID, name, lastName, phone, address, latitude, longitude);
            // Segunda vez que lo ingresamos
            Database.Instance.AddWorker(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            int counterWorkerMatch = 0;
            foreach (Worker worker in Database.Instance.GetWorkers())
            {
                if (worker.UserID == userID)
                {
                    counterWorkerMatch += 1;
                    Console.WriteLine($"Cotnador vale {counterWorkerMatch}");
                }
            }
            Assert.IsTrue(counterWorkerMatch == 1);
        }

        /// <summary>
        /// Se trata de agregar un usuario cuando ya esta en la lista de admins
        /// </summary>
        [Test]
        public void AddWorkerButUserAlreadyExistAsAdmin()
        {
            // Configuración
            // Como queremos comprobar si la id del worker ya esta en la lista de admins
            // hay que poner ese 
            workerID += 1;
            long userID = workerID;
           long userAdminID = userID;

            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            // Primero lo agregamos como admin
            Database.Instance.AddAdmin(userAdminID);
            // Segunda vez que lo ingresamos
            Database.Instance.AddWorker(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerNotExist = true;
            foreach (Worker worker in Database.Instance.GetWorkers())
            {
                if (worker.UserID == userID)
                {
                    workerNotExist = false; ;
                }
            }
            Assert.IsTrue(workerNotExist);
        }

        /// <summary>
        /// Se trata de agregar un usuario cuando ya esta en la lista de empleadores
        /// </summary>
        [Test]
        public void AddWorkerButUserAlreadyExistAsEmployer()
        {
            // Configuración
            // Como queremos comprobar si la id del worker ya esta en la lista de admins
            // hay que poner ese 
            workerID += 1;
            long userID = workerID;
           long userAdminID = userID;
            //Comportamiento
            // Primero lo agregamos como admin
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            // Segunda vez que lo ingresamos
            Database.Instance.AddWorker(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = Database.Instance.ExistWorker(userID);
            Assert.IsFalse(workerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer
        /// </summary>
        [Test]
        public void AddEmployer()
        {
            // Configuración
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "Junior";
            const string lastName = "Gonazalez";
            const string phone = "099184399";
            const string address = "Blanqueada";
            const double latitude = -34.9999323;
            const double longitude = -50.5734321;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsTrue(employerExist);
        }



        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el nombre 
        /// del user esta vacío
        /// </summary>
        [Test]
        public void AddEmployerFailddNameEmpty()
        {
            // Configuración
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "";
            const string lastName = "Toya";
            const string phone = "095184399";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el nombre 
        /// del user son espacios vacíos
        /// </summary>
        [Test]
        public void AddEmployerFailddNameWhiteSpace()
        {
            // Configuración
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            // Datos
            // Introducimos los erroneos y no se debería de crear el employer
            const string name = "    ";
            const string lastName = "Toya";
            const string phone = "095184399";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el nombre 
        /// del user esta nullo
        /// </summary>
        [Test]
        public void AddEmployerFailddNameNull()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el employer
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = null;
            const string lastName = "Toya";
            const string phone = "095184399";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el apellido 
        /// del user esta vacío
        /// </summary>
        [Test]
        public void AddEmployerFaildLastNameEmpty()
        {
            // Configuración

            // Introducimos los erroneos y no se debería de crear el employer
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "Agustín";
            const string lastName = "";
            const string phone = "095185392";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el apellido 
        /// del user esta solo con espacios en blanco
        /// </summary>
        [Test]
        public void AddEmployerFaildLastNameWhiteSpace()
        {
            // Configuración

            // Introducimos los erroneos y no se debería de crear el employer
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "Agustín";
            const string lastName = "    ";
            const string phone = "095184399";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el apellido 
        /// del user esta nulo
        /// </summary>
        [Test]
        public void AddEmployerFaildLastNameNull()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el employer
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "Agustin";
            const string lastName = null;
            const string phone = "095184399";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el 
        /// phone esta vacío
        /// </summary>
        [Test]
        public void AddEmployerFaildPhoneEmpty()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el employer
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el phone 
        /// del user son espacios en blanco
        /// </summary>
        [Test]
        public void AddEmployerFaildPhonWhiteSpace()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el employer
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "    ";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer, cuando debería de fallar por que el phone 
        /// del user son espacios en blanco
        /// </summary>
        [Test]
        public void AddEmployerFaildPhoneNull()
        {
            // Configuración
            // Introducimos los erroneos y no se debería de crear el employer
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = null;
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerExist = true;
                }
            }
            Assert.IsFalse(employerExist);
        }

        /// <summary>
        /// Se encarga de probar la funcionalidad de agregar un employer dos veces. 
        /// Haciendo que se agrege la primera vez, pero que no se cree otro con los mismos datos
        /// </summary>
        [Test]
        public void AddEmployerDuplicate()
        {
            // Configuración
            // Incrementamos la id para independizarnos del resto
            employerID += 1;
            long userID = employerID;
            // Introducimos los erroneos y no se debería de crear el employer
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            // Segunda vez que lo ingresamos
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            int counterEmployerMatch = 0;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    counterEmployerMatch += 1;
                    Console.WriteLine($"Cotnador vale {counterEmployerMatch}");
                }
            }
            Assert.IsTrue(counterEmployerMatch == 1);
        }

        /// <summary>
        /// Se trata de agregar un usuario cuando ya esta en la lista de admins
        /// </summary>
        [Test]
        public void AddEmployerButUserAlreadyExistAsAdmin()
        {
            // Configuración
            // Como queremos comprobar si la id del employer ya esta en la lista de admins
            // hay que poner ese 
            employerID += 1;
            long userID = employerID;
           long userAdminID = userID;

            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            // Primero lo agregamos como admin
            Database.Instance.AddAdmin(userAdminID);
            // Segunda vez que lo ingresamos
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerNotExist = true;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerNotExist = false; ;
                }
            }
            Assert.IsTrue(employerNotExist);
        }

        /// <summary>
        /// Se trata de agregar un empleador cuando ya esta en la lista de workers
        /// </summary>
        [Test]
        public void AddEmployerButUserAlreadyExistAsWorker()
        {
            // Configuración
            // Como queremos comprobar si la id del employer ya esta en la lista de admins
            // hay que poner ese 
            employerID += 1;
            long userID = employerID;
           long userAdminID = userID;

            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            // Primero lo agregamos como worker
            Database.Instance.AddWorker(userID, name, lastName, phone, address, latitude, longitude);
            // Segunda vez que lo ingresamos
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerNotExist = true;
            foreach (Employer employer in Database.Instance.GetEmployers())
            {
                if (employer.UserID == userID)
                {
                    employerNotExist = false; ;
                }
            }
            Assert.IsTrue(employerNotExist);
        }


        /// <summary>
        /// Se encarga de probar que funcione correctamente el guardar un admin en la database luego de 
        /// creaerlo al seguir creator
        /// </summary>
        [Test]
        public void AddAdmin()
        {
            // Configuración
            adminID += 1;
            long userID = adminID;
            //Comportamiento
            Database.Instance.AddAdmin(userID);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool adminExist = false;
            foreach (Admin admin in Database.Instance.GetAdmins())
            {
                if (admin.UserID == userID)
                {
                    adminExist = true;
                }
            }
            Assert.IsTrue(adminExist);
        }

        /// <summary>
        /// Se encarga de probar que no se agregue el amdin dos veces a la lista
        /// </summary>
        [Test]
        public void AddAdminDuplicate()
        {
            // Configuración
            adminID += 1;
            long userID = adminID;
            //Comportamiento
            // Cramos el admin una primera vez
            Database.Instance.AddAdmin(userID);
            // Lo agregamos la segunda  vez
            Database.Instance.AddAdmin(userID);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos y no se repitan
            int counterAdminrMatch = 0;
            foreach (Admin admin in Database.Instance.GetAdmins())
            {
                if (admin.UserID == userID)
                {
                    counterAdminrMatch += 1;
                }
            }
            Assert.IsTrue(counterAdminrMatch == 1);
        }

        /// <summary>
        /// Prueba que no se agregue el admin si ya esta registrado como un worker
        /// </summary>
        [Test]
        public void AddAdminButUserAlreadyExistAsWorker()
        {
            // Configuración
            adminID += 1;
            long userID = adminID;
            long userWorkerID = userID;
            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            // Primero lo agregamos como worker
            Database.Instance.AddWorker(userWorkerID, name, lastName, phone, address, latitude, longitude);
            // Luego lo agregamos como admin
            Database.Instance.AddAdmin(userID);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos y no se repitan
            bool adminNotExist = true;
            foreach (Admin admin in Database.Instance.GetAdmins())
            {
                if (admin.UserID == userID)
                {
                    adminNotExist = false; ;
                }
            }
            Assert.IsTrue(adminNotExist);
        }

        /// <summary>
        /// Prueba que no se agregue el admin si ya esta registrado como un employer
        /// </summary>
        [Test]
        public void AddAdminButUserAlreadyExistAsEmployerr()
        {
            // Configuración
            adminID += 1;
            long userID = adminID;
            long userEmployerID = userID;

            const string name = "Agustin";
            const string lastName = "Toya";
            const string phone = "095185397";
            const string address = "Cordón";
            const double latitude = -34.9999313;
            const double longitude = -50.5731;
            //Comportamiento
            // Primero lo agregamos como employer
            Database.Instance.AddEmployer(userEmployerID, name, lastName, phone, address, latitude, longitude);
            // Luego lo agregamos como admin
            Database.Instance.AddAdmin(userID);
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos y no se repitan
            bool adminNotExist = true;
            foreach (Admin admin in Database.Instance.GetAdmins())
            {
                if (admin.UserID == userID)
                {
                    adminNotExist = false; ;
                }
            }
            Assert.IsTrue(adminNotExist);
        }

        /// <summary>
        /// Como es un singleton, y creamos un worker antes, al menos va tener un worker, pero si 
        /// solo revisamos ese  al correrlo solo daría una falla. Entonces creamos un Admin, y validamos que 
        /// la lista no este vacía, al mismo tiempo que revisamos que este ese Admin en particular, 
        /// para comprobar que funciona bien
        /// </summary>
        [Test]
        public void GetWorkers()
        {
            // Configuración
            workerID += 1;
            long userID = workerID;
            const string name = "Manolo";
            const string lastName = "Cruz";
            const string phone = "094181399";
            const string address = "Tres Cruces";
            const double latitude = -34.9999223;
            const double longitude = -50.5734121;
            Database.Instance.AddWorker(userID, name, lastName, phone, address, latitude, longitude);
            //Comportamiento
            ReadOnlyCollection<Worker> workers = Database.Instance.GetWorkers();
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool workerExist = false;
            if (workers.Count > 0)
            {
                int i = 0;
                while (!workerExist && i < workers.Count)
                {
                    if (workers[i].UserID == userID)
                    {
                        workerExist = true;
                    }
                    i += 1;

                }
            }
            Assert.IsTrue(workerExist);
        }

        /// <summary>
        /// Como es un singleton, y creamos un empoloyer antes, al menos va tener un empoloyer, pero si 
        /// solo revisamos ese  al correrlo solo daría una falla. Entonces creamos un Admin, y validamos que 
        /// la lista no este vacía, al mismo tiempo que revisamos que este ese Admin en particular, 
        /// para comprobar que funciona bien
        /// </summary>
        [Test]
        public void GetEmployers()
        {
            // Configuración
            workerID += 1;
            long userID = workerID;
            const string name = "Pepe";
            const string lastName = "Lud";
            const string phone = "094184399";
            const string address = "Tres Cruces";
            const double latitude = -34.9999123;
            const double longitude = -50.5734121;
            Database.Instance.AddEmployer(userID, name, lastName, phone, address, latitude, longitude);
            //Comportamiento
            ReadOnlyCollection<Employer> employers = Database.Instance.GetEmployers();
            //Como el user viene de afuera y tiene que ser único condiciona que todos sean distintos
            bool employerExist = false;
            if (employers.Count > 0)
            {
                int i = 0;
                while (!employerExist && i < employers.Count)
                {
                    if (employers[i].UserID == userID)
                    {
                        employerExist = true;
                    }
                    i += 1;

                }
            }
            Assert.IsTrue(employerExist);
        }

        /// <summary>
        /// Como es un singleton, y creamos un admin antes, al menos va tener un admin, pero si 
        /// solo revisamos ese  al correrlo solo daría una falla. Entonces creamos un Admin, y validamos que 
        /// la lista no este vacía, al mismo tiempo que revisamos que este ese Admin en particular, 
        /// para comprobar que funciona bien
        /// </summary>
        [Test]
        public void GetAdmins()
        {
            // Configuración
            adminID += 1;
            long userID = adminID;
            Database.Instance.AddAdmin(userID);
            ReadOnlyCollection<Admin> listAdmins = Database.Instance.GetAdmins();
            bool adminExist = false;
            if (listAdmins.Count > 0)
            {
                int i = 0;
                while (!adminExist && i < listAdmins.Count)
                {
                    if (listAdmins[i].UserID == userID)
                    {
                        adminExist = true;
                    }
                    i += 1;

                }

            }
            Assert.IsTrue(adminExist);
        }


        // Test de nuevos métodos desde la última vez que se hicieron estos 
        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe un worker
        /// cuando se pasa información correcta y se crea el worker
        /// </summary>
        [Test]
        public void MethodExistWorkerWhenWorkerExist()
        {
            workerID +=1;
            long workerTestID = workerID;
            Database.Instance.AddWorker(workerTestID, name, lastName, phone, address, latitude, longitude);
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existWorker = Database.Instance.ExistWorker(workerTestID);
            bool isTrueExistWorker = false;

            foreach(Worker worker in Database.Instance.GetWorkers())
            {
                if(worker.UserID == workerTestID)
                {
                    isTrueExistWorker = true;
                }
            }
            bool successed = isTrueExistWorker && existWorker;
            Assert.IsTrue(successed);
        }

         // Test de nuevos métodos desde la última vez que se hicieron estos 
        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe un worker
        /// cuando se busca si existe un id de un worker que no existe
        /// </summary>
        [Test]
        public void MethodExistWorkerWhenWorkerNotExist()
        {
            // Le damos el último employerID que cargamos, efectivamente no será de un worker
            long workerTestID = employerID;
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existWorker = Database.Instance.ExistWorker(workerTestID);
            bool isTrueExistWorker = false;
            // Esto nunca tendría que encontrarlo por lo cual nunca debería cambiar a true
            foreach(Worker worker in Database.Instance.GetWorkers())
            {
                if(worker.UserID == workerTestID)
                {
                    isTrueExistWorker = true;
                }
            }
            bool notSucccessed = !isTrueExistWorker && !existWorker;
            Assert.IsTrue(notSucccessed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe un employer
        /// cuando se pasa información correcta y se crea el worker
        /// </summary>
        [Test]
        public void MethodExistEmployerWhenEmployerExist()
        {
            employerID +=1;
            long employerTestID = employerID;
            Database.Instance.AddEmployer(employerTestID, name, lastName, phone, address, latitude, longitude);
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existEmployer = Database.Instance.ExistEmployer(employerTestID);
            bool isTrueExistEmployer = false;

            foreach(Employer employer in Database.Instance.GetEmployers())
            {
                if(employer.UserID == employerTestID)
                {
                    isTrueExistEmployer = true;
                }
            }
            bool successed = isTrueExistEmployer && existEmployer;
            Assert.IsTrue(successed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe un employer
        /// cuando se busca si existe un id de un worker que no existe
        /// </summary>
        [Test]
        public void MethodExistEmployerWhenEmployerNotExist()
        {
            // Le cargamos el último id de un worker, por lo que no podrá ser un employer
            long employerTestID = workerID;
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existEmployer = Database.Instance.ExistEmployer(employerTestID);
            bool isTrueExistEmployer = false;
            // Esto nunca tendría que encontrarlo por lo cual nunca debería cambiar a true
            foreach(Employer employer in Database.Instance.GetEmployers())
            {
                if(employer.UserID == employerTestID)
                {
                    isTrueExistEmployer = true;
                }
            }
            bool notSucccessed = !isTrueExistEmployer && !existEmployer;
            Assert.IsTrue(notSucccessed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe un admin
        /// cuando se pasa información correcta y se crea el worker
        /// </summary>
        [Test]
        public void MethodExistAdminWhenAdminrExist()
        {
            adminID +=1;
            long adminTestID = adminID;
            Database.Instance.AddAdmin(adminTestID);
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existAdmin = Database.Instance.ExistAdmin(adminTestID);
            bool isTrueExistAdmin = false;

            foreach(Admin admin in Database.Instance.GetAdmins())
            {
                if(admin.UserID == adminTestID)
                {
                    isTrueExistAdmin = true;
                }
            }
            bool successed = isTrueExistAdmin && existAdmin;
            Assert.IsTrue(successed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe un admin
        /// cuando se busca si existe un id de un worker que no existe
        /// </summary>
        [Test]
        public void MethodExistAdminWhenAdminNotExist()
        {   
            // Le cargamos el último id de un worker, por lo que no podrá ser un admin
            long adminTestID = workerID;
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existEmployer = Database.Instance.ExistAdmin(adminTestID);
            bool isTrueExistAdmin = false;
            // Esto nunca tendría que encontrarlo por lo cual nunca debería cambiar a true
            foreach(Admin admin in Database.Instance.GetAdmins())
            {
                if(admin.UserID == adminTestID)
                {
                    isTrueExistAdmin = true;
                }
            }
            bool notSucccessed = !isTrueExistAdmin && !existEmployer;
            Assert.IsTrue(notSucccessed);
        }


        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe una oferta de trabajo
        /// </summary>
        [Test]
        public void MethodExistWorkOfferWhenOfferExist()
        {
            workerID += 1;
            long workerOwnerID = workerID;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Primero debemos crear una nueva oferta de trabajo, con algún dato diferente, si ingresamos una con la info de las anteriores, va a rebotar en 
            // AddWorkOffer por estar repetidas
            const string description5 = "Quinta oferta de trabajo";
            int offer5ID = Database.Instance.UltimateIDWorkOffer + 1;
            Database.Instance.AddWorkOffer(description5, currency, price, workerOwnerID, categories, durationWorkOffer);
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existWorkOffer = Database.Instance.ExistWorkOffer(offer5ID);
            bool isTrueExistWorkOffer = false;
            // Esto tendría que encontrarlo por lo cual  debería cambiar a true
            foreach(WorkOffer offer in Database.Instance.GetAllWorkOffers())
            {
                if(offer.Identify == offer5ID)
                {
                    isTrueExistWorkOffer = true;
                }
            }
            bool succcessed = isTrueExistWorkOffer && existWorkOffer;
            Assert.IsTrue(succcessed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que no existe una oferta de trabajo con ese
        /// identificador en el sistema
        /// </summary>
        [Test]
        public void MethodExistWorkOfferWhenOfferNotExist()
        {
            // Identificador que no existe de una oferta de trabajo
            int identifyNotExist = Database.Instance.UltimateIDWorkOffer * 2;

            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            bool existWorkOffer = Database.Instance.ExistWorkOffer(identifyNotExist);
            bool isTrueExistWorkOffer = false;
            // Esto tendría que no encontrarlo por lo cual nunca debería cambiar a true
            foreach(WorkOffer offer in Database.Instance.GetAllWorkOffers())
            {
                if(offer.Identify == identifyNotExist)
                {
                    isTrueExistWorkOffer = true;
                }
            }
            bool notSucccessed = !isTrueExistWorkOffer && !existWorkOffer;
            Assert.IsTrue(notSucccessed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que existe una oferta de trabajo
        /// </summary>
        [Test]
        public void MethodSearchOfferWhenOfferExist()
        {
            workerID += 1;
            long workerOwnerID = workerID;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Primero debemos crear una nueva oferta de trabajo, con algún dato diferente, si ingresamos una con la info de las anteriores, va a rebotar en 
            // AddWorkOffer por estar repetidas
            const string description6 = "Sexta oferta de trabajo";
            int offer6ID = Database.Instance.UltimateIDWorkOffer + 1;
            Database.Instance.AddWorkOffer(description6, currency, price, workerOwnerID, categories, durationWorkOffer);
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            WorkOffer workOffer = Database.Instance.SearchWorkOffer(offer6ID);
            // Ahora hay que comprobar que no sea null
            bool responseNotNull = workOffer != null;
            bool isTrueExistWorkOffer = false;
            // Como se agregó, este foreach  debería encontrarlo, por ende se comprueba el funcionamiento de 
            // que el método devuelve el objeto cuando lo encuentra
            foreach(WorkOffer offer in Database.Instance.GetAllWorkOffers())
            {
                if(offer.Identify == offer6ID)
                {
                    isTrueExistWorkOffer = true;
                }
            }
            bool succcessed = isTrueExistWorkOffer && responseNotNull;
            Assert.IsTrue(succcessed);
        }

         /// <summary>
        /// Test probando el método de la base de datos de que devuelve el objeto de 
        /// la base de datos cuando este no existe. 
        /// </summary>
        [Test]
        public void MethodSearchOfferWhenOfferNotExist()
        {
            // Identificador que no existe de una oferta de trabajo
            int identifyNotExist = Database.Instance.UltimateIDWorkOffer * 2;

            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            WorkOffer workOffer = Database.Instance.SearchWorkOffer(identifyNotExist);
            // Ahora hay que comprobar que no sea null
            bool responseNotNull = workOffer != null;
            bool isTrueExistWorkOffer = false;
            // Como nunca se agregó, este foreach no debería encontrarlo, por ende se comprueba el funcionamiento de 
            // que el método devuelve null cuando no lo encuentra
            foreach(WorkOffer offer in Database.Instance.GetAllWorkOffers())
            {
                if(offer.Identify == identifyNotExist)
                {
                    isTrueExistWorkOffer = true;
                }
            }
            bool notSucccessed = !isTrueExistWorkOffer && !responseNotNull;
            Assert.IsTrue(notSucccessed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve que el objeto worker si es 
        /// que existe un worker que tenga de userID el que se esta especificando
        /// </summary>
        [Test]
        public void MethodSearchWorkerWhenWorkerExist()
        {
            workerID +=1;
            long workerTestID = workerID;
            Database.Instance.AddWorker(workerTestID, name, lastName, phone, address, latitude, longitude);
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            Worker workerReturned = Database.Instance.SearchWorker(workerTestID);
            bool returnIsNotNull = workerReturned != null;
            bool isTrueExistWorker = false;

            foreach(Worker worker in Database.Instance.GetWorkers())
            {
                if(worker.UserID == workerTestID)
                {
                    isTrueExistWorker = true;
                }
            }
            bool successed = isTrueExistWorker && returnIsNotNull;
            Assert.IsTrue(successed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve el objeto worker cuando lo encuentra, si no lo 
        /// encuentra devuelve null, que es lo que va a suceder aquí
        /// </summary>
        [Test]
        public void MethodSerachWorkerWhenWorkerNotExist()
        {
            // Le cargamos el último id de un employer, por lo que no podrá ser un worker
            long workerTestID = employerID;
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            Worker  workerReturned = Database.Instance.SearchWorker(workerTestID);
            // Como no existe un worker con ese id, tiene que devolver si funciona bien un objeto null
            bool workerNotNull = workerReturned != null;
            bool isTrueExistWorker = false;
            // Esto nunca tendría que encontrarlo por lo cual nunca debería cambiar a true
            foreach(Worker worker in Database.Instance.GetWorkers())
            {
                if(worker.UserID == workerTestID)
                {
                    isTrueExistWorker = true;
                }
            }
            bool notSucccessed = !isTrueExistWorker && !workerNotNull;
            Assert.IsTrue(notSucccessed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que devuelve el objeto employer
        /// al pasarle un id de un employer que existe
        /// </summary>
        [Test]
        public void MethodSearchEmployerWhenEmployerExist()
        {
            employerID +=1;
            long employerTestID = employerID;
            Database.Instance.AddEmployer(employerTestID, name, lastName, phone, address, latitude, longitude);
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            Employer employerReturned = Database.Instance.SearchEmployer(employerTestID);
            bool employerNotNull = employerReturned != null;
            bool isTrueExistEmployer = false;

            foreach(Employer employer in Database.Instance.GetEmployers())
            {
                if(employer.UserID == employerTestID)
                {
                    isTrueExistEmployer = true;
                }
            }
            bool successed = isTrueExistEmployer && employerNotNull;
            Assert.IsTrue(successed);
        }

        /// <summary>
        /// Prueba el método de la clasde database que devulve un employer cuando encuentra un employer 
        /// que tenga la id que se le espcecifica. En este caso se prueba de que el método devulva null
        /// cuando se le pasa una id que no corresponde a un employer
        /// </summary>
        [Test]
        public void MethodSearchEmployerWhenEmployerNotExist()
        {
            // Le cargamos el último id de un worker, por lo que no podrá ser un employer
            long employerTestID = workerID;
            // El método devolverá true, pero coprobamos de la forma larga que este resultado es veridico
            Employer employerReturned = Database.Instance.SearchEmployer(employerTestID);
            // Ahora evaluamos si es o no null, en este caso debería ser null
            bool employerIsNotNull = employerReturned != null;
            bool isTrueExistEmployer = false;
            // Esto nunca tendría que encontrarlo por lo cual nunca debería cambiar a true
            foreach(Employer employer in Database.Instance.GetEmployers())
            {
                if(employer.UserID == employerTestID)
                {
                    isTrueExistEmployer = true;
                }
            }
            bool notSucccessed = !isTrueExistEmployer && !employerIsNotNull;
            Assert.IsTrue(notSucccessed);
        }

        /// <summary>
        /// Test probando el método de la base de datos de que revisa si hay una workOffer que ya tenga todo los datos que 
        /// se le estan pasando, en la base de datos. 
        /// </summary>
        [Test]
        public void MethodMatchOfferWhenOfferExist()
        {
            workerID += 1;
            long workerOwnerID = workerID;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Primero debemos crear una nueva oferta de trabajo, con algún dato diferente, si ingresamos una con la info de las anteriores, va a rebotar en 
            // AddWorkOffer por estar repetidas
            const string description7 = "Septima oferta de trabajo";
            int offer7ID = Database.Instance.UltimateIDWorkOffer + 1;
            Database.Instance.AddWorkOffer(description7, currency, price, workerOwnerID, categories, durationWorkOffer);
            int matchs = Database.Instance.MatachWorkOffer(description7, currency, price, workerOwnerID, categories, durationWorkOffer);
            // El método deberá devolver 1
            WorkOffer workOffer = Database.Instance.SearchWorkOffer(offer7ID);
            Assert.IsTrue(matchs == 1);
        }

         /// <summary>
         /// Test probando el método de la base de datos de que revisa si hay una workOffer que ya tenga todo los datos que 
        /// se le estan pasando, en la base de datos.  En este caso deberá devolver 0 por que no se agrego ninguna oferta con esos datos
        /// </summary>
        [Test]
        public void MethodMatchOfferWhenOfferNotExist()
        {
            // Configuración
            workerID += 1;
            long workerOwnerID = workerID;
            // Cramos el worker por las dudas
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            const string description8 = "No va a haber ninguna oferta con esta descripción _________";

            // Ejecutamos comportamiento
            // El método deberá devolver 0 por que no había ninguna oferta así
            int matchs = Database.Instance.MatachWorkOffer(description8, currency, price, workerOwnerID, categories, durationWorkOffer);

            // Comprobamos
            // Ahora hay que comprobar que haya dado 0
            Assert.IsTrue(matchs == 0);
        }

        /// <summary>
        /// Test que prueba el método limpiar la lista de workers
        /// </summary>
        [Test]
        public void ClearWorkers()
        {
            // Configuración
            // Para asegurarnos que no este vacía desde el inicio, le agregamos un worker
            workerID +=1;
            long workerIDTest = workerID;
            Database.Instance.AddWorker(workerIDTest, name, lastName, phone, address, latitude, latitude);
            // Comprobamos que eso haya salido bien
            int cantWorkers = Database.Instance.GetWorkers().Count;
            Assert.IsTrue(cantWorkers > 0);
            // Ahora ejecutamos el comportamiento de eliminar todo en la lista, llamando a ClearWorkers()
            Database.Instance.ClearWorkers();
            // Comprobamos que no haya quedado nada
            cantWorkers = Database.Instance.GetWorkers().Count;
            Assert.IsTrue(cantWorkers == 0);
        }

        /// <summary>
        /// Test que prueba el método limpiar la lista de employers
        /// </summary>
        [Test]
        public void ClearEmployers()
        {
            // Configuración
            // Para asegurarnos que no este vacía desde el inicio, le agregamos un employer
            employerID +=1;
            long employerIDTest = employerID;
            Database.Instance.AddEmployer(employerIDTest, name, lastName, phone, address, latitude, latitude);
            // Comprobamos que eso haya salido bien
            int cantEmployers = Database.Instance.GetEmployers().Count;
            Assert.IsTrue(cantEmployers > 0);
            // Ahora ejecutamos el comportamiento de eliminar todo en la lista, llamando a ClearEmployers()
            Database.Instance.ClearEmployers();
            // Comprobamos que no haya quedado nada
            cantEmployers = Database.Instance.GetEmployers().Count;
            Assert.IsTrue(cantEmployers == 0);
        }

        /// <summary>
        /// Test que prueba el método limpiar la lista de admins
        /// </summary>
        [Test]
        public void ClearAdmins()
        {
            // Configuración
            // Para asegurarnos que no este vacía desde el inicio, le agregamos un admin
            adminID +=1;
            long adminIDTest = adminID;
            Database.Instance.AddAdmin(adminIDTest);
            // Comprobamos que eso haya salido bien
            int cantAdmins = Database.Instance.GetAdmins().Count;
            Assert.IsTrue(cantAdmins > 0);
            // Ahora ejecutamos el comportamiento de eliminar todo en la lista, llamando a ClearAdmins()
            Database.Instance.ClearAdmins();
            // Comprobamos que no haya quedado nada
            cantAdmins = Database.Instance.GetAdmins().Count;
            Assert.IsTrue(cantAdmins == 0);
        }

         /// <summary>
        /// Test que prueba el método limpiar la lista de ofertas de trabajo
        /// </summary>
        [Test]
        public void ClearWorkOffers()
        {
            // Configuración
            // Para asegurarnos que no este vacía desde el inicio, le agregamos una oferta
            workerID += 1;
            long workerOwnerID = workerID;
            Database.Instance.AddWorker(workerOwnerID, name, lastName, phone, address, latitude, longitude);
            // Primero debemos crear una nueva oferta de trabajo, con algún dato diferente, si ingresamos una con la info de las anteriores, va a rebotar en 
            // AddWorkOffer por estar repetidas
            const string description9 = "Novena oferta de trabajo";
            Database.Instance.AddWorkOffer(description9, currency, price, workerOwnerID, categories, durationWorkOffer);
            // Comprobamos que eso haya salido bien
            int cantWorkOffers = Database.Instance.GetAllWorkOffers().Count;
            Assert.IsTrue(cantWorkOffers > 0);
            // Ahora ejecutamos el comportamiento de eliminar todo en la lista, llamando a ClearWorkers()
            Database.Instance.ClearWorkOffers();
            // Comprobamos que no haya quedado nada
            cantWorkOffers = Database.Instance.GetAllWorkOffers().Count;
            Assert.IsTrue(cantWorkOffers == 0);
        }

        /// <summary>
        ///  Prueba que se limpie la lista de categorías
        /// </summary>
        [Test]
        public void ClearCategories()
        {
            // Primero le cargamos una categoría para asegurarnnos que haya algo
            Database.Instance.AddCategory("Pruebas");

            // Cromprobamos que haya algo, o sea que no este vacía
            int cantCategories = Database.Instance.GetAllCategories().Count;
            Assert.IsTrue(cantCategories > 0);

            // Ahora si eso dio true, vamos a seguir y vaciamos la lista
            Database.Instance.ClearCategories();
            cantCategories = Database.Instance.GetAllCategories().Count;
            Assert.IsTrue(cantCategories == 0);
            
        }



    }
}