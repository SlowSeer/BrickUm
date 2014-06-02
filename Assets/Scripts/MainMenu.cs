using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

    public static bool skipToLevelSelect = false;

    // List of Levels Objects.
    public GameObject gameSetupPrefab;
    private GameSetup gameSetup;

    public GameObject background;

    public Material backgroundMenu;
    public Material backgroundCampaign;
    public Material backgroundKidMode;
    public Material backgroundFreePlay;

    // UI Toolkit Instances; Buttons & Text atlases.
    public UIToolkit buttonToolkit;
    public UIToolkit textToolkit;

    UIText text;
    UIText textSmall;

    // Main Menu Screen.
    UIVerticalLayout containerMenu;
    UIButton buttonOptions;
    UITextInstance tipText;

    // Options Screen Container
    UIScrollableVerticalLayoutContainer containerOptions;

    // Loading Scren
    UIVerticalLayoutWrap containerLoading;

    // Level List Screens.
    UIScrollableVerticalLayoutContainer containerCampaign;
    UIScrollableVerticalLayoutContainer containerKidMode;

    UIHorizontalLayoutWrap containerFreePlay;

    UIButton backButton;
    UITextInstance txtGameMode;


    // Get list of levels from the _Levels object.
    void Awake() {
        GameObject _Levels;
        if (GameObject.FindGameObjectWithTag("GameSetup") == null) {
            _Levels = GameObject.Instantiate(gameSetupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        } else {
            _Levels = GameObject.FindGameObjectWithTag("GameSetup");
        }
        gameSetup = _Levels.GetComponent<GameSetup>();
		
		
    }
	
	int menuScale = 1;
	
    // Define UI buttons
	void Start () {
		/*
		if(Screen.height > 480){
			Debug.Log("bigger: " + Screen.height +" "+Screen.width);
			float perc = ((float)Screen.height / 480.0f) * 100.0f;
			//Screen.SetResolution (
			//	(int)((float)Screen.width / 100.0f * perc), 
			//	480, 
			//	true);
			Screen.SetResolution (Screen.width / 2 , Screen.height / 2, true);
			
		}*/
		Debug.Log("Screen: w-" + Screen.width +" h-"+Screen.height);
		
		if(Screen.height >= 720){
			menuScale = 2;
			UI.scaleFactor = menuScale;
		}
		
		
		
        text = new UIText(textToolkit, "Arial55", "Arial55_0.tga");
        textSmall = new UIText(textToolkit, "Arial20", "Arial20_0.tga");

        // Build Back button.
        backButton = UIButton.create("icon_back_up.png", "icon_back_down.png", 0, 0);
        backButton.scale *= menuScale;
        backButton.zIndex = 0;
        backButton.onTouchUpInside += (sender) => ButtonMenu();
        backButton.hidden = true;

        // Game Mode.
        txtGameMode = text.addTextInstance("Campaign ", 0, 0);
        txtGameMode.color = Color.red;
        txtGameMode.zIndex = -2;
		txtGameMode.scale *= menuScale;
        txtGameMode.positionFromTopRight(0, 0);
        txtGameMode.alphaTo(4.5f, 0.25f, Easing.Linear.easeOut);
        txtGameMode.hidden = true;

        // Load Main Menu Screen
        BuildMenu();

        BuildOptions();
        containerOptions.hidden = true;

        BuildLoading();
        HideLoading();
		
        // Load & Hide Level Screen.
        BuildContainerCampaign();
        HideCampaign();

        BuildContainerKidMode();
        HideKidMode();

        BuildFreePlay();
        HideFreePlay();


        if(skipToLevelSelect){
            if (GameManager.gameMode == GameMode.KidMode) { ButtonKidMode(); } 
            else if (GameManager.gameMode == GameMode.Campaign) { ButtonCampaign(); } 
            else { ButtonFreePlay(); }
        }
		
		
		
		
    }


    void Update() {
        // If back is pressed 
        if(Input.GetKeyDown(KeyCode.Escape)){
            if (containerMenu.hidden == true) {
                // Go back to menu
                HideCampaign();
                HideKidMode();
				HideOptions();
				HideFreePlay();
                ShowMenu();
            } else {
                // Exit App
                Application.Quit();
            }
        }
    }


    // Build Main Menu
    private void BuildMenu() {
        var brickum = UIButton.create("brickum.png", "brickum.png", 0, 0);
		brickum.scale *= menuScale;
		
        var buttonCampaign = UIButton.create("button_campaign.png", "button_campaign_down.png", 0, 0);
        buttonCampaign.scale *= menuScale;
		buttonCampaign.onTouchUpInside += (sender) => ButtonCampaign();
		
		
        var buttonFreeplay = UIButton.create("button_freeplay.png", "button_freeplay_down.png", 0, 0);
		buttonFreeplay.scale *= menuScale;
        buttonFreeplay.onTouchUpInside += (sender) => ButtonFreePlay();

        var buttonKidmode = UIButton.create("button_kidmode.png", "button_kidmode_down.png", 0, 0);
		buttonKidmode.scale *= menuScale;
        buttonKidmode.onTouchUpInside += (sender) => ButtonKidMode();

        containerMenu = new UIVerticalLayout(5);
        containerMenu.addChild(brickum, buttonCampaign, buttonFreeplay, buttonKidmode);
        containerMenu.matchSizeToContentSize();
        containerMenu.positionCenter();

        tipText = textSmall.addTextInstance("Tip: Tips go here", 0, 0);
        tipText.text = "Tip: " + gameSetup.GetTip();
		tipText.scale *= menuScale;
        tipText.positionFromBottom(0.03f, 0);

        // Options Button
        buttonOptions = UIButton.create("options_up.png", "options_down.png", 0, 0);
		buttonOptions.scale *= menuScale;
        buttonOptions.onTouchUpInside += (sender) => ButtonOptions();
        buttonOptions.positionFromTopRight(0, 0);
        buttonOptions.alphaTo(1, 0.3f, Easing.Linear.easeOut);
    }

    // Build Options Menu
    private void BuildOptions() {
        // Options List box
        containerOptions = new UIScrollableVerticalLayoutContainer(0);
        containerOptions.setSize(Screen.width - backButton.width, Screen.height); //scrollable.setSize(width, height);
        containerOptions.position = new Vector3(backButton.width, 0, 0); //scrollable.position = new Vector3((Screen.width - width) / 2, -Screen.height + height, 0);
        containerOptions.zIndex = 1;
        containerOptions.myPadding = backButton.width;

        UIHorizontalLayoutWrap WrapperColors = new UIHorizontalLayoutWrap(0);
        UIToggleButton buttonColors = UIToggleButton.create("hexbox_false.png", "hexbox_true.png", "hexbox_down.png", 0, 0);
		buttonColors.scale *= menuScale;
        buttonColors.onToggle += (sender, newValue) => buttonOptionsColors(sender, newValue);
        buttonColors.selected = gameSetup.showColorTransitions;

        var textColors = text.addTextInstance("Color Transitions", 0, 0);
		textColors.scale *= menuScale;
        textColors.color = Color.white;
        textColors.zIndex = -1;
        WrapperColors.addChild(buttonColors, textColors);

        UIHorizontalLayoutWrap WrapperFPS = new UIHorizontalLayoutWrap(0);
        UIToggleButton buttonFPS = UIToggleButton.create("hexbox_false.png", "hexbox_true.png", "hexbox_down.png", 0, 0);
		buttonFPS.scale *= menuScale;
        buttonFPS.onToggle += (sender, newValue) => buttonOptionsFPS(sender, newValue);
        buttonFPS.selected = gameSetup.showFPS;

        var textFPS = text.addTextInstance("Show FPS", 0, 0);
		textFPS.scale *= menuScale;
        textFPS.color = Color.white;
        textFPS.zIndex = -1;
        WrapperFPS.addChild(buttonFPS, textFPS);

        containerOptions.addChild(WrapperColors, WrapperFPS);
    }

    private void BuildLoading() {
        var textLoading = text.addTextInstance("Loading...", 0, 0);
		textLoading.scale *= menuScale;
        textLoading.color = Color.white;
        textLoading.zIndex = -1;

        containerLoading = new UIVerticalLayoutWrap(5);
        containerLoading.addChild(textLoading);
        containerLoading.matchSizeToContentSize();
        containerLoading.positionCenter();
    }

    // Build FreePlay Screen
    private void BuildFreePlay() {
        containerFreePlay = new UIHorizontalLayoutWrap(30);
        
        // ---------------------------------------- SQUARE THEME
        var buttonFreePlay0 = UIButton.create("button_freeplay_square_up.png", "button_freeplay_square_down.png", 0, 0);
		buttonFreePlay0.scale *= menuScale;
        buttonFreePlay0.onTouchUpInside += (sender) => LoadFreePlay(0);

        var buttonbox0 = new UIAbsoluteLayoutWrap(0);
        buttonFreePlay0.localPosition = new Vector3(0, 0, 0);
        buttonbox0.addChild(buttonFreePlay0);

        var scoreText0 = textSmall.addTextInstance("123,456", 0, 0);
        //scoreText0.color = Color.white;
        scoreText0.text = (GetScore(0, 0, GameMode.FreePlay) > 0) ? GetScore(0, 0, GameMode.FreePlay).ToString() : "Play!";
		scoreText0.scale *= menuScale;
        //scoreText0.zIndex = -1;

        var scorebox0 = new UIAbsoluteLayoutWrap(0);
        scoreText0.localPosition = new Vector3((buttonFreePlay0.width / 2) - (scoreText0.width / 2), 0, 0);
        scorebox0.addChild(scoreText0);

        var inBox0 = new UIVerticalLayoutWrap(3); //<----- fix the vertical layout issues w/ Text.
        inBox0.addChild(buttonbox0, scorebox0);

        containerFreePlay.addChild(inBox0);
        // ---------------------------------------- HEX THEME
        var buttonFreePlayHex = UIButton.create("button_freeplay_hex_up.png", "button_freeplay_hex_down.png", 0, 0);
		buttonFreePlayHex.scale *= menuScale;
        buttonFreePlayHex.onTouchUpInside += (sender) => LoadFreePlay(1);

        var buttonbox1 = new UIAbsoluteLayoutWrap(0);
        buttonFreePlayHex.localPosition = new Vector3(0, 0, 0);
        buttonbox1.addChild(buttonFreePlayHex);

        var scoreText1 = textSmall.addTextInstance("&", 0, 0);
        //scoreText1.color = Color.white;
        scoreText1.text = (GetScore(0, 1, GameMode.FreePlay) > 0) ? GetScore(0, 1, GameMode.FreePlay).ToString() : "Play!";
		scoreText1.scale *= menuScale;

        var scorebox1 = new UIAbsoluteLayoutWrap(0);
        scorebox1.localPosition = new Vector3(-((buttonFreePlayHex.width / 2) - (scoreText1.width / 2)), 0, 0);
        
        //Debug.Log("Button: " + buttonFreePlayHex.width + " | Text: " + scoreText1.width + " + " + scorebox1.localPosition.ToString());
        scorebox1.addChild(scoreText1);

        var inBox1 = new UIVerticalLayoutWrap(3); //<----- fix the vertical layout issues w/ Text.
        inBox1.addChild(buttonbox1, scorebox1);

        containerFreePlay.addChild(inBox1);
        // ---------------------------------------- OCT THEME
        var buttonFreePlayOct = UIButton.create("button_freeplay_oct_up.png", "button_freeplay_oct_down.png", 0, 0);
		buttonFreePlayOct.scale *= menuScale;
        buttonFreePlayOct.onTouchUpInside += (sender) => LoadFreePlay(2);

        var buttonbox2 = new UIAbsoluteLayoutWrap(0);
        buttonFreePlayHex.localPosition = new Vector3(0, 0, 0);
        buttonbox2.addChild(buttonFreePlayOct);

        var scoreText2 = textSmall.addTextInstance("&", 0, 0);
        //scoreText1.color = Color.white;
        scoreText2.text = (GetScore(0, 2, GameMode.FreePlay) > 0) ? GetScore(0, 2, GameMode.FreePlay).ToString() : "Play!";
		scoreText2.scale *= menuScale;

        var scorebox2 = new UIAbsoluteLayoutWrap(0);
        scorebox2.localPosition = new Vector3(-((buttonFreePlayOct.width / 2) - (scoreText2.width / 2)), 0, 0);
        scorebox2.addChild(scoreText2);

        var inBox2 = new UIVerticalLayoutWrap(3); //<----- fix the vertical layout issues w/ Text.
        inBox2.addChild(buttonbox2, scorebox2);

        containerFreePlay.addChild(inBox2);
        // ----------------------------------------
        // FreePlay Levels List box
        containerFreePlay.matchSizeToContentSize();
        containerFreePlay.positionCenter();
    }

    // Display list of Campaigns/Levels
    private void BuildContainerCampaign() {
        // Campaign List Scroll Box.
        containerCampaign = new UIScrollableVerticalLayoutContainer(2);
        containerCampaign.setSize(Screen.width - backButton.width, Screen.height); //scrollable.setSize(width, height);
        containerCampaign.position = new Vector3(backButton.width, 0, 0); //scrollable.position = new Vector3((Screen.width - width) / 2, -Screen.height + height, 0);
        containerCampaign.zIndex = 1;
        containerCampaign.myPadding = backButton.width;
        ListLevels(containerCampaign, GameMode.Campaign);
    }

    private void BuildContainerKidMode() {

        // Campaign List Scroll Box.
        containerKidMode = new UIScrollableVerticalLayoutContainer(2);
        containerKidMode.setSize(Screen.width - backButton.width, Screen.height); //scrollable.setSize(width, height);
        containerKidMode.position = new Vector3(backButton.width, 0, 0); //scrollable.position = new Vector3((Screen.width - width) / 2, -Screen.height + height, 0);
        containerKidMode.zIndex = 1;
        containerKidMode.myPadding = backButton.width;
        ListLevels(containerKidMode, GameMode.KidMode);
        
    }

    public void ListLevels(UIScrollableVerticalLayoutContainer container, GameMode gameMode) {
        // List of Campaigns.
        for (int c = 0; c < gameSetup.gameCampaigns.Length; c++) {
            var campaign = c;
            GameCampaign tempCampaign = gameSetup.gameCampaigns[campaign];
            var txtCampaignName = text.addTextInstance(tempCampaign.campaignName, 0, 0);
			txtCampaignName.scale *= menuScale;
            txtCampaignName.color = tempCampaign.campaignNameColor;
            container.addChild(txtCampaignName);

            // List of Levels.
            var horizontalList = new UIHorizontalLayoutWrap(10);
            for (int l = 0; l < gameSetup.gameCampaigns[campaign].campaignLevels.Length; l++) {
                var level = l;
                GameLevel tempLevel = gameSetup.gameCampaigns[campaign].campaignLevels[level];

                // Level
                string tempUp = (tempLevel.icon_up_override != "") ? tempLevel.icon_up_override : tempCampaign.icon_up;
                string tempDown = (tempLevel.icon_down_override != "") ? tempLevel.icon_down_override : tempCampaign.icon_down;
                var levelButton = UIButton.create(tempUp, tempDown, 0, 0);
				levelButton.scale *= menuScale;
                if (tempLevel.useButtonColor) levelButton.color = tempLevel.buttonColor;


                var buttonbox = new UIAbsoluteLayoutWrap(0);
                levelButton.localPosition = new Vector3(0, 0, 0);
                buttonbox.addChild(levelButton);

                // Score Text
                var scoreText = textSmall.addTextInstance(" ", 0, 0);
				scoreText.scale *= menuScale;
                scoreText.color = tempCampaign.campaignNameColor;
                scoreText.zIndex = -1;

                if (isLocked(c,l, gameMode)) {
                    var lockedButton = UIButton.create("lock.png", "lock.png", 0, 0);
					lockedButton.scale *= menuScale;
                    lockedButton.zIndex = -1;
                    lockedButton.localPosition = new Vector3((levelButton.width / 2) - (lockedButton.width / 2), -((levelButton.height / 2) - (lockedButton.height / 1.5f)), 0);
                    lockedButton.zIndex = -1;
                    buttonbox.addChild(lockedButton);
                } else {
                    // Level is playable
                    levelButton.onTouchUpInside += (sender) => { StartLevel(sender, campaign, level); };
                    scoreText.text = (GetScore(c, l, gameMode) > 0) ? GetScore(c, l, gameMode).ToString() : "Play!";

                    if (tempCampaign.showLevelNumber) {
                        // Level Text
                        var levelText = text.addTextInstance((l + 1).ToString(), 0, 0);
						levelText.scale *= menuScale;
                        levelText.color = Color.white;
                        levelText.zIndex = -1;
                        levelText.text = "" + (l + 1);

                        levelText.localPosition = new Vector3((levelButton.width / 2) - (levelText.width / 2), -((levelButton.height / 2) - (levelText.height / 1.5f)), 0);
                        levelText.zIndex = -1;
                        buttonbox.addChild(levelText);
                    }
                }

                // Absolute container to center the score text
                var scorebox = new UIAbsoluteLayoutWrap(0);
                scoreText.localPosition = new Vector3((levelButton.width / 2) - (scoreText.width / 2), 0, 0);
                scorebox.addChild(scoreText);

                var inBox = new UIVerticalLayoutWrap(0); //<----- fix the vertical layout issues w/ Text.
                inBox.addChild(buttonbox, scorebox);


                horizontalList.addChild(inBox);
            }
            container.addChild(horizontalList);
        }

    }

    bool isLocked(int campaign, int level, GameMode gameMode) {
        if (level == 0) return false;

        string temp = "";
        if (gameMode == GameMode.KidMode) {
            temp = "K_" + campaign + "_lock";
        } else {
            temp = "C_" + campaign + "_lock";
        }
        if (PlayerPrefs.GetInt(temp) >= level) {
            return false;
        }
        return true;
    }

    void ButtonMenu() {
        HideKidMode();
        HideCampaign();
        HideOptions();
        HideFreePlay();

        ShowMenu();
    }

    void ButtonFreePlay() {
        skipToLevelSelect = false;

        HideMenu();
        HideCampaign();
        HideKidMode();

        ShowFreePlay();
    }

    void ButtonCampaign() {
        skipToLevelSelect = false;

        HideMenu();
        HideKidMode();
        HideFreePlay();

        ShowCampaign();
    }

    void ButtonKidMode() {
        skipToLevelSelect = false;

        HideMenu();
        HideCampaign();
        HideFreePlay();

        ShowKidMode();
    }

    void ButtonOptions() {
        HideMenu();

        ShowOptions();
    }

    void buttonOptionsColors(UIToggleButton sender, bool value) {
        gameSetup.showColorTransitions = value;
    }

    void buttonOptionsFPS(UIToggleButton sender, bool value) {
        gameSetup.showFPS = value;
    }



    // Show Menu Screen
    void ShowMenu() {
        background.renderer.material = backgroundMenu;
        containerMenu.hidden = false;
        buttonOptions.hidden = false;
        tipText.hidden = false;
        tipText.text = "Tip: " + gameSetup.GetTip();
        tipText.positionFromBottom(0.03f, 0);
    }

    void ShowOptions() {
        background.renderer.material = backgroundMenu;
        containerOptions.hidden = false;
        backButton.hidden = false;
        txtGameMode.hidden = false;

        txtGameMode.text = "Options ";
        txtGameMode.color = Color.grey;
        txtGameMode.positionFromTopRight(0, 0);
        txtGameMode.alphaTo(4.5f, 0.25f, Easing.Linear.easeOut);
    }

    void ShowLoading() {
        //background.renderer.material = backgroundMenu;
        containerLoading.hidden = false;

        tipText.hidden = false;
        tipText.text = "Tip: " + gameSetup.GetTip();
        tipText.positionFromBottom(0.03f, 0);
    }

    void ShowFreePlay() {
        background.renderer.material = backgroundFreePlay;
        containerFreePlay.hidden = false;
        backButton.hidden = false;
        txtGameMode.hidden = false;

        GameManager.gameMode = GameMode.FreePlay;
        txtGameMode.text = "Free Play ";
        txtGameMode.color = new Color(1, 0, 1);
        txtGameMode.positionFromTopRight(0, 0);
        txtGameMode.alphaTo(4.5f, 0.25f, Easing.Linear.easeOut);
    }

    // Show Campaign Screen
    void ShowCampaign() {
        background.renderer.material = backgroundCampaign;
        containerCampaign.hidden = false;
        backButton.hidden = false;
        txtGameMode.hidden = false;

        GameManager.gameMode = GameMode.Campaign;
        txtGameMode.text = "Campaign ";
        txtGameMode.color = Color.green;
        txtGameMode.positionFromTopRight(0, 0);
        txtGameMode.alphaTo(4.5f, 0.25f, Easing.Linear.easeOut);
    }

    // Show Kid Mode Screen
    void ShowKidMode() {
        background.renderer.material = backgroundKidMode;
        containerKidMode.hidden = false;
        backButton.hidden = false;
        txtGameMode.hidden = false;

        GameManager.gameMode = GameMode.KidMode;
        txtGameMode.text = "Kid Mode ";
        txtGameMode.color = Color.blue;
        txtGameMode.positionFromTopRight(0, 0);
        txtGameMode.alphaTo(4.5f, 0.25f, Easing.Linear.easeOut);
    }



    void HideMenu() {
        containerMenu.hidden = true;
        buttonOptions.hidden = true;
        tipText.hidden = true;
    }

    void HideOptions() {
        containerOptions.hidden = true;
        backButton.hidden = true;
        txtGameMode.hidden = true;
    }

    void HideLoading() {
        //background.renderer.material = backgroundMenu;
        containerLoading.hidden = true;
    }

    void HideFreePlay() {
        containerFreePlay.hidden = true;
        backButton.hidden = true;
        txtGameMode.hidden = true;
    }

    void HideCampaign() {
        containerCampaign.hidden = true;
        backButton.hidden = true;
        txtGameMode.hidden = true;
    }

    void HideKidMode() {
        containerKidMode.hidden = true;
        backButton.hidden = true;
        txtGameMode.hidden = true;
    }

    // Get Score for the campaign / level.
    int GetCampaignScore(int campaign, int level, bool kidMode) {
        string temp = (kidMode) ? "K_" + campaign + "_" + level : "C_" + campaign + "_" + level;
        return PlayerPrefs.GetInt(temp);
    }

    // Get the score for the level from the player prefs.
    int GetScore(int campaign, int level, GameMode gameMode) {
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

    void LoadFreePlay(int lvl) {
        GameManager.gameMode = GameMode.FreePlay;
        StartLevel(null, 1, lvl); // Campaign wont matter since its [FreePlay]
    }

    // Load this level.
    private void StartLevel(UIButton sender, int campaign, int level) {
        //Debug.Log("Starting Level: " + campaign + ", " + level + " - " + Time.time);

        // Confirm everything is hidden.
        HideKidMode();
        HideCampaign();
        HideFreePlay();
        HideOptions();
        HideMenu();

        ShowLoading();

        GameManager.SetLevel(campaign, level);
        Application.LoadLevel("GameScene");
    }
}
