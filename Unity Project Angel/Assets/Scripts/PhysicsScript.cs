using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsScript : MonoBehaviour
{

    //The min distance between a physics object and the tilemap
    private const float objectSpacing = 0.01f;

    //Movement
    public float horizontalVelocity, verticalVelocity;
    public float gravity;


    bool leftCheck, topCheck, rightCheck, bottomCheck; //This is to active when first touched
    public bool colLeft, colTop, colRight, colBottom; //This activates when and after first touched

    Transform theColliders;
    public bool leftBothTouching, rightBothTouching;


    public virtual void Start()
    {
        theColliders = gameObject.transform.Find("ColliderPoints");
    }

    public virtual void Update()
    {
        Movement();
        CollisionRaycasts();
        GravityCalcuation();
    }


    private void Movement()
    {
        //Distance to ground is 0 if the object does not hit the ground on this update
        float[] distanceToGround = new float[] {
            CollisionDistance(theColliders.Find("BottomLeft"), Vector2.down, Mathf.Abs(verticalVelocity * Time.deltaTime)),
            CollisionDistance(theColliders.Find("BottomRight"), Vector2.down, Mathf.Abs(verticalVelocity * Time.deltaTime))
        };
        if ((colBottom && verticalVelocity > 0) || distanceToGround[0] == 0 && distanceToGround[1] == 0)
            gameObject.transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime);
        else
        { // our current velocity will move us underground, move the object directly to ground level
            gameObject.transform.Translate(Vector2.down * (distanceToGround[0] - objectSpacing));
            verticalVelocity = 0;
        }

        float[] distaceToWall = new float[]
        {
            CollisionDistance(theColliders.Find((Mathf.Sign(horizontalVelocity) > 0)?"TopRight":"TopLeft"), Vector2.right * Mathf.Sign(horizontalVelocity), Mathf.Abs(horizontalVelocity * Time.deltaTime)),
            CollisionDistance(theColliders.Find((Mathf.Sign(horizontalVelocity) > 0)?"BottomRight":"BottomLeft"), Vector2.right * Mathf.Sign(horizontalVelocity), Mathf.Abs(horizontalVelocity * Time.deltaTime))
        };
        if (distaceToWall[0] == 0 && distaceToWall[1] == 0)
            gameObject.transform.Translate(Vector2.right * horizontalVelocity * Time.deltaTime);
        else
        {
            gameObject.transform.Translate(Vector2.right * Mathf.Sign(horizontalVelocity) * (Mathf.Max(distaceToWall) -objectSpacing));
            horizontalVelocity = 0;
            verticalVelocity = 0;
        }
    }

    void GravityCalcuation()
    {
        if (colBottom)
        {
            OneTimeCallDirection("Bottom");
            if (verticalVelocity < 0) verticalVelocity = 0;
            if (colLeft && colRight)
            { // The object is in the ground
                gameObject.transform.Translate(Vector2.up * 0.5f * Time.deltaTime);
            }
            else
            { // the object is floating without vertical velocity
                float[] distanceToGround = new float[] {
                CollisionDistance(theColliders.Find("BottomLeft"), Vector2.down, Mathf.Infinity),
                CollisionDistance(theColliders.Find("BottomRight"), Vector2.down, Mathf.Infinity)
            };

                if (verticalVelocity == 0 && distanceToGround[0] > objectSpacing && distanceToGround[1] > objectSpacing)
                {
                    gameObject.transform.Translate(Vector2.down * (Mathf.Min(distanceToGround) - objectSpacing));
                }
            }

        }
        else if (colLeft || colRight)
        { // Wall slide
            verticalVelocity += gravity / 2;
        }
        else
        {
            verticalVelocity += gravity;
            bottomCheck = true;
        }


        if (colTop && verticalVelocity > 0.05f) verticalVelocity = 0;
    }

    void CollisionRaycasts()
    {
        colBottom = CheckingBothVectors(new[] { "BottomLeft", "BottomRight" }, Vector2.down);
        colRight = CheckingBothVectors(new[] { "BottomRight", "TopRight" }, Vector2.right);
        colLeft = CheckingBothVectors(new[] { "BottomLeft", "TopLeft" }, Vector2.left);
        colTop = CheckingBothVectors(new[] { "TopRight", "TopLeft" }, Vector2.up);
    }

    bool CheckingBothVectors(string[] pos, Vector2 dir)
    {
        Vector2 pos1 = theColliders.Find(pos[0]).position;
        Vector2 pos2 = theColliders.Find(pos[1]).position;
        if (RaycastCollision(pos1, dir, objectSpacing * 2) != null && RaycastCollision(pos2, dir, objectSpacing * 2) != null && (dir == Vector2.right || dir == Vector2.left)) //If both are touching
        {
            rightBothTouching = (dir == Vector2.right);
            leftBothTouching = (dir == Vector2.left);
            return true;
        }
        else if (RaycastCollision(pos1, dir, objectSpacing * 2) != null || RaycastCollision(pos2, dir, objectSpacing * 2) != null)
        {
            return true;
        }
        else
        {
            rightBothTouching = (dir == Vector2.right) ? false : rightBothTouching;
            leftBothTouching = (dir == Vector2.left) ? false : leftBothTouching;
            return false;
        }
    }

    private float CollisionDistance(Transform collider, Vector2 direction, float distance)
    {
        return CollisionDistance(collider.position, direction, distance);
    }

    /**
     * Report the distance to the nearest object within distanceCheck
     */
    private float CollisionDistance(Vector2 pos, Vector2 direction, float distanceCheck)
    {
        return Physics2D.Raycast(pos, direction, distanceCheck).distance;
    }

    //returns the object that a raycast hits, else returns null
    GameObject RaycastCollision(Vector2 pos, Vector2 dir, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, distance);
        Debug.DrawRay(pos, dir * distance, Color.red);
        if (hit.collider != null) return hit.collider.gameObject;
        return null;
    }

    void OneTimeCallDirection(string dir)
    {
        switch (dir)
        {
            case ("Bottom"):
                bottomCheck = false;
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
