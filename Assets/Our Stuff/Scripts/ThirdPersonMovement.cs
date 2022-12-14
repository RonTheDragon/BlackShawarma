using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    // Visible //

    [Header("Walking")]
    [Tooltip("The movement speed of the player")]
    [SerializeField] float Speed = 10;

    [Header("Jumping")]
    [Tooltip("The Height of the jumps")]
    [SerializeField] float Jump = 20;
    [Tooltip("The Falling Speed")]
    [SerializeField] float Gravity = 10;

    [Header("Sliding")]
    [Tooltip("on What floor angle we start to slide and cant jump on")]
    [SerializeField] float slopeLimit = 45;
    [Tooltip("The Speed of sliding")]
    [SerializeField] float SlideSpeed = 5;
    [Tooltip("Are we sliding?")]
    [ReadOnly][SerializeField] bool isSliding;
    [Tooltip("The Normal of the floor, (how steep is the floor)")]
    [ReadOnly][SerializeField] Vector3 hitNormal;

    [Header("Ground Check")]
    [Tooltip("the Y position of the Ground Checkbox")]
    [SerializeField] float Y;
    [Tooltip("how wide the Ground Checkbox")]
    [SerializeField] float Wide = 0;
    [Tooltip("how tall the Ground Checkbox")]
    [SerializeField] float Height = .15f;
    [Tooltip("a layer that contains anything we can jump on")]
    [SerializeField] LayerMask Jumpable;
    [Tooltip("Are we On The Ground?")]
    [ReadOnly][SerializeField] bool isGrounded;

    [Header("References")]
    [Tooltip("Place The Player's Camera Here")]
    [SerializeField] Transform cam;


    // Invisible //

    // Auto Referencing
    CharacterController CC => GetComponent<CharacterController>();
    Camera _cam => cam.GetComponent<Camera>();


    // Stored Data
    Vector2 _movement;
    Vector3 _forceDirection;
    float   _gravityPull;
    float   _forceStrength;
    float   f;

    // Ground Check 
    Vector3 _boxPosition => CC.transform.position + (Vector3.up * CC.bounds.extents.y) * Y;
    Vector3 _boxSize     => new Vector3(CC.bounds.extents.x + Wide, Height * 2, CC.bounds.extents.z + Wide);


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // Update is called once per frame
    void Update()
    {
        groundCheck();
        gravitation();
        jumping();
        movement();
        applyingForce();
        slide();
    }

    /// <summary> Allows the player to walk. </summary>
    void movement()
    {
        _movement        = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 Movement = _movement.normalized; //Get input from player for movem

        float targetAngle  = Mathf.Atan2(Movement.x, Movement.y) * Mathf.Rad2Deg + cam.eulerAngles.y; //get where player is looking
        float Angle        = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref f, 0.1f); //Smoothing
        transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation

        if (Movement.magnitude > 0.1f)
        {
            Vector3 MoveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            CC.Move(MoveDir * Speed * Time.deltaTime);
        }
    }

    /// <summary> Allows the player to jump. </summary>
    private void jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            AddForce(Vector3.up, Jump);
        }
    }

    /// <summary> Takes care of Gravity </summary>
    private void gravitation()
    {
        if (isGrounded)
        {
            _gravityPull = .1f;
        }
        else if (_gravityPull < 1)
        {
            _gravityPull += .2f * Time.deltaTime;
        }
        CC.Move(Vector3.down * Gravity * _gravityPull * Time.deltaTime);
    }

    /// <summary> Checking the ground and tell the player if he is grounded or sliding. </summary>
    private void groundCheck()
    {
        isGrounded = Physics.CheckBox(_boxPosition, _boxSize, quaternion.identity, Jumpable);
        if (isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(_boxPosition + Vector3.up * .5f, Vector3.down, out hit, 1, Jumpable))
            {
                hitNormal = hit.normal;
                //Debug.Log(hit.transform.name);
            }
        }
        else
        {
            hitNormal = hitNormal * .99f;
        }
        isSliding = (!(Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit));
    }

    /// <summary> Making the player to slide on steep slopes. </summary>
    private void slide()
    {
        if (isSliding)
        {
            Vector3 slid = Vector3.zero;
            slid.x += ((1f - hitNormal.y) * hitNormal.x * SlideSpeed);
            slid.z += ((1f - hitNormal.y) * hitNormal.z * SlideSpeed);

            CC.Move(slid * Time.deltaTime);
        }
    }


    /// <summary> Adding custom force to the character controller </summary>
    public void AddForce(Vector3 dir, float force)
    {
        _forceDirection = dir;
        _forceStrength  = force;
    }

    /// <summary> Makes the added force move the player Overtime. </summary>
    private void applyingForce()
    {
        if (_forceStrength > 0)
        {
            CC.Move(_forceDirection.normalized * _forceStrength * Time.deltaTime);
            _forceStrength -= _forceStrength * 2 * Time.deltaTime;
        }
    }

    //Gizmos
    void OnDrawGizmosSelected()
    {
        // Draw a Box in the Editor to show whether we are touching the ground, Blue is Touching, Red is Not Touching.
        Gizmos.color = isGrounded ? Color.blue : Color.red; Gizmos.DrawCube(_boxPosition, _boxSize * 2);
    }
}

#region Magic Trick That enables ReadOnly
public class ReadOnlyAttribute : PropertyAttribute
{

}

#endregion