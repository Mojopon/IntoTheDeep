using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using System;

public class CharacterController : MonoBehaviour
{
    public ReactiveProperty<bool> IsMoving = new ReactiveProperty<bool>();

    private Queue<ICharacterMotion> motionQueue = new Queue<ICharacterMotion>();

    public IObservable<Unit> Move(Vector3 destination)
    {
        motionQueue.Enqueue(new MoveMotion(transform, destination));
        return SequenceStartMotion().ToObservable();
    }

    public IObservable<Unit> KnockBack(Vector3 destination)
    {
        motionQueue.Enqueue(new KnockbackMotion(transform, destination));
        return SequenceStartMotion().ToObservable();
    }

    IEnumerator SequenceStartMotion()
    {
        if (IsMoving.Value) yield break;

        IsMoving.Value = true;

        while (motionQueue.Count > 0)
        {
            yield return StartCoroutine(motionQueue.Dequeue().SequenceMotion());
        }

        IsMoving.Value = false;
    }


    private interface ICharacterMotion
    {
        IEnumerator SequenceMotion();
    }

    private class MoveMotion : ICharacterMotion
    {
        private Transform target;
        private Vector3 destination;
        public MoveMotion(Transform target, Vector3 destination)
        {
            this.target = target;
            this.destination = destination;
        }

        public IEnumerator SequenceMotion()
        {
            return SequenceMoveTransform(destination);
        }

        IEnumerator SequenceMoveTransform(Vector3 destination)
        {
            float speed = 1 / GlobalSettings.Instance.CharacterMoveSpeed;

            float progress = 0;
            while (Vector3.Distance(target.position, destination) > Mathf.Epsilon)
            {
                progress += speed * Time.deltaTime;
                target.position = Vector3.Lerp(target.position, destination, progress);
                yield return null;
            }

            yield break;
        }
    }

    private class KnockbackMotion : ICharacterMotion
    {
        private Transform target;
        private Vector3 destination;
        public KnockbackMotion(Transform target, Vector3 destination)
        {
            this.target = target;
            this.destination = destination;
        }

        public IEnumerator SequenceMotion()
        {
            return SequenceKnockback(destination);
        }

        IEnumerator SequenceKnockback(Vector3 destination)
        {
            float speed = 1 / GlobalSettings.Instance.CharacterMoveSpeed;

            float progress = 0;

            var initialPosition = target.position;

            while (progress < 1)
            {
                progress += speed * Time.deltaTime;
                target.position = Vector3.Lerp(initialPosition, destination, Mathf.PingPong(progress * 2, 1));
                yield return null;
            }
            target.position = initialPosition;

            yield break;
        }
    }
}
