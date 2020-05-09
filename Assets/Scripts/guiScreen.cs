using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guiScreen : MonoBehaviour {

    
    // List of all children.
    private List<GameObject> children = new List<GameObject>();

    // List of all buttons so we can disable them during transitions.
    private List<guiButton> buttons = new List<guiButton>();

    // Keep their default colors for fade-in.
    private Color[] defaultColor;

    void Start() {
        foreach (Transform trans in this.gameObject.transform) {
            children.Add(trans.gameObject);
            if (trans.gameObject.GetComponent<guiButton>() != null) {
                buttons.Add(trans.gameObject.GetComponent<guiButton>());
            }
        }

        defaultColor = new Color[children.Count];

        for (int i = 0; i < children.Count; i++) {
            defaultColor[i] = children[i].GetComponent<Renderer>().material.color;
        }
    }

    

    public void CheckTouch(myTouch t, RaycastHit hit) {
        foreach (guiButton b in buttons) {
            if (b.enabled) b.CheckTouch(t, hit);
        }
    }

    public void FadeOut(float time) {
        foreach (GameObject go in children) {
            StartCoroutine(FadeOut(go, time));
        }

        foreach (guiButton b in buttons) {
            b.enabled = false;
        }
    }

    public void FadeIn(float time) {
        foreach (guiButton b in buttons) {
            b.enabled = true;
        }

        for (int i = 0; i < children.Count; i++) {
            StartCoroutine(FadeTo(children[i], defaultColor[i], time));
        }
    }


    IEnumerator FadeOut(GameObject go, float time) {
        Color temp = go.GetComponent<Renderer>().material.color;
        while (temp.a > 0) {
            temp.a -= Time.deltaTime / time;
            go.GetComponent<Renderer>().material.color = temp;
            yield return null;
        }
        go.SetActiveRecursively(false);
    }

    IEnumerator FadeTo(GameObject go, Color color, float time) {
        go.SetActiveRecursively(true);
        Color temp = color;
        temp.a = 0.01f;
        while (temp.a < color.a) {
            temp.a += Time.deltaTime / time;
            go.GetComponent<Renderer>().material.color = temp;
            yield return null;
        }
    }
}
