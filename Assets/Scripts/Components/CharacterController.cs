using UnityEngine;
using System.Collections;
using UniRx;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour
{
    public ReactiveProperty<bool> IsMoving = new ReactiveProperty<bool>();

    private Queue<Vector3> moveQueue = new Queue<Vector3>();

    public void Move(Vector3 destination)
    {
        moveQueue.Enqueue(destination);
        if(IsMoving.Value != true)
        {
            StartCoroutine(SequenceMove());
        }
    }

    IEnumerator SequenceMove()
    {
        IsMoving.Value = true;

        while(moveQueue.Count > 0)
        {
            yield return SequenceMoveTransform(moveQueue.Dequeue());
        }

        IsMoving.Value = false;
    }

    // Process Move Object in the Game Scene
    IEnumerator SequenceMoveTransform(Vector3 destination)
    {
        float speed = 1 / GlobalSettings.Instance.CharacterMoveSpeed;

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
