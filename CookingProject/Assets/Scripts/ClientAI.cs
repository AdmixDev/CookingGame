using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClientAI : MonoBehaviour
{
    public enum ClientInputs {FIND_TABLE, ORDERING, EATING, GO_OUT }
    private EventFSM<ClientInputs> _myFsm;
    private NavMeshAgent _agent;
    private State<ClientInputs> _initState;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        InitStateMachine();
    }

    private void Start()
    {
        //con todo ya creado, creo la FSM y le asigno el primer estado
        _myFsm = new EventFSM<ClientInputs>(_initState);
    }

    private void InitStateMachine()
    {
        //Creo los estados
        var findTable = new State<ClientInputs>("FindTable");
        var ordering = new State<ClientInputs>("Ordering");
        var eating = new State<ClientInputs>("Eating");
        var goOut = new State<ClientInputs>("GoOut");

        //creo las transiciones
        StateConfigurer.Create(findTable)
            .SetTransition(ClientInputs.ORDERING, ordering)
            .Done(); //aplico y asigno

        StateConfigurer.Create(ordering)
            .SetTransition(ClientInputs.EATING, eating)
            .SetTransition(ClientInputs.GO_OUT, goOut)
            .Done(); //aplico y asigno

        //Die no va a tener ninguna transición HACIA nada (uno puede morirse, pero no puede pasar de morirse a caminar)
        //entonces solo lo creo e inmediatamente lo aplico asi el diccionario de transiciones no es nulo y no se rompe nada.
        StateConfigurer.Create(goOut).Done(); //aplico y asigno - De Die no hay otro estado.

        //PARTE 2: SETEO DE LOS ESTADOS

        //findTable
        findTable.OnEnter += x =>
        {
            Table table = DeliveryManager.Instance.GetTable();
            if (table)
            {
                Debug.Log("Table received");
            }
        };
        findTable.OnUpdate += () =>
        {

        };

        ordering.OnEnter += x =>
        {

        };
        ordering.OnUpdate += () =>
        {

        };

        eating.OnEnter += x =>
        {

        };
        eating.OnUpdate += () =>
        {

        };

        goOut.OnEnter += x => 
        { 
        
        };
        goOut.OnUpdate += () =>
        {

        };

        /*
            Dado que nuestras transiciones son una clase en si, le agregamos la
            funcionalidad de llamar a una accion al momento de hacerse esa transicion en si
            ¡Esto es aparte del Exit de los estados!
        */

        //En cambio si estamos en "findTable" y se le pone el input de ClientInputs.ORDERING se ejecutaria esto
        findTable.GetTransition(ClientInputs.ORDERING).OnTransition += x =>
        {
            Debug.Log("Transition findTable to ordering");
        };

        //En cambio si estamos en "ordering" y se le pone el input de ClientInputs.EATING se ejecutaria esto
        ordering.GetTransition(ClientInputs.EATING).OnTransition += x =>
        {
            Debug.Log("Transition moving to idle");
        };
        //En cambio si estamos en "ordering" y se le pone el input de ClientInputs.GO_OUT se ejecutaria esto
        ordering.GetTransition(ClientInputs.GO_OUT).OnTransition += x =>
        {
            Debug.Log("Paso el tiempo de espera");
        };

        _initState = findTable;
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

    private void GoTo(Vector3 destiny)
    {
        _agent.SetDestination(destiny);
    }
}