using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerForm
{
    public int playerNumber;
    public string action;
    public int row1; //플레이어 위치
    public int col1; //플레이어 위치
    public int row2; //장애물 설치는 여기까지 사용
    public int col2; //장애물 설치는 여기까지 사용

    public PlayerForm(int playerNumber, string action, int col1, int row1, int col2, int row2)
    {
        this.playerNumber = playerNumber;
        this.action = action;
        this.row1 = row1;
        this.col1 = col1;
        this.row2 = row2;
        this.col2 = col2;
    }

    public string getAction() {
        return action;
    }

    public void setAction(string action) {
        this.action = action;
    }

    public int getPlayerNumber() {
        return playerNumber;
    }

    public void setPlayerNumber(int playerNumber) {
        this.playerNumber = playerNumber;
    }

    public int getRow1() {
        return row1;
    }

    public void setRow1(int row1) {
        this.row1 = row1;
    }

    public int getCol1() {
        return col1;
    }

    public void setCol1(int col1) {
        this.col1 = col1;
    }

    public int getRow2() {
        return row2;
    }

    public void setRow2(int row2) {
        this.row2 = row2;
    }

    public int getCol2() {
        return col2;
    }

    public void setCol2(int col2) {
        this.col2 = col2;
    }
}
