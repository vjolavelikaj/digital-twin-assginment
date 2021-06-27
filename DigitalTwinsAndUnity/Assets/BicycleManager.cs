using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BicycleManager : MonoBehaviour
{
    public Transform exitA;
    public Transform exitB;
    public Transform exitC;
    public Transform exitD;

    public Transform entryA;
    public Transform entryB;
    public Transform entryC;
    public Transform entryD;

    public NavMeshAgent prefab;

    private NavMeshAgent agentA;
    private NavMeshAgent agentB;
    private NavMeshAgent agentC;
    private NavMeshAgent agentD;

    // Start is called before the first frame update
    void Start()
    {
        InitialBicycleInstatiation();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agentA){
            agentA = Instantiate(prefab, entryA.position, prefab.transform.rotation);
            SetPosition("A");
        }
        if (!agentB){
            agentB = Instantiate(prefab, entryB.position, prefab.transform.rotation);
            SetPosition("B");
        }
        if (!agentC){
            agentC = Instantiate(prefab, entryC.position, prefab.transform.rotation);
            SetPosition("C");
        }
        if (!agentD){
            agentD = Instantiate(prefab, entryD.position, prefab.transform.rotation);
            SetPosition("D");
        }
    }

    void InitialBicycleInstatiation (){
        agentA = Instantiate(prefab, entryA.position, prefab.transform.rotation);
        agentB = Instantiate(prefab, entryB.position, prefab.transform.rotation);
        agentC = Instantiate(prefab, entryC.position, prefab.transform.rotation);
        agentD = Instantiate(prefab, entryD.position, prefab.transform.rotation);

        agentA.destination = exitB.position;
        agentB.destination = exitC.position;
        agentC.destination = exitD.position;
        agentD.destination = exitA.position;
    }

     void SetPosition(string position){
        if(position == "A"){
            agentA.destination = exitB.position;
        } else if(position == "B"){
            agentB.destination = exitB.position;
        } else if(position == "C"){
            agentC.destination = exitB.position;
        } else if(position == "D"){
            agentD.destination = exitB.position;
        }
    }
}
