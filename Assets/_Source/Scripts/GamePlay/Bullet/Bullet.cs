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

    // ��������� ��� ������� ���������
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

        // ������������� �������� ���� ��� ���� �������
        if (_arcCoroutine != null)
        {
            StopCoroutine(_arcCoroutine);
            _arcCoroutine = null;
        }

        // ������� ������ ��������� ���� ����
        if (_arcIndicator != null)
        {
            Destroy(_arcIndicator);
            _arcIndicator = null;
        }
    }

    private void Update()
    {
        // ��� ������ ��������� ���������� ����������� ��������
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

        // ���������� ���� � ������� ����
        Vector3 direction = (target - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // ��������� �������� ��� �������� ��������
        if (_arcCoroutine != null)
            StopCoroutine(_arcCoroutine);
        _arcCoroutine = StartCoroutine(ArcMovementCoroutine());

        // ������� ��������� ����
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

            // �������������� ��������
            Vector3 currentPos = Vector3.Lerp(startPosition, _arcTarget, progress);
            currentPos.y += _arcHeight * Mathf.Sin(progress * Mathf.PI);

            // ������� � ����������� ��������
            Vector3 moveDirection = (currentPos - transform.position).normalized;
            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            transform.position = currentPos;
            yield return null;
        }

        // ������� ��������� ��� �����������
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

        // �������� �� ���� �����
        if (_isArcShot && groundLayer == (groundLayer | (1 << collider.gameObject.layer)))
        {
            // ������� ��������� ��� ��������� � �����
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
        // ������������� �������� ��� ���������� �������
        if (_arcCoroutine != null)
        {
            StopCoroutine(_arcCoroutine);
            _arcCoroutine = null;
        }

        // ������� ���������
        if (_arcIndicator != null)
        {
            Destroy(_arcIndicator);
            _arcIndicator = null;
        }
    }
}
