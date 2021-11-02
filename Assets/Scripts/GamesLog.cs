using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesLog 
{
    public const int MaxGame = 1;
    public int CntGame{ get; set; }
    int cntBlackWin;
    int cntWhiteWin;
    int cntDraw;
    

    public GamesLog() {
        CntGame     = 0;
        cntBlackWin = 0;
        cntWhiteWin = 0;
        cntDraw     = 0;
    }

    public void UpdateLog(Board board, PlayerInformation playerInformation, int CntGames) {
        // Debug.Log(CntGame);
        if(CntGame >  MaxGame) return;
        if(CntGame == MaxGame) {
            if(CntGames % 2 == 0) {
                Debug.Log(playerInformation.WeightNumberOfHands          + ", " +
                            playerInformation.WeightNumberOfSettledStones  + ", " +
                            playerInformation.WeightDangerousHands             + ", " +
                            1 + ", " + cntBlackWin + ", " + cntDraw + ", " + cntWhiteWin);
            }
            else {
                Debug.Log(playerInformation.WeightNumberOfHands          + ", " +
                            playerInformation.WeightNumberOfSettledStones  + ", " +
                            playerInformation.WeightDangerousHands             + ", " +
                            0 + ", " + cntWhiteWin + ", " + cntDraw + ", " + cntBlackWin);
            }
            // Debug.Log("Black " + cntBlackWin + " - " + cntDraw + " - " + cntWhiteWin + " White");
            ++CntGame;
            return;
        }
        ++CntGame;
        int blackScore = 0;
        int whiteScore = 0;
        string result = "";
        board.GetResult(ref blackScore, ref whiteScore, ref result);
        if(blackScore >  whiteScore) cntBlackWin++;
        if(blackScore <  whiteScore) cntWhiteWin++;
        if(blackScore == whiteScore) cntDraw++;
        // Debug.Log(blackScore + ", " + whiteScore + ", " + result);
    }
}