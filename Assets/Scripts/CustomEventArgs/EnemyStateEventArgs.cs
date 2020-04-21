using System;

namespace Assets.Scripts.CustomEventArgs
{
    public class EnemyStateEventArgs : EventArgs
    {
        public bool state;

        public EnemyStateEventArgs(bool state)
        {
            this.state = state;
        }
    }
}
