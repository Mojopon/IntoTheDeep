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
        void Start()
        {
            Converter = new Func<Coord, Vector3>((coord) =>new Vector3(coord.x, coord.y));

            InputManager.Root.MoveDirectionObservable
                             .Where(x => x != Direction.None)
                             .Subscribe(x => MoveCharacter(x));
        }

        void MoveCharacter(Direction direction)
        {
            currentCoord += direction.ToCoord();

            character.Move(Converter(currentCoord));
        }
    }
}
