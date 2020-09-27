using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    public GameManager gameManager;

    public delegate void RickRoll();
    public static event RickRoll OnRickRoll;

    private void OnTriggerEnter(Collider other)
    {
        if (SceneManager.GetActiveScene().name == "Level03")
            OnRickRoll?.Invoke();
        gameManager.CompleteLevel();
    }
}
