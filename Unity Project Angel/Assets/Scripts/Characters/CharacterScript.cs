﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Characters
{
    public abstract class CharacterScript : PhysicsScript
    {
        public int Health;

        public float jumpForce, speed;

        protected bool onLadder;

        public virtual void Start()
        {
            base.Start(this);
        }

        public override void Update()
        {
            base.Update();
            
            if (verticalVelocity != 0 || horizontalVelocity != 0)
            {
                Moving();
            } else
            {
                Idle();
            }
        }

        public abstract void Ladder();
        public abstract void Idle();
        public abstract void Moving();
        
        public void SetLadder(bool onLadder)
        {
            this.onLadder = onLadder; 
        }
    }
}
