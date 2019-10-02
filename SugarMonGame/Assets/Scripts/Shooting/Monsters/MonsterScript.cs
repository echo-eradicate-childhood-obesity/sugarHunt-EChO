//This file was created by Mark Botaish on June 7th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class MonsterScript : MonoBehaviour
{
    #region PRIVATE_VARS
    protected Vector3 _cameraPosition;        // The position of the camera in the scene
    protected Vector3 _forwardDirection;      // The forward direction of the camera based on the spawn location
    protected Vector3 _frontClippingSpace;    // The position infront of the camera where monsters can't wonder beyond
    protected float _radius;                  // Raidus of the AI movement distance

    protected int _moveCounter = 0;           // The number bounces off the edge of the movement space
    protected int _chargeAtMove;              // The number of boucnes off the edgge of the movement space before charging the player
    protected float _health;                  // The current health of the monster
    protected float _maxHealth;               // The max health of the monster
    protected float _damage;                  // The damage given to the player
    protected int _coins;                     // Coins to give to the player on death
    protected int _xp;                        // Xp to give to the player on death

    public float _startingDamage = 25;                  // The damage of the monster at level 0
    public float _startingHealth = 100;                 // The health of the monster at level 0
    public int _startingXP = 50;                        // The xp to give to the player on death at level 0
    public int _startingCoins = 50;                     // The coins to give to the player on death at level 0
    protected float _damageMultiplierPerLevel = 1.5f;   // Multiplier of damage per the level in sugar group
    protected float _healthMultiplierPreLevel = 1.5f;   // Multiplier of health per the level in sugar group
    protected float _XPMultiplierPreLevel = 1.5f;       // Multiplier of xp per the level in sugar group
    protected float _CoinsMultiplierPreLevel = 1.5f;    // Multiplier of coins per the level in sugar group

    protected SpawnMonsters _sm;              // The instance to the SpawnMonster script
    protected Rigidbody _rigid;               // The Rigid body of this gameobject

    protected GameObject _redFill;            // A reference to the slider on the canvas
    protected GameObject _monsterCanvas;             // A reference to the canvas on the monster
    protected GameObject _damageText;         // The prefab to spawn when a projectile has hit the enemy

    protected PlayerScript _player;           // Singleton of the player script
    protected Camera _camera;                 // A reference to the main camera
    protected GameObject _canvas;             // A reference to the canvas

    protected bool _didDealDamage = false;    // A check to see if the monster has dealt damage to the player or not
    #endregion

    public void Start()
    {
        _damageText = Resources.Load("InWorldUI/DamageText") as GameObject;
        _rigid = GetComponent<Rigidbody>();
        _sm = SpawnMonsters.instance;
        _chargeAtMove = Random.Range(5,10);
        _monsterCanvas = transform.Find("MonsterCanvas").gameObject;
        _redFill = _monsterCanvas.transform.Find("Red").gameObject;
        _player = PlayerScript.instance;
        _camera = GameObject.Find("ARCore Device").GetComponentInChildren<Camera>();
    }

    /// <summary>
    ///  Init the variables need for the monster AI to work. Pos is the postion of the camera. The radius
    ///  is the max distance from the spawn point the monster can get and the health is the health of the 
    ///  monster.
    /// </summary>
    /// -This file gets called from the SpawnMonsters script-
    public void InitMonster(Vector3 pos, float radius, GameObject canvas, int level)
    {
        _cameraPosition = pos;

        //Create a position that is at the same height of the camera
        Vector3 temp = new Vector3(transform.position.x, pos.y, transform.position.z); 

        //Create a horizontal ray from the camera to the the spawn point
        _forwardDirection = (temp - pos).normalized;
        _frontClippingSpace = _cameraPosition + _forwardDirection * 2;

        _radius = radius;

        _health = _maxHealth = _startingHealth * Mathf.Pow(_healthMultiplierPreLevel, level);
        _damage = _startingDamage * Mathf.Pow(_damageMultiplierPerLevel, level); 
        _coins = (int)(_startingCoins * Mathf.Pow(_CoinsMultiplierPreLevel, level));
        _xp = (int)(_startingXP * Mathf.Pow(_XPMultiplierPreLevel, level));

        _canvas = canvas;
    }

    void FixedUpdate()
    {
        Attack();    
    }

    /// <summary>
    /// An abtract class that all enemies need to have. Used 
    /// to determine how an enemy can attack 
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// This function stops the monsters from going out of view of the camera. A new velocity is set
    /// when the distance is outside of the radius, the monster position is behind the camera and/or 
    /// when the monster is not in the correct downward/upward view.
    /// </summary>
    /// -This function gets called from the Update function-
    protected void checkPosition()
    { 
        //Check the raidus || check the front clipping space || check the downward/upward views
        if((transform.position - _frontClippingSpace).sqrMagnitude >= (_radius * _radius) || Vector3.Dot(_forwardDirection, transform.position - _frontClippingSpace) < 0 || !TestPoint(gameObject.transform.position))
        {
            if(_sm.GetNumOfMonstersAlive() > 1)
            {
                Vector3 vel = (_sm.GetNewPosition(gameObject) - transform.position).normalized * Random.Range(1,4);
                _rigid.velocity = vel;
            }
            else
            {
                _rigid.velocity = -_rigid.velocity;
            }
            _moveCounter++;
        }
    }

    /// <summary>
    ///  This function determines if the position is inside of the roaming area, specifically
    ///  the downward/upward view using straight line functions
    /// </summary>
    /// -This function gets called from the checkPosition function-
    bool TestPoint(Vector3 pos)
    {
        float mag = CheckEquation(new Vector2(pos.x, pos.z).magnitude);

        if (pos.y < mag && pos.y > -mag)
            return true;

        return false;
    }

    /// <summary> 
    /// This function is to get the y value from an x value of an equation. The current equation
    /// y = x, but can easily be changed later 
    /// </summary> 
    /// -This fucntion gets called from the TestPoint function-
    float CheckEquation(float x)
    {
        return x;
    }

    /// <summary>
    /// This function is to deal damage to the monster and update the UI slider.
    /// </summary>
    /// -This function gets called from the OnMouseDown as of now-
    public void DealDamage(float damage)
    {
        _health -= damage;       

        if(_health <= 0) 
            Destroy(gameObject);        
        else        
            _redFill.transform.localScale = new Vector3(_health / _maxHealth, 1,1);
    }
    
    private void OnDestroy() {
        if (!_didDealDamage)
        {
            PlayerInfoScript info = PlayerInfoScript.instance;
            if (info) //If the PlayerInfoScript exists (ran game from main menu) add proper xp/coins
            {
                info.AddCoinsInLevel(_coins);
                info.AddXPInLevel(_xp);
                info.AddToTotalKills();
            }
            else //For testing when running the game from this scene
            {
                Debug.Log("No PlayerInfoScript found!");
            }
        }       
        _sm.RemoveMonster(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            DealDamage(_player.GetDamage()); //Deal the proper damage to the enemy

            GameObject obj = Instantiate(_damageText, other.transform.position, Quaternion.identity); //Spawn 3D text
            obj.transform.LookAt(_cameraPosition); // Look at the camera
            obj.GetComponent<TextMeshPro>().text = "-" + _player.GetDamage(); // Update the 3D text damage
            obj.GetComponent<Rigidbody>().velocity = Vector3.up + Vector3.right; // Set the velocity of going up 

            //Clean up the projectile and the text
            Destroy(other.gameObject);
            Destroy(obj,1);
        }
    }

    /// <summary>
    /// Get the damage the monster can deal 
    /// </summary>
    /// <returns></returns>
    public float GetDamage() { return _damage; }

    /// <summary>
    /// Set the check to see if the monster has dealt damage before. 
    /// </summary>
    public void SetDidDealDamage() { _didDealDamage = true; }
}
