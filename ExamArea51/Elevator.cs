using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamArea51
{
    public enum Floors
    {
        G,
        S, 
        T1,
        T2
    }

    public enum State
    {
        Waiting,
        Moving
    }

    public class Elevator
    {
        private Floors currentFloor;
        private State state;
        private List<Agent> agenstInside;

    }
}
