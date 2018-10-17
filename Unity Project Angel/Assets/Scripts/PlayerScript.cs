using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : PhysicsScript
{

    public bool allowInput = true;
    /// <summary>
    /// Mid wall jump if index 0 is true
    /// finalize wall jump if you collide with
    /// the opposite wall or hit the ground
    /// </summary>
    private bool[] wallJump = { false, false, false };

    public float jumpForce, speed;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        MovementInput();
    }

    void MovementInput()
    {

        wallJump[0] = (wallJump[0]) ? !(sides[SIDES.BOTTOM] || (wallJump[1] == sides[SIDES.RIGHT] && wallJump[2] == sides[SIDES.LEFT])) : false;
        if (allowInput && !wallJump[0])
        {
            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (sides[SIDES.BOTTOM] || sides[SIDES.LEFTBODY] || sides[SIDES.RIGHTBODY])
                    verticalVelocity = jumpForce; // Normal Jump
                if (sides[SIDES.RIGHTBODY] && !sides[SIDES.BOTTOM]) // Right Wall jump
                {
                    horizontalVelocity = -speed;
                    wallJump = new bool[] { true, false, true };
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    return;
                }
                else if (sides[SIDES.LEFTBODY] && !sides[SIDES.BOTTOM])
                {
                    horizontalVelocity = speed;
                    wallJump = new bool[] { true, true, false };
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    return;
                }

            }


            //Left and Right
            if (Input.GetKey(KeyCode.A) && !sides[SIDES.LEFT])
            {
                horizontalVelocity = -speed;
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
                gameObject.GetComponent<Animator>().Play("PlayerWalking", 0);
            }
            if (Input.GetKey(KeyCode.D) && !sides[SIDES.RIGHT])
            {
                horizontalVelocity = speed;
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
                gameObject.GetComponent<Animator>().Play("PlayerWalking", 0);
            }
            //Original was this: if (!Input.GetKey(KeyCode.D) != false && !Input.GetKey(KeyCode.A) != false)
            // WHYYYYYYYYY
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                horizontalVelocity = 0;
                gameObject.GetComponent<Animator>().Play("PlayerIdle", 0);
            }
        }
    }
}