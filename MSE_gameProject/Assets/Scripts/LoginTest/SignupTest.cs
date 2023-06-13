using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class SignupTest : MonoBehaviour
{
    private string signup_url = "http://localhost:8080/user/sign-up";
    public InputField signupName;
    public InputField signupPassword;

    public GameObject signupFailPop;
    public GameObject signupSuccPop;

    public void SignUp()
    {
        StartCoroutine(SignUpRequest());
    }

    private string ParseInput()
    {
        MemberData m = new MemberData();
        m.name = signupName.text;
        m.password = signupPassword.text;
        string json = JsonUtility.ToJson(m);
        return json;
    }

    IEnumerator SignUpRequest()
    {
        string memberJSON = ParseInput();

        //UnityWebRequest webRequest = UnityWebRequest.Post(signup_url, memberJSON);
        UnityWebRequest webRequest = new UnityWebRequest(signup_url, UnityWebRequest.kHttpVerbPOST);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(memberJSON);
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
            
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                webRequest.Dispose();
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + webRequest.error);
                webRequest.Dispose();
                break;
            case UnityWebRequest.Result.Success:
                // everything is ok.
                Debug.Log("Data sent successfully!");
                ValidData validdata = JsonUtility.FromJson<ValidData>(webRequest.downloadHandler.text);
                if(validdata.valid) {
                    signupSuccPop.SetActive(true);
                }
                else {
                    signupFailPop.SetActive(true);
                }
                signupName.text = "";
                signupPassword.text = "";
                webRequest.downloadHandler.Dispose();
                webRequest.Dispose();
                break;
        }
    }
}
