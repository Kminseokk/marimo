using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GameManager manager;


    public float speed; //인스팩터창에서 설정하기 위해 공개설정
    public GameObject[] weapons;
    public bool[] hasWeapons;

    public int ammo; //총알
    public int coin; //코인
    public int health; //체력

    public Camera followCamera;

    public AudioSource jumpSound;
    public AudioSource coinSound;
    public AudioSource atkSound;
    public AudioSource dieSound;

    public int maxAmmo; //총알
    public int maxCoin; //코인
    public int maxHealth; //체력 unity내에서 수정가능.
    public int score;

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool fDown; //공격
    bool tDown; //재장전

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isAttReady = true; //공격
    bool isBorder;
    bool isDamage; //무적타이밍 만들기 위해
    bool isShop; //쇼핑중
    bool isDead; //죽음 

    bool iDown; //r 키
    bool sDown1; //1번장비
    bool sDown2; //2번장비

    Vector3 moveVec;
    Vector3 dodgeVec; //이게 없으면 닷지중 방향전환이 됨
    Rigidbody rigid;
    Animator anim;

    MeshRenderer[] meshs; //플레이어 피격시 색깔 바꿀때 필요함 재질 가져오기 함수 몸뚱이다가져오기
    GameObject nearObject;
    public Weapon equipWeapon; //game매니저에서 쓰기위해 퍼블릭선언
    int equipWeaponIndex = -1;
    float AttDelay; //공격

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        PlayerPrefs.SetInt("MaxScore",0);
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload();
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk"); //누를때만 적용되도록 갯버튼
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        tDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interaction"); //edit->프젝세팅 인터엑션, R키
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //노멀라이즈 = 방향값 1 보정

        if (isDodge)
            moveVec = dodgeVec; //이게 없으면 닷지중 방향전환이 됨

        if (isSwap || isReload || (!isAttReady && !isJump ) || isDead)
            moveVec = Vector3.zero;
        
       // if (wDown)
            //transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        //else
            //transform.position += moveVec * speed * Time.deltaTime;

        if (!isBorder)//벽관통을 막음 
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        //3항연산자로도 ㄱㄴ.

        anim.SetBool("IsRun", moveVec != Vector3.zero);
        anim.SetBool("IsWalk", wDown);
    }

    void Turn()
    {
        //키보드 조작
        transform.LookAt(transform.position + moveVec); //캐릭이 나아가는 방향을 보는 설정


        //마우스 커서를 눌렀을 때 그쪽 보기
        if(fDown && !isDead) {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit; 
            if (Physics.Raycast(ray, out rayHit, 100)) //100은 레이의 길이,out은 return 처럼 반환값을 변수에 저장
              {
               Vector3 nextVec = rayHit.point - transform.position;
               nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
             }

        }
        
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);  //점프높이변경시 15변경
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            jumpSound.Play();
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        AttDelay += Time.deltaTime;
        isAttReady = equipWeapon.rate < AttDelay;

        if (fDown && isAttReady && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipWeapon.use();

            atkSound.Play();

            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ?"doSwing" : "doReload");
            AttDelay = 0;
        }

    }

    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (tDown && !isJump && !isDodge && !isSwap && isAttReady && !isShop && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);


        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.haveAmmo = equipWeapon.maxAmmo;
        ammo -= reAmmo;

        isReload = false;
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isShop && !isDead)
        {
            dodgeVec = moveVec; //이게 없으면 닷지중 방향전환이 됨
            speed *= 2;            
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); // 파라미터(함수),시간차
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;


        int weaponIndex = -1;
        if (sDown1) weaponIndex =0;
        if (sDown2) weaponIndex =1;


        if ((sDown1 || sDown2) && !isJump && !isDodge && !isShop && !isDead)
        {
            if(equipWeapon != null)
               equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);


            anim.SetTrigger("doSwap");

            isSwap = true;


            Invoke("SwapOut", 0.5f); // 파라미터(함수),시간차
        }

    }

    void SwapOut()
    {
        
        isSwap = false;
    }

    void Interaction()
    {
        if(iDown && nearObject != null && !isDead & !isJump & !isDodge & !isShop){ //제한{
            if(nearObject.tag == "Weapon"){
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if(nearObject.tag == "Shop"){
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this); //this 는 player       
                isShop = true;         
            }
                
        }
    }

    void DontmoveCha()
    {
        rigid.angularVelocity = Vector3.zero; //회전속도
    }
    void DontgoWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate() //충돌로 인해 캐릭터 회전 방지
    {
        DontmoveCha();
        DontgoWall();
    }

    void OnCollisionEnter(Collision Collision) //착지
    {
        if (Collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item") {
            Item item= other.GetComponent<Item>();
            coinSound.Play();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;

                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;

                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag=="EnemyBullet"){
            if (!isDamage){
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                if(other.GetComponent<Rigidbody>()  != null ) //미사일이 플레이어 만나면 삭제
                    Destroy(other.gameObject);
                StartCoroutine(OnDamage()); //리액션 취하기
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
        //else if (other.tag == "Finish")
        //{
        //    SceneManager.LoadScene("Stage"+(manager.stage+1));
        //}
    }

    IEnumerator OnDamage(){

        isDamage = true;
        foreach(MeshRenderer mesh in meshs){
            mesh.material.color = Color.red;
        }

        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach(MeshRenderer mesh in meshs){
            mesh.material.color = Color.white;
        }



    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
        dieSound.Play();
    }

    void OnTriggerStay(Collider other) //아이템 입수
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
            nearObject = other.gameObject;

       
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null; 
        else if (other.tag == "Shop"){
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;            
        }
            
    }
}


