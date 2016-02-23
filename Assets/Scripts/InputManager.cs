using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;

public class InputGroup : IPlayerInput
{
    public IObservable<bool> OnEnterButtonObservable { get; set; }
    public IObservable<bool> OnCancelButtonObservable { get; set; }
    public IObservable<Direction> MoveDirectionObservable { get; set; }
}

public class InputManager : MonoBehaviour
{
    public static InputGroup Root { get { return instance.rootInputs; } }
    public static InputGroup Menu { get { return instance.menuInputs; } }
    public static IObservable<bool> OnMenuButtonObservable { get { return instance.toggleMenuInput; } }

    private static InputManager instance;

    public static void ToggleInput(PlayerInputTarget target)
    {
        instance.inputLayer.ToggleInput(target);
    }

    public InputGroup rootInputs = new InputGroup();
    public InputGroup menuInputs = new InputGroup();
    public InputGroup popupInputs = new InputGroup();
    public IObservable<bool> toggleMenuInput;

    public InputLayer inputLayer = new InputLayer();

    void Awake()
    {
        if (instance != null && instance!= this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;

        SetupInputGroup(rootInputs, () => inputLayer.Target == PlayerInputTarget.Root);
        SetupInputGroup(menuInputs, () => inputLayer.Target == PlayerInputTarget.Menu);
        SetupInputGroup(popupInputs, () => inputLayer.Target == PlayerInputTarget.PopupWindow);

        toggleMenuInput = this.UpdateAsObservable()
                              .Select(x => Input.GetKeyDown(KeyCode.Space));
    }

    void SetupInputGroup(InputGroup group, Func<bool> predicate)
    {

        group.OnEnterButtonObservable = this.UpdateAsObservable()
                                            .Where(x => predicate())
                                            .Select(x => Input.GetKeyDown(KeyCode.Z));

        group.OnCancelButtonObservable = this.UpdateAsObservable()
                                             .Where(x => predicate())
                                             .Select(x => Input.GetKeyDown(KeyCode.X));

        group.MoveDirectionObservable = this.UpdateAsObservable()
                                            .Where(x => predicate())
                                            .Select(x =>
                                             {
                                                 var direction = Direction.None;
                                                 if (Input.GetKeyDown(KeyCode.RightArrow))
                                                 {
                                                     direction = Direction.Right;
                                                 }
                                                 else if (Input.GetKeyDown(KeyCode.LeftArrow))
                                                 {
                                                     direction = Direction.Left;
                                                 }
                                                 else if (Input.GetKeyDown(KeyCode.UpArrow))
                                                 {
                                                     direction = Direction.Up;
                                                 }
                                                 else if (Input.GetKeyDown(KeyCode.DownArrow))
                                                 {
                                                     direction = Direction.Down;
                                                 }
                                                 return direction;
                                             });
    }

    public class InputLayer
    {
        private bool inputToRoot = false;
        private bool inputToMenu = false;
        private bool inputToPopup = false;

        public PlayerInputTarget Target { get; private set; }

        public InputLayer()
        {
            ToggleInput(PlayerInputTarget.Root);
        }

        public void ToggleInput(PlayerInputTarget target)
        {
            switch (target)
            {
                case PlayerInputTarget.Root:
                    inputToRoot = !inputToRoot;
                    break;
                case PlayerInputTarget.PopupWindow:
                    inputToPopup = !inputToPopup;
                    break;
                case PlayerInputTarget.Menu:
                    inputToMenu = !inputToMenu;
                    break;
            }


            if (inputToMenu)
            {
                this.Target = PlayerInputTarget.Menu;
                return;
            }

            if(inputToPopup)
            {
                this.Target = PlayerInputTarget.PopupWindow;
                return;
            }

            this.Target = PlayerInputTarget.Root;
        }
    }
}
