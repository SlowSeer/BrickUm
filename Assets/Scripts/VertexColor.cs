using UnityEngine;
using System.Collections;

public class VertexColor : MonoBehaviour {

    public Color myColor;


    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;


	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];

        for (var i = 0; i < vertices.Length; i++) {
            colors[i] = Color.green;
        }
        mesh.colors = colors;
	}
	
	// Update is called once per frame
	void Update () {
        for (var i = 0; i < vertices.Length; i++) {
            colors[i] = Color.Lerp(mesh.colors[i], myColor, Time.deltaTime * 10);
        }
        mesh.colors = colors;
	}
}
