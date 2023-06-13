using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UIController;

public enum PlayerState
{
    MYTURN,
    SENDING,
    UPDATING,
    WAITING,
    OTHERTURN,
    AFTERVALID,
    GAMEOVER
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] Material[] playerMaterial;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject actionBTN;
    [SerializeField] private TextMeshProUGUI[] profileID;
    [SerializeField] private TextMeshProUGUI[] profileRate;
    [SerializeField] private Image[] profileBackground;
    [SerializeField] private Sprite[] turnIMG;
    private GameClient client;
    private GameObject[,] board;
    private GameObject[] player = new GameObject[2]; 
    private GameObject turn;
    public CreateObstacle creatingObstacle;
    private int index;
    private GameObject timer;
    private bool myturn = false;
    private PlayerForm fetchedData;
    public int playerType = 0;
    public PlayerState state;
    public int maxObstacle = 15;
    public TextMeshProUGUI nOfObstacleTXT;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    private void Start()
    {   
        CreateBoard();
        CreatePlayers();
        timer = GameObject.FindGameObjectWithTag("Timer");
        client = GameClient.GetInstance();
        myturn = client.turnindex > 1 ? false : true;
        if (myturn)
        {
            SetPlayerState(PlayerState.MYTURN);
            playerType = 1;
            updateProfile(client.MyData, profileID[0], profileRate[0]);
            updateProfile(client.OtherData, profileID[1], profileRate[1]);
            
        }
        else
        {
            SetPlayerState(PlayerState.OTHERTURN);
            playerType = 2;
            updateProfile(client.OtherData, profileID[0], profileRate[0]);
            updateProfile(client.MyData, profileID[1], profileRate[1]);
            GetData();
        }
        profileBackground[0].sprite = turnIMG[0];
        profileBackground[1].sprite = turnIMG[1];
    }

    // Update is called once per frame
    void Update()
    {
        if((player[0].transform.position.x == 0 || player[1].transform.position.x == 16)&& state != PlayerState.GAMEOVER)
        {
            if (state == PlayerState.UPDATING) return;
            SetPlayerState(PlayerState.GAMEOVER);
            if(player[0].transform.position.x == 0 && playerType == 1) //host only call
            {
                //host 승
                player[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isWin");
                player[1].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isLose");
                WinLose result = new WinLose(client.MyData.getName(), client.OtherData.getName());
                StartCoroutine(client.sendResult(result, GameOver));
            }

            if(player[1].transform.position.x == 16 && playerType == 1)
            {
                //host 패
                player[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isLose");
                player[1].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isWin");
                WinLose result = new WinLose(client.OtherData.getName(), client.MyData.getName());
                StartCoroutine(client.sendResult(result, GameOver));
            }

            if (player[0].transform.position.x == 0 && playerType == 2) //client only call
            {
                //host 승
                player[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isWin");
                player[1].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isLose");
                GameOver();
            }

            if (player[1].transform.position.x == 16 && playerType == 2)
            {
                //host 패
                player[0].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isLose");
                player[1].transform.GetChild(0).GetComponent<Animator>().SetTrigger("isWin");
                GameOver();
            }
        }
    }

    private void GameOver()
    {   
        buttons[2].SetActive(true);
    }

    public void ExitGame() 
    {
        SceneManager.LoadScene("Lobby");
    }

    private void CreateBoard()
    {
        GameObject gameBoard = GameObject.FindGameObjectWithTag("Board");
        gameBoard.GetComponent<BoardManager>().CreateBoard();
        board = gameBoard.GetComponent<BoardManager>().gameBoard;
    }

    private void CreatePlayers()
    {
        player[0] = Instantiate(playerPrefab, new Vector3(board[16, 8].transform.position.x, 0, board[16, 8].transform.position.z), Quaternion.identity);
        player[0].transform.GetChild(0).GetComponent<Renderer>().material = playerMaterial[0];
        player[0].transform.GetChild(0).localRotation = Quaternion.Euler(0, -90, 0);
        board[16, 8].GetComponent<TileManager>().occupiedPlayer = 1;
        player[1] = Instantiate(playerPrefab, new Vector3(board[0, 8].transform.position.x, 0, board[0, 8].transform.position.z), Quaternion.identity);
        player[1].transform.GetChild(0).GetComponent<Renderer>().material = playerMaterial[1];
        player[1].transform.GetChild(0).localRotation = Quaternion.Euler(0, 90, 0);
        board[0, 8].GetComponent<TileManager>().occupiedPlayer = 1;
        
        turn = player[0];    
    }

    public void TurnChange()
    {
        if(timer.GetComponent<TimerManager>().leftTime <= 0)
        {
            turn.GetComponent<PlayerManager>().playermoving = 0;
            GameObject.FindGameObjectWithTag("Obstacle").GetComponent<CreateObstacle>().createobstacle = 0;
        }
         timer.GetComponent<TimerManager>().leftTime = 30;
            if(index == player.Length-1)
            {
                profileBackground[index].sprite = turnIMG[1];
                Debug.Log(index);
                turn = player[0];
                index = 0;
                profileBackground[index].sprite = turnIMG[0];
            }
            else
            {
                profileBackground[index].sprite = turnIMG[1];
                Debug.Log(index);
                index++;
                turn = player[index];
                profileBackground[index].sprite = turnIMG[0];
            }
            buttons[0].SetActive(true);
            buttons[1].SetActive(true);
    }

    public void SetActionMovement()
    {
        buttons[0].SetActive(false);
        buttons[1].SetActive(false);
        turn.GetComponent<PlayerManager>().playermoving = 1;
    }
    public void SetActionObstacle()
    {
        if (maxObstacle != 0)
        {
            CreateObstacle obstacleInstance = GameObject.FindGameObjectWithTag("Obstacle").GetComponent<CreateObstacle>();
            buttons[0].SetActive(false);
            buttons[1].SetActive(false);
            obstacleInstance.createobstacle = 1;
            maxObstacle -= 1;
            nOfObstacleTXT.text = "" + maxObstacle;
        }

    }

    public int GetIndex() { return index; }

    public void SetPlayerState(PlayerState state)
    {
        if (this.state == PlayerState.GAMEOVER) return;

        if (state == PlayerState.MYTURN)
        {
            actionBTN.SetActive(true);
        }
        if (state == PlayerState.OTHERTURN)
        {
            actionBTN.SetActive(false);
        }
        this.state = state;
        Debug.Log("State change: " + state);
    }

    //상대 턴 일 때 데이터 풀링
    public void GetData()
    {
        StartCoroutine(GetDataPooling());
    }
    IEnumerator GetDataPooling()
    {
        while (state == PlayerState.OTHERTURN)
        {
            yield return StartCoroutine(client.EFetchPosition(playerType));

            if (fetchedData.getCol1() == -1 && fetchedData.getRow1() == -1 && fetchedData.getCol2() == -1 && fetchedData.getRow2() == -1)
                yield return 0.1;
            else if (fetchedData.getAction() == "moving"){
                if(Vector2.Distance(new Vector2(fetchedData.getCol1(), fetchedData.getRow1()), new Vector2(turn.transform.position.x, -turn.transform.position.z)) < Mathf.Epsilon)
                    yield return 0.1;
                else
                {
                    Debug.Log("received changed player data");
                    SetPlayerState(PlayerState.UPDATING); 
                    board[(int)turn.transform.position.x, -(int)turn.transform.position.z].GetComponent<TileManager>().occupiedPlayer = 0;
                    board[(int)fetchedData.getCol1(), (int)fetchedData.getRow1()].GetComponent<TileManager>().occupiedPlayer = 1;
                    yield return StartCoroutine(turn.GetComponent<PlayerManager>().MoveCharacter(new Vector3(fetchedData.getCol1(), turn.transform.position.y, -fetchedData.getRow1())));
                    //turn.transform.position = new Vector3(fetchedData.getCol1(), turn.transform.position.y, -fetchedData.getRow1());
                    Debug.Log("GetDataPooling: Update finish");
                    yield return StartCoroutine(client.ESetTurn(playerType));
                }
            }
            else if (fetchedData.getAction() == "blocking"){
                if(board[fetchedData.getCol1(), fetchedData.getRow1()].GetComponent<TileManager>().occupiedOtc == 1 &&
                board[fetchedData.getCol2(), fetchedData.getRow2()].GetComponent<TileManager>().occupiedOtc == 1)
                    yield return 0.1;
                else
                {
                    Debug.Log("received changed obstacle data");
                    SetPlayerState(PlayerState.UPDATING);
                    if(fetchedData.getCol1() != fetchedData.getCol2()) {
                        creatingObstacle.setObstacleState(1);
                    }
                    else if(fetchedData.getRow1() != fetchedData.getRow2()) {
                        creatingObstacle.setObstacleState(0);
                    }
                    Vector3 pos = new Vector3(fetchedData.getCol1() + (creatingObstacle.cursorObj.transform.localScale.x * 0.5f), 
                    (creatingObstacle.offset + creatingObstacle.cursorObj.transform.localScale.y)*0.5f, 
                    -fetchedData.getRow1());
                    creatingObstacle.PlaceObstacle(pos);
                    board[fetchedData.getCol1(), fetchedData.getRow1()].GetComponent<TileManager>().occupiedOtc = 1;
                    board[fetchedData.getCol2(), fetchedData.getRow2()].GetComponent<TileManager>().occupiedOtc = 1;
                    Debug.Log("GetDataPooling: Update finish");
                    yield return StartCoroutine(client.ESetTurn(playerType));
                }
            }
        }

        
    }

    public void WaitUpdateFinish()
    {
        StartCoroutine(WaitUpdateEnd());
    }
    IEnumerator WaitUpdateEnd()
    {
        while(state == PlayerState.WAITING)
        {
            yield return StartCoroutine(client.EGetTurn(playerType));
        }
        yield return null;
    }
    //client 에서 받아온 데이터 받기
    public void setFetchedData(PlayerForm playerData)
    {
        fetchedData = playerData;
    }

    /*public void WaitValid()
    {
        StartCoroutine(WaitValidData());
    }
    IEnumerator WaitValidData()
    {
        while(state == PlayerState.WAITING)
        {
            yield return StartCoroutine(client.EFetchOValid());
        }
        yield return null;
    }*/

    public void SetValidPlace(bool valid)
    {
        creatingObstacle.validPlace = valid;
    }

    public void updateProfile(User user, TextMeshProUGUI nameTXT, TextMeshProUGUI WinningrateTXT)
    {
        string winnigrate = "Winnig rate: " + user.getWinnigRate() + "%(" + user.getWin() + "/" + user.getTotal() + ")";
        UIController.UIController.UpdateProfile(nameTXT, WinningrateTXT, "ID: " + user.getName(), winnigrate);
    }

    public static GameManager GetInstance() { return instance; }
}

