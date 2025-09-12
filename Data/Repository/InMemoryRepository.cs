using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository
{
    public class InMemoryRepository
    {
        
        public async Task<IReadOnlyList<Kpi>> GetKpiStorageAsync(CancellationToken ct = default)
        {
            try
            {
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
                string filePath = Path.Combine(directoryPath, "kpi.csv");

                // Проверяем существование директории и файла
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine("Директория Downloads не существует");
                    return new List<Kpi>().AsReadOnly();
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Файл kpi.csv не существует");
                    return new List<Kpi>().AsReadOnly();
                }

                // Чтение файла
                List<Kpi> kpis = ReadCsvFile(filePath);

                // Удаляем дубликаты
                var uniqueKpis = kpis
                    .Where(k => k != null) // Фильтруем null записи
                    .DistinctBy(k => k.Id)
                    .ToList();

                return uniqueKpis.AsReadOnly();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Операция отменена");
                return new List<Kpi>().AsReadOnly();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении KPI storage: {ex.Message}");
                return new List<Kpi>().AsReadOnly();
            }
        }

        
        private List<Kpi> ReadCsvFile(string filePath)
        {
            var kpiList = new List<Kpi>();
            var lineNumber = 0;
            try
            {
                using var reader = new StreamReader(filePath);
                // Читаем заголовок
                var headerLine = reader.ReadLine();
                lineNumber++;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var values = line.Split(';');

                        if (values.Length >= 9)
                        {

                            var kpi = new Kpi
                            {
                                Id = long.Parse(values[0].Trim('"').Trim()),
                                ShopId = int.Parse(values[1].Trim('"').Trim()),
                                Date = DateTime.Parse(values[2].Trim('"').Trim()),
                                Position = values[3].Trim('"').Trim(),
                                Name = values[4].Trim('"').Trim(),
                                LocalId = values[5].Trim('"').Trim(),
                                TNumber = values[6].Trim('"').Trim(),
                                CardType = values[7].Trim('"').Trim(),
                                Checks = int.TryParse(values[8].Trim('"').Trim(), out int checks) ? checks : 0,
                                SpecialChecks = int.TryParse(values[9].Trim('"').Trim(), out int specialChecks) ? specialChecks : 0
                            };
                            kpiList.Add(kpi);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка в строке {lineNumber}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
            return kpiList;
        }
    }
}
