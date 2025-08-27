using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Health _helathPlayer;
    [SerializeField] private UIStateMachine _stateMachine;

    private void Start()
    {
        _helathPlayer.HealthOver += StateGame.PauseGame;
        _helathPlayer.HealthOver += PrepareGameOver;
    }

    private void OnDestroy()
    {
        _helathPlayer.HealthOver -= StateGame.PauseGame;
        _helathPlayer.HealthOver -= PrepareGameOver;
    }

    public void SetHealthPlayer(Health health)
    {
        _helathPlayer = health;
    }

    private void PrepareGameOver()
    {
        _stateMachine.ShowGameOver();
        LocalBank.TryChangeScore();
        SetHealthPlayer(_helathPlayer);
    }
}
