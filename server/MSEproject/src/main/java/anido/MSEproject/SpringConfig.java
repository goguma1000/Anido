package anido.MSEproject;

import anido.MSEproject.controller.Validation;

import anido.MSEproject.domain.Obstacle;
import anido.MSEproject.repository.UserRepository;

import anido.MSEproject.service.GameService;
import anido.MSEproject.service.UserService;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class SpringConfig {

//    @Bean
//    public UserService userService() {return new UserService(userRepository());}

//    @Bean
//    public UserRepository userRepository(){return new MemoryUserRepository();}

    @Bean
    public Validation validation() {return new Validation();}

    @Bean
    public GameService gameService() {
        return new GameService();
    }

    @Bean
    public Obstacle obstacle(){return new Obstacle();}

}
