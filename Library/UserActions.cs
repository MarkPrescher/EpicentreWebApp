using Epicentre.Areas.Identity.Data;
using Epicentre.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Epicentre.Library
{
    public static class UserActions
    {
        // public static string UserEmail { get; set; }
        public static string UserEmail = "msprescher@gmail.com";

        public static bool UserExists(EpicentreDataContext context)
        {
            var userFound = context.UserDetail.FirstOrDefault(u => u.EMAIL_ADDRESS == UserEmail);
            if (userFound == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}