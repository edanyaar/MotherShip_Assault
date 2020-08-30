using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_mover : MonoBehaviour
{
    public float speed;
    public float distance_from_top;

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
    private bool done_moving;
    void Start()
    {
        done_moving = false;
        rb = gameObject.GetComponentInParent<Rigidbody>();
        StartCoroutine(move_boss());
    }

    IEnumerator move_boss(){
        rb.velocity = new Vector3(0,0,-1)*speed;
        yield return new WaitForSeconds (distance_from_top);
        rb.velocity = new Vector3(0,0,0);
        curspeed = 0;
        done_moving = true;
        StartCoroutine(evade());
        yield return new WaitForSeconds (distance_from_top);
    }

    IEnumerator evade()
    {
        yield return new WaitForSeconds(Random.Range(startWait.x,startWait.y));
        while (true){
            targetManeuver = Random.Range(1,ManeuverDistance) * (-Mathf.Sign(rb.position.x));
            yield return new WaitForSeconds(Random.Range(ManeuverTime.x,ManeuverTime.y));
            targetManeuver = 0;
            yield return new WaitForSeconds(Random.Range(ManeuverWait.x,ManeuverWait.y));
        }
    }

    void FixedUpdate()
    {
        if(done_moving){
            float newManeuver = Mathf.MoveTowards(rb.velocity.x,targetManeuver,Time.deltaTime*smoothing);
            rb.velocity = new Vector3 (newManeuver,0.0f,curspeed);
            rb.position = new Vector3 (
                Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
                Mathf.Clamp(rb.position.y, boundary.zMin, boundary.zMax),
                Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );
            rb.rotation = Quaternion.Euler(0,0,rb.velocity.x*(-tilt));
        }
    }
}
