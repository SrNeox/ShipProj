using System.Collections;
using TMPro;
using UnityEngine;

public class TryShoot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textTimer;

    private WaitForSeconds _delay = new(10);
    private WaitForSeconds _delayText = new(1);
    private Coroutine _coroutine;
    private int _time = 10;

    private DrawLine _drawLine => _drawLine != null ? _drawLine : FindAnyObjectByType<DrawLine>();
    private EnemyMover _enemyMover => _enemyMover != null ? _enemyMover : FindAnyObjectByType<EnemyMover>();

    private void Start()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    private IEnumerator Shoot()
    {
        DisableTry();

        while (_time > 0)
        {
            _textTimer.SetText(_time.ToString());
            yield return _delayText;
            --_time;
        }

        _time = 10;

        EnebleTry();




        yield return _delay;
    }

    private void DisableTry()
    {
        _drawLine.enabled = false;
        _enemyMover.enabled = false;
    }

    private void EnebleTry()
    {
        _drawLine.enabled = true;
        _enemyMover.enabled = true;
    }
}   
