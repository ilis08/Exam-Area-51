using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamArea51
{
    class Program
    {
        static void Main(string[] args)
        {

            var tasks = new List<Task>();

            var elevator = Elevator.ElevatorInstance;

            var agent1 = new Agent(SecurityLevel.Confidential, elevator);
            var agent2 = new Agent(SecurityLevel.Secret, elevator);
            var agent3 = new Agent(SecurityLevel.TopSecret, elevator);

            tasks.Add(Task.Run(agent1.StartWorkDay));
            tasks.Add(Task.Run(agent2.StartWorkDay));
            tasks.Add(Task.Run(agent3.StartWorkDay));

            Task.WhenAll(tasks).Wait();

        }
    }
}
