using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Enemy enemy;

    public GameObject itemshop;
    public GameObject weaponshop;
    public GameObject startZone;
    //public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    //public int enemyCntD; //보스 예정

    //인게임내 변수요소

    public Transform[] enemyZones; //겜 내 적 리스폰지역 관리
    public GameObject[] enemies; //게임 내 적 그룹 관리
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;

    public Text maxScroeText; //메뉴판넬
    public Text scoreText;
    public Text stageText;
    public Text playtimeText;
    public Text playerHealthText;
    public Text playerAmmoText;
    public Text playerCoinText;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public Image weapon1img;
    public Image weapon2img;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public Text curScoreText;
    public Text bestText;


    void Awake() //메뉴판넬 채고점수
    {
        enemyList = new List<int>();
        maxScroeText.text = string.Format("{0:n0}",PlayerPrefs.GetInt("MaxScore"));

       //if(PlayerPrefs.Haskey("MaxScore"))
         //   PlayerPrefs.SetInt("MaxScore",0);
    }

    public void Gamestart()
    {
        menuCam.SetActive(false); //게임 시작 화면이 생기게 함.
        gameCam.SetActive(true);

        
        menuPanel.SetActive(false); //게임 내 화면이 생기게 함. (인게임)
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true); // 유저 생기게 함
    }

    public void GameOver()
    {     
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreText.text;

        Param param = new Param();   //param 통해서 서버로 데이터가 보내지게한다.
        param.Add("score", player.score);      
        BackendReturnObject BRO = Backend.GameData.Insert("score",param);  

        Where where = new Where();
        var bro = Backend.GameData.GetMyData("score", where, 1).ToString();

        var maxScore = PlayerPrefs.GetInt("MaxScore");
        
        if(player.score > maxScore) {       
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore",player.score);         
        }

    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        itemshop.SetActive(false);
        weaponshop.SetActive(false);
        startZone.SetActive(false);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }    


    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.5f;

        itemshop.SetActive(true);
        weaponshop.SetActive(true);
        startZone.SetActive(true);

        foreach(Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        for(int index=0; index < stage; index++){
            int ran = Random.Range(0,3);
            enemyList.Add(ran);

            switch (ran) {
                case 0:
                    enemyCntA++;
                    break;
                case 1:
                    enemyCntB++;
                    break;
                case 2:
                    enemyCntC++;
                    break;
            }
        }

        while(enemyList.Count > 0) {
            int ranZone = Random.Range(0,4);
            GameObject instantEnemy = Instantiate(enemies[enemyList[0]],enemyZones[ranZone].position, enemyZones[ranZone].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;

            enemy.manager =  this; //일반몬스터

            enemyList.RemoveAt(0);            
            yield return new WaitForSeconds(5f); //5초간 
        };

        while (enemyCntA + enemyCntB + enemyCntC >0 ) { //남은 몬스터 수 체크
            yield return null;
        }

        yield return  new WaitForSeconds(4f);
        StageEnd();
    }

    void Update()
    {
        if(isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate() //업데이트가 끝나구 호출되는 함수
    {
        //상단 UI
        scoreText.text = string.Format("{0:n0}",player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime/3600);
        int min = (int)((playTime - hour*3600) /60);
        int sec = (int)(playTime % 60);
                                        //{0:00}은 두자리로 고정 , {0:0}은 한자리로 고정
        playtimeText.text = string.Format("{0:00} ", hour) + ":"+ string.Format("{0:00} ", min) +":" + string.Format("{0:00} ", sec) ;

        //우측하단 UI
        playerHealthText.text = player.health + " /" + player.maxHealth;
        playerCoinText.text = string.Format("{0:n0}",player.coin);
        if(player.equipWeapon == null) //무기가없다면 표시안함
            playerAmmoText.text = "- / "+player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / "+player.ammo;
        else
            playerAmmoText.text = player.equipWeapon.haveAmmo + " / "+player.ammo;

        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();
        
        //중앙하단 UI
        weapon1img.color = new Color(1,1,1, player.hasWeapons[0] ? 1 : 0);
        
        weapon2img.color = new Color(1,1,1, player.hasWeapons[1] ? 1 : 0);
        //보스체력 UI
        //bossHealthBar.localScale= new Vector3((float) enemy.nowHealth / enemy.maxHealth,1,1);
    }
}
