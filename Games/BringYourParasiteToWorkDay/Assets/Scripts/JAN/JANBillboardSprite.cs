using UnityEngine;

// Snaps a quad to face the camera in 8 yaw steps, the way DOOM billboards did.
// Parent this on the visual, not the CharacterController, so the hit capsule stays put.
public class JANBillboardSprite : MonoBehaviour
{
    [SerializeField] int directions = 8;

    bool frozen;

    public void Freeze()
    {
        frozen = true;
    }

    void LateUpdate()
    {
        if (frozen)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 toCamera = cam.transform.position - transform.position;
        toCamera.y = 0f;
        if (toCamera.sqrMagnitude < 0.0001f)
            return;

        float yaw = Mathf.Atan2(toCamera.x, toCamera.z) * Mathf.Rad2Deg;
        float step = 360f / Mathf.Max(1, directions);
        yaw = Mathf.Round(yaw / step) * step;
        transform.rotation = Quaternion.Euler(0f, yaw + 180f, 0f);
    }
}
