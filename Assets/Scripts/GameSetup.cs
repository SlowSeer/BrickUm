using UnityEngine;
using System.Collections;

/// <summary>
/// This is where all the setup for the game is located.
/// </summary>
public class GameSetup : MonoBehaviour {

    // Components
    private GuyFPS guyFPS;
    
    public bool showFPS{
        get { return (PlayerPrefs.GetInt("ShowFPS") == 0) ? false : true; }
        set {
            guyFPS.enabled = value;
            PlayerPrefs.SetInt("ShowFPS", value ? 1 : 0 );
        }
    }

    public bool showColorTransitions {
        get { return (PlayerPrefs.GetInt("showColorTransitions") == 0) ? false : true; }
        set { PlayerPrefs.SetInt("showColorTransitions", value ? 1 : 0 ); }
    }

    // Level to use for FreePlay mode.
    public GameLevel[] freePlayLevels;

    // List of Game Campaigns.
    // Set Campaign and Levels from the inspector.
    public GameCampaign[] gameCampaigns;

    // Set up color themes for bricks.
    public BrickTheme[] brickThemes;

    // Level Building, Set in Inspector.
    //public BrickPrefabs brickPrefabs;

    public GameObject[] BrickTypes;

    public GameObject[] BallTypes;

    public Material[] backgrounds;

    public GameObject[] GameBuffs;

    private string[] myGameTips = {
        "Free Play ends when you lose the ball.",
        "You can position the paddle and ball at the start of a level.",
        "Tips about the game can show up right here!",
        "The ball gets faster the longer you survive in Free Play.",
        "Sometimes it's worth letting the ball miss the paddle to keep your combo bonus.",
        "Combo points are lost when the ball hits the paddle.",
        "Combo points are gained by hitting bricks.",
        "Use the magnet often.",
        "Bombs are a great way to build combo points.",
        "Bombs will take one life from every brick in its radius.",
        "Watch out for Tiny Paddle! :O",
        "You cant lose the ball in Kid Mode.",
        "Kid Mode has a slower ball.",
        "Having more lives gives you a higher score bonus at the end of a level.",
        "Points + Life Bonus - Time = Final Score"
        };

    void Awake() {
        guyFPS = GetComponent<GuyFPS>();
        guyFPS.enabled = showFPS;
    }

	void Start () {
        // Dont want to destroy this, so we can use it to load levels in the Game Scene.
        DontDestroyOnLoad(this.gameObject);
	}

    public string GetTip() {
        return myGameTips[Random.Range(0, myGameTips.Length)];
    }

}



[System.Serializable]
public class GameLevel {
    public string levelName;
    public TextAsset levelData;
    public bool useButtonColor = false;
    public Color buttonColor;
    public int brickTheme;
    public Color paddleColor;
    public int ball;
    public int background;

    public string icon_up_override;
    public string icon_down_override;
}


[System.Serializable]
public class GameCampaign {

    // Name/Color of campaign on the Main Menu.
    public string campaignName;
    public Color campaignNameColor;

    public bool showLevelNumber = true;

    // Icons to use for this campaign on the Main Menu.
    public string icon_up;
    public string icon_down;
    public string icon_locked;

    // List of levels in this campaign.
    public GameLevel[] campaignLevels;
}

[System.Serializable]
public class BrickTheme {
    public Color[] brickColors;
}