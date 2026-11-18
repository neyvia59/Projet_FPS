
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ThrowPreview : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AOEPreview aoePreview;
    [SerializeField]private LineRenderer line;

  

    [Header("Trajectory")]
    [SerializeField] private int resolution = 40;
    [SerializeField] private float projectileRadius = 0.15f;
    [SerializeField] private LayerMask collisionMask = ~0;




    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        Hide();
    }

    public void ShowTrajectory(
        Vector3 start,
        Vector3 velocity,
        float timer,
        float aoeRadius
        )
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 previousPoint = start;
        points.Add(start);

        Vector3 explosionPosition = start;

        bool collisionDetected = false;

        for (int i = 1; i <= resolution; i++)
        {
            float t = timer * i / resolution;

            Vector3 currentPoint =
                start +
                velocity * t +
                0.5f * Physics.gravity * t * t;

            Vector3 direction = currentPoint - previousPoint;
            float distance = direction.magnitude;

            if (distance > 0f &&
                Physics.SphereCast(
                    previousPoint,
                    projectileRadius,
                    direction.normalized,
                    out RaycastHit hit,
                    distance,
                    collisionMask,
                    QueryTriggerInteraction.Ignore))
            {
                explosionPosition =
                    previousPoint + direction.normalized * hit.distance;



                points.Add(explosionPosition);
                collisionDetected = true;
                break;
            }



            points.Add(currentPoint);
            previousPoint = currentPoint;
            explosionPosition = currentPoint;
        }

        points[0] = start;

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());


        line.enabled = true;

        if (aoePreview != null)
        {
            aoePreview.Show(explosionPosition, aoeRadius);
        }
    }

    public void Hide()
    {
        if (line != null)
            line.enabled = false;

        if (aoePreview != null)
            aoePreview.Hide();
    }
}
