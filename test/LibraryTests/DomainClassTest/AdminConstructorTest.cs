using Exceptions;
using NUnit.Framework;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe y Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Test que se encarga de probar el constructor de Admin.
    /// </summary>
    [TestFixture]
    public class AdminConstructorTest
    {
        /// <summary>
        /// Probamos a construir los admins con los datos adecuados.
        /// </summary>
        [Test]
        public void ConstructAdmin()
        {
            // Crea la instancia y determina los valores esperados
            const long expectedUserID = 777;
            // Comportamiento
            Admin adminTest = new Admin(expectedUserID);
            // Comprueba
            Assert.AreEqual(adminTest.UserID, expectedUserID);
           
        }
        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal el userID
        /// </summary>
        [Test]
        public void ConstructFailedAdminByWrongID()
        {
            // Configuración
            const long wrongUserID1  = 0;
            const long wrongUserID2  = -761;

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID no puede ser nunca menor o igual a cero"),
            delegate {  new Admin(wrongUserID1); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID no puede ser nunca menor o igual a cero"),
            delegate {  new Admin(wrongUserID2); });
        }
    }
}