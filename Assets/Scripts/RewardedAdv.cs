using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Button))]
public class RewardedAdv : MonoBehaviour
{
    [SerializeField] private Health _playerHealth;
    [SerializeField] private UIStateMachine _stateMachineUi;
    [SerializeField] private PoolBullet _bulletPool;

    private Button _rewardedButton;

    public string rewardID;


    private void Start()
    {
        _rewardedButton = GetComponent<Button>();

        _rewardedButton.onClick.AddListener(RevivePlayer);
    }

    private void RevivePlayer()
    {
        YG2.RewardedAdvShow(rewardID, () =>
        {
            _bulletPool.ReaturnAllObject();
            _playerHealth.SetHealth();
            _stateMachineUi.ShowActiveGameUi();
            _rewardedButton.gameObject.SetActive(false);
            StateGame.ResumeGame();
        });
    }
}
