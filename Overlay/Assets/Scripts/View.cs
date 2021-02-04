﻿using CommandWheelOverlay.Connection;
using CommandWheelOverlay.Controller;
using CommandWheelOverlay.Settings;
using CommandWheelOverlay.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour, IOverlayView
{
    public CursorMovement cursorMovement;
    private IOverlayController controller;

    private void Start()
    {
        controller = new TcpOverlayController(this, 7777);
#if !UNITY_EDITOR
        controller.Connect();
#endif
    }

    public void Hide()
    {
        throw new System.NotImplementedException();
    }

    public void MoveLeft()
    {
        throw new System.NotImplementedException();
    }

    public void MoveRight()
    {
        throw new System.NotImplementedException();
    }

    public void SendMouseMovement(int[] deltas)
    {
        cursorMovement.AddMovement(new Vector2(deltas[0], -deltas[1]));
    }

    public void Show()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateElements(SimplifiedWheelElements elements)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateSettings(IUserSettings settings)
    {
        throw new System.NotImplementedException();
    }
}