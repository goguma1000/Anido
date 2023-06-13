using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using TMPro;

public class GameClient : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputfeild;
    private static GameClient instance;
    public User MyData;
    public User OtherData;
    public int turnindex = 0;
    public int mapIndex = 0;
    private static string initGameURL = "http://localhost:8080/game/init";
    private static string updatePlayerInfoURL = "http://localhost:8080/action/update/player";
    private static string fetchPlayerInfoURL = "http://localhost:8080/fetch/info/player";
    private static string getWaitingplayerURL = "http://localhost:8080/room/waitingPlayer";
    private static string getStartstateURL = "http://localhost:8080/room/startstatus";
    private static string getUserDataURL = "http://localhost:8080/user/findByName";

    private static string validObsInfoURL = "http://localhost:8080/install/block/valid";
    //private static string fetchValidObsInfoURL = "";

    private static string fetchPlayerTurnInfoURL = "http://localhost:8080/current/player-turn-info";
    private static string setPlayerTurnInfoURL = "http://localhost:8080/current/player-turn-set";
    private static string createRoomURL = "http://localhost:8080/room/join1";
    private static string joinRoomURL = "http://localhost:8080/room/join2";
    private static string gameStartURL = "http://localhost:8080/room/start";
    private static string fetchMapNameURL = "http://localhost:8080/room/maps";
    private static string updateGameResultURL = "http://localhost:8080/game/end";
    private static string resetRoomData = "http://localhost:8080/room/reset";
    private void Awake()
    {
        //init singleton
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    
    // return client instance
    public static GameClient GetInstance()
    {
        return instance;
    }

    public IEnumerator InitGame() 
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(initGameURL);
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
                Debug.Log("Get valid data");
                string data = webRequest.downloadHandler.text;
                Debug.Log(data);
                ValidData valid = JsonUtility.FromJson<ValidData>(data);
                
                //valid data 사용
                bool validValue = valid.valid;
                    
                if(validValue == true)
                {
                    Debug.Log("Success to init game");
                }
                else
                    Debug.Log("Fail to init game");
        
                break;
        }
    }

    //send player's action data
    public IEnumerator ESendData(int playerType,string action ,int x1, int y1, int x2, int y2)
    {
        PlayerForm form = new PlayerForm(playerType, action, x1, y1, x2, y2);
        string jsonData = JsonUtility.ToJson(form);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(updatePlayerInfoURL, jsonData))
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
                    Debug.Log("data sent successfully!");
                    Debug.Log("ESend: Wait finishing update");
                    GameManager.GetInstance().SetPlayerState(PlayerState.WAITING);
                    // wait update end
                    GameManager.GetInstance().WaitUpdateFinish();
                    break;
            }
            webRequest.Dispose();
        }
    }
    // get data
    public IEnumerator EFetchPosition(int playerNum)
    {
        int temp = playerNum == 1 ? 2 : 1;
        string url = fetchPlayerInfoURL + "?playerNum=" + temp;
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
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
                Debug.Log("data get successfully!");
                string data = webRequest.downloadHandler.text;
                Debug.Log(data);
                PlayerForm form = JsonUtility.FromJson<PlayerForm>(data);
                GameManager.GetInstance().setFetchedData(form);
                break;
        }
    }

    //----------------------------------------

    
    //send Obstacle's valid action data
    public IEnumerator ESendOValidData(int x1, int y1, int x2, int y2)
    {
        Obstacle form = new Obstacle(x1, y1, x2, y2);
        string jsonData = JsonUtility.ToJson(form);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(validObsInfoURL, jsonData))
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
                    Debug.Log("data sent successfully!");
                    
                    Debug.Log("Get valid data");
                    string data = webRequest.downloadHandler.text;
                    Debug.Log(data);
                    ValidData valid = JsonUtility.FromJson<ValidData>(data);
                
                    //valid data 사용
                    bool validValue = valid.valid;
                    
                    if(validValue == true)
                    {
                        GameManager.GetInstance().SetValidPlace(true);
                        GameManager.GetInstance().SetPlayerState(PlayerState.AFTERVALID);
                    }
                    else if(validValue == false)
                    {
                        GameManager.GetInstance().SetValidPlace(false);
                        GameManager.GetInstance().SetPlayerState(PlayerState.AFTERVALID);
                    }
        
                    break;
            }
            webRequest.Dispose();
        }
    }

    // get obstacle valid data validation
    /*public IEnumerator EFetchOValid()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(fetchValidObsInfoURL);
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
                Debug.Log("data get successfully!");
                string data = webRequest.downloadHandler.text;
                Debug.Log(data);
                ValidData valid = JsonUtility.FromJson<ValidData>(data);
                //valid data 사용
                //bool validValue = valid.getValid();
                if(validValue == ) //데이터 못받음
                {

                }
                else //데이터 받음 
                {
                    if(validVale == true)
                    {
                        createobstacle.validPlace = true;
                    }
                    else if(validValue == false)
                    {
                        createobstacle.validPlace = false;
                    }
                    GameManager.GetInstance().SetPlayerState(PlayerState.AFTERVALID);
                }
                break;
        }
    }*/


     //----------------------------------------


    //state == waiting
    public IEnumerator EGetTurn(int playerNum)
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
                if (temp != playerNum) {
                    Debug.Log("EGetTurn: the other finishes update");
                    GameManager.GetInstance().SetPlayerState(PlayerState.OTHERTURN);
                    GameManager.GetInstance().TurnChange();
                    GameManager.GetInstance().GetData();
                }
                break;
        }
    }

    //after update finish
    public IEnumerator ESetTurn(int playerNum)
    {
        Debug.Log("ESetTurn: Set My turn");
        //업데이트 후 내 턴으로 변경
        string turn = playerNum == 1 ? "player1" : "player2";
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
                    GameManager.GetInstance().SetPlayerState(PlayerState.MYTURN);
                    GameManager.GetInstance().TurnChange();
                    break;
            }
            webRequest.Dispose();
        }
    }

    //create matching room
    public IEnumerator JoinRoom(User user, int type)
    {   
        Debug.Log("Join Room");
        string url = type == 1 ? createRoomURL : joinRoomURL;
        if(type == 1)
        {
            string temp = mapIndex == 0 ? "Fall" : "Summer";
            url = url + "?map=" + temp;
        }
        string jsonData = JsonUtility.ToJson(user);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, jsonData))
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
                    Debug.LogError("The room is already full" + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    // everything is ok.
                    if (type == 2)
                    {
                        Debug.Log("Join the room successfully!");
                        string hostinfo = webRequest.downloadHandler.text;
                        OtherData = JsonUtility.FromJson<User>(hostinfo);
                    }
                    else
                    {
                        Debug.Log("create room successfully!");
                    }
                    LoadScene("WaitingRoom");
                    break;
            }
            webRequest.Dispose();
        }
    }

    public IEnumerator StartGame(TextMeshProUGUI text) 
    {
        string url = gameStartURL + "?button=" + "true";
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.SetRequestHeader("Accept", "application/json");
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                text.text = "Cannot start the game";
                break;
            case UnityWebRequest.Result.Success:
                // everything is ok.
                Debug.Log("Game Start");
                webRequest = UnityWebRequest.Get(initGameURL);
                yield return webRequest.SendWebRequest();
                string mapName = mapIndex == 0 ? "Fall" : "Summer";
                LoadScene(mapName);
                break;
        }
    }

    public IEnumerator getWaitingPlayer()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(getWaitingplayerURL);
        webRequest.SetRequestHeader("Accept", "application/json");
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.Success:
                string data = webRequest.downloadHandler.text;
                if (data.CompareTo("") != 0)
                {
                    Debug.Log("Enter other player!");
                    Debug.Log(data);
                    OtherData = JsonUtility.FromJson<User>(data);
                }
                break;
        }
    }

    public IEnumerator getStartState(Action<string> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(getStartstateURL);
        webRequest.SetRequestHeader("Accept", "application/json");
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.Success:
                string data = webRequest.downloadHandler.text;
                if (data.CompareTo("true") == 0)
                {
                    Debug.Log("Start Game");
                    webRequest = UnityWebRequest.Get(fetchMapNameURL);
                    webRequest.SetRequestHeader("Accept", "application/json");
                    yield return webRequest.SendWebRequest();
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        string mapName = webRequest.downloadHandler.text;
                        Debug.Log("Map Name: " + mapName);
                        callback(mapName);
                    }
                    else
                    {
                        callback("Fall");
                    }
                }
                break;
        }
    }

    public IEnumerator sendResult(WinLose result, Action callback)
    {
        Debug.Log("Send Game Result");
        string jsonData = JsonUtility.ToJson(result);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(updateGameResultURL, jsonData))
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
                    Debug.LogError("The room is already full" + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    // everything is ok.
                    Debug.Log("Send Result successfully");
                    UnityWebRequest temp = UnityWebRequest.Post(resetRoomData, "");
                    temp.SendWebRequest();
                    callback();
                    break;
            }
            webRequest.Dispose();
        }
    }

    public IEnumerator getUserData(string name)
    {
        string url = getUserDataURL + "?name=" + name;
        Debug.Log("Get User: " + url);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.SetRequestHeader("Accept", "application/json");
        yield return webRequest.SendWebRequest();
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.Success:
                string data = webRequest.downloadHandler.text;
                Debug.Log("Get User data succcessfully!! " + data);
                MyData = JsonUtility.FromJson<User>(data);
                break;
        }

    }
    public void LoadScene(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
}

public class MoveForm {
    public int x;
    public int y;
}

