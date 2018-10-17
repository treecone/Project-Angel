using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PhysicsScript : MonoBehaviour
{
    /// <summary>
    /// Theses values are tracked when colliding with the grid
    /// </summary>
    public enum SIDES
    {
        TOP, BOTTOM, LEFT, RIGHT,
        LEFTBODY, RIGHTBODY // used when an object is flush against the grid
    }

    private Dictionary<SIDES, bool> sides;
    private Dictionary<SIDES, List<Transform>> transforms = new Dictionary<SIDES, List<Transform>>();

    /// Movement
    public float horizontalVelocity, verticalVelocity;
    public float gravity;

    private Transform colliders;


    /// The min distance between a physics object and the tilemap
    private const float objectSpacing = 0.01f;

    public PhysicsScript()
    {
        this.sides = new Dictionary<SIDES, bool>();
        foreach (SIDES side in System.Enum.GetValues(typeof(SIDES)))
        {
            sides.Add(side, false);
            transforms.Add(side, new List<Transform>());
        }
    }

    public virtual void Start()
    {
        colliders = gameObject.transform.Find("ColliderPoints");
        foreach (Transform t in colliders)
        {
            System.Text.RegularExpressions.MatchCollection matchCollection
                = System.Text.RegularExpressions.Regex.Matches(t.name, "[a-z]+|[A-Z]([a-z])*");
            foreach (System.Text.RegularExpressions.Match match in matchCollection)
            {
                SIDES side = (SIDES)System.Enum.Parse(typeof(SIDES), match.Value.ToUpper());
                transforms[side].Add(t);
            }
        }
    }

    public virtual void Update()
    {
        Movement();
        CollisionRaycasts();
        GravityCalcuation();
    }

    private void Movement()
    {
        List<float> distanceToGround = new List<float>();
        foreach (Transform transform in transforms[SIDES.BOTTOM])
        {
            distanceToGround.Add(CollisionDistance(transform, Vector2.down, Mathf.Abs(verticalVelocity * Time.deltaTime)));
        }

        if ((sides[SIDES.BOTTOM] && verticalVelocity > 0) || distanceToGround.TrueForAll(x => x == 0))
            gameObject.transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime);
        else
        { // our current velocity will move us underground, move the object directly to ground level
            gameObject.transform.Translate(Vector2.down * (min(distanceToGround) - objectSpacing));
            verticalVelocity = 0;
        }


        List<float> distanceToWall = new List<float>();
        SIDES moveTowards = (Mathf.Sign(horizontalVelocity) > 0) ? SIDES.RIGHT : SIDES.LEFT;
        foreach (Transform transform in transforms[moveTowards])
        {
                distanceToWall.Add(
                    CollisionDistance(transform,
                    Vector2.right * Mathf.Sign(horizontalVelocity),
                    Mathf.Abs(horizontalVelocity * Time.deltaTime))
                    );
        }
        if (distanceToWall.TrueForAll(x => x == 0))
            gameObject.transform.Translate(Vector2.right * horizontalVelocity * Time.deltaTime);
        else
        {
            gameObject.transform.Translate(Vector2.right * Mathf.Sign(horizontalVelocity) * (min(distanceToWall) - objectSpacing));
            horizontalVelocity = 0;
            verticalVelocity = 0;
        }
    }

    private void GravityCalcuation()
    {
        if (sides[SIDES.BOTTOM])
        {
            if (verticalVelocity < 0) verticalVelocity = 0;
            if (sides[SIDES.LEFT] && sides[SIDES.RIGHT])
            { // The object is in the ground
                gameObject.transform.Translate(Vector2.up * 0.5f * Time.deltaTime);
            }
            else
            { // the object is floating without vertical velocity
                List<float> distanceToGround = new List<float>();
                foreach (Transform transform in transforms[SIDES.BOTTOM])
                {
                    distanceToGround.Add(CollisionDistance(transform, Vector2.down, Mathf.Abs(verticalVelocity * Time.deltaTime)));
                }

                if (verticalVelocity == 0 && distanceToGround.TrueForAll(x => x > objectSpacing))
                {
                    gameObject.transform.Translate(Vector2.down * (min(distanceToGround) - objectSpacing));
                }
            }

        }
        else if (sides[SIDES.LEFTBODY] || sides[SIDES.RIGHTBODY])
        { // Wall slide
            verticalVelocity += gravity / 2;
        }
        else
        {
            verticalVelocity += gravity;
        }


        if (sides[SIDES.TOP] && verticalVelocity > 0.05f) verticalVelocity = 0;
    }

    private void CollisionRaycasts()
    {
        sides[SIDES.TOP] = CheckingBothVectors(transforms[SIDES.TOP], Vector2.up);
        sides[SIDES.BOTTOM] = CheckingBothVectors(transforms[SIDES.BOTTOM], Vector2.down);
        sides[SIDES.RIGHT] = CheckingBothVectors(transforms[SIDES.RIGHT], Vector2.right);
        sides[SIDES.LEFT] = CheckingBothVectors(transforms[SIDES.LEFT], Vector2.left);
    }


    private bool CheckingBothVectors(List<Transform> points, Vector2 dir)
    {
        Vector2 pos1 = points[0].position;
        Vector2 pos2 = points[1].position;
        if (RaycastCollision(pos1, dir, objectSpacing * 2) != null
                && RaycastCollision(pos2, dir, objectSpacing * 2) != null
                && (dir == Vector2.right || dir == Vector2.left)
                ) //If both are touching
        {
            sides[SIDES.RIGHTBODY] = (dir == Vector2.right);
            sides[SIDES.LEFTBODY] = (dir == Vector2.left);
            return true;
        }
        else if (RaycastCollision(pos1, dir, objectSpacing * 2) != null || RaycastCollision(pos2, dir, objectSpacing * 2) != null)
        {
            return true;
        }
        else
        {
            if (dir == Vector2.right)
                sides[SIDES.RIGHTBODY] = false;
            else if (dir == Vector2.left)
                sides[SIDES.LEFTBODY] = false;
            return false;
        }
    }

    /// <summary>
    /// Returns the distance to the object the raycast hit, if none 0
    /// </summary>
    /// <param name="collider">A collider that the raycast should start at</param>
    /// <param name="dir">The direction the raycast travels</param>
    /// <param name="distance">The distance to travel before terminating</param>
    /// <returns>The distance to the object hit, otherwise null</returns>
    private float CollisionDistance(Transform collider, Vector2 direction, float distance)
    {
        return CollisionDistance(collider.position, direction, distance);
    }

    /// <summary>
    /// Returns the distance to the object the raycast hit, if none 0
    /// </summary>
    /// <param name="pos">The position the raycast should begin at</param>
    /// <param name="dir">The direction the raycast travels</param>
    /// <param name="distance">The distance to travel before terminating</param>
    /// <returns>The distance to the object hit, otherwise null</returns>
    private float CollisionDistance(Vector2 pos, Vector2 direction, float distanceCheck)
    {
        return Physics2D.Raycast(pos, direction, distanceCheck).distance;
    }

    /// <summary>
    /// Returns the object the raycast hit, if none then null
    /// </summary>
    /// <param name="pos">The position the raycast should begin at</param>
    /// <param name="dir">The direction the raycast travels</param>
    /// <param name="distance">The distance to travel before terminating</param>
    /// <returns>The object hit, otherwise null</returns>
    public GameObject RaycastCollision(Vector2 pos, Vector2 dir, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, distance);
        if (hit.collider != null) return hit.collider.gameObject;
        return null;
    }

    /// <summary>
    /// Get the sides that are in contact with the grid
    /// </summary>
    /// <returns>
    /// a new dictionary containing the state of the colliding sides
    /// </returns>
    public Dictionary<SIDES, bool> getCollsions()
    {
        return new Dictionary<SIDES, bool>(sides);
    }

    /// <summary>
    /// Get the transform points that make up the object
    /// </summary>
    /// <returns>
    /// a new dictionary containing the state of the tranform points
    /// </returns>
    public Dictionary<SIDES, List<Transform>> getTranforms()
    {
        return new Dictionary<SIDES, List<Transform>>(transforms);
    }


    public static float min(List<float> s)
    {
        if (s.Count < 1) return Mathf.NegativeInfinity;
        float min = s[0];
        foreach(float f in s)
        {
            min = Mathf.Min(min, f);
        }
        return min;
    }

    public static float max(List<float> s)
    {
        if (s.Count < 1) return Mathf.NegativeInfinity;
        float min = s[0];
        foreach (float f in s)
        {
            min = Mathf.Max(min, f);
        }
        return min;
    }
}
