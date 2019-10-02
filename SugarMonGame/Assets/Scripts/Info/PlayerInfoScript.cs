//This script was created by Mark Botaish on June 2nd 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoScript : MonoBehaviour
{

    #region STRUCTS

    //This struct contains all of the info needed for a powerup
    [System.Serializable]
    public struct PowerUpGroup
    {
        public string _name;                // This is the name of the powerup

        //Settings
        public float _startingValue;        // This is the starting value of the powerup
        public float _increasePreLevel;     // This is the value that will increase the current value after a level up (additive or multiplicative)
        public bool _isAdditive;            // A Check to see if the _increasePreLevel is additive or multiplicative

        [Header("-----------------------------")]
        public int _level;                  // The current level of the powerup
        public int _maxLevel;               // The max level this powerup can have 
        [Header("-----------------------------")]

        //Buying
        public int _coinValue;              // The current coin cost to level up 
        public float _increaseCoinValue;     // This is the value that will increase the current value after a level up (additive or multiplicative)
        public bool isCoinAdditive;          // A Check to see if the _increaseCoinValue is additive or multiplicative

        private float _currentValue;        // The current value of the powerup 

        public void SetValue(float val) { _currentValue = val; }
        public float GetCurrentValue() { return _currentValue; }
    }

    #endregion

    #region PUBLIC VAR

    public static PlayerInfoScript instance;    //This is a reference to the singleton of this script
    [Tooltip("A list of all powerups and their infomation.")] public List<PowerUpGroup> powerGroup;

    #endregion

    #region PRIVATE_VARS

    private int _coins = 0;                         // The number of coins the player has
    private int _level = 0;                         // The current level of the player
    private int _xp = 0;                            // The current xp of the player
    private int _xpForNextLevel = 100;              // The xp needed to reach the next level
    private int _prevXP = 0;
    private float _nextLevelXPMultiplier = 1.3f;    // The multiplier to determine the xp needed for the next level

    private Dictionary<string, int> powerupsDict = new Dictionary<string, int>(); // A dictionary from name to index in the powerup list

    //While playing each level
    private int[] _currentLevelInGroup = {0,0,0,0,0,0,0};                         // The current level of each sugar group in the game
    private int _currentSugarGroup = 0;                                           // The current sugar group the player is playing
    private int _coinsFromLevel = 0;                                              // The amount of coins gained from the current level
    private int _xpFromLevel = 0;                                                 // The amount of xp gain from the current level

    private bool _hasAchievementBeenLoaded = false;                               // A check to see if the game has been just loaded
    private int _levelAchievementIndex = 1;                                       // The current completed achievement index for the level achievements
    private int _killAchievementIndex = 1;                                        // The current completed achievement index for the kills achievements

    private int _numberOfEnemiesKilled = 0;                                       // The total number of enemies killed
    #endregion
     
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        _coinsFromLevel = 0;
        _xpFromLevel = 0;

        int size = powerGroup.Count;
        for (int i = 0; i < size; i++)
        {
            PowerUpGroup power = powerGroup[i];
            powerupsDict.Add(power._name, i);
            power.SetValue(powerGroup[i]._startingValue);

            powerGroup[i] = power;
        }

        DontDestroyOnLoad(gameObject);
    }

    #region POWER_INFO_FUNCTIONS

    /// <summary>
    /// This fucntion is used to increase the power level of a certain powerup.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="level"></param>
    public void SetPowerLevel(string name, int level)
    {
        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];
            power._level = level;

            if (power._isAdditive)
                power.SetValue((power._increasePreLevel * power._level) + power._startingValue);
            else
                power.SetValue(power.GetCurrentValue() * Mathf.Pow(power._increasePreLevel, level) + power._startingValue);

            powerGroup[powerupsDict[name]] = power;
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
    }

    /// <summary>
    /// This fucntion is used to increase the power value of a certain powerup.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetPowerValue(string name, float value)
    {
        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];
            power.SetValue(value);

            powerGroup[powerupsDict[name]] = power;
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
    }

    /// <summary>
    /// This fucntion is used to add to the power level of a certain powerup. Defaulted at 1 level
    /// </summary>
    /// <param name="name"></param>
    /// <param name="level"></param>
    public int BuyLevel(string name, int level = 1)
    {

        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];

            if (power._level >= power._maxLevel)
                return 0;

            if (_coins < power._coinValue)
                return -1;

           _coins -= power._coinValue;
            power._level += level;

            if (power._isAdditive)
                power.SetValue(power._increasePreLevel + power.GetCurrentValue());
            else
                power.SetValue(power._increasePreLevel * power.GetCurrentValue());

            if (power.isCoinAdditive)
                power._coinValue += (int)power._increaseCoinValue;
            else
                power._coinValue = (int)(power._coinValue * power._increaseCoinValue);

            powerGroup[powerupsDict[name]] = power;

            return power._coinValue;
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
        return -1;
    }

    public int GetBuyAmmount(string name)
    {
        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];
            return power._coinValue;
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
        return -1;
    }

    /// <summary>
    /// This fucntion is used to add to the power value of a certain powerup.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddPowerValue(string name, int value)
    {
        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];
            power.SetValue(value + power.GetCurrentValue());

            powerGroup[powerupsDict[name]] = power;
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
    }

    /// <summary>
    /// This function is used to get the power level of a certain powerup
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetPowerLevel(string name)
    {
        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];
            return power._level;
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
        return -1;
    }

    /// <summary>
    /// This function is used to get the current value of a certain powerup
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public float GetPowerValue(string name)
    {
        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];
            return power.GetCurrentValue();
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
        return -1;
    }

    /// <summary>
    /// This function is used to get the max power level of a power up
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetMaxPowerLevel(string name)
    {
        if (powerupsDict.ContainsKey(name))
        {
            PowerUpGroup power = powerGroup[powerupsDict[name]];
            return power._maxLevel;
        }
        else
        {
            Debug.LogError("The key <" + name + "> does not exist in the current dictionary");
        }
        return -1;
    }

    #endregion

    //Setters
    public void SetLevel(int level) { _level = level; _xpForNextLevel = (int)(_xpForNextLevel * Mathf.Pow(_nextLevelXPMultiplier, _level)); }
    public void SetXP(int xp) { _xp = xp; }
    public void SetCoins(int coins) { _coins = coins; }
    public void SetPrevXp(int xp) { _prevXP = xp; }
    public void SetCurrentSugarGroup(int level) {
        _currentSugarGroup = level;
        _coinsFromLevel = 0;
        _xpFromLevel = 0;
    }
    public void setAchievementLoaded(bool tof) { _hasAchievementBeenLoaded = tof; }
    public void SetAchievementLevel(int level) { _levelAchievementIndex = level; }
    public void SetAchievementKillLevel(int level) { _killAchievementIndex = level; }

    //Changers 
    public void AddCoins(int coins) { _coins += coins; print(_coins + " coins \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"); }
    public void AddXp(int xp)
    {
        _xp += xp;
        while (_xp >= _xpForNextLevel)
        {
            _prevXP = _xpForNextLevel;
            _level++;
            _xpForNextLevel = (int)(_xpForNextLevel * _nextLevelXPMultiplier);
        }
        print(_xp + " XPs \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
    }
    public void AddLevelInSugarGroup() { _currentLevelInGroup[_currentSugarGroup]++; }
    public void AddCoinsInLevel(int coins) { _coinsFromLevel += coins; AddCoins(coins); }
    public void AddXPInLevel(int xp) { _xpFromLevel += xp; AddXp(xp); }
    public void AddToTotalKills() { _numberOfEnemiesKilled++; }

    //Getters
    public int GetCoinCount(){return _coins;}
    public int GetLevel() { return _level; }
    public int GetXp() { return _xp; }
    public float GetPercentageToNextLevel() { return ((float)(_xp - _prevXP)) / ((float)(_xpForNextLevel - _prevXP)); }
    public int GetLevelInSugarGroup(int index) { return _currentLevelInGroup[index]; }
    public int GetCurrentGroup() { return _currentSugarGroup; }
    public int GetXpFromLevel() { return _xpFromLevel; }
    public int GetCoinsFromLevel() { return _coinsFromLevel; }
    public bool HasAchievementBeenLoaded() { return _hasAchievementBeenLoaded; }
    public int GetLevelAchievementIndex() { return _levelAchievementIndex; }
    public int GetKillAchievementIndex() { return _killAchievementIndex; }
    public int GetTotalKills() { return _numberOfEnemiesKilled; }
}
