using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private cutsceneManagerSheep cutsceneManager;

    // Update is called once per frame
    void Update()
    {
        if (cutsceneManager.inCutscene) return;
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    }
}
