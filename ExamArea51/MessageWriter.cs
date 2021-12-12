using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamArea51
{
    public class MessageWriter
    {
        public static void ShowMessage(string message)
        {
            lock (Console.Out)
            {
                Console.WriteLine(message);
            }
        }

    }
}
