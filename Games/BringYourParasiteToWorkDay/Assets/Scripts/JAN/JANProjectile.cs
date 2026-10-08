using UnityEngine;

public interface IJANDamageable
{
    bool IsDead { get; }
    void TakeDamage(float amount, Vector3 hitPoint, Vector3 knockDirection);
}

public class JANProjectile : MonoBehaviour
{
    Vector3 direction;
    float speed;
    float damage;
    bool fromPlayer;
    float life = 2.4f;
    JANGameManager manager;

    public void Launch(Vector3 travelDirection, float travelSpeed, float hitDamage, bool playerOwned)
    {
        direction = travelDirection.normalized;
        speed = travelSpeed;
        damage = hitDamage;
        fromPlayer = playerOwned;
        manager = JANGameManager.Instance;
    }

    void Update()
    {
        if (manager != null && manager.Ended)
        {
            Destroy(gameObject);
            return;
        }

        float step = speed * Time.deltaTime;
        Vector3 origin = transform.position;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, step + 0.08f, ~0, QueryTriggerInteraction.Ignore))
        {
            IJANDamageable damageable = hit.collider.GetComponentInParent<IJANDamageable>();
            bool hitPlayer = damageable is JANPlayerController;
            bool shouldHurt = damageable != null && !damageable.IsDead && hitPlayer != fromPlayer;
            if (shouldHurt)
                damageable.TakeDamage(damage, hit.point, Vector3.zero);

            SpawnSpark(hit.point);
            Destroy(gameObject);
            return;
        }

        transform.position = origin + direction * step;
        life -= Time.deltaTime;
        if (life <= 0f)
            Destroy(gameObject);
    }

    static void SpawnSpark(Vector3 point)
    {
        GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spark.name = "Spark";
        spark.transform.position = point;
        spark.transform.localScale = Vector3.one * 0.18f;
        Collider collider = spark.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);
        spark.GetComponent<Renderer>().sharedMaterial = JANArt.Flat(new Color(0.8f, 0.95f, 1f));
        Destroy(spark, 0.12f);
    }
}
