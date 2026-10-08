using UnityEngine;
using UnityEngine.Tilemaps;

public class scrollingMap : MonoBehaviour
{
    [SerializeField] private GameManagerTruck gameManager;
    [SerializeField] private TilemapRenderer firstMap;
    [SerializeField] private TilemapRenderer secondMap;
    [SerializeField] private Camera sceneCamera;
    private float sectionSpacing;

    private void Awake()
    {
        if (sceneCamera == null)
            sceneCamera = Camera.main;
    }

    private void Start()
    {
        sectionSpacing = Mathf.Abs(firstMap.transform.position.y - secondMap.transform.position.y);

    }

    private void Update()
    {
        if (gameManager == null || firstMap == null || secondMap == null || sceneCamera == null)
            return;

        Vector3 movement = Vector3.down * gameManager.TruckSpeed * Time.deltaTime;
        firstMap.transform.position += movement;
        secondMap.transform.position += movement;

        float cameraBottom = sceneCamera.transform.position.y - sceneCamera.orthographicSize;
        RecycleLowerMap(firstMap, secondMap, cameraBottom);
        RecycleLowerMap(secondMap, firstMap, cameraBottom);
    }

    private void RecycleLowerMap(TilemapRenderer map, TilemapRenderer otherMap, float cameraBottom)
    {
        Bounds mapBounds = map.bounds;
        if (mapBounds.max.y > cameraBottom)
            return;

        Vector3 position = map.transform.position;
        position.y = otherMap.transform.position.y + sectionSpacing;
        map.transform.position = position;
    }
}
