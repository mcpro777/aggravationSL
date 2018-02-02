using System;
using System.Collections.Generic;

namespace Aggravation
{
    public class NewGameWindowStartEventArgs : EventArgs
    {
        public Int32 StartPlayerNum { get; set; }
        public List<NewPlayerPrompt> PlayerPrompts { get; set; }

        public NewGameWindowStartEventArgs(Int32 startPlayerNum, List<NewPlayerPrompt> playerPrompts)
        {
            this.StartPlayerNum = startPlayerNum;
            this.PlayerPrompts = playerPrompts;
        }
    }
}
