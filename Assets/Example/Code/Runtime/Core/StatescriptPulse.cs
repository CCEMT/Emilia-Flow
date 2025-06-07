using System;

namespace Emilia.Statescript
{
    public class StatescriptPulse
    {
        private Action _onDone;

        public event Action onDone
        {
            add => this._onDone += value;
            remove => this._onDone -= value;
        }

        public void Done()
        {
            this._onDone?.Invoke();
        }

        public void Clear()
        {
            this._onDone = null;
        }
    }
}