using UnityEngine;
using System.Collections;

public class ClickStart : MonoBehaviour {

    public GameObject gameManagerObject;
    private GameManager gameManager;

	void Awake () {
        gameManager = gameManagerObject.GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && guiTexture.HitTest(Input.GetTouch(0).position)) {
            StartGame();
        }
	}

    void OnMouseDown() {
        StartGame();
    }


    // Tell game manager to start the game then disable this game object.
    void StartGame() {
        gameManager.StartGame();
        gameObject.active = false;
    }
}
