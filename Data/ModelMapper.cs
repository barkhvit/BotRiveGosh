using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Models;
using LinqToDB.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data
{
    internal static class ModelMapper
    {
        public static Prizes MapFromModel(PrizesModel model)
        {
            return new Prizes
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                InArchive = model.InArchive
            };
        }
        public static PrizesModel MapToModel(Prizes entity)
        {
            return new PrizesModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                InArchive = entity.InArchive
            };
        }
        public static ShopsModel MapToModel(Shops entity)
        {
            return new ShopsModel
            {
                Id = entity.Id,
                NameBU = entity.NameBU,
                NameQV = entity.NameQV,
                NameUU = entity.NameUU,
                Region = entity.Region,
                City = entity.City,
                Location = entity.Location,
                Category = entity.Category,
                RK = entity.RK, SN = entity.SN
            };

        }
        public static Shops MapFromModel(ShopsModel model)
        {
            return new Shops
            {
                Id = model.Id,
                NameBU = model.NameBU,
                NameQV = model.NameQV,
                NameUU = model.NameUU,
                Region = model.Region,
                City = model.City,
                Location = model.Location,
                Category = model.Category,
                RK = model.RK,
                SN = model.SN
            };

        }
        public static KpiModel MapToModel(Kpi entity)
        {
            return new KpiModel
            {
                Id = entity.Id,
                ShopId = entity.ShopId,
                Date = entity.Date,
                Position = entity.Position,
                Name = entity.Name,
                LocalId = entity.LocalId,
                TNumber = entity.TNumber,
                CardType = entity.CardType,
                Checks = entity.Checks,
                SpecialChecks = entity.SpecialChecks
            };
        }
        public static Kpi MapFromModel(KpiModel model)
        {
            return new Kpi
            {
                Id = model.Id,
                ShopId = model.ShopId,
                Date = model.Date,
                Position = model.Position,
                Name = model.Name,
                LocalId = model.LocalId,
                TNumber = model.TNumber,
                CardType = model.CardType,
                Checks = model.Checks,
                SpecialChecks = model.SpecialChecks
            };
        }
        public static KpiResult MapFromModel(KpiResultModel model)
        {
            return new KpiResult
            {
                Shop = model.Shop,
                Category = model.Category,
                Name = model.Name,
                TotalChecks = model.TotalChecks,
                SpChecks = model.SpChecks,
                Result = model.Result
            };
        }
        public static KpiResultModel MapToModel(KpiResult entity)
        {
            return new KpiResultModel
            {
                Shop = entity.Shop,
                Category = entity.Category,
                Name = entity.Name,
                TotalChecks = entity.TotalChecks,
                SpChecks = entity.SpChecks,
                Result = entity.Result
            };
        }
        public static User MapFromModel(UserModel model)
        {
            return new User
            {
                Id = model.Id,
                TelegramId = model.TelegramId,
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = model.CreatedAt,
                AccessAllowed = model.AccessAllowed
            };
        }

        public static UserModel MapToModel(User entity)
        {
            return new UserModel
            {
                Id = entity.Id,
                TelegramId = entity.TelegramId,
                Username = entity.Username,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                CreatedAt = entity.CreatedAt,
                AccessAllowed = entity.AccessAllowed
            };
        }
    }
}
