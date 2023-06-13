using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UIController;
using UnityEngine.UI;
public class WaitingRoomManager : MonoBehaviour
{
    public TextMeshProUGUI myNameTXT;
    public TextMeshProUGUI myWinningrateTXT;
    public TextMeshProUGUI otherNameTXT;
    public TextMeshProUGUI otherWinningrateTXT;
    public TextMeshProUGUI errortext;
    public Button startBTN;
    public TextMeshProUGUI btnTXT;
    private GameClient client;
    public void Start()
    {
        client = GameClient.GetInstance();
        updateProfile(client.MyData, myNameTXT, myWinningrateTXT);
        if (client.turnindex == 1)
        {
            StartCoroutine(WaitingPlayer());

            btnTXT.text = "Waiting player";
            startBTN.enabled = false;
            //플레이어 기다리기
        }
        else
        {
            updateProfile(client.OtherData, otherNameTXT, otherWinningrateTXT);
            btnTXT.text = "Waiting for host to start game";
            startBTN.enabled = false;
            //gamestate 확인.
            StartCoroutine(waitingGamestart());
        }
    }
    public void GameStart()
    {
        StartCoroutine(GameClient.GetInstance().StartGame(errortext));
    }

    public void updateProfile(User user, TextMeshProUGUI nameTXT, TextMeshProUGUI WinningrateTXT)
    {
        string winnigrate = "Winnig rate: " + user.getWinnigRate() + "%(" + user.getWin() + "/" + user.getTotal() + ")";
        UIController.UIController.UpdateProfile(nameTXT, WinningrateTXT, "ID: " + user.getName(), winnigrate);
    }

    IEnumerator WaitingPlayer()
    {
        while (client.OtherData == null)
        {
            yield return StartCoroutine(client.getWaitingPlayer());//서버로 waitplayer얻어오기
            if (client.OtherData != null)
            {
                updateProfile(client.OtherData, otherNameTXT, otherWinningrateTXT);
                btnTXT.text = "Start Game";
                startBTN.enabled = true;
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    IEnumerator waitingGamestart()
    {
        while (true)
        {
            yield return StartCoroutine(client.getStartState(StartGame));
        }
    }

    private void StartGame(string mapName)
    {
        StopCoroutine(waitingGamestart());
        Debug.Log("Callback:");
        client.LoadScene(mapName);
    }

}
