using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage; //데미지 설정
    public bool isMelee; //근접공격 에어리어가 벽이나 바닥에 닿아도 삭제되지 않도록.

    void OnCollisionEnter(Collision collision){

        if (collision.gameObject.tag == "Floor") //땅에 닿음
        {
            Destroy(gameObject,3);
        } 
    }


    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall") //총알이 벽에 닿음
        {
            Destroy(gameObject);
        }
    }

}
