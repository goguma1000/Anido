# **Anido**
REST API를 이용한 2인 멀티플레이 보드게임.

코드 경로: [Scripts](https://github.com/goguma1000/Anido/tree/main/MSE_gameProject/Assets/Scripts)  

**트레일러:** [Link](https://youtu.be/aqVc1HXOh_s) (01:04)  

## **목차**  
**[게임 워크플로우](#게임-워크플로우)**  
**[징에믈 설치](#장애물-설치)**  
**[로비](#로비)**  
**[매칭 룸](#매칭-룸)**  

---
### **게임 워크플로우**
각 턴마다 30초에 시간이 주어진다.  
플레이어는 다음과 같은 State를 가진다.
~~~cs
public enum PlayerState
{
    MYTURN,     //나의 턴인 상태
    SENDING,    //서버에 데이터를 전송 중인 상태
    UPDATING,   // 동기화 할 데이터를 받은 후 동기화하고 있는 상태
    WAITING,    // 턴이 바뀌기를 기다리는 상태
    OTHERTURN,  // 다른 플레이어의 턴인 상태 
    AFTERVALID, // 해당 칸에 장애물을 설치 할 수 있는 상태
    GAMEOVER    // 승패가 판정 난 상태
}
~~~
게임은 다음과 같이 진행된다.

**Flow Chart**  

![그림6](https://github.com/goguma1000/Anido/assets/102130574/30af86a6-c2bc-4db9-ad7f-4f1b818d9c2c)

1. State가 MYTURN인 플레이어는 말을 움직일지, 장애물을 설치할지 선택한다.  
2. 장애물을 선택한 경우  
   1. 장애물을 설치 할 칸을 선택한다.
   2. 해당 칸에 설치할 수 있는지 확인하기 위해 서버에 쿼리를 보낸다.(State = SENDING)
   3. 설치할 수 없는 칸이면 2.1과정을 다시 수행한다.
   4. 설치할 수 있는 칸이면 (State = AFTERVALID) 서버에 장애물을 설치 할 좌표를 보낸다. (State = SENDING)
3. 말 이동을 선택한 경우
    1. 말을 움직일 칸을 선택한다.
    2. 서버에 말이 움직일 칸의 좌표를 보낸다(State = SENDING)
4. 선택한 행동에 대한 데이터를 보낸 후 상대방이 해당 데이터를 동기화하고 턴을 변경할 때까지 기다린다.   (State = Waiting)  
   이때 서버에 계속 쿼리를 보내 상대방이 턴을 변경했는지 확인한다.
5. 상대방이 동기화를 끝내고 턴을 변경하면 나의 턴을 바꾼다.(State = OTHERTURN)
6. State가 OTHERTURN인 플레이어는 서버에 계속 쿼리를 보내 상대방의 행동에 대한 데이터가 왔는지 확인한다.
7. 행동에 대한 데이터가 오면 보드판을 업데이트한다.(State = UPDATING)
8. 업데이트가 끝나면 나의 턴을 변경하고 (State = MYTUEN) 서버에 턴을 변경했다고 알린다.
9. 1-8의 과정을 반복하다가 어느 플레이어가 승리 조건을 만족하면 게임을 종료하고(State = GAMEOVER) 승패 결과를 서버에 보낸다.
  
만약 각 턴당 주어진 시간이 초과되면 다음과 같이 진행된다.  

1. State가 MYTURN인경우 서버에 행동 데이터로 -1 값을 보내고 턴을 변경한다.
2. State가 OTHERTURN인 경우 30초 동안 새로운 행동 데이터가 오지 않으면 상대방이 게임을 나갔다고 판단하고 게임을 종료한다.  
</br>

 **관련 코드 링크 :**  
    [GameManager.cs](https://github.com/goguma1000/Anido/blob/main/MSE_gameProject/Assets/Scripts/GameManager.cs)   
    [GameClient.cs](https://github.com/goguma1000/Anido/blob/main/MSE_gameProject/Assets/Scripts/GameClient.cs)  
    [TimerManager.cs](https://github.com/goguma1000/Anido/blob/main/MSE_gameProject/Assets/Scripts/TimerManager.cs)  
    </br>
    
### **장애물 설치**
![그림1](https://github.com/goguma1000/Anido/assets/102130574/a7860ac6-bc36-4675-82de-26ebf79f9e00) | ![그림2](https://github.com/goguma1000/Anido/assets/102130574/c90997de-36d1-4740-a29c-b1d50dee36d6)
---|---|   
</br>

장애물은 90º or -90º 만큼 회전할 수 있다.  
장애물을 곂쳐서 설치할 수 없다.  
상대방이 반대편 라인에 도달할 수 없도록 하는 위치에 장애물을 설치할 수 없다.  

 **관련 코드 링크 :**  
    [CreateObstacle.cs](https://github.com/goguma1000/Anido/blob/main/MSE_gameProject/Assets/Scripts/CreateObstacle.cs) 
    </br></br>  

### **로비**
로비는 다음과 같이 구성되어 있다. 

![그림8](https://github.com/goguma1000/Anido/assets/102130574/4913a37c-4b89-4750-9d97-daabb306de26)  
- 매칭 룸 생성  
   Create Game 버튼을 누르면 맵을 선택할 수 있는 UI가 뜬다.  

   ![그림4](https://github.com/goguma1000/Anido/assets/102130574/cc46b8a2-de5b-40d7-996d-26ba70f73d5b)

   Create 버튼을 누르면 서버에 선택한 맵 정보와 유저 데이터를 보내어 매칭 룸을 생성한다. 

- 매칭 룸 참가
  Join Game 버튼을 누르면 서버에 유저 데이터를 보내어 매칭 룸에 접속한다.  
  이때 매칭 룸에 정상적으로 접속하면 서버에서 호스트 유저 정보를 받을 수 있다.     
 </br>  

 **관련 코드 링크 :**  
    [LobbyManager.cs](https://github.com/goguma1000/Anido/blob/main/MSE_gameProject/Assets/Scripts/LobbyManager.cs) 
    </br></br>  

### **매칭 룸**
매칭 룸은 다음과 같이 구성되어 있다.

**Flow Chart**  

![그림3](https://github.com/goguma1000/Anido/assets/102130574/e92ed6ac-109f-4208-8d9d-2c76449abefc)  

접속한 유저가 호스트 면 계속  서버에 쿼리를 날려 다른 유저가 방에 들어왔는지 확인한다.  
만약 다른 유저가 들어오면 다른 유저의 정보를 화면에 업데이트하고 게임 시작 버튼을 활성화한다.  
접속한 유저가 호스트가 아니면 화면에 호스트 정보를 업데이트하고 호스트가 게임을 시작할 때까지 기다린다.  
</br>  

 **관련 코드 링크 :**  
    [WaitingRoomManager.cs](https://github.com/goguma1000/Anido/blob/main/MSE_gameProject/Assets/Scripts/WaitingRoomManager.cs) 
    </br></br>  
