using BotRiveGosh.Data;
using BotRiveGosh.Data.Repository;
using BotRiveGosh.Data.Repository.Interfaces;
using BotRiveGosh.Handlers;
using BotRiveGosh.Scenarios;
using BotRiveGosh.Scenarios.Scenario;
using BotRiveGosh.Services;
using BotRiveGosh.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;
using BotRiveGosh.BackGroundServices;
using BotRiveGosh.Views;
using System.Reflection;

namespace BotRiveGosh
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "RiveGoshService";
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventLog(settings =>
                    {
                        settings.SourceName = "RiveGoshService";
                    });
                })
                .Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Логируйте в файл или другую систему
                File.WriteAllText("crash_log.txt", ex.ToString());

                // Даем время прочитать сообщение перед закрытием
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем бота
            services.AddSingleton<ITelegramBotClient>(sp =>
            {
                string token = Environment.GetEnvironmentVariable("Bot_RiveGosh", EnvironmentVariableTarget.Machine);
                //string token = Environment.GetEnvironmentVariable("BotForTesting", EnvironmentVariableTarget.Machine);
                return new TelegramBotClient(token);
            });

            //база данных
            services.AddSingleton<IDataContextFactory<DBContext>>(sp =>
            {
                string connectionString = ConfigurationManager.ConnectionStrings["localhost"].ConnectionString;
                return new DataContextFactory(connectionString);
            });

            //обработчики сообщений
            services.AddScoped<MainHandler>();
            services.AddScoped<MessageUpdateHandler>();
            services.AddScoped<CallBackUpdateHandler>();
            services.AddScoped<DocumentUpdateHandler>();

            //репозитории
            services.AddScoped<IUserRepository, SqlUserRepository>();
            services.AddScoped<IKpiResultRepository, SqlKpiResultRepository>();
            services.AddScoped<IScenarioContextRepository, InMemoryScenarioContextRepository>();
            services.AddScoped<InMemoryRepository>();
            services.AddScoped<IKpiRepository, SqlKpiRepository>();
            services.AddScoped<IPrizesRepository, SqlPrizesRepository>();
            services.AddScoped<ITodoRepository, SqlTodoRepository>();

            //сервисы
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IKpiResultService, KpiResultService>();
            services.AddScoped<InMemoryStorageService>();
            services.AddScoped<IKpiService, KpiService>();
            services.AddScoped<InputFileService>();
            services.AddScoped<IPrizesService, PrizesService>();
            services.AddScoped<ITodoService, TodoService>();

            //Views
            // Автоматически регистрируем все классы, наследующие от BaseView
            var viewTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseView)));

            foreach (var viewType in viewTypes)
            {
                services.AddScoped(viewType);
            }

            //сценарии
            // Регистрируем каждый сценарий отдельно
            services.AddScoped<IScenario, ShowKpiResultScenario>();
            services.AddScoped<IScenario, AddTodoScenario>();

            services.AddSingleton<IEnumerable<IScenario>>(sp =>
            {
                var kpiResultService = sp.GetRequiredService<IKpiResultService>();
                var todoService = sp.GetRequiredService<ITodoService>();
                var userService = sp.GetRequiredService<IUserService>();
                return new List<IScenario>
                {
                    new ShowKpiResultScenario(kpiResultService),
                    new AddTodoScenario(todoService, userService)
                };
            });

            // Фоновый сервис для бота
            services.AddHostedService<BotBackgroundService>();

            // фоновый сервис для нотификаций
            services.AddHostedService<NotificationBackgroundService>();
        }
    }
}
