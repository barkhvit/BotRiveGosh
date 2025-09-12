using BotRiveGosh.Data;
using BotRiveGosh.Data.Repository;
using BotRiveGosh.Data.Repository.Interfaces;
using BotRiveGosh.Handlers;
using BotRiveGosh.Handlers.Commands;
using BotRiveGosh.Scenarios;
using BotRiveGosh.Scenarios.Scenario;
using BotRiveGosh.Services;
using BotRiveGosh.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BotRiveGosh
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);

            // Создаем провайдер сервисов
            var serviceProvider = services.BuildServiceProvider();

            // Создаем CancellationTokenSource
            var cts = new CancellationTokenSource();

            // Регистрируем CancellationToken в контейнере (опционально)
            services.AddSingleton(cts);

            // Перестраиваем провайдер с учетом новых сервисов
            serviceProvider = services.BuildServiceProvider();

            // Создаем и запускаем бота через DI
            using var scope = serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var mainHandler = scope.ServiceProvider.GetRequiredService<MainHandler>();

            //прослушка бота
            try
            {
                botClient.StartReceiving(
                updateHandler: mainHandler.HandleUpdateAsync,
                errorHandler: mainHandler.HandleErrorAsync,
                cancellationToken: cts.Token);

                Console.WriteLine("Бот запущен");

                await Task.Delay(-1, cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем бота
            services.AddSingleton<ITelegramBotClient>(sp =>
            {
                string token = Environment.GetEnvironmentVariable("Bot_RiveGosh", EnvironmentVariableTarget.User);
                return new TelegramBotClient(token);
            });

            //база данных
            services.AddSingleton<IDataContextFactory<DBContext>>(sp =>
            {
                string connectionString = "User ID=postgres;Password=Alekseev4+;Host=localhost;Port=5432;Database=rivegosh;Include Error Detail=true";
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

            //сервисы
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IKpiResultService, KpiResultService>();
            services.AddScoped<InMemoryStorageService>();
            services.AddScoped<IKpiService, KpiService>();
            services.AddScoped<InputFileService>();

            //команды
            services.AddScoped<CommandsForKpi>();
            services.AddScoped<CommandsForMainMenu>();
            services.AddScoped<CommandsForUpdate>();

            //сценарии
            // Регистрируем каждый сценарий отдельно
            services.AddScoped<IScenario, ShowKpiResultScenario>();

            services.AddSingleton<IEnumerable<IScenario>>(sp =>
            {
                var kpiResultService = sp.GetRequiredService<IKpiResultService>();
                return new List<IScenario>
                {
                    new ShowKpiResultScenario(kpiResultService)
                };
            });

        }
    }
}
