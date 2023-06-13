package anido.MSEproject.repository;

import anido.MSEproject.domain.RoomStatus;
import org.springframework.data.jpa.repository.JpaRepository;

public interface RoomRepository extends JpaRepository<RoomStatus,Long> {
}
