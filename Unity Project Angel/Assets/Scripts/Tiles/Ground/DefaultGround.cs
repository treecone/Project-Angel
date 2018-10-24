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
        public void Touch(CharacterScript character)
        {
            throw new NotImplementedException();
        }
    }
}
