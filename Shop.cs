using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public GameObject[] itemObject;
    public int[] itemPrice;
    public Transform[] itemPos;
    public string[] talkData;
    public Text talkText;


    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    // Update is called once per frame
    public void Exit()
    {
        uiGroup.anchoredPosition = Vector3.down * 1000;
        
    }

    public void Buy(int index){
        int price = itemPrice[index];
        if(price > enterPlayer.coin){ //돈없어서 못살때 못산다 말해주기.
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3,3) +
                         Vector3.forward * Random.Range(-3,3);
        Instantiate(itemObject[index],itemPos[index].position + ranVec,itemPos[index].rotation);  

    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1]; //대화 내용 출력
        yield return new WaitForSeconds(2);
        talkText.text = talkData[0];
    }
}
