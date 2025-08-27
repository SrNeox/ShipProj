using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

[RequireComponent(typeof(SearchPlayer))]
public class EnemyShoot : MonoBehaviour
{
    [Inject] private readonly PoolBullet _poolBullet;
    [Inject] private readonly Container _container;

    private Transform[] _firePoints;
    private AudioSource _audioSource;

    private float _fireRate = 1f;
    private float _nextFireTime = 0f;
    private float _speedBullet;
    private float _damage;

    private SearchPlayer _searchPlayer;

    private void Start()
    {
        SetFirePoints();
        _searchPlayer = GetComponent<SearchPlayer>();
    }

    private void Update()
    {
        if (_searchPlayer._playerPosition != null)
        {
            RotateToPlyer();

            if (Time.time > _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + 1f / _fireRate;
            }
        }
    }

    public void Init(float fireRate, Transform[] firePoints, float speedBullet, float damage, AudioSource audioSource)
    {
        _speedBullet = speedBullet;
        _damage = damage;
        _firePoints = firePoints;
        _fireRate = fireRate;
        _audioSource = audioSource;
    }

    private void RotateToPlyer()
    {
        Vector3 direction = _searchPlayer._playerPosition.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
    }

    private void Shoot()
    {
        for (int i = 0; i < _firePoints.Length; i++)
            SpawnBullet(_firePoints[i]);
    }

    private void SpawnBullet(Transform firepoint)
    {
        Bullet bullet = _poolBullet.GetObject();
        AttributeInjector.Inject(bullet, _container);
        bullet.Init(firepoint, _damage, _speedBullet);
        _audioSource.Play();
    }

    private void SetFirePoints()
    {
        int countFirePoints = transform.childCount;
        _firePoints = new Transform[countFirePoints];

        for (int i = 0; i < _firePoints.Length; i++)
            _firePoints[i] = transform.GetChild(i);
    }
}
