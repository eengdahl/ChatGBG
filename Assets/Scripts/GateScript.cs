using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    Transform transform;
    public bool locker;
    float speed = 5f;
    Vector3 newpos;

    private void Start()
    {
        transform = GetComponent<Transform>();
        locker = true;
        newpos = transform.position;
    }

    private void Update()
    {
        if (locker) { return; }
        newpos.y += 0.001f;
        transform.position = newpos;
    }

    public void MoveGate()
    {
        locker = false;
    }
}
