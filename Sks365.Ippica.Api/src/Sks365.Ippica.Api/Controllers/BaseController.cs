using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Common.Utility;

namespace Sks365.Ippica.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private IGlobalValidator _globalValidator;
        private IMapperLocator _mapperLocator;
        private IBetService _betService;
        private IUserService _userService;

        protected IGlobalValidator GlobalValidator => _globalValidator ?? (_globalValidator = HttpContext.RequestServices.GetService<IGlobalValidator>());
        protected IMapperLocator MapperLocator => _mapperLocator ?? (_mapperLocator = HttpContext.RequestServices.GetService<IMapperLocator>());
        protected IBetService BetService => _betService ?? (_betService = HttpContext.RequestServices.GetService<IBetService>());
        protected IUserService UserService => _userService ?? (_userService = HttpContext.RequestServices.GetService<IUserService>());
    }
}


