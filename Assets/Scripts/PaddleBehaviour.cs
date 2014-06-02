using UnityEngine;
using System.Collections;

public class PaddleBehaviour : MonoBehaviour {

    // Components
    private Transform _transform;

    // Public Settings
    public Color paddleColor = Color.white;
    public float paddleSpeed = 50;
    
    public LayerMask paddleLayer;
    public LayerMask paddleMovementLayer;

    private Ray ray;
    private RaycastHit hit = new RaycastHit();

    public Transform paddleMain;
    public Transform paddleGrip;

    // Vertex Color Info.
    private Mesh paddleMainMesh;
    private Vector3[] paddleMainVertices;
    private Color[] paddleMainColors;

    private Mesh paddleGripMesh;
    private Vector3[] paddleGripVertices;
    private Color[] paddleGripColors;

    // The finger touching the paddle
    int paddleController = -1;

    // Defaults
    private Vector3 defaultScale;
    private Vector3 bigScale;
    private Vector3 smallScale;

    // Buffs - Big Paddle / Small Paddle
    private float buffTime = 0;
    private bool big = true;

    private float bigTime = 8;
    private float smallTime = 5;

    // Cache Components
    void Awake() {
        _transform = transform;
        defaultScale = transform.localScale;
        bigScale = defaultScale * 1.25f;
        smallScale = defaultScale * 0.75f;

        // Vertex Color Variables
        paddleMainMesh = paddleMain.GetComponent<MeshFilter>().mesh;
        paddleMainVertices = paddleMainMesh.vertices;
        paddleMainColors = new Color[paddleMainVertices.Length];

        paddleGripMesh = paddleGrip.GetComponent<MeshFilter>().mesh;
        paddleGripVertices = paddleGripMesh.vertices;
        paddleGripColors = new Color[paddleGripVertices.Length];
    }
	

	void Update () {
			
        if (GameManager.instance.gameSetup.showColorTransitions) {
            //Transition Paddle Main
            if (paddleMainMesh.colors.Length > 1 && paddleMainColors[0] != paddleColor) {
                for (var i = 0; i < paddleMainVertices.Length; i++) {
                    paddleMainColors[i] = Color.Lerp(paddleMainMesh.colors[i], paddleColor, Time.deltaTime * 1);
                }
                paddleMainMesh.colors = paddleMainColors;
            }
            //Transition Paddle Grip
            if (paddleGripMesh.colors.Length > 1 && paddleGripColors[0] != paddleColor) {
                for (var i = 0; i < paddleGripVertices.Length; i++) {
                    paddleGripColors[i] = Color.Lerp(paddleGripMesh.colors[i], paddleColor, Time.deltaTime * 1);
                }
                paddleGripMesh.colors = paddleGripColors;
            }
        } else {
            // No Transitions, Set Paddle Color.
            //if (paddleMainMesh.colors.Length > 1 && paddleMainColors[0] != paddleColor) SetPaddleMainColor(paddleColor);
            //if (paddleGripMesh.colors.Length > 1 && paddleGripColors[0] != paddleColor) SetPaddleGripColor(paddleColor);
			SetPaddleMainColor(paddleColor);
            SetPaddleGripColor(paddleColor);
        }
        if(GameManager.instance.gameState == GameState.Play){
            if (buffTime >= 0) {
                buffTime -= Time.deltaTime;
                _transform.localScale = Vector3.Lerp(_transform.localScale, (big) ? bigScale : smallScale , Time.deltaTime);
            } else {
                _transform.localScale = Vector3.Lerp(_transform.localScale, defaultScale, Time.deltaTime);
            }
        }
	}

    public void BigPaddle() {
        if (big) {
            buffTime += bigTime;
        } else {
            big = true;
            buffTime = bigTime;
        }
    }

    public void SmallPaddle() {
        if (!big) {
            buffTime += smallTime;
        } else {
            big = false;
            buffTime = smallTime;
        }
    }

    public void CheckTouch(myTouch t) {
        float xPos = _transform.position.x;

        if (t.phase == TouchPhase.Began) {
            if (paddleController < 0) { // No paddle controller? Lets check if anything is touching it
                ray = Camera.main.ScreenPointToRay(t.position);
                if (Physics.Raycast(ray, out hit, 10, paddleLayer)) {
                    paddleController = t.fingerId;
                }
            }

        } else if (t.phase == TouchPhase.Moved) {
            if (t.fingerId == paddleController) {
                ray = Camera.main.ScreenPointToRay(t.position);
                if (Physics.Raycast(ray, out hit, 10, paddleMovementLayer)) {

                    xPos = hit.point.x;
                    
                    // Clamp the paddle in the arena.
                    xPos = Mathf.Clamp(xPos, -2.5f, 2.5f);
                    _transform.position = Vector3.Lerp(
                                            _transform.position, 
                                            new Vector3(xPos, _transform.position.y, _transform.position.z), 
                                            Time.deltaTime * paddleSpeed);
                }
            }
        } else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) {
            if (t.fingerId == paddleController) {
                paddleController = -1;
            }
        }
    }

    public void BigPaddle(float t) {

    }


    void SetPaddleMainColor(Color c) {
        for (var i = 0; i < paddleMainVertices.Length; i++) { paddleMainColors[i] = c; }
        paddleMainMesh.colors = paddleMainColors;
    }

    void SetPaddleGripColor(Color c) {
        for (var i = 0; i < paddleGripVertices.Length; i++) { paddleGripColors[i] = c; }
        paddleGripMesh.colors = paddleGripColors;
    }

    void SetPaddleColor(Color c) {
        SetPaddleMainColor(c);
        SetPaddleGripColor(c);
    }


    public void Magnet() {
        if (GameManager.instance.gameSetup.showColorTransitions) {
            SetPaddleColor(Color.white);
        }
    }


    void OnCollisionEnter(Collision collision) {
        // Did the padel hit the ball? Do some cool stuff.
        if (collision.gameObject.tag == "Ball") {

            GameManager.instance.PaddleHit();

            if (GameManager.instance.gameSetup.showColorTransitions) {
                SetPaddleColor(paddleColor / 2);
            }
        }
    }
}
