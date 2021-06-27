using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleManager : MonoBehaviour
{
    public Transform exitA;
    public Transform exitB;
    public Transform exitC;
    public Transform exitD;

    public Transform entryA;
    public Transform entryB;
    public Transform entryC;
    public Transform entryD;

    public NavMeshAgent prefabA;
    public NavMeshAgent prefabB;
    public NavMeshAgent prefabC;
    public NavMeshAgent prefabD;

    private NavMeshAgent agentA;
    private NavMeshAgent agentB;
    private NavMeshAgent agentC;
    private NavMeshAgent agentD;

    public static List<int> cycles = new List<int>() { 0, 0, 0, 0}; 

    // Start is called before the first frame update
    void Start()
    {
        InitialVehicleInstatiation();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agentA){
            agentA = Instantiate(prefabA, entryA.position, prefabA.transform.rotation);
            SetPosition("A");
        }
        if (!agentB){
            agentB = Instantiate(prefabB, entryB.position, prefabB.transform.rotation);
            SetPosition("B");
        }
        if (!agentC){
            agentC = Instantiate(prefabC, entryC.position, prefabC.transform.rotation);
            SetPosition("C");
        }
        if (!agentD){
            agentD = Instantiate(prefabD, entryD.position, prefabD.transform.rotation);
            SetPosition("D");
        }
    }

    void InitialVehicleInstatiation (){
        agentA = Instantiate(prefabA, entryA.position, prefabA.transform.rotation);
        agentB = Instantiate(prefabB, entryB.position, prefabB.transform.rotation);
        agentC = Instantiate(prefabC, entryC.position, prefabC.transform.rotation);
        agentD = Instantiate(prefabD, entryD.position, prefabD.transform.rotation);

        agentA.destination = exitB.position;
        agentB.destination = exitC.position;
        agentC.destination = exitD.position;
        agentD.destination = exitA.position;
    }

// entryA -> exitA, exitB, exitC
// entryB -> exitB, exitC, exitD
// entryC -> exitC, exitD, exitA
// entryD -> exitD, exitA, exitB

    void getcycle() {
        if (cycles[cycles.Count - 1] == 3)
            cycles[cycles.Count - 1] = 0;
        else
            cycles[cycles.Count - 1]++;
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