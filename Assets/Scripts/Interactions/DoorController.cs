using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DoorController : Interactable
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public float doorOpenSpeed = 4f;
    public float doorOpenDist = 0.625f;
    public bool startLocked = false;

    private bool locked;
    private float totalOpenDist;

    public bool Locked
    {
        get { return locked; }
        set { locked = value; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Assert.IsNotNull(leftDoor);
        Assert.IsNotNull(rightDoor);

        locked = startLocked;
        totalOpenDist = 0;
        enabled = false;
    }

    void Update()
    {
        if (totalOpenDist >= doorOpenDist)
        {
            enabled = false;
            return;
        }

        float moveDist = doorOpenSpeed * Time.deltaTime;
        if (totalOpenDist + moveDist > doorOpenDist)
        {
            moveDist = doorOpenDist - totalOpenDist;
        }
        totalOpenDist += moveDist;

        Vector3 positionOffset = Vector3.zero;
        positionOffset.x = moveDist;
        rightDoor.transform.localPosition += positionOffset;
        leftDoor.transform.localPosition -= positionOffset;
    }

    public override void Interact()
    {
        if (locked)
        {
            return;
        }

        enabled = true;
    }
}
