using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Characters;
using UnityEngine;

namespace Assets.Scripts.Tiles.Special
{
    class Ladder : MonoBehaviour, ITile
    {
        public void Part(CharacterScript character)
        {
            character.gravity = -0.2f;
            character.SetLadder(false);
        }

        public void Touch(CharacterScript character)
        { 
            character.SetLadder(true);
        }

    }
}
