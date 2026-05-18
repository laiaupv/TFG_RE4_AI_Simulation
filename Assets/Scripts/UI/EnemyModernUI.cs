using UnityEngine;
using TMPro;

public class EnemyModernUI : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI noiseText;
    public EnemyModern enemy;
    public PlayerController player;

    void Update()
    {
        if (enemy != null && stateText != null)
            stateText.text = "Estado enemigo: " + enemy.GetCurrentState();

        if (player != null && noiseText != null)
        {
            if (player.isMakingNoise)
            {
                noiseText.text = "Jugador: corriendo (haciendo ruido)";
                noiseText.color = Color.red;
            }
            else
            {
                noiseText.text = "Jugador: caminando (silencioso)";
                noiseText.color = Color.green;
            }
        }
    }
}
