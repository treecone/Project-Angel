using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsScript : MonoBehaviour
{

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
        CollsionRaycasts();
    }

    public void LateUpdate()
    {

        if ((colBottom && verticalVelocity > 0) || RaycastCollision(theColliders.Find("BottomLeft").position, Vector2.down, Mathf.Abs(verticalVelocity * Time.deltaTime)) == null)
            gameObject.transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime);
        else
        {
            gameObject.transform.Translate(Vector2.down *
                (CollisionDistance(theColliders.Find("BottomLeft").position, Vector2.down, Mathf.Abs(verticalVelocity * Time.deltaTime))));
            verticalVelocity = 0;
        }

        gameObject.transform.Translate(Vector2.right * horizontalVelocity * Time.deltaTime);
    }

    void Movement()
    {
        //Gravity
        if (colBottom)
        {
            OneTimeCallDirection("Bottom");
            if (verticalVelocity < 0) verticalVelocity = 0;
            if (colLeft && colRight) gameObject.transform.Translate(Vector2.up * 0.3f * Time.deltaTime);
        }
        else
        {
            if (!colLeft && !colRight) verticalVelocity += gravity;
            bottomCheck = true;
        }

        if (colRight || colLeft)
        {
            if (colRight && horizontalVelocity > 0.05f || colLeft && horizontalVelocity < -0.05f)
            {
                horizontalVelocity = 0;
                verticalVelocity = 0;
            }
            if (!colBottom)
            {
                if (rightBothTouching) verticalVelocity += (gravity / 20);
                else verticalVelocity += gravity;
            }
        }

        if (colTop && verticalVelocity > 0.05f) verticalVelocity = 0;
    }

    void CollsionRaycasts()
    {
        colBottom = CheckingBothVectors(new[] {"BottomLeft", "BottomRight"}, Vector2.down);
        colRight = CheckingBothVectors(new[] { "BottomRight", "TopRight"}, Vector2.right);
        colLeft = CheckingBothVectors(new[] { "BottomLeft", "TopLeft"}, Vector2.left);
        colTop = CheckingBothVectors(new[] { "TopRight", "TopLeft"}, Vector2.up);
    }

    bool CheckingBothVectors(string[] pos, Vector2 dir)
    {
        Vector2 pos1 = theColliders.Find(pos[0]).position;
        Vector2 pos2 = theColliders.Find(pos[1]).position;
        if (RaycastCollision(pos1, dir, 0.2f) != null && RaycastCollision(pos2, dir, 0.2f) != null && (dir == Vector2.right || dir == Vector2.left)) //If both are touching
        {
            rightBothTouching = (dir == Vector2.right);
            leftBothTouching = (dir == Vector2.left);
            Debug.Log(dir.ToString());
            return true;
        }
        else if (RaycastCollision(pos1, dir, 0.2f) != null || RaycastCollision(pos2, dir, 0.2f) != null)
        {
            return true;
        }
        else
        {
            rightBothTouching = !(dir == Vector2.right);
            leftBothTouching = !(dir == Vector2.left);
            return false;
        }
    }

    /**
     * Report the distance to the nearest object within distanceCheck
     */
    float CollisionDistance(Vector2 pos, Vector2 dir, float distanceCheck)
    {
        return Physics2D.Raycast(pos, dir, distanceCheck).distance;
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
