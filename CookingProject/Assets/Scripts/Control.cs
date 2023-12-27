using UnityEngine;
using System;

public class Control
{
    public Action ArtificialUpdate;

    float _horizontal;
    float _vertical;

    Entity _entity;

    public Control(Entity e) 
    {
        _entity = e;
        ChangeControls(Controls.Normal);
    }

    public void InputUpdate()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
    }

    public void ChangeControls(Controls controls)
    {
        switch (controls)
        {
            case Controls.Normal:
                ArtificialUpdate = NormalControls;
                break;
            case Controls.Paused:
                ArtificialUpdate = PausedControls;
                break;
            default:
                break;
        }
    }

    public void OnUpdate()
    {
        ArtificialUpdate?.Invoke();
    }

    void NormalControls()
    {
        InputUpdate();

        _entity.Move(_horizontal, _vertical);

        if (Input.GetKeyDown(KeyCode.LeftControl)) _entity.Crouch();
        else if (Input.GetKeyUp(KeyCode.LeftControl)) _entity.Crouch();

        if (Input.GetKeyDown(KeyCode.LeftShift)) _entity.Run();
        else if(Input.GetKeyUp(KeyCode.LeftShift)) _entity.Run();

        if (Input.GetKeyDown(KeyCode.Space)) _entity.MainAction();
        if (Input.GetKeyDown(KeyCode.F)) _entity.AlternativeAction();

        if (Input.GetKeyDown(KeyCode.Escape)) ChangeControls(Controls.Paused);
    }

    void PausedControls()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeControls(Controls.Normal);
        }
    }
}

public enum Controls
{
    Normal,
    Paused
}
