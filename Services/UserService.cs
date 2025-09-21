using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository.Interfaces;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            bool accessAllowed = false;

            //проверка на админа, если пользователь админ, то accessAllowed = true
            string? strAdmins = ConfigurationManager.AppSettings["AdminTelegramId"];
            if (strAdmins != null)
            {
                long[] admins = strAdmins.Split(',')
                .Select(long.Parse)
                .ToArray();
                accessAllowed = admins.Contains(userId) ? true : false;
            }

            //проверяем регистрацию пользователя
            var User = await _userRepository.GetByTelegramIdAsync(userId, ct);
            if (User!=null)
            {
                //если пользователь админ, зарегистрирован, но accessAllowed = false, то меняем на accessAllowed = true
                if (accessAllowed && !User.AccessAllowed)
                {
                    User.AccessAllowed = true;
                    await _userRepository.UpdateAsync(User, ct);
                }
                return User;
            }


            var newUser = new Core.Entities.User
            {
                Id = Guid.NewGuid(),
                TelegramId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                CreatedAt = DateTime.UtcNow,
                AccessAllowed = accessAllowed
            };

            await _userRepository.CreateAsync(newUser, ct);
            return newUser;
        }

        public async Task<bool> UpdateAsync(Core.Entities.User user, CancellationToken ct)
        {
            var result = await _userRepository.UpdateAsync(user, ct);
            return result;
        } 

        public async Task<Core.Entities.User?> GetUserByIdAsync(Guid id, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            return user;
        }

        public Task<Core.Entities.User?> GetUserByTelegramIdAsync(long telegramId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
