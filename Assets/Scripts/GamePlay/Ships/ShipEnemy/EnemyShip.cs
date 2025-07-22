using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemyShoot))]
[RequireComponent(typeof(SearchPlayer))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Awardee))]
public class EnemyShip : MonoBehaviour
{
    [Inject] private Container _container;

    [Header("Healt")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _valueHealth;

    [Header("Mover")]
    [SerializeField] private float _speed;

    [Header("Bullet Setting")]
    [SerializeField] private float _speedBullet;
    [SerializeField] private float _damage;
    [SerializeField] private float _fireRate;
    [SerializeField] private Transform[] _firePoints;

    [Header("SearchPlayer Setting")]
    [SerializeField] private LayerMask _layerMask;

    [Header("Reward")]
    [SerializeField] private int _score;

    private EnemyMover _mover;
    private EnemyShoot _shoot;
    private SearchPlayer _searchPlayer;
    private Health _health;
    private AudioSource _audioSource;
    private Awardee _reward;
    
    public Transform[] MovePoints { get; private set; }

    public void RestoreHealth() => _valueHealth = _maxHealth;

    public void SetMovePoints(Transform[] movePoint) => MovePoints = movePoint;

    private void Start()
    {
        SetComponent();
        Init();
        _valueHealth = _maxHealth;
    }

    public void Buff()
    {
        _maxHealth += 10;
        _speed += 0.3f;
        _speedBullet += 0.3f;
        _damage += 7;
        _fireRate += 0.2f;
        _valueHealth = _maxHealth;
    }

    private void Init()
    {
        AttributeInjector.Inject(_mover, _container);
        AttributeInjector.Inject(_shoot, _container);

        _mover.Init(_speed, MovePoints);
        _shoot.Init(_fireRate, _firePoints, _speedBullet, _damage, _audioSource);
        _searchPlayer.Init(_layerMask);
        _health.Init(_valueHealth, _maxHealth);
        _reward.Init(_score);
    }

    private void SetComponent()
    {
        _searchPlayer = GetComponent<SearchPlayer>();
        _health = GetComponent<Health>();
        _mover = GetComponent<EnemyMover>();
        _shoot = GetComponent<EnemyShoot>();
        _audioSource = GetComponent<AudioSource>();
        _reward = GetComponent<Awardee>();
    }
}
