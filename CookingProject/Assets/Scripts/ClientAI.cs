using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClientAI : MonoBehaviour
{
    public enum ClientInputs { MOVE, IDLE, DIE }
    private EventFSM<ClientInputs> _myFsm;

    private void Awake()
    {
        //Creo los estados
        var idle = new State<ClientInputs>("Idle");
        var moving = new State<ClientInputs>("Moving");
        var die = new State<ClientInputs>("Die");

        //creo las transiciones
        StateConfigurer.Create(idle)
            .SetTransition(ClientInputs.MOVE, moving)
            .SetTransition(ClientInputs.DIE, die)
            .Done(); //aplico y asigno

        StateConfigurer.Create(moving)
            .SetTransition(ClientInputs.IDLE, idle)
            .SetTransition(ClientInputs.DIE, die)
            .Done(); //aplico y asigno

        //Die no va a tener ninguna transición HACIA nada (uno puede morirse, pero no puede pasar de morirse a caminar)
        //entonces solo lo creo e inmediatamente lo aplico asi el diccionario de transiciones no es nulo y no se rompe nada.
        StateConfigurer.Create(die).Done(); //aplico y asigno - De Die no hay otro estado.

        //PARTE 2: SETEO DE LOS ESTADOS
        //IDLE
        idle.OnEnter += x =>
        {
            Debug.Log("Idle");
        };
        idle.OnUpdate += () =>
        {
            //Debug.Log("Idle Update");
            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0) SendInputToFSM(ClientInputs.MOVE);
        };

        //MOVING
        moving.OnEnter += x =>
        {
            Debug.Log("Move");
        };
        moving.OnUpdate += () =>
        {
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                SendInputToFSM(ClientInputs.IDLE);
        };

        /*
            Dado que nuestras transiciones son una clase en si, le agregamos la
            funcionalidad de llamar a una accion al momento de hacerse esa transicion en si
            ¡Esto es aparte del Exit de los estados!
        */

        //En cambio si estamos en "idle" y se le pone el input de ClientInputs.MOVE se ejecutaria esto
        idle.GetTransition(ClientInputs.MOVE).OnTransition += x =>
        {
            Debug.Log("Transition idle to move");
        };

        //En cambio si estamos en "move" y se le pone el input de ClientInputs.IDLE se ejecutaria esto
        moving.GetTransition(ClientInputs.IDLE).OnTransition += x =>
        {
            Debug.Log("Transition moving to idle");
        };

        //con todo ya creado, creo la FSM y le asigno el primer estado
        _myFsm = new EventFSM<ClientInputs>(idle);
    }

    private void Start()
    {

    }

    private void Update()
    {
        _myFsm.Update();
    }

    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
    }

    private void SendInputToFSM(ClientInputs inp)
    {
        _myFsm.SendInput(inp);
    }
}