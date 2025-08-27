using TMPro;
using UnityEngine;
using YG;

public class ViewRecornd : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = $"{YG2.saves.Score}";
    }
}
