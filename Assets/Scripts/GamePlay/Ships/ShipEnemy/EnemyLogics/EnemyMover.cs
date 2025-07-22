using UnityEngine;

public class EnemyMover : MonoBehaviour, IMove
{
    private Transform[] _pointPosition;

    private float _speed;
    private int _targetPoint;

    private void Update()
    {
        Move(_speed);
    }

    public void Init(float valueSpeed, Transform[] movePosition)
    {
        _speed = valueSpeed;
        _pointPosition = movePosition;
    }

    public void Move(float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, _pointPosition[_targetPoint].position, speed * Time.deltaTime);

        if ((transform.position - _pointPosition[_targetPoint].position).sqrMagnitude < 0.01f)
        {
            TakeRandomPoint();
        }
    }

    private void TakeRandomPoint()
    {
        _targetPoint = RandomNumber.Create(0, _pointPosition.Length);
    }
}
