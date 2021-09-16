using System.Collections.Generic;
using System.Linq;

namespace Epicentre.Library
{
    public class UrlParams
    {
        private readonly static Dictionary<string, string> TEST_TYPES = new Dictionary<string, string>() {
            {
                "dZr9RcABjq", "PCR Swab Test"
            },
            {
                "SkwFaATkYv", "Rapid Antigen Test"
            },
            {
                "hcYPmydyWZ", "Antibody Test"
            }
        };

        private readonly static Dictionary<string, string> TEST_LOCATIONS = new Dictionary<string, string>()
        {
            {
                "JZjKyupGXO", "Stellenbosch, Western Cape"
            },
            {
                "eC4LdaFsWu", "Bellville, Western Cape"
            },
            {
                "ABIf3LMQ4l", "Rondebosch, Western Cape"
            },
            {
                "WH2fbXxIdU", "Hillcrest, KwaZulu-Natal"
            },
            {
                "UQLsvizNLJ", "Pietermaritzburg, KwaZulu-Natal"
            },
            {
                "NmCGgsAEnZ", "Durban Central, KwaZulu-Natal"
            },
            {
                "2464QfRQfc", "Randburg, Gauteng"
            },
            {
                "gqoz3cT0jA", "Midrand, Gauteng"
            }
        };

        public static string GetTestType(string testTypeKey)
        {
            return TEST_TYPES.FirstOrDefault(t => t.Key == testTypeKey).Value;
        }

        public static string GetTestLocation(string testLocationKey)
        {
            return TEST_LOCATIONS.FirstOrDefault(t => t.Key == testLocationKey).Value;
        }
    }
}