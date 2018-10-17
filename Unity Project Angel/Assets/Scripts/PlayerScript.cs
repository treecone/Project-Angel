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

        wallJump[0] = (wallJump[0]) ? !(getCollsions()[SIDES.BOTTOM] || (wallJump[1] == getCollsions()[SIDES.RIGHT] && wallJump[2] == getCollsions()[SIDES.LEFT])) : false;
        if (allowInput && !wallJump[0])
        {
            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (getCollsions()[SIDES.BOTTOM] || getCollsions()[SIDES.LEFTBODY] || getCollsions()[SIDES.RIGHTBODY])
                    verticalVelocity = jumpForce; // Normal Jump
                if (getCollsions()[SIDES.RIGHTBODY] && !getCollsions()[SIDES.BOTTOM]) // Right Wall jump
                {
                    horizontalVelocity = -speed;
                    wallJump = new bool[] { true, false, true };
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    return;
                }
                else if (getCollsions()[SIDES.LEFTBODY] && !getCollsions()[SIDES.BOTTOM])
                {
                    horizontalVelocity = speed;
                    wallJump = new bool[] { true, true, false };
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    return;
                }

            }


            //Left and Right
            if (Input.GetKey(KeyCode.A) && !getCollsions()[SIDES.LEFT])
            {
                horizontalVelocity = -speed;
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (Input.GetKey(KeyCode.D) && !getCollsions()[SIDES.RIGHT])
            {
                horizontalVelocity = speed;
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            //Original was this: if (!Input.GetKey(KeyCode.D) != false && !Input.GetKey(KeyCode.A) != false)
            // WHYYYYYYYYY
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) horizontalVelocity = 0;
        }
    }
}