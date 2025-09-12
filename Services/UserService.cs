using BotRiveGosh.Data.Repository.Interfaces;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotRiveGosh.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Core.Entities.User> GetOrCreateUserAsync(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            var isExist = await _userRepository.ExistsAsync(userId, ct);

            if (isExist)
            {
                return await _userRepository.GetByTelegramIdAsync(userId, ct);
            }

            var newUser = new Core.Entities.User
            {
                Id = Guid.NewGuid(),
                TelegramId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(newUser, ct);
            return newUser;
        }

        public Task<Core.Entities.User?> GetUserByIdAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Core.Entities.User?> GetUserByTelegramIdAsync(long telegramId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
