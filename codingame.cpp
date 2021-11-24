#include <iostream>
#include <map>
#include <tuple>
#include <string>
#include <vector>
using namespace std;
using ulong = unsigned long;

class Player
{
    Board *board = new Board;//static?
    //PlayerInformation* blackInformation;
    //PlayerInformation* whiteInformation;
    //PlayerInformation* player1Information;
    //PlayerInformation* player2Information;
    const bool      isHuman1                        = false;
    const long      MaxNodes1                       = 100;
    const double    WeightNumberOfHands1            = 0.3;
    const double    WeightNumberOfSettledStones1    = 1;
    const double    WeightDangerousHands1           = 2;
    const double    WeightCellPoints1               = 0.2;
    const bool      isHuman2                        = true;
    const long      MaxNodes2                       = 1000;
    const double    WeightNumberOfHands2            = 0.3;
    const double    WeightNumberOfSettledStones2    = 1;
    const double    WeightDangerousHands2           = 2;
    
    const double    WeightCellPoints2               = 0.5;
    const int maxGames = 1;
    const int CntGames = 0;
    double bestEval = -(1e9);
    static map<Board,double> EvalDict1;
    static map<Board,double> EvalDict2;
    map<tuple<int, int, int>, int> hoge;
    static map <tuple<ulong,ulong>,ulong> BlackDict;
    static map <tuple<ulong,ulong>,ulong> WhiteDict;
    static void Main()
    {
        //Random rand = new Random();
        Player *player = new Player;
        //int player_id = int.Parse(Console.ReadLine()); // id of your player.
        int player_id;
        cin>>player_id;
        int opponent_id = player_id ^ 1;
        //char player_char = Convert.ToChar(player_id+48);
        char player_char = '0' + player_id;
        char opponent_char = '0' + opponent_id;
        //char opponent_char = Convert.ToChar(opponent_id+48);
        //string player_st = player_id.ToString();
        //string opponent_st = opponent_id.ToString();
        int boardSize;
        cin>>boardSize;
        ulong playerboard = 0x0000000000000000;
        ulong opponentboard = 0x0000000000000000;
        ulong mask = 0x8000000000000000;

        PlayerInformation *player1Information = new PlayerInformation(player->isHuman1, player->MaxNodes1, player->WeightNumberOfHands1, player->WeightNumberOfSettledStones1, player->WeightDangerousHands1,player->WeightCellPoints1);
        PlayerInformation *player2Information = new PlayerInformation(player->isHuman2, player->MaxNodes2, player->WeightNumberOfHands2, player->WeightNumberOfSettledStones2, player->WeightDangerousHands2,player->WeightCellPoints2);
                
        if (player_id==0) {
            PlayerInformation *blackInformation = player1Information;
            PlayerInformation *whiteInformation = player2Information;
        }
        else {
            PlayerInformation *blackInformation = player2Information;
            PlayerInformation *whiteInformation = player1Information;
        }

        while (true)
        {
            playerboard = 0x0000000000000000;
            opponentboard = 0x0000000000000000;
            for (int i = 0; i < 8; i++)
            {
                string line;
                cin>>line;
                for (int j = 0; j < 8; j++) {
                    if (line[j]==player_char) {
                        playerboard |= (mask>>(8*i+j));
                    }
                    else if (line[j]==opponent_char) {
                        opponentboard |= (mask>>(8*i+j));
                    }
                }
            }
            int actionCount;
            cin>>actionCount;
            vector<string> possible_actions(actionCount);
            for (int i = 0; i < actionCount; i++)
            {
                cin>>possible_actions[i];
            }
            board.PlayerBoard = playerboard;
            board.OpponentBoard = opponentboard;
            board.GameFinished = false;
            if (player_id == 0) {
                board.NowTurn = 100;
            }
            else {
                board.NowTurn = -100;
            }
            ulong put = player.AIPlay(); 
            string decided_action = player.PutToIO(put);
            cout<<decided_action<<endl;
        }
    }
    ulong IOToPut(string IO) {
        int x = (int)IO[0] - 96;//1~8;
        //int y = int.Parse(IO[1]);//1~8;
        int y = (int)Char.GetNumericValue(IO[1]);
        int bit_number = 72 - x - 8*y;
        ulong put = ((ulong) 1)<<bit_number;
        
        return put;
    }
    string PutToIO(ulong put) {
        ulong mask = 0x8000000000000000;
        int bit_number=0;
        for (int i=0;i<64;i++) {
            if ((put & mask) >0) {
                bit_number = 63-i;
                break;
            }
            mask>>=1;
        }
        int xd = (63-bit_number)%8;
        int yd = (63-bit_number)/8;
        string IO = ((char)(xd+97)).ToString() + (yd+1).ToString();
        return IO;
    }
    ulong AIPlay() {
        PlayerInformation AIInformation = blackInformation;
        if(board.NowTurn == Board.WhiteTurn) AIInformation = whiteInformation;
        ulong put = GetAIPutFromBoard_updated(board, AIInformation);
        //board.UpdateBoard(put);//remove
        return put;
    }



    int EvaluationValue_fullsearch_new(Board boardToEvaluate, int rest_depth,int alpha,int beta) {
        int res;
        int INF = 70;
        int tmp_value;
        //Board newBoard;
        //PlayerInformation playerInformation = blackInformation;
        //if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(rest_depth==0) {
            //res = board.BitCount(boardToEvaluate.PlayerBoard);//added
            res = board.BitCount(boardToEvaluate.PlayerBoard) - board.BitCount(boardToEvaluate.OpponentBoard);
            
            if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1;
            return res;
        }
        List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
        if (puts.Count==0) {
            res = board.BitCount(boardToEvaluate.PlayerBoard) - board.BitCount(boardToEvaluate.OpponentBoard);
            if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1;
                return res;   
        }
        //playerInformation.LastNumberOfHands.Add(puts.Count);
    // 現在のノードが手番側の場合
        if(boardToEvaluate.NowTurn == board.NowTurn) {
            res = -INF;
            foreach(ulong put in puts) {
                Board newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue_fullsearch_new(newBoard,rest_depth-1,alpha,beta);
                if (tmp_value > res) {
                    res = tmp_value;
                    alpha = res;
                }
                if (res > beta) {
                    return res;
                }
            }
        }
        else {
            res = INF;
            foreach(ulong put in puts) {
                Board newBoard = new Board(boardToEvaluate);
                newBoard.UpdateBoard(put);
                tmp_value = EvaluationValue_fullsearch_new(newBoard,rest_depth-1,alpha,beta);
                if (tmp_value < res) {
                    res = tmp_value;
                    beta = res;
                }
                if (res < alpha) {
                    return res;
                }
            }
        }
        return res;
    }

    double EvaluationValue(Board boardToEvaluate, int nowDepth,double alpha,double beta) {
        double res;
        const double INF = 1e9;
        double tmp_value;
        Board newBoard;

        PlayerInformation playerInformation = blackInformation;
        if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
        //int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation) ;
        int maxDepth = 4;
        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(nowDepth == maxDepth) {
            //Console.Error.WriteLine(maxDepth);
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
                if (res > beta) {
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
            //int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);
            //maxDepth = 4;
            int maxDepth = 4;
            //Console.Error.WriteLine(maxDepth);
            //Console.Error.WriteLine("maxDepth "+ maxDepth.ToString());

            // 再帰の終了条件: 読みの深さが上限に達した場合
            if(nowDepth == maxDepth) {
                res =  boardToEvaluate.CalcDifferenceNumberOfHands()    * playerInformation.WeightNumberOfHands
                    + boardToEvaluate.CalcDifferenceSettledStone()     * playerInformation.WeightNumberOfSettledStones
                    - boardToEvaluate.CountDifferenceDangerousHands()  * playerInformation.WeightDangerousHands
                    + boardToEvaluate.CalcDifferenceCellPoints()       * playerInformation.WeightCellPoints
                    - boardToEvaluate.CalcOpenness(recput,origBoard) *0.3;
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

    int CalcMaxDepthFromPlayerInformation(PlayerInformation playerInformation) {
        //Console.Error.WriteLine(playerInformation.MaxNodes);
        //Console.Error.WriteLine(1+playerInformation.LastAvgNumberOfHands);
        return System.Math.Max(1, (int)(System.Math.Log(playerInformation.MaxNodes, 1 + playerInformation.LastAvgNumberOfHands)));
    }

    ulong GetAIPutFromBoard_updated(Board board, PlayerInformation AIInformation) {
        ulong blackBoard = board.PlayerBoard;
        ulong whiteBoard = board.OpponentBoard;

        if (board.NowTurn == Board.WhiteTurn) {
            blackBoard = board.OpponentBoard;
            whiteBoard = board.PlayerBoard;
        }
        int num_of_stones = BitOperations.PopCount(board.PlayerBoard) + BitOperations.PopCount(board.OpponentBoard);
        int rest_depth = 64 - num_of_stones;
        if (rest_depth <= 10) {
            const double INF = 1e9;
            List<ulong> puts = board.MakePlayerLegalPutList();
            //AIInformation.LastNumberOfHands.Add(puts.Count);
            double[] evals = new double[puts.Count];
            bestEval = -INF;
            for (int i = 0; i < puts.Count; ++i) {
                Board newBoard = new Board(board);
                newBoard.UpdateBoard(puts[i]);
                evals[i] = EvaluationValue_fullsearch_new(newBoard,rest_depth-1,-80,80);
                if (evals[i] > bestEval) bestEval = evals[i]; 
            }
            //AIInformation.UpdateLastAvgNumberOfHands();
            List<ulong> bestPuts = new List<ulong>();
            for (int i = 0; i < puts.Count; ++i) {
                if (evals[i] == bestEval) bestPuts.Add(puts[i]);
            }
            Random rand = new Random();
            int bi = rand.Next(0,bestPuts.Count);
            return bestPuts[bi];            
        }
        else if (rest_depth <= 35) {
            const double INF = 1e9;
            List<ulong> puts = board.MakePlayerLegalPutList();
            //AIInformation.LastNumberOfHands.Add(puts.Count);
            //AIInformation.UpdateLastAvgNumberOfHands();
            double[] evals = new double[puts.Count];
            bestEval = -INF;
            for (int i = 0; i < puts.Count; ++i) {
                Board newBoard = new Board(board);
                newBoard.UpdateBoard(puts[i]);
                evals[i] = EvaluationValue(newBoard, 1, -INF, INF);
                if (evals[i] > bestEval) bestEval = evals[i]; 
            }
            List<ulong> bestPuts = new List<ulong>();
            for (int i = 0; i < puts.Count; ++i) {
                if (evals[i] == bestEval) bestPuts.Add(puts[i]);
            }
            //AIInformation.UpdateLastAvgNumberOfHands();
            Random rand = new Random();
            int bi = rand.Next(0,bestPuts.Count);
            return bestPuts[bi];
        }
        else {

            const double INF = 1e9;
            List<ulong> puts = board.MakePlayerLegalPutList();
            AIInformation.LastNumberOfHands.Add(puts.Count);
            //AIInformation.UpdateLastAvgNumberOfHands();
            if (board.NowTurn == Board.BlackTurn) {
                Console.Error.WriteLine("black");
                var check = System.Tuple.Create(blackBoard,whiteBoard);
                if (BlackDict.ContainsKey(check)) {
                    Console.Error.WriteLine("hit b");
                    return BlackDict[check];
                }
            }
            else {
                Console.Error.WriteLine("white");
                var check = System.Tuple.Create(whiteBoard,blackBoard);
                if (WhiteDict.ContainsKey(check)) {
                    Console.Error.WriteLine("hit w");
                    return WhiteDict[check];
                }
            }
            double[] evals = new double[puts.Count];
            //double bestEval = -INF;
            bestEval = -INF;
            for (int i = 0; i < puts.Count; ++i) {
                Board newBoard = new Board(board);
                newBoard.UpdateBoard(puts[i]);
                evals[i] = EvaluationValue_Memo_Open(board,puts[i],newBoard, 1, -INF, INF);
                if (evals[i] > bestEval) bestEval = evals[i]; 
            }
            //AIInformation.UpdateLastAvgNumberOfHands();
            List<ulong> bestPuts = new List<ulong>();
            for (int i = 0; i < puts.Count; ++i) {
                if (evals[i] == bestEval) bestPuts.Add(puts[i]);
            }
            //AIInformation.UpdateLastAvgNumberOfHands();
            Random rand = new Random();
            int bi = rand.Next(0,bestPuts.Count);
            return bestPuts[bi];
        }
    }//add full-search option
}

class PlayerInformation
{
    public:
    bool IsHuman{ get; set; }
    long MaxNodes{ get; set; }
    double LastAvgNumberOfHands{ get; set; }
    List<double> LastNumberOfHands{ get; set; }
    double WeightNumberOfHands{ get; set; }
    double WeightNumberOfSettledStones{ get; set; }
    double WeightDangerousHands{ get; set; }
    double WeightCellPoints{ get; set; }

    PlayerInformation(bool IsHuman, long MaxNodes, double WeightNumberOfHands, double WeightNumberOfSettledStones, double WeightDangerousHands, double WeightCellPoints) {
        this.IsHuman = IsHuman;
        this.MaxNodes = MaxNodes;
        this.LastAvgNumberOfHands = 10.0;
        this.LastNumberOfHands = new List<double>();
        this.WeightNumberOfHands            = WeightNumberOfHands;
        this.WeightNumberOfSettledStones    = WeightNumberOfSettledStones;
        this.WeightDangerousHands           = WeightDangerousHands;
        this.WeightCellPoints               = WeightCellPoints;
    }

    PlayerInformation(PlayerInformation playerInformation) {
        this.IsHuman                        = playerInformation.IsHuman;
        this.MaxNodes                       = playerInformation.MaxNodes;
        this.LastAvgNumberOfHands           = playerInformation.LastAvgNumberOfHands;
        this.LastNumberOfHands              = playerInformation.LastNumberOfHands;
        this.WeightNumberOfHands            = playerInformation.WeightNumberOfHands;
        this.WeightNumberOfSettledStones    = playerInformation.WeightNumberOfSettledStones;
        this.WeightDangerousHands           = playerInformation.WeightDangerousHands;
        this.WeightCellPoints               = playerInformation.WeightCellPoints;
    }

    void UpdateLastAvgNumberOfHands() {
        LastAvgNumberOfHands = 1.0;
        int n = LastNumberOfHands.Count;
        foreach(double x in LastNumberOfHands) {
            LastAvgNumberOfHands *= System.Math.Pow(x, 1.0 / n);
        }
        LastNumberOfHands.Clear();
    }
}

class Board {
    public:
    // MARK: Constant
    const int BlackTurn       =  100;
    const int WhiteTurn       = -100;

    const int NumCell = 64;

    // MARK: Properties
    int NowTurn{ get; set; }
    bool GameFinished{ get; set; }
    ulong PlayerBoard{ get; set; }
    ulong OpponentBoard{ get; set; }

    // MARK: Initialization
    Board() {
        this.NowTurn    = BlackTurn;
        this.GameFinished = false;

        // 一般的な初期配置を指定
        this.PlayerBoard    = 0x0000000810000000;
        this.OpponentBoard  = 0x0000001008000000;
    }

    // MARK: Copy Constructor
    Board(Board brd) {
        this.NowTurn        = brd.NowTurn;
        this.GameFinished   = brd.GameFinished;
        this.PlayerBoard    = brd.PlayerBoard;
        this.OpponentBoard  = brd.OpponentBoard;
    }
  
    bool CanPut(ulong put) {
        ulong legalBoard = MakePlayerLegalBoard();
        return (put & legalBoard) == put;
    }

    void Reverse(ulong put) {
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
    
    bool IsPass() {
        // 手番側の合法手ボードを生成
        ulong playerLegalBoard = MakePlayerLegalBoard();

        // 相手側の合法手ボードを生成
        ulong opponentLegalBoard = MakeOpponentLegalBoard();

        // 手番側だけがパスの場合
        return playerLegalBoard == 0x0000000000000000 && opponentLegalBoard != 0x0000000000000000;
    }

    bool IsGameFinished() {
        ulong playerLegalBoard = MakePlayerLegalBoard();
        ulong opponentLegalBoard = MakeOpponentLegalBoard();
        // 両手番とも置く場所がない場合
        return playerLegalBoard == 0x0000000000000000 && opponentLegalBoard == 0x0000000000000000;
    }

    void SwapBoard() {
        // ボードの入れ替え
        ulong tmp = PlayerBoard;
        PlayerBoard = OpponentBoard;
        OpponentBoard = tmp;
        // 色の入れ替え
        NowTurn *= -1;
    }

    ulong Transfer(ulong put, int k) {
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
    ulong MakePlayerLegalBoard() {
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

    ulong MakeOpponentLegalBoard() {
        SwapBoard();
        ulong opponentLegalBoard = MakePlayerLegalBoard();
        SwapBoard();
        return opponentLegalBoard;
    }

    List<ulong> MakePlayerLegalPutList() {
        List<ulong> legalPutList = new List<ulong>();
        ulong put = 0x8000000000000000;
        ulong legalBoard = MakePlayerLegalBoard();
        while (put > 0) {
            if ((put & legalBoard) > 0) legalPutList.Add(put);
            put >>= 1;
        }
        return legalPutList;
    }

    void UpdateBoard(ulong put) {
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

    int BitCount(ulong put) {
        return BitOperations.PopCount(put);
    }

    int CalcOpenness(ulong put,Board origboard) {
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
        return res;        
    }

    int CalcPlayerNumberOfHands() {
        ulong playerLegalBoard = MakePlayerLegalBoard();
        int playerNumberOfHands = BitCount(playerLegalBoard);
        return playerNumberOfHands;
    }

    int CalcOpponentNumberOfHands() {
        SwapBoard();
        int opponentNumberOfHands = CalcPlayerNumberOfHands();
        SwapBoard();
        return opponentNumberOfHands;
    }

    int CalcDifferenceNumberOfHands() {
        return CalcPlayerNumberOfHands() - CalcOpponentNumberOfHands();
    }

    int CalcPlayerSettledStone() {
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

    int CalcOpponentSettledStone() {
        SwapBoard();
        int opponentSettledStone = CalcPlayerSettledStone();
        SwapBoard();
        return opponentSettledStone;
    }
  
    int CalcDifferenceSettledStone() {
        return CalcPlayerSettledStone() - CalcOpponentSettledStone();
    }

    int CalcPlayerCellPoints() {
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

    int CalcOpponentCellPoints() {
        SwapBoard();
        int res = CalcPlayerCellPoints();
        SwapBoard();
        return res;
    }
    
    int CalcDifferenceCellPoints() {
        return CalcPlayerCellPoints() - CalcOpponentCellPoints();
    }

    int CountPlayerDangerousHands() {
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

    int CountOpponentDangerousHands() {
        SwapBoard();
        int res = CountPlayerDangerousHands();
        SwapBoard();
        return res;
    }

    int CountDifferenceDangerousHands() {
        return CountPlayerDangerousHands() - CountOpponentDangerousHands();
    }

    bool IsSettledStone(ulong judgedStone, ulong settledStoneBoard) {
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
}