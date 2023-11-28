using NUnit.Framework;
using ClassLibrary;
using Exceptions;

// Test responsabilidad de Emiliano Labarthe y Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Test que se encarga de probar el constructor de Worker.
    /// </summary>
    [TestFixture]
    public class WorkerConstructorTest
    {
        /// <summary>
        /// Probamos a construir los workers con los datos adecuados.
        /// </summary>
        [Test]
        public void ConstructWorker()
        {
            // Crea la instancia y determina los valores esperados
            Worker workerTest = new Worker(11, "Agus", "Troya", "095321002", "Migues", 13, 24);
            const long expectedUserID = 11;
            const string expectedName = "Agus";
            const string expectedLastName = "Troya";
            const string expectedPhoneNumber = "095321002";
            const string expectedLocationName = "Migues";
            const double expectedLatitude = 13;
            const double expectedLongitude = 24;

            // Comprueba
            Assert.AreEqual(workerTest.UserID, expectedUserID);
            Assert.AreEqual(workerTest.Name, expectedName);
            Assert.AreEqual(workerTest.LastName, expectedLastName);
            Assert.AreEqual(expectedPhoneNumber, workerTest.Phone);
            Assert.AreEqual(workerTest.Location.FormatAddress, expectedLocationName);
            Assert.AreEqual(workerTest.Location.Latitude, expectedLatitude);
            Assert.AreEqual(workerTest.Location.Longitude, expectedLongitude);
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal el userID
        /// </summary>
        [Test]
        public void ConstructFailedWorkerByWrongID()
        {
            // Configuración
            // El resto de datos para crear el worker son arbitrarios, no se van a llegar
            // a comprobar, por lo que los especificamos
            // en la llamada al constructor
            const long wrongUserID1  = 0;
            const long wrongUserID2  = -761;

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID no puede ser nunca menor o igual a cero"),
            delegate {  new Worker(wrongUserID1, "", "", "", "", 0, 0); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID no puede ser nunca menor o igual a cero"),
            delegate {  new Worker(wrongUserID2, "", "", "", "", 0, 0); });
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal el nombre, ya sea por estar vacío, ser espacios en blanco o ser nulo
        /// </summary>
        [Test]
        public void ConstructFailedWorkerByWrongName()
        {
            // Configuración
            // Los únicos datos por los que tenemos que preocuparnos son por el UserID (ya que si no es valido 
            // no llegaremos al nombre), y el nombre
            const long validUserID = 2;
            const string emptyName  = "";
            const string whiteSpacesName  = "   ";
            const string nullName  = null;

            // Comportamiento y Comprobaciones
            
            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El nombre no puede ser nulo, estar vació o ser espacios en blanco"),
            delegate {  new Worker(validUserID, emptyName, "", "", "", 0, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El nombre no puede ser nulo, estar vació o ser espacios en blanco"),
            delegate {  new Worker(2, whiteSpacesName,"", "", "", 0, 0); });

              // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El nombre no puede ser nulo, estar vació o ser espacios en blanco"),
            delegate {  new Worker(validUserID, nullName,"", "", "", 0, 0); });
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal el apellido, ya sea por estar vacío, ser espacios en blanco o ser nulo
        /// </summary>
        [Test]
        public void ConstructFailedWorkerByWrongLastname()
        {
            // Configuración
            // Los únicos datos por los que tenemos que preocuparnos son por el UserID y el nombre (ya que si no son validos 
            // no llegaremos al apellido), y el apellido
            const long validUserID = 2;
            const string validName = "José";
            const string emptyLastname  = "";
            const string whiteSpacesLastname  = "   ";
            const string nullLastname  = null;

            // Comportamiento y Comprobaciones
            
            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El apellido no puede ser nulo, estar vació o ser espacios en blanco"),
            delegate {  new Worker(validUserID, validName, emptyLastname, "", "", 0, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El apellido no puede ser nulo, estar vació o ser espacios en blanco"),
            delegate {  new Worker(validUserID, validName, whiteSpacesLastname, "", "", 0, 0); });

              // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El apellido no puede ser nulo, estar vació o ser espacios en blanco"),
            delegate {  new Worker(validUserID, validName, nullLastname,"", "", 0, 0); });
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal el celular, ya sea por estar vacío, ser espacios en blanco o ser nulo
        /// </summary>
        [Test]
        public void ConstructFailedWorkerByWrongPhone()
        {
            // Configuración
            // Los datos por los que tenemos que preocuparnos son por el UserID, el nombre, el apellido (ya que si no son validos 
            // no llegaremos al phone), y el phone
            const long validUserID = 2;
            const string validName = "José";
            const string validLastName = "Perez";
            const string phoneEmpty = "";
            const string phoneWhiteSpaces = "  ";
            const string phoneNull = null;
            // Hay otros casos en los que no es valido, como que no tenga algo además de números o que no tenga 9 digitos
            const string phoneNotHaveOnlyNumbers = "Hola12345";
            const string phoneLenghtMore9 = "123456789012";
            const string phoneLenghtLess9 = "Hola12";
            // Otro caso es que arranque en una denominación distinta de 09
            const string phoneWrong = "129321874";
            // También no puede ser un 0 el tercer digito de izquierda a derecha
            const string phoneWrong1 = "090541871";

            const string messageResponseNullEmptyOrWhiteSpace = "El celular no puede ser nulo, estar vació o ser espacios en blanco";
            const string messageResponseFormatInvalid = "El celular que ingreso no tiene un formato valido. El formato valido es: 09[1-9]xxxxxx. Cambiando las x por los números correspondientes";
            

            // Comportamiento y Comprobaciones
            
            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName, phoneEmpty, "", 0, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName, phoneWhiteSpaces, "", 0, 0); });

              // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName, phoneNull, "", 0, 0); });

            // Ahora probamos el caso en el que no tenga solo números pero tenga 9 caracteres
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFormatInvalid),
            delegate {  new Worker(validUserID, validName, validLastName, phoneNotHaveOnlyNumbers, "", 0, 0); });

            // Ahora probamos el caso en el que tenga solo números pero sean más de nueve
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFormatInvalid),
            delegate {  new Worker(validUserID, validName, validLastName, phoneLenghtMore9, "", 0, 0); });

            // Ahora probamos el caso en el que tenga solo números pero sean menos de nueve
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFormatInvalid),
            delegate {  new Worker(validUserID, validName, validLastName, phoneLenghtLess9, "", 0, 0); });

            // Ahora probamos el caso en el que tenga solo números pero sean menos de nueve
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFormatInvalid),
            delegate {  new Worker(validUserID, validName, validLastName, phoneWrong, "", 0, 0); });

            // Ahora probamos el caso en el que tenga solo números pero sean menos de nueve
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseFormatInvalid),
            delegate {  new Worker(validUserID, validName, validLastName, phoneWrong1, "", 0, 0); });
        
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la dirección, ya sea por estar vacío, ser espacios en blanco o ser nulo
        /// </summary>
        [Test]
        public void ConstructFailedWorkerByWrongAddress()
        {
            // Configuración
            // Los datos por los que tenemos que preocuparnos son por el UserID, el nombre, el apellido, el celular (ya que si no son validos 
            // no llegaremos al phone), y la dirección
            const long validUserID = 2;
            const string validName = "José";
            const string validLastName = "Perez";
            const string phone = "095187432";
            const string AddressEmpty = "";
            const string AddressWhiteSpaces = "  ";
            const string AddressNull = null;

            const string messageResponseNullEmptyOrWhiteSpace = "Su dirección no puede ser nula, estar vacia o ser espacios en blanco";

            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName, phone, AddressEmpty, 0, 0); });

            // Ahora probamos con que sea espacios en blanco
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName,  phone, AddressWhiteSpaces, 0, 0); });

              // Ahora probamos con que sea nulo
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName, phone, AddressNull,  0, 0); });

            
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la latitud, ya sea estando por debajo de -90 o por arriba de 90
        /// </summary>
        [Test]
        public void ConstructFailedWorkerByWrongLatitude()
        {
            // Configuración
            // Los datos por los que tenemos que preocuparnos son por el UserID, el nombre, el apellido, el celular (ya que si no son validos 
            // no llegaremos al phone), y la dirección
            const long validUserID = 2;
            const string validName = "José";
            const string validLastName = "Perez";
            const string phone = "095187432";
            const string address = "Av. 8 de Octubre 2722";
            const double latitudeMore90 = 90.621;
            const double latitudeLessNegative90 = -91.875;

            const string messageResponseNullEmptyOrWhiteSpace = "La latitud no puede ser menor a -90 ni ser mayor a 90";

            // Probamos el caso en el que la latitud sea mayor al borde superior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName, phone, address, latitudeMore90, 0); });

            // Ahora probamos con que la latitud sea menor al borde inferior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName,  phone, address, latitudeLessNegative90, 0); });           
        }

         /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal la longitud, estando por encima de 180 o por debajo de -180
        /// </summary>
        [Test]
        public void ConstructFailedWorkerByWrongLongitude()
        {
            // Configuración
            // Los datos por los que tenemos que preocuparnos son por el UserID, el nombre, el apellido, el celular (ya que si no son validos 
            // no llegaremos al phone), y la dirección
            const long validUserID = 2;
            const string validName = "José";
            const string validLastName = "Perez";
            const string phone = "095187432";
            const string address = "Av. 8 de Octubre 2722";
            const double latitude = -34.098751;
            const double longitudeMore180 = 190.875;
            const double longitudeLessNegative90 = -187.8325;

            const string messageResponseNullEmptyOrWhiteSpace = "La longitud no puede ser menor a -180 ni mayor a 180";

            // Probamos el caso en el que la longitud sea mayor al borde superior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName, phone, address, latitude, longitudeMore180); });

            // Ahora probamos con que la longitud sea menor al borde inferior
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseNullEmptyOrWhiteSpace),
            delegate {  new Worker(validUserID, validName, validLastName,  phone, address, latitude, longitudeLessNegative90); });           
        }
    }
}