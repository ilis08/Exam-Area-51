using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ExamArea51
{
    public enum SecurityLevel
    {
        Confidential,
        Secret,
        TopSecret
    }

    public enum AgentState
    {
        OutsideTheBase,
        InsideTheBase,
        GoHome
    }

    public class Agent
    {
        private SecurityLevel securityLevel;
        private Floors currentFloor;
        private Elevator elevator;
        private AgentState state;
        private int agentCode;
        private Random r = new Random();
        private MyTimer timer = new MyTimer();

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
            timer.SetTimer(GoHome);
        }

        public async Task StartWorkDay()
        {
            MessageWriter.ShowMessage($"Agent {agentCode} came to the gates of the base.", ConsoleColor.Yellow);

            await Task.Delay(r.Next(1000, 3000));

            while (state == AgentState.OutsideTheBase)
            {
                var result = r.Next(1, 101);

                if (result < 60)
                {
                    MessageWriter.ShowMessage($"Agent {this.agentCode} with security level : {this.securityLevel} entered the base and is currently at {this.currentFloor} floor.", ConsoleColor.Green);

                    state = AgentState.InsideTheBase;

                    await Task.Delay(r.Next(1000, 5000));

                    await ActivityInBase();
                }
                else if (result < 90)
                {
                    await WalkAroundTheBase();
                }
                else 
                {
                    GoHome(this, null);
                    MessageWriter.ShowMessage($"Agent {agentCode} is going home.", ConsoleColor.DarkRed);
                }
            }
        }

        private async Task WalkAroundTheBase()
        {
            MessageWriter.ShowMessage($"Agent {agentCode} walk around the base.", ConsoleColor.White);

            await Task.Delay(3000);
        }

        private async Task ActivityInBase()
        {
            while (state == AgentState.InsideTheBase)
            {
                elevator.PressCallButton(this, null);

                await Task.Delay(r.Next(5000, 9000));
            }
        }

        private void GoHome(object sender, ElapsedEventArgs args)
        {
            if (state == AgentState.OutsideTheBase)
            {
                state = AgentState.GoHome;
            }
            else if(state != AgentState.GoHome)
            {
                state = AgentState.GoHome;
                MessageWriter.ShowMessage($"Agent {agentCode} is going to go home", ConsoleColor.Blue);
                elevator.PressCallButton(this, Floors.G);
                MessageWriter.ShowMessage($"Agent {agentCode} is going home.", ConsoleColor.DarkRed);
            }
        }

        public static Floors ChooseFloor()
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

        public void SetCurrentFloor(Floors currentFloor)
        {
            this.currentFloor = currentFloor;
        }

        private static int GenerateAgentCode()
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
