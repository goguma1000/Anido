using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLose
{
    public string winner;
    public string loser;

    public WinLose(string winner, string loser)
    {
        this.winner = winner;
        this.loser = loser;
    }
    public string getWinner()
    {
        return winner;
    }

    public void setWinner(string winner)
    {
        this.winner = winner;
    }

    public string getLoser()
    {
        return loser;
    }

    public void setLoser(string loser)
    {
        this.loser = loser;
    }
}
