package anido.MSEproject.controller;

import anido.MSEproject.Form.UserForm;
import anido.MSEproject.domain.Player;
import anido.MSEproject.domain.User;
import anido.MSEproject.service.GameService;
import anido.MSEproject.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;


@Controller
@RequiredArgsConstructor
public class UserController {

    private final UserService userService;
    private final GameService gameService;
    private final Validation validation = new Validation();



    /*
     * 로그인 기능
     * method : get
     * url : http://localhost:8080/sign-in
     * 주의점 : url은 로컬에서 실행하는 경우를 가정한 것
     * input : name
     * output : {"valid" : {Boolean} } //로그인에 성공한 경우 true, 실패한 경우 false
     */
    @PostMapping("/user/sign-in")
    @ResponseBody
    public User signIn(@RequestBody UserForm userForm){
        Optional<User> result = userService.signIn(userForm.getName(), userForm.getPassword());
        //로그인 성공한 경우
        if(result.isPresent()) {
            //로그인한 유저 정보를 이용해서 플레이어 객체 생성
            Player player = new Player(result.get(), -1, -1);
            //게임 서비스에 플레이어 리스트에 넣어줌.
            gameService.addPlayer(player);
            return result.get();
        }

        return null;
    }
    /*
     * 회원가입 기능
     * method : post
     * url : http://localhost:8080/sign-up
     * 주의점 : url은 로컬에서 실행하는 경우를 가정한 것
     * input : { "name" : {string} }
     * output : {"valid" : {Boolean} } //회원가입에 성공한 경우 true, 실패한 경우 false
     */
    @PostMapping("/user/sign-up")
    @ResponseBody
    public Validation signUp(@RequestBody UserForm userForm){
        User user = new User();
        user.setName(userForm.getName());
        user.setPassword(userForm.getPassword());
        //sign-up
        boolean isValid = userService.signUp(user);
        if(isValid) validation.setValid(true);
        else validation.setValid(false);
        return validation;
    }
    /*
     * 전체 유저 조회
     * method : get
     * url : http://localhost:8080/find-all
     * 주의점 : url은 로컬에서 실행하는 경우를 가정한 것
     * output 예시 : [
        {
            "id": 1,
            "name": "성호"
        },
        {
            "id": 2,
            "name": "팔달"
        },
        {
            "id": 3,
            "name": "율곡"
         }
     ]
     */
    @GetMapping("/user/find-all")
    @ResponseBody
    public List<User> findAll(){
        return userService.findAllUsers();
    }
    
    @GetMapping("/user/findByName")
    @ResponseBody
    public User findByName(@RequestParam("name")String name) {
    	User user = userService.findByName(name).get();
    	return user;
    }

}