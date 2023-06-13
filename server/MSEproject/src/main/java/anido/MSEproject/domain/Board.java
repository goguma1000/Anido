package anido.MSEproject.domain;

public class Board {

    public static final int BOARD_SIZE = 17;
    private int[][] board = new int[BOARD_SIZE][BOARD_SIZE];

    public Board(){
        for(int i=0; i<BOARD_SIZE; i++){
            for(int j=0; j<BOARD_SIZE; j++){
                this.board[i][j] = 0;
            }
        }
    }

    public void setBoard(int[][] board) {
        this.board = board;
    }

    public int[][] getBoard() {
        return board;
    }

    public void clear(){
        for(int i=0; i<BOARD_SIZE; i++){
            for(int j=0; j<BOARD_SIZE; j++){
                setBoardValue(i,j,0);
            }
        }
    }
    public void installObstacle(Obstacle obstacle){
        setBoardValue(obstacle.getRow1(), obstacle.getCol1(), 1);
        setBoardValue(obstacle.getRow2(), obstacle.getCol2(), 1);
    }
    public void setBoardValue(int x, int y, int value){
        board[x][y] = value;
    }
    public int getBoardValue(int x, int y){
        return board[x][y];
    }

    public void copyTo(Board dest){
        for(int i=0; i<BOARD_SIZE; i++){
            for(int j=0; j<BOARD_SIZE; j++){
                dest.setBoardValue(i,j, board[i][j]);
            }
        }
    }
    
    public void printBoard() {
    	for(int i = 0; i < 17; i++) {
    		for(int j = 0; j < 17; j++) {
    			System.out.print(board[i][j] + " ");
    		}
    		System.out.println("");
    	}
    }
}
