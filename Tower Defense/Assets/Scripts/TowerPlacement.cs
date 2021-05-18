using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public Tower _placedTower;
    bool _isPlaced = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isPlaced == false)
        {
            if (_placedTower != null)
            {
                return;
            }

            Tower tower = collision.GetComponent<Tower>();
            if (tower != null)
            {
                tower.SetPlacePosition(transform.position);
                _placedTower = tower;
                _isPlaced = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(_placedTower == null)
        {
            return;
        }

        _placedTower.SetPlacePosition(null);
        _placedTower = null;
    }
}
