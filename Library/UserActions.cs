using Epicentre.Data;
using System.Linq;

namespace Epicentre.Library
{
    public static class UserActions
    {
        //public static string UserEmail { get; set; }
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