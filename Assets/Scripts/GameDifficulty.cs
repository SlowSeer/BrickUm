using UnityEngine;
using System.Collections;

public class GameDifficulty {
    public static GameDifficulty Easy = new GameDifficulty(1.25f, 1);
    public static GameDifficulty Normal = new GameDifficulty(1.75f, 2);
    public static GameDifficulty Hard = new GameDifficulty(2.25f, 4);

    public float ballSpeed; // Speed of the ball
    public int magnetCooldown; // Cooldown time of the magnet

    public GameDifficulty(float bSpeed, int mCD) {
        ballSpeed = bSpeed;
        magnetCooldown = mCD;
    }
}