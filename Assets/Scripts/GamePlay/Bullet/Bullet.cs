using Reflex.Attributes;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Inject] private readonly PoolBullet _poolBullet;

    private float _speed;
    private float _damage;

    private void Update()
    {
        Move();
    }

    public void Init(Transform position, float damage, float speedBullet)
    {
        _speed = speedBullet;
        _damage = damage;
        transform.SetPositionAndRotation(position.position, position.rotation);
    }

    private void Move() => transform.Translate(_speed * Time.deltaTime * Vector3.forward, Space.Self);

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Health health))
        {
            health.TakeDamage(_damage);
            _poolBullet.ReturnObject(this);
        }

        if (collider.TryGetComponent(out Bullet bullet))
        {
            _poolBullet.ReturnObject(this);
        }
    }
}
