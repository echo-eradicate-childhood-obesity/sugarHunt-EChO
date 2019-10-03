//This file was created by Mark Botaish on June 14th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    //Singleton instance
    public static PlayerScript instance;

    #region PUBLIC_VARS
    [Header("Testing")]
    [Tooltip("Enables the ability to test on computers")]   public bool _IsTesting = false;   

    [Header("Mouse Settings")]
    [Tooltip("Horizontal mouse sensitivity")]               public float _mouseSensitivityX = 3.0f;    
    [Tooltip("Vertical mouse sensitivity")]                 public float _mouseSensitivityY = 3.0f;    

    [Header("Shooting Settings")]
    [Tooltip("The projectile prefab")]                      public GameObject _projectile;                        
    [Tooltip("How fast the projectile is shot")]            public float _projectileSpeed = 10;        

    [Header("UI")]
    [Tooltip("The Health bar image")]                       public GameObject _redHealthImage;
    [Tooltip("The red panel to flash when damaged")]        public GameObject _damageHealthImage;
    [Tooltip("A reference to the death panel")]             public GameObject _deathPanel;
    #endregion

    #region PRIVATE_VARS
    //Bullet settings
    private bool _canShoot = true;                  // A check to see if the bullet can shoot. This allows for a delayed shot
    private float _bulletTimer = 0;                 // The bullet timer to wait the time between shots

    //Mouse settings
    private float _mouseX = 0.0f;                   // The current x rotation based on the mouse movement
    private float _mouseY = 0.0f;                   // The current y rotation based on the mouse movement

    private float _health;                          // The current Health of the player from the PlayerInfoScript
    private float _startingHealth;                  // The startng health of the player
    private float _damage;                          // The current Damage of the player from the PlayerInfoScript
    private float _armour;                          // The current armour of the player from the PlayerInfoScript
    private float _rapidDelay;                      // The current delay between bullets from the PlayerInfoScript

    private bool _hasDied = false;                  // A check to see if the player is dead or not 
    #endregion

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (_IsTesting) //If you are using a computer, lock the mouse to the middle of the Game window
            Cursor.lockState = CursorLockMode.Locked;

        if(PlayerInfoScript.instance != null)
        {
            PlayerInfoScript info = PlayerInfoScript.instance;
            _health = info.GetPowerValue("Health");
            _damage = info.GetPowerValue("Damage");
            _armour = info.GetPowerValue("Armour");
            _rapidDelay = info.GetPowerValue("Rapid");
        }
        else
        {
            if(!_IsTesting)
                Debug.LogError("No PlayerInfoScript found");

            //Default values 
            _health = 100.0f; 
            _damage = 25.0f;
            _armour = 1.0f;
            _rapidDelay = 0.15f;
        }
        _startingHealth = _health;
    }

    private void Update()
    {
        if (_IsTesting) //If you are testing on computer, then the camera moves with the mouse
        {
            _mouseX += Input.GetAxis("Mouse X") * _mouseSensitivityX;
            _mouseY -= Input.GetAxis("Mouse Y") * _mouseSensitivityY;

            transform.eulerAngles = new Vector3(_mouseY, _mouseX, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10,null);
            print("I AM DYING, Health = " + _health);
        }

        Shoot();
    }

    /// <summary>
    /// This function is used to shoot a projectile. For now, holding/clicking the left mouse button will shoot.
    /// </summary>
    private void Shoot()
    {
        Transform trans;
        if (_IsTesting)
            trans = transform;
        else
            trans = transform.GetChild(0);

        //If between shots update the timer
        if (!_canShoot) 
        {
            _bulletTimer += Time.deltaTime;
            //If the timer is about the delay time, enable the ability to shoot
            if (_bulletTimer > _rapidDelay)
                _canShoot = true;
        }

        //If you can shoot and you are holding the left mouse button down
        if (_canShoot && (Input.touchCount > 0 || Input.GetMouseButton(0))) 
        {
            Vector3 shootPos = trans.position - trans.up * 0.1f + trans.forward * 0.1f; //Shooting the projectile slightly below and infront of the camera
            GameObject obj = Instantiate(_projectile, shootPos, Quaternion.identity); //Spawn the projectile

            //This makes it look like the projectile it moving at the crosshair location
            if (Physics.Raycast(trans.position, trans.forward, out RaycastHit hit, Mathf.Infinity, 9)) //If the crosshair hits something move the projectile to that postion
                obj.GetComponent<Rigidbody>().velocity = (hit.point - shootPos).normalized * _projectileSpeed;
            else // Move the projectile at a position that is 60 meters direction in the path of the crosshair
                obj.GetComponent<Rigidbody>().velocity = ((trans.forward * 60 + trans.position) - trans.position).normalized * _projectileSpeed;
            
            
            _canShoot = false;//Enable the day 
            _bulletTimer = 0;//Reset the timer

            //Cleanup for the projectile if it doesnt hit anything within 3 seconds
            Destroy(obj, 3); 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Monster") //If the collider belongs to a monster deal damage    
        {
            TakeDamage(collision.gameObject.GetComponent<MonsterScript>().GetDamage(), collision.gameObject);
            collision.gameObject.GetComponent<MonsterScript>().SetDidDealDamage();
            print("2");
        }           
        if(collision.gameObject.tag == "EnemyBullet") //If the collider belongs to an enemy bullet deal damage
            TakeDamage(collision.gameObject.GetComponent<EnemyBulletScript>()._damage, collision.gameObject);              
    }

    /// <summary>
    /// This function is used to subtract the damage from the health and
    /// to update the health bar.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="obj"></param>
    private void TakeDamage(float damage, GameObject obj)
    {
        _health -= (damage * _armour);
        
        Destroy(obj);
        if (_health <= 0) //If dead
        {
            RemoveAllEnemies();
            _hasDied = true;
            _deathPanel.SetActive(true);
            if (_IsTesting) //If you are using a computer, unlock the mouse.
                Cursor.lockState = CursorLockMode.None;
            this.enabled = false;
            _health = 0;

            PlayerInfoScript info = PlayerInfoScript.instance;
            string stats = "Coins: " + info.GetCoinsFromLevel() + "\nXP: " + info.GetXpFromLevel();
            _deathPanel.transform.Find("Stats").GetComponent<TextMeshProUGUI>().text = stats;

        }
        StartCoroutine(Damage()); //flash red on screen
        _redHealthImage.transform.localScale = new Vector3(_health / _startingHealth, 1, 1); //Update the x scale of health bar
    }

    /// <summary>
    /// This function is used to remove all monsters in the scene. MUST BE TAGGED -Monster-
    /// </summary>
    private void RemoveAllEnemies()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        foreach(GameObject enemy in monsters)
        {
            Destroy(enemy);
        }
    }

    /// <summary>
    /// This function is used to flash the screen red when the player has take damage.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Damage()
    {
        _damageHealthImage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _damageHealthImage.SetActive(false);
    }

    /// <summary>
    /// Return the damage a player can do.
    /// </summary>
    /// <returns></returns>
    public float GetDamage() { return _damage; }

    /// <summary>
    /// Return true if the player is considered dead (health <= 0)
    /// </summary>
    /// <returns></returns>
    public bool IsDead() { return _hasDied; }
}
