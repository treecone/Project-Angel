using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Characters {

    public class PlayerScript : CharacterScript
    {

        public bool allowInput = true;
        /// <summary>
        /// Mid wall jump if index 0 is true
        /// finalize wall jump if you collide with
        /// the opposite wall or hit the ground
        /// </summary>
        private bool[] wallJump = { false, false, false };

        public override void Update()
        {
            base.Update();
            MovementInput();
        }

        void MovementInput()
        {

            wallJump[0] = (wallJump[0]) ? !(ContactingSide(SIDES.BOTTOM) || (wallJump[1] == ContactingSide(SIDES.RIGHT)
                                                                                && wallJump[2] == ContactingSide(SIDES.LEFT))) : false;
            if (allowInput && !wallJump[0])
            {
                //Jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (ContactingSide(SIDES.BOTTOM) || FlushWithSide(SIDES.RIGHT) || FlushWithSide(SIDES.LEFT))
                        verticalVelocity = jumpForce; // Normal Jump
                    if (FlushWithSide(SIDES.RIGHT) && !ContactingSide(SIDES.BOTTOM)) // Right Wall jump
                    {
                        horizontalVelocity = -speed;
                        wallJump = new bool[] { true, false, true };
                        gameObject.GetComponent<SpriteRenderer>().flipX = true;
                        return;
                    }
                    else if (FlushWithSide(SIDES.LEFT) && !ContactingSide(SIDES.BOTTOM))
                    {
                        horizontalVelocity = speed;
                        wallJump = new bool[] { true, true, false };
                        gameObject.GetComponent<SpriteRenderer>().flipX = false;
                        return;
                    }

                }


                //Left and Right
                if (Input.GetKey(KeyCode.A) && !ContactingSide(SIDES.LEFT))
                {
                    horizontalVelocity = -speed;
                }
                if (Input.GetKey(KeyCode.D) && !ContactingSide(SIDES.RIGHT))
                {
                    horizontalVelocity = speed;
                    
                    
                }
                //Original was this: if (!Input.GetKey(KeyCode.D) != false && !Input.GetKey(KeyCode.A) != false)
                // WHYYYYYYYYY
                if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                {
                    horizontalVelocity = 0;
                    //
                }

                if(onLadder && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
                {
                    verticalVelocity = speed * ((Input.GetKey(KeyCode.W)) ? 1 : -1);
                    gravity = 0;
                    //gameObject.transform.Translate(Vector2.up * speed * Time.deltaTime * ((Input.GetKey(KeyCode.W)) ? 1 : -1));
                } else if(onLadder && Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
                {
                    verticalVelocity = 0;
                }

            }
        }

        public override void Idle()
        {
            gameObject.GetComponent<Animator>().Play("PlayerIdle", 0);
        } 

        public override void Moving()
        {
            if (horizontalVelocity != 0) {
                gameObject.GetComponent<SpriteRenderer>().flipX = horizontalVelocity < 0;
                gameObject.GetComponent<Animator>().Play("PlayerWalking", 0);
            } else if(horizontalVelocity == 0)
            {
                gameObject.GetComponent<Animator>().Play("PlayerIdle", 0);
            }
        }

        public override void Ladder()
        {
            // throw new System.NotImplementedException();
        }
    }
}