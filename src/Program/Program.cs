using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using System.Collections.Generic;
using ClassLibrary;


namespace Ucu.Poo.TelegramBot
{
    /// <summary>
    /// Un programa que implementa un bot de Telegram.
    /// </summary>
    public class Program
    {
        // La instancia del bot.
        private static TelegramBotClient Bot;

        // El token provisto por Telegram al crear el bot. Mira el archivo README.md en la raíz de este repo para
        // obtener indicaciones sobre cómo configurarlo.
        private static string token;

        // Esta clase es un POCO -vean https://en.wikipedia.org/wiki/Plain_old_CLR_object- para representar el token
        // secreto del bot.
        private class BotSecret
        {
            public string Token { get; set; }
        }

        // Una interfaz requerida para configurar el servicio que lee el token secreto del bot.
        private interface ISecretService
        {
            string Token { get; }
        }

        // Una clase que provee el servicio de leer el token secreto del bot.
        private class SecretService : ISecretService
        {
            private readonly BotSecret _secrets;

            public SecretService(IOptions<BotSecret> secrets)
            {
                _secrets = secrets.Value ?? throw new ArgumentNullException(nameof(secrets));
            }

            public string Token { get { return _secrets.Token; } }
        }


        // Configura la aplicación.
        private static void Start()
        {
            // Lee una variable de entorno NETCORE_ENVIRONMENT que si no existe o tiene el valor 'development' indica
            // que estamos en un ambiente de desarrollo.
            var developmentEnvironment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment =
                string.IsNullOrEmpty(developmentEnvironment) ||
                developmentEnvironment.ToLower() == "development";

            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // En el ambiente de desarrollo el token secreto del bot se toma de la configuración secreta
            if (isDevelopment)
            {
                builder.AddUserSecrets<Program>();
            }

            var configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();

            // Mapeamos la implementación de las clases para  inyección de dependencias
            services
                .Configure<BotSecret>(configuration.GetSection(nameof(BotSecret)))
                .AddSingleton<ISecretService, SecretService>();

            var serviceProvider = services.BuildServiceProvider();
            var revealer = serviceProvider.GetService<ISecretService>();
            token = revealer.Token;
        }



        /// <summary>
        /// Punto de entrada al programa.
        /// </summary>
        public static void Main()
        {
            Start();

            Bot = new TelegramBotClient(token);
            var cts = new CancellationTokenSource();



            // Comenzamos a escuchar mensajes. Esto se hace en otro hilo (en background). El primer método
            // HandleUpdateAsync es invocado por el bot cuando se recibe un mensaje. El segundo método HandleErrorAsync
            // es invocado cuando ocurre un error.
            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions()
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                },
                cts.Token
            );
            // Carga los datos necesarios
            LoadInitialData();
            Console.WriteLine($"Bot is up!");

            // Esperamos a que el usuario aprete Enter en la consola para terminar el bot.
            Console.ReadLine();

            // Terminamos el bot.
            cts.Cancel();
        }

        /// <summary>
        /// Maneja las actualizaciones del bot (todo lo que llega), incluyendo mensajes, ediciones de mensajes,
        /// respuestas a botones, etc. En este ejemplo sólo manejamos mensajes de texto.
        /// </summary>
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                // Sólo respondemos a mensajes de texto
                if (update.Type == UpdateType.Message)
                {
                    await HandleMessageReceived(botClient, update.Message);
                }
            }
            catch (Exception e)
            {
                await HandleErrorAsync(botClient, e, cancellationToken);
            }
        }

        /// <summary>
        /// Maneja los mensajes que se envían al bot a través de handlers de una chain of responsibility.
        /// </summary>
        /// <param name="message">El mensaje recibido</param>
        /// <param name="botClient">Instancia concreta del tipo ITelegramBotClient</param>
        /// <returns></returns>
        private static async Task HandleMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Received a message from {message.From.FirstName} saying: {message.Text}");

            string response = string.Empty;

            //firstHandler.Handle(message, out response);
            // Como solo nos importa usar el userID y el texto en sí, separamos esos datos y se los 
            // pasamos a TranslateToCommand
            long userID = message.From.Id;
            string text = message.Text;
            response = InputInterfacer.Instance.TranslateToCommand(userID, text);


            if (!string.IsNullOrEmpty(response))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, response);
            }
        }

        /// <summary>
        /// Manejo de excepciones. Por ahora simplemente la imprimimos en la consola.
        /// </summary>
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Se encarga de cargar el sistema con algunos datos para que no arranque de 0
        /// </summary>
        public static void LoadInitialData()
        {
            // Datos que vamos a necesitar para varios objetos
            // Workers
            long workerID1 = 1234567890;
            long workerID2 = 1234567891;
            long workerID3 = 1234567892;
            long jpWorkerID = 5406610453;
            // Employers
            long employerID1 = 9876543210;
            long employerID2 = 9876543211;
            long employerID3 = 9876543212;
            // Admins
            long agusAdminID = 1432713680;

            // Agregamos a un admin
            Database.Instance.AddAdmin(agusAdminID);
            // Agregamos los worker
            Database.Instance.AddWorker(workerID1, "Tomás", "Lucero", "094317398", "Dr. Alejandro Gallinal 1862, 11400 Montevideo, Departamento de Montevideo", -34.88405150658886, -56.09976360277566);
            Database.Instance.AddWorker(workerID2, "María", "Belen", "094318998", "Magallanes 2035, 11800 Montevideo, Departamento de Montevideo", -34.89284470480923, -56.18572271588001);
            Database.Instance.AddWorker(workerID3, "Walter", "Silva", "094118998", "4V94+X94, 11400 Montevideo, Departamento de Montevideo", -34.87993773865513, -56.14411387360546);
            Database.Instance.AddWorker(jpWorkerID, "Juan Pablo", "Amorin", "094488167", "Av. Luis Alberto de Herrera 1092, Departamento de Montevideo", -34.87993773865513, -56.14411387360546);
            // Agregamos los employers
            Database.Instance.AddEmployer(employerID1, "Lucas", "Delgado", "098123451", "Av. Italia 4250, 11400 Montevideo, Departamento de Montevideo", -34.88642079268584, -56.118604445865394);
            Database.Instance.AddEmployer(employerID2, "Betina", "Zullen", "091023451", "Av. 8 de Octubre 2492, 11600 Montevideo, Departamento de Montevideo", -34.89193281734301, -56.16320949711448);
            Database.Instance.AddEmployer(employerID3, "Rosa", "Mullin", "091023051", "Comandante Braga, 11600 Montevideo, Departamento de Montevideo", -34.88647850056879, -56.15873265102278);
            // Agregamos catgorías
            Database.Instance.AddCategory("Tralados");
            Database.Instance.AddCategory("Reposteria");
            Database.Instance.AddCategory("Seguridad");
            Database.Instance.AddCategory("Jardinería");
            Database.Instance.AddCategory("Clases particulares");

            // La lista de categorías de cada oferta
            IList<string> categoriesWorkOffer1 = new List<string>(){"TRASLADOS"};
            IList<string> categoriesWorkOffer2 = new List<string>(){"TRASLADOS, SEGURIDAD"};
            IList<string> categoriesWorkOffer3 = new List<string>(){"SEGURIDAD"};
            IList<string> categoriesWorkOffer4 = new List<string>(){"Reposteria"};
            IList<string> categoriesWorkOffer5 = new List<string>(){"Resposteria", "Traslados"};
            IList<string> categoriesWorkOffer6 = new List<string>(){"Clases particulares", "Reposteria"};
            IList<string> categoriesWorkOffer7 = new List<string>(){"Jardinería"};
            // Identificadores de cada oferta
            int identify1 = 0;
            int identify2 = 0;
            int identify3 = 0;
            int identify4 = 0;
            int identify5 = 0;
            int identify6 = 0;
            int identify7 = 0;
            // Agregamos ofertas de trabajo
            Database.Instance.AddWorkOffer("Soy chofer y transporto personas de un lugar a otro durante unas 4 horas al mismo precio", "UYU", 1000, workerID1, categoriesWorkOffer1, 1);
            identify1 = Database.Instance.UltimateIDWorkOffer;
            Database.Instance.AddWorkOffer("Acompaño a depositar y retirar grandes sumas de efecitvo", "UYU", 1000, workerID1, categoriesWorkOffer2, 1);
            identify2 = Database.Instance.UltimateIDWorkOffer;
            Database.Instance.AddWorkOffer("Brindo servicio de guardaspaldas, en un inicio por dos semanas", "UYU", 25000, workerID1, categoriesWorkOffer3, 14);
            identify3 = Database.Instance.UltimateIDWorkOffer;
            Database.Instance.AddWorkOffer("Hago tortas de cumpleaños por encargo", "UYU", 1000, workerID2, categoriesWorkOffer4, 1);
            identify4 = Database.Instance.UltimateIDWorkOffer;
            Database.Instance.AddWorkOffer("Llevo los postres que elaboramos de forma artesanal a domicilio", "UYU", 500, workerID2, categoriesWorkOffer5, 1);
            identify5 = Database.Instance.UltimateIDWorkOffer;
            Database.Instance.AddWorkOffer("Te enseño a hacer deliciosos postres en poco tiempo y de forma sencilla", "U$D", 300, workerID2, categoriesWorkOffer6, 30);
            identify6 = Database.Instance.UltimateIDWorkOffer;
            Database.Instance.AddWorkOffer("Soy experta en podas, dejando formas impresionantes, puedo dejar tu jardín con arbustos decorativos muy atractivos", "USD", 100, workerID3, categoriesWorkOffer7, 5);
            identify7 = Database.Instance.UltimateIDWorkOffer;
            Database.Instance.AddWorkOffer("Soy el mejor que puedas encontrar para dejar tu patio pronto para una fiesta, no solo lo acondiciono, lo restauro cuando esta seco", "USD", 200, jpWorkerID, categoriesWorkOffer7, 3);

            // Le damos de baja alguna, así se genera una notificación al worker
            Database.Instance.DeleteWorkOffer(identify3);
            // Usamos el contacto entre un employer y un worker, y el restante lo dejamos sin notificaciones
            string nameCommand = "/hireemployee";
            InputInterfacer.Instance.TranslateToCommand(employerID1,nameCommand);
            // Ahora le pasamos el identificador de la oferta
            InputInterfacer.Instance.TranslateToCommand(employerID1,identify2.ToString());
            // Repetimos con el ID de Juan Pablo
            InputInterfacer.Instance.TranslateToCommand(employerID1,nameCommand);
            // Ahora le pasamos el identificador de la oferta
            InputInterfacer.Instance.TranslateToCommand(employerID1,jpWorkerID.ToString());
            // Ahora él worker responde, el primero
            InputInterfacer.Instance.TranslateToCommand(workerID1, "/responseaemployer");
            // Envía el id de la notificación
            InputInterfacer.Instance.TranslateToCommand(workerID1, "1");
            // Y ahora su respuesta
            InputInterfacer.Instance.TranslateToCommand(workerID1, "Yes, send");
            // Ahora calificamos a ambos, como no ha pasado el tiempo de las ofertas
            // toca usar el método directamente para poner la fecha simulada
            // Pero antes necesitamos los objetos
            Worker worker1 = Database.Instance.SearchWorker(workerID1);
            Employer employer1 = Database.Instance.SearchEmployer(employerID1);
            // Calificamos al worker
            worker1.ReciveCalification(9, employerID1, identify2, true);
            employer1.ReciveCalification(4, workerID1, identify2, true);





            
        }
    }
}