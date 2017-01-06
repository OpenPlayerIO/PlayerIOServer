using System;

namespace SocketSlim
{
    public class StateChangedEventArgs<TState> : EventArgs
    {
        private readonly TState oldState;
        private readonly TState newState;

        public StateChangedEventArgs(TState oldState, TState newState)
        {
            this.oldState = oldState;
            this.newState = newState;
        }

        public TState NewState
        {
            get { return newState; }
        }

        public TState OldState
        {
            get { return oldState; }
        }
    }
}