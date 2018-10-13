using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : PhysicsScript {

    public bool allowMovement;
    public float jumpForce, speed;

    public override void Start ()
    {
        base.Start();
	}
	
	public override void Update ()
    {
        base.Update();
        MovementInput();
    }

    void MovementInput()
    {
        if (allowMovement){
            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (colBottom) verticalVelocity = jumpForce;
                else if (colRight)
                {
                    horizontalVelocity = -speed;
                    verticalVelocity = jumpForce;
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (colLeft)
                {
                    horizontalVelocity = speed;
                    verticalVelocity = jumpForce;
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
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
}