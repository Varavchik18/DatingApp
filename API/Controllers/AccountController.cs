using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        public DataContext _context { get; }
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;

            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>>Register(RegisterDTO _registerDTO){

            if(await UserExists(_registerDTO.UserName)) return BadRequest("Username is already taken");
            

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = _registerDTO.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>>Login(LoginDto _loginDto){
            var user = await _context.Users.
            Include(p=>p.Photos).
            SingleOrDefaultAsync(x=> x.UserName == _loginDto.UserName);

            if (user == null) return Unauthorized("Invalid UserName");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_loginDto.Password));

            for (int i = 0; i<computedHash.Length;i++)
            {
                if(computedHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
                
            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
            };       
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync( x=> x.UserName == username.ToLower());
        }
    }
}