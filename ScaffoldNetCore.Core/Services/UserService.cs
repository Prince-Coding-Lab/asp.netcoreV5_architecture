using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ScaffoldNetCore.Core.Common;
using ScaffoldNetCore.Core.Dto;
using ScaffoldNetCore.Core.Entities;
using ScaffoldNetCore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldNetCore.Core.Services
{
    public class UserService : IUserService
    {
        #region Fields
        internal IDataAccessHelper _dataHelper = null;
        private readonly IConfiguration _configuration;
        #endregion
        #region Constructors 
        public UserService(IDataAccessHelper dataHelper, IConfiguration configuration)
        {
            _dataHelper = dataHelper;
            _configuration = configuration;
        }
        #endregion
        public async Task<UserDto> AuthenticateAsync(string username, string password)
        {
            try
            {
                UserDto user = null;

                SqlParameter[] parameters =
                {
                    new SqlParameter( "@Email",  SqlDbType.NVarChar ),
                    new SqlParameter( "@Password",  SqlDbType.NVarChar )
                };

                parameters[0].Value = username;
                parameters[1].Value = password;
                _dataHelper.CommandWithParams(ProcedureNames.SpsAuthUser, parameters);

                DataTable dt = new DataTable();

                int result = await _dataHelper.RunAsync(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    user = (from model in dt.AsEnumerable()
                            select new UserDto()
                            {
                                Id = model.Field<int>("Id"),
                                FirstName = model.Field<string>("FirstName"),
                                LastName = model.Field<string>("LastName"),
                                Email = model.Field<string>("Email"),
                                Role = model.Field<string>("Role"),
                               
                            }).FirstOrDefault();
                }

                // return null if user not found
                if (user == null)
                    return null;

                // authentication successful so generate jwt token
                user.Token = GenerateJwtToken(user);
                return user;
            }
            finally
            {
                _dataHelper.Dispose();
            }
        }

        public async Task<DatabaseResponse> CreateUserAsync(Users user)
        {
            try
            {
                SqlParameter[] parameters =
                {
                        new SqlParameter( "@FirstName",  SqlDbType.NVarChar ),
                        new SqlParameter( "@LastName",  SqlDbType.NVarChar ),
                        new SqlParameter( "@Email",  SqlDbType.NVarChar ),
                        new SqlParameter( "@Password",  SqlDbType.NVarChar ),
                        new SqlParameter( "@RoleId",  SqlDbType.Int )
                };

                parameters[0].Value = user.FirstName;
                parameters[1].Value = user.LastName;
                parameters[2].Value = user.Email;
                parameters[3].Value = user.Password;
                parameters[4].Value = user.RoleId;

                _dataHelper.CommandWithParams(ProcedureNames.SpiUsers, parameters);

                DataTable dt = new DataTable();
                UserDto userData = null;
                int result = await _dataHelper.RunAsync(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    userData = (from model in dt.AsEnumerable()
                                select new UserDto()
                                {
                                    Id = model.Field<int>("Id"),
                                    FirstName = model.Field<string>("FirstName"),
                                    LastName = model.Field<string>("LastName"),
                                    Email = model.Field<string>("Email"),
                                    Role = model.Field<string>("Role")
                                }).FirstOrDefault();
                }

                return new DatabaseResponse { ResponseCode = result };
            }
            finally
            {

                _dataHelper.Dispose();
            }
        }

        #region Private Methods
        private string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings").GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role.ToLower())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration.GetSection("AppSettings").GetSection("Audience").Value,//<string>("AppSettings:Audience"),
                Issuer = _configuration.GetSection("AppSettings").GetSection("Issuer").Value,// _configuration.GetValue<string>("AppSettings:Issuer")
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
        #endregion
    }
}
