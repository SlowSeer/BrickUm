using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public enum GameState {
    Start,
    Play,
    Pause,
    Over,
    Win
}

public enum GameMode {
    Campaign,
    KidMode,
    FreePlay
}

public class GameManager : MonoSingleton<GameManager> {

    // Static Level Setup (Used from the menu scene
    public static int sSelectedCampaign = -1;
    public static int sSelectedLevel = -1;
    public static GameLevel sSelectedGameLevel;
    public static GameMode gameMode = GameMode.Campaign;

    // Game Setup Prefab (level setup, etc)
    public GameObject gameSetupPrefab;
    public GameSetup gameSetup;

    // Public Settings
    private int points = 0; // Score for the level.

    BrickLevel brickLevel = new BrickLevel();

    // Game Objects
    public Transform paddleObject; // Link the paddle
    private PaddleBehaviour paddle;

    public GameOverBar gameOverBar; // Link the game over bar.

    public Transform ballSpawn; // Link the ball spawn point
    private GameObject ballObject;
    public BallBehaviour ball;

    public GameObject background;

    // Define Difficulty Settings
    public GameDifficulty gameDifficulty = GameDifficulty.Normal;
    public GameState gameState = GameState.Start;

    // Game Variables
    private float gameTime; // Time the game has gone on for.
    private List<GameObject> bricksActive = new List<GameObject>(); // List of active Bricks in-game.
    private List<int> brickSpawns = new List<int>(); // List of destroyed bricks (position in bricklevel) for respawning in FreePlay.

    private int combo = 0; // Current combo
    private int comboAddition = 1; // Number to add on each combo
    private int comboMax = 5; // maximum combo bonus.

    private float floatingScore = 0;

    // Color theme for the bricks.
    public BrickTheme brickTheme;

    // Bomb prefab
    public GameObject bombPrefab;

    // GUI: Game
    public TextMesh textScore;
    public TextMesh textTime;
    public TextMesh textCombo;
    public TextMesh textBlocks;
    public TextMesh textLives;

    public TextMesh textBuff;
    public Color textBuffColor;
    public Renderer textBuffRenderer;

    // GUI: Pause Screen
    public guiScreen pauseScreen;

    public guiButton guiMagnetButton;
    public guiButton guiBombButton;

    public guiButton guiPauseButton;

    public guiButton guiStartButton;

    public TextMesh textTitle;
    public TextMesh textSubtitle;
    public TextMesh textStartButton;

    // GUI: Score Screen
    public guiScreen scoreScreen;

    public TextMesh textScorePoints;
    public TextMesh textScoreBonus;
    public TextMesh textScoreTime;
    public TextMesh textScoreTotal;
    public TextMesh textScoreTotalTEXT;

    // TIMERS
    //private float cooldownMagnet = 2; // Cooldown timer for Magnet Ball
    private float cooldownMagnetCurrent = 2; // internal tracker.

    private float cooldownBuff = 1;
    private float cooldownBuffCurrent = 0;

    private float respawnTimer = 10;

    private int buffChance = 10; // % Chance on brick hit to spawn buff.

    // Touch
    private Vector2? lastMousePosition;


//Using the unity editor as my level editor.
#if UNITY_EDITOR 
    private XmlDocument myLevelSave; // XML Doc for saving.
#endif

    
    // Initialize (MonoSingleton) 
    public override void Init() {
        paddle = paddleObject.GetComponent<PaddleBehaviour>();

        GameObject _gameSetup;
        if (GameObject.FindGameObjectWithTag("GameSetup") == null) {
            _gameSetup = GameObject.Instantiate(gameSetupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        } else {
            _gameSetup = GameObject.FindGameObjectWithTag("GameSetup");
        }
        gameSetup = _gameSetup.GetComponent<GameSetup>();
        brickTheme = gameSetup.brickThemes[1]; // Just do grayscale as default and let it switch to color.

        // Delete prefs.
        //PlayerPrefs.DeleteAll();
    }


	void Start () {

        // Have we transitioned into this scene properly? If not, go back to menu.
        if (gameSetup == null || sSelectedCampaign < 0 || sSelectedLevel < 0) {
            Application.LoadLevel("MainMenu"); return;
        }
        
        // Grab all the bricks in the scene and keep them in a list. -- Shouldn't be any.
        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Brick")) { bricksActive.Add(b); }

        
        
        // Load Level (Some quick checks to make sure a level has been selected)
        if (gameSetup != null) {

            // Get Selected Game Level... FreePlay or Campaign/KidMode
            if (gameMode == GameMode.FreePlay) sSelectedGameLevel = gameSetup.freePlayLevels[sSelectedLevel];
            else sSelectedGameLevel = gameSetup.gameCampaigns[sSelectedCampaign].campaignLevels[sSelectedLevel];
            
            if (sSelectedGameLevel != null && sSelectedGameLevel.levelData != null) {
                // Load our level Date.
                LoadLevel(sSelectedGameLevel.levelData);

                // Set Background.
                background.GetComponent<Renderer>().material = gameSetup.backgrounds[sSelectedGameLevel.background];

                // Set Paddle Color
                paddle.paddleColor = sSelectedGameLevel.paddleColor;

                // Set the color theme of the bricks.
                brickTheme = gameSetup.brickThemes[sSelectedGameLevel.brickTheme];

                // Create Ball.
                ballObject = GameObject.Instantiate(gameSetup.BallTypes[sSelectedGameLevel.ball], ballSpawn.position, Quaternion.identity) as GameObject;
                ball = ballObject.GetComponent<BallBehaviour>();
            }
        }

        // Initiate Game Settings.

        // Kid mode gets a higher buff chance and easier difficulty.
        if (gameMode == GameMode.KidMode) {
            buffChance = 20;
            gameDifficulty = GameDifficulty.Easy;
        }

        textTitle.text = sSelectedGameLevel.levelName;
        textSubtitle.text = "Position paddle then tap the ball to start!";

        textBuff.GetComponent<Renderer>().enabled = false;
        textBuffRenderer = textBuff.GetComponent<Renderer>();
        textBuffColor = textBuffRenderer.material.color;

        // We dont start with a bomb, do disable the button.
        guiBombButton.usable = false;
	}
    
	void Update () {

        // Handle Touch / Mouse input
        if (Input.touchCount > 0) {
            // Examine all current touches
            for (int i = 0; i < Input.touchCount; i++) {
                myTouch t = new myTouch();
                t.set(Input.GetTouch(i));
                CheckTouch(t);
            }
        }
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_WEBGL
        else {
            if (Input.GetMouseButtonDown(0)) { CheckTouch(UITouchMaker.createTouchFromInput(UIMouseState.DownThisFrame, ref lastMousePosition)); }
            if (Input.GetMouseButton(0)) { CheckTouch(UITouchMaker.createTouchFromInput(UIMouseState.HeldDown, ref lastMousePosition)); }
            if (Input.GetMouseButtonUp(0)) { CheckTouch(UITouchMaker.createTouchFromInput(UIMouseState.UpThisFrame, ref lastMousePosition)); }
        }
#endif
        
        // If game is playing
        if (gameState == GameState.Play) {

            // Count game time.
            gameTime += Time.deltaTime;
            respawnTimer -= Time.deltaTime;

            // Remove destroyed bricks from brick array.
            //for (int i = 0; i < bricks.Count; i++) 
            //    if (bricks[i] == null) bricks.Remove(bricks[i]);

            // Fade out the buff info text.
            if (textBuffRenderer.material.color.a > 0.01f) {
                Color temp = textBuffRenderer.material.color;
                temp.a -= Time.deltaTime / 3;
                textBuffRenderer.material.color = temp;
            } else {
                textBuffRenderer.enabled = false;
            }

            // Magnet Button Cooldown timer.
            if (cooldownMagnetCurrent < gameDifficulty.magnetCooldown) {
                guiMagnetButton.GetComponent<Renderer>().material.SetFloat("_Cutoff", cooldownMagnetCurrent / gameDifficulty.magnetCooldown);
                cooldownMagnetCurrent += Time.deltaTime;
            } else {
                guiMagnetButton.usable = true;
            }

            // Buff Spawn Cooldown timer.
            if (cooldownBuffCurrent > 0) cooldownBuffCurrent -= Time.deltaTime;


            if (gameMode == GameMode.FreePlay) { // [FreePlay] only logic.
                // Respawn Brick - minimum brick limit.
                if (bricksActive.Count <= 15 && brickSpawns.Count > 0) {
                    int bPos = brickSpawns[Random.Range(0, brickSpawns.Count)];
                    Brick b =  brickLevel.bricks[bPos];
                    //b.life = Random.Range(1, 6);
                    BrickSpawn(b, bPos);
                }
                // Respawn Brick - random time respawn.
                if (respawnTimer <= 0 && brickSpawns.Count > 0) {
                    respawnTimer = Random.Range(8, 15);
                    int bPos = brickSpawns[Random.Range(0, brickSpawns.Count)];
                    Brick b = brickLevel.bricks[bPos];
                    //b.life = Random.Range(1, 6);
                    BrickSpawn(b, bPos);
                }
            } else { // [Campaign] and [KidMode] logic.
                // Woo we won the game!
                if (bricksActive.Count <= 0) {
                    WinGame();
                    gameState = GameState.Win;
                }
            }
        }

        // Update Game Info text on screen.
        floatingScore = Mathf.Lerp(floatingScore, points, Time.deltaTime * 5);
        textScore.text = floatingScore.ToString("F0");
        textTime.text = gameTime.ToString("f1");
        textBlocks.text = bricksActive.Count + " Blocks Remaining";
        textLives.text = gameOverBar.life.ToString();
        textCombo.text = "+" + combo;

        // Escape is the back button on Android.
        if (Input.GetKeyUp(KeyCode.Escape)) TogglePause();

        // M can pull the ball towards the paddle.
        if (Input.GetKeyDown(KeyCode.M)) MagnetBall();

        // B explodes and damages all bricks in a radius
        if (Input.GetKeyDown(KeyCode.B)) BombBall();
       
	}

    

    private Ray ray;
    private RaycastHit hit = new RaycastHit();
    public LayerMask buttonLayer;

    // Handle Touch Input (inclusing mouse).
    void CheckTouch(myTouch t) {
        // OH MY GOD REBUILD HOW TOUCH IS HANDLED, SO BAD.

        // Screen point touched.
        ray = Camera.main.ScreenPointToRay(t.position);
        Physics.Raycast(ray, out hit, 10, buttonLayer);
        
        if (GameManager.instance.gameState == GameState.Play) {
            // Check game elements.
            paddle.CheckTouch(t);

            // Check Buttons - Goal here is to get down to one raycast.
            //if (Physics.Raycast(ray, out hit, 10, buttonLayer)) {
                // check all buffs.
                for (int i = 0; i < gameBuffButtons.Count; i++) {
                    if (gameBuffButtons[i].CheckTouch(t, hit)) return;
                }

                // Check GUI elements
                guiMagnetButton.CheckTouch(t, hit);
                guiBombButton.CheckTouch(t, hit);
                guiPauseButton.CheckTouch(t, hit);
           // }
        } else {
           // if (Physics.Raycast(ray, out hit, 10, buttonLayer)) {
                pauseScreen.CheckTouch(t, hit);
            //}

            if (GameManager.instance.gameState == GameState.Start) {
                // Check game elements.
                paddle.CheckTouch(t);
                ball.CheckTouch(t);
            }
        }
    }


    // Pull ball towards paddle.
    public void MagnetBall() {
        if (gameState == GameState.Play && guiMagnetButton.usable) {
            ball.Push(paddleObject.position - ballObject.transform.position);
            paddle.Magnet();
            Debug.Log("Magnet Ball @ " + Time.time.ToString());
            cooldownMagnetCurrent = 0;
            guiMagnetButton.usable = false;

            // pull bufs to magnet too
            /*for (int i = 0; i < gameBuffButtons.Count; i++) {
                gameBuffButtons[i].SetDirection(paddle.transform.position + gameBuffButtons[i].transform.position);
            }*/
        }
    }


    public void BombBall() {
        if (gameState == GameState.Play && guiBombButton.usable) {
            // Bomb button pressed, disable it.
            guiBombButton.usable = false;

            // Build the bomb action here.
            Collider[] overlaps = Physics.OverlapSphere(ball.transform.position, 0.75f);

            for (int i = 0; i < overlaps.Length; i++ ) {
                if (overlaps[i].tag == "Brick") {
                    overlaps[i].gameObject.GetComponent<BrickBehaviour>().BrickHit(
                        overlaps[i].transform.position);
                }
            }

            Instantiate(bombPrefab, ball.transform.position + Vector3.back, Quaternion.identity);
            Debug.Log("Bomb Ball @ " + Time.time.ToString());
        }
    }

    /// <summary>
    /// Start the game.
    /// </summary>
    public void StartGame() {
        //ball.transform.parent = null;
        ball.speed = gameDifficulty.ballSpeed;
        ball.Drop();

        gameState = GameState.Play;
        pauseScreen.FadeOut(0.5f);
    }

    public void PlayGame() {
        if (gameState == GameState.Start) {
            StartGame();
        } else if (gameState == GameState.Pause) {
            gameState = GameState.Play;
            pauseScreen.FadeOut(0.5f);
        }      
    }

    public void PauseGame() {

        gameState = GameState.Pause;

        textSubtitle.text = "Game Paused";
        textStartButton.text = "Continue";

        pauseScreen.FadeIn(0.5f);
    }

    // Called from the ball when it hits the GameOver trigger.
    public void GameOver() {
        // TODO
        gameState = GameState.Over;
        pauseScreen.FadeIn(0.5f);

        if (gameMode != GameMode.FreePlay) {
            // Normal GameOver Screen
            textSubtitle.text = "Game Over :(";
            textStartButton.text = "Restart";
            guiStartButton.methodCall = "RestartLevel";
        } else {
            // FreePlay GameOver Screen
            string message = "Mathmagical! [S]!";
            if (gameTime < 20) message = "That's Terrible! [F-]";
            else if (gameTime < 60) message = "Thats not good. [F]";
            else if (gameTime < 120) message = "Thats not bad. [C]";
            else if (gameTime < 180) message = "Good Effort. [C+]";
            else if (gameTime < 240) message = "Great Work! [B]";
            else if (gameTime < 300) message = "Super! [B+]";
            else if (gameTime < 360) message = "Intangible! [A]";
            else if (gameTime < 420) message = "Otherworldly! [A+]";
            else if (gameTime < 480) message = "Rocket Man! [A++]";

            textSubtitle.text = "You lasted " + gameTime.ToString("f1") + " seconds. " + message;
            textStartButton.text = "Restart";
            guiStartButton.methodCall = "RestartLevel";

            int calcScore = CalcScore();
            if (calcScore > getScore(sSelectedCampaign, sSelectedLevel)) {
                ShowScore(calcScore, true);
            } else {
                ShowScore(calcScore, false);
            }
        }
        
    }


    public void WinGame() {
        pauseScreen.FadeIn(0.5f);

        // Show Next level, Next Campaign, or Smiley Face if there is nothing left.
        if (sSelectedLevel < gameSetup.gameCampaigns[sSelectedCampaign].campaignLevels.Length - 1) {
            textStartButton.text = "Next Level";
            guiStartButton.methodCall = "NextLevel";
        } else if (sSelectedLevel >= gameSetup.gameCampaigns[sSelectedCampaign].campaignLevels.Length - 1 && sSelectedCampaign < gameSetup.gameCampaigns.Length - 1) {
            textStartButton.text = "Next Campaign";
            guiStartButton.methodCall = "NextCampaign";
        } else {
            textStartButton.text = " :) ";
            guiStartButton.methodCall = "";
        }

        // Show Win message / save high score.
        string winMessage = "You won, Congratulations!";

        int calcScore = CalcScore();
        if (calcScore > getScore(sSelectedCampaign, sSelectedLevel)) {
            winMessage = "You got the high score, " + calcScore + " !";
            ShowScore(calcScore, true);
        } else {
            ShowScore(calcScore, false);
        }
        textSubtitle.text = winMessage;

        UnlockLevel(); // unlock next level from the list
    }


    void UnlockLevel() {
        string temp = "";
        if (gameMode == GameMode.KidMode) {
            temp = "K_" + sSelectedCampaign + "_lock";
        } else {
            temp = "C_" + sSelectedCampaign + "_lock";
        }

        if (PlayerPrefs.GetInt(temp) <= sSelectedLevel) PlayerPrefs.SetInt(temp, sSelectedLevel + 1);
    }

    // Calculate the score, display it, and save it.
    void ShowScore(int scr, bool high) {
        if (high) {
            setScore(sSelectedCampaign, sSelectedLevel, scr);
            textScoreTotalTEXT.text = "High Score!";
        }

        scoreScreen.FadeIn(0.5f);
        textScorePoints.text = points.ToString();
        textScoreBonus.text = "" + (int)(points * (0.01f * (float)gameOverBar.life));
        textScoreTime.text = "" + (int)gameTime;
        textScoreTotal.text = scr.ToString();

    }


    // Calculate the game score.
    int CalcScore() {
        return points + 
            (int)(points * (0.01f * (float)gameOverBar.life)) -
            (int)gameTime;
    }

    // Used for ESC key press. (back on mobile)
    private void TogglePause() {
        if (gameState == GameState.Play) 
            PauseGame();
        else if(gameState == GameState.Pause) 
            PlayGame();
    }


    // Start next level.
    public void NextLevel() {
        SetLevel(sSelectedCampaign, sSelectedLevel + 1);
        Application.LoadLevel(Application.loadedLevel);

    }


    // Start next Campaign.
    public void NextCampaign() {
        SetLevel(sSelectedCampaign + 1, 0);
        Application.LoadLevel(Application.loadedLevel);
    }


    // Reload the current scene to restart the level.
    public void RestartLevel() {
        Application.LoadLevel(Application.loadedLevel);
    }


    // Hit a brick, calculate the score.
    public void BrickHit(Vector3 hitPos, GameObject brickObject, int listPos, bool broke = false) {
        if (combo > comboMax) combo = comboMax;

        AddScore(10 + combo);
        combo += comboAddition;

        // Chance to spawn a buff on hit.
        SpawnBuff(hitPos);

        if (broke) {
            AddScore(10); // Bonus points when you destroy a brick.
            // Destroy a brick and remove references to it.
            bricksActive.Remove(brickObject); // Remove from active list
            brickSpawns.Add(listPos); // Add brick index position to respawn list
        }
    }


    // Ball hit the paddle, reset combo counter.
    public void PaddleHit() {
        combo = 0;
    }


    // Go to main menu level selection.
    public void LoadMenu() {
        MainMenu.skipToLevelSelect = true;
        Application.LoadLevel("MainMenu");
    }


    // Add to the current score.
    public void AddScore(int add) {
        points += add;
    }

    
    // Set the score for the level in the player prefs.
    void setScore(int campaign, int level, int scr) {
        string temp = "";
        if (gameMode == GameMode.FreePlay) {
            temp = "FreePlay_" + level;
        } else if (gameMode == GameMode.KidMode) {
            temp = "K_" + campaign + "_" + level;
        } else {
            temp = "C_" + campaign + "_" + level;
        }
        //Debug.Log("Setting Score: " + temp + " | " + scr);
        PlayerPrefs.SetInt(temp, scr);
    }


    // Get the score for the level from the player prefs.
    int getScore(int campaign, int level) {
        string temp = "";
        if (gameMode == GameMode.FreePlay) {
            temp = "FreePlay_" + level;
        } else if (gameMode == GameMode.KidMode) {
            temp = "K_" + campaign + "_" + level;
        } else {
            temp = "C_" + campaign + "_" + level;
        }
        //Debug.Log("Getting Score: " + temp + " | " + PlayerPrefs.GetInt(temp));
        return PlayerPrefs.GetInt(temp);
    }

    // ----------------------------------------------------
    // Buffs
    // ----------------------------------------------------

    private List<GameBuffButton> gameBuffButtons = new List<GameBuffButton>();

    void BuffText(string msg) {
        textBuff.GetComponent<Renderer>().enabled = true;
        textBuff.text = msg;
        textBuffRenderer.enabled = true;
        textBuffRenderer.material.color = textBuffColor;
    }

    public void RemoveBuffButton(GameBuffButton gbb) {
        gameBuffButtons.Remove(gbb);
    }

    // Spawn a buff button.
    void SpawnBuff(Vector3 pos) {
        if(cooldownBuffCurrent <= 0 && Random.Range(0, 100) < buffChance){
            cooldownBuffCurrent = cooldownBuff;
            GameObject go = Instantiate(
                gameSetup.GameBuffs[Random.Range(0, gameSetup.GameBuffs.Length)], 
                pos - (Vector3.forward * 0.35f), // Buff Layer
                gameSetup.GameBuffs[0].transform.rotation) as GameObject;
            GameBuffButton gbb = go.GetComponent<GameBuffButton>();
            gameBuffButtons.Add(gbb);
        }
    }

    public void BuffOneUp() {
        if (gameOverBar.life < 5) { 
            gameOverBar.OneUp();
            BuffText("One Up");
        } else {
            // if we already have 5 lives, just give 1 point.
            AddScore(1);
            BuffText("Max Life: +1 point");
        } 
    }

    public void BuffBomb() {
        if (!guiBombButton.usable) {
            BuffText("Bomb");
            guiBombButton.usable = true;
        } else {
            // if we already have a bomb, just give 1 point.
            AddScore(1);
            BuffText("Have Bomb: +1 point");
        } 
    }

    public void BuffBigPaddle() {
        BuffText("Big Paddle!");
        paddle.BigPaddle();
    }

    public void DebuffSmallPaddle() {
        BuffText("Small Paddle! :(");
        paddle.SmallPaddle();
    }

    public void BuffMystery() {
        switch (Random.Range(1, 5)) { // Max is exclusive
            case 1:
                BuffOneUp();
                break;
            case 2:
                BuffBigPaddle();
                break;
            case 3:
                BuffBomb();
                break;
            case 4:
                DebuffSmallPaddle();
                break;
        }
    }

    // ----------------------------------------------------
    // Save and Load
    // ----------------------------------------------------

    // Set the static campaign and level.
    public static void SetLevel(int campaign, int level) {
        sSelectedCampaign = campaign;
        sSelectedLevel = level;
    }

//Using the unity editor as my level editor.
#if UNITY_EDITOR 

    // Show the Save button (only in editor).
    void OnGUI() {
        /*if(GUI.Button(new Rect( Screen.width - 55, Screen.height/2 - 10, 50, 20), "Save")){
            SaveLevel("TestLevel");
        }*/
    }

    // Save the current level in an xml file.
    public void SaveLevel(string boardName) {
        int index = 0;
        bricksActive.Clear();

        BrickLevel brickLevel = new BrickLevel();

        foreach (GameObject b in GameObject.FindGameObjectsWithTag("Brick")) { bricksActive.Add(b); }
        brickLevel.bricks = new Brick[bricksActive.Count];
        foreach (GameObject b in bricksActive) {
            Brick temp = b.GetComponent<BrickBehaviour>().GetBrick();
            brickLevel.bricks[index] = temp;
            index += 1;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(BrickLevel));
        string xml;
        using (StringWriter writer = new StringWriter()) {
            serializer.Serialize(writer, brickLevel);
            xml = writer.ToString();
        }
        myLevelSave = new XmlDocument();
        myLevelSave.LoadXml(xml);
        myLevelSave.Save(Application.dataPath + "/data/" + boardName + ".xml");
        Debug.Log("Saved Level.");
    }
#endif

    // Load a level into the current scene.
    void LoadLevel(TextAsset lvl) {
        XmlSerializer serializer = new XmlSerializer(typeof(BrickLevel));
        StringReader stringReader = new StringReader(lvl.text);

        using (XmlReader reader = XmlReader.Create(stringReader)) {
            brickLevel = (BrickLevel)serializer.Deserialize(reader);
                
            // Destroy all bricks currently in the arena.
            foreach (GameObject b in bricksActive) Destroy(b);
            bricksActive.Clear();

            // Build new bricks.
            for (int i = 0; i < brickLevel.bricks.Length; i++ ) {
                BrickSpawn(brickLevel.bricks[i], i);
            }
        }
    }

    // Spawn a new brick.
    void BrickSpawn(Brick b, int bPos) {
        GameObject tempBrick = GameObject.Instantiate(gameSetup.BrickTypes[b.brickType], b.position, b.rotation) as GameObject;
        tempBrick.GetComponent<BrickBehaviour>().Load(b, bPos);
        bricksActive.Add(tempBrick); // add brick to list of active bricks.
        brickSpawns.Remove(bPos); // remove brick from list of spawn points. [FreePlay]

        // If we're already playing then the brick is a respawn.
        if (gameState == GameState.Play) {
            tempBrick.transform.localScale *= 0.01f;
        }
    }


    // If a game is in progress, pause it when the application is paused.
    void OnApplicationPause(bool pause) {
        if (pause) {
            if (gameState == GameState.Play) PauseGame();
        }
    }


    // If a game is in progress, pause it when the application loses focus.
    void OnApplicationFocus(bool focus) {
        if (!focus) {
            if (gameState == GameState.Play) PauseGame();
        }
    }

}