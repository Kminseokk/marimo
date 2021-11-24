using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

 
public class BackEndAuthentication : MonoBehaviour
{
    public InputField idInput;
    public InputField paInput;
    
    // 회원가입1 - 동기 방식
    public void OnClickSignUp()
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(idInput.text, paInput.text, "회원가입 테스트");

        if (BRO.IsSuccess())
        {
            Debug.Log("[동기방식] 회원 가입 성공");
        }

        else
        {
            BackEndManager.MyInstance.ShowErrorUI(BRO);
        }


        // 회원 가입을 한뒤 결과를 BackEndReturnObject 타입으로 반환한다.
        string error = Backend.BMember.CustomSignUp(idInput.text, paInput.text, "Test1").GetErrorCode();
 
        // 회원 가입 실패 처리
        switch(error)
        {
            case "DuplicatedParameterException":
                Debug.Log("중복된 customId 가 존재하는 경우");
                break;
 
            default:
                Debug.Log("회원 가입 완료");
                break;
        }
 
        Debug.Log("동기 방식============================================= ");
 
    }

    public void OnClickLogin()
    {   
        BackendReturnObject BRO = Backend.BMember.CustomLogin(idInput.text, paInput.text);

        if (BRO.IsSuccess())
        {
            Debug.Log("[동기방식] 로그인 성공");

        }

        else
        {
            BackEndManager.MyInstance.ShowErrorUI(BRO);
        }
    }
}