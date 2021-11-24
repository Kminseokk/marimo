using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type {A, B, C, D}; //enemy 타입조정 일반, 저글링, 보스
    public Type enemyType; 

    public int maxHealth;
    public int nowHealth;

    public int score;

    public Transform target;
    public BoxCollider meleeArea; //몬스터의 공격범위 변수화
    public GameObject bullet; //미사일담기
    public GameObject[] coins;

    public GameManager manager;

    public bool isChase; //몬스터의 추격
    public bool isAttack; //공격중인가?
    public bool isDead; //죽음?

    Rigidbody rigid;
    BoxCollider boxcollider;
    Material mat;
    NavMeshAgent nav; //nav ai 설정
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }


    void Update()
    {
        if(nav.enabled){
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }        
    }

    void DontmoveEnemy()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero; //물리적인속도
            rigid.angularVelocity = Vector3.zero; //회전속도
        }
    }

    void Targeting()
    {
        

        if (!isDead && enemyType != Type.D) {
            float targetRadius = 0;
            float targetRange = 0;

            switch(enemyType){
            case Type.A: //일반, 강력한근접
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B: //빠르지만 약한 근접 몬스터
                targetRadius = 1f;
                targetRange = 15f;
                break;
            case Type.C: //원거리 몬스터
                targetRadius = 0.5f;
                targetRange = 30f;
                break; 
            }
        

            RaycastHit[] rayHits = 
                Physics.SphereCastAll(transform.position,
                                    targetRadius,
                                    transform.forward,
                                    targetRange,
                                    LayerMask.GetMask("Player")
                                    ); //플레이어를 추적하게 함

            if(rayHits.Length > 0 && !isAttack){ //공격 범위 안에 플레이어가 있다면, 공격중이 아닐 때, 플레이어 공격함
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack",true); //공격 애니메이션 실행

        ////////////// 위 공통 로직

        switch (enemyType){
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20,ForceMode.Impulse);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);            
                break;

            case Type.C:
                yield return new WaitForSeconds(0.7f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;
                yield return new WaitForSeconds(2f);     
                 
                break;
        }


        //아래 공통로직//////////

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack",false);


    }

    void FixedUpdate() //충돌로 인해 캐릭터 회전 방지
    {

        DontmoveEnemy();
        Targeting();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            nowHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
            //Debug.Log("Melee : " + nowHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            nowHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
            //Debug.Log("Range : " + nowHealth);
        }

    }


    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;

        if (nowHealth > 0 && !isDead) 
        {
            mat.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;

            isDead = true;
            isChase = false; // 죽는 상황에서 플레이어 찾는거 금지
            nav.enabled = false; 
            anim.SetTrigger("doDie");
            Player player = target.GetComponent<Player>();
            player.score += score;
            int ranCoin = Random.Range(0,3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            switch(enemyType){
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                //case Type.D:
                    //manager.enemyCntD--;
                    //break;                        
            }

            //넉백
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            //넉백

            Destroy(gameObject, 2);
        }
    }
}
