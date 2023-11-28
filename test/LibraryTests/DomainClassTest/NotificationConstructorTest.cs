using NUnit.Framework;
using Exceptions;
using ClassLibrary;

// Test responsabilidad de Emiliano Labarthe y Agustín Toya.

namespace Tests
{
    /// <summary>
    /// Test que se encarga de probar el constructor de Notification.
    /// </summary>
    [TestFixture]
    public class NotificationConstructorTest
    {
        /// <summary>
        /// Probamos a construir las notifications con los datos adecuados.
        /// </summary>
        [Test]
        public void ConstructNotification()
        {   
            // Creo un employer que envíe una notificación que ue genere la notificación
            long senderID1 = 900;
            Database.Instance.AddEmployer(senderID1, "Manu", "Escobar", "099488588", "Cancún", 5, 5);
            // Creamos un admin para el caso de la notificación de que se eliminio la oferta de trabajo
            long adminID = 910;
            Database.Instance.AddAdmin(adminID);
            // Lo que esperamos que llegue a sus properties.
            const string expectedMessage1 = "Disculpa, di de baja tu oferta por que es muy cara para estar ofrenciendo cortar el pasto";
            long expectedSenderID1= adminID;
            const int expectedNotificationID1 = 711;
            const string expectedMessage2 = "Buenas, ¿Puedes venir hoy a terminar la pared del fondo?";
            long expectedSenderID2= senderID1;
            const int expectedNotificationID2 = 712;
            const string expectedMessage3 = "Si claro, no tengo problema, en la tarde voy a terminar tu pared";
            long expectedSenderID3= senderID1;
            const int expectedNotificationID3 = 713;
            // Como tiene varias razones o asuntos posibles, vamos a crear varias notificacione
            // probando que el constructor no falle en ninguno de estos posibles estados

            // Construimos
            Notification notification1 = new Notification("Disculpa, di de baja tu oferta por que es muy cara para estar ofrenciendo cortar el pasto", adminID, 711, Notification.Reasons.AdminDeleteWorkOffer);
            Notification notification2 = new Notification("Buenas, ¿Puedes venir hoy a terminar la pared del fondo?", senderID1, 712, Notification.Reasons.EmployerWantContactWorker);
            Notification notification3 = new Notification("Si claro, no tengo problema, en la tarde voy a terminar tu pared", senderID1, 713, Notification.Reasons.WorkerResponseAnEmployer);

            // Comprobamos que los datos ingresados lleguen bien a destino la primera notificación
            Assert.AreEqual(expectedMessage1, notification1.Message);
            Assert.AreEqual(expectedSenderID1, notification1.SenderID);
            Assert.AreEqual(expectedNotificationID1, notification1.NotificationID);
            Assert.AreEqual(Notification.Reasons.AdminDeleteWorkOffer, notification1.NotificationReasons);

            // Comprobamos la segunda notificación
            Assert.AreEqual(expectedMessage2, notification2.Message);
            Assert.AreEqual(expectedSenderID2, notification2.SenderID);
            Assert.AreEqual(expectedNotificationID2, notification2.NotificationID);
            Assert.AreEqual(Notification.Reasons.EmployerWantContactWorker, notification2.NotificationReasons);

            // Comprobamos la tercera notificación
            Assert.AreEqual(expectedMessage3, notification3.Message);
            Assert.AreEqual(expectedSenderID3, notification3.SenderID);
            Assert.AreEqual(expectedNotificationID3, notification3.NotificationID);
            Assert.AreEqual(Notification.Reasons.WorkerResponseAnEmployer, notification3.NotificationReasons);
        }

        /// <summary>
        /// Se probarán los tres posibles casos en los que el mensaje esta mal, y deberá fallar por cada uno
        /// </summary>
        [Test]
        public void ConstructFailedNotificationMessageInvalid()
        {
            // Como comprobamos en el test pasado que todo funcionaba correctamente 
            // con cuaquier reason, además no es un tipo que pueda ponerse fuera de los que se establecieron
            // no es algo que pueda probarse que este mal   
            const string messageNull = null;
            const string messageEmpty = "";
            const string messageWhitSpace = "    ";
            const string responseInvalidMessage = "El mensaje no puede ser nulo, vacío o ser espacios en blanco";

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>().And.Message.EqualTo(responseInvalidMessage),
            delegate {new Notification(messageNull,0,0, Notification.Reasons.WorkerResponseAnEmployer);});

            // Revisamos el caso en el que el mensje esta vacío
            Assert.Throws(Is.TypeOf<Check.PreconditionException>().And.Message.EqualTo(responseInvalidMessage),
            delegate {new Notification(messageEmpty,0,0, Notification.Reasons.WorkerResponseAnEmployer);});

            // Revisamos el caso en el que el mensaje es espacios en blanco solamente. 
               Assert.Throws(Is.TypeOf<Check.PreconditionException>().And.Message.EqualTo(responseInvalidMessage),
            delegate {new Notification(messageWhitSpace,0,0, Notification.Reasons.WorkerResponseAnEmployer);});
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal el userID del que genera la notificación
        /// </summary>
        [Test]
        public void ConstructFailedMessageWrongSenderID()
        {
            // Configuración
            const long wrongSenderID1  = 0;
            const long wrongSenderID2  = -761;
            const string messageValid = "Hola, quería contactarme contigo para que me hicieras el trabajo de ...";
            const string messageReponseFaild = "El id del user que envio un mensaje o provoco una notificación, no puede ser menor a 1";
            // También hay que comprobar el caso en el que sea un id valido, mayor a 0, pero al no estar cargado en la base 
            // de datos de un error
            const long userIDForStranger = 97312;
            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate {  new Notification(messageValid, wrongSenderID1 , 0, Notification.Reasons.WorkerResponseAnEmployer); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate {  new Notification(messageValid, wrongSenderID2 , 0, Notification.Reasons.WorkerResponseAnEmployer); });

                   // Ahora probamos que no haga el objeto por que el userId no corresponde a ningún user del sistema
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo("El userID del calificador no corresponde a ningún worker ni a ningún employer"),
            delegate {  new Rating(userIDForStranger, 0, 0); }); 
        }

        /// <summary>
        /// Prueba que levante las excepciones correpondientes a las precondiciones
        /// al estar mal el id de la notificación, el cual se va incrementando en la clase User 
        /// por cada notificación que recibe
        /// </summary>
        [Test]
        public void ConstructFailedMessageWrongID()
        {
            // Configuración
            const long senderID  = 190;
            const int wrongNotificationID1 = 0;
            const int wrongNotificationID2  = -121;
            const string messageValid = "Hola, quería contactarme contigo para que me hicieras el trabajo de ...";
            const string messageReponseFaild = "El identificador de las ofertas no puede ser menor a 1";

            // Comportamiento y Comprobaciones

            //Además de comprobar que levante una exepción, comprobamos que el mensaje que devuelva sea el mismo
            // que el corresponde a que falle el dato que estamos probando
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate {  new Notification(messageValid, senderID , wrongNotificationID1, Notification.Reasons.WorkerResponseAnEmployer); });

            // Ahora probamos con que sea menor a 0
            Assert.Throws(Is.TypeOf<Check.PreconditionException>()
                 .And.Message.EqualTo(messageReponseFaild),
            delegate {  new Notification(messageValid, senderID , wrongNotificationID2, Notification.Reasons.WorkerResponseAnEmployer); });
        }

    }
}