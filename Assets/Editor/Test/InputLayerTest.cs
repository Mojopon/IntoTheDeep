using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class InputLayerTest
{
    InputManager.InputLayer inputLayer;

    [SetUp]
    public void Initialize()
    {
        inputLayer = new InputManager.InputLayer();
    }

    [Test]
    public void DefaultTargetShouldBeRoot()
    {
        Assert.AreEqual(PlayerInputTarget.Root, inputLayer.Target);
    }

    [Test]
    public void ShouldInputToPopupWindow()
    {
        inputLayer.ToggleInput(PlayerInputTarget.PopupWindow);
        Assert.AreEqual(PlayerInputTarget.PopupWindow, inputLayer.Target);

        inputLayer.ToggleInput(PlayerInputTarget.PopupWindow);
        Assert.AreEqual(PlayerInputTarget.Root, inputLayer.Target);
    }

    [Test]
    public void ShouldPrioritizeInputToMenu()
    {
        inputLayer.ToggleInput(PlayerInputTarget.Menu);
        Assert.AreEqual(PlayerInputTarget.Menu, inputLayer.Target);

        inputLayer.ToggleInput(PlayerInputTarget.PopupWindow);
        Assert.AreEqual(PlayerInputTarget.Menu, inputLayer.Target);

        inputLayer.ToggleInput(PlayerInputTarget.Menu);
        Assert.AreEqual(PlayerInputTarget.PopupWindow, inputLayer.Target);

        inputLayer.ToggleInput(PlayerInputTarget.PopupWindow);
        Assert.AreEqual(PlayerInputTarget.Root, inputLayer.Target);
    }
}
