using Microsoft.AspNetCore.Http;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.SessionTracker.Client;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISessionTracker _sessionTracker;

        public UserService(IHttpContextAccessor context, ISessionTracker sessionTracker)
        {
            _serviceProvider = context.HttpContext.RequestServices;
            _sessionTracker = sessionTracker;
        }

        public SportWallet GetSportWallet(int userId)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                return unitOfWork.WalletRepository.GetSportWallet(userId);
            }
        }

        public BonusWallet GetBonusWallet(int userId)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                return unitOfWork.WalletRepository.GetBonusWallet(userId);
            }
        }

        public User GetUser(string userName, BookmakerEnum bookmakerId, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                var user = unitOfWork.UserRepository.GetUser(userName, bookmakerId);
                if (getUserDetails) user.Details = unitOfWork.UserRepository.GetUserDetails((int)user.UserId);
                if (getAdditionalData) user.AdditionalData = unitOfWork.UserRepository.GetUserAdditionalData((int)user.UserId);
                if (getWallets)
                {
                    user.SportWallet = unitOfWork.WalletRepository.GetSportWallet((int)user.UserId);
                    user.BonusWallet = unitOfWork.WalletRepository.GetBonusWallet((int)user.UserId);
                }

                return user;
            }
        }

        public User GetUser(string session, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false)
        {
            var sessionData = (_sessionTracker.GetSession(session)).Result;
            if (sessionData.SessionExists)
            {
                var user = GetUser(sessionData.Username, (BookmakerEnum)sessionData.BookmakerId, getWallets, getUserDetails, getAdditionalData);
                return user;
            }

            return null;
        }

        public User GetUser(int userId, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                var user = unitOfWork.UserRepository.GetUser(userId);
                if (getUserDetails) user.Details = unitOfWork.UserRepository.GetUserDetails((int)user.UserId);
                if (getAdditionalData) user.AdditionalData = unitOfWork.UserRepository.GetUserAdditionalData((int)user.UserId);
                if (getWallets)
                {
                    user.SportWallet = unitOfWork.WalletRepository.GetSportWallet((int)user.UserId);
                    user.BonusWallet = unitOfWork.WalletRepository.GetBonusWallet((int)user.UserId);
                }

                return user;
            }
        }

        public User GetUserByTicketId(string ticketId, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;
            using (unitOfWork)
            {
                var userId = unitOfWork.BetRepository.GetBet(ticketId)?.UserId;
                if (userId.HasValue)
                {
                    var user = GetUser((int)userId, getWallets, getUserDetails, getAdditionalData);
                    return user;
                }
            }

            return null;
        }

        public List<UserAdditionalData> GetUserAdditionalData(int userId)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                return unitOfWork.UserRepository.GetUserAdditionalData(userId);
            }
        }

        public UserAdditionalData GetUserAdditionalDataValue(int userId, UserDataTypeEnum dataType)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                return unitOfWork.UserRepository.GetUserAdditionalDataValue(userId, dataType);
            }
        }

        public string GetUserParameterValue(int userId, UserParameterTypeEnum? userParameterId)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                return unitOfWork.UserRepository.GetUserParameterValue(userId, userParameterId);
            }
        }
    }
}
