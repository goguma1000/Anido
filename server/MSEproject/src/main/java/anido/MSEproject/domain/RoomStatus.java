
package anido.MSEproject.domain;


import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;


/*

    혹시 여기에 데베 넣을만한거 없으면 엔티티 제외할 예정

 */

@Entity
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Table(name="roomstatus_table")
public class RoomStatus {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

   // @Embedded
   // private User host;
   // @Embedded
   // private User waitingPlayer;

    
    /*
        유저가 하나고, 방을 join1, join2로 나눠서 들어가는 구조라서
        Room:Many -> User:One 맵핑
     */
    @ManyToOne
    @JoinColumn(name="host_id")
    private User host;

    @ManyToOne
    @JoinColumn(name="waiting_player_id")
    private User waitingPlayer;


    @Transient
    private boolean hostReady;
    @Transient
    private boolean waitingPlayerReady;
    @Transient
    private boolean isHostCheck;
    @Transient
    private boolean isGameStarted;
    @Transient
    private String message;

}
