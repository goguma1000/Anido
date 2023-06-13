using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleState
{
    HORIZONTAL,
    VERTICAL
}
public class CreateObstacle : MonoBehaviour
{
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] Material[] obstacleMaterial;
    [SerializeField] Material[] playerObstacleMaterial;
    [SerializeField] LayerMask layerMask;
    public GameObject cursorObj;
    public GameObject tileObj;
    public float offset;
    private bool canPlace;
    private ObstacleState obstacleState;
    public int createobstacle;
    private GameManager gameManager;
    private GameClient client;
    private Vector3 clickPos;
    private Vector2 clickTile;
    public bool validPlace;
    private AudioSource audio;
    private GameObject[,] board;

    //public int[,] mapdata;
    
        // Start is called before the first frame update
    void Start()
    {
        client = GameClient.GetInstance();
        obstacleState = ObstacleState.HORIZONTAL;
        cursorObj = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity);
        cursorObj.SetActive(false);
        canPlace = true;
        gameManager = GameManager.GetInstance();
        createobstacle = 0;
        offset = tileObj.transform.GetChild(0).localScale.y;
        validPlace = false;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (createobstacle == 1 && gameManager.state == PlayerState.MYTURN )
        {
            if (Input.GetKeyDown(KeyCode.R)) ChangeObstacleState();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                cursorObj.SetActive(true);
                Vector3 cursorPosition = new Vector3(hit.transform.position.x + (cursorObj.transform.localScale.x * 0.5f), hit.transform.position.y + (hit.transform.GetChild(0).localScale.y + cursorObj.transform.localScale.y)*0.5f, hit.transform.position.z);

                if (cursorObj.transform.position != cursorPosition) cursorObj.transform.position = cursorPosition;

                if (!CheckValid(hit.transform))
                {
                    canPlace = false;
                    cursorObj.transform.GetChild(1).GetComponentInChildren<Renderer>().material = obstacleMaterial[1];
                }
                else
                {
                    cursorObj.transform.GetChild(1).GetComponentInChildren<Renderer>().material = obstacleMaterial[0];
                    canPlace = true;
                }

                if (Input.GetMouseButtonDown(0) && hit.transform != null && canPlace)
                {
                    clickPos = cursorPosition;
                    clickTile = new Vector2((int)hit.transform.position.x, -(int)hit.transform.position.z);
                    switch (obstacleState)
                    {
                        case ObstacleState.HORIZONTAL:
                            StartCoroutine(SendObstacleValidData((int)clickTile.x, (int)clickTile.y,
                            (int)clickTile.x+1, (int)clickTile.y));
                            break;
                        case ObstacleState.VERTICAL:
                            StartCoroutine(SendObstacleValidData((int)clickTile.x, (int)clickTile.y,
                            (int)clickTile.x, (int)clickTile.y+1));
                            break;
                    }
                }

            }
            else
            {
                cursorObj.SetActive(false);
            }
        }

        

        if (createobstacle == 1 && gameManager.state == PlayerState.AFTERVALID)
        {

            if(validPlace)
            {
                PlaceObstacle(clickPos);

                switch (obstacleState)
                {
                    case ObstacleState.HORIZONTAL:
                        board[(int)clickTile.x, (int)clickTile.y].GetComponent<TileManager>().occupiedOtc = 1;
                        board[(int)clickTile.x+1, (int)clickTile.y].GetComponent<TileManager>().occupiedOtc = 1;
                        StartCoroutine(SendObstacleData(gameManager.playerType, (int)clickTile.x, (int)clickTile.y, (int)clickTile.x+1, (int)clickTile.y));
                        break;
                    case ObstacleState.VERTICAL:
                        board[(int)clickTile.x, (int)clickTile.y].GetComponent<TileManager>().occupiedOtc = 1;
                        board[(int)clickTile.x, (int)clickTile.y+1].GetComponent<TileManager>().occupiedOtc = 1;
                        StartCoroutine(SendObstacleData(gameManager.playerType, (int)clickTile.x, (int)clickTile.y, (int)clickTile.x, (int)clickTile.y+1));
                        break;
                }
            }
            else
            {
                //경고메시지
                gameManager.SetPlayerState(PlayerState.MYTURN);
            }
        }

        if(createobstacle == 0)
        {
            cursorObj.SetActive(false);
        }
    }

    IEnumerator SendObstacleData(int playerType, int x1, int y1, int x2, int y2)
    {
        Debug.Log("SendObstacleData: Send Data");
        gameManager.SetPlayerState(PlayerState.SENDING);
        yield return StartCoroutine(client.ESendData(playerType,"blocking",x1, y1, x2, y2));
        createobstacle = 0;
    }
    IEnumerator SendObstacleValidData(int x1, int y1, int x2, int y2)
    {
        Debug.Log("SendOstacleValidData: Send Data");
        gameManager.SetPlayerState(PlayerState.SENDING);
        yield return StartCoroutine(client.ESendOValidData(x1, y1,x2, y2));
    }

    public void PlaceObstacle(Vector3 cursorPosition)
    {
        audio.Play();
        GameObject go = Instantiate(obstaclePrefab, cursorPosition, Quaternion.identity);
        go.transform.GetChild(0).localPosition = cursorObj.transform.GetChild(0).localPosition;
        go.transform.GetChild(0).localRotation = cursorObj.transform.GetChild(0).localRotation;
        go.transform.GetChild(1).localPosition = cursorObj.transform.GetChild(1).localPosition;
        go.transform.GetChild(1).localRotation = cursorObj.transform.GetChild(1).localRotation;
        go.transform.GetChild(1).GetComponentInChildren<Renderer>().material = playerObstacleMaterial[gameManager.GetIndex()];
    }
    private bool CheckValid(Transform t)
    {  
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>().gameBoard;
        switch (obstacleState)
        { 
            case ObstacleState.HORIZONTAL:
                if ((int)t.position.x == 16) return false;
                if (board[(int)t.position.x, -(int)t.position.z].GetComponent<TileManager>().occupiedOtc == 0
                && board[(int)t.position.x, -(int)t.position.z].GetComponent<TileManager>().occupiedPlayer == 0
                && board[(int)t.position.x+1, -(int)t.position.z].GetComponent<TileManager>().occupiedOtc == 0
                && board[(int)t.position.x+1, -(int)t.position.z].GetComponent<TileManager>().occupiedPlayer == 0)
                {
                    return true;
                }
                return false;
            case ObstacleState.VERTICAL:
                if (-(int)t.position.z == 16) return false;
                if (board[(int)t.position.x, -(int)t.position.z].GetComponent<TileManager>().occupiedOtc == 0
                && board[(int)t.position.x, -(int)t.position.z].GetComponent<TileManager>().occupiedPlayer == 0
                && board[(int)t.position.x, -(int)t.position.z+1].GetComponent<TileManager>().occupiedOtc == 0
                && board[(int)t.position.x, -(int)t.position.z+1].GetComponent<TileManager>().occupiedPlayer == 0)
                    return true;
                else return false;
            default: return false;
        }
    }

    private void ChangeObstacleState()
    {
        if (obstacleState == ObstacleState.HORIZONTAL)
        {   
            obstacleState = ObstacleState.VERTICAL;
            cursorObj.transform.GetChild(0).Rotate(new Vector3(0, -90, 0), Space.World);
            cursorObj.transform.GetChild(0).localPosition = new Vector3(0, 0f, -1);
            cursorObj.transform.GetChild(1).Rotate(new Vector3(0, -90, 0), Space.World);
            cursorObj.transform.GetChild(1).localPosition = new Vector3(0, 0f, -1);
        }
        else
        {
            obstacleState = ObstacleState.HORIZONTAL;
            cursorObj.transform.GetChild(0).Rotate(new Vector3(0, 90, 0), Space.World);
            cursorObj.transform.GetChild(0).localPosition = new Vector3(0.5f, 0f, - 0.5f);
            cursorObj.transform.GetChild(1).Rotate(new Vector3(0, 90, 0), Space.World);
            cursorObj.transform.GetChild(1).localPosition = new Vector3(0.5f, 0f, - 0.5f);
        }
    }

    public void setObstacleState(int state)
    {
        if (state == 0)
        {   
            obstacleState = ObstacleState.VERTICAL;
            cursorObj.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            cursorObj.transform.GetChild(0).localPosition = new Vector3(0, 0f, -1);
            cursorObj.transform.GetChild(1).localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            cursorObj.transform.GetChild(1).localPosition = new Vector3(0, 0f, -1);
        }
        else if (state == 1)
        {
            obstacleState = ObstacleState.HORIZONTAL;
            cursorObj.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            cursorObj.transform.GetChild(0).localPosition = new Vector3(0.5f, 0f, - 0.5f);
            cursorObj.transform.GetChild(1).localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            cursorObj.transform.GetChild(1).localPosition = new Vector3(0.5f, 0f, - 0.5f);
        }
    }

}
