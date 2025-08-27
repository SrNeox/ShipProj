using UnityEngine;
using YG;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private int sETSOCER;
    private void Awake()
    {
        YG2.saves.Score = sETSOCER;
    }
}
