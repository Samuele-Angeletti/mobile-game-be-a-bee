using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM
{
    public abstract class State
    {
        public virtual void OnBegin() { }
        public virtual void OnEnd() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
    }
}
