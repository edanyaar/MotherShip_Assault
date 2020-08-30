using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ForceField))]
public class ImpactOnClick : MonoBehaviour {

    Collider collider;
    ForceField forceField;
	// Use this for initialization
	void Start () {
        collider = GetComponent<Collider>();
        forceField = GetComponent<ForceField>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (collider.Raycast(ray, out hit, 1000))
            {
                forceField.AddImpact(hit.point, -hit.normal);
            }
        }
	}
}
