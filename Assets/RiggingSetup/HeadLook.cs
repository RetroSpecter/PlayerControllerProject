using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeadLook : MonoBehaviour
{

    public float lookRadius;
    public Collider[] lookableObjects;
    public LayerMask lookable;
    public Transform lookTarget;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = lookTarget.localPosition;
    }

    private void LateUpdate()
    {
        lookableObjects = Physics.OverlapSphere(transform.position, lookRadius, lookable);
        lookableObjects = lookableObjects.OrderBy((d) => (d.transform.position - transform.position).sqrMagnitude).ToArray();


        if (lookableObjects.Length > 0) {
            lookTarget.position = Vector3.Lerp(lookTarget.position, lookableObjects[0].transform.position, Time.deltaTime * 2);
        } else {
            lookTarget.localPosition = Vector3.Lerp(lookTarget.localPosition, initialPosition, Time.deltaTime * 2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
