package anido.MSEproject.service;

import anido.MSEproject.domain.RoomStatus;
import anido.MSEproject.domain.User;
import anido.MSEproject.repository.RoomRepository;
import anido.MSEproject.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class RoomService {
    private final RoomRepository roomRepository;
    private final UserRepository userRepository;


    public void saveRoomStatus(RoomStatus roomStatus) {
        roomRepository.save(roomStatus);
    }

    public void saveHostInfo(User hostInfo) {
        userRepository.save(hostInfo);
    }

    public void saveWaitingPlayerInfo(User waitingPlayerInfo) {
        userRepository.save(waitingPlayerInfo);
    }
}