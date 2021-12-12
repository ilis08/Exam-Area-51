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
        private SecurityLevel securityLevel;
        private Floors currentFloor;
        private Elevator elevator;
        private bool isInTheBase = false;
        private int agentCode;

        public int AgentCode
        {
            get
            {
                return agentCode;
            }
        }

        public SecurityLevel AgentSecurityLevel
        {
            get
            {
                return securityLevel;
            }
        }


        public Agent(SecurityLevel securityLevel, Elevator elevator)
        {
            this.securityLevel = securityLevel;
            this.currentFloor = Floors.G;
            this.elevator = elevator;
            this.agentCode = GenerateAgentCode();
        }

        public async Task StartWorkDay()
        {
            MessageWriter.ShowMessage($"Agent {this.agentCode} with security level : {this.securityLevel} entered the base and is currently at {this.currentFloor} floor.");

            isInTheBase = true;

            await Task.Delay(1000);

            await ActivityInBase();
        }

        private async Task ActivityInBase()
        {
            while (isInTheBase)
            {
                await elevator.Call(this, GetCurrentAgentFloor);

                await Task.Delay(10000);
            }
        }

        public Floors ChooseFloor()
        {
            Random r = new Random();

            var res = r.Next(0, 4);

            if (res == 0)
            {
                return Floors.G;
            }
            else if (res == 1)
            {
                return Floors.S;
            }
            else if (res == 2)
            {
                return Floors.T1;
            }
            else
            {
                return Floors.T2;
            }
        }

        private int GenerateAgentCode()
        {
            Random r = new();

            return r.Next(1000, 10000);
        }

        public Floors GetCurrentAgentFloor()
        {
            return this.currentFloor;
        }

    }
}
