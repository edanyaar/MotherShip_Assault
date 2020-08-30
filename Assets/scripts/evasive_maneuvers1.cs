using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class evasive_maneuvers1 : MonoBehaviour
{
    public Vector2 startWait;
    public Vector2 ManeuverTime;
    public Vector2 ManeuverWait;
    public float ManeuverDistance;
    public float smoothing;
    public float tilt;
    public Boundary boundary;
    private float targetManeuver;
    private float curspeed;
    private Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        curspeed = rb.velocity.z;
        StartCoroutine(evade());
    }

    IEnumerator evade()
    {
        yield return new WaitForSeconds(Random.Range(startWait.x,startWait.y));
        while (true){
            targetManeuver = Random.Range(1,ManeuverDistance) * (-Mathf.Sign(transform.position.x));
            yield return new WaitForSeconds(Random.Range(ManeuverTime.x,ManeuverTime.y));
            targetManeuver = 0;
            yield return new WaitForSeconds(Random.Range(ManeuverWait.x,ManeuverWait.y));
        }
    }

    void FixedUpdate()
    {
        float newManeuver = Mathf.MoveTowards(rb.velocity.x,targetManeuver,Time.deltaTime*smoothing);
        rb.velocity = new Vector3 (newManeuver,0.0f,curspeed);
        rb.position = new Vector3 (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );
        rb.rotation = Quaternion.Euler(0,0,rb.velocity.x*(-tilt));
    }
}
