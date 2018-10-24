using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Tiles.Ground
{
    abstract class Ground
    {
        /// <summary>
        /// When a character touches an object containing this script 
        /// this method is called
        /// </summary>
        /// <param name="character">The character touching this object</param>
        public abstract void Touch(Characters.CharacterScript character);
    }
}
