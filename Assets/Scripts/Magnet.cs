using UnityEngine;
using System.Collections;

public class Magnet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && guiTexture.HitTest(Input.GetTouch(0).position)) {
            GameManager.instance.MagnetBall();
        }
    }

    void OnMouseDown() {
        Debug.Log("moused on magnet");
        GameManager.instance.MagnetBall();
    }
}
