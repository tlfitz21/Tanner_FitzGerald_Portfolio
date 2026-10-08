using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
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
        text.text = gameManager.getDisplaytime();
    }

}
