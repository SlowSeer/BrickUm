using UnityEngine;
using System.Collections;

/// <summary>
/// Brick Level Data.
/// </summary>
public class BrickLevel{
    
    public Brick[] bricks;

    public BrickLevel() {
	
	}
}

/// <summary>
/// A save game, probably only have save games in free-play mode.
/// </summary>
public class BrickLevelSave : BrickLevel{

    public int lives;
    public int ballTransform;

    public BrickLevelSave() {

    }
}

/// <summary>
/// A class to keep bricks in for levels.
/// </summary>
[System.Serializable]
public class Brick {

    public int brickType = 1;
    public int life = 1;
    public bool oscillate = false;
    public float oscillateRate = 0.8f;

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public Brick() {

    }

    public Brick(Brick b) {
        brickType = b.brickType;
        life = b.life;
        oscillate = b.oscillate;
        oscillateRate = b.oscillateRate;

        position = b.position;
        rotation = b.rotation;
        scale = b.scale;
    }

}


/// <summary>
/// Use this to maintin a list of prefabs in the game manager.
/// </summary>
[System.Serializable]
public class BrickPrefabs {
    public GameObject brickSquare;
    public GameObject brickRectangle;
    public GameObject brickHexagon;
}