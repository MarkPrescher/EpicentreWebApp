using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Models
{
    public class CovidTest
    {
        [Key]
        public Guid TEST_ID { get; set; }
        
        [Required(ErrorMessage = "Please enter a test type")]
        [StringLength(25, ErrorMessage = "Please enter a value with a maximum of 25 characters")]
        public string TEST_TYPE { get; set; }

        [Required(ErrorMessage = "Please enter a test date")]
        [StringLength(55, ErrorMessage = "Please enter a value with a maximum of 55 characters")]
        public string TEST_DATE { get; set; }

        [Required(ErrorMessage = "Please input test status")]
        [StringLength(55, ErrorMessage = "Please enter a value with a maximum of 55 characters")]

        public string TEST_STATUS { get; set; }

        [Required(ErrorMessage = "Please input test result")]
        [StringLength(55, ErrorMessage = "Please enter a value with a maximum of 55 characters")]
        public string TEST_RESULT { get; set; }

        [Required(ErrorMessage = "Please enter user ID")]
        [StringLength(55, ErrorMessage = "Please enter a value with a maximum of 55 characters")]
        public string USER_ID { get; set; }
        
    }
}
