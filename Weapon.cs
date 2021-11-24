using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee,Range } // 근,원거리
    public Type type; //타입
    public int damage; //뎀지
    public float rate; //공속
    public int maxAmmo; //최대탄창
    public int haveAmmo; //현재
    
    

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;


    public void use()
    {
        if (type == Type.Melee) //근접무기 휘두르기
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && haveAmmo > 0)
        {
            haveAmmo--;
            StartCoroutine("Shot");
        }

    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); //위에꺼 실행, 0.1초 대기, 아래 실행
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
 
    }
    //Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴
    //코루틴 : Use() -> Swing() 같이 실행. 서브루틴x 코루틴o (Co에 유래)

    IEnumerator Shot()
    {
        //총알발사]
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //50ㅇ느 속도

        yield return null;
        //탄피  
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody CaseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -1) + Vector3.up * Random.Range(2, 3);
        CaseRigid.AddForce(caseVec, ForceMode.Impulse);
        //CaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //회전함수
        
    }
}
