using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SearchPlayer))]
public class EnemyShoot : MonoBehaviour
{
    private PoolBullet _poolBullet;

    private Transform[] _firePoints;
    private AudioSource _audioSource;

    private float _fireRate = 1f;
    private float _nextFireTime = 0f;
    private float _speedBullet;
    private float _damage;
    private float _arcShotProbability = 0.3f;

    private SearchPlayer _searchPlayer;

    private void Start()
    {
        SetFirePoints();
        _searchPlayer = GetComponent<SearchPlayer>();
        _poolBullet = FindObjectOfType<PoolBullet>();
    }

    private void Update()
    {
        if (_searchPlayer._playerPosition != null)
        {
            RotateToPlayer();

            if (Time.time > _nextFireTime)
            {
                DetermineShotType();
                _nextFireTime = Time.time + 1f / _fireRate;
            }
        }
    }

    public void Init(float fireRate, Transform[] firePoints, float speedBullet, float damage, AudioSource audioSource, float arcShotChance = 0.3f)
    {
        _speedBullet = speedBullet;
        _damage = damage;
        _firePoints = firePoints;
        _fireRate = fireRate;
        _audioSource = audioSource;
        _arcShotProbability = arcShotChance;
    }

    private void RotateToPlayer()
    {
        Vector3 direction = _searchPlayer._playerPosition.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
    }

    private void DetermineShotType()
    {
        bool isArcShot = Random.value < _arcShotProbability;

        if (isArcShot && _searchPlayer._playerPosition != null)
        {
            StartCoroutine(PerformArcShot());
        }
        else
        {
            PerformDirectShot();
        }
    }

    private void PerformDirectShot()
    {
        for (int i = 0; i < _firePoints.Length; i++)
            SpawnBullet(_firePoints[i]);
    }

    private IEnumerator PerformArcShot()
    {
        Vector3 randomPointNearPlayer = _searchPlayer.GetRandomPointInDetectionZone();

        // ∆дем перед выстрелом
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < _firePoints.Length; i++)
            SpawnArcBullet(_firePoints[i], randomPointNearPlayer);
    }

    private void SpawnBullet(Transform firepoint)
    {
        Bullet bullet = _poolBullet.GetObject();
        bullet.Init(firepoint, _damage, _speedBullet);
        _audioSource.Play();
    }

    private void SpawnArcBullet(Transform firepoint, Vector3 targetPosition)
    {
        Bullet bullet = _poolBullet.GetObject();
        bullet.InitArc(firepoint, _damage, _speedBullet * 0.5f, targetPosition, 5f);
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
