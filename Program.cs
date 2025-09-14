﻿using BotRiveGosh.Data;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;

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
                    options.ServiceName = "BotRiveGoshService";
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventLog(settings =>
                    {
                        settings.SourceName = "BotRiveGoshService";
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
                string token = "6827114928:AAHf39H0HP_tTJ1El0SFnE-MjJAxQqJJYW8";
                //string token = Environment.GetEnvironmentVariable("Bot_RiveGosh", EnvironmentVariableTarget.User);
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

            // Фоновый сервис для бота
            services.AddHostedService<BotBackgroundService>();

        }
    }
}
