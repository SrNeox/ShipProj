using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Health _helathPlayer;
    [SerializeField] private UIStateMachine _stateMachine;

    private void OnEnable()
    {
        _helathPlayer.HealthOver += StateGame.PauseGame;
        _helathPlayer.HealthOver += PrepareGameOver;
    }

    private void OnDisable()
    {
        _helathPlayer.HealthOver -= StateGame.PauseGame;
        _helathPlayer.HealthOver -= PrepareGameOver;
    }

    private void PrepareGameOver()
    {
        _stateMachine.ShowGameOver();
        LocalBank.TryChangeScore();
    }
}
