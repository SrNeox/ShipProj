using System.Collections;
using System.Collections.Generic;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

[RequireComponent(typeof(DrawLine))]
[RequireComponent(typeof(InputPlayer))]
public class Shoot : MonoBehaviour
{
    [Inject] private readonly PoolBullet _pool;
    [Inject] private readonly Container _container;

    private Transform _firePoint;
    private InputPlayer _inputPlayer;
    private DrawLine _drawLine;
    private AudioSource _audioSource;

    private bool _canShoot = true;
    private float _damage;
    private float _speed;

    private Coroutine _shootDelay;
    private Coroutine _moveLineBullet;
    private WaitForSeconds _delay;

    private void Awake()
    {
        _inputPlayer = GetComponent<InputPlayer>();

        Debug.Log(_inputPlayer == null);

        _drawLine = GetComponent<DrawLine>();
    }

    private void Update()
    {
        ShootBullet();
    }

    public void Init(Transform firePoint, float delay, float damage, float speed, AudioSource audioSource)
    {
        _delay = new(delay);
        _damage = damage;
        _speed = speed;
        _firePoint = firePoint;
        _audioSource = audioSource;
    }

    public void ShootBullet()
    {
        if (_inputPlayer.Shoot() && _canShoot && _drawLine.MousePositionList.Count > 3)
        {
            ShooBulletUI();
        }
    }

    public void ShooBulletUI()
    {
        Debug.Log("Shot");

        if (_shootDelay != null)
            StopCoroutine(_shootDelay);

        _shootDelay = StartCoroutine(ShootDelay());
    }

    private IEnumerator ShootDelay()
    {
        _canShoot = false;

        if (_moveLineBullet != null)
            StopCoroutine(_moveLineBullet); 

        _moveLineBullet = StartCoroutine(MoveLineBullet());

        yield return _delay;

        _canShoot = true;
    }

    private Bullet CreateBullet()
    {
        Bullet bullet = _pool.GetObject();
        AttributeInjector.Inject(bullet, _container);
        bullet.Init(_firePoint, _damage, _speed);
        _audioSource.Play();

        return bullet;
    }

    private IEnumerator MoveLineBullet()
    {
        if (_drawLine.MousePositionList.Count == 0)
            yield break;

        var bullet = CreateBullet();

        List<Vector3> mousePositionsCopy = new(_drawLine.MousePositionList);

        foreach (Vector3 point in mousePositionsCopy)
        {
            Vector3 targetPosition = point;

            while ((bullet.transform.position - targetPosition).sqrMagnitude > 0.05f)
            {
                Vector3 direction = (targetPosition - bullet.transform.position).normalized;
                bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, targetPosition, Time.deltaTime * 10);
                bullet.transform.LookAt(bullet.transform.position + direction);
                yield return null;
            }
        }

        yield return null;
    }
}