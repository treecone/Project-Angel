using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsScript : MonoBehaviour {

    //Movement
    public float horizontalVelocity, verticalVelocity;
    public float gravity;


    bool leftCheck, topCheck, rightCheck, bottomCheck; //This is to active when first touched
    public bool colLeft, colTop, colRight, colBottom; //This activates when and after first touched

    Transform theColliders;


    public virtual void Start ()
    {
        theColliders = gameObject.transform.Find("ColliderPoints");
    }
	
	public virtual void Update ()
    {
        Movement();
        CollsionRaycasts();
    }

    void Movement ()
    {
        gameObject.transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime);
        gameObject.transform.Translate(Vector2.right * horizontalVelocity * Time.deltaTime);

        //Gravity
        if (colBottom == true)
        {
            OneTimeCallDirection("Bottom");
            if (verticalVelocity < 0)
                verticalVelocity = 0;
        }
        else
        {
            if(!colLeft && !colRight)
            {
                verticalVelocity += gravity;
            }
            bottomCheck = true;
        }

        if (colRight) { if (horizontalVelocity > 0.05f) { horizontalVelocity = 0; verticalVelocity = 0; } if (!colBottom) { verticalVelocity += (gravity / 20); } }
        if (colLeft) { if (horizontalVelocity < -0.05f) { horizontalVelocity = 0; verticalVelocity = 0; } if (!colBottom) { verticalVelocity += (gravity / 20); } }
        if (colTop) { if (verticalVelocity > 0.05f) { verticalVelocity = 0; } }
    }

    void CollsionRaycasts ()
    {       
        colBottom = CheckingBothVectors(theColliders.Find("BottomLeft").position, theColliders.Find("BottomRight").position, Vector2.down);
        colRight = CheckingBothVectors(theColliders.Find("BottomRight").position, theColliders.Find("TopRight").position, Vector2.right);
        colLeft = CheckingBothVectors(theColliders.Find("BottomLeft").position, theColliders.Find("TopLeft").position, Vector2.left);
        colTop = CheckingBothVectors(theColliders.Find("TopRight").position, theColliders.Find("TopLeft").position, Vector2.up);
    }

    bool CheckingBothVectors (Vector2 pos1, Vector2 pos2, Vector2 dir)
    {
        if (RaycastCollision(pos1, dir, 0.2f) != null)
        {
            return true;
        }
        else if (RaycastCollision(pos2, dir, 0.2f) != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //returns the object that a raycast hits, else returns null
    GameObject RaycastCollision(Vector2 pos, Vector2 dir, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, distance);
        Debug.DrawRay(pos, dir * distance, Color.red);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else { return null; }
    }

    void OneTimeCallDirection (string dir)
    {
        switch (dir)
        {
            case ("Bottom"):
                if(bottomCheck == true)
                {
                    //Do stuff for the one time
                    bottomCheck = false;
                }
                break;

            case ("Left"):
                break;

            case ("Right"):
                break;

            case ("Top"):
                break;
        }
    }
}
