using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESFuelManager")]

public class ESFuelManager : MonoBehaviour
{
    public float FuelAmount = 5000f;
    [Range(0, 10)]
    public float Expense = 2f;
    [Tooltip("dont edit, just let it do its thing")]
    public bool Empty = false;
    private Rigidbody VechicleRigidbody;
    private void Start()
    {
        VechicleRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        FuelControl();
    }

    private void FuelControl()
    {
        if (VechicleRigidbody.velocity.magnitude > 0.1f && FuelAmount > 0f)
        {
            FuelAmount -= Expense;
        }

        if (FuelAmount < 2f)
        {
            Empty = true;
        }
        else
        {
            Empty = false;
        }

    }
}
