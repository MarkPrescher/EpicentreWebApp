using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Models
{
    public class UserDetail
    {
        public Guid ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string ID_NUMBER { get; set; }
        public string CONTACT_NUMBER { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string GENDER { get; set; }
        public string MEDICAL_AID { get; set; }
        public string MEMBERSHIP_NUMBER { get; set; }
        public string AUTH_NUMBER { get; set; }
    }
}