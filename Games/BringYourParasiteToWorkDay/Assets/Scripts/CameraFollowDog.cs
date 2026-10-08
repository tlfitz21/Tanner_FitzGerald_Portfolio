using UnityEngine;

public class CameraFollowDog : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothTime = 0.18f;
    [SerializeField] bool lockY = true;

    float lockedY;
    float velocityX;

    void Start()
    {
        lockedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        float x = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocityX, smoothTime);
        float y = lockY ? lockedY : target.position.y;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
