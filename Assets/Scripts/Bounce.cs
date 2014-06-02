using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {

    // Components
    private Transform _transform;
    private CharacterController _controller;
    // Test Variables
    private Vector3 moveDir = new Vector3(0, -1, 0);



    private ControllerColliderHit _ground;

    // Cache Components
	void Awake () {
        _transform = this.transform;
        _controller = this.GetComponent<CharacterController>();
	}

    bool handled = true;
	void Update () {
        if (!handled) {
            moveDir = Vector3.Reflect(moveDir, _ground.normal);
            handled = true;
        }

        _controller.Move(moveDir.normalized * Time.deltaTime );
	}

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        _ground = hit;
        handled = false;
        float angle = Vector3.Angle(moveDir, _ground.normal);
        Debug.Log("angle = " + angle);
        
    }

    void OnGUI() {
        Debug.DrawLine(_transform.position, _transform.position + moveDir.normalized );

    }
}
