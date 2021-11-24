using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            manager.StageStart(); //스타트 존에 플레이어가 올시 (스테이지)게임이 시작됨.
    }
}
