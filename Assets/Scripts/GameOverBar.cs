using UnityEngine;
using System.Collections;

public class GameOverBar : MonoBehaviour {
    // Side Objects
    public GameObject hexLeft;
    public GameObject hexRight;

    // Life of the bar.
    public int life = 3;
    public float hitTolerance = 0.75f;
    public Color barColor = Color.white;

    private float hitTime = 0;

    // Vertex Color Info.
    private Mesh meshBar;
    private Vector3[] verticesBar;
    private Color[] colorsBar;

    private Mesh meshHexLeft;
    private Vector3[] verticesHexLeft;
    private Color[] colorsHexLeft;

    private Mesh meshHexRight;
    private Vector3[] verticesHexRight;
    private Color[] colorsHexRight;

	// Cache Components
	void Awake () {
        // Vertex Color Variables
        meshBar = GetComponent<MeshFilter>().mesh;
        verticesBar = meshBar.vertices;
        colorsBar = new Color[verticesBar.Length];
        // Vertex Color Variables
        meshHexLeft = hexLeft.GetComponent<MeshFilter>().mesh;
        verticesHexLeft = meshHexLeft.vertices;
        colorsHexLeft = new Color[verticesHexLeft.Length];
        // Vertex Color Variables
        meshHexRight = hexRight.GetComponent<MeshFilter>().mesh;
        verticesHexRight = meshHexRight.vertices;
        colorsHexRight = new Color[verticesHexRight.Length];
	}

    void Start() {
        SetColor(GameManager.instance.brickTheme.brickColors[Mathf.Clamp(life, 0, 5)]);
    }

	// Update is called once per frame
	void Update () {
        
        // Re-enable if any lives are gained from 0.
        if (!GetComponent<Renderer>().enabled && life > 0) { Enable(true); }

        // Bar Color.
        if (GetComponent<Renderer>().enabled) {
            barColor = GameManager.instance.brickTheme.brickColors[Mathf.Clamp(life, 0, 5)];
            if (GameManager.instance.gameSetup.showColorTransitions) {
                if (meshBar.colors.Length > 1 && colorsBar[0] != barColor)
                    SetColor(Color.Lerp(colorsBar[0], barColor, Time.deltaTime * 5));
            } else {
                if (meshBar.colors.Length > 1 && colorsBar[0] != barColor)
                    SetColor(barColor);
            }
        }
	}

    public void SetColor(Color c) {
        for (int i = 0; i < verticesBar.Length; i++) colorsBar[i] = c;
        meshBar.colors = colorsBar;

        for (int i = 0; i < verticesHexLeft.Length; i++) colorsHexLeft[i] = c;
        meshHexLeft.colors = colorsHexLeft;

        for (int i = 0; i < verticesHexRight.Length; i++) colorsHexRight[i] = c;
        meshHexRight.colors = colorsHexRight;
    }

    public void OneUp() {
        life++;
    }

    public void Enable(bool e) {
            GetComponent<Renderer>().enabled = e;
            GetComponent<Collider>().enabled = e;
    }

    void OnCollisionEnter(Collision collision) {
        // Did the ball hit a brick? Do some cool stuff.
        if (collision.gameObject.tag == "Ball") {

            // Only take one life per second.
            if ((Time.time - hitTime) > hitTolerance && life > 0) {
                life--;
                hitTime = Time.time;
                SetColor(Color.white); // Indicate a hit on the bar
                
                // If the bar has no life and the GameMode isnt KidMode, disable the bar.
                if (life <= 0 && GameManager.gameMode != GameMode.KidMode) {
                    Enable(false);
                }
            }
        }
    }
}
