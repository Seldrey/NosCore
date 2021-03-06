﻿//  __  _  __    __   ___ __  ___ ___  
// |  \| |/__\ /' _/ / _//__\| _ \ __| 
// | | ' | \/ |`._`.| \_| \/ | v / _|  
// |_|\__|\__/ |___/ \__/\__/|_|_\___| 
// 
// Copyright (C) 2018 - NosCore
// 
// NosCore is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NosCore.Configuration;
using NosCore.Core.Encryption;
using NosCore.Data.WebApi;
using NosCore.DAL;
using NosCore.Shared.Enumerations.Account;
using NosCore.Shared.I18N;

namespace NosCore.MasterServer.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly WebApiConfiguration _apiConfiguration;

        public TokenController(WebApiConfiguration apiConfiguration)
        {
            _apiConfiguration = apiConfiguration;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ConnectUser(string userName, string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BadRequest(LogLanguage.Instance.GetMessageFromKey(LanguageKey.AUTH_ERROR)));
            }

            var account = DaoFactory.AccountDao.FirstOrDefault(s => s.Name == userName);
            if (!(account?.Password.ToLower().Equals(EncryptionHelper.Sha512(password)) ?? false))
            {
                return BadRequest(LogLanguage.Instance.GetMessageFromKey(LanguageKey.AUTH_INCORRECT));
            }

            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userName),
                new Claim(ClaimTypes.Role, account.Authority.ToString())
            });
            var keyByteArray = Encoding.Default.GetBytes(EncryptionHelper.Sha512(_apiConfiguration.Password));
            var signinKey = new SymmetricSecurityKey(keyByteArray);
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = claims,
                Issuer = "Issuer",
                Audience = "Audience",
                SigningCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            });
            return Ok(handler.WriteToken(securityToken));
        }

        [AllowAnonymous]
        [HttpPost("ConnectServer")]
        public IActionResult ConnectServer([FromBody] WebApiToken serverWebApiToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BadRequest(LogLanguage.Instance.GetMessageFromKey(LanguageKey.AUTH_ERROR)));
            }

            if (serverWebApiToken.ServerToken != _apiConfiguration.Password)
            {
                return BadRequest(LogLanguage.Instance.GetMessageFromKey(LanguageKey.AUTH_INCORRECT));
            }

            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "Server"),
                new Claim(ClaimTypes.Role, nameof(AuthorityType.Root))
            });
            var keyByteArray = Encoding.Default.GetBytes(EncryptionHelper.Sha512(_apiConfiguration.Password));
            var signinKey = new SymmetricSecurityKey(keyByteArray);
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = claims,
                Issuer = "Issuer",
                Audience = "Audience",
                SigningCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            });
            return Ok(handler.WriteToken(securityToken));
        }
    }
}