using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

public class SpawerEnemy : MonoBehaviour
{
    [Inject] private PoolEnemies _poolEnemies;
    [Inject] private Container _container;

    [SerializeField] private Transform[] _movePoint;
    [SerializeField] private ScoreTable _scoreTable;

    private Health _healthEnemy;
    private EnemyShip _enemyShip;

    private void Awake()
    {
        Spawn();
    }

    private void Spawn()
    {
        _enemyShip = _poolEnemies.GetObject();

        InitEnemy();
    }

    private void ReturnShip()
    {
        _healthEnemy.HealthOver -= ReturnShip;
        _enemyShip.RestoreHealth();
        _enemyShip.Buff();
        _poolEnemies.ReturnObject(_enemyShip);

        Spawn();
    }

    private void InitEnemy()
    {
        AttributeInjector.Inject(_enemyShip, _container);

        _healthEnemy = _enemyShip.GetComponent<Health>();

        _healthEnemy.HealthOver += ReturnShip;
        _scoreTable.Init(_healthEnemy);

        if (_enemyShip.MovePoints == null)
        {
            _enemyShip.SetMovePoints(_movePoint);
        }

        _enemyShip.transform.position = transform.position;
    }
}
