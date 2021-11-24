using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackEndInitialize : MonoBehaviour
{
     void Awake()
    {
        Backend.Initialize(HandleBackendCallback);
    }

    void HandleBackendCallback()
    {
        if (Backend.IsInitialized)
        {
            Debug.Log("뒤끝SDK 초기화 완료");

            // example 
            // 버전체크 -> 업데이트 

            // 구글 해시키 획득 
            if (!Backend.Utils.GetGoogleHash().Equals(""))
                Debug.Log(Backend.Utils.GetGoogleHash());

            // 서버시간 획득
            Debug.Log(Backend.Utils.GetServerTime());
        }
        // 실패
        else
        {
            Debug.LogError("Failed to initialize the backend");
        }
    }


/*
    private void Awake()
    {
        // .Net 3 버전
        // 초기화
        Backend.Initialize(BRO =>
        {
            // 초기화 성공한 경우 실행
            if (BRO.IsSuccess())
            {
                // 구글 해시키 획득 
                if (!Backend.Utils.GetGoogleHash().Equals(""))
                    Debug.Log(Backend.Utils.GetGoogleHash());
            }
            // 초기화 실패한 경우 실행 
            else
            {
            }
        });
    }
*/
}
