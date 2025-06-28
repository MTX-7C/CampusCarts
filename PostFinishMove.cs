using UnityEngine;
using UnityEngine.Splines;

public class PostFinishMove : MonoBehaviour
{
    SplineContainer splineContainer;
    bool finished = false;
    public const float Speed = 50;

    public void ContinueAfterFinish()
    {
        finished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
        {
            var localPoint = splineContainer.transform.InverseTransformPoint(transform.position);

            SplineUtility.GetNearestPoint(splineContainer.Spline, localPoint, out var nearest, out var ratio, 10, 10);
            var tangent = SplineUtility.EvaluateTangent(splineContainer.Spline, ratio);
            var rotation = Quaternion.LookRotation(tangent);
            transform.rotation = rotation;

            var globalNearest = splineContainer.transform.TransformPoint(nearest);
            var perpendicular = Vector3.Cross(tangent, Vector3.up);
            var position = globalNearest + perpendicular.normalized;
            transform.position = new Vector3(position.x, transform.position.y, position.z);
            transform.Translate(Vector3.forward * Speed * Time.deltaTime, Space.Self);
        }
    }

    private void Start()
    {
        splineContainer = GameObject.Find("Spline").GetComponent<SplineContainer>();
    }
}
