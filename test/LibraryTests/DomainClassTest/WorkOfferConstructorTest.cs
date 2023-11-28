using System.Collections.Generic;
using NUnit.Framework;
using Exceptions;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe y Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Test que se encarga de probar el constructor de WorkOffer
    /// </summary>
    [TestFixture]
    public class WorkOfferConstructorTest
    {
        /// <summary>
        /// Probamos a construir las WorkOffers con los datos adecuados.
        /// </summary>
        [Test]
        public void ConstructWorkOffer()
        {
            List<string> categoriesForTest = new List<string>() { "Jardinería", "Cuidado de hogar" };
            WorkOffer testWorkOffer = new WorkOffer(15, "Jardinero", "UYU", 200, 10, categoriesForTest, 4);
            const int ExpectedIdentify = 15;
            const string ExpectedDescription = "Jardinero";
            const string ExpectedCurrency = "UYU";
            const int ExpectedPrice = 200;
            const long ExpectedOwnerID = 10;
            List<string> ExpectedCategories = categoriesForTest;
            const int ExpectedDuration = 4;

            Assert.AreEqual(testWorkOffer.Identify, ExpectedIdentify);
            Assert.AreEqual(testWorkOffer.Description, ExpectedDescription);
            Assert.AreEqual(testWorkOffer.Currency, ExpectedCurrency);
            Assert.AreEqual(testWorkOffer.Price, ExpectedPrice);
            Assert.AreEqual(testWorkOffer.OwnerWorkerID, ExpectedOwnerID);
            Assert.AreEqual(testWorkOffer.GetCateogories(), ExpectedCategories);
            Assert.AreEqual(testWorkOffer.DurationInDays, ExpectedDuration);
        }

        /// <summary>
        /// Constructor con datos vacíos o negativos para testear las validaciones.
        /// </summary>
        [Test]
        public void ConstructFailedWorkOfferWrongID()
        {
            List<string> categoriesForTest = new List<string>() { "Jardinería", "Cuidado de hogar" };
            const int wrongID1 = 0;
            const int wrongID2 = -2022;
            const string messageResponseFaild = "El indentificador de la oferta debe ser mayor a 0";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(wrongID1, "", "", 0, 0, categoriesForTest, 0); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(wrongID2, "", "", 0, 0, categoriesForTest, 0); });
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la descripción, ya sea por estar vacía, ser espacios en blanco o ser nula
        /// </summary>
        [Test]
        public void ConstructFailedWorkOfferByWrongDescrption()
        {
            // Configuración
            const int validWorkOfferID = 2;
            const string emptyDescrption = "";
            const string whiteSpacesDescrption = "   ";
            const string nullDescrption = null;
            List<string> categoriesForTest = new List<string>() { "Jardinería", "Cuidado de hogar" };
            const string messageReponseFaild = "La descripción no puede ser nula, vacía o ser solo espacios en blanco";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate { new WorkOffer(validWorkOfferID, emptyDescrption, "", 0, 0, categoriesForTest, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate { new WorkOffer(validWorkOfferID, whiteSpacesDescrption, "", 0, 0, categoriesForTest, 0); });

            // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate { new WorkOffer(validWorkOfferID, nullDescrption, "", 0, 0, categoriesForTest, 0); });
        }


        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la moneda, ya sea por estar vacía, ser espacios en blanco, siendo nula o una fuera de las correctas
        /// </summary>
        [Test]
        public void ConstructFailedWorkOfferByWrongCurrency()
        {
            // Configuración
            const int validWorkOfferID = 2;
            const string expectedDescription = "Jardinero";
            const string emptyCurrency = "";
            const string whiteSpacesCurrency = "   ";
            const string nullCurrency = null;
            const string invalidCurrency = "JPY";
            List<string> categoriesForTest = new List<string>() { "Jardinería", "Cuidado de hogar" };
            const string messageReponseFaild = "La moneda no puede ser nula, vacía o ser solo espacios en blanco";
            const string messageReponseFaildInvalidCurrency = "La moneda tiene que ser una de las siguientes: USD', UYU, $U, U$S";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, emptyCurrency, 0, 0, categoriesForTest, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, whiteSpacesCurrency, 0, 0, categoriesForTest, 0); });

            // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, nullCurrency, 0, 0, categoriesForTest, 0); });

            // Ahora probamos con que sea un valor invalido, de los que no estan en la lista de valores posibles. 
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaildInvalidCurrency),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, invalidCurrency, 0, 0, categoriesForTest, 0); });
        }

        /// <summary>
        /// Constructor con datos vacíos o negativos para testear las validaciones.
        /// </summary>
        [Test]
        public void ConstructFailedWorkOfferWrongPrice()
        {
            List<string> categoriesForTest = new List<string>() { "Jardinería", "Cuidado de hogar" };
            const int validWorkOfferID = 2;
            const string expectedDescription = "Jardinero";
            const string currency = "UYU";
            const int wrongPrice1 = 0;
            const int wrongPrice2 = -2022;
            const string messageResponseFaild = "El precio tiene que ser mayor a 0, no se ofrecen servicios gratuitos en esta plataforma";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, wrongPrice1, 0, categoriesForTest, 0); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, wrongPrice2, 0, categoriesForTest, 0); });
        }

        /// <summary>
        /// Constructor con datos vacíos o negativos para testear las validaciones.
        /// </summary>
        [Test]
        public void ConstructFailedWorkOfferWrongOwnerID()
        {
            List<string> categoriesForTest = new List<string>() { "Jardinería", "Cuidado de hogar" };
            const int validWorkOfferID = 2;
            const string expectedDescription = "Jardinero";
            const string currency = "UYU";
            const int price = 900;
            const long wrongOwnerID1 = 0;
            const long wrongOwnerID2 = -1240;
            const string messageResponseFaild = "El id del trabajador que creo la oferta nunca sería menor a 1";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, wrongOwnerID1, categoriesForTest, 0); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, wrongOwnerID2, categoriesForTest, 0); });
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la moneda, ya sea por estar vacía, ser espacios en blanco, siendo nula o una fuera de las correctas
        /// </summary>
        [Test]
        public void ConstructFailedWorkOfferByWrongCategories()
        {
            // Configuración
            const int validWorkOfferID = 2;
            const string expectedDescription = "Jardinero";
            const string currency = "UYU";
            const int price = 9312;
            const long ownerWorkerID = 9012;
            List<string> categoriesCount0 = new List<string>();
            List<string> categoriesWithANull = new List<string>() { null, "Viajes" };
            List<string> categoriesWithAEmtpy = new List<string>() { "Viajes", "", "Bartender" };
            List<string> categoriesWithAWhiteSapaces = new List<string>() { "Viajes", "  ", "Bartender" };
            const string messageReponseFaild = "No pueden haber ofertas sin categorías";
            const string messageReponseFaildInvalidCurrency = "No pueden haber categorías nulas, vacías o que sean espacios en blanco";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, ownerWorkerID, categoriesCount0, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaildInvalidCurrency),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, ownerWorkerID, categoriesWithANull, 0); });

            // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaildInvalidCurrency),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, ownerWorkerID, categoriesWithAEmtpy, 0); });

            // Ahora probamos con que sea un valor invalido, de los que no estan en la lista de valores posibles. 
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaildInvalidCurrency),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, ownerWorkerID, categoriesWithAWhiteSapaces, 0); });
        }

        /// <summary>
        /// Constructor con datos vacíos o negativos para testear las validaciones.
        /// </summary>
        [Test]
        public void ConstructFailedWorkOfferWrongDuration()
        {
            List<string> categoriesForTest = new List<string>() { "Jardinería", "Cuidado de hogar" };
            const int validWorkOfferID = 2;
            const string expectedDescription = "Jardinero";
            const string currency = "UYU";
            const int price = 900;
            const long ownerID = 21220;
            const int wrongDuration1 = 0;
            const int wrongDuration2 = -9012;
            const string messageResponseFaild = "La duración de un trabajo no puede ser menor a un día";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, ownerID, categoriesForTest, wrongDuration1); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFaild),
            delegate { new WorkOffer(validWorkOfferID, expectedDescription, currency, price, ownerID, categoriesForTest, wrongDuration2); });
        }
    }
}