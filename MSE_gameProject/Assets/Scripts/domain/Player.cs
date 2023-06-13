using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string action;
    public int posX;
    public int posY;

    public Player(string action, int x, int y)
    {
        this.action = action;
        this.posX = x;
        this.posY = y;
    }
    public int getPosX()
    {
        return posX;
    }

    public void setPosX(int posX)
    {
        this.posX = posX;
    }

    public int getPosY()
    {
        return posY;
    }

    public void setPosY(int posY)
    {
        this.posY = posY;
    }

    public string getAction()
    {
        return action;
    }

    public void setAction(string action)
    {
        this.action = action;
    }
}
