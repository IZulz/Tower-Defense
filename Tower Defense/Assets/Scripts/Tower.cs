using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("TOwer Component")]
    [SerializeField] SpriteRenderer _towerPlace;
    [SerializeField] SpriteRenderer _towerHead;

    [Header("Tower porperties")]
    [SerializeField] int _shoootPower = 1;
    [SerializeField] float _shootDistance = 1f;
    [SerializeField] float _shootDelay = 5f;
    [SerializeField] float _bulletSpeed = 1f;
    [SerializeField] float _bulletSpalshRadius = 0f;

    [Header("Bullet")]
    [SerializeField] Bullet _bulletPrefabs;
    float _runningShootDelay;
    Enemy _targetEnemy;
    Quaternion _targetRotation;

    public void CheckNearestEnemy(List<Enemy> enemies)
    {
        if (_targetEnemy != null)
        {            
            if (!_targetEnemy.gameObject.activeSelf || Vector3.Distance(transform.position, _targetEnemy.transform.position) > _shootDistance)
            {
                _targetEnemy = null;
            }
            else
            {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;

        Enemy nearestEnemy = null;

        foreach(Enemy enemi in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemi.transform.position);

            if(distance > _shootDistance)
            {
                continue;
            }

            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemi;
            }
        }

        _targetEnemy = nearestEnemy;
    }

    public void ShootTarget()
    {
        if(_targetEnemy == null)
        {
            return;
        }

        _runningShootDelay -= Time.unscaledDeltaTime;
        if(_runningShootDelay <= 0)
        {
            bool headHasAimed = Mathf.Abs(_towerHead.transform.rotation.eulerAngles.z - _targetRotation.eulerAngles.z) < 10f;

            if (!headHasAimed)
            {
                return;
            }

            Bullet bullet = LevelManager.Instance.GetBUlletFromPool(_bulletPrefabs);
            bullet.transform.position = transform.position;
            bullet.SetProperties(_shoootPower, _bulletSpalshRadius, _bulletSpeed);
            bullet.SetTargetEnemy(_targetEnemy);
            bullet.gameObject.SetActive(true);

            _runningShootDelay = _shootDelay;
        }
    }

    public void SeekTarget()
    {
        if (_targetEnemy == null)
        {
            return;
        }

        Vector3 direction = _targetEnemy.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));

        _towerHead.transform.rotation = Quaternion.RotateTowards(_towerHead.transform.rotation, _targetRotation, Time.deltaTime * 180f);

    }

    public Vector2? PlacePosition { get; private set; }

    public void SetPlacePosition(Vector2? newPosition)
    {
        PlacePosition = newPosition;
    }

    public void LockPlacement()
    {
        transform.position = (Vector2)PlacePosition;
    }

    public void ToggleOrderInLayer(bool toFront)
    {
        int orderInLayer = toFront ? 2 : 0;
        _towerPlace.sortingOrder = orderInLayer;
        _towerHead.sortingOrder = orderInLayer;
    }

    public Sprite GetTowerHeadIcon()
    {
        return _towerHead.sprite;
    }
}
