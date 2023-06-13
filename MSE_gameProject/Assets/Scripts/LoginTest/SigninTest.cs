using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class SigninTest : MonoBehaviour
{
    private string signin_url = "http://localhost:8080/user/sign-in";
    public InputField signinName;
    public InputField signinPassword;

    public GameObject signinFailPop;
    public GameObject signinSucPop;
    public GameObject GameStartButton;
    

    public void SignIn()
    {
        StartCoroutine(SignInRequest());
    }

    private string ParseInput()
    {
        MemberData m = new MemberData();
        m.name = signinName.text;
        m.password = signinPassword.text;
        string json = JsonUtility.ToJson(m);
        return json;
    }

    IEnumerator SignInRequest()
    {
        string memberString = ParseInput();

        UnityWebRequest webRequest = new UnityWebRequest(signin_url, UnityWebRequest.kHttpVerbPOST);
        byte[] stringToSend = new System.Text.UTF8Encoding().GetBytes(memberString);
        webRequest.uploadHandler = new UploadHandlerRaw(stringToSend);
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
                string temp = webRequest.downloadHandler.text;
                Debug.Log(temp);
                User user = JsonUtility.FromJson<User>(temp);
                if(user != null) {
                    signinSucPop.SetActive(true);
                    TextMeshProUGUI text = signinSucPop.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    text.text = "Welcome!\n";
                    text.text += signinName.text;
                    GameClient.GetInstance().MyData = user;
                    GameStartButton.SetActive(true);
                }
                else {
                    signinFailPop.SetActive(true);
                }
                signinName.text = "";
                signinPassword.text = "";
                webRequest.downloadHandler.Dispose();
                webRequest.Dispose();
                break;
        }
    }
}
