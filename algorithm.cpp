bool isValidObstacle(vector<pair<int,int>> obstacleCoord, vector<pair<int,int>> playerCoord) {
    
    //For player1
    _initialize();

    pair<int, int> obstacle_F;
    pair<int, int> obstacle_S;
    
    pair<int, int> player1 = playerCoord[0];
    pair<int, int> player2 = playerCoord[1];

    visited[obstacle_F.X][obstacle_F.Y] = BLOCKED;
    visited[obstacle_S.X][obstacle_S.Y] = BLOCKED;
    
    
    bool flag1 = false;
    queue<pair<int, int>> Q1;
    Q1.push({ player1.X, player1.Y });
    
    while (!Q1.empty()) {
        auto cur = Q1.front(); Q1.pop();
        for (int direction = 0; direction < 4; direction++) {
            int nx = dx[direction] + cur.X;
            int ny = dy[direction] + cur.Y;

            if (nx < 0 || nx >= BORD_SIZE || ny < 0 || ny >= BORD_SIZE) continue;
            if (board[nx][ny] == BLOCKED || visited[nx][ny] == VISITED) continue;
            if (ny == (BORD_SIZE - 1)) {
                flag1 = true;
                break;
            }
            //길 찾았을 경우

            visited[nx][ny] = VISITED;
            Q1.push({ nx,ny });
        }
        if (flag1 == true) break;
    }

    //For player2
    _initialize();

    pair<int, int> obstacle_F;
    pair<int, int> obstacle_S;
    
    pair<int, int> player1 = playerCoord[0];
    pair<int, int> player2 = playerCoord[1];

    visited[obstacle_F.X][obstacle_F.Y] = BLOCKED;
    visited[obstacle_S.X][obstacle_S.Y] = BLOCKED;
    
    bool flag2 = false;
    queue<pair<int, int>> Q2;
    Q2.push({ player1.X, player1.Y });

    while (!Q2.empty()) {
        auto cur = Q2.front(); Q2.pop();
        for (int direction = 0; direction < 4; direction++) {
            int nx = dx[direction] + cur.X;
            int ny = dy[direction] + cur.Y;

            if (nx < 0 || nx >= BORD_SIZE || ny < 0 || ny >= BORD_SIZE) continue;
            if (board[nx][ny] == BLOCKED || visited[nx][ny] == VISITED) continue;
            if (ny == (BORD_SIZE - 1)) {
                flag1 = true;
                break;
            }
            visited[nx][ny] = VISITED;
            Q2.push({ nx,ny });
        }
        if (flag1 == true) break;
    }

    if (flag1 == true && flag2 == true) return true;
    else return false;
}
void _initialize() {
    //init visited
    for (int i = 0; i < BORD_SIZE; i++) {
        for (int j = 0; j < BORD_SIZE; j++) {
            visited[i][j] = 0;
        }
    }

    //copy visited -> board
    for (int i = 0; i < BORD_SIZE; i++) {
        for (int j = 0; j < BORD_SIZE; j++) {
            visited[i][j] = board[i][j];
        }
    }
}
