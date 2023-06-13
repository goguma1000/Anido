using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UIController;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviour
{
    public TextMeshProUGUI nameTXT;
    public TextMeshProUGUI WinningrateTXT;
    private GameClient client;


    private void Start()
    {
        client = GameClient.GetInstance();
        client.mapIndex = 0;
        if (client.OtherData != null) client.OtherData = null;
        StartCoroutine(PatchUserData());
    }
    public void CreateRoom()
    {
        client.turnindex = 1;

        StartCoroutine(client.JoinRoom(client.MyData, 1));
    }

    public void JoinRoom()
    {
        GameClient.GetInstance().turnindex = 2;
        StartCoroutine(GameClient.GetInstance().JoinRoom(client.MyData, 2));
    }

    public void updateProfile()
    {
        User mydata = client.MyData;
        string winnigrate = "Winnig rate: " + mydata.getWinnigRate() + "%(" + mydata.getWin() + "/" + mydata.getTotal() + ")";
        UIController.UIController.UpdateProfile(nameTXT, WinningrateTXT, "ID: " + client.MyData.getName(), winnigrate);
    }

    public void SetMapindex(int idx)
    {
        client.mapIndex = idx;
    }

    IEnumerator PatchUserData()
    {
        yield return StartCoroutine(client.getUserData(client.MyData.getName()));
        updateProfile();
    }
}
