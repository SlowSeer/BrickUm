using UnityEngine;
using System.Collections;

public class guiButton : MonoBehaviour {

    // Components
    private Material _material;
    private MeshCollider meshCollider;

    // Public Settings
    public LayerMask buttonLayer;

    public Texture buttonUpTexture;
    public Texture buttonDownTexture;
    public Texture buttonUnusableTexture;
    public Color buttonDownColor;

    public GameManager gameManager;
    public string methodCall;
    public bool onButtonDown = true;

    private bool buttonDown = false;
    private Color defaultColor;
    private Color unusableColor;

    private bool _usable = true;

    public bool usable {
        get { return _usable; }
        set {
            Usable(value);
            _usable = value;
        }
    }

    //Touch
    private int touchFinger = -1;
    
    // Cache Components and Defaults
	void Awake () {
        _material = this.renderer.material;
        defaultColor = _material.color;
        unusableColor = new Color(_material.color.r, _material.color.r, _material.color.r, _material.color.a * 0.25f );
	}

    // Press Button
    private void ButtonDown() {
        buttonDown = true;
        if (buttonDownTexture != null) { _material.mainTexture = buttonDownTexture; } 
        else {  _material.color = buttonDownColor; }

        if (onButtonDown && methodCall != "") gameManager.Invoke(methodCall, 0);
    }

    // Release Button
    private void ButtonUp(bool inside = true) {
        buttonDown = false;
        if (buttonUpTexture != null) { _material.mainTexture = buttonUpTexture; }
        else { _material.color = defaultColor; }

        if (!onButtonDown && inside && methodCall != "") gameManager.Invoke(methodCall, 0);
    }

    // handle usable changes.
    private void Usable(bool canUse) {
        if (canUse) {
            if (buttonUpTexture != null) { _material.mainTexture = buttonUpTexture; }
            //_material.color = defaultColor;
        } else {
            ButtonUp(false);
            if (buttonUnusableTexture != null) { _material.mainTexture = buttonUnusableTexture; }
            //_material.color = unusableColor;
        }
    }

    // Handle Touch events.
    public void CheckTouch(myTouch t, RaycastHit hit) {
        if (t.phase == TouchPhase.Began) {
            if (touchFinger < 0) {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                    touchFinger = t.fingerId;
                    if (!buttonDown && _usable) ButtonDown();
                }
            }
            
        } else if (t.phase == TouchPhase.Moved) {
            if (t.fingerId == touchFinger) {
                if (hit.collider == null || hit.collider.gameObject != this.gameObject) {
                    //finger moved out of button, set it to up.
                    if (buttonDown) ButtonUp(false);
                }
            }

        } else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) {
            if (t.fingerId == touchFinger) {
                touchFinger = -1;
                if (buttonDown) ButtonUp();
            }
        }
    }
}