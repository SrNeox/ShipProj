using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float lifeTime = 5f;

    [Header("Arc Shot Indicator")]
    [SerializeField] private GameObject arcIndicatorPrefab;

    private PoolBullet _poolBullet;
    private float _speed;
    private float _damage;

    // Параметры для дуговых выстрелов
    private bool _isArcShot = false;
    private Vector3 _arcTarget;
    private float _arcHeight;
    private Coroutine _arcCoroutine;
    private GameObject _arcIndicator;

    private void Awake()
    {
        _poolBullet = FindObjectOfType<PoolBullet>();
    }

    private void OnEnable()
    {
        _isArcShot = false;

        // Останавливаем корутину если она была активна
        if (_arcCoroutine != null)
        {
            StopCoroutine(_arcCoroutine);
            _arcCoroutine = null;
        }

        // Удаляем старый индикатор если есть
        if (_arcIndicator != null)
        {
            Destroy(_arcIndicator);
            _arcIndicator = null;
        }
    }

    private void Update()
    {
        // Для прямых выстрелов используем стандартное движение
        if (!_isArcShot)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                ReturnToPool();
                return;
            }
            MoveStraight();
        }
    }

    public void Init(Transform position, float damage, float speedBullet)
    {
        _speed = speedBullet;
        _damage = damage;
        transform.SetPositionAndRotation(position.position, position.rotation);
        _isArcShot = false;
        lifeTime = 5f;
    }

    public void InitArc(Transform firePoint, float damage, float speed, Vector3 target, float arcHeight)
    {
        _speed = speed;
        _damage = damage;
        transform.position = firePoint.position;
        _isArcShot = true;
        _arcTarget = target;
        _arcHeight = arcHeight;
        lifeTime = 10f;

        // Направляем пулю в сторону цели
        Vector3 direction = (target - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Запускаем корутину для дугового движения
        if (_arcCoroutine != null)
            StopCoroutine(_arcCoroutine);
        _arcCoroutine = StartCoroutine(ArcMovementCoroutine());

        // Создаем индикатор цели
        CreateArcIndicator(target);
    }

    private void MoveStraight()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.forward, Space.Self);
    }

    private IEnumerator ArcMovementCoroutine()
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, _arcTarget);
        float duration = distance / _speed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Параболическое движение
            Vector3 currentPos = Vector3.Lerp(startPosition, _arcTarget, progress);
            currentPos.y += _arcHeight * Mathf.Sin(progress * Mathf.PI);

            // Поворот в направлении движения
            Vector3 moveDirection = (currentPos - transform.position).normalized;
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            transform.position = currentPos;
            yield return null;
        }

        // Удаляем индикатор при приземлении
        if (_arcIndicator != null)
        {
            Destroy(_arcIndicator);
            _arcIndicator = null;
        }

        ReturnToPool();
    }

    private void CreateArcIndicator(Vector3 targetPosition)
    {
        if (arcIndicatorPrefab != null)
        {
            _arcIndicator = Instantiate(arcIndicatorPrefab, targetPosition, Quaternion.identity);
            _arcIndicator.transform.SetParent(null);
            _arcIndicator.transform.position = targetPosition + Vector3.up * 0.1f;
        }
    }

    private void ReturnToPool()
    {
        if (_poolBullet != null)
        {
            _poolBullet.ReturnObject(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Health health))
        {
            health.TakeDamage(_damage);
            ReturnToPool();
        }

        if (collider.TryGetComponent(out Bullet bullet))
        {
            ReturnToPool();
        }

        // Проверка по слою земли
        if (_isArcShot && groundLayer == (groundLayer | (1 << collider.gameObject.layer)))
        {
            // Удаляем индикатор при попадании в землю
            if (_arcIndicator != null)
            {
                Destroy(_arcIndicator);
                _arcIndicator = null;
            }

            ReturnToPool();
        }
    }

    private void OnDisable()
    {
        // Останавливаем корутину при выключении объекта
        if (_arcCoroutine != null)
        {
            StopCoroutine(_arcCoroutine);
            _arcCoroutine = null;
        }

        // Удаляем индикатор
        if (_arcIndicator != null)
        {
            Destroy(_arcIndicator);
            _arcIndicator = null;
        }
    }
}
