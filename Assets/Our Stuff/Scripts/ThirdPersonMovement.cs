using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;
//using UnityEngine.UIElements;

public class ThirdPersonMovement : MonoBehaviour
{
    public float               Speed   = 10;
    public float               Gravity = 10;
    public CharacterController CC;
    public Transform           cam;

    float f;

    Vector3 ForceDirection;
    float   ForceStrength;

    public float Jump = 20;
    public float gravityPull;

    public bool Grounded;

    Vector3 hitNormal;

    public float slopeLimit = 80;

    public float slideFriction;

    private void Awake()
    {
        CC = GetComponent<CharacterController>();   
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Grounded)
        {
            gravityPull = 0.1f;
        }
        else if (gravityPull < 1)
        {
            gravityPull += 0.2f * Time.deltaTime;
        }
        CC.Move(Vector3.down * Gravity * gravityPull * Time.deltaTime);

        bool G = !CC.isGrounded;
        
        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            AddForce(Vector3.up, Jump);
        }

        Vector2 Movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Get input from player for movem
        
            float targetAngle     = Mathf.Atan2(Movement.x, Movement.y) * Mathf.Rad2Deg + cam.eulerAngles.y; //get where player is looking
            float Angle             = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref f, 0.1f); //Smoothing
            transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation

            if (Movement.magnitude > 0.1f)
            {
                Vector3 MoveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                CC.Move(MoveDir.normalized * Speed* Time.deltaTime);
            }
        

        if (ForceStrength > 0)
        {
            CC.Move(ForceDirection.normalized * ForceStrength * Time.deltaTime);
            ForceStrength -= ForceStrength* 2 * Time.deltaTime;
        }
        Grounded = ((Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit) && !G);

        
        if (!Grounded && !G)
        {
            Vector3 slid = Vector3.zero;
            slid.x += ((1f - hitNormal.y) * hitNormal.x * (1f - slideFriction));
            slid.z += ((1f - hitNormal.y) * hitNormal.z * (1f - slideFriction));

            CC.Move(slid);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

    void AddForce(Vector3 dir, float force)
    {
        ForceDirection = dir;
        ForceStrength  = force;
    }
}
