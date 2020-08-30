using UnityEngine;
using System.Collections;

public class ReleaseOnClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseUp()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.None;
    }
}
