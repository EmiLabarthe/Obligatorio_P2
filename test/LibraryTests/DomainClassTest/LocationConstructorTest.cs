using Exceptions;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe y Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Test que se encarga de probar el constructor de Location.
    /// </summary>
    [TestFixture]
    public class LocationConstructorTest
    {
        /// <summary>
        /// Probamos a construir los location con los datos adecuados.
        /// </summary>
        [Test]
        public void ConstructLocation()
        {
            // Lo que esperamos que llegue a sus properties.
            const string expectedaddress = "Doha";
            const double expectedLatitude = 70;
            const double expectedLongitude = 140;

            // Construimos
            LocationUser location = new LocationUser("Doha", 70, 140);

            // Comprobamos que los datos ingresados lleguen bien a destino
            Assert.AreEqual(expectedaddress, location.FormatAddress);
            Assert.AreEqual(expectedLatitude, location.Latitude);
            Assert.AreEqual(expectedLongitude, location.Longitude);
        }

        
        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la dirección, ya sea por estar vacío, ser espacios en blanco o ser nulo
        /// </summary>
        [Test]
        public void ConstructFailedEmployerByWrongAddress()
        {
            // Configuración
            const string AddressEmpty = "";
            const string AddressWhiteSpaces = "  ";
            const string AddressNull = null;

            const string messageResponseNullEmptyOrWhiteSpace = "Su dirección no puede ser nula, estar vacia o ser espacios en blanco";

            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new LocationUser( AddressEmpty, 0, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new LocationUser( AddressWhiteSpaces, 0, 0); });

              // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new LocationUser( AddressNull,  0, 0); });

            
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la latitud, ya sea estando por debajo de -90 o por arriba de 90
        /// </summary>
        [Test]
        public void ConstructFailedEmployerByWrongLatitude()
        {
            // Configuración
            const string address = "Av. 8 de Octubre 2722";
            const double latitudeMore90 = 90.621;
            const double latitudeLessNegative90 = -91.875;

            const string messageResponseNullEmptyOrWhiteSpace = "La latitud no puede ser menor a -90 ni ser mayor a 90";

            // Probamos el caso en el que la latitud sea mayor al borde superior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new LocationUser( address, latitudeMore90, 0); });

            // Ahora probamos con que la latitud sea menor al borde inferior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new LocationUser( address, latitudeLessNegative90, 0); });           
        }

         /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la longitud, estando por encima de 180 o por debajo de -180
        /// </summary>
        [Test]
        public void ConstructFailedEmployerByWrongLongitude()
        {
            // Configuración
            const string address = "Av. 8 de Octubre 2722";
            const double latitude = -34.098751;
            const double longitudeMore180 = 190.875;
            const double longitudeLessNegative90 = -187.8325;

            const string messageResponseNullEmptyOrWhiteSpace = "La longitud no puede ser menor a -180 ni mayor a 180";

            // Probamos el caso en el que la longitud sea mayor al borde superior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new LocationUser( address, latitude, longitudeMore180); });

            // Ahora probamos con que la longitud sea menor al borde inferior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new LocationUser( address, latitude, longitudeLessNegative90); });           
        }
    }
}