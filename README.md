## Server Specification

- project : gradle-Groovy
- Language : java
- Spring Boot Version : 2.7.11
- Java Version : 11
- 참고 홈페이지 : [https://start.spring.io/](https://start.spring.io/)

## 계층 구조

### DB 구조

DB를 바꿀 수 있도록, MemberRepository 라는 interface 사용  
 다른 DB를 이용하고자 한다면, 구현 class만 바꾸면 된다. <br>

![DB 구조](/readmeImg/repository.png)

### 폴더 계층 구조

폴더의 형태는 아래 이미지와 같다.

![폴더 구조](/readmeImg/struct.png)

Controller : MVC에서 Controller를 의미한다.  
service : 핵심 로직들을 구현한 곳.  
repository : DB 접근을 위한 것들.  
domain : 서비스와 관련된 객체들이 들어있는 곳

## UML

### Class Diagram

# server 회의 내용

![폴더 구조](/readmeImg/gameService.png)

## <이동관련한 내용만 포함됨>

## api 사용법

## UserController  
### 1. Sign-Up
- description : sign-up
- method : post
- url : "http://localhost:8080/user/sign-up"
- input format : {"name" : {string}, "password" : {string}}
- output format : <br> {"response" : true || false}


### 2. Sign-In
- description : sign-in
- method : post
- url : http://localhost:8080/user/sign-in
- input format : {"name" : {string}, "password" : {string}}
- output format : {"response" : true || false}

### 3. find All Users
- description : find all users
- method : get
- url : http://localhost:8080/find-all
- input format : x
- output formate= : [ {"id" : {Long}, "name" : {string}, "password" : {string}} ... ]


## GameController 
### 1. player turn set
  - description : Set Player's Turn
  - method : Post
  - url : http://localhost:8080/current/player-turn-set
  - input format : {"turn" {String}}
  - Note: The string that turn can have is "player1" or "player2"
  - output : {"valid" : true || false}


### 2. get turn information
  - description : Check Player's Turn
  - method : Get
  - url : http://localhost:8080/current/player-turn-info
  - input format : x
  - output format : {"turn" : {string}}

### 3. Player position & obstacle install information update 
- description : Update player location and obstacle installation information
- method : post
- url : http://localhost:8080/action/update/player
- input format : {<br>
    "playerNumber" :{int},<br>
    "action" : {String},<br>
    "x1" : {int},<br>
    "y1" : {int},<br>
    "x2" : {int/ default는 -1},<br>
    "y2" : {int/ default는 -1}<br>
}
- output format : {"valid" : true || false}
- output format : x
- note : The possible states for an action are "moving" or   
  "blocking" || possible states for an playerNumber are 1 or 2 

### 4. get player information
- description : get player information
- method : post
- url : http://localhost:8080/fetch/info/player
- input format : int playerNum
- output format : Player
- input : {
    "action" : {"moving" | "blocking"}  
    "row1" : {int}  
    "col1" : {int}  
    "row2" : {int}  
    "col2" : {int}  
  }

### 5. Obstacle installation vaild check
- description : It is responsible for verifying the validity of the obstacle installation.
- method : post
- url : http://localhost:8080/install/block/valid
- input format : {<br>
  "row1" : {int}  
  "col1" : {int}  
  "row2" : {int}  
  "col2" : {int}  
}
- output format : {"valid" : true | false}

### 6. install obstacle
- description : Install obstacle.
- method : post
- url : http://localhost:8080/install/block
- input format : {<br>
  "row1" : {int}
  "col1" : {int}
  "row2" : {int}
  "col2" : {int}
}
- output : {"valid" : true | false}

### 7. Save results after the end of the game
- description : After the game, update information on wins and losses.
- method : post
- url : http://localhost:8080/game/end
- input format : {<br>
  "winner" : {int}  
  "loser" : {int}  
}
- output format : {"valid" : true | false}
  
## RoomController
### 1. Join Room1
- description : Allows a player to join the room
- method : post
- url : http://localhost:8080/room/join1
- input format : {  
  "name" : {string},  
  "password" : {string},  
  "win" : {int}, "lose" : {int}  
}  <br><br>
-output format : {    
  "id": {int}, "name": {string},  
  "password": {string},  
  "win": {int}, "lose": {int}   
}

### 2. Join Room2
- description : Allows a player to join the room.
- method : post
- url : http://localhost:8080/room/join2
- input format : {  
  "name": {string}, "password": {string},  
  "win": {int}, "lose": {int}  
} <br><br>
- output format :{    
  "id": {int}, "name": {string},  
  "password": {string},  
  "win": {int}, "lose": {int}   
}<br><br>

### 3. start Game
- description : Starts the game if the host and a waiting player are  
present and the game start button is pressed.
- method : post
- url : http://localhost:8080/room/start
- input format : {  
  "gameStartButtonPressed" : {Boolean}  
} <br><br>
- output format : [  
"host": {  
  "id": {Long}, "name": {string},  
  "password": {string},  
  "win": {int}, "lose": {int}  
},  
"waitingPlayer": {  
  "id": {Long}, "name": {string},  
  "password": {string},  
  "win": {int}, "lose": {int}  
},  
"hostReady": {boolean},  
"waitingPlayerReady": {boolean},  
"message": {string},  
"gameStarted": {boolean},  
"hostCheck": {boolean}  
]

### 4. get waiting player information
- description : Returns the waiting player information.
- method : get
- url : http://localhost:8080/room/waitingPlayer
- intput format : x
- output format : {  
  "id" : {int}, "name" : {string},  
  "password" : {string},  
  "win" : {int}, "lose" : {int}  
  }

### 5. get game start status
- description : Returns the game start status.
- method : get
- url : http://localhost:8080/room/startstatus
- input format : x
- output format : {
  "return" : {boolean}
}

### 6. get map by index
- description : Returns the map theme at the specified index.
- method : get
- url : http://localhost:8080/room/maps
- input format : x
- output format :  
      @param idx The index of the theme in the array  
      localhost:8080/room/maps?idx=value (0 or 1)  
      @return The map theme at the specified index. 

### 7. reset room
- description : resets the room by initializing hostInfo,   waitingPlayerInfo, and isGameStart
- method : post
- url : http://localhost:8080/room/reset
- input format : x
- output format : {
 "success message" : "Room reset successfully"
}
## server-client 통신 api 호출 시나리오
--------------------------
### 플레이어1 차례로 가정
  1. 이동한 경우
      - step1 : updatePlayerInfo(PlayerForm) : Validation
      - step2 : setPlayerTurnInfo(TurnForm) : Validation   
      [플레이엉 좌표정보 업데이트 -> 차례 넘김]
      
  <br><br>
  1. 장애물을 설치하는 경우  
     - step1 : IsValidInstall(Obstacle) : Validation
     - step2 : updatePlayerInfo(PlayerForm) : Validation
     - step3 : - step2 : setPlayerTurnInfo(TurnForm) : Validation  
      [장애물 유효성 검사 -->장애물 설치 정보 반영 -> 차례넘김]
