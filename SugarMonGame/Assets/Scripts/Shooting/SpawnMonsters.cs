//This file was created by Mark Botaish on June 7th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnMonsters : MonoBehaviour
{
    #region PUBLIC_VARS
    public static SpawnMonsters instance;
    public int sugarCatagory;

    [Tooltip("Chosen Monster")]                                     private GameObject _chosenMonster;
    [Tooltip("Cane Monster")]                                       public GameObject _caneMonster;
    [Tooltip("Concentrate Monster")]                                public GameObject _concentrateMonster;
    [Tooltip("Dextrin Monsters")]                                   public GameObject _dextrinMonster;
    [Tooltip("Obvios Monster")]                                     public GameObject _obviosMonster;
    [Tooltip("OSE Monster")]                                        public GameObject _OSEMonster;
    [Tooltip("Strange Monster")]                                    public GameObject _strangeMonster;
    [Tooltip("Syrup Monster")]                                      public GameObject _syrupMonster;

    [Tooltip("The max distance from the camera a monster can get")] public float _radius = 10;
    [Tooltip("Win Panel reference")]                                public GameObject _winPanel;
    #endregion

    #region PRIVATE_VARS
    private Transform _cameraTransform;                              // The transfrom of the camera in the scene
    private List<GameObject> _monsters = new List<GameObject>();     // The list of monsters
    private GameObject _canvas;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = GameObject.Find("ARCore Device").transform.GetChild(0);
        _canvas = GameObject.Find("Canvas");
        StartCoroutine(SpawnEnemies(10));
    }

    /// <summary>
    /// This fucntion is used to spawn a number of monsters.
    /// </summary>
    IEnumerator SpawnEnemies(int num)
    {
        PlayerInfoScript player = PlayerInfoScript.instance;
        int index;

        if (player)
        {
            index = player.GetCurrentGroup();
        }
        else
        {
            index = 0;
        }

        // set monsters depending on the current chosen sugar type
        switch (index)
        {
            // cane
            case 0:
                _chosenMonster = _caneMonster;
                break;
            // concentrate
            case 1:
                _chosenMonster = _concentrateMonster;
                break;
            // dextrin
            case 2:
                _chosenMonster = _dextrinMonster;
                break;
            // Obvios
            case 3:
                _chosenMonster = _obviosMonster;
                break;
            // ose
            case 4:
                _chosenMonster = _OSEMonster;
                break;
            // strange
            case 5:
                _chosenMonster = _strangeMonster;
                break;
            // syrup
            case 6:
                _chosenMonster = _syrupMonster;
                break;
            default:
                _chosenMonster = _OSEMonster;
                break;
        }

        while (_monsters.Count < num)
        {
            player = PlayerInfoScript.instance;
            GameObject monster = Instantiate(_chosenMonster, transform.position, Quaternion.identity);
            // generate random velocity random
            Vector3 vel = Random.onUnitSphere * Random.Range(2, 5);
            // setting the velocity 
            monster.GetComponent<Rigidbody>().velocity = vel;

            //If the PlayerInfoScript exists (started game from menu)
            if (player)
            {
                //Each level is offset but 10 levels to make it more challenging in other groups
                int level = player.GetLevelInSugarGroup(player.GetCurrentGroup());
                print("level: " + level);
                monster.GetComponent<MonsterScript>().InitMonster(_cameraTransform.position, _radius, _canvas, level);//leveling setting here ##############
            }
            else //For testing purposes 
                monster.GetComponent<MonsterScript>().InitMonster(_cameraTransform.position, _radius, _canvas, 0);

            monster.transform.LookAt(_cameraTransform.position);
            _monsters.Add(monster);
            yield return null;
        }
    }

    /// <summary>
    ///  This function gets the position of a random monster in the list. This allows the AI to look more
    ///  randomizes. This position is used to determine the new direction of the gameobject
    /// </summary>
    ///  -This function gets called from the MonsterScript-
    public Vector3 GetNewPosition(GameObject obj)
    {
        GameObject newObj = null;
        if (_monsters.Count > 1)
        {
            do
            {
                newObj = _monsters[Random.Range(0, _monsters.Count)];
            } while (obj == newObj);
        }
        else
            newObj = gameObject;

        return newObj.transform.position;
    }

    /// <summary>
    /// This function is used to remove a destroyed monster from the list.
    /// </summary>
    /// -This function gets called from the MonterScript-
    public void RemoveMonster(GameObject obj)
    {
        _monsters.Remove(obj);
        if (_monsters.Count <= 0 && !PlayerScript.instance.IsDead())
        {
            PlayerInfoScript info = PlayerInfoScript.instance;
            if (PlayerScript.instance._IsTesting) //If you are using a computer, lock the mouse to the middle of the Game window
                Cursor.lockState = CursorLockMode.None;

            //Update the stats of the player
            info.AddLevelInSugarGroup();
            _winPanel.SetActive(true); //Display win screen 
            string stats = "Coins: " + info.GetCoinsFromLevel() + "\nXP: " + info.GetXpFromLevel();
            _winPanel.transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = stats; //Update stats
        }

    }

    /// <summary>
    /// This function is used to determine the currebt number of alive monsters in the scene.
    /// </summary>
    /// -This function gets called from the MonsterScript-
    public int GetNumOfMonstersAlive(){return _monsters.Count;}
}
