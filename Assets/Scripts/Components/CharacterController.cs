using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    public void Move(Vector3 destination)
    {
        StopCoroutine("SequenceMoveTransform");
        StartCoroutine("SequenceMoveTransform", destination);
    }

    private float timeToFinishMove = 0.1f;
    // Process Move Object in the Game Scene
    IEnumerator SequenceMoveTransform(Vector3 destination)
    {
        float speed = 1 / timeToFinishMove;

        float progress = 0;
        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            progress += speed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, destination, progress);
            yield return null;
        }

        yield break;
    }
}
