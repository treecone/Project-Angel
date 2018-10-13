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
    public bool[] wallJump = { false, false, false };

    float freezeMovement;

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

        wallJump[0] = (wallJump[0]) ? !(colBottom || (wallJump[1] == colRight && wallJump[2] == colLeft)) : false;
        if (allowInput && !wallJump[0])
        {
            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (colBottom || rightBothTouching || leftBothTouching)
                    verticalVelocity = jumpForce; // Normal Jump
                if (rightBothTouching && !colBottom) // Right Wall jump
                {
                    horizontalVelocity = -speed;
                    wallJump = new bool[] { true, false, true };
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    return;
                }
                else if (leftBothTouching && !colBottom)
                {
                    horizontalVelocity = speed;
                    wallJump = new bool[] { true, true, false };
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    return;
                }

            }


            //Left and Right
            if (Input.GetKey(KeyCode.A) && !colLeft)
            {
                horizontalVelocity = -speed;
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (Input.GetKey(KeyCode.D) && !colRight)
            {
                horizontalVelocity = speed;
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            //Original was this: if (!Input.GetKey(KeyCode.D) != false && !Input.GetKey(KeyCode.A) != false)
            // WHYYYYYYYYY
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) horizontalVelocity = 0;
        }
    }

    void blockMovement(float seconds)
    {
        allowInput = false;
        freezeMovement = Time.realtimeSinceStartup + seconds;
    }
}