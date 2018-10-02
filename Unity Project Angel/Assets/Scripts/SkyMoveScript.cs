using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyMoveScript : MonoBehaviour {

    public bool colLeft, colRight, colBottom, colTop;
    public float verticalSpeed, horizontalSpeed;
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
        gameObject.transform.Translate(Vector2.up * verticalSpeed * Time.deltaTime);
        gameObject.transform.Translate(Vector2.right * horizontalSpeed * Time.deltaTime);

        //Applying gravity
        if (colBottom)
        {
            if(verticalSpeed < 0)
            verticalSpeed = 0;
        }
        else if (colRight || colLeft)
        {
            verticalSpeed -= (gravity / 20);
        }
        else
        {
            verticalSpeed -= gravity;
        }

        //Stopping player when running into wall
        if (colRight) { if(horizontalSpeed > 0.05f) { horizontalSpeed = 0; verticalSpeed = 0; } }
        if (colLeft) { if (horizontalSpeed < -0.05f) { horizontalSpeed = 0; verticalSpeed = 0; } }
        if (colTop) { if (verticalSpeed > 0.05f) { verticalSpeed = 0; } }
    }

    void MovementInput ()
    {
        //Jump
        if(Input.GetKeyDown (KeyCode.Space) && (colBottom || (colRight || colLeft)))
        {
            verticalSpeed = jumpForce;
        }

        //Left and Right
        if (Input.GetKey(KeyCode.A) && !colLeft)
        {
            horizontalSpeed = -speed;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (Input.GetKey(KeyCode.D) && !colRight)
        {
            horizontalSpeed = speed;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        if (!Input.GetKey(KeyCode.D) != false && !Input.GetKey(KeyCode.A) != false)
        {
            horizontalSpeed = 0;
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
