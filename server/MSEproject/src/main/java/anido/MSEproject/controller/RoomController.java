
package anido.MSEproject.controller;

import anido.MSEproject.domain.Player;
import anido.MSEproject.domain.RoomStatus;
import anido.MSEproject.domain.User;


import anido.MSEproject.service.RoomService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Random;


@RestController
@RequiredArgsConstructor
@RequestMapping("/room")
public class RoomController {

    private Player hostInfo;
    private Player waitingPlayerInfo;
    private boolean isGameStart = false;

    private final RoomService roomService;
    private String[] themes = {"Fall", "Summer"};
    private String mapName;

    /**
     * API: Join Room
     * Description: Allows a player to join the room.
     * Method: POST
     * Endpoint: '/room/join1'
     * Input:
     * {
     *     "id": 1, // 얘는 나중에 JPA 적용하면 PK로 줄거라서 input에서 뺼거임
     *     "name": "I Am Host",
     *     "password": "123456",
     *     "win": 10,
     *     "lose": 5
     * }
     * Output:
     *      - host information
     * {
     *     "id": 1,
     *     "name": "I Am Host",
     *     "password": "123456",
     *     "win": 10,
     *     "lose": 5
     * }
     */
    @PostMapping("/join1")
    public ResponseEntity<?> joinRoom1(@RequestParam(name="map",required=true) String map,@RequestBody Player player) {
        RoomStatus roomStatus = new RoomStatus();
        System.out.println(map);
        mapName= map;
        hostInfo = player;

//        roomService.saveHostInfo(hostInfo);
//        roomService.saveRoomStatus(roomStatus);

        if (hostInfo == null) {
            return createBadRequestResponse(roomStatus,"Host is already exists.");
        }

        return ResponseEntity.ok(hostInfo);

    }




    /**
     * API: Join Room
     * Description: Allows a player to join the room.
     * Method: POST
     * Endpoint: '/room/join2'
     * Input:
     *  {
     *      "id":2, // 얘는 나중에 JPA 적용하면 PK로 줄거라서 input에서 뺼거임
     *      "name": "I Am Waiting Player",
     *      "password": "123456",
     *      "win": 10,
     *      "lose": 5
     *  }
     * Output:
     *      - host information
     *
     * {
     *     "id": 1,
     *     "name": "I Am Host",
     *     "password": "123456",
     *     "win": 10,
     *     "lose": 5
     * }
     */
    @PostMapping("/join2")
    public ResponseEntity<?> joinRoom2(@RequestBody Player player) {
        RoomStatus roomStatus = new RoomStatus();

        if (hostInfo == null) {
            return createBadRequestResponse(roomStatus,"Cannot join room. There is no Host.");
        }

        waitingPlayerInfo = player;
//        roomService.saveWaitingPlayerInfo(waitingPlayerInfo);
        // roomService.saveRoomStatus(roomStatus);

        if (waitingPlayerInfo == null) {
            return createBadRequestResponse(roomStatus,"Room is already full.");
        }

        return ResponseEntity.ok(hostInfo);
    }



    /**
     * API: Start Game
     * Description: Starts the game if the host and a waiting player are present and the game start button is pressed.
     * Method: POST
     * Endpoint: '/room/start'
     * Input:
     *     - gameStartButtonPressed: Boolean indicating if the game start button was pressed.
     *
     *     client -> localhost:8080/room/start?button=value
     *
     * Output:
     *     - room status
     * {
     *     "host": {
     *         "id": 1,
     *         "name": "I Am Host",
     *         "password": "123456",
     *         "win": 10,
     *         "lose": 5
     *     },
     *     "waitingPlayer": {
     *         "id": 2,
     *         "name": "I Am WaitingPlayer",
     *         "password": "123456",
     *         "win": 10,
     *         "lose": 5
     *     },
     *     "hostReady": true,
     *     "waitingPlayerReady": true,
     *     "message": "Game started!",
     *     "gameStarted": true,
     *     "hostCheck": false
     * }
     */
    @GetMapping("/start")
    public ResponseEntity<RoomStatus> startGame(@RequestParam(required = true, name = "button") boolean button) {
        RoomStatus roomStatus = new RoomStatus();

        if (!button) {
            return createBadRequestResponse(roomStatus, "Cannot start the game.");
        }

       roomStatus.setHost(hostInfo);
       roomStatus.setWaitingPlayer(waitingPlayerInfo);
       //roomService.saveRoomStatus(roomStatus);


        roomStatus.setHostReady(true);
        roomStatus.setWaitingPlayerReady(true);
        roomStatus.setGameStarted(true);
        isGameStart = true;

        roomStatus.setMessage("Game started!");

//        roomService.saveRoomStatus(roomStatus);

        return ResponseEntity.ok(roomStatus);
    }




    /**
     * API: Get Waiting Player Information
     * Description: Returns the waiting player information.
     * Method: GET
     * Endpoint: '/room/waitingPlayer'
     * Output:
     *      - Waiting player information
     *
     * {
     *     "id": 2,
     *     "name": "I Am Waiting Player",
     *     "password": "123456",
     *     "win": 10,
     *     "lose": 5
     * }
     */
    @GetMapping("/waitingPlayer")
    public ResponseEntity<User> getWaitingPlayerInfo() {
        if (waitingPlayerInfo == null) {
            return ResponseEntity.notFound().build();
        }
        return ResponseEntity.ok(waitingPlayerInfo);
    }

    /**
     * API: Get Game Start Status
     * Description: Returns the game start status.
     * Method: GET
     * Endpoint: '/room/startstatus'
     * Output:
     *      - Game start status
     *
     * true
     */
    @GetMapping("/startstatus")
    public ResponseEntity<?> getGameStartStatus() {
        if(hostInfo == null || waitingPlayerInfo == null){
            return ResponseEntity.notFound().build();
        }
        return ResponseEntity.ok(isGameStart);
    }

    /**
     * API: Reset Room
     * Description: Resets the room by initializing hostInfo, waitingPlayerInfo, and isGameStart.
     * Method: POST
     * Endpoint: '/room/reset'
     * Output:
     *      - Success message
     *
     * "Room reset successfully."
     */
    @PostMapping("/reset")
    public ResponseEntity<String> resetInfo() {
        hostInfo = null;
        waitingPlayerInfo = null;
        isGameStart = false;

        return ResponseEntity.ok("reset successfully.");
    }


    /**
     * API: Get Map by Index
     * Description: Returns the map theme at the specified index.
     * Method: GET
     * Endpoint: '/room/maps'
     *
     * @param idx The index of the theme in the array
     *            localhost:8080/room/maps?idx=value (0 or 1)
     * @return The map theme at the specified index.
     */
    @GetMapping("/maps")
    public ResponseEntity<String> getMapByIndex() {
       if(mapName.compareTo("Fall") == 0 || mapName.compareTo("Summer")==0){
           return ResponseEntity.ok(mapName);
       }

       return ResponseEntity.badRequest().body("Invalid index");
    }

    private ResponseEntity<RoomStatus> createBadRequestResponse(RoomStatus roomStatus, String message) {

        roomStatus.setMessage(message);

        return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(roomStatus);
    }



}
