using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int _bulletPower;
    float _bulletSpeed;
    float _bulletSlashRadius;

    Enemy _targetEnemy;

    private void FixedUpdate()
    {
        if (LevelManager.Instance.IsOver)
        {
            return;
        }

        if (_targetEnemy != null)
        {
            if (!_targetEnemy.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                _targetEnemy = null;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.transform.position, _bulletSpeed * Time.fixedDeltaTime);

            Vector3 direction = _targetEnemy.transform.position - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_targetEnemy == null)
        {
            return;
        }

        if (collision.gameObject.Equals(_targetEnemy.gameObject))
        {
            gameObject.SetActive(false);

            //Bullet radius
            if(_bulletSlashRadius > 0)
            {
                LevelManager.Instance.ExplodeAt(transform.position, _bulletSlashRadius, _bulletPower);
            }//single target
            else
            {
                _targetEnemy.ReduceEnemyHealth(_bulletPower);
            }

            _targetEnemy = null;
        }
    }

    public void SetProperties(int bulletPower, float bulletSplashRadius, float bulletSpeed)
    {
        _bulletPower = bulletPower;
        _bulletSpeed = bulletSpeed;
        _bulletSlashRadius = bulletSplashRadius;
    }

    public void SetTargetEnemy(Enemy enemy)
    {
        _targetEnemy = enemy;
    }
}
