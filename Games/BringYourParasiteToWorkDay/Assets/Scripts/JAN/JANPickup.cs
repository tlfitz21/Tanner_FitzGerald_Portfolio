using UnityEngine;

public class JANPickup : MonoBehaviour
{
    public enum Kind
    {
        Health,
        Armor,
        Ammo
    }

    public Kind kind;
    public float amount = 25f;
    Vector3 origin;

    public static JANPickup Create(Kind pickupKind, Vector3 position, float pickupAmount)
    {
        Color color = pickupKind == Kind.Health
            ? new Color(0.25f, 0.75f, 0.3f)
            : pickupKind == Kind.Armor
                ? new Color(0.25f, 0.45f, 0.9f)
                : new Color(0.3f, 0.85f, 0.9f);
        GameObject box = JANArt.Cube(pickupKind.ToString(), position, Vector3.one * 0.45f, color, null);
        Collider collider = box.GetComponent<Collider>();
        collider.isTrigger = true;
        Rigidbody body = box.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        JANPickup pickup = box.AddComponent<JANPickup>();
        pickup.kind = pickupKind;
        pickup.amount = pickupAmount;
        pickup.origin = position;
        return pickup;
    }

    void Update()
    {
        transform.Rotate(0f, 55f * Time.deltaTime, 0f, Space.World);
        Vector3 bob = origin;
        bob.y += Mathf.Sin(Time.time * 2.4f + origin.x) * 0.1f;
        transform.position = bob;
    }

    public void Collect(JANPlayerController player)
    {
        if (player != null && player.TryPickup(kind, amount))
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Collect(other.GetComponent<JANPlayerController>());
    }
}
