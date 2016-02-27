using UnityEngine;
using System.Collections;
using UniRx;
using System;

namespace TestScene
{

    public class CharacterMover : MonoBehaviour
    {
        public CharacterController character;

        private Coord currentCoord;
        private Func<Coord, Vector3> Converter;
        private Direction previousDirection = Direction.Left;
        void Start()
        {
            Converter = new Func<Coord, Vector3>((coord) =>new Vector3(coord.x, coord.y));

            InputManager.Root.MoveDirectionObservable
                             .Where(x => x != Direction.None)
                             .Subscribe(x => MoveCharacter(x));


            InputManager.Root.OnEnterButtonObservable
                             .Where(x => x)
                             .Subscribe(x => KnockbackCharacter());

        }

        void MoveCharacter(Direction direction)
        {
            previousDirection = direction;
            currentCoord += direction.ToCoord();

            character.Move(Converter(currentCoord)).StartAsCoroutine();
        }

        void KnockbackCharacter()
        {
            var knockbackDirection = previousDirection.GetOpposide().ToCoord();
            var knockbackMagnitude = new Vector3(knockbackDirection.x, knockbackDirection.y) / 2;
            var currentPosition = new Vector3(currentCoord.x, currentCoord.y);
            character.KnockBack(currentPosition + knockbackMagnitude).StartAsCoroutine();
        }
    }
}
