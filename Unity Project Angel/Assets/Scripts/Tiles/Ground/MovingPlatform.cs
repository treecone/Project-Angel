using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Characters;
using UnityEngine;

namespace Assets.Scripts.Tiles.Ground
{
    class MovingPlatform : MonoBehaviour, IGround
    {
        HashSet<CharacterScript> contactingCharacters = new HashSet<CharacterScript>();

        public float maxDisplacement = 3;
        public float currentDisplacement = 0;
        public float speed = 0.2f;
        
        public bool backForth = false;


        private void LateUpdate()
        {
            currentDisplacement += speed * Time.deltaTime;
            if (backForth && Math.Abs(currentDisplacement) > maxDisplacement &&
                Math.Sign(speed) == Math.Sign(currentDisplacement)) speed *= -1;

            gameObject.transform.Translate(Vector2.right * speed * Time.deltaTime);

            foreach(CharacterScript character in contactingCharacters)
            {
                character.transform.Translate(Vector2.right * speed * Time.deltaTime);
            }
        }

        public void Part(CharacterScript character)
        {
            contactingCharacters.Remove(character);
        }

        public void Touch(CharacterScript character)
        {
            Dictionary<PhysicsScript.SIDES, List<GameObject>> contacts = character.GetCollsions();
            if (contacts[PhysicsScript.SIDES.BOTTOM].Contains(gameObject) || 
                contacts[(speed > 0)?PhysicsScript.SIDES.LEFT:PhysicsScript.SIDES.RIGHT].Contains(gameObject))
            {
                contactingCharacters.Add(character);
            }
        }
    }
}
