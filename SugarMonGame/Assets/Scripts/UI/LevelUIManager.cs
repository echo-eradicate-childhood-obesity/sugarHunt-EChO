//This file was create by Mark Botaish on June 12th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class LevelUIManager : MonoBehaviour
{
    #region   STRUCTS/CLASSES
    //The infomation needed for each sugar group
    [System.Serializable]
    public class SugarButton
    {
        public Button _currentButton;
        public GameObject _currentGroup;
        public int _currentButtonIndex;
        public int _buttonCount;
        public string _name;

        public void init(GameObject panel, int index)
        {
            _name = panel.name.Remove(panel.name.Length - 3);
            _buttonCount = panel.transform.childCount;
            _currentButtonIndex = index;
            _currentGroup = panel;    
            
            if(_currentButtonIndex < panel.transform.childCount)
                _currentButton = panel.transform.GetChild(_currentButtonIndex).gameObject.GetComponent<Button>();
        }
    }

    [System.Serializable]
    public struct Powers
    {
        public Button _powerup;
        public int _levelToUnlock;
    }
    #endregion

    #region PUBLIC_VARS
    [Tooltip("A list of powerup")]public List<Powers> powers;          
    [Tooltip("A reference to the coins text in the main menu")] public TextMeshProUGUI tempCoins;       
    [Tooltip("A reference to the level text in the main menu ")] public TextMeshProUGUI tempLevel;      

    [Tooltip("A reference to the selection screen left arrow")] public GameObject _leftArrow;           
    [Tooltip("A reference to the selection screen right arrow")] public GameObject _rightArrow;         
    #endregion

    #region PRIVATE_VARS
    private List<SugarButton> buttonGroups = new List<SugarButton>();   // This is the list of SugarButtons, which contains all the information needed for the sugar group
    private List<GameObject> _panelOrder = new List<GameObject>();
    private GameObject _allPanels;                                      // This is a reference to the Head of the sugar panels
    private GameObject _backButton;                                     // This is a reference to the back button in the scene
    private GameObject _currentLevels;                                  // This is a reference to the current active panel 
    private GameObject _buttonPanel;                                    // This is a reference to the back button in the scene
    private GameObject _selectionPanel;                                 // This is a reference to the selection panel in the scene
    private GameObject _skillsPanel;                                    // This is a reference to the skills panel in the scene
    private GameObject _mainMenuPanel;                                  // This is a reference to the main menu panel in the scene
    private GameObject _achievementPanel;                               // This is a reference to the achievements panel in the scene

    private int _currentSelection = 0;                                  // This is a current button index that is showing on screen
    private float _selectionOffset;                                     // This is the offset position of the first button
    private float _targetLocation;                                      // This is the current target position       
    private Coroutine _selectionAnim = null;                            // This is a reference to the current coroutine running the animation
    private Image _xpSlider;                                            // A reference to the xp slider in the scene

    private PlayerInfoScript info;                                      // A reference to the PlayerInfoScript singleton 
    #endregion

    private void Start()
    {
        //Get all of the references in the game
        _buttonPanel = GameObject.Find("Buttons");
        _allPanels = GameObject.Find("SugarPanels");
        _backButton = GameObject.Find("BackButton");
        _selectionPanel = GameObject.Find("Selection");
        _skillsPanel = GameObject.Find("SkillsPanel");
        _mainMenuPanel = GameObject.Find("MainMenu");
        _achievementPanel = GameObject.Find("AchievementsPanel");
        _leftArrow.SetActive(false);

        _xpSlider = tempLevel.transform.parent.transform.Find("Fill").GetChild(0).GetComponent<Image>();

        _allPanels.SetActive(false);   
        _selectionPanel.SetActive(false);        
        _skillsPanel.SetActive(false);
        _achievementPanel.SetActive(false);

        info = PlayerInfoScript.instance;
        tempCoins.text = info.GetCoinCount().ToString("00000000");
        _xpSlider.fillAmount = info.GetPercentageToNextLevel();
        tempLevel.text = "" + info.GetLevel(); 
        UpdatePowers(info.GetLevel());

        _currentLevels = _mainMenuPanel;

        int size = _allPanels.transform.childCount;

        //Loop through the sugar groups panels
        for(int i = 0; i < size; i++)
        {
            SugarButton group = new SugarButton();
            Transform panel = _allPanels.transform.GetChild(i).GetChild(0);

            _allPanels.transform.GetChild(i).GetComponent<ScrollRect>().verticalNormalizedPosition = 1; //Automatically scroll to the top
            group.init(panel.gameObject,PlayerInfoScript.instance.GetLevelInSugarGroup(i));//Init the sugar group

            buttonGroups.Add(group);

            //Update the titles
            Transform child = panel.parent.Find("Text");
            
            if (group._currentButton)
            {
                group._currentButton.onClick.AddListener(() => PlayLevel(panel.GetSiblingIndex())); //Set the current button to be active
                print(panel.GetSiblingIndex() + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                group._currentButton.interactable = true;
                child.GetComponent<Text>().text = buttonGroups[i]._name + " - " + (buttonGroups[i]._currentButtonIndex) + "/" + buttonGroups[i]._buttonCount;
            }
            else
            {
                child.GetComponent<Text>().text = buttonGroups[i]._name + " - " + (buttonGroups[i]._buttonCount) + "/" + buttonGroups[i]._buttonCount;
            }
            
            //Update the buttons to be at the location last saved (Future: From the save file)
            UpdateButtons(buttonGroups[i]);

           

            panel.parent.gameObject.SetActive(false);
        }

        //Settings for the "map" selection animations
        _selectionOffset = -_selectionPanel.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x / buttonGroups.Count;
        _targetLocation = -_selectionPanel.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition.x;

    }
    
    #region  PUBLIC FUNCTIONS

    /// <summary>
    /// This function is called when a level button has been clicked. 
    /// This sets the current settings for the level to load.
    /// </summary>
    /// <param name="index"></param>
    public void PlayLevel(int index)
    {
        info.SetCurrentSugarGroup(index);
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// This function is used to activate the correct sugar group panel 
    /// </summary>
    /// <param name="num"></param>
    public void ActivatePanel(int num)
    {
        _panelOrder.Add(_currentLevels);
        _currentLevels = _allPanels.transform.GetChild(num).gameObject;
        _currentLevels.transform.parent.gameObject.SetActive(true);
        _currentLevels.SetActive(true);
        ToggleButtons();
    }

    /// <summary>
    /// This function is used to go back to the sugar group buttons and disable
    /// the current sugar buttons panel
    /// </summary>
    public void GoBack()
    {
        _currentLevels.SetActive(false);
        if (_panelOrder.Count > 0)
        {
            _currentLevels = _panelOrder[_panelOrder.Count - 1];
            _panelOrder.RemoveAt(_panelOrder.Count - 1);
        }           
        else
            _currentLevels = _mainMenuPanel;
        
        _currentLevels.SetActive(true);
    }

    /// <summary>
    /// This function is used to move the selection screen to the correct button. This function should be called
    /// by the arrows in the scene.
    /// </summary>
    /// <param name="dir"></param>
    public void MoveSelectionScreen(int dir)
    {
        dir = (dir / Mathf.Abs(dir)); //Ensure that die is either -1 or 1
        int prev = _currentSelection; //Get the currently selected
        _currentSelection = Mathf.Clamp(_currentSelection + dir, 0, buttonGroups.Count - 1); //Change selection but clamp the values

        if (!_leftArrow.activeSelf && _currentSelection > 0)
            _leftArrow.SetActive(true);

        if (_leftArrow.activeSelf && _currentSelection <= 0)
            _leftArrow.SetActive(false);

        if (_rightArrow.activeSelf && _currentSelection >= buttonGroups.Count - 1)
            _rightArrow.SetActive(false);

        if (!_rightArrow.activeSelf && _currentSelection < buttonGroups.Count - 1)
            _rightArrow.SetActive(true);

        if (prev != _currentSelection) //If the two selections are different move the panel
        {
            _targetLocation += dir * _selectionOffset;
            if (_selectionAnim != null)
                StopCoroutine(_selectionAnim);
            _selectionAnim = StartCoroutine(MoveSelection(_selectionPanel.transform.GetChild(0).GetComponent<RectTransform>()));
        }

    }


    /// <summary>
    /// This function is used to increase the power level of a talent. This function also checks 
    /// if the taletn can be bought or not.
    /// </summary>
    /// <param name="text"></param>
    public void IncreasePowerLevel(TextMeshProUGUI text)
    {
        string name = text.gameObject.transform.parent.name;
        int coins = info.BuyLevel(name);

        if(coins > 0)
        {
            if (info.GetMaxPowerLevel(name) == info.GetPowerLevel(name))
                text.text = "MAXED";
            else
                text.text = info.GetPowerLevel(name) + "/" + info.GetMaxPowerLevel(name) + "\n" + info.GetBuyAmmount(name);
            tempCoins.text = info.GetCoinCount().ToString("00000000");
        }else if (coins == 0)
        {
            print("MAX LEVEL");
            text.text = "MAXED";
        }
        else
        {
            print("NO ENOUGH COINS");
        }

    }

    /// <summary>
    /// This function is used to go to the map selection screen.
    /// </summary>
    public void GoToLevelSelectScreen()
    {
        _currentLevels.SetActive(false);
        _currentLevels = _selectionPanel;
        _currentLevels.SetActive(true);
    }

    /// <summary>
    /// This function is used to go to the talent screen. 
    /// </summary>
    public void GoToSkillsScreen()
    {
        _currentLevels.SetActive(false);
        _currentLevels = _skillsPanel;
        _currentLevels.SetActive(true);
    }

    /// <summary>
    /// This fucntion is used to go to the achievement screen 
    /// </summary>
    public void GoToAchievementScreen()
    {
        _currentLevels.SetActive(false);
        _currentLevels = _achievementPanel;
        _currentLevels.SetActive(true);
    }

    /// <summary>
    /// This fucntion is used to update the coins and xp bars on the stats panel.
    /// </summary>
    public void UpdateUIStats()
    {
        PlayerInfoScript play = PlayerInfoScript.instance;
        tempCoins.text = play.GetCoinCount().ToString("00000000");
        tempLevel.transform.parent.transform.Find("Fill").GetChild(0).GetComponent<Image>().fillAmount = play.GetPercentageToNextLevel();
        tempLevel.text = "" + play.GetLevel();
    }

    #endregion

    #region PRIVATE FUNCTIONS   

    /// <summary>
    /// This function is used to check to see if buttons need to be unlocked.
    /// </summary>
    /// <param name="level"></param>
    private void UpdatePowers(int level)
    {
        for(int i = 0; i < powers.Count; i++)
        {
            if(level >= powers[i]._levelToUnlock)
            {
                string name = powers[i]._powerup.name;
                powers[i]._powerup.interactable = true;
                if (info.GetMaxPowerLevel(name) == info.GetPowerLevel(name))
                    powers[i]._powerup.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "MAXED";
                else
                    powers[i]._powerup.transform.Find("Level").GetComponent<TextMeshProUGUI>().text = info.GetPowerLevel(name) + "/" + info.GetMaxPowerLevel(name) + "\n" + info.GetBuyAmmount(name);
                powers.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// This function is used to turn all of the buttons green before the current buttons. 
    /// This will visually show that the levels have been completed in the past. 
    /// This should be used in conjunction with a saved file.
    /// </summary>
    /// <param name="group"></param>
    private void UpdateButtons(SugarButton group)
    {
        Transform trans = group._currentGroup.transform;       
        int size = Mathf.Clamp(group._currentButtonIndex, 0, trans.childCount);
        for (int i = 0; i < size; i++)
        {
            Button but = trans.GetChild(i).GetComponent<Button>();

            ColorBlock block = but.colors;
            block.disabledColor = Color.green;
            but.colors = block;
        }
    }

    /// <summary>
    /// This function is used to toggle the Sugar Group button panels
    /// </summary>
    private void ToggleButtons() { _selectionPanel.SetActive(!_selectionPanel.activeSelf); }

    /// <summary>
    /// This fucntion is to lerp between the two postions to create a smooth animation 
    /// when transitioning between buttons.
    /// </summary>
    /// <param name="panel"></param>
    /// <returns></returns>
    private IEnumerator MoveSelection(RectTransform panel)
    {
        float timer = 0;
        float timeToComplete = 1;

        while (panel.anchoredPosition.x != _targetLocation)
        {
            float x = Mathf.Lerp(panel.anchoredPosition.x, _targetLocation, timer / timeToComplete);
            panel.anchoredPosition = new Vector2(x, panel.anchoredPosition.y);
            timer += Time.deltaTime;

            yield return null;
        }
        _selectionAnim = null;
    }

    /// <summary>
    /// This function is used to increase the players xp and update various UI elements.
    /// </summary>
    /// <param name="xp"></param>
    private void IncreaseXP(int xp)
    {
        int prevLevel = info.GetLevel();
        info.AddXp(xp);
        _xpSlider.fillAmount = info.GetPercentageToNextLevel();
        int currentLevel = info.GetLevel();

        tempLevel.text = "" + currentLevel;

        if (powers.Count > 0 && prevLevel < currentLevel)
            UpdatePowers(currentLevel);
    }

    /// <summary>
    /// This function is used to increase the coin count and update various UI elements.
    /// </summary>
    /// <param name="coins"></param>
    private void IncreaseCoinCount(int coins)
    {
        info.AddCoins(coins);
        tempCoins.text = info.GetCoinCount().ToString("00000000");
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            print("ADDING COINS");
            IncreaseCoinCount(100);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            print("ADDING XP");
            IncreaseXP(100);
        }
    }

}
