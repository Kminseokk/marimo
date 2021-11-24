using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackEndManager : MonoBehaviour
{
    private static BackEndManager instance = null;
    public static BackEndManager MyInstance { get => instance; set => instance = value; }

    void Awake(){
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);            
        }
        else{
            Destroy(this.gameObject);            
        }

    }

    void Start(){
        InitBackEnd();
    }

    //뒤끝 초기화
    private void InitBackEnd()
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

        // BackEnd.Initialize(BRO =>
        // {

        //     Debug.Log("뒤끝 초기화"+ BRO);
        
        //     if (BRO.IsSuccess())
        //     {
        //         Debug.Log(BackEnd.Utils.GetServerTime());
        //     }
        //     else //실패
        //     {           
        //     }
        // });
    }

    public void ShowErrorUI(BackendReturnObject backendReturn)
    {
        int statusCode = int.Parse(backendReturn.GetStatusCode());

        Debug.Log("오류 발생. statusCode 분리 작업 미 적용.");
        /*
        switch(statusCode)
        {   
            Debug.Log("오류 발생. statusCode 분리 작업 미 적용.");
        }
        */
    }

}

