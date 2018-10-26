using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Tiles
{
    interface ITile
    {
        /// <summary>
        /// Behavior can be applied to a character when they touch the tile
        /// </summary>
        /// <param name="character">The character touching this object</param>
        void Touch(Characters.CharacterScript character);

        /// <summary>
        /// Behavior to be applied to a character when they stop touching the tile
        /// </summary>
        /// <param name="character"></param>
        void Part(Characters.CharacterScript character);
    }
}
