using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamArea51
{
    public enum Floors
    {
        G = 0,
        S = 1,
        T1 = 2,
        T2 = 3
    }

    public class Elevator
    {

        private int elevatorSpeedPerFloor = 1000;//ms
        private Floors currentFloor;
        private Floors targetFloor;
        private List<Agent> agentsInside;
        private object locker = new object();
        private static object creationLocker = new();

        private Elevator()
        {
            currentFloor = Floors.G;
            agentsInside = new();

        }

        private static volatile Elevator instance;

        // Here I use a Singletone Pattern
        public static Elevator ElevatorInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (creationLocker)
                    {
                        if (instance == null)
                        {
                            instance = new Elevator();
                        }
                    }
                }
                return instance;
            }
        }


        private readonly Mutex mutex = new();

        public void PressCallButton(Agent agent, Floors? targetFloor)
        {
            MessageWriter.ShowMessage($"Agent {agent.AgentCode} calls the elevator.", ConsoleColor.White);

            Call(agent, targetFloor);
        }

        private void Call(Agent agent, Floors? floor)
        {
            mutex.WaitOne();

            SetTargetFloor(agent.GetCurrentAgentFloor());

            GoToFloor();

            if (agent.GetCurrentAgentFloor() == currentFloor)
            {
                lock (agentsInside)
                {
                    EnterAgentIntoElevator(agent);
                }
            }

            if (floor!=null)
            {
                lock(agentsInside)
                {
                    targetFloor = (Floors)floor;
                }
            }
            else
            {
                lock (agentsInside)
                {
                    this.targetFloor = Agent.ChooseFloor();
                }
            }


            MessageWriter.ShowMessage($"Agent {agent.AgentCode} pressed the button for floor {this.targetFloor}", ConsoleColor.White);

            var checker = CheckAgentSecurityLevel(agent, this.targetFloor);

            if (checker.Contains(this.targetFloor))
            {
                GoToFloor();
                agent.SetCurrentFloor(currentFloor);
                lock (agentsInside)
                {
                    LeaveAgentFrom(agent);
                }
            }
            else
            {
                MessageWriter.ShowMessage($"The Agent {agent.AgentCode} has no permission to go to the {this.targetFloor} floor.", ConsoleColor.DarkRed);
                lock (agentsInside)
                {
                    LeaveAgentFrom(agent);
                }
            }

            mutex.ReleaseMutex();
        }

        private void EnterAgentIntoElevator(Agent agent)
        {
            agentsInside.Add(agent);

            MessageWriter.ShowMessage($"The Agent {agent.AgentCode} entered the elevator.", ConsoleColor.White);
        }

        private void LeaveAgentFrom(Agent agent)
        {
            MessageWriter.ShowMessage($"The Agent {agent.AgentCode} got off the elevator.", ConsoleColor.White);

            agentsInside.Remove(agent);
        }

        private void GoToFloor()
        {
            int distance = Math.Abs(currentFloor - targetFloor);

            if (distance == 0)
            {
                Task.Delay(1000);
                MessageWriter.ShowMessage($"Agent is currently on {currentFloor}", ConsoleColor.White);
            }
            else
            {
                for (int i = 0; i < distance; i++)
                {
                    MessageWriter.ShowMessage($"Elevator is going to {targetFloor} floor", ConsoleColor.White);

                    Task.Delay(elevatorSpeedPerFloor).Wait();
                }
                SetCurrentFloor(targetFloor);

                MessageWriter.ShowMessage($"The elevator arrived at {currentFloor}", ConsoleColor.White);
            }
        }

        public void SetTargetFloor(Floors targetFloor)
        {
            this.targetFloor = targetFloor;
        }

        public void SetCurrentFloor(Floors currentFloor)
        {
            this.currentFloor = currentFloor;
        }


        public Floors GetCurrentFloor()
        {
            return currentFloor;
        }

        public List<Floors> CheckAgentSecurityLevel(Agent agent, Floors floor)
        {
            List<Floors> floors = new();

            switch (agent.AgentSecurityLevel)
            {
                case SecurityLevel.Confidential:
                    floors.Add(Floors.G);
                    return floors;
                case SecurityLevel.Secret:
                    floors.Add(Floors.G);
                    floors.Add(Floors.S);
                    return floors;
                case SecurityLevel.TopSecret:
                    floors.Add(Floors.G);
                    floors.Add(Floors.S);
                    floors.Add(Floors.T1);
                    floors.Add(Floors.T2);
                    return floors;
                default:
                    return floors;
            }
        }

    }
}
