using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FaceDetector : MonoBehaviour
{
    Dice dice;

    private void Awake()
    {
        dice = FindObjectOfType<Dice>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (dice != null)
        {
            if (dice.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                dice.diceFaceNum = int.Parse(other.name);
            }
        }
    }
}

