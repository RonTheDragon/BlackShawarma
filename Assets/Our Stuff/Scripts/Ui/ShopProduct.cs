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

    [SerializeField] private float _speed = 5;
    [SerializeField] private float _speedByDistance = 5;
    [SerializeField] private Vector3 _rotating = new Vector3();
    [SerializeField] private float _rotateResetSpeed = 360;

    private void Start()
    {
        _startPos = transform.position;
        _destination = transform.position;
        _initialRotation = _objectHolder.rotation;
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
}
