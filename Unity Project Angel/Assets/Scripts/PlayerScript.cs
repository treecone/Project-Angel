using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float horizontalSpeed;
    public float jumpForce;
    Rigidbody2D rb;


    void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        Movement();
    }

    void Movement()
    {
        if(Input.GetKey (KeyCode.A))
        {
            rb.velocity = new Vector2(horizontalSpeed * -1, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(horizontalSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce);
        }
    }

}
