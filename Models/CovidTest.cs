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
        
        public string TEST_TYPE { get; set; }

        public string TEST_DATE { get; set; }

        public string TEST_TIME { get; set; }

        public string TEST_LOCATION { get; set; }

        public string TEST_STATUS { get; set; }

        public string TEST_RESULT { get; set; }

        public string USER_ID { get; set; }
        
    }
}
