using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class backup : MonoBehaviour
{
    [Header("Projection")]
    [SerializeField]
    [Range(10, 100)]
    private int linepoints = 25;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float timeBetweenPoints = 0.1f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LayerMask layer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //void DrawProjection()
        //{
        //    lineRenderer.enabled = true;
        //    lineRenderer.positionCount = Mathf.CeilToInt(linepoints / timeBetweenPoints) + 1;
        //    Vector3 StartPosition = barrel.position;
        //    Vector3 StartVelocity = 100 * barrel.forward / 1;
        //    int i = 0;
        //    lineRenderer.SetPosition(i, StartPosition);
        //    for (float time = 0; time < linepoints; time += timeBetweenPoints)
        //    {
        //        i++;
        //        Vector3 point = StartPosition + time * StartVelocity;
        //        point.y = StartPosition.y + StartVelocity.y * time + (Physics.gravity.y / 2f * time * time);
        //        lineRenderer.SetPosition(i, point);
        //        Vector3 lastPostion = lineRenderer.GetPosition(i - 1);
        //        if (Physics.Raycast(lastPostion, (point - lastPostion).normalized,
        //            out RaycastHit hit, (point - lastPostion).magnitude, layer))
        //        {
        //            lineRenderer.SetPosition(i, hit.point);
        //            lineRenderer.positionCount = i + 1;
        //            return;
        //        }
        //    }
        //}
    }
}
