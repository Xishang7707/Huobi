using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using HuobiProject.Models;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace HuobiProject.Utils
{
    public class Token
    {
        private readonly IConfiguration Configuration;
        public Token(IConfiguration Configuration) => this.Configuration = Configuration;

        public string CreateToken(TUser user)
        {
            //Payload
            var userClaims = new ClaimsIdentity(new[]{
                    new Claim(JwtRegisteredClaimNames.NameId, user.UserId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }
            );
            //密钥
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Issuer"],
                Subject = userClaims,
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };
            //tokenHandler.ReadJwtToken("").
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        /// <summary>
        /// 获取Token的用户Id
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetUserId(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token).Payload["nameid"].ToString();
        }
    }
}