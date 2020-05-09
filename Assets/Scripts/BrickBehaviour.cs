using UnityEngine;
using System.Collections;

public class BrickBehaviour : MonoBehaviour {

    // Components
    private Transform _transform;

    // Public Settings
    public Brick brick = new Brick(); // Brick settings
    //public int brickType = 0; // The prefab type.
    public int listPosition; // Position of the brick in the list.

    // Default Settings
    private Vector3 defaultScale;

    // Private Settings
    private Color brickColor = Color.white;
    private float colorSpeed = 5.0f;

    // Private Settings - Oscilate
    private bool up = true; // Oscillating up?
    private float transitionTime = 0;

    // Vertex Color Info.
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;


    // Cache Components
    void Awake() {
        _transform = this.transform;

        // Vertex Color Variables
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];
    }

    // Set Defaults
	void Start () {
        // Defaults
        //defaultScale = _transform.localScale;
        brick.position = _transform.position;
        brick.rotation = _transform.rotation;
        brick.scale = _transform.localScale;
	}

    /// <summary>
    /// Load a brick and set scale (position/rotation will be handled by the instantiate)
    /// </summary>
    /// <param name="b">The brick to be loaded.</param>
    /// <param name="b">Index position in the main list of bricks.</param>
    public void Load( Brick b , int listPos) {
        brick = new Brick(b);
        listPosition = listPos;
        _transform.localScale = b.scale; 
        defaultScale = b.scale;
         SetColor(GameManager.instance.brickTheme.brickColors[b.life]);
    }

    public void SetColor(Color c) {
        for (int i = 0; i < vertices.Length; i++) colors[i] = c;
        mesh.colors = colors;
    }

    public Brick GetBrick() {
        brick.position = _transform.position;
        brick.rotation = _transform.rotation;
        brick.scale = _transform.localScale;
        return brick;
    }
    
	void Update () {
        // set up color transition speed.
        float tempSpeed = (brick.oscillate) ? 8 : colorSpeed;
        brickColor = GameManager.instance.brickTheme.brickColors[brick.life];
        
        if (mesh.colors.Length > 1 && colors[0] != brickColor) {
            if (GameManager.instance.brickTheme != null) {
                for (var i = 0; i < vertices.Length; i++) {
                    if (GameManager.instance.gameSetup.showColorTransitions) {
                        colors[i] = Color.Lerp(mesh.colors[i], GameManager.instance.brickTheme.brickColors[brick.life], Time.deltaTime * tempSpeed);
                    } else {
                        colors[i] = brickColor;
                    }
                }
                mesh.colors = colors;
            }
        }

        // Color transitions are fine when not in play mode, but scale isnt.
#if UNITY_EDITOR
        if (GameManager.instance.gameState == GameState.Play) {
#else
        if (GameManager.instance.gameState == GameState.Play || GameManager.instance.gameState == GameState.Start) {
#endif
            // Oscillate behaviour
            if (brick.oscillate) {
                transitionTime += Time.deltaTime;
                if (transitionTime >= brick.oscillateRate) {
                    transitionTime -= brick.oscillateRate;
                    if (brick.life >= 5) up = false;
                    else if (brick.life <= 1) up = true;
                    brick.life = (up) ? brick.life + 1 : brick.life - 1;
                }
            }
        }

        if (GameManager.instance.gameState == GameState.Play
         || GameManager.instance.gameState == GameState.Over
         || GameManager.instance.gameState == GameState.Win) {
            // Lerp scale back to default - Removed scale changes for batching.
            if (_transform.localScale != defaultScale) {
                _transform.localScale = Vector3.Lerp(_transform.localScale, defaultScale, Time.deltaTime * 5);
            }


            if (brick.life < 1) {
            }
        }
	}

    void SetLife(int life) {
        brick.life = life;
        SetColor(GameManager.instance.brickTheme.brickColors[brick.life]);
    }

    // Indicate a hit on the brick
    public void BrickHit(Vector3 pos) {
        brick.oscillate = false;
        brick.life--;
        SetColor(Color.white);
        _transform.localScale = defaultScale * 0.9f;

        if (brick.life <= 0) {
            // Brick was hit & destroyed.
            GameManager.instance.BrickHit(pos, gameObject, listPosition, true);

            defaultScale = Vector3.zero;
            this.GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 0.5f); // Destroy Brick

        } else {
            GameManager.instance.BrickHit(pos, gameObject, listPosition);
        }
    }

    void OnCollisionEnter(Collision collision) {
        // Did the ball hit a brick? Do some cool stuff.
        if (collision.gameObject.tag == "Ball") {
            BrickHit(collision.transform.position);
        }
    }
}