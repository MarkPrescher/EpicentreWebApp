using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Models
{
    public class CovidVaccination
    {
        [Key]
        public Guid VACCINATION_ID { get; set; }

        [Required(ErrorMessage = "Please enter a vaccination type")]
        [StringLength(55, ErrorMessage = "Please enter a value with a maximum of 55 characters")]
        public string VACCINATION_TYPE { get; set; }

        [Required(ErrorMessage ="Please enter a date")]
        [StringLength(55,ErrorMessage ="Please enter a value with a maximum of 55 characters")]
        public string VACCINATION_DATE { get; set; }

        public string VACCINATION_NEXT_DATE { get; set; }

        public string VACCINATION_STATUS { get; set; }

        public string USER_ID { get; set; }

    }
}
