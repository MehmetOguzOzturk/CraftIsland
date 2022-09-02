using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothspeed =10f;
    // Start is called before the first frame update

    private void LateUpdate()
    {
        Vector3 desiredPos= target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothspeed*Time.deltaTime);
    }
}
