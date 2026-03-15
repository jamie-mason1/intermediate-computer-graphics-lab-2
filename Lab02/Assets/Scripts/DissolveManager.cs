using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveManager : MonoBehaviour
{
    public Material mat;
    public float dissolveSpeed = 0.2f;

    float dissolveAmount = 0;

    void Update()
    {
        dissolveAmount += Time.deltaTime * dissolveSpeed;
        mat.SetFloat("_DissolveAmount", dissolveAmount);
    }
}
