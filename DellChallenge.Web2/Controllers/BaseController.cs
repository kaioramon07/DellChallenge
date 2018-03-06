﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DellChallenge.Domain.Enitities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace DellChallenge.Web2.Controllers
{
    public class BaseController : Controller
    {
        protected User UsuarioLogado => GetUser();

        protected static string SESSION_NAME = "user-on";

        protected async Task RegisterUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Normal")
            };

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            var result = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));
            HttpContext.Session.Set(SESSION_NAME, result);
        }

        protected async Task RemoveUser()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private User GetUser()
        {
            try
            {
                HttpContext.Session.TryGetValue(SESSION_NAME, out byte[] usuario);

                if (usuario != null && usuario.Length > 0)
                    return JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(usuario));
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PageNotFound(string returnUrl = null)
        {

            return View("PageNotFound");
        }

        [HttpGet]
        [AllowAnonymous]
        public new IActionResult StatusCode(int id)
        {
            var statusmessage = string.Empty;
            switch (id)
            {
                case 400:
                    statusmessage = "Bad request: The request cannot be fulfilled due to bad syntax";
                    break;

                case 403:
                    statusmessage = "Forbidden";
                    break;

                case 404:
                    statusmessage = "Page not found";
                    break;

                case 408:
                    statusmessage = "The server timed out waiting for the request";
                    break;

                case 500:
                    statusmessage = "Internal Server Error - server was unable to finish processing the request";
                    break;

                default:
                    statusmessage = "That’s odd... Something we didn't expect happened";
                    break;
            }

            ViewBag.ErrorMessage = statusmessage;
            ViewBag.Code = id;
            return View("StatusCode");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {

            return View("AccessDenied");
        }
    }
}