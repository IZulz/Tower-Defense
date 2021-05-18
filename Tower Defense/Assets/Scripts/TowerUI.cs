using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image _towerIcon;

    Tower _towerPrefabs;
    Tower _currentSpawnedTower;

    public void SetTowerPrefabs(Tower tower)
    {
        _towerPrefabs = tower;
        _towerIcon.sprite = tower.GetTowerHeadIcon();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject newTowerObj = Instantiate(_towerPrefabs.gameObject);

        _currentSpawnedTower = newTowerObj.GetComponent<Tower>();
        _currentSpawnedTower.ToggleOrderInLayer(true);
    }

    public void OnDrag(PointerEventData evendata)
    {
        Camera mainCamera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        _currentSpawnedTower.transform.position = targetPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_currentSpawnedTower.PlacePosition == null)
        {
            Destroy(_currentSpawnedTower.gameObject);
        }
        else
        {
            _currentSpawnedTower.LockPlacement();
            _currentSpawnedTower.ToggleOrderInLayer(false);

            LevelManager.Instance.RegisterSpawnedTower(_currentSpawnedTower);

            _currentSpawnedTower = null;
        }
    }
}
