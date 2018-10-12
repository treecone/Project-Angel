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
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && allowMovement)
        {
            if (colBottom)
            {
                verticalVelocity = jumpForce;
            }
            else if (colRight)
            {
                verticalVelocity = jumpForce;
                horizontalVelocity = -speed;
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
        if (Input.GetKey(KeyCode.A) && !colLeft && allowMovement)
        {
            horizontalVelocity = -speed;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (Input.GetKey(KeyCode.D) && !colRight && allowMovement)
        {
            horizontalVelocity = speed;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        if (!Input.GetKey(KeyCode.D) != false && !Input.GetKey(KeyCode.A) != false && /*colBottom && */ allowMovement)
        {
            horizontalVelocity = 0;
        }
    }
}