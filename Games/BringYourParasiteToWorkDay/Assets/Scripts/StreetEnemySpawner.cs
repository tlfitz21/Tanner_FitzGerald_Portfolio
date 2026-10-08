using UnityEngine;

public class StreetEnemySpawner : MonoBehaviour
{
    [SerializeField] StreetEnemy template;
    [SerializeField] Vector2[] extraPositions =
    {
        new Vector2(-1f, -2.3f),
        new Vector2(4f, -0.4f),
        new Vector2(8f, -2.1f),
        new Vector2(12f, -0.8f),
        new Vector2(46f, -1.2f),
        new Vector2(53f, 0.4f),
        new Vector2(60f, -2.4f),
        new Vector2(67f, -0.6f),
        new Vector2(74f, 0.5f),
        new Vector2(81f, -2f),
        new Vector2(88f, -0.8f),
        new Vector2(94f, -2.2f)
    };

    static readonly Color[] colors =
    {
        new Color(0.18f, 0.48f, 0.28f),
        new Color(0.62f, 0.18f, 0.22f),
        new Color(0.45f, 0.4f, 0.15f),
        new Color(0.2f, 0.28f, 0.62f),
        new Color(0.55f, 0.28f, 0.12f),
        new Color(0.35f, 0.16f, 0.42f),
        new Color(0.15f, 0.42f, 0.42f)
    };

    [SerializeField] int openingExtras = 4;

    void Awake()
    {
        if (template == null)
            return;

        StreetLevel level = GetComponent<StreetLevel>();
        template.SetGateGuard();
        if (level != null)
            level.AddGuard(template);

        for (int i = 0; i < extraPositions.Length; i++)
        {
            StreetEnemy copy = Instantiate(template, extraPositions[i], Quaternion.identity);
            copy.name = "StreetEnemy " + (i + 2);
            copy.SetColor(colors[i % colors.Length]);
            if (i < openingExtras)
            {
                copy.SetGateGuard();
                if (level != null)
                    level.AddGuard(copy);
            }
            else
                copy.WaitForHulk();
        }
    }
}
