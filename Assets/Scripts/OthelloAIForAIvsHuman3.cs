//add alpha-beta search
//add full search at Late stage
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class OthelloAIForAIvsHuman3 : MonoBehaviour
{
    [SerializeField]private GameObject blackStone;
    [SerializeField]private GameObject whiteStone;
    [SerializeField]private GameObject batu;
    [SerializeField]private GameObject star;

    public Board board;
    //変更
    PlayerInformation blackInformation;
    PlayerInformation whiteInformation;
    PlayerInformation player1Information;
    PlayerInformation player2Information;
    // GamesLog gamesLog;

    const bool      isHuman1                        = false;
    const int       MaxNodes1                       = 1000000;
    const double    WeightNumberOfHands1            = 0.3;
    const double    WeightNumberOfSettledStones1    = 1;
    const double    WeightDangerousHands1           = 2;
    const double    WeightCellPoints1               = 0.5;
    const bool      isHuman2                        = true;
    const int       MaxNodes2                       = 100;
    const double    WeightNumberOfHands2            = 0.3;
    const double    WeightNumberOfSettledStones2    = 1;
    const double    WeightDangerousHands2           = 2;

    const double    WeightCellPoints2               = 0.0;
    
    const int maxGames = 1;
    public int CntGames{ get; set; } 
    
    public double bestEval;

    Dictionary<Board,double> EvalDict1 = new Dictionary<Board, double>();
    Dictionary<Board,double> EvalDict2 = new Dictionary<Board, double>();
    void Start() {
        File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\result3.txt","-1"+System.Environment.NewLine);
        Random.InitState(System.DateTime.Now.Millisecond);
        player1Information = new PlayerInformation(isHuman1, MaxNodes1, WeightNumberOfHands1, WeightNumberOfSettledStones1, WeightDangerousHands1,WeightCellPoints1);
        player2Information = new PlayerInformation(isHuman2, MaxNodes2, WeightNumberOfHands2, WeightNumberOfSettledStones2, WeightDangerousHands2,WeightCellPoints2);
        CntGames = 0;
        RestartButtonCilcked();
    }



    void Update() {
        if(board.GameFinished) {
            if (++CntGames < maxGames) RestartButtonCilcked();
        }
        else if(IsHumanTurn()) {
            if(Input.GetMouseButtonDown(0)) {
                Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (BoardIsClicked(clickPosition)) {
                    ulong put = ClickPositionToBit(clickPosition);
                    board.UpdateBoard(put);
                    UpdateBoardDisplay();
                }
            }
        } else {
            AIPlay();
            UpdateBoardDisplay();
            DestroyAllRegalPut();
            AddAllRegalPut();
            //DestroyAllStar();
        }
    }

    /// <summary>
    /// ゲームをリスタート
    /// </summary>
    void RestartGame() {
        bool Player1IsBlack = (CntGames % 2 == 0);
        if (Player1IsBlack) {
            blackInformation = new PlayerInformation(player1Information);
            whiteInformation = new PlayerInformation(player2Information);
        } else {
            blackInformation = new PlayerInformation(player2Information);
            whiteInformation = new PlayerInformation(player1Information);
        }
        board = new Board();
        UpdateBoardDisplay();
    }

    /// <summary>
    /// Restartボタンが押されたときの関数
    /// </summary>
    public void RestartButtonCilcked() {
        // gamesLog = new GamesLog();
        RestartGame();
    }

    /// <summary>
    /// 盤の表示を更新
    /// </summary>
    void UpdateBoardDisplay() {
        DestroyAllStone();
        AddAllStoneOnBoard();
    }

    /// <summary>
    /// 石を全てDestroy
    /// </summary>
    void DestroyAllStone() {
        GameObject[] stones = GameObject.FindGameObjectsWithTag("Stone");
        foreach (GameObject stone in stones) {
            Destroy(stone);
        }
    }

    ///<summary>
    ///候補手を全てDestroy
    ///</summary>
    void DestroyAllRegalPut() {
        GameObject[] regals = GameObject.FindGameObjectsWithTag("Regal");
        foreach (GameObject regal in regals) {
            //Debug.Log("regal");
            Destroy(regal);
        }
    }

    void DestroyAllStar() {
        GameObject [] stars = GameObject.FindGameObjectsWithTag("Pointer");
        foreach (GameObject star in stars) {
            Destroy(star);
        }
    }


    /// <summary>
    /// 盤上の石を全て追加
    ///</summary>
    /// <param name="board">盤</param>
    void AddAllStoneOnBoard() {
        ulong blackBoard = board.PlayerBoard;
        ulong whiteBoard = board.OpponentBoard;
        if (board.NowTurn == Board.WhiteTurn) {
            blackBoard = board.OpponentBoard;
            whiteBoard = board.PlayerBoard;
        }
        ulong mask = 0x8000000000000000;
        for (float y = 1.75f; y >= -1.75f; y -= 0.5f) {
            for (float x = -1.75f; x <= 1.75f; x += 0.5f) {
                if ((mask & blackBoard) > 0) {
                    GameObject stone = Instantiate(blackStone) as GameObject;
                    stone.transform.position = new Vector3(x, y, 0);
                } else if ((mask & whiteBoard) > 0) {
                    GameObject stone = Instantiate(whiteStone) as GameObject;
                    stone.transform.position = new Vector3(x, y, 0);
                }
                mask >>= 1;
            }
        }
    }

    //add original function;
    void AddAllRegalPut() {
        ulong blackBoard = board.PlayerBoard;
        ulong whiteBoard = board.OpponentBoard;
        if (board.NowTurn == Board.WhiteTurn) {
            blackBoard = board.OpponentBoard;
            whiteBoard = board.PlayerBoard;
        }
        ulong regalputBoard = board.MakePlayerLegalBoard();
        ulong mask = 0x8000000000000000;
        for (float y = 1.75f; y >= -1.75f; y -= 0.5f) {
            for (float x = -1.75f; x <= 1.75f; x += 0.5f) {
                if ((mask & regalputBoard) >0) {
                    GameObject stone = Instantiate(batu) as GameObject;
                    stone.transform.position = new Vector3(x, y, 0);
                }
                mask >>= 1;
            }
        }
    }

    void AddAllStarOnBoard(ulong put) {
        ulong mask = 0x8000000000000000;
        for (float y = 1.75f; y >= -1.75f; y -= 0.5f) {
            for (float x = -1.75f; x <= 1.75f; x += 0.5f) {
                if ((mask & put) > 0) {
                    GameObject stone = Instantiate(star) as GameObject;
                    stone.transform.position = new Vector3(x, y, 0);
                }
                mask >>= 1;
            }
        }
    }
    /// <summary>
    /// 人間のターンであるか判定
    /// </summary>
    /// <returns>人間のターンならtrue</returns>
    bool IsHumanTurn() {
        if(board.NowTurn == Board.BlackTurn && blackInformation.IsHuman) return true;
        if(board.NowTurn == Board.WhiteTurn && whiteInformation.IsHuman) return true;
        return false;
    }

    /// <summary>
    /// クリック位置が盤上であるか判定
    /// </summary>
    /// <returns>クリック位置が盤上ならtrue</returns>
    bool BoardIsClicked(Vector3 clickPosition) {
        float x = clickPosition.x;
        float y = clickPosition.y;
        return x >= -2.0f && x < 2.0f &&  y >= -2.0f && y < 2.0f;
    }

    /// <summary>
    /// クリック位置をbitに変換
    ///</summary>
    /// <param name="clickPosition">クリック位置のWorld座標</param>
    /// <returns>着手位置にのみフラグが立っている64ビット</returns>
    ulong ClickPositionToBit(Vector3 clickPosition) {
        float x = clickPosition.x;
        float y = clickPosition.y;
        ulong mask = 0x8000000000000000;

        // x方向へのシフト
        int xshift = (int)((x + 2.0f) * 2.0f);      // [-2, 2) -> [0, 8)
        mask >>= xshift;
        
        // y方向へのシフト
        int yshift = 7 - (int)((y + 2.0f) * 2.0f);  // [-2, 2) -> (8, 0]
        mask >>= yshift * 8;

        return mask;
    }

    /// <summary>
    /// AIの着手を行なう
    /// <summary>
    void AIPlay() {
        PlayerInformation AIInformation = blackInformation;
        if(board.NowTurn == Board.WhiteTurn) AIInformation = whiteInformation;
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        ulong put = GetAIPutFromBoard_updated(board, AIInformation);
        sw.Stop();
        Debug.Log("elapsed "+sw.ElapsedMilliseconds);
        File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\result3.txt",sw.ElapsedMilliseconds.ToString()+System.Environment.NewLine);
        Thread.Sleep(300);
        DestroyAllStar();
        AddAllStarOnBoard(put);
        board.UpdateBoard(put);

    }


    double EvaluationValue_fullsearch(Board boardToEvaluate, int nowDepth,int maxDepth) {
        double res;
        const double INF = 1e9;
        Board newBoard;

        PlayerInformation playerInformation = blackInformation;
        if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
        //int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);


        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(nowDepth == maxDepth) {
            res = board.BitCount(boardToEvaluate.PlayerBoard);//added
            if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1.0;
            return res;
        }
        
        List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
        //playerInformation.LastNumberOfHands.Add(puts.Count);

        // 現在のノードが手番側の場合
        if(boardToEvaluate.NowTurn == board.NowTurn) {
            res = -INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                res = System.Math.Max(res, EvaluationValue_fullsearch_ab(newBoard, nowDepth + 1, maxDepth,-INF,INF));
            }
        }

        // 現在のノードが相手側の場合
        else {
            res = INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                res = System.Math.Min(res, EvaluationValue_fullsearch_ab(newBoard, nowDepth + 1, maxDepth,-INF,INF));
            }
        }

        return res;
    }
    double EvaluationValue_fullsearch_ab(Board boardToEvaluate, int nowDepth,int maxDepth,double alpha,double beta) {
        double res;
        const double INF = 1e9;
        double tmp_value;
        Board newBoard;

        PlayerInformation playerInformation = blackInformation;
        if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
        //int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);


        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(nowDepth == maxDepth) {
            res = board.BitCount(boardToEvaluate.PlayerBoard);//added
            if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1.0;
            return res;
        }
        
        List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
        //playerInformation.LastNumberOfHands.Add(puts.Count);

        // 現在のノードが手番側の場合
        if(boardToEvaluate.NowTurn == board.NowTurn) {
            res = -INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue_fullsearch_ab(newBoard,nowDepth+1,maxDepth,alpha,beta);
                if (tmp_value > res) {
                    res = tmp_value;
                    alpha = res;
                }
                if (res > beta) {
                    return res;
                }
                //res = System.Math.Max(res, EvaluationValue_fullsearch_ab(newBoard, nowDepth + 1, maxDepth,alpha,beta));
            }
        }

        // 現在のノードが相手側の場合
        else {
            res = INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue_fullsearch_ab(newBoard,nowDepth+1,maxDepth,alpha,beta);
                if (tmp_value < res) {
                    res = tmp_value;
                    beta = res;
                }
                if (res<alpha) {
                    return res;
                }
                //res = System.Math.Min(res, EvaluationValue_fullsearch_ab(newBoard, nowDepth + 1, maxDepth,alpha,beta));
            }
        }

        return res;
    }
    double EvaluationValue_fullsearch_ab_Memo(Board boardToEvaluate, int nowDepth,int maxDepth,double alpha,double beta) {
        double res;
        const double INF = 1e9;
        double tmp_value;
        Board newBoard;
        if (EvalDict2.ContainsKey(boardToEvaluate)) {
            return EvalDict2[boardToEvaluate];
        }

        PlayerInformation playerInformation = blackInformation;
        if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
        //int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);


        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(nowDepth == maxDepth) {
            res = board.BitCount(boardToEvaluate.PlayerBoard);//added
            if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1.0;
            EvalDict2[boardToEvaluate] = res;
            return res;
        }
        
        List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
        //playerInformation.LastNumberOfHands.Add(puts.Count);

        // 現在のノードが手番側の場合
        if(boardToEvaluate.NowTurn == board.NowTurn) {
            res = -INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue_fullsearch_ab_Memo(newBoard,nowDepth+1,maxDepth,alpha,beta);
                if (tmp_value > res) {
                    res = tmp_value;
                    alpha = res;
                }
                if (res > beta) {
                    EvalDict2[boardToEvaluate] = res;
                    return res;
                }
                //res = System.Math.Max(res, EvaluationValue_fullsearch_ab(newBoard, nowDepth + 1, maxDepth,alpha,beta));
            }
        }

        // 現在のノードが相手側の場合
        else {
            res = INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue_fullsearch_ab_Memo(newBoard,nowDepth+1,maxDepth,alpha,beta);
                if (tmp_value < res) {
                    res = tmp_value;
                    beta = res;
                }
                if (res<alpha) {
                    EvalDict2[boardToEvaluate] = res;
                    return res;
                }
                //res = System.Math.Min(res, EvaluationValue_fullsearch_ab(newBoard, nowDepth + 1, maxDepth,alpha,beta));
            }
        }
        EvalDict2[boardToEvaluate] = res;
        return res;
    }
    /// <summary>
    /// 評価値を再帰的に求める
    /// </summary>
    /// <param name="boardToEvaluate">評価する盤面</param>
    /// <param name="nowDepth">現在の読みの深さ</param>
    /// <returns>評価値</returns>
    double EvaluationValue(Board boardToEvaluate, int nowDepth,double alpha,double beta) {
        double res;
        const double INF = 1e9;
        double tmp_value;
        Board newBoard;

        PlayerInformation playerInformation = blackInformation;
        if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
        int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);

        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(nowDepth == maxDepth) {
            res =  boardToEvaluate.CalcDifferenceNumberOfHands()    * playerInformation.WeightNumberOfHands
                 + boardToEvaluate.CalcDifferenceSettledStone()     * playerInformation.WeightNumberOfSettledStones
                 - boardToEvaluate.CountDifferenceDangerousHands()  * playerInformation.WeightDangerousHands
                 + boardToEvaluate.CalcDifferenceCellPoints()       * playerInformation.WeightCellPoints;
            if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1.0;
            return res;
        }
        
        List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
        playerInformation.LastNumberOfHands.Add(puts.Count);

        // 現在のノードが手番側の場合
        if(boardToEvaluate.NowTurn == board.NowTurn) {
            res = -INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue(newBoard,nowDepth + 1, alpha,beta);
                if (tmp_value > res) {
                    res = tmp_value;
                    alpha = res;
                }
                //res = System.Math.Max(res, EvaluationValue(newBoard, nowDepth + 1, alpha, beta));//compare child value vs tmp value
                if (res> beta) {
                    return res;
                }
            }
        }

        // 現在のノードが相手側の場合
        else {
            res = INF;
            foreach(ulong put in puts) {
                newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue(newBoard,nowDepth +1 ,alpha,beta);
                if (tmp_value < res) {
                    res = tmp_value;
                    beta = res;
                }
                
                if (res < alpha) {
                    return res;
                }
                //res = System.Math.Min(res, EvaluationValue(newBoard, nowDepth + 1, alpha, beta));
            }
        }

        return res;
    }

    double EvaluationValue_Memo(Board boardToEvaluate, int nowDepth,double alpha,double beta) {
            double res;
            const double INF = 1e9;
            double tmp_value;
            Board newBoard;
            if (EvalDict1.ContainsKey(boardToEvaluate)) {
                return EvalDict1[boardToEvaluate];
            }

            PlayerInformation playerInformation = blackInformation;
            if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
            int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);

            // 再帰の終了条件: 読みの深さが上限に達した場合
            if(nowDepth == maxDepth) {
                res =  boardToEvaluate.CalcDifferenceNumberOfHands()    * playerInformation.WeightNumberOfHands
                    + boardToEvaluate.CalcDifferenceSettledStone()     * playerInformation.WeightNumberOfSettledStones
                    - boardToEvaluate.CountDifferenceDangerousHands()  * playerInformation.WeightDangerousHands
                    + boardToEvaluate.CalcDifferenceCellPoints()       * playerInformation.WeightCellPoints;
                if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1.0;
                EvalDict1[boardToEvaluate] = res;
                return res;
            }
            
            List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
            playerInformation.LastNumberOfHands.Add(puts.Count);

            // 現在のノードが手番側の場合
            if(boardToEvaluate.NowTurn == board.NowTurn) {
                res = -INF;
                foreach(ulong put in puts) {
                    newBoard = new Board(boardToEvaluate);
                    newBoard.UpdateBoard(put);
                    tmp_value = EvaluationValue(newBoard,nowDepth + 1, alpha,beta);
                    if (tmp_value > res) {
                        res = tmp_value;
                        alpha = res;
                    }
                    //res = System.Math.Max(res, EvaluationValue(newBoard, nowDepth + 1, alpha, beta));//compare child value vs tmp value
                    if (res> beta) {
                        EvalDict1[boardToEvaluate] = res;
                        return res;
                    }
                }
            }

            // 現在のノードが相手側の場合
            else {
                res = INF;
                foreach(ulong put in puts) {
                    newBoard = new Board(boardToEvaluate);
                    newBoard.UpdateBoard(put);
                    tmp_value = EvaluationValue(newBoard,nowDepth +1 ,alpha,beta);
                    if (tmp_value < res) {
                        res = tmp_value;
                        beta = res;
                    }
                    
                    if (res < alpha) {
                        EvalDict1[boardToEvaluate] = res;
                        return res;
                    }
                    //res = System.Math.Min(res, EvaluationValue(newBoard, nowDepth + 1, alpha, beta));
                }
            }
            EvalDict1[boardToEvaluate] = res;
            return res;
    }



    double EvaluationValue_Memo_Open(Board origBoard,ulong recput,Board boardToEvaluate, int nowDepth,double alpha,double beta) {
            double res;
            const double INF = 1e9;
            double tmp_value;
            Board newBoard;
            if (EvalDict1.ContainsKey(boardToEvaluate)) {
                return EvalDict1[boardToEvaluate];
            }

            PlayerInformation playerInformation = blackInformation;
            if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
            int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);

            // 再帰の終了条件: 読みの深さが上限に達した場合
            if(nowDepth == maxDepth) {
                res =  boardToEvaluate.CalcDifferenceNumberOfHands()    * playerInformation.WeightNumberOfHands
                    + boardToEvaluate.CalcDifferenceSettledStone()     * playerInformation.WeightNumberOfSettledStones
                    - boardToEvaluate.CountDifferenceDangerousHands()  * playerInformation.WeightDangerousHands
                    + boardToEvaluate.CalcDifferenceCellPoints()       * playerInformation.WeightCellPoints
                    - boardToEvaluate.CalcOpenness(recput,origBoard) *1000;
                if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1.0;
                EvalDict1[boardToEvaluate] = res;
                return res;
            }
            
            List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
            playerInformation.LastNumberOfHands.Add(puts.Count);

            // 現在のノードが手番側の場合
            if(boardToEvaluate.NowTurn == board.NowTurn) {
                res = -INF;
                foreach(ulong put in puts) {
                    newBoard = new Board(boardToEvaluate);
                    newBoard.UpdateBoard(put);
                    tmp_value = EvaluationValue_Memo_Open(boardToEvaluate,recput,newBoard,nowDepth + 1, alpha,beta);
                    if (tmp_value > res) {
                        res = tmp_value;
                        alpha = res;
                    }
                    //res = System.Math.Max(res, EvaluationValue(newBoard, nowDepth + 1, alpha, beta));//compare child value vs tmp value
                    if (res> beta) {
                        EvalDict1[boardToEvaluate] = res;
                        return res;
                    }
                }
            }

            // 現在のノードが相手側の場合
            else {
                res = INF;
                foreach(ulong put in puts) {
                    newBoard = new Board(boardToEvaluate);
                    newBoard.UpdateBoard(put);
                    tmp_value = EvaluationValue_Memo_Open(boardToEvaluate,recput,newBoard,nowDepth +1 ,alpha,beta);
                    if (tmp_value < res) {
                        res = tmp_value;
                        beta = res;
                    }
                    
                    if (res < alpha) {
                        EvalDict1[boardToEvaluate] = res;
                        return res;
                    }
                    //res = System.Math.Min(res, EvaluationValue(newBoard, nowDepth + 1, alpha, beta));
                }
            }
            EvalDict1[boardToEvaluate] = res;
            return res;
    }



    /// <summary>
    /// PlayerInformationから読みの深さの上限を計算する
    /// </summary>
    /// <param name="playerInformation">手番側のPlayerInformation</param>
    /// <returns>読みの深さの上限</returns>
    int CalcMaxDepthFromPlayerInformation(PlayerInformation playerInformation) {
        return System.Math.Max(1, (int)(System.Math.Log(playerInformation.MaxNodes, 1 + playerInformation.LastAvgNumberOfHands)));
    }

    /// <summary>
    /// 盤面からAIの着手を求める
    /// </summary>
    /// <param name="board">盤面</param>
    /// <param name="AIInformation">AIのPlayerInformation</param>
    /// <returns>AIの着手位置にフラグが立っている64ビット</returns>
    ulong GetAIPutFromBoard(Board board, PlayerInformation AIInformation) {
        const double INF = 1e9;
        List<ulong> puts = board.MakePlayerLegalPutList();
        AIInformation.LastNumberOfHands.Add(puts.Count);

        double[] evals = new double[puts.Count];
        //double bestEval = -INF;
        bestEval = -INF;
        for (int i = 0; i < puts.Count; ++i) {
            Board newBoard = new Board(board);
            newBoard.UpdateBoard(puts[i]);
            evals[i] = EvaluationValue_Memo_Open(board,puts[i],newBoard, 1, -INF, INF);
            if (evals[i] > bestEval) bestEval = evals[i]; 
        }
        AIInformation.UpdateLastAvgNumberOfHands();
        List<ulong> bestPuts = new List<ulong>();
        for (int i = 0; i < puts.Count; ++i) {
            if (evals[i] == bestEval) bestPuts.Add(puts[i]);
        }
        int bi = Random.Range(0, bestPuts.Count);
        //System.Console.WriteLine(bestEval);
        return bestPuts[bi];
    }

    ulong GetAIPutFromBoard_updated(Board board, PlayerInformation AIInformation) {
        ulong blackBoard = board.PlayerBoard;
        ulong whiteBoard = board.OpponentBoard;
        if (board.NowTurn == Board.WhiteTurn) {
            blackBoard = board.OpponentBoard;
            whiteBoard = board.PlayerBoard;
        }
        int num_of_stones = board.BitCount(board.PlayerBoard) + board.BitCount(board.OpponentBoard);
        //Board nowBoard = new Board(board);
        int rest_depth = 64 - num_of_stones;
        if (rest_depth <= 12) {
            const double INF = 1e9;
            List<ulong> puts = board.MakePlayerLegalPutList();
            //AIInformation.LastNumberOfHands.Add(puts.Count);

            double[] evals = new double[puts.Count];
            //double bestEval = -INF;
            bestEval = -INF;
            for (int i = 0; i < puts.Count; ++i) {
                Board newBoard = new Board(board);
                newBoard.UpdateBoard(puts[i]);
                evals[i] = EvaluationValue_fullsearch(newBoard, 1 ,rest_depth);
                if (evals[i] > bestEval) bestEval = evals[i]; 
            }
            //AIInformation.UpdateLastAvgNumberOfHands();
            List<ulong> bestPuts = new List<ulong>();
            for (int i = 0; i < puts.Count; ++i) {
                if (evals[i] == bestEval) bestPuts.Add(puts[i]);
            }
            int bi = Random.Range(0, bestPuts.Count);
            //System.Console.WriteLine(bestEval);
            return bestPuts[bi];            
        }
        else {
        const double INF = 1e9;
        List<ulong> puts = board.MakePlayerLegalPutList();
        AIInformation.LastNumberOfHands.Add(puts.Count);

        double[] evals = new double[puts.Count];
        //double bestEval = -INF;
        bestEval = -INF;
        for (int i = 0; i < puts.Count; ++i) {
            Board newBoard = new Board(board);
            newBoard.UpdateBoard(puts[i]);
            evals[i] = EvaluationValue_Memo_Open(board,puts[i],newBoard, 1, -INF, INF);
            if (evals[i] > bestEval) bestEval = evals[i]; 
        }
        AIInformation.UpdateLastAvgNumberOfHands();
        List<ulong> bestPuts = new List<ulong>();
        for (int i = 0; i < puts.Count; ++i) {
            if (evals[i] == bestEval) bestPuts.Add(puts[i]);
        }
        int bi = Random.Range(0, bestPuts.Count);
        //System.Console.WriteLine(bestEval);
        return bestPuts[bi];
        }
    }//add full-search option

    /// <summary>
    /// 盤面を表す文字列からAIの着手を座標として求める
    /// </summary>
    /// <param name="strBoard">
    /// 盤面を表す文字列
    /// #: 黒石, O: 白石, -: 空きマス
    /// 例: 初期配置
    /// --------
    /// --------
    /// --------
    /// ---O#---
    /// ---#O---
    /// --------
    /// --------
    /// --------
    /// </param>
    /// <param name="AIIsBlack">AIが黒番ならtrue</param>
    /// <param name="AIInformation">AIのPlayerInformation</param>
    /// <returns>AIの着手を座標として出力 (例: F5)</returns>
    string GetAIPutAsCoordinateFromStrBoard(string strBoard, bool AIIsBlack, PlayerInformation AIInformation) {
        Board board = StrBoardToBoard(strBoard, AIIsBlack);
        //var sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        ulong put = GetAIPutFromBoard_updated(board, AIInformation);
        //sw.Stop();
        //Debug.Log("elapsed "+sw.ElapsedMilliseconds);
        string coordinate = BitToCoordinate(put);
        return coordinate;
    }//changed

    /// <summary>
    /// 盤面を表す文字列をBoardクラスに変換
    /// </summary>
    /// <param name="strBoard">盤面を表す文字列</param>
    /// <param name="AIIsBlack">AIが黒番ならtrue</param>
    /// <returns>盤面を表すBoardクラス</returns>
    Board StrBoardToBoard(string strBoard, bool AIIsBlack) {
        // #, O, -を数え, 合計が64マスでなければerror
        int cntCells = 0;
        for (int i = 0; i < strBoard.Length; ++i) {
            if (strBoard[i] == '#' || strBoard[i] == 'O' || strBoard[i] == '-') ++cntCells;
        }
        Debug.Assert(cntCells == 64);

        ulong blackBoard = 0;
        ulong whiteBoard = 0;
        ulong curCell = 0x8000000000000000;
        for (int i = 0; i < strBoard.Length; ++i) {
            switch (strBoard[i]) {
                case '#':
                    blackBoard += curCell;
                    curCell >>= 1;
                    break;
                case 'O':
                    whiteBoard += curCell;
                    curCell >>= 1;
                    break;
                case '-':
                    curCell >>= 1;
                    break;
                default:
                    break;
            }
        }

        Board board = new Board();
        if (AIIsBlack) {
            board.NowTurn       = Board.BlackTurn;
            board.PlayerBoard   = blackBoard;
            board.OpponentBoard = whiteBoard;
        } else {
            board.NowTurn       = Board.WhiteTurn;
            board.PlayerBoard   = whiteBoard;
            board.OpponentBoard = blackBoard;
        }
        board.GameFinished = board.IsGameFinished();
        return board;
    }

    /// <summary>
    /// bitを座標に変換
    /// </summary>
    /// <param name="bit">盤上のただ1マスにのみフラグが立っている64ビット</param>
    /// <returns>座標</returns>
    string BitToCoordinate(ulong bit) {
        // フラグが立っている行, 列を求める
        ulong mask = 0x8000000000000000;
        int cnt = 0;
        int row = -1;
        int clm = -1;
        while (mask > 0) {
            if (mask == bit) {
                row = cnt / 8;
                clm = cnt % 8;
            }
            mask >>= 1;
            ++cnt;
        }
        Debug.Assert(row != -1 && clm != -1);

        string[] strRows = {"1", "2", "3", "4", "5", "6", "7", "8"};
        string[] strClms = {"A", "B", "C", "D", "E", "F", "G", "H"};
        string coordinate = "";
        coordinate += strClms[clm];
        coordinate += strRows[row];
        return coordinate;
    }
}
