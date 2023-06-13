using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnForm
{
    public string turn;

    public TurnForm(string turn)
    {
        this.turn = turn;
    }
    public string getTurn()
    {
        return turn;
    }

    public void setTurn(string turn)
    {
        this.turn = turn;
    }
}
