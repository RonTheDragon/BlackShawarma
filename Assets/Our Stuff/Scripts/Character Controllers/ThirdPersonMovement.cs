using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.Rendering.DebugUI.Table;

public class ThirdPersonMovement : MonoBehaviour
{

    [Header("Walking")]
    [Tooltip("The movement speed of the player")]
    [SerializeField] float Speed = 10;

    [Header("Sprint")]
    [SerializeField] private float _sprintSpeed;
    private bool IsSprinting;
    public float MaxStamina;
    private float _currentStamina;
    public float StaminaRegen;
    public float StaminaDrain;
    [SerializeField] private float _staminaRegenCooldown;
    private float _currentStaminaRegenCooldown;
    [HideInInspector] public Action<float> OnStamina;

    [Header("Look")]
    [SerializeField] private Transform _lookAt;
    [SerializeField] private Vector2 _sensitivity;
    private float _currentSensitivity;
    [SerializeField] private Cinemachine.AxisState _xAxis;
    [SerializeField] private Cinemachine.AxisState _yAxis;


    [Header("FreeRoam")]
    public bool FreeRoam = true;

    [SerializeField] private float _freeRoamAfterDuration=2;
    private float _freeRoamAfter;
    [SerializeField] private float _rotateSpeed = 0.1f;

    [SerializeField] private Rig _rig;
    [SerializeField] private float _rigChangeSpeed = 3;

    #region Unused
    //[Header("Jumping")]
    [Tooltip("The Height of the jumps")]
    float Jump = 20;
    [Tooltip("The Falling Speed")]
    float Gravity = 10;
    [Header("Sliding")]
    [Tooltip("on What floor angle we start to slide and cant jump on")]
    float slopeLimit = 45;
    [Tooltip("The Speed of sliding")]
    float SlideSpeed = 5;
    [Tooltip("Are we sliding?")]
    bool isSliding;
    [Tooltip("The Normal of the floor, (how steep is the floor)")]
    Vector3 hitNormal;

    [Header("Ground Check")]
    [Tooltip("the Y position of the Ground Checkbox")]
    float Y;
    [Tooltip("how wide the Ground Checkbox")]
    float Wide = 0;
    [Tooltip("how tall the Ground Checkbox")]
    float Height = .15f;
    [Tooltip("a layer that contains anything we can jump on")]
    LayerMask Jumpable;
    [Tooltip("Are we On The Ground?")]
    bool isGrounded;

    Vector3 _forceDirection;
    float   _gravityPull;
    float   _forceStrength;
    #endregion

    Vector2 _torqueDirection;
    float _torqueStrength;

    [Header("References")]
    [Tooltip("Place The Player's Camera Here")]
    [SerializeField] Transform cam;
    [SerializeField] private Animator _anim;


    // Auto Referencing
    CharacterController CC => GetComponent<CharacterController>();
    //Camera _cam => cam.GetComponent<Camera>();


    // Stored Data
    Vector2 _movement;
    float   _f;


    // Ground Check 
    Vector3 _boxPosition => CC.transform.position + (Vector3.up * CC.bounds.extents.y) * Y;
    Vector3 _boxSize     => new Vector3(CC.bounds.extents.x + Wide, Height * 2, CC.bounds.extents.z + Wide);


    // Start is called before the first frame update
    void Start()
    {
        SetupMouse();
    }

    // Update is called once per frame
    void Update()
    {
        //groundCheck();
        //jumping();
        //slide();
        applyingForce();
        gravitation();
        SprintSystem();
        movement();
        Look();
        FreeRoamSupport();
    }

    private void OnDisable()
    {
        _anim.SetBool("Walk", false);
    }

    /// <summary> Allows the player to walk. </summary>
    void movement()
    {
        _movement        = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 Movement = _movement.normalized; //Get input from player for movem

        float targetAngle  = Mathf.Atan2(Movement.x, Movement.y) * Mathf.Rad2Deg + cam.eulerAngles.y; //get where player is looking
        float Angle        = Mathf.SmoothDampAngle(transform.eulerAngles.y, FreeRoam ? targetAngle : cam.eulerAngles.y, ref _f, FreeRoam ? _rotateSpeed : 0); //Smoothing

        if (!FreeRoam)
        transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation

        if (Movement.magnitude > 0.1f)
        {
            if (FreeRoam)
            {
                transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation
            }
            Vector3 MoveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            CC.Move(MoveDir * GetSpeed() * Time.deltaTime);

            _anim.SetBool("Walk", true);
            _anim.SetFloat("MoveSpeed", GetSpeed()/Speed);
        }
        else
        {
            _anim.SetBool("Walk", false);
        }
    }

    private void SprintSystem()
    {
        if (_currentStaminaRegenCooldown > 0)
        {
            _currentStaminaRegenCooldown -= Time.deltaTime;
        }
        else
        {
            if (_currentStamina < MaxStamina)
            {
                _currentStamina += StaminaRegen * Time.deltaTime;
            }
            else if (_currentStamina > MaxStamina)
            {
                _currentStamina = MaxStamina;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_currentStamina > 0 && _movement.magnitude>0)
            {
            _currentStaminaRegenCooldown = _staminaRegenCooldown;
                IsSprinting = true;
                _currentStamina -= StaminaDrain * Time.deltaTime;
            }
            else
            {
                IsSprinting = false;
            }
        }
        else
        {
            IsSprinting = false;
        }
        OnStamina(_currentStamina/MaxStamina);
    }

    private float GetSpeed()
    {
        if (IsSprinting) 
            return _sprintSpeed;
        return Speed;
    }

    private void FreeRoamSupport()
    {
        if (_freeRoamAfter > 0) _freeRoamAfter -= Time.deltaTime;
        else FreeRoam = true;

        _rig.weight = Mathf.Lerp(_rig.weight, FreeRoam ? 0 : 1, (FreeRoam ? _rigChangeSpeed : 100) * Time.deltaTime);
    }

    public void StopFreeRoaming()
    {
        FreeRoam = false;
        _freeRoamAfter = _freeRoamAfterDuration;
    }
    
    private void SetupMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (!PlayerPrefs.HasKey("Sensitivity")) { PlayerPrefs.SetFloat("Sensitivity", 0.5f); }
        _currentSensitivity = Mathf.Lerp(_sensitivity.x, _sensitivity.y, PlayerPrefs.GetFloat("Sensitivity"));
        _xAxis.m_MaxSpeed *= _currentSensitivity;
        _yAxis.m_MaxSpeed *= _currentSensitivity;
    }

    public void MultiplySensitivity(float mult)
    {
        float s = _currentSensitivity * mult;
        _xAxis.m_MaxSpeed *= s;
        _yAxis.m_MaxSpeed *= s;
    }

    public void DivideSensitivity(float mult)
    {
        float s = _currentSensitivity * mult;
        _xAxis.m_MaxSpeed /= s;
        _yAxis.m_MaxSpeed /= s;
    }

    private void Look()
    {
        _xAxis.Update(Time.fixedDeltaTime);
        _yAxis.Update(Time.fixedDeltaTime);

        if (_torqueStrength > 0)
        {
            Vector2 rot = _torqueDirection.normalized * _torqueStrength * Time.deltaTime;

            _xAxis.Value += rot.x;
            _yAxis.Value += rot.y;

            _torqueStrength -= (_torqueStrength + 0.01f) * 15 * Time.deltaTime;
        }

        _lookAt.eulerAngles = new Vector3(_yAxis.Value, _xAxis.Value, 0);
    }

    public void AddLookTorque(Vector2 dir, float force)
    {
        _torqueDirection = dir;
        _torqueStrength = force;       
    }

    public void FullStamina()
    {
        _currentStamina = MaxStamina;
    }

    //Gizmos
    //void OnDrawGizmosSelected()
    //{
    //    // Draw a Box in the Editor to show whether we are touching the ground, Blue is Touching, Red is Not Touching.
    //   // Gizmos.color = isGrounded ? Color.blue : Color.red; Gizmos.DrawCube(_boxPosition, _boxSize * 2);
    //}

    #region Unused

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
        //if (isGrounded)
        //{
        //    _gravityPull = .1f;
        //}
        //else if (_gravityPull < 1)
        //{
        //    _gravityPull += .2f * Time.deltaTime;
        //}
        //CC.Move(Vector3.down * Gravity * _gravityPull * Time.deltaTime);
        CC.Move(Vector3.down * 0.1f * Time.deltaTime);
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

         //   CC.Move(slid * Time.deltaTime);
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
            _forceStrength -= (_forceStrength+0.01f) * 2 * Time.deltaTime;
        }
    }
    #endregion

}

#region Magic Trick That enables ReadOnly
public class ReadOnlyAttribute : PropertyAttribute
{

}

#endregion