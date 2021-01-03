using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCollision : MonoBehaviour
{
    public LayerMask groundLayer;
    public Transform parent, topCog;
    private float initialFootPosition;

    private void Start()
    {
        if (parent != null && topCog != null)
            initialFootPosition = topCog.position.y - parent.position.y;
    }

    private void LateUpdate()
    {
        if (parent != null && topCog != null)
            this.transform.position = parent.transform.position; 
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 3, Vector3.down, out hit, 4, groundLayer)) {
            //transform.position = hit.point + Vector3.up * (initialFootPosition - (topCog.position.y - parent.position.y));
            //transform.rotation = parent.rotation;

            
            if (parent != null && topCog != null)
            {
                transform.position = hit.point + Vector3.up * (initialFootPosition - (topCog.position.y - parent.position.y));
                transform.eulerAngles = parent.eulerAngles;
            }
            else {
                transform.position = hit.point;
                transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
            }
            
        }        
    }
}
