using UnityEngine;
using TMPro;

public class EnemyStateUI : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public EnemyClassic enemy;

    void Update()
    {
        if (enemy != null && stateText != null)
            stateText.text = "Estado: " + enemy.GetCurrentState();
    }
}