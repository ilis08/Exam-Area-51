using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamArea51
{
    public enum SecurityLevel
    {
        Confidential,
        Secret,
        TopSecret
    }

    public class Agent
    {
        public SecurityLevel Level { get; set; }

    }
}
