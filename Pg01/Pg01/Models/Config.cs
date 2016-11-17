using System.Collections.Generic;

namespace Pg01.Models
{
    public class Config
    {
        public Basic Basic { get; set; }
        public List<ApplicationGroup> ApplicationGroups { get; set; }
    }
}