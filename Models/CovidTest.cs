using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Models
{
    public class CovidTest
    {
        [Key]
        public Guid TEST_ID { get; set; }
        [DisplayName("Test Type")]
        public string TEST_TYPE { get; set; }
        [DisplayName("Test Date")]

        public string TEST_DATE { get; set; }
        [DisplayName("Test Time")]

        public string TEST_TIME { get; set; }
        [DisplayName("Test Location")]

        public string TEST_LOCATION { get; set; }
        [DisplayName("Test Status")]

        public string TEST_STATUS { get; set; }
        [DisplayName("Test Result")]

        public string TEST_RESULT { get; set; }

        public string USER_EMAIL { get; set; }
    }
}
