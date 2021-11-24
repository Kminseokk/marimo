using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 피격 효과 관련 스크립트 */
public class BoxObj : MonoBehaviour
{
    public int maxHealth;
    public int nowHealth;

    Rigidbody rigid;
    BoxCollider boxcollider;
    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;  

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee") //근접무기를 만날때
        {
            Weapon weapon = other.GetComponent<Weapon>();
            nowHealth -= weapon.damage; //유니티 오브젝트 설정에서 데미지 관련
            
            StartCoroutine(OnDamage()); //색상변경 효과를 주기위한 코루틴
            //Debug.Log("Melee : " + nowHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            nowHealth -= bullet.damage;

            Destroy(other.gameObject); //떨어지는 탄피가 삭제되는 효과

            StartCoroutine(OnDamage()); //색상변경 효과를 주기위한 코루틴
            //Debug.Log("Range : " + nowHealth);
        }

    }


    IEnumerator OnDamage()
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f); //색상이 지속해서 변하지 않도록 함.

        if (nowHealth > 0)
        {
            mat.color = Color.white;
        }
        else //피가 0이하로 됐을 때
        {
            mat.color = Color.gray; 
            gameObject.layer = 15; //layer 15번 으로 설정

            Destroy(gameObject, 1);
        }
    }

}
