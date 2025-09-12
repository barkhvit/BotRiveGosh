using BotRiveGosh.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotRiveGosh.Services
{
    public class InputFileService
    {
        private readonly ITelegramBotClient _botClient;
        public InputFileService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<string> ReadAndSafeFileAsync(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            //получаем Id файла и имя файла
            var fileId = GetFileIdFromUpdate(update);
            if (fileId == null) return "Ошибка Id файла";
            var fileName = GetNameDocumentFromUpdate(update);

            if(fileName == "kpi.csv")
            {
                //скачиваем файл
                var filePath = await DownloadFileAsync(_botClient, fileId, fileName, ct);
                if (filePath != null)
                    return $"Файл {fileName} успешно сохранен в папку: {filePath}";
            }
            return "Файл не соответствует стандарту.";
        }

        //получение имя документа
        private string? GetNameDocumentFromUpdate(Update update)
        {
            if (update.Message?.Document != null)
            {
                return update.Message.Document.FileName;
            }
            return null;
        }

        //получение ID документа
        private string? GetFileIdFromUpdate(Update update)
        {
            if (update.Message?.Document != null)
            {
                return update.Message.Document.FileId;
            }
            else if (update.Message?.Photo != null)
            {
                // Photo приходит как массив PhotoSize, берём наибольшее разрешение
                var photo = update.Message.Photo.LastOrDefault();
                return photo?.FileId;
            }
            else if (update.Message?.Audio != null)
            {
                return update.Message.Audio.FileId;
            }
            else if (update.Message?.Video != null)
            {
                return update.Message.Video.FileId;
            }
            else if (update.Message?.Voice != null)
            {
                return update.Message.Voice.FileId;
            }
            else if (update.Message?.VideoNote != null)
            {
                return update.Message.VideoNote.FileId;
            }
            else if (update.Message?.Sticker != null)
            {
                return update.Message.Sticker.FileId;
            }

            return null;
        }

        //скачивание файла
        private async Task<string?> DownloadFileAsync(ITelegramBotClient botClient, string fileId,
            string fileName, CancellationToken ct)
        {
            try
            {
                //получаем файл
                var file = await botClient.GetFile(fileId, cancellationToken: ct);

                // Формируем полный путь для сохранения
                var downloadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
                Directory.CreateDirectory(downloadsPath);
                var filePath = Path.Combine(downloadsPath, fileName);

                // Удаляем старый существующий файл, если он есть
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Скачиваем файл
                using (var saveStream = System.IO.File.OpenWrite(filePath))
                {
                    await botClient.DownloadFile(file.FilePath, saveStream, ct);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
                return null;
            }
        }
    }
}
