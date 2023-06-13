using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private LayerMask TileLayer;

    private GameObject[,] board;
    public int playermoving;
    public float movingSpeed = 2;
    private GameManager gameManager;
    private GameClient client;
    [SerializeField] Material[] availableMaterial;
    private ArrayList available;
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        available = new ArrayList();
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>().gameBoard;
        gameManager = GameManager.GetInstance();
        client = GameClient.GetInstance();
        playermoving = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(playermoving == 1 && gameManager.state == PlayerState.MYTURN)
        {
            CalculateAvailable();
            ShowAvailable();
            TileManager tileMouseOver = IsMouseOverATile();
            
            if(tileMouseOver != null && tileMouseOver.occupiedOtc == 0 && tileMouseOver.occupiedPlayer == 0)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    if(IsValid(tileMouseOver.transform.position))
                    {
                        board[(int)transform.position.x, -(int)transform.position.z].GetComponent<TileManager>().occupiedPlayer = 0;
                        tileMouseOver.occupiedPlayer = 1;
                        Vector3 pos = tileMouseOver.transform.position;
                        board[(int)pos.x, -(int)pos.z].GetComponent<TileManager>().occupiedPlayer = 1;
                        Debug.Log(pos);
                    
                        StartCoroutine(SendData(gameManager.playerType, (int)pos.x, -(int)pos.z));
                        StartCoroutine(MoveCharacter(new Vector3(pos.x, transform.position.y, pos.z)));
                    }
                }
            }
        }
        else 
        {
             RemoveAvailable();
             available.Clear();
        }
    }

    public IEnumerator MoveCharacter(Vector3 targetpos)
    {
        this.transform.GetChild(0).GetComponent<Animator>().SetBool("isWalking", true);

        float angle = Vector3.Angle(transform.GetChild(0).forward, targetpos - transform.position);
        angle = Vector3.Dot(transform.GetChild(0).right, targetpos - transform.position) > 0 ? angle : -angle;
        float i = 0;
        while (i < 1 && angle != 0)
        {
            i += 0.05f;
            transform.GetChild(0).Rotate(Vector3.up, Mathf.LerpAngle(0, angle, 1 / 20f));
            yield return null;
        }
        while (true)
        {
            if (!audio.isPlaying)
            {
                audio.Play();
            }
            if (Vector3.Distance(transform.position, targetpos) < Mathf.Epsilon) break;
            transform.position = Vector3.MoveTowards(transform.position, targetpos, Time.deltaTime * movingSpeed);
            yield return null;
        }
        audio.Stop();
        transform.position = targetpos; //위치 보정
        i = 0;
        while (i < 1 && angle != 0)
        {
            i += 0.05f;
            transform.GetChild(0).Rotate(Vector3.up, Mathf.Lerp(0, -angle, 1 / 20f));
            yield return null;
        }

        this.transform.GetChild(0).GetComponent<Animator>().SetBool("isWalking", false);
    }

    IEnumerator SendData(int playerType, int col, int row)
    {
        Debug.Log("SendData: Send Data");
        gameManager.SetPlayerState(PlayerState.SENDING);
        yield return StartCoroutine(client.ESendData(playerType,"moving",col, row,-1,-1));
        playermoving = 0;
    }

    private void CalculateAvailable() 
    {
        if(InBoard(new Vector2(transform.position.x - 1, -(int)transform.position.z)))
        {
            if(board[(int)transform.position.x - 1, -(int)transform.position.z].GetComponent<TileManager>().occupiedPlayer == 0)
            {
                available.Add(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z));
            }
            else
            {
                if(InBoard(new Vector2(transform.position.x - 2, -(int)transform.position.z)))
                {
                    if(board[(int)transform.position.x - 2, -(int)transform.position.z].GetComponent<TileManager>().occupiedOtc == 0)
                    {
                        available.Add(new Vector3(transform.position.x - 2, transform.position.y, transform.position.z));
                    }
                    else
                    {
                        if(InBoard(new Vector2(transform.position.x - 1, -((int)transform.position.z - 1))))
                            available.Add(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1));
                        if(InBoard(new Vector2(transform.position.x - 1, -((int)transform.position.z + 1))))
                            available.Add(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1));
                    }
                }
            }
        }
        
        if(InBoard(new Vector2(transform.position.x + 1, -(int)transform.position.z)))
        {
            if(board[(int)transform.position.x + 1, -(int)transform.position.z].GetComponent<TileManager>().occupiedPlayer == 0)
            {
                available.Add(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z));
            }
            else
            {
                if(InBoard(new Vector2(transform.position.x + 2, -(int)transform.position.z)))
                {
                    if(board[(int)transform.position.x + 2, -(int)transform.position.z].GetComponent<TileManager>().occupiedOtc == 0)
                    {
                        available.Add(new Vector3(transform.position.x + 2, transform.position.y, transform.position.z));
                    }
                    else
                    {
                        if(InBoard(new Vector2(transform.position.x + 1, -((int)transform.position.z - 1))))
                            available.Add(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1));
                        if(InBoard(new Vector2(transform.position.x + 1, -((int)transform.position.z + 1))))
                            available.Add(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1));
                    }
                }
            }
        }

        if(InBoard(new Vector2(transform.position.x, -((int)transform.position.z - 1))))
        {
            if(board[(int)transform.position.x, -((int)transform.position.z - 1)].GetComponent<TileManager>().occupiedPlayer == 0)
            {
                available.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1));
            }
            else
            {
                if(InBoard(new Vector2(transform.position.x, -((int)transform.position.z - 2))))
                {
                    if(board[(int)transform.position.x, -((int)transform.position.z - 2)].GetComponent<TileManager>().occupiedOtc == 0)
                    {
                        available.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z - 2));
                    }
                    else
                    {
                        if(InBoard(new Vector2(transform.position.x - 1, -((int)transform.position.z - 1))))
                            available.Add(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1));
                        if(InBoard(new Vector2(transform.position.x + 1, -((int)transform.position.z - 1))))
                            available.Add(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1));
                    }
                }
            }
        }

        if(InBoard(new Vector2(transform.position.x, -((int)transform.position.z + 1))))
        {
            if(board[(int)transform.position.x, -((int)transform.position.z + 1)].GetComponent<TileManager>().occupiedPlayer == 0)
            {
                available.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1));
            }
            else
            {
                if(InBoard(new Vector2(transform.position.x, -((int)transform.position.z + 2))))
                {
                    if(board[(int)transform.position.x, -((int)transform.position.z + 2)].GetComponent<TileManager>().occupiedOtc == 0)
                    {
                        available.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z + 2));
                    }
                    else
                    {
                        if(InBoard(new Vector2(transform.position.x - 1, -((int)transform.position.z + 1))))
                            available.Add(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1));
                        if(InBoard(new Vector2(transform.position.x + 1, -((int)transform.position.z + 1))))
                            available.Add(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1));
                    }
                }
            }
        }
    }

    private bool InBoard(Vector2 point) 
    {
        if(point.x >= 0 && point.x < 17 && point.y >= 0 && point.y < 17)
        {
            return true;
        }
        return false;
    }

    private void ShowAvailable()
    {
        for(int i = 0; i < available.Count; i++)
        {
            if(InBoard(new Vector2(((Vector3)available[i]).x, -((int)((Vector3)available[i]).z))))
            {
                board[(int)((Vector3)available[i]).x, -(int)((Vector3)available[i]).z].transform.GetChild(1).GetComponent<Renderer>().material = availableMaterial[0];
            }
        }
    }
    
    private void RemoveAvailable()
    {
        for(int i = 0; i < available.Count; i++)
        {
            if(InBoard(new Vector2(((Vector3)available[i]).x, -((int)((Vector3)available[i]).z))))
            {
                board[(int)((Vector3)available[i]).x, -(int)((Vector3)available[i]).z].transform.GetChild(1).GetComponent<Renderer>().material = availableMaterial[1];
            }
        }
    }

    private bool IsValid(Vector3 click)
    {
        for(int i = 0; i < available.Count; i++)
        {
            if(click.Equals(available[i]))
            {
                return true;
            }
        }
        return false;
    
    }

    //Return the tile if mouse is over
    private TileManager IsMouseOverATile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 100f, TileLayer))
        {
            return hitInfo.transform.GetComponent<TileManager>();
        }
        else
        {
            return null;
        }
    }

    
}

