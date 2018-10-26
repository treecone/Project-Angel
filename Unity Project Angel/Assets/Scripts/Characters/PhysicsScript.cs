using Assets.Scripts.Characters;
using Assets.Scripts.Tiles;
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
        TOP, BOTTOM, LEFT, RIGHT
    }
    
    /// <summary>
    /// Stores the objects that a character touches
    /// </summary>
    protected HashSet<GameObject> touching = new HashSet<GameObject>(); 
    protected Dictionary<SIDES, List<GameObject>> sides;
    protected Dictionary<SIDES, List<Transform>> transforms = new Dictionary<SIDES, List<Transform>>();

    /// Movement
    public float horizontalVelocity, verticalVelocity;
    public float gravity;
    private CharacterScript character;
    private Transform colliders;

    // Mask
    int GroundMask = 0;


    /// The min distance between a physics object and the tilemap
    private const float objectSpacing = 0.01f;

    public PhysicsScript()
    {
        this.sides = new Dictionary<SIDES, List<GameObject>>();
        foreach (SIDES side in System.Enum.GetValues(typeof(SIDES)))
        {
            sides.Add(side, new List<GameObject>());
            transforms.Add(side, new List<Transform>());
        }
    }

    public virtual void Start(CharacterScript character)
    {
        this.GroundMask = LayerMask.GetMask("Ground");
        this.character = character;
        colliders = gameObject.transform.Find("ColliderPoints");
        foreach (Transform t in colliders)
        {
            System.Text.RegularExpressions.MatchCollection matchCollection
                = System.Text.RegularExpressions.Regex.Matches(t.name, "[a-z]+|[A-Z]([a-z])*");
            foreach (System.Text.RegularExpressions.Match match in matchCollection)
            {
                try
                {
                    SIDES side = (SIDES)System.Enum.Parse(typeof(SIDES), match.Value.ToUpper());
                    transforms[side].Add(t);
                }
                catch (System.ArgumentException ex)
                {
                    Debug.Log("Collider point " + ex + " is not a valid side");
                }
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

        if ((ContactingSide(SIDES.BOTTOM) && verticalVelocity > 0) || distanceToGround.TrueForAll(x => x == 0))
            gameObject.transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime);
        else
        { // our current velocity will move us underground, move the object directly to ground level
            gameObject.transform.Translate(Vector2.down * (Min(distanceToGround) - objectSpacing));
            verticalVelocity = 0;
        }


        List<float> distanceToWall = new List<float>();
        // Find the side that the object is moving towards
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
            gameObject.transform.Translate(Vector2.right * Mathf.Sign(horizontalVelocity) * (Max(distanceToWall) - objectSpacing));
            horizontalVelocity = 0;
            verticalVelocity = 0;
        }
    }

    private void GravityCalcuation()
    {
        if (ContactingSide(SIDES.BOTTOM))
        {
            if (verticalVelocity < 0) verticalVelocity = 0;
            if (ContactingSide(SIDES.LEFT) && ContactingSide(SIDES.RIGHT))
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
                    gameObject.transform.Translate(Vector2.down * (Min(distanceToGround) - objectSpacing));
                }
            }

        }
        else if (verticalVelocity <= 0 && (FlushWithSide(SIDES.LEFT) || FlushWithSide(SIDES.RIGHT)))
        { // Wall slide
            verticalVelocity += gravity / 2;
        }
        else
        {
            verticalVelocity += gravity;
        }


        if (ContactingSide(SIDES.TOP) && verticalVelocity > 0.05f) verticalVelocity = 0;
    }

    private void CollisionRaycasts()
    {
        HashSet<GameObject> newTouching = new HashSet<GameObject>();
        newTouching.UnionWith(CheckGroundColliders(SIDES.TOP, Vector2.up));
        newTouching.UnionWith(CheckGroundColliders(SIDES.BOTTOM, Vector2.down));
        newTouching.UnionWith(CheckGroundColliders(SIDES.RIGHT, Vector2.right));
        newTouching.UnionWith(CheckGroundColliders(SIDES.LEFT, Vector2.left));

        HashSet<GameObject> dup = new HashSet<GameObject>(newTouching);
        dup.ExceptWith(touching);
        foreach(GameObject gameObject in dup)
        {
            foreach(ITile tile in gameObject.GetComponents(typeof(ITile)))
            {
                tile.Touch(character);
            }
        }
        dup = new HashSet<GameObject>(newTouching);
        touching.ExceptWith(dup);
        foreach (GameObject gameObject in touching)
        {
            foreach (ITile tile in gameObject.GetComponents(typeof(ITile)))
            {
                tile.Part(character);
            }
        }


        touching = newTouching;
    }


    private HashSet<GameObject> CheckGroundColliders(SIDES side, Vector2 dir)
    {
        HashSet<GameObject> toucing = new HashSet<GameObject>();
        List<GameObject> colliders = new List<GameObject>();
        foreach (Transform x in transforms[side]) {
            GameObject temp = RaycastCollision(x.position, dir, objectSpacing * 2);
            if (temp != null)
            {
                toucing.Add(temp);
                if (temp.GetComponent(typeof(Assets.Scripts.Tiles.Ground.IGround)) != null)
                    colliders.Add(temp);
            }
        }
        sides[side] = colliders;
        return toucing;
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
    /// Returns the distance from the 
    /// </summary>
    /// <param name="pos">The position the raycast should begin at</param>
    /// <param name="dir">The direction the raycast travels</param>
    /// <param name="distance">The distance to travel before terminating</param>
    /// <returns>The distance to the object hit, otherwise null</returns>
    private float CollisionDistance(Vector2 pos, Vector2 direction, float distanceCheck)
    {
        return Physics2D.Raycast(pos, direction, distanceCheck, GroundMask).distance;
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
    public Dictionary<SIDES, List<GameObject>> GetCollsions()
    {
        return new Dictionary<SIDES, List<GameObject>>(sides);
    }

    /// <summary>
    /// Get the transform points that make up the object
    /// </summary>
    /// <returns>
    /// a new dictionary containing the state of the tranform points
    /// </returns>
    public Dictionary<SIDES, List<Transform>> GetTranforms()
    {
        return new Dictionary<SIDES, List<Transform>>(transforms);
    }

    public bool FlushWithSide(SIDES side)
    {
        return transforms[side].Count == this.sides[side].Count;
    }

    public bool ContactingSide(SIDES side)
    {
        return this.sides[side].Count > 0;
    }

    public static float Min(List<float> s)
    {
        if (s.Count < 1) return Mathf.NegativeInfinity;
        float min = s[0];
        foreach(float f in s)
        {
            min = Mathf.Min(min, f);
        }
        return min;
    }

    public static float Max(List<float> s)
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
