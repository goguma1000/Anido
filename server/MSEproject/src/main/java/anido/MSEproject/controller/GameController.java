package anido.MSEproject.controller;

import anido.MSEproject.Form.ObstacleCoordForm;
import anido.MSEproject.Form.PlayerForm;
import anido.MSEproject.Form.TurnForm;
import anido.MSEproject.Form.WinLose;
import anido.MSEproject.domain.Obstacle;
import anido.MSEproject.domain.Player;
import anido.MSEproject.domain.User;
import anido.MSEproject.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Controller;

import anido.MSEproject.service.GameService;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@Controller
@RequiredArgsConstructor
public class GameController {
    private final GameService gameService;
    private final UserService userService;
    private final Obstacle obstacle;




    //플레이어 좌표 초기 세팅
    @GetMapping("/game/init")
    @ResponseBody
    public Validation initGame(){
        Validation validation = new Validation();
        
        System.out.println("ok");
        Player player1 = gameService.getPlayerInfo(1);
        Player player2 = gameService.getPlayerInfo(2);
        
        //플레이어 초기 좌표 세팅
        player1.setRow(8);
        player1.setCol(16);

        player2.setRow(8);
        player2.setCol(0);
        
        player1.setAction(null);
        player2.setAction(null);
        
        gameService.resetOriginBoard();
        validation.setValid(true);
        
        return validation;
    }

    //순서 set
    @PostMapping("/current/player-turn-set")
    @ResponseBody
    public Validation setPlayerTurnInfo(@RequestBody TurnForm turnForm) {
        Validation validation = new Validation();
        gameService.setTurn(turnForm.getTurn());

        validation.setValid(true);

        return validation;
    }

    //순서 조회 + 블럭 조회까지 추가해야함...
    @GetMapping("/current/player-turn-info")
    @ResponseBody
    public TurnForm getPlayerTurnInfo() {
        TurnForm turnForm = new TurnForm();
        turnForm.setTurn(gameService.getTurn());
        return turnForm;
    }

    @PostMapping("/action/update/player")
    @ResponseBody
    public Validation updatePlayerInfo(@RequestBody PlayerForm playerForm) {
        int row1, col1, row2, col2;

        row1 = playerForm.getRow1();
        col1 = playerForm.getCol1();
        row2 = playerForm.getRow2();
        col2 = playerForm.getCol2();

        Validation validation = new Validation();
        //플레이어 위치만 업데이트
        if(playerForm.getAction().equals("moving")) {
            gameService.updatePlayerInfo(playerForm);
        }
        //블럭설치
        else if(playerForm.getAction().equals("blocking")) {
                gameService.getPlayerInfo(playerForm.getPlayerNumber()).
                        setAction("blocking");
                obstacle.setCoord(row1, col1, row2, col2);
                gameService.installObstacle(obstacle);
        }

        validation.setValid(true);
        return validation;
    }

    //플레이어 action 조회
    @GetMapping("/fetch/info/player")
    @ResponseBody
    public PlayerForm getPlayerInfo(@RequestParam(name="playerNum") int playerNum) {
        Player player = gameService.getPlayerInfo(playerNum);
        PlayerForm playerForm = new PlayerForm();

        playerForm.setPlayerNumber(playerNum);
        playerForm.setAction(player.getAction());
        if(player.getAction() != null) {
            if (player.getAction().equals("moving")) {
                playerForm.setCoord(player.getRow(), player.getCol(), -1, -1);
            } else if (player.getAction().equals("blocking")) {
                playerForm.setCoord(obstacle.getRow1(), obstacle.getCol1(),
                        obstacle.getRow2(), obstacle.getCol2());
            } else {
            }

        }
        return playerForm;
    }

    //블럭 설치 유효성 검사
    @PostMapping("/install/block/valid")
    @ResponseBody
    public Validation IsValidInstall(@RequestBody ObstacleCoordForm obstacleCoordForm){
    	
        int row1, col1, row2, col2;
        row1 = obstacleCoordForm.getRow1();
        col1 = obstacleCoordForm.getCol1();
        row2 = obstacleCoordForm.getRow2();
        col2 = obstacleCoordForm.getCol2();

        Boolean result = gameService.isValidInstall(row1,col1,row2,col2);

        Validation validation = new Validation();
        validation.setValid(result);

        return validation;
    }
    
    
    //블럭 설치
    @PostMapping("/install/block")
    @ResponseBody
    public Validation installBlock(@RequestBody Obstacle obstacle){
        gameService.installObstacle(obstacle);
        Validation validation = new Validation();
        validation.setValid(true);

        return validation;
    }

    @GetMapping("/findAllPlayer")
    @ResponseBody
    public List<Player> getAllPlayer(){
        return gameService.getPlayers();
    }

    //승패 기록 컨트롤러
    @PostMapping("/game/end")
    @ResponseBody
    public Validation recordWinLose(@RequestBody WinLose winLose){
    	User winner = userService.findByName(winLose.getWinner()).get();
    	User loser = userService.findByName(winLose.getLoser()).get();
    	// Player winner = gameService.getPlayerInfo(winLose.getWinner());
        //Player loser = gameService.getPlayerInfo(winLose.getLoser());

        User user1 = new User(winner.getId(), winner.getName(), winner.getPassword(), winner.getWin() + 1, winner.getLose());
        User user2 = new User(loser.getId(), loser.getName(), loser.getPassword(), loser.getWin(), loser.getLose()+1);

        userService.updateUserInfo(user1, user2);



        Validation validation =  new Validation();
        validation.setValid(true);

        return validation;
    }
}
