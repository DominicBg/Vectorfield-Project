using UnityEngine;

public class InwardVectorFunction : VectorFunction
{
    [SerializeField] float y = 1;
    [SerializeField] float rotateAmmmount = 90;
    [SerializeField] float distance;

    public override Vector3 GetVector(Vector3 point)
    {
        if (point.x == 0 && point.z == 0)
            return Vector3.zero;

        float d = point.SetY(0).magnitude;
        float t = (d / distance);
        float r = Mathf.Lerp(0, rotateAmmmount, t);

        Vector3 newV = point.SetY(y);
        return GameMath.RotateVectorY(r, newV);
    }
}
