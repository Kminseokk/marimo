using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        transform.position = target.position + offset; //플레이어 카메라 위치를 객체(플레이어)를 기준으로 하게 함.
        
    }
}
