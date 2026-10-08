using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    private GameWideManager gameManager;

    private TextMeshProUGUI text;

    void Awake()
    {
        gameManager = GameWideManager.myGameManager;
        text = GetComponent<TextMeshProUGUI>();

    }

    void Update()
    {
        if (gameManager == null)
            gameManager = GameWideManager.EnsureExists();
        if (text == null || gameManager == null)
            return;
        text.text = "Score: " + gameManager.getScore();
    }
}
