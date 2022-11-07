using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSystem : MonoBehaviour
{
    private static DebugSystem instance;

    public GameObject spawnObject;

    public static DebugSystem Instance
    {
        get { return instance; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Create a spawnObject instance at the mouse if P is pressed
        if (Input.GetKeyDown(KeyCode.P) && spawnObject != null)
        {
            GameObject obj = Instantiate(spawnObject);
            Vector3 objPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            objPos.z = 0f;
            obj.transform.position = objPos;
        }
        // Create a spawnObject instance at the mouse if O is held
        if (Input.GetKey(KeyCode.O) && spawnObject != null)
        {
            GameObject obj = Instantiate(spawnObject);
            Vector3 objPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            objPos.z = 0f;
            obj.transform.position = objPos;
        }
    }
}
