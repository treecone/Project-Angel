using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyMoveScript : MonoBehaviour {

    public bool colLeft, colRight, colBottom, colTop;
    public float verticalVelocity, horizontalVelocity;
    public float jumpForce, speed;
    public float gravity;
    public GameObject lastTouchingCollider;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        Movement();
        CollisionDetection();
        MovementInput();
    }

    void CollisionDetection ()
    {
        //These set the col bools to determine if the player is next to a collider.
        Vector2 playerPos = gameObject.transform.position;
        colLeft = RaycastCollision(new Vector2(playerPos.x - 0.4f, playerPos.y), Vector2.left, 0.15f, true);
        colRight = RaycastCollision(new Vector2(playerPos.x + 0.4f, playerPos.y), Vector2.right, 0.15f, true);
        colBottom = RaycastCollision(new Vector2(playerPos.x, playerPos.y - 0.8f), Vector2.down, 0.15f, true);
        colTop = RaycastCollision(new Vector2(playerPos.x, playerPos.y + 0.8f), Vector2.up, 0.15f, true);
    }

    void Movement ()
    {
        //Moving because of gravity
        gameObject.transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime);
        gameObject.transform.Translate(Vector2.right * horizontalVelocity * Time.deltaTime);

        //Applying gravity
        if (colBottom)
        {
            if(verticalVelocity < 0)
            verticalVelocity = 0;
        }
        else if (colRight || colLeft)
        {
            verticalVelocity -= (gravity / 20);
        }
        else
        {
            verticalVelocity -= gravity;
        }

        //Stopping player when running into wall
        if (colRight) { if(horizontalVelocity > 0.05f) { horizontalVelocity = 0; verticalVelocity = 0; } }
        if (colLeft) { if (horizontalVelocity < -0.05f) { horizontalVelocity = 0; verticalVelocity = 0; } }
        if (colTop) { if (verticalVelocity > 0.05f) { verticalVelocity = 0; } }
    }

    void MovementInput ()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (colBottom)
            {
                verticalVelocity = jumpForce;
            }
            else if (colRight)
            {
                verticalVelocity = jumpForce;
                horizontalVelocity = -5;
            }
            else if (colLeft)
            {
                horizontalVelocity = 5;
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
        if (!Input.GetKey(KeyCode.D) != false && !Input.GetKey(KeyCode.A) != false && colBottom)
        {
            horizontalVelocity = 0;
        }
    }

    //Uses raycasts to hit colliders in the scene.
    bool RaycastCollision(Vector2 pos, Vector2 dir, float distance, bool applyLastCollider)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, distance);
        Debug.DrawRay(pos, dir * distance, Color.red);
        if(hit.collider != null)
        {
            if(applyLastCollider) {lastTouchingCollider = hit.collider.gameObject;}
            return true;
        }
        else { return false; }
    }
}
