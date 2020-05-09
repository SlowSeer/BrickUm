using UnityEngine;
using System.Collections;

public class GameBuffButton : MonoBehaviour {

    // Components
    private Transform _transform;
    private Renderer _renderer;


    // Buff Button Setup
    public float speed = 0.5f;
    public GameBuffAction gameBuffAction = GameBuffAction.Mystery;

    // Private variabes
    private float createTime;
    private Vector3 direction = Vector3.up;
    public bool fadingOut = false;

    private Vector3 defaultScale;
    private Vector3 miniScale;

    private Color defaultColor;
    private Color miniColor;

    // Touch Variables.
    //private bool touched = false;
    private Ray ray;
    private RaycastHit hit = new RaycastHit();


    // Cache Components
    void Awake() {
        _transform = transform;
        _renderer = GetComponent<Renderer>();
    }


    // Setup
	void Start () {
        // Time the button was created.
        createTime = Time.time;

        // Set up the default and goto scale values.
        defaultScale = _transform.localScale;
        miniScale = _transform.localScale * 0.3f;
        _transform.localScale = miniScale;

        // Set up the default and goto color values.
        defaultColor = _renderer.material.color;
        miniColor = _renderer.material.color; miniColor.a *= 0.01f;
        _renderer.material.color = miniColor;

        //Initial direction of the button.
        direction = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * direction;

        StartCoroutine(FadeIn());
	}
	

	// Logic
	void Update () {
        if (GameManager.instance.gameState == GameState.Play) {
            _transform.Translate(direction.normalized * Time.deltaTime * speed);

            if (Time.time - createTime > 3 && !fadingOut) {
                StartCoroutine(FadeOut());
                fadingOut = true;
            }
        }
	}

    void FadeOutButton() {

    }

    public void SetDirection(Vector3 dir) {
        direction = dir;
        direction.z = 0;
    }

    public bool CheckTouch(myTouch t, RaycastHit hit) {
        if (t.phase == TouchPhase.Began) {
            if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                Touched();
                return true;
            }
        }
        return false;
    }

    void Touched() {
        //touched = true;
        switch (gameBuffAction) {
            case GameBuffAction.Mystery:
                Debug.Log("Mystery button touched");
                GetComponent<ParticleSystem>().Play();
                GameManager.instance.BuffMystery();
                break;
            case GameBuffAction.OneUp:
                Debug.Log("OneUp button touched");
                GetComponent<ParticleSystem>().Play();
                GameManager.instance.BuffOneUp();
                break;
            case GameBuffAction.Bomb:
                Debug.Log("Bomb button touched");
                GetComponent<ParticleSystem>().Play();
                GameManager.instance.BuffBomb();
                break;
            case GameBuffAction.BigPaddle:
                Debug.Log("BigPaddle button touched");
                GetComponent<ParticleSystem>().Play();
                GameManager.instance.BuffBigPaddle();
                break;
        }

        DisableButton();
    }

    // the button has been pressed, so disable it.
    void DisableButton() {
        GameManager.instance.RemoveBuffButton(this);
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GameObject.Destroy(gameObject, 1.0f); // Destroy in a sec to allow particles to play out
    }

    IEnumerator FadeIn() {
        bool c = false;
        bool s = false;
        while (!c || !s) {
            //if (touched) break;

            if (!c && _renderer.material.color != defaultColor) {
                _renderer.material.color = Color.Lerp(_renderer.material.color, defaultColor, Time.deltaTime);
            } else { c = true; }

            if (!s && _transform.localScale != defaultScale) {
                _transform.localScale = Vector3.Lerp(_transform.localScale, defaultScale, Time.deltaTime);
            } else { s = true; }
            
            yield return null;
        }
    }

    public bool c = false;
    public bool s = false;

    IEnumerator FadeOut() {
        //c = false;
        //s = false;
        while (!c || !s) {
            //if (touched) break;

            if (!c && _renderer.material.color != miniColor) {
                _renderer.material.color = Color.Lerp(_renderer.material.color, miniColor, Time.deltaTime);
            } else { c = true; }

            if (!s && _transform.localScale != miniScale) {
                _transform.localScale = Vector3.Lerp(_transform.localScale, miniScale, Time.deltaTime);
            } else { s = true; }

            yield return null;
        }

        DisableButton();
    }
}

// Button Types.
// There wont be negative types, just a chance to get a negative effect from the mystery button. (small paddle)
public enum GameBuffAction {
    Mystery,
    OneUp,
    BigPaddle,
    Bomb
}