using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Tiles.Ground
{
    interface IGround
    {
        /// <summary>
        /// Behavior can be applied to a character when they touch the ground
        /// </summary>
        /// <param name="character">The character touching this object</param>
        void Touch(Characters.CharacterScript character);
    }
}
