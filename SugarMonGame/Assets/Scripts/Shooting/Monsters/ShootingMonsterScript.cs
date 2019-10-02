//This script was created by Mark Botaish on July 2nd, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingMonsterScript : MonsterScript
{
    #region PUBLIC_VAR
    public GameObject _bulletPrefab;        // A reference to the bullet prefab the monster will shoot
    public float _bulletSpeed;              // The bullet speed of the monsters
    #endregion

    #region PRIVATE_VARS
    private bool _isTurning = false;            // Check to see if the monster is turning towards the player
    private bool _canShoot = false;             // Check to see if the monster is ready to shoot or not
    #endregion

    /// <summary>
    /// The attack for the shooting monster. 
    /// If the monster has stop bouncing, turn towards the player 
    /// and fire two shots. Then continue moving.
    /// </summary>
    public override void Attack()
    {
        if (_moveCounter < _chargeAtMove)
        {
            checkPosition();
            gameObject.transform.LookAt(gameObject.transform.position + _rigid.velocity);
        }           
        else
        {
            _rigid.velocity = Vector3.zero;
            if (!_isTurning)
            {
                StartCoroutine(TurnToPlayer());
            }

            if (_canShoot)
            {
                StartCoroutine(Shoot());
            }            
        }            
    }

    /// <summary>
    /// This function is used to lerp the rotation to face the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurnToPlayer()
    {
        _isTurning = true;
        Quaternion lookRotation = Quaternion.LookRotation(_cameraPosition - transform.position);
        while (Quaternion.Angle(lookRotation, gameObject.transform.rotation) > 0.5f)
        {
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2.0f);
            yield return null;
        }   

        yield return null;
        _canShoot = true;
    }

    /// <summary>
    /// This function is used to shoot twice at the player with a delay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shoot()
    {
        _canShoot = false;
        for(int i = 0; i < 2; i++)
        {
            GameObject bullet = Instantiate(_bulletPrefab, gameObject.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = (_cameraPosition - gameObject.transform.position).normalized * _bulletSpeed;
            bullet.GetComponent<EnemyBulletScript>()._damage = _damage;
            CanvasScript.instance.CreateWarningFromObject(bullet);
            yield return new WaitForSeconds(1.0f);
        }

        yield return null;
        _isTurning = false;
        _moveCounter = 0;
        _chargeAtMove = Random.Range(3, 7);
        _rigid.velocity = (_sm.GetNewPosition(gameObject) - transform.position).normalized * Random.Range(1, 4); ;
    }
}
