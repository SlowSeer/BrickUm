using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {
    // Components 
    private Transform _transform;

    // Public Settings
    public Transform target;
    public float speed = 50.0f;

	// Cache Components
	void Awake () {
        _transform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
        _transform.RotateAround(target.position, Vector3.Cross(_transform.position - target.position, _transform.forward), Time.deltaTime * speed);
	}
}
