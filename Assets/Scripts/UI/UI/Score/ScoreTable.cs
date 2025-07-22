using TMPro;
using UnityEngine;

public class ScoreTable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textScore;

    private Health _health;

    private void OnEnable()
    {
        if( _health != null )
            _health.HealthOver += UpdateScoreText;

        UpdateScoreText();
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.HealthOver -= UpdateScoreText;
    }

    private void UpdateScoreText()
    {
        _textScore.SetText($"{LocalBank.Score}");
    }

    public void Init(Health health)
    {
        if (_health != null)
        {
            _health.HealthOver -= UpdateScoreText;
        }

        _health = health;
        _health.HealthOver += UpdateScoreText;

        UpdateScoreText();
    }
}
