using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManagerTruck : MonoBehaviour
{
    public float TruckSpeed;
    public float Distance;
    [SerializeField] public float minSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] private TextMeshProUGUI speedometer;
    [SerializeField] private Image healthBar;
    [SerializeField] public Image distanceBar;
    [SerializeField] public float desitination;
    public int health;

    void Awake()
    {
        TruckSpeed = minSpeed;
        Distance = 0;
        health = 100;
        healthBar.fillAmount = 1;
        healthBar.fillMethod = Image.FillMethod.Horizontal;
        distanceBar.fillAmount = 0;
        distanceBar.fillMethod = Image.FillMethod.Horizontal;
    }

    void Update()
    {
        speedometer.text = "Speed: " + (TruckSpeed + 50).ToString("F0") + " MPH";
    }

    public bool takeDamage(int amount)
    {
        health -= amount;
        healthBar.fillAmount = health / 100f;
        if (health < 0)
        {
            health = 0;   
        }
        return health > 0;
    }
}

