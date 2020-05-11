using System;

namespace Assets.Scripts.CustomEventArgs
{
    public class DieEventArgs : EventArgs
    {
        public bool KilledByPlayer { get; private set; }

        public DieEventArgs(bool killedByPlayer)
        {
            KilledByPlayer = killedByPlayer;
        }
    }
}