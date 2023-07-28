using UnityEngine;

public class ShopProduct : MonoBehaviour
{
    public SOUpgrade TheUpgradeSO;

    private bool IsHovered;
    private GameManager _gm;
    private Shop _theShop;
    private Transform _objectHolder => transform.GetChild(0);
    private Vector3 _startPos;
    private Vector3 _destination;
    private Quaternion _initialRotation;
    private bool _onShelf;
    private bool _growing;
    private Vector3 initialSize;

    [Header("Speed")]
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _speedByDistance = 5;

    [Header("Rotation")]
    [SerializeField] private Vector3 _rotating = new Vector3();
    [SerializeField] private float _rotateResetSpeed = 360;

    [Header("Size")]
    [SerializeField] private Vector3 _growToSize = new Vector3(1.5f, 1.5f, 1.5f);
    [SerializeField] private Vector3 _shrinkToSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private float _growingSpeed = 1;
    [SerializeField] private float _shrinkingSpeed = 1;

    private void Start()
    {
        _startPos = transform.position;
        _destination = transform.position;
        _initialRotation = _objectHolder.rotation;
        initialSize = _objectHolder.localScale;
        StartCoroutine(LateStart());
    }

    private System.Collections.IEnumerator LateStart()
    {
        yield return null; // Wait for one frame
        _gm = GameManager.Instance;
        _theShop = _gm.TheShop;

        // Continue with the rest of your initialization logic
        // ...
    }

    private void Update()
    {
        MoveToDestination();
        SetSize();
    }

    private void MoveToDestination()
    {
        float dist = Vector3.Distance(transform.position, _destination);
        if (dist > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination,
                (_speed + (dist * _speedByDistance)) * Time.deltaTime);
        }
        
        if (dist < 0.01f && _startPos == _destination)
        {
            _objectHolder.rotation = Quaternion.Lerp(_objectHolder.rotation, _initialRotation, _rotateResetSpeed * Time.deltaTime);
            _onShelf = true;
            if (IsHovered) { _theShop.SetHomelessPointing(transform.position); }
        }
        else
        {
            _objectHolder.Rotate(_rotating * Time.deltaTime); //Makes it spin
            _onShelf = false;
            if (IsHovered) { _theShop.SetHomelessStopPointing(); }
        }
    }

    private void SetSize()
    {
        if (IsHovered)
        {
            if (_growing)
            {
                if (_objectHolder.localScale.magnitude < _growToSize.magnitude)
                {
                    _objectHolder.localScale += _growToSize * _growingSpeed * Time.deltaTime;
                }
                else
                {
                    _growing = false;
                }
            }
            else
            {
                if (_objectHolder.localScale.magnitude > _shrinkToSize.magnitude)
                {
                    _objectHolder.localScale -= _shrinkToSize * _shrinkingSpeed * Time.deltaTime;
                }
                else
                {
                    _growing = true;
                }
            }
        }
        else if(_objectHolder.localScale != initialSize )
        {
            _objectHolder.localScale = initialSize;      
        }
    }





    public void OnClick() 
    {
        _theShop.ShowUpgrade(this);
    }

    public void OnHoverEnter() 
    { 
        if (_onShelf)
        {
            _theShop.SetHomelessPointing(transform.position);
        }
        else
        {
            _theShop.SetHomelessStopPointing();
        }
        IsHovered = true;
    }

    public void OnHoverExit() 
    {
        _theShop.SetHomelessStopPointing();
        IsHovered = false;
        _objectHolder.transform.localScale = initialSize;
    }

    public bool GetIsHovered()
    {
        return IsHovered;
    }

    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }

    public void SetDestinationBack()
    {
        _destination = _startPos;
    }

    public void TeleportBack()
    {
        SetDestinationBack();
        transform.position = _startPos;
        _objectHolder.rotation = _initialRotation;
    }
}
