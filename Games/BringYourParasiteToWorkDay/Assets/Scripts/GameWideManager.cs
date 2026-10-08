using UnityEngine;

public class GameWideManager : MonoBehaviour
{
    public static GameWideManager myGameManager { get; private set; }

    [SerializeField]private float secondsRemaining;
    private bool paused;

    private int score;

    public bool hasStarted;

    public int madFarmers;

    public float getSeconds()
    {
        return secondsRemaining;
    }

    void Awake()
    {
        if(myGameManager != null && myGameManager != this)
        {
            Destroy(gameObject);
            return;
        } 
        myGameManager = this;
        DontDestroyOnLoad(gameObject);
        hasStarted = false;
        paused = false;
        score = 0;
        madFarmers = 0;
    }

    void FixedUpdate()
    {
        if (!paused)
        {
            secondsRemaining = Mathf.Max(0f, secondsRemaining - Time.fixedDeltaTime);
        }
    }

    public string getDisplaytime()
    {
    int totalSeconds = Mathf.CeilToInt(secondsRemaining);
    return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }

    public string getScore()
    {
        return score.ToString();
    }

    public void modScore(int mod)
    {
        score += mod;
    }

    public static GameWideManager EnsureExists()
    {
        if (myGameManager != null)
            return myGameManager;

        GameObject go = new GameObject("GameWideManager");
        GameWideManager manager = go.AddComponent<GameWideManager>();
        manager.secondsRemaining = 1440f;
        return manager;
    }
}
