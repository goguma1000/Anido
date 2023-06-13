using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TimerManager : MonoBehaviour
{
    public Slider sliderTime;

    private GameManager gamemanager;
    public float leftTime;
    public bool inTurn;
    private static string fetchPlayerTurnInfoURL = "http://localhost:8080/current/player-turn-info";
    private static string setPlayerTurnInfoURL = "http://localhost:8080/current/player-turn-set";
    private void Start()
    {
        inTurn = true;
        gamemanager = GameManager.GetInstance();
    }

    private void FixedUpdate()
    {
        if (inTurn)
        {
            leftTime -= 1 * Time.deltaTime;
            sliderTime.value = leftTime;

            if (leftTime <= 0 && (gamemanager.state == PlayerState.MYTURN || gamemanager.state == PlayerState.OTHERTURN))
            {
                if (gamemanager.state == PlayerState.MYTURN)
                {
                    StartCoroutine(TimeOver_SetTurn(gamemanager.playerType));
                    inTurn = false;
                }
                else if (gamemanager.state == PlayerState.OTHERTURN)
                {
                    StartCoroutine(CheckPlayerExist());
                    inTurn = false;
                }
            }
        }
    }

    IEnumerator CheckPlayerExist()
    {
        float timer = 0f;
        while (gamemanager.state == PlayerState.OTHERTURN && timer < 30f)
        {
            yield return StartCoroutine(TimeOver_GetTurn(gamemanager.playerType));
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        if (timer > 30f)
        {
            Debug.Log("Player Quit");
            GameClient.GetInstance().LoadScene("Lobby");
        }
    }

    //state == waiting
    public IEnumerator TimeOver_GetTurn(int playerNum)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(fetchPlayerTurnInfoURL);
        webRequest.SetRequestHeader("Accept", "application/json");
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                // everything is ok.
                Debug.Log("get turn successfully!");
                string data = webRequest.downloadHandler.text;
                Debug.Log(data);
                TurnForm form = JsonUtility.FromJson<TurnForm>(data);
                int temp = 0;
                if (form.getTurn().CompareTo("player1") == 0) temp = 1;
                else temp = 2;
                //턴이 바뀜
                if (temp == playerNum)
                {
                    Debug.Log("EGetTurn: the other finishes update");
                    GameManager.GetInstance().SetPlayerState(PlayerState.MYTURN);
                    GameManager.GetInstance().TurnChange();
                    inTurn = true;
                }
                break;
        }
    }

    //after update finish
    public IEnumerator TimeOver_SetTurn(int playerNum)
    {
        Debug.Log("ESetTurn: Set My turn");
        //업데이트 후 내 턴으로 변경
        string turn = playerNum == 1 ? "player2" : "player1";
        TurnForm form = new TurnForm(turn);
        string jsonData = JsonUtility.ToJson(form);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(setPlayerTurnInfoURL, jsonData))
        {
            webRequest.uploadHandler.Dispose();
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    // everything is ok.
                    Debug.Log("set turn successfully!");
                    GameManager.GetInstance().SetPlayerState(PlayerState.OTHERTURN);
                    GameManager.GetInstance().TurnChange();
                    inTurn = true;
                    GameManager.GetInstance().GetData();
                    break;
            }
            webRequest.Dispose();
        }
    }
}