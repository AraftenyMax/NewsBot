using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Components
{
    public class StateChangeEventArgs: EventArgs
    {
        public StateChangeEventArgs(ComponentState state)
        {
            State = state;
        }

        public ComponentState State { get; set; }
    }

    public delegate void StateChangeEventHandler(object sender, StateChangeEventArgs args);
}
