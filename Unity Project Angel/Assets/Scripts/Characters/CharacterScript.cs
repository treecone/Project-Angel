using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Characters
{
    public abstract class CharacterScript : PhysicsScript
    {
        private void Start()
        {
            base.Start(this);
        }
    }
}
