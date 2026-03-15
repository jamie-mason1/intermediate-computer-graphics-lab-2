using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAngle : MonoBehaviour
{

    Camera cam;
    Transform target;
    [SerializeField] Vector3 offset;
    

    void Start()
    {
        cam = Camera.main;
        target = GameObject.Find("Basketball(Player)").transform;

        offset = transform.position - target.position;
        Debug.Log(offset);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        //offset = transform.position - target.position;
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
