using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private int[] sides;
    [SerializeField] private float maxRandomForceValue, startRollingForce;

    private float forceX, forceY, forceZ, forceForward;
    Rigidbody body;

    public int diceFaceNum;

    public void RollDice()
    {
        body.isKinematic = false;

        // Случайные значения для торкса
        forceX = Random.Range(-maxRandomForceValue, maxRandomForceValue);
        forceY = Random.Range(-maxRandomForceValue, maxRandomForceValue);
        forceZ = Random.Range(-maxRandomForceValue, maxRandomForceValue);

        // Случайная сила вперед с большим диапазоном
        forceForward = Random.Range(startRollingForce/2, startRollingForce);

        // Добавляем силу в случайном направлении
        body.AddForce(Vector3.forward * forceForward, ForceMode.Impulse);

        // Добавляем торксы
        body.AddTorque(forceX, forceY, forceZ, ForceMode.Impulse);
    }


    private void Initialize()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        transform.rotation = new Quaternion(0f,0f, 0f,0);
    }

}
