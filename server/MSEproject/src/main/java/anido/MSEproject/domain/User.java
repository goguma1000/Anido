package anido.MSEproject.domain;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Table(name="user_table")
//@Embeddable
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name="user_name")
    private String name;

    @Column(name="user_password")
    private String password;

    @Column(name="user_win")
    private int win;

    @Column(name="user_lose")
    private int lose;

}
