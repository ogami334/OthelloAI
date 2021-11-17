using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class Board {
    
    // MARK: Constant
    public const int BlackTurn       =  100;
    public const int WhiteTurn       = -100;

    public const int NumCell = 64;

    // MARK: Properties
    public int NowTurn{ get; set; }
    public bool GameFinished{ get; set; }
    public ulong PlayerBoard{ get; set; }
    public ulong OpponentBoard{ get; set; }

    // MARK: Initialization
    public Board() {
        this.NowTurn    = BlackTurn;
        this.GameFinished = false;

        // 一般的な初期配置を指定
        this.PlayerBoard    = 0x0000000810000000;
        this.OpponentBoard  = 0x0000001008000000;
    }

    // MARK: Copy Constructor
    public Board(Board brd) {
        this.NowTurn        = brd.NowTurn;
        this.GameFinished   = brd.GameFinished;
        this.PlayerBoard    = brd.PlayerBoard;
        this.OpponentBoard  = brd.OpponentBoard;
    }

    // MARK: Instance Function

    /// <summary>
    /// 着手可否の判定
    /// </summary>
    /// <param name="put">置いた位置にのみフラグが立っている64ビット</param>
    /// <returns>着手可能ならtrue</returns>
    public bool CanPut(ulong put) {
        ulong legalBoard = MakePlayerLegalBoard();
        return (put & legalBoard) == put;
    }

    /// <summary>
    /// 着手し, 反転処理を行う
    /// </summary>
    /// <param name="put">着手位置にのみフラグが立っている64ビット</param>
    public void Reverse(ulong put) {
        // 着手した場合のボードを生成
        ulong rev = 0;
        for (int k = 0; k < 8; ++k) {
            ulong rev_ = 0;
            ulong mask = Transfer(put, k);
            while (mask != 0 && (mask & OpponentBoard) != 0) {
                rev_ |= mask;
                mask = Transfer(mask, k);
            }
            if ((mask & PlayerBoard) != 0) {
                rev |= rev_;
            }
        }
        // 反転する
        this.PlayerBoard   ^= put | rev;
        this.OpponentBoard ^= rev;
        // 現在何手目かを更新
    }
    
    /// <summary>
    /// パス判定 (= プレイヤーのみが置けないとき)
    /// </summary>
    /// <returns>パスならtrue</returns>
    public bool IsPass() {
        // 手番側の合法手ボードを生成
        ulong playerLegalBoard = MakePlayerLegalBoard();

        // 相手側の合法手ボードを生成
        ulong opponentLegalBoard = MakeOpponentLegalBoard();

        // 手番側だけがパスの場合
        return playerLegalBoard == 0x0000000000000000 && opponentLegalBoard != 0x0000000000000000;
    }

    /// <summary>
    /// 終局判定
    /// </summary>
    /// <returns>終局ならtrue</returns>
    public bool IsGameFinished() {
        ulong playerLegalBoard = MakePlayerLegalBoard();
        ulong opponentLegalBoard = MakeOpponentLegalBoard();
        // 両手番とも置く場所がない場合
        return playerLegalBoard == 0x0000000000000000 && opponentLegalBoard == 0x0000000000000000;
    }

    /// <summary>
    /// 手番の入れ替え
    /// </summary>
    public void SwapBoard() {
        // ボードの入れ替え
        ulong tmp = PlayerBoard;
        PlayerBoard = OpponentBoard;
        OpponentBoard = tmp;

        // 色の入れ替え
        NowTurn *= -1;
    }

    /// <summary>
    /// 結果を取得する
    /// </summary>
    public void GetResult(ref int blackScore, ref int whiteScore, ref string result) {
        // 石数を取得
        blackScore = BitCount(PlayerBoard);
        whiteScore = BitCount(OpponentBoard);



        if (NowTurn == WhiteTurn) {
            int tmp = blackScore;
            blackScore = whiteScore;
            whiteScore = tmp;
        }
        // 勝敗情報を取得
        if (blackScore > whiteScore) {
            result = "Black win";
            blackScore = NumCell - whiteScore;
        } else if (blackScore < whiteScore) {
            result = "White win";
            whiteScore = NumCell - blackScore;
        } else {
            result = "Draw";
        }
        File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\result_check.txt","number_of_regal " + result + System.Environment.NewLine);     
    }


    public void GetResult2() {
        // 石数を取得
        int blackScore = BitCount(PlayerBoard);
        int whiteScore = BitCount(OpponentBoard);
        string result;


        if (NowTurn == WhiteTurn) {
            int tmp = blackScore;
            blackScore = whiteScore;
            whiteScore = tmp;
        }
        // 勝敗情報を取得
        if (blackScore > whiteScore) {
            result = "Black win";
            blackScore = NumCell - whiteScore;
        } else if (blackScore < whiteScore) {
            result = "White win";
            whiteScore = NumCell - blackScore;
        } else {
            result = "Draw";
        }
        File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\result_check.txt",result + System.Environment.NewLine);     
    }
    /// <summary>
    /// 手番側が負けているか
    /// </summary>
    public bool IsLose() {
        if (GameFinished) {
            int blackScore = BitCount(PlayerBoard);
            int whiteScore = BitCount(OpponentBoard);
            if (NowTurn == WhiteTurn) {
                int tmp = blackScore;
                blackScore = whiteScore;
                whiteScore = tmp;
            }
            if (blackScore < whiteScore) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }

    }
    /*public bool IsLose() {
        if (GameFinished) {
            int PlayerScore = BitCount(OpponentBoard);
            int OpponentScore = BitCount(PlayerBoard);
            if (PlayerScore > OpponentScore) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }*/


    /*public bool IsDraw() {
        if (GameFinished) {
            int PlayerScore = BitCount(OpponentBoard);
            int OpponentScore = BitCount(PlayerBoard);
            if (PlayerScore == OpponentScore) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }*/

    /// <summary>
    /// 手番側が引き分けているか
    /// </summary>
    public bool IsDraw() {
        if (GameFinished) {
            int blackScore = BitCount(PlayerBoard);
            int whiteScore = BitCount(OpponentBoard);
            if (NowTurn ==WhiteTurn) {
                int tmp = blackScore;
                blackScore = whiteScore;
                whiteScore = tmp;
            }
            if (blackScore == whiteScore) {
                return true;
            }
            else {
                return false;
            }            
        }        
        else {
            return false;
        }
    }
    /// <summary>
    /// 手番側が勝っているか
    /// </summary>
    public bool IsWin() {
        if (GameFinished) {
            int blackScore = BitCount(PlayerBoard);
            int whiteScore = BitCount(OpponentBoard);
            if (NowTurn==WhiteTurn) {
                int tmp = blackScore;
                blackScore = whiteScore;
                whiteScore = tmp;
            }
            if (blackScore > whiteScore) {
                return true;
            }
            else {
                return false;
            }    
        }
        else {
            return false;
        }
 
    }

    /*public bool IsWin() {
        if (GameFinished) {
            int PlayerScore = BitCount(OpponentBoard);
            int OpponentScore = BitCount(PlayerBoard);
            
            if (PlayerScore < OpponentScore) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }*/

    /// <summary>
    /// 反転箇所を求める
    /// </summary>
    /// <param name="put">着手位置のビット値</param>
    /// <returns>反転位置にフラグが立っている64ビット</returns>
    private ulong Transfer(ulong put, int k) {
        switch (k) {
        case 0: //上
            return (put << 8) & 0xffffffffffffff00;
        case 1: //右上
            return (put << 7) & 0x7f7f7f7f7f7f7f00;
        case 2: //右
            return (put >> 1) & 0x7f7f7f7f7f7f7f7f;
        case 3: //右下
            return (put >> 9) & 0x007f7f7f7f7f7f7f;
        case 4: //下
            return (put >> 8) & 0x00ffffffffffffff;
        case 5: //左下
            return (put >> 7) & 0x00fefefefefefefe;
        case 6: //左
            return (put << 1) & 0xfefefefefefefefe;
        case 7: //左上
            return (put << 9) & 0xfefefefefefefe00;
        default:
            return 0;
        }
    }

    /// <summary>
    /// 手番側の合法手ボードを生成
    /// </summary>
    /// <returns>playerから見て, 置ける位置にのみフラグが立っている64ビット</returns>
    public ulong MakePlayerLegalBoard() {
        // 左右端の番人
        ulong horizontalWatchBoard = OpponentBoard & 0x7e7e7e7e7e7e7e7e;
        // 上下端の番人
        ulong verticalWatchBoard   = OpponentBoard & 0x00FFFFFFFFFFFF00;
        // 全辺の番人
        ulong allSideWatchBoard    = OpponentBoard & 0x007e7e7e7e7e7e00;
        // 空きマスのみにビットが立っているボード
        ulong blankBoard = ~(PlayerBoard | OpponentBoard);
        // 隣に相手の色があるかを一時保存する
        ulong tmp;
        // 返り値
        ulong playerLegalBoard;

        // 8方向チェック
        // 一度に返せる石は最大6つ
        // 左
        tmp = horizontalWatchBoard & (PlayerBoard << 1);
        for (int i = 0; i < 5; ++i) {
            tmp |= horizontalWatchBoard & (tmp << 1);
        }
        playerLegalBoard = blankBoard & (tmp << 1);

        // 右
        tmp = horizontalWatchBoard & (PlayerBoard >> 1);
        for (int i = 0; i < 5; ++i) {
            tmp |= horizontalWatchBoard & (tmp >> 1);
        }
        playerLegalBoard |= blankBoard & (tmp >> 1);

        // 上
        tmp = verticalWatchBoard & (PlayerBoard << 8);
        for (int i = 0; i < 5; ++i) {
            tmp |= verticalWatchBoard & (tmp << 8);
        }
        playerLegalBoard |= blankBoard & (tmp << 8);

        // 下
        tmp = verticalWatchBoard & (PlayerBoard >> 8);
        for (int i = 0; i < 5; ++i) {
            tmp |= verticalWatchBoard & (tmp >> 8);
        }
        playerLegalBoard |= blankBoard & (tmp >> 8);

        // 右上
        tmp = allSideWatchBoard & (PlayerBoard << 7);
        for (int i = 0; i < 5; ++i) {
            tmp |= allSideWatchBoard & (tmp << 7);
        }
        playerLegalBoard |= blankBoard & (tmp << 7);

        // 左上
        tmp = allSideWatchBoard & (PlayerBoard << 9);
        for (int i = 0; i < 5; ++i) {
            tmp |= allSideWatchBoard & (tmp << 9);
        }
        playerLegalBoard |= blankBoard & (tmp << 9);

        // 右下
        tmp = allSideWatchBoard & (PlayerBoard >> 9);
        for (int i = 0; i < 5; ++i) {
            tmp |= allSideWatchBoard & (tmp >> 9);
        }
        playerLegalBoard |= blankBoard & (tmp >> 9);

        // 左下
        tmp = allSideWatchBoard & (PlayerBoard >> 7);
        for (int i = 0; i < 5; ++i) {
            tmp |= allSideWatchBoard & (tmp >> 7);
        }
        playerLegalBoard |= blankBoard & (tmp >> 7);

        return playerLegalBoard;
    }

    /// <summary>
    /// 相手側の合法手ボードを生成
    /// </summary>
    /// <returns>opponentから見て, 置ける位置にのみフラグが立っている64ビット</returns>
    private ulong MakeOpponentLegalBoard() {
        SwapBoard();
        ulong opponentLegalBoard = MakePlayerLegalBoard();
        SwapBoard();
        return opponentLegalBoard;
    }

    /// <summary>
    /// 手番側の合法手のListを生成
    /// </summary>
    /// <returns>合法手にのみフラグが立っている64ビット</returns>
    public List<ulong> MakePlayerLegalPutList() {
        List<ulong> legalPutList = new List<ulong>();
        ulong put = 0x8000000000000000;
        ulong legalBoard = MakePlayerLegalBoard();
        while (put > 0) {
            if ((put & legalBoard) > 0) legalPutList.Add(put);
            put >>= 1;
        }
        return legalPutList;
    }

    public int CalcOpenness(ulong put,Board origboard) {
        int res = 0;
        ulong orig = origboard.PlayerBoard;
        ulong newb = OpponentBoard;
        ulong newbp = PlayerBoard;
        //newboard ^= put;
        ulong mask = 0x8000000000000000;
        for (int i=0;i<8;i++) {
            for (int j=0;j<8;j++) {
                if (((orig&mask) ^ (newb&mask) ^ put) >0) {
                    for (int dy=-1;dy<2;dy++) {
                        for (int dx=-1;dx<2;dx++) {
                            if ((0<=dx) &&(dx<8) && (0<=dy) && (dy<8)) {
                                if (((newb & (((ulong)1)<<(64-8*i-8*dy-j-dx))) ==0) && ((newbp & (((ulong)1)<<(64-8*i-8*dy-j-dx)) ) ==0)) {
                                    res += 1;
                                }
                            }
                        }
                    }
                }
                mask >>=1;
            }
        }
        //Console.Error.WriteLine(orig);
        //Console.Error.WriteLine(newb);
        //Console.Error.WriteLine(put);
        //Debug.Log("orig"+orig.ToString("x"));
        //Debug.Log("newb"+newb.ToString("x"));
        //Debug.Log("put"+put.ToString("x"));
        return res;        
    }

    public ulong Random_action() {
        List <ulong> legalPutList;
        legalPutList = MakePlayerLegalPutList();
        int length = legalPutList.Count;
        //Debug.Log("playerboard"+PlayerBoard.ToString("x"));
        //Debug.Log("opponentboard"+OpponentBoard.ToString("x"));
        //Debug.Log("length"+length);
        //Debug.Log("fin"+ GameFinished);
        System.Random rand = new System.Random();
        int bi = rand.Next(0,length);
        return legalPutList[bi];
    }


    /// <summary>
    /// 着手をもとにPlayerBoard, OpponentBoardを更新
    /// </summary>
    public void UpdateBoard(ulong put) {
        if (CanPut(put)) {
            Reverse(put);
            SwapBoard();
        }
        if (IsPass()) {
            SwapBoard();
        }
        if (IsGameFinished()) {
            GameFinished = true;
        }
    }

    /// <summary>
    /// 64ビットの立っているフラグの数を数える
    /// </summary>
    /// <returns>立っているフラグの数</returns>
    public int BitCount(ulong put) {
        int res = 0;
        while(put > 0) {
            if ((put & 1) == 1) res++;
            put >>= 1;
        }
        return res;
    }

    /// <summary>
    /// 手番側の手数を計算
    /// </summary>
    /// <returns>手番側の手数</returns>
    private int CalcPlayerNumberOfHands() {
        ulong playerLegalBoard = MakePlayerLegalBoard();
        int playerNumberOfHands = BitCount(playerLegalBoard);
        return playerNumberOfHands;
    }

    /// <summary>
    /// 相手側の手数を計算
    /// </summary>
    /// <returns>相手側の手数</returns>
    private int CalcOpponentNumberOfHands() {
        SwapBoard();
        int opponentNumberOfHands = CalcPlayerNumberOfHands();
        SwapBoard();
        return opponentNumberOfHands;
    }

    /// <summary>
    /// 手番側と相手側の手数の差を計算
    /// </summary>
    /// <returns>手番側の手数 - 相手側の手数</returns>
    public int CalcDifferenceNumberOfHands() {
        return CalcPlayerNumberOfHands() - CalcOpponentNumberOfHands();
    }

    /// <summary>
    /// 手番側の確定石数を求める
    /// </summary>
    /// <returns>手番側の確定石数</returns>
    private int CalcPlayerSettledStone() {
        //Debug.Log("black "+BitCount(PlayerBoard));
        //Debug.Log("white "+whiteScore);   
        ulong playerSettledStoneBoard = 0;
        while(true) {
            int cntNewSettledStone = 0;
            for(ulong mask = 0x8000000000000000; mask > 0; mask >>= 1) {
                // 手番側の石でないならばcontinue
                if((mask & PlayerBoard) == 0) continue;

                // 既に確定石ならばcontinue
                if((mask & playerSettledStoneBoard) > 0) continue;

                // 新たに確定石となる場合
                if(IsSettledStone(mask, playerSettledStoneBoard)) {
                    playerSettledStoneBoard |= mask; // 新たな確定石maskを追加
                    cntNewSettledStone++;
                }
            }
            if(cntNewSettledStone == 0) break;
        }
        return BitCount(playerSettledStoneBoard);
    }

    /// <summary>
    /// 相手側の確定石数を求める
    /// </summary>
    /// <returns>相手側の確定石数</returns>
    private int CalcOpponentSettledStone() {
        SwapBoard();
        int opponentSettledStone = CalcPlayerSettledStone();
        SwapBoard();
        return opponentSettledStone;
    }

    /// <summary>
    /// 手番側と相手側の確定石数の差を求める
    /// </summary>
    /// <returns>手番側の確定石数 - 相手側の確定石数</returns>
    public int CalcDifferenceSettledStone() {
        return CalcPlayerSettledStone() - CalcOpponentSettledStone();
    }

    /// <summary>
    /// 手番側のマス得点を計算する
    /// </summary>
    /// <returns>手番側のマス得点</returns>
    private int CalcPlayerCellPoints() {
        // 位置ごとの得点
        const int PtCenter      = -1;   // 中央
        const int PtBoxEdge     = -1;   // ボックス辺
        const int PtBoxCorner   = 0;    // ボックスコーナー
        const int PtMiddleB     = -3;   // 中B
        const int PtMiddleA     = -3;   // 中A
        const int PtX           = -15;  // X
        const int PtB           = -1;   // B
        const int PtA           = 0;    // A
        const int PtC           = -12;  // C
        const int PtCorner      = 30;   // 隅

        // 位置ごとのbit
        const ulong BitCenter       = 0x0000001818000000;
        const ulong BitBoxEdge      = 0x0000182424180000;
        const ulong BitBoxCorner    = 0x0000240000240000;
        const ulong BitMiddleB      = 0x0018004242001800;
        const ulong BitMiddleA      = 0x0024420000422400;
        const ulong BitX            = 0x0042000000004200;
        const ulong BitB            = 0x1800008181000018;
        const ulong BitA            = 0x2400810000810024;
        const ulong BitC            = 0x4281000000008142;
        const ulong BitCorner       = 0x8100000000000081;

        int res =   PtCenter    * BitCount(PlayerBoard & BitCenter)
                  + PtBoxEdge   * BitCount(PlayerBoard & BitBoxEdge)
                  + PtBoxCorner * BitCount(PlayerBoard & BitBoxCorner)
                  + PtMiddleB   * BitCount(PlayerBoard & BitMiddleB)
                  + PtMiddleA   * BitCount(PlayerBoard & BitMiddleA)
                  + PtX         * BitCount(PlayerBoard & BitX)
                  + PtB         * BitCount(PlayerBoard & BitB)
                  + PtA         * BitCount(PlayerBoard & BitA)
                  + PtC         * BitCount(PlayerBoard & BitC)
                  + PtCorner    * BitCount(PlayerBoard & BitCorner);
        return res;
    }

    /// <summary>
    /// 相手側のマス得点を計算する
    /// </summary>
    /// <returns>相手側のマス得点</returns>
    private int CalcOpponentCellPoints() {
        SwapBoard();
        int res = CalcPlayerCellPoints();
        SwapBoard();
        return res;
    }

    /// <summary>
    /// 手番側と相手側のマス得点の差
    /// </summary>
    /// <returns>手番側のマス得点 - 相手側のマス得点</returns>
    public int CalcDifferenceCellPoints() {
        return CalcPlayerCellPoints() - CalcOpponentCellPoints();
    }

    /// <summary>
    /// 手番側の危険石数を求める
    /// </summary>
    /// <returns>手番側の危険石数</returns>
    private int CountPlayerDangerousHands() {
        int res = 0;
        ulong blankBoard = ~(PlayerBoard | OpponentBoard);
        // 左上
        if((blankBoard & 0x8000000000000000) > 0) res += BitCount(PlayerBoard & 0x40c0000000000000);
        // 右上
        if((blankBoard & 0x0100000000000000) > 0) res += BitCount(PlayerBoard & 0x0203000000000000);
        // 左下
        if((blankBoard & 0x0000000000000080) > 0) res += BitCount(PlayerBoard & 0x000000000000c040);
        // 右下
        if((blankBoard & 0x0000000000000001) > 0) res += BitCount(PlayerBoard & 0x0000000000000302);
        return res;
    }

    /// <summary>
    /// 相手側の危険石数を求める
    /// </summary>
    /// <returns>相手側の危険石数</returns>
    private int CountOpponentDangerousHands() {
        SwapBoard();
        int res = CountPlayerDangerousHands();
        SwapBoard();
        return res;
    }

    /// <summary>
    /// 手番側と相手側の危険石数の差を求める
    /// </summary>
    /// <returns>手番側の危険石数 - 相手側の危険石数</returns>
    public int CountDifferenceDangerousHands() {
        return CountPlayerDangerousHands() - CountOpponentDangerousHands();
    }

    /// <summary>
    /// 確定石か判定
    /// </summary>
    /// <param name="judgeStone">判定する石にのみフラグが立っている64ビット</param>
    /// <param name="settledStoneBoard">確定石にのみフラグが立っている64ビット</param>
    /// <returns>確定石ならtrue</returns>
    private bool IsSettledStone(ulong judgedStone, ulong settledStoneBoard) {
        bool directionSettled;
        ulong mask1, mask2;
        ulong stoneBorad = PlayerBoard | OpponentBoard;

        // 横
        directionSettled = false;
        // 左
        mask1 = judgedStone;
        while(mask1 == (mask1 & 0x7f7f7f7f7f7f7f7f)) mask1 |= mask1 << 1;
        if(mask1 == (mask1 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 右
        mask2 = judgedStone;
        while(mask2 == (mask2 & 0xfefefefefefefefe)) mask2 |= mask2 >> 1;
        if(mask2 == (mask2 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 一列埋まっている
        mask1 |= mask2; // 一列にのみフラグが立っている64ビット
        if(mask1 == (mask1 & stoneBorad)) directionSettled = true;
        // 横に確定していなければfalse
        if(!directionSettled) return false;

        // 縦
        directionSettled = false;
        // 上
        mask1 = judgedStone;
        while(mask1 == (mask1 & 0x00ffffffffffffff)) mask1 |= mask1 << 8;
        if(mask1 == (mask1 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 下
        mask2 = judgedStone;
        while(mask2 == (mask2 & 0xffffffffffffff00)) mask2 |= mask2 >> 8;
        if(mask2 == (mask2 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 一列埋まっている
        mask1 |= mask2; // 一列にのみフラグが立っている64ビット
        if(mask1 == (mask1 & stoneBorad)) directionSettled = true;
        // 縦に確定していなければfalse
        if(!directionSettled) return false;

        // 右上がり
        directionSettled = false;
        // 右上
        mask1 = judgedStone;
        while(mask1 == (mask1 & 0x00fefefefefefefe)) mask1 |= mask1 << 7;
        if(mask1 == (mask1 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 左下
        mask2 = judgedStone;
        while(mask2 == (mask2 & 0x7f7f7f7f7f7f7f00)) mask2 |= mask2 >> 7;
        if(mask2 == (mask2 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 一列埋まっている
        mask1 |= mask2; // 一列にのみフラグが立っている64ビット
        if(mask1 == (mask1 & stoneBorad)) directionSettled = true;
        // 右上がりに確定していなければfalse
        if(!directionSettled) return false;

        // 右下がり
        directionSettled = false;
        // 左上
        mask1 = judgedStone;
        while(mask1 == (mask1 & 0x007f7f7f7f7f7f7f)) mask1 |= mask1 << 9;
        if(mask1 == (mask1 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 右下
        mask2 = judgedStone;
        while(mask2 == (mask2 & 0xfefefefefefefe00)) mask2 |= mask2 >> 9;
        if(mask2 == (mask2 & (judgedStone | settledStoneBoard))) directionSettled = true;
        // 一列埋まっている
        mask1 |= mask2; // 一列にのみフラグが立っている64ビット
        if(mask1 == (mask1 & stoneBorad)) directionSettled = true;
        // 右下がりに確定していなければfalse
        if(!directionSettled) return false;

        return true;
    }

    /// <summary>
    /// 盤面を文字列として表示
    /// </summary>
    /// <returns>盤面を表す文字列('#': 黒石, 'O': 白石, '-': 空きマス)</returns>
    public string DisplayBoardAsString() {
        ulong blackBoard = PlayerBoard;
        ulong whiteBoard = OpponentBoard;
        if (NowTurn == WhiteTurn) {
            blackBoard = OpponentBoard;
            whiteBoard = PlayerBoard;
        }
        string strBoard = "";
        ulong curCell = 0x8000000000000000;
        int cnt = 0;
        while (curCell > 0) {
            if      ((curCell & blackBoard) > 0)  strBoard += "#";
            else if ((curCell & whiteBoard) > 0)  strBoard += "O";
            else                            strBoard += "-";
            if (++cnt % 8 == 0) strBoard += "\n";
            curCell >>= 1;
        }
        return strBoard;
    }
}