using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackEndGameInfo : MonoBehaviour
{
    // Insert 는 '생성' 작업에 주로 사용된다. 
    public void OnClickInsertData()
    {
        int charScore = Random.Range(0, 99999);

        // Param은 뒤끝 서버와 통신을 할 떄 넘겨주는 파라미터 클래스 
        Param param = new Param();   
        param.Add("score", charScore);     
        
        BackendReturnObject BRO = Backend.GameInfo.Insert("score", param);

        if(BRO.IsSuccess())
        {
            Debug.Log("indate : " + BRO.GetInDate());
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "404":
                    Debug.Log("존재하지 않는 tableName인 경우");
                    break;

                case "412":
                    Debug.Log("비활성화 된 tableName인 경우");
                    break;

                case "413":
                    Debug.Log("하나의 row( column들의 집합 )이 400KB를 넘는 경우");
                    break;

                default:
                    Debug.Log("서버 공통 에러 발생: " + BRO.GetMessage());
                    break;
            }
        }
    }
}