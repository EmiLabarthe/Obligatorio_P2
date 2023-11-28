using System.Collections.Generic;
using System;
using NUnit.Framework;
using Exceptions;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe y Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Clase que se encarga de probar el constructor de Rating.
    /// </summary>
    [TestFixture]
    public class RatingConstructorTest
    {
        /// <summary>
        /// Probamos a construir las Rates con los datos adecuados.
        /// </summary>
        [Test]
        public void ConstructRate()
        {
            // Hay que pregutarle si estas clases podrían acceder a la base de datos sin problema para comprobar estas cosas
            // o si no es necesario
            //Por si hay que crear todos los datos
            const long workerID = 80;
            const long employerID = 600;
            // Crea la instancia y determina los valores esperados
            Database.Instance.AddEmployer(employerID, "Manu", "Escobar", "099488588", "Cancún", 5, 5);
            Database.Instance.AddWorker(workerID, "Lorenzo", "Gimenez", "099482881", "Buenos Aires", 5.931, 5);
            // Ahora creamos una oferta de trabajo
            List<string> cats = new List<string>(){"VENTAS"};
            Database.Instance.AddCategory("Ventas");
            Database.Instance.AddWorkOffer("Venta de pollos", "UYU", 731, workerID, cats, 4 );
            DateTime expectedStartCountAMonth = DateTime.Today.AddDays(4);
            int expectedWorkOfferID = Database.Instance.UltimateIDWorkOffer;
            //int expectedWorkOfferID = 11;
            // Fin configuración
            // Comportamiento
            Rating testRate = new Rating(employerID, 4, expectedWorkOfferID);

            Assert.AreEqual(testRate.CalificatorID, employerID);
            Assert.AreEqual(testRate.StartCountAMonth, expectedStartCountAMonth);
            Assert.AreEqual(testRate.WorkOfferID, expectedWorkOfferID);
        }
        
        /// <summary>
        /// No se crea el Rating y levanta una excepción al no encontrar valido el userID del calificador
        /// </summary>
        [Test]
        public void FaildConstructRateWrongCalificatorID()
        {
            const long wrongUserID1  = 0;
            const long wrongUserID2  = -761;
            // También hay que comprobar el caso en el que sea un id valido, mayor a 0, pero al no estar cargado en la base 
            // de datos de un error
            const long userIDForStranger = 97312;
            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID del calificador no puede ser menor a 1 nunca"),
            delegate {  new Rating(wrongUserID1, 4, 4); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID del calificador no puede ser menor a 1 nunca"),
            delegate {  new Rating(wrongUserID2, 0,0); });

            // Ahora probamos que no haga el objeto por que el userId no corresponde a ningún user del sistema
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID del calificador no corresponde a ningún worker ni a ningún employer"),
            delegate {  new Rating(userIDForStranger, 0, 0); }); 
        }

        /// <summary>
        /// No se crea el Rating y levanta una excepción al no encontrar valida la duración de la oferta de trabajo
        /// </summary>
        [Test]
        public void FaildConstructRateWrongDuration()
        {
            const long workerID = 80;
            Database.Instance.AddWorker(workerID, "Lorenzo", "Gimenez", "099482881", "Buenos Aires", 5.931, 5);
            const int wrongDuration  = 0;
            const int wrongDuration2  = -761;
            // También hay que comprobar el caso en el que sea un id valido, mayor a 0, pero al no estar cargado en la base 
            // de datos de un error
            const string messageResponseDurationInvalid = "La duración del trabajo no puede ser nunca menor a 1";
            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseDurationInvalid),
            delegate {  new Rating(workerID, wrongDuration, 4); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageResponseDurationInvalid),
            delegate {  new Rating(workerID, wrongDuration2, 0); });

        }

        /// <summary>
        /// No se crea el Rating y levanta una excepción al no encontrar valido el identificador del workOffer
        /// </summary>
        [Test]
        public void FaildConstructRateWrongWorkOfferID()
        {
            
            const int wrongWorKOfferID1  = 0;
            const long employerID = 600;
            const int durationWorkOffer = 2;
            const int wrongWorKOfferID2  = -761;
            // También hay que comprobar el caso en el que sea un id valido, mayor a 0, pero al no estar cargado en la base 
            // de datos de un error
            const int workOfferIDStranger = 97312;
           List<string> cats = new List<string>(){"VENTAS"};
           Database.Instance.AddCategory("Ventas");
           Database.Instance.AddEmployer(employerID, "Manu", "Escobar", "099488588", "Cancún", 5, 5);
            const string messageResponseWorOfferIDLessOne = "El identificador de la oferta de trabajo no puede ser menor a 1 nunca";
            const string messageReponseWorkOfferNoExist = "No existe una oferta de trabajo con este identificador";
            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
            .And.Message.EqualTo(messageResponseWorOfferIDLessOne),
            delegate {  new Rating(employerID, durationWorkOffer, wrongWorKOfferID1); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
            .And.Message.EqualTo(messageResponseWorOfferIDLessOne),
            delegate {  new Rating(employerID, durationWorkOffer, wrongWorKOfferID2); });

            // Ahora probamos que no haga el objeto por que el userId no corresponde a ningún user del sistema
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseWorkOfferNoExist),
            delegate {  new Rating(employerID, durationWorkOffer, workOfferIDStranger); });
        }
    }
}