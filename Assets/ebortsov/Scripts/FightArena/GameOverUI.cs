using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;

    private void Start()
    {
        Hide();
    }

    public void ShowWin()
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = "YOU WIN";
    }

    public void ShowLose()
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = "YOU LOSE";
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
    }
}