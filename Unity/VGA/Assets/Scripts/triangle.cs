using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class triangle : MonoBehaviour
{
    GameObject targetToLocAt;
/*    private void FixedUpdate()
    {

    }*/
    public void updateRotation() {
        if (targetToLocAt == null)
            return;

        Vector3 relative = transform.InverseTransformPoint(targetToLocAt.transform.position);
        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        transform.Rotate(0, 0, -angle);

    }
    public void setTarget(GameObject g) { targetToLocAt = g; }
}
