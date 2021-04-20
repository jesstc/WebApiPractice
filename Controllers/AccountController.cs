using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSKA.Model;
using WebSKA.Service;

namespace WebSKA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        public readonly DataService _dbdata;

        public AccountController(DataService dictionaryService)
        {
            _dbdata = dictionaryService;
        }

        [HttpGet("Login/{userName}/{password}")]
        public ActionResult Login(string userName, string password)
        {
            var loginAccountId = _dbdata.accountData.Where(x => x.Name.Equals(userName) && x.Password.Equals(password)).Select(i=> i.UserId).ToArray();
            if(loginAccountId.Length != 0)
            {
                var option = new CookieOptions {Expires = DateTime.Now.AddMinutes(10)};
                Response.Cookies.Append("token", loginAccountId[0], option);
                return Ok("Login success!");
            }
            return _dbdata.accountData.Any(x => x.Name.Equals(userName)) ? Ok("Wrong password, please try again!") : Ok("Cannot find user " + userName + ", please try again!");
        }

        [HttpGet("ListAllAccount")]
        public ActionResult<List<Account>> GetAllAccount()
        {
            if (!Request.Cookies.ContainsKey("token") || !_dbdata.accountData.Any())
            {
                return Ok("Please login again!");
            }
            return Ok(_dbdata.accountData);
        }

        [HttpGet("GetUserById/{id}")]
        public ActionResult GetUserById(string id)
        {
            if (!Request.Cookies.ContainsKey("token")) return Ok("Please login again!");
            var result = _dbdata.accountData.Where(x => x.UserId.Equals(id));
            return !result.Any() ? Ok("Cannot find the userId: " + id): Ok(result);
        }

        [HttpGet("GetLoginUser")]
        public ActionResult GetLoginUser()
        {
            if (!Request.Cookies.ContainsKey("token")) return Ok("Please login again!");

            var result = _dbdata.accountData.Where(x => x.UserId.Equals(Request.Cookies["token"]));
            return result.Any() ? Ok(result) : Ok("Please login again!");
        }

        [HttpPost("CreateAccount/{Name}/{password}")]
        public ActionResult<Account> CreateAccount(string Name, string password)
        {
            var newAccount = new Account() { UserId = Guid.NewGuid().ToString(), Password = password, Name = Name };
            _dbdata.accountData.Add(newAccount);

            return Ok(newAccount);
        }

        [HttpPatch("UpdatePassword/{newPassword}")]
        public ActionResult<Account> UpdatePassword(string newPassword)
        {
            if (!Request.Cookies.ContainsKey("token")) return Ok("Please login again!");
            var updateId = Request.Cookies["token"];
            _dbdata.accountData.Where(x => x.UserId.Equals(updateId)).ToList().ForEach(x =>
            {
                x.Password = newPassword;
            });
            var result = _dbdata.accountData.Where(x => x.UserId.Equals(updateId));
            return result.Any() ? Ok(result) : Ok("Please login again!");
        }

        [HttpDelete("Delete/{Name}")]
        public ActionResult<Account> DeleteAccount(string Name)
        {
            if (!Request.Cookies.ContainsKey("token")) return Ok("Please login again!");
            var changedRaw = _dbdata.accountData.RemoveAll(x => x.Name.Equals(Name));
            return changedRaw == 0 ? Ok("Cannot find match data") : Ok(_dbdata.accountData);
        }
    }
}
