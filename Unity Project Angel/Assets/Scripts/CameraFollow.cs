using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    public float followSpeed;

	void Start ()
    {
		
	}
	
	void LateUpdate ()
    {
        gameObject.transform.position = Vector3.Slerp(this.transform.position, new Vector3 (target.transform.position.x, target.transform.position.y, -10), followSpeed * Time.deltaTime);
	}
}
