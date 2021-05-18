using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
            }
            return _instance;
        }
    }
    [Header("Player")]
    [SerializeField] private int _maxLives = 3;
    [SerializeField] private int _totalEnemy = 15;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Text _statusInfo;
    [SerializeField] private Text _livesInfo;
    [SerializeField] private Text _totalEnemyInfo;

    [SerializeField] Transform _towerUiParent;
    [SerializeField] GameObject _towerUiPrefabs;

    [SerializeField] Tower[] _towerPrefabs;
    [SerializeField] Enemy[] _enemyPrefabs;

    [SerializeField] Transform[] _enemyPath;
    [SerializeField] float _spawnDelay;

    List<Tower> _spawnedTower = new List<Tower>();
    List<Enemy> _spawnedEnemy = new List<Enemy>();
    List<Bullet> _spawnedBullet = new List<Bullet>();

    private int _currentLives;
    private int _enemyCounter;

    float _runningSpawnDellay;

    public bool IsOver { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InstantiateAlltowerUI();

        SetCurrentLives(_maxLives);

        SetTotalEnemy(_totalEnemy);
    }

    void InstantiateAlltowerUI()
    {
        foreach(Tower tower in _towerPrefabs)
        {
            GameObject newTowerUIObj = Instantiate(_towerUiPrefabs.gameObject, _towerUiParent);
            TowerUI newTowerUI = newTowerUIObj.GetComponent<TowerUI>();

            newTowerUI.SetTowerPrefabs(tower);
            newTowerUI.transform.name = tower.name;
        }
    }

    private void Update()
    {
        _runningSpawnDellay -= Time.unscaledDeltaTime;

        if (_runningSpawnDellay <= 0)
        {
            SpawnEnemy();
            _runningSpawnDellay = _spawnDelay;
        }

        foreach(Enemy enemy in _spawnedEnemy)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }

            if(Vector2.Distance(enemy.transform.position, enemy.TargetPosition) < 0.1f)
            {
                enemy.SetCurrentPathIndex(enemy.CurrentPathIndex + 1);
                    
                if(enemy.CurrentPathIndex < _enemyPath.Length)
                {
                    enemy.SetTargetPosition(_enemyPath[enemy.CurrentPathIndex].position);
                }
                else
                {
                    enemy.gameObject.SetActive(false);
                }
            }
            else
            {
                enemy.MoveToTarget();
            }
        }

        foreach (Tower tower in _spawnedTower)
        {
            tower.CheckNearestEnemy(_spawnedEnemy);
            tower.SeekTarget();
            tower.ShootTarget();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (IsOver)
        {
            return;
        }
    }

    public void RegisterSpawnedTower(Tower tower)
    {
        _spawnedTower.Add(tower);
    }

    private void SpawnEnemy()
    {
        SetTotalEnemy(--_enemyCounter);
        if (_enemyCounter < 0)
        {
            bool isAllEnemyDestroyed = _spawnedEnemy.Find(e => e.gameObject.activeSelf) == null;
            if (isAllEnemyDestroyed)
            {
                SetGameOver(true);
            }
            return;
        }

        int randomIndex = Random.Range(0, _enemyPrefabs.Length);
        string enemyIndexString = (randomIndex + 1).ToString();

        GameObject newEnemyObj = _spawnedEnemy.Find(
            e => !e.gameObject.activeSelf && e.name.Contains(enemyIndexString)
        )?.gameObject;

        if (newEnemyObj == null)
        {
            newEnemyObj = Instantiate(_enemyPrefabs[randomIndex].gameObject);
        }

        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();

        if (!_spawnedEnemy.Contains(newEnemy))
        {
            _spawnedEnemy.Add(newEnemy);
        }

        newEnemy.transform.position = _enemyPath[0].position;
        newEnemy.SetTargetPosition(_enemyPath[1].position);
        newEnemy.SetCurrentPathIndex(1);
        newEnemy.gameObject.SetActive(true);
    }

    public void ReduceLives(int value)
    {
        SetCurrentLives(_currentLives - value);
        if (_currentLives <= 0)
        {
            SetGameOver(false);
        }
    }

    public void SetTotalEnemy(int totalEnemy)
    {
        _enemyCounter = totalEnemy;
        _totalEnemyInfo.text = $"Total Enemy: {Mathf.Max(_enemyCounter, 0)}";
    }

    public void SetGameOver(bool isWin)
    {
        IsOver = true;
        _statusInfo.text = isWin ? "You Win!" : "You Lose!";
        _panel.gameObject.SetActive(true);
    }
    public void SetCurrentLives(int currentLives)
    {
        _currentLives = Mathf.Max(currentLives, 0);
        _livesInfo.text = $"Lives: {_currentLives}";
    }

    public Bullet GetBUlletFromPool(Bullet prefab)
    {
        GameObject newBulletObj = _spawnedBullet.Find(b => !b.gameObject.activeSelf && b.name.Contains(prefab.name))?.gameObject;

        if(newBulletObj == null)
        {
            newBulletObj = Instantiate(prefab.gameObject);
        }

        Bullet newBullet = newBulletObj.GetComponent<Bullet>();
        if (!_spawnedBullet.Contains(newBullet))
        {
            _spawnedBullet.Add(newBullet);
        }

        return newBullet;
    }

    public void ExplodeAt(Vector2 point, float radius, int damage)
    {
        foreach(Enemy enemy in _spawnedEnemy)
        {
            if (enemy.gameObject.activeSelf)
            {
                if(Vector2.Distance(enemy.transform.position, point) <= radius)
                {
                    enemy.ReduceEnemyHealth(damage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _enemyPath.Length - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_enemyPath[i].position, _enemyPath[i + 1].position);
        }
    }
}
