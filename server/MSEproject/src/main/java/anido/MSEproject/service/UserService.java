package anido.MSEproject.service;

import anido.MSEproject.domain.User;
import anido.MSEproject.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
@RequiredArgsConstructor
public class UserService {

    private final UserRepository userRepository;


    /*
    <회원가입>
    회원가입 성공시 true
    회원가입 실패시 false
    중복 name 허용x
    내부적으로 중복 확인하고 있음.
     */
    public Boolean signUp(User user) {
        boolean isDuplicate = userRepository.findByName(user.getName()).isPresent();
        if(isDuplicate) return false;
        userRepository.save(user);
        return true;
    }
    public void updateUserInfo(User winner, User loser){

        userRepository.save(winner);
        userRepository.save(loser);
    }
    /*
    <로그인>
     */
    public Optional<User> signIn(String name, String password){
        Optional<User> user = userRepository.findByName(name);
        //일치하는 유저가 있는 경우
        if(user.isPresent()){
            //비밀번호 일치하는지 확인
            boolean isMatch = user.get().getPassword().equals(password);
            //비밀번호가 일치하는 경우
            if(isMatch) return user;
            //유저는 있지만, 비밀번호가 일치하지 않는 경우
            else return Optional.empty();
        }
        //유저가 존재하지 않는 경우
        return Optional.empty();
    }
    /*
    전체 user 조회
     */
    public List<User> findAllUsers(){
        return userRepository.findAll();
    }

    /*
    id로 user 한명 조회
     */
    public Optional<User> findOne(Long userId){
        return userRepository.findById(userId);
    }

    //이름으로 user 조회
    public Optional<User> findByName(String name){
        return userRepository.findByName(name);
    }
}