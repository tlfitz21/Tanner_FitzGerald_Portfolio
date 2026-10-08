using System.Collections;
using UnityEngine;

public class DogIntro : MonoBehaviour
{
    [SerializeField] Transform door;

    Vector2 streetHome;

    void Awake()
    {
        PlayerControllerDog dog = FindAnyObjectByType<PlayerControllerDog>();
        if (dog == null || door == null)
            return;

        streetHome = dog.BodyPosition;
        Vector2 exit = door.position;
        Rigidbody2D body = dog.GetComponent<Rigidbody2D>();
        dog.FaceDirection(streetHome.x - exit.x);

        if (body != null)
            body.position = exit;
        dog.transform.position = exit;
        dog.BeginScripted(exit);
    }

    void Start()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        PlayerControllerDog dog = FindAnyObjectByType<PlayerControllerDog>();
        if (dog == null || door == null)
            yield break;

        Vector2 home = streetHome;
        Vector2 exit = door.position;
        yield return new WaitForSeconds(0.45f);
        dog.SetScriptedMoving(true);

        Vector2[] path =
        {
            exit,
            new Vector2(home.x, exit.y),
            home
        };

        for (int i = 1; i < path.Length; i++)
        {
            Vector2 from = path[i - 1];
            Vector2 to = path[i];
            float distance = Vector2.Distance(from, to);
            float walked = 0f;
            while (walked < distance)
            {
                walked = Mathf.Min(distance, walked + 3.1f * Time.deltaTime);
                float along = distance <= 0.001f ? 1f : walked / distance;
                float hop = from.y > 1.1f && to.y < 1.1f ? Mathf.Sin(along * Mathf.PI) * 0.4f : 0f;
                dog.SetScriptedPose(Vector2.Lerp(from, to, along), hop);
                yield return null;
            }
        }

        dog.SetScriptedPose(home, 0f);
        dog.SetScriptedMoving(false);
        yield return new WaitForFixedUpdate();
        dog.EndScripted();
    }
}
