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
            Debug.Log("Parted Ladder");
        }

        public void Touch(CharacterScript character)
        {
            Debug.Log("Touched Ladder");
        }
    }
}
