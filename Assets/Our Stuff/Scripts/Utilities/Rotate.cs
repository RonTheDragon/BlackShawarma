using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 Rotating = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Rotating * Time.deltaTime); //Makes it spin
    }
}
