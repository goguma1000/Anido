using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public long id;
    public string name;
    public string password;

    public int win;
    public int lose;

    public User(long id, string name, string password)
    {
        this.id = id;
        this.name = name;
        this.password = password;
        this.win = 0;
        this.lose = 0;
    }


    public string getPassword()
    {
        return password;
    }

    public void setPassword(string password)
    {
        this.password = password;
    }

    public long getId()
    {
        return id;
    }
    public void setId(long id)
    {
        this.id = id;
    }
    public string getName()
    {
        return name;
    }
    public void setName(string name)
    {
        this.name = name;
    }

    public int getWin()
    {
        return win;
    }

    public void setWin(int win)
    {
        this.win = win;
    }

    public int getLose()
    {
        return lose;
    }

    public void setLose(int lose)
    {
        this.lose = lose;
    }

    public int getTotal()
    {
        return win + lose;
    }

    public int getWinnigRate()
    {
        int rate = win != 0 ? (int)((win / (float)getTotal()) * 100) : 0;
        return rate;
    }
}
