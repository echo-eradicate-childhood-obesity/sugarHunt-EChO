using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementsScript : MonoBehaviour 
{
    public static AchievementsScript instance;

    #region STRUCTS
    [System.Serializable]
    public class Achieve
    {
        [HideInInspector] public GameObject _UIElement;
        public string wording;
        public int _value;
        public int _coinReward;
        public int _xpReward;
        public bool isHidden;
    }
    #endregion

    #region PUBLIC_VAR
    [Tooltip("A list of achievements for reaching a certain level.")] public List<Achieve> _levelAchievements;
    [Tooltip("A list of achievements for kills a certain amount of enemies.")] public List<Achieve> _killAchivements;

    [Tooltip("The prefab for the achievement GameObject")] public GameObject _achievementPrefab;
    [Tooltip("A reference to the AchievementScroller")] public GameObject _achievementScroller;
    [Tooltip("A reference to the LevelUIScript")]public LevelUIManager _levelUIManager;                    
    #endregion

    #region PRIVATE_VAR
    private PlayerInfoScript info;                              // A reference to the PlayerInfoScript
    private List<Achieve> _completed = new List<Achieve>();     // A list of completed achievements. 
    #endregion

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        info = PlayerInfoScript.instance; 

        if (info == null)
            Debug.LogError("PlayerInfoScript not found");

        //Update the achievement list and get the index of completion 
        int levelIndex = UpdateAchievements(info.GetLevelAchievementIndex(), info.GetLevel(), _levelAchievements, 0);
        int killIndex = UpdateAchievements(info.GetKillAchievementIndex(), info.GetTotalKills(), _killAchivements, 1);

        _achievementScroller.GetComponent<ScrollRect>().verticalNormalizedPosition = 1; //Automatically scroll to the top

        //If the game hasn't be loaded before, then set the achievement index for each achievement.
        if (!info.HasAchievementBeenLoaded())
        {
            info.SetAchievementLevel(levelIndex);
            info.SetAchievementKillLevel(killIndex);
        }
        
        //Update the UI for coins and the xp bar
        _levelUIManager.UpdateUIStats();
        OrderAchievements();
    }

    /// <summary>
    /// This functon is used to update the achievement list. Upon completion, the text will turn green
    /// and the achievement will be placed in the completed list. 
    /// </summary>
    /// <param name="completedIndex"></param>
    /// <param name="value"></param>
    /// <param name="achievement"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private int UpdateAchievements(int completedIndex, int value, List<Achieve> achievement, int key = 0)
    {
        int xpToAdd = 0;
        int index = 0;
        for (int i = 0; i < achievement.Count; i++)
        {
            //If a UI element hasnt been made for the achievement, then create onne
            if (achievement[i]._UIElement == null) 
            {
                GameObject achieve = Instantiate(_achievementPrefab, _achievementScroller.transform.GetChild(0));
                achievement[i]._UIElement = achieve;
                if (achievement[i].isHidden)
                    achieve.GetComponent<TextMeshProUGUI>().text = " HIDDEN";
                else
                    achieve.GetComponent<TextMeshProUGUI>().text = " " + achievement[i].wording;
            }


            //If the value is create than the achievement value, then set this achievement as complete
            if (value >= achievement[i]._value)
            {
                _completed.Add(achievement[i]);

                if (index > completedIndex - 1)
                {
                    print(achievement[i].wording);
                    info.AddCoins(achievement[i]._coinReward);
                    xpToAdd += achievement[i]._xpReward;
                }

                achievement[i]._UIElement.GetComponent<TextMeshProUGUI>().color = Color.green;
                achievement.RemoveAt(i);
                i--;
                index++;
            }
        }

        //If an achievement has been made and you need the rewards, gain the rewards and make sure that another achievement hasn't been achieved
        if (xpToAdd > 0)
        {
            info.AddXp(xpToAdd);
            switch (key)
            {
                case 0:
                    break;
                case 1:
                    UpdateAchievements(info.GetKillAchievementIndex(), info.GetTotalKills(), _killAchivements, 1);
                    break;
                default:
                    Debug.Log("KEY NOT FOUND");
                    break;
            };
            UpdateAchievements(info.GetLevelAchievementIndex(), info.GetLevel(), _levelAchievements, 0);
        }
        return index;
    }

    /// <summary>
    /// This functions ensures that all completed achievements are to be placed at the bottom of the 
    /// achievement list in the menu panel.
    /// </summary>
    private void OrderAchievements()
    {
        foreach(Achieve complete in _completed)
        {
            complete._UIElement.transform.SetAsLastSibling();
        }
    }

}
