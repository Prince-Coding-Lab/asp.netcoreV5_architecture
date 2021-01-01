using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ScaffoldNetCore.Core.Common;
using ScaffoldNetCore.Core.Dto;
using ScaffoldNetCore.Core.Entities;
using ScaffoldNetCore.Core.Enums;
using ScaffoldNetCore.Core.Interfaces;
using ScaffoldNetCore5._0.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScaffoldNetCore5._0.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        IConfiguration _iconfiguration;
        #endregion

        #region Constructors 
        public AccountsController(IUserService userService,
            IMapper mapper, IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _iconfiguration = configuration;
        }
        #endregion
        #region Action Methods
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok("test");
        }
        /// <summary>
        /// Create a New User
        /// This is a anonymous action method.
        /// </summary>
        /// <param name="user">New User object</param>
        /// <returns>API Response</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] UserCreateDto user)
        {
            if (!ModelState.IsValid)
            {
                return Ok(ApiResponse.ValidationErrorResponse(ModelState));
            }
            var addUser = _mapper.Map<Users>(user);
            if (!string.IsNullOrEmpty(addUser.Password))
            {
                addUser.Password = Cryptography.Encrypt(user.Password);
            }
            DatabaseResponse response = await _userService.CreateUserAsync(addUser);

            if (response.ResponseCode == (int)DbReturnValue.CreateSuccess)
            {
                return Ok(ApiResponse.OkResult(true, response.Results, DbReturnValue.CreateSuccess));
            }
            else
            {
                return Ok(ApiResponse.OkResult(true, response.Results, DbReturnValue.RecordExists));
            }

        }


        /// <summary>
        /// Login user with credential username password.
        /// </summary>
        /// <param name="model">a model hold username and password</param>
        /// <returns>Api Response</returns>
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserAuthDto model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, Cryptography.Encrypt(model.Password));

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
    }
    #endregion
}