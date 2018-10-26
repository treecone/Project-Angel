using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Characters;
using UnityEngine;

namespace Assets.Scripts.Tiles.Ground
{
    class DefaultGround : MonoBehaviour, Ground.IGround
    {
        public void Part(CharacterScript character)
        {
            Debug.Log("Character Parted from Me");
        }

        public void Touch(CharacterScript character)
        {
            Debug.Log("Character Touched Me");
        }
    }
}
