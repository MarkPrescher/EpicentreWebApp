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

        public static string GetTestType(string testTypeKey)
        {
            return TEST_TYPES.FirstOrDefault(t => t.Key == testTypeKey).Value;
        }
    }
}