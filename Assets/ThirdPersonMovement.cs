using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
    private Rigidbody rigid;
    private Camera cam;

    public Animator anim;
    public GameObject tiltObject, sideWaysTiltObject;

    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    [Space]
    public float maxLean = 40;
    public float leanSmoothTime = 0.035f;
    private float leanSmoothVelocity;
    [Space]
    public float turnSmoothTime = 5f;
    private float turnSmoothVelocity;
    private float smoothVelocity;

    private void Start() {
        cam = Camera.main;
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        float targetAngle = Mathf.Atan2(input.normalized.x, input.normalized.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;

        if (input.magnitude >= 0.1f)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float targetSpeed = Input.GetKey(KeyCode.Mouse0) ? runSpeed : walkSpeed;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward.normalized;
            rigid.MovePosition(transform.position + moveDir * targetSpeed * Time.fixedDeltaTime);
        }

        Debug.DrawRay(transform.position, transform.forward, Color.red);

        Vector3 dir = (Vector3)(Quaternion.Euler(0, targetAngle, 0) * Vector3.forward);
        float horizMovement = Vector3.SignedAngle(transform.forward, dir, Vector3.up) * input.magnitude;
        float leanAngle = Mathf.SmoothDampAngle(anim.transform.localRotation.x, Mathf.Lerp(0, maxLean, Mathf.Abs(horizMovement)) * Mathf.Sign(horizMovement), ref leanSmoothVelocity, leanSmoothTime);
        tiltObject.transform.localRotation = Quaternion.Euler(0, 0, -leanAngle);
        sideWaysTiltObject.transform.localRotation = Quaternion.Euler(0,leanAngle,0);

        float animSpeed = Mathf.SmoothDamp(anim.GetFloat("movementSpeed"), input.magnitude * (Input.GetKey(KeyCode.Mouse0) ? 2.1f : 1), ref smoothVelocity, turnSmoothTime);
        anim.SetFloat("movementSpeed", animSpeed);
    }
}
