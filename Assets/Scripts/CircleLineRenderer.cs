using UnityEngine;
using System.Collections;

public class CircleLineRenderer : MonoBehaviour {

    // Components
    private LineRenderer _lineRenderer;

    // Circle Settings
    public float timeOut = 1.0f;
    public int degreesPerSegment = 10;
    public float radialScale = 0.75f;
    Color temp = Color.red;

    private float curAngle = 0;

    void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
    }


	void Start () {
        _lineRenderer.SetVertexCount(360 / degreesPerSegment + 1);
        CreatePoints();
	}
	
	void CreatePoints(){
       float x, y;

       for (int i = 0; i < 360/degreesPerSegment + 1; i++){

          x=Mathf.Sin(curAngle * Mathf.Deg2Rad);
          y=Mathf.Cos(curAngle * Mathf.Deg2Rad);
          _lineRenderer.SetPosition(i, new Vector3(x, y, 0) * radialScale);
          curAngle += degreesPerSegment;
        }
    }


	void Update () {
        temp.a -= Time.deltaTime / timeOut;
        _lineRenderer.material.color = temp;

        if (temp.a <= 0.01) {
            //Resources.UnloadUnusedAssets();
            GameObject.Destroy(_lineRenderer.material);
            GameObject.Destroy(gameObject);
        }
	}


}
