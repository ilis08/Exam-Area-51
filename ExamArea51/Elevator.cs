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

    public enum State
    {
        Waiting,
        Moving
    }

    public class Elevator
    {

        private int elevatorSpeedPerFloor = 1000;//ms
        private Floors currentFloor;
        private Floors targetFloor;
        private State state;
        private List<Agent> agentsInside;
        private object locker = new object();
        private static object creationLocker = new();

        private Elevator()
        {
            currentFloor = Floors.G;
            state = State.Waiting;
            agentsInside = new();

        }

        private static volatile Elevator instance;

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


        private readonly Semaphore semaphore = new(1, 1);

        public async Task Call(Agent agent, Func<Floors> targetFloor)
        {
            semaphore.WaitOne();

            this.targetFloor = targetFloor.Invoke();

            await GoToFloor();

            if (agent.GetCurrentAgentFloor() == currentFloor)
            {
                EnterAgentIntoElevator(agent);
            }

            while (state == State.Waiting)
            {
                lock (agentsInside)
                {
                    this.targetFloor = agent.ChooseFloor();
                }

                var checker = await CheckAgentSecurityLevel(agent, this.targetFloor);

                if (checker.Contains(this.targetFloor))
                {
                    await GoToFloor();
                }
                else
                {
                    LeaveAgentFrom(agent);
                }
            }
        }

        private void EnterAgentIntoElevator(Agent agent)
        {
            agentsInside.Add(agent);

            MessageWriter.ShowMessage($"The Agent {agent.AgentCode} entered the elevator.");
        }

        private void LeaveAgentFrom(Agent agent)
        {
            agentsInside.Remove(agent);

            MessageWriter.ShowMessage($"The Agent {agent.AgentCode} got off the elevator.");
        }

        private async Task GoToFloor()
        {
            int distance = Math.Abs(currentFloor - targetFloor);

            if (distance == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < distance; i++)
                {
                    MessageWriter.ShowMessage($"Elevator with is going to {targetFloor} floor");

                    SetElevatorState(State.Moving);

                    await Task.Delay(elevatorSpeedPerFloor);                    
                }

                MessageWriter.ShowMessage($"The elevator arrived at {currentFloor}");

                SetElevatorState(State.Waiting);

                currentFloor = targetFloor;
            }
        }

        private void SetElevatorState(State targetState)
        {
            lock (agentsInside)
            {
                this.state = targetState;
            }
        }


        public Floors GetCurrentFloor()
        {
            return currentFloor;
        }

        public async Task<List<Floors>> CheckAgentSecurityLevel(Agent agent, Floors floor)
        {
            List<Floors> floors = new();

            switch (agent.AgentSecurityLevel)
            {
                case SecurityLevel.Confidential:
                    floors.Add(Floors.G);
                    return await Task.FromResult(floors);
                case SecurityLevel.Secret:
                    floors.Add(Floors.G);
                    floors.Add(Floors.S);
                    return await Task.FromResult(floors);
                case SecurityLevel.TopSecret:
                    floors.Add(Floors.G);
                    floors.Add(Floors.S);
                    floors.Add(Floors.T1);
                    floors.Add(Floors.T2);
                    return await Task.FromResult(floors);
                default:
                    return floors;
            }
        }

    }
}
