using UnityEngine;
using System.Collections;

public class BallBehaviour : MonoBehaviour {

    public enum BallAnimationType{
        None,
        LookForward,    // 3D look forward.
        FlatForward,    // 2D look forward.
        FakeTorque      // 2D Fake physics torque.
    }

    // Components
    private Transform _transform;
    private Rigidbody _rigidbody;

    public Vector3 velocity {
        get { return _rigidbody.velocity; }
    }

    // Public Settings / Set-up
    public float speed = 1.0f;

    public Transform ballInner;
    public BallAnimationType ballAnimationType = BallAnimationType.None;

    public bool ballRotate = false;

    // Private Variables
    private Vector3 paused;
    private Ray ray;
    private RaycastHit hit = new RaycastHit();

    // Speed Increase in [FreePlay] mode.
    private float bumpTime = 0;
    private float bumpTimeInteval = 30; // bump up speed every 30 seconds.

    // Cache Components
    void Awake() {
        _transform = this.transform;
        _rigidbody = this.GetComponent<Rigidbody>();
    }

    // Drop the ball. Used when starting the game.
	public void Drop () {
        _rigidbody.velocity = Vector3.up * speed;
	}

    // Push the ball in a direction.
    public void Push(Vector3 direction) {
        _rigidbody.velocity = direction.normalized * speed;
    }

    // Check for touch.
    // Used at the start of game, can touch ball or press 'start' to begin the game.
    public void CheckTouch(myTouch t) {
        if (t.phase == TouchPhase.Began && GameManager.instance.gameState == GameState.Start) {
            ray = Camera.main.ScreenPointToRay(t.position);
            if (Physics.Raycast(ray, out hit, 10, 1<<gameObject.layer)) {
                Debug.Log("hit the ball: " + hit.transform.name);
                GameManager.instance.StartGame();
            }
        }
    }

    
	void Update () {
        if (GameManager.instance.gameState == GameState.Play) {

            // Playing in FreePlay mode? 
            // Speed up the ball as time goes on.
            if (GameManager.gameMode == GameMode.FreePlay) {
                bumpTime += Time.deltaTime;
                if(bumpTime >= bumpTimeInteval){
                    bumpTime = 0;
                    speed += 0.1f;
                }
            }

            // Resuming from GameState.Pause
            if (_rigidbody.velocity == Vector3.zero) {
                _rigidbody.velocity = paused;
                paused = Vector3.zero;
            }
            
            // If the ball has an inner model so some animation.
            if (ballInner != null) {
                if (ballAnimationType == BallAnimationType.LookForward) {
                    // Look in the direction of the velocity - 3D. (Bee, UFO)
                    ballInner.LookAt(ballInner.position + _rigidbody.velocity);
                } else if (ballAnimationType == BallAnimationType.FlatForward) {

                    // Look in the direction of the velocity - 2D. (Meteor)
                    Quaternion targetRotation = Quaternion.LookRotation(ballInner.forward, _rigidbody.velocity.normalized);
                    //ballInner.rotation = Quaternion.Slerp(ballInner.rotation, targetRotation, Time.deltaTime);
                    ballInner.rotation = targetRotation;

                } else if (ballAnimationType == BallAnimationType.FakeTorque) {
                    // Fake Torque - Its good enough for now.
                    ballInner.RotateAround(Vector3.forward, _rigidbody.velocity.x * Time.deltaTime);
                }
            }

            // Want the ball-inner to rotate? (UFO?)
            if (ballRotate) ballInner.RotateAround(ballInner.up, Time.deltaTime);

            // Failsafe: Check the balls distance from the center and reset if it goes through a wall.
            // There is a chance of this happening on [FreePlay] mode if the ball gets fast enough.
            if (Vector3.Distance(_transform.position, Vector3.zero) > 6) {
                _transform.position = GameManager.instance.ballSpawn.position;
                _rigidbody.velocity = Vector3.up;
            }

            // We want to keep the ball at a steady'ish velocity.
            //_rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, _rigidbody.velocity.normalized * speed, Time.deltaTime * 3);
            _rigidbody.velocity = _rigidbody.velocity.normalized * speed;

        } else if (GameManager.instance.gameState == GameState.Start) {
            // If the game is still in the start state, allow the ball to follow the paddle position.
            _rigidbody.velocity = (GameManager.instance.ballSpawn.position - _transform.position) * speed * 2;
        } else if (GameManager.instance.gameState == GameState.Over || GameManager.instance.gameState == GameState.Win) {
            _rigidbody.velocity = Vector3.zero;
        } else {
            if(paused == Vector3.zero){
                paused = _rigidbody.velocity;
                _rigidbody.velocity = Vector3.zero;
            }
        }      
	}

    

    void OnCollisionEnter(Collision collision) {
        
        if (collision.gameObject.tag == "Paddle") {
            //Debug.Log("Hit Paddle");
            // TESTING: Getting some funky physics with the ball colliding with a moving paddle.
            // Hopefully this works. Test on Android device for responsiveness.
            //Push(Vector3.Reflect(_rigidbody.velocity, collision.contacts[0].normal));

            // If ball angle is greater than 90, set it to 90
            if (Vector3.Angle(_rigidbody.velocity, Vector3.up) >= 90) {
                Push(collision.contacts[0].normal);
                Debug.Log("Angle Correction");
            }
        } 

        // Impliment minimum collision angles?
        // Doesnt seem to be a need for it now with the Magnet & no friction.
        /*if (collision.gameObject.tag == "SideWall") {
            // TODO: Impliment minimum angles.
            //float angle = (Vector3.Angle(_rigidbody.velocity, collision.contacts[0].normal) * Mathf.Rad2Deg) % 180;
            //Debug.Log("Hit SideWall: Angle - " + angle);

        }*/
            
        if (collision.gameObject.tag == "Trigger_GameOver") {
            GameManager.instance.GameOver();
        }
    }
}