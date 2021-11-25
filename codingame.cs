using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
class Player
{
    static Board board = new Board();//static?
    static PlayerInformation blackInformation;
    static PlayerInformation whiteInformation;
    static PlayerInformation player1Information;
    static PlayerInformation player2Information;
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
    static Dictionary<Board,double> EvalDict1 = new Dictionary<Board, double>();
    static Dictionary<Board,double> EvalDict2 = new Dictionary<Board, double>();

    static Dictionary <System.Tuple<ulong,ulong>,ulong> BlackDict = new Dictionary <System.Tuple<ulong,ulong>,ulong>();
    static Dictionary <System.Tuple<ulong,ulong>,ulong> WhiteDict = new Dictionary <System.Tuple<ulong,ulong>,ulong>();
    static Dictionary <ulong,string> PutTOOutput = new Dictionary <ulong,string>();
    static void Main(string[] args)
    {
        Random rand = new Random();
        var player = new Player();
        int player_id = int.Parse(Console.ReadLine()); // id of your player.
        int opponent_id = player_id ^ 1;
        char player_char = Convert.ToChar(player_id+48);
        char opponent_char = Convert.ToChar(opponent_id+48);
        string player_st = player_id.ToString();
        string opponent_st = opponent_id.ToString();
        int boardSize = int.Parse(Console.ReadLine());
        ulong playerboard = 0x0000000000000000;
        ulong opponentboard = 0x0000000000000000;
        ulong mask = 0x8000000000000000;

        BlackDict.Add(Tuple.Create((ulong) 0x810000000 , (ulong)0x1008000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100810000000 , (ulong)0x201008000000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3010000000 , (ulong)0x380808000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20003010000000 , (ulong)0x780808000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2810000000 , (ulong)0x201008000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101800000000 , (ulong)0x202038000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x501800000000 , (ulong)0x20202038000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x814000000 , (ulong)0x1008040000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x18080000 , (ulong)0x1c04040000), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x180a0000 , (ulong)0x1c04040400), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x810080000 , (ulong)0x1008040000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80c000000 , (ulong)0x10101c0000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80c000400 , (ulong)0x10101e0000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20202000000000 , (ulong)0x181838000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x700000000000 , (ulong)0x83838000000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0xe0000 , (ulong)0x1c1c100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x4040400 , (ulong)0x1c18180000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20203000000000 , (ulong)0x180818200000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x701000000000 , (ulong)0x42838000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80e0000 , (ulong)0x1c14200000), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0xc040400 , (ulong)0x41810180000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20202000000000 , (ulong)0x181818100000), (ulong)0x8000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x700000000000 , (ulong)0x3c38000000), (ulong)0x40000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0xe0000 , (ulong)0x1c3c000000), (ulong)0x200000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x4040400 , (ulong)0x81818180000), (ulong)0x1000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10100010000000 , (ulong)0x287808000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x6800000000 , (ulong)0x10301038000000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x16000000 , (ulong)0x1c080c0800), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x800080800 , (ulong)0x101e140000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10003010000000 , (ulong)0x20380808000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x105800000000 , (ulong)0x602038000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1a080000 , (ulong)0x1c04060000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80c000800 , (ulong)0x10101c0400), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10100000000000 , (ulong)0x283838000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x6000000000 , (ulong)0x381838000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x6000000 , (ulong)0x1c181c0000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80800 , (ulong)0x1c1c140000), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x8100010000000 , (ulong)0x287808000000), (ulong)0x400000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2840000000 , (ulong)0x10301038000000), (ulong)0x20000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x214000000 , (ulong)0x1c080c0800), (ulong)0x400 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x800081000 , (ulong)0x101e140000), (ulong)0x20000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x4081000000000 , (ulong)0x302838000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1020400000 , (ulong)0x382818000000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20408000000 , (ulong)0x18141c0000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x8102000 , (ulong)0x1c140c0000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x42800000000 , (ulong)0x381018100000), (ulong)0x400000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100010200000 , (ulong)0x203c28000000), (ulong)0x20000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40800080000 , (ulong)0x143c040000), (ulong)0x400 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x14200000 , (ulong)0x818081c0000), (ulong)0x20000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c10000000 , (ulong)0x382048000000), (ulong)0x8000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1810100000 , (ulong)0x8302028000000), (ulong)0x40000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80818000000 , (ulong)0x14040c1000), (ulong)0x200000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x838000000 , (ulong)0x12041c0000), (ulong)0x1000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0xc00000000 , (ulong)0x383038000000), (ulong)0x8000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10100000 , (ulong)0x383828000000), (ulong)0x40000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80800000000 , (ulong)0x141c1c0000), (ulong)0x200000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x30000000 , (ulong)0x1c0c1c0000), (ulong)0x1000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2c00000000 , (ulong)0x381018100000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100010100000 , (ulong)0x203c28000000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80800080000 , (ulong)0x143c040000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x34000000 , (ulong)0x818081c0000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3410000000 , (ulong)0x38080c000000), (ulong)0x4000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101800100000 , (ulong)0x202038080000), (ulong)0x400000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80018080000 , (ulong)0x101c04040000), (ulong)0x20000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x82c000000 , (ulong)0x30101c0000), (ulong)0x2000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c000000 , (ulong)0x383820000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x808080000 , (ulong)0x383030000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101010000000 , (ulong)0xc0c1c0000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3800000000 , (ulong)0x41c1c0000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1018040000 , (ulong)0x382840000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1808040000 , (ulong)0x8302030000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201018000000 , (ulong)0xc040c1000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201808000000 , (ulong)0x2141c0000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x18040000 , (ulong)0x383820000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x808040000 , (ulong)0x383030000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201010000000 , (ulong)0xc0c1c0000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201800000000 , (ulong)0x41c1c0000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x818080000 , (ulong)0x10301000000000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1818080000 , (ulong)0x10302040000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c000000 , (ulong)0x207000000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x181c000000 , (ulong)0x8306000000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3810000000 , (ulong)0xe040000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3818000000 , (ulong)0x60c1000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101810000000 , (ulong)0x80c0800), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101818000000 , (ulong)0x2040c0800), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1818080000 , (ulong)0x10302020000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x181c000000 , (ulong)0x386000000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3818000000 , (ulong)0x61c0000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101818000000 , (ulong)0x4040c0800), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c000000 , (ulong)0x10301000000000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2000081c000000 , (ulong)0x10381000000000), (ulong)0x1000000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x818080000 , (ulong)0x207000000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x400818080000 , (ulong)0x207020000000), (ulong)0x8000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101810000000 , (ulong)0xe040000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101810020000 , (ulong)0x40e040000), (ulong)0x1000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3810000000 , (ulong)0x80c0800), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3810000400 , (ulong)0x81c0800), (ulong)0x8 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x181c000000 , (ulong)0x10302040000000), (ulong)0x1000000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1818080000 , (ulong)0x8306000000000), (ulong)0x8000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101818000000 , (ulong)0x60c1000), (ulong)0x1000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3818000000 , (ulong)0x2040c0800), (ulong)0x8 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x181c000000 , (ulong)0x10302020000000), (ulong)0x40000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1818080000 , (ulong)0x386000000000), (ulong)0x8000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101818000000 , (ulong)0x61c0000), (ulong)0x1000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3818000000 , (ulong)0x4040c0800), (ulong)0x200000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2814000000 , (ulong)0x10301008040000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100810080000 , (ulong)0x207008040000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100810080000 , (ulong)0x20100e040000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2814000000 , (ulong)0x2010080c0800), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101000181c000000 , (ulong)0x380000000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0xd818080000 , (ulong)0x202020000000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10181b000000 , (ulong)0x404040000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3818000808 , (ulong)0x1c0000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c000000 , (ulong)0x381000000000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x181c000000 , (ulong)0x382040000000), (ulong)0x4000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x818080000 , (ulong)0x203020000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1818080000 , (ulong)0x8302020000000), (ulong)0x400000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101810000000 , (ulong)0x40c040000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101818000000 , (ulong)0x4040c1000), (ulong)0x20000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3810000000 , (ulong)0x81c0000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3818000000 , (ulong)0x2041c0000), (ulong)0x2000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c000000 , (ulong)0x383020000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x818080000 , (ulong)0x383020000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101810000000 , (ulong)0x40c1c0000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3810000000 , (ulong)0x40c1c0000), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101800000000 , (ulong)0x38000000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100810200000 , (ulong)0x201028000000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3030200000 , (ulong)0x380808000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3010000000 , (ulong)0x80808000000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x42810000000 , (ulong)0x281008000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c1800000000 , (ulong)0x202038000000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80c000000 , (ulong)0x1010100000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x814200000 , (ulong)0x1008140000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x18380000 , (ulong)0x1c04040000), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x18080000 , (ulong)0x1c00000000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40810080000 , (ulong)0x1408040000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40c0c000000 , (ulong)0x10101c0000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100010200000 , (ulong)0x81828000000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2030200000 , (ulong)0x20181808000000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x42800000000 , (ulong)0x81038000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c0800000000 , (ulong)0x403038000000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x14200000 , (ulong)0x1c08100000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10380000 , (ulong)0x1c0c020000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40800080000 , (ulong)0x1418100000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40c04000000 , (ulong)0x1018180400), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2030200000 , (ulong)0x10181808000000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c0800000000 , (ulong)0x7038000000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10380000 , (ulong)0x1c0e000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40c04000000 , (ulong)0x1018180800), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100010100000 , (ulong)0x81828000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x140010100000 , (ulong)0x8081828000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2c00000000 , (ulong)0x81038000000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2c00200000 , (ulong)0x81078000000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x34000000 , (ulong)0x1c08100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40034000000 , (ulong)0x1e08100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80800080000 , (ulong)0x1418100000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80800280000 , (ulong)0x1418101000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c0010100000 , (ulong)0x1c28000000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2c20200000 , (ulong)0x81018100000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40434000000 , (ulong)0x81808100000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80800380000 , (ulong)0x1438000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100408100000 , (ulong)0x81830200000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2408100000 , (ulong)0xc1830000000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81024000000 , (ulong)0xc18300000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81020080000 , (ulong)0x40c18100000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100410100000 , (ulong)0x81828080000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2c00100000 , (ulong)0x8103c000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80034000000 , (ulong)0x3c08100000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x80820080000 , (ulong)0x101418100000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10001c100000 , (ulong)0x81c20000000), (ulong)0x8000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2c08080000 , (ulong)0x81030100000), (ulong)0x40000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101034000000 , (ulong)0x80c08100000), (ulong)0x200000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x83800080000 , (ulong)0x438100000), (ulong)0x1000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x808080000 , (ulong)0x10101030000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200808080000 , (ulong)0x10181030000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c000000 , (ulong)0x87800000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20001c000000 , (ulong)0x87820000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3800000000 , (ulong)0x1e100000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3800040000 , (ulong)0x41e100000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101010000000 , (ulong)0xc08080800), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101010040000 , (ulong)0xc08180800), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x818080000 , (ulong)0x10101020202000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c000000 , (ulong)0xe7000000000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3810000000 , (ulong)0xe700000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101810000000 , (ulong)0x4040408080800), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x808280000 , (ulong)0x10101030100000), (ulong)0x2000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x4001c000000 , (ulong)0x87c00000000), (ulong)0x20000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3800200000 , (ulong)0x3e100000), (ulong)0x400000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x141010000000 , (ulong)0x80c08080800), (ulong)0x4000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100808080000 , (ulong)0x81030000000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101800180000 , (ulong)0x8003c000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201c000000 , (ulong)0x81820000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3414000000 , (ulong)0x80828080000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3804000000 , (ulong)0x418100000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x282c000000 , (ulong)0x101410100000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101010080000 , (ulong)0xc08100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x180018080000 , (ulong)0x3c00100000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101800080000 , (ulong)0x3c000000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x102010080000 , (ulong)0x8182c000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3014000000 , (ulong)0x80808080000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x102804000000 , (ulong)0x81038080000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x280c000000 , (ulong)0x101010100000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2014080000 , (ulong)0x101c08100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100018080000 , (ulong)0x3c00000000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100804080000 , (ulong)0x3418100000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x103810000000 , (ulong)0x2c080800), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x103810000000 , (ulong)0x8000e080000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c080000 , (ulong)0x107000100000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c080000 , (ulong)0x10103400000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100010180000 , (ulong)0x8182c000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2c04000000 , (ulong)0x81038080000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2034000000 , (ulong)0x101c08100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x180800080000 , (ulong)0x3418100000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101810000000 , (ulong)0x2c180800), (ulong)0x40000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3810000000 , (ulong)0x8040e080000), (ulong)0x8000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x81c000000 , (ulong)0x107020100000), (ulong)0x1000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x818080000 , (ulong)0x10183400000000), (ulong)0x200000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101808040000 , (ulong)0x34081000), (ulong)0x800 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3018040000 , (ulong)0x80a04080000), (ulong)0x2000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20180c000000 , (ulong)0x102050100000), (ulong)0x4000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201018080000 , (ulong)0x8102c00000000), (ulong)0x10000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1008080c0000 , (ulong)0x81034000000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201c040000 , (ulong)0x81820080000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x203804000000 , (ulong)0x100418100000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x301010080000 , (ulong)0x2c08100000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101808000000 , (ulong)0x30080400), (ulong)0x40000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101070000000 , (ulong)0x80808080400), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3018000000 , (ulong)0x80804020000), (ulong)0x8000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x8083800000000 , (ulong)0x3c020000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x180c000000 , (ulong)0x402010100000), (ulong)0x1000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1c101000 , (ulong)0x403c00000000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x1018080000 , (ulong)0x20100c00000000), (ulong)0x200000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0xe08080000 , (ulong)0x20101010100000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101020400000 , (ulong)0x80818080400), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x4083000000000 , (ulong)0x83c020000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0xc102000 , (ulong)0x403c10000000), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20408080000 , (ulong)0x20101810100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101808200000 , (ulong)0x2030080400), (ulong)0x400000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x43018000000 , (ulong)0x180804020000), (ulong)0x4000000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x180c200000 , (ulong)0x402010180000), (ulong)0x2000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x41018080000 , (ulong)0x20100c04000000), (ulong)0x20000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100808080800 , (ulong)0x81030000400), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201e000000 , (ulong)0x81820020000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x7804000000 , (ulong)0x400418100000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10101010080000 , (ulong)0x20000c08100000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x808040000 , (ulong)0x10101030000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20100800040000 , (ulong)0x1000103c000000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x18040000 , (ulong)0x87800000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x402010040000 , (ulong)0x85808080000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201800000000 , (ulong)0x1e100000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200804020000 , (ulong)0x10101a100000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201010000000 , (ulong)0xc08080800), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200010080400 , (ulong)0x3c08000800), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201000040000 , (ulong)0x1010083c000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201000040000 , (ulong)0x86818080000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200008040000 , (ulong)0x101816100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200008040000 , (ulong)0x3c10080800), (ulong)0x20000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x808240000 , (ulong)0x10101030100000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x40018040000 , (ulong)0x87c00000000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201800200000 , (ulong)0x3e100000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x241010000000 , (ulong)0x80c08080800), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100808040000 , (ulong)0x81030000000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200808040000 , (ulong)0x10181030000000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2018040000 , (ulong)0x81820000000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200018040000 , (ulong)0x87820000000), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201804000000 , (ulong)0x418100000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201800040000 , (ulong)0x41e100000), (ulong)0x40000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201010080000 , (ulong)0xc08100000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201010040000 , (ulong)0xc08180800), (ulong)0x200000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x301008040000 , (ulong)0xc0830000000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x203008040000 , (ulong)0x80830200000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20100c040000 , (ulong)0x40c10100000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2010080c0000 , (ulong)0xc10300000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x301800040000 , (ulong)0x8003c000000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x203010040000 , (ulong)0x80828080000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20080c040000 , (ulong)0x101410100000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x2000180c0000 , (ulong)0x3c00100000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101008040000 , (ulong)0x40830000000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x10080c040000 , (ulong)0xc1030000000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3008040000 , (ulong)0x80810200000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20180c0000 , (ulong)0x81820200000), (ulong)0x200000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20100c000000 , (ulong)0x40810100000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x301804000000 , (ulong)0x40418100000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x201008080000 , (ulong)0xc10200000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x203010080000 , (ulong)0xc08300000), (ulong)0x40000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x101800040000 , (ulong)0x3c000000), (ulong)0x80000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x100010240000 , (ulong)0x8182c000000), (ulong)0x400000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x3010040000 , (ulong)0x80808080000), (ulong)0x4000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x42800040000 , (ulong)0x81038080000), (ulong)0x100000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x20080c000000 , (ulong)0x101010100000), (ulong)0x2000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200014200000 , (ulong)0x101c08100000), (ulong)0x80000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x200018080000 , (ulong)0x3c00000000), (ulong)0x100000000000 );
        BlackDict.Add(System.Tuple.Create((ulong) 0x240800080000 , (ulong)0x3418100000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8000000 , (ulong) 0x101810000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x200008000000 , (ulong) 0x103810000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x180808000000 , (ulong) 0x20203010000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x680808000000 , (ulong) 0x30103010000000), (ulong)0x800000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8000000 , (ulong) 0x3810000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x2038000000 , (ulong) 0x701800000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x20200038000000 , (ulong) 0x507800000000), (ulong)0x80000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1000000000 , (ulong) 0x81c000000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1000040000 , (ulong) 0x81c080000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c04000000 , (ulong) 0x180e0000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c00040400 , (ulong) 0x1e0a0000), (ulong)0x100000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1000000000 , (ulong) 0x818080000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1010180000 , (ulong) 0x80c040400), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1010160000 , (ulong) 0x80c080c00), (ulong)0x10 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x181818000000 , (ulong) 0x20202020200000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x3838000000 , (ulong) 0x7c0000000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c1c000000 , (ulong) 0x3e0000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1818180000 , (ulong) 0x40404040400), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81838000000 , (ulong) 0x28302000000000), (ulong)0x400000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81838000000 , (ulong) 0x702040000000), (ulong)0x20000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c18100000 , (ulong) 0x2040e0000), (ulong)0x400 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c18100000 , (ulong) 0x40c1400), (ulong)0x20000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x180038000000 , (ulong) 0x20203c00000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x82828000000 , (ulong) 0x701010100000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1414100000 , (ulong) 0x808080e0000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c00180000 , (ulong) 0x3c040400), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x80818200000 , (ulong) 0x30303000000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40838000000 , (ulong) 0x707000000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c10200000 , (ulong) 0xe0e0000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x41810100000 , (ulong) 0xc0c0c00), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x180808200000 , (ulong) 0x20203010100000), (ulong)0x80000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x42038000000 , (ulong) 0x701c00000000), (ulong)0x4000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c04200000 , (ulong) 0x380e0000), (ulong)0x2000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x41010180000 , (ulong) 0x8080c040400), (ulong)0x100000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x80818200000 , (ulong) 0x28303000000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40838000000 , (ulong) 0x703040000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c10200000 , (ulong) 0x20c0e0000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x41810100000 , (ulong) 0xc0c1400), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x818200000 , (ulong) 0x203c3000000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40818000000 , (ulong) 0x703020200000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1810200000 , (ulong) 0x4040c0e0000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x41810000000 , (ulong) 0xc3c0400), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81818100000 , (ulong) 0x28302000000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c38000000 , (ulong) 0x702040000000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c38000000 , (ulong) 0x2040e0000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81818100000 , (ulong) 0x40c1400), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1818100000 , (ulong) 0x203c2000000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c18000000 , (ulong) 0x702020200000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1838000000 , (ulong) 0x404040e0000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81818000000 , (ulong) 0x43c0400), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x280808000000 , (ulong) 0x10103010000000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x85808000000 , (ulong) 0x10702010000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x200038000000 , (ulong) 0x107800000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10001038000000 , (ulong) 0x20306800000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c00040000 , (ulong) 0x1e080000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c08000800 , (ulong) 0x160c0400), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1010140000 , (ulong) 0x80c080800), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101a100000 , (ulong) 0x8040e0800), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x285808000000 , (ulong) 0x10102050000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10201038000000 , (ulong) 0x8106800000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c08040800 , (ulong) 0x16081000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101a140000 , (ulong) 0xa04080800), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x207808000000 , (ulong) 0x10180410000000), (ulong)0x20000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10301018000000 , (ulong) 0x6820100000), (ulong)0x400000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x18080c0800 , (ulong) 0x80416000000), (ulong)0x20000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101e040000 , (ulong) 0x820180800), (ulong)0x400 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x20180808000000 , (ulong) 0x10207010000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x402038000000 , (ulong) 0x10305800000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c04020000 , (ulong) 0x1a0c0800), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1010180400 , (ulong) 0x80e040800), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x180808000000 , (ulong) 0x2030203010000000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x2038000000 , (ulong) 0xf05800000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c04000000 , (ulong) 0x1a0f0000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1010180000 , (ulong) 0x80c040c04), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x20280808000000 , (ulong) 0x18103010000000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x600038000000 , (ulong) 0x107840000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c00060000 , (ulong) 0x21e080000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1010140400 , (ulong) 0x80c081800), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x83838000000 , (ulong) 0x10700000000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x181838000000 , (ulong) 0x20206000000000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c18180000 , (ulong) 0x6040400), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c1c100000 , (ulong) 0xe0800), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x83838000000 , (ulong) 0x10304000000000), (ulong)0x400000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x181838000000 , (ulong) 0x10206000000000), (ulong)0x20000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c18180000 , (ulong) 0x6040800), (ulong)0x400 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c1c100000 , (ulong) 0x20c0800), (ulong)0x20000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x281838000000 , (ulong) 0x10102040000000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x281838000000 , (ulong) 0x8106000000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c18140000 , (ulong) 0x6081000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c18140000 , (ulong) 0x204080800), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x282828000000 , (ulong) 0x10101010100000), (ulong)0x80000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x380038000000 , (ulong) 0x7c00000000), (ulong)0x4000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c001c0000 , (ulong) 0x3e000000), (ulong)0x2000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1414140000 , (ulong) 0x80808080800), (ulong)0x100000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x203838000000 , (ulong) 0x101c0000000000), (ulong)0x20000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381818000000 , (ulong) 0x6020200000), (ulong)0x400000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x18181c0000 , (ulong) 0x40406000000), (ulong)0x20000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c1c040000 , (ulong) 0x380800), (ulong)0x400 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x203838000000 , (ulong) 0x10180400000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381818000000 , (ulong) 0x6020100000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x18181c0000 , (ulong) 0x80406000000), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c1c040000 , (ulong) 0x20180800), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x283038000000 , (ulong) 0x10100804000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381828000000 , (ulong) 0x6010080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x14181c0000 , (ulong) 0x100806000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c0c140000 , (ulong) 0x2010080800), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x280808000000 , (ulong) 0x8103010000000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x85808000000 , (ulong) 0x8702010000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x200038000000 , (ulong) 0x103840000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10001038000000 , (ulong) 0x20302840000000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c00040000 , (ulong) 0x21c080000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c08000800 , (ulong) 0x2140c0400), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1010140000 , (ulong) 0x80c081000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101a100000 , (ulong) 0x8040e1000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x300808000000 , (ulong) 0x4083010000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x302038000000 , (ulong) 0x4081c00000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202018000000 , (ulong) 0x101820400000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x382808000000 , (ulong) 0x1030500000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1804040000 , (ulong) 0x20418080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10141c0000 , (ulong) 0xa0c08000000), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10100c0000 , (ulong) 0x80c102000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c040c0000 , (ulong) 0x38102000), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x380008000000 , (ulong) 0x43810000000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1018100000 , (ulong) 0x7c2800000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202028000000 , (ulong) 0x101810200000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c08000000 , (ulong) 0x20302030200000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1404040000 , (ulong) 0x40818080000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1038000000 , (ulong) 0x40c040c0400), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10001c0000 , (ulong) 0x81c200000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81808000000 , (ulong) 0x143e0000), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381008100000 , (ulong) 0x42810200000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x203428000000 , (ulong) 0x140810200000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x142c040000 , (ulong) 0x40810280000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x810081c0000 , (ulong) 0x40814200000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x201018100000 , (ulong) 0x81c2800000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x201c08000000 , (ulong) 0x102070200000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1038040000 , (ulong) 0x40e04080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81808040000 , (ulong) 0x14381000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381000100000 , (ulong) 0x42818080000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x203420000000 , (ulong) 0x10081c200000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x42c040000 , (ulong) 0x43810080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x800081c0000 , (ulong) 0x101814200000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x380008000000 , (ulong) 0x3c10000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x282048000000 , (ulong) 0x20101c10000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202028000000 , (ulong) 0x101810100000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8300028000000 , (ulong) 0x403810100000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1404040000 , (ulong) 0x80818080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x14000c1000 , (ulong) 0x8081c020000), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10001c0000 , (ulong) 0x83c000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1204140000 , (ulong) 0x838080400), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x302048000000 , (ulong) 0x8081c10000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8302008000000 , (ulong) 0x1870100000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10040c1000 , (ulong) 0x80e18000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x12040c0000 , (ulong) 0x838101000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x283038000000 , (ulong) 0x20100c00000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381828000000 , (ulong) 0x402010100000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x14181c0000 , (ulong) 0x80804020000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c0c140000 , (ulong) 0x30080400), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383028000000 , (ulong) 0xc10200000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383028000000 , (ulong) 0x40810100000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x140c1c0000 , (ulong) 0x80810200000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x140c1c0000 , (ulong) 0x40830000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x303038000000 , (ulong) 0x10080c00000000), (ulong)0x4000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383808000000 , (ulong) 0x4030100000), (ulong)0x80000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101c1c0000 , (ulong) 0x80c02000000), (ulong)0x100000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c0c0c0000 , (ulong) 0x30100800), (ulong)0x2000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383030000000 , (ulong) 0xc08100000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383820000000 , (ulong) 0x418100000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x41c1c0000 , (ulong) 0x81820000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc0c1c0000 , (ulong) 0x81030000000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x303038000000 , (ulong) 0x8080c00000000), (ulong)0x4000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383808000000 , (ulong) 0x70100000), (ulong)0x80000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101c1c0000 , (ulong) 0x80e00000000), (ulong)0x100000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c0c0c0000 , (ulong) 0x30101000), (ulong)0x2000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383030000000 , (ulong) 0xc08080000), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383820000000 , (ulong) 0x1c100000), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x41c1c0000 , (ulong) 0x83800000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc0c1c0000 , (ulong) 0x101030000000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81018100000 , (ulong) 0x20302c00000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c28000000 , (ulong) 0x702010100000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1438000000 , (ulong) 0x808040e0000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x81808100000 , (ulong) 0x340c0400), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381008100000 , (ulong) 0x2c10200000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x203428000000 , (ulong) 0x140810100000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x142c040000 , (ulong) 0x80810280000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x810081c0000 , (ulong) 0x40834000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x301018100000 , (ulong) 0x10082c00000000), (ulong)0x4000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x203c08000000 , (ulong) 0x104030100000), (ulong)0x400000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x103c040000 , (ulong) 0x80c02080000), (ulong)0x20000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x818080c0000 , (ulong) 0x34100800), (ulong)0x2000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x381000100000 , (ulong) 0x2c18080000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x203420000000 , (ulong) 0x10081c100000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x42c040000 , (ulong) 0x83810080000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x800081c0000 , (ulong) 0x101834000000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x30080c000000 , (ulong) 0x4083410000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202018080000 , (ulong) 0x101820500000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101804040000 , (ulong) 0xa0418080000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x30100c0000 , (ulong) 0x82c102000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x38000c000000 , (ulong) 0x43c10000000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202028080000 , (ulong) 0x101810300000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101404040000 , (ulong) 0xc0818080000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x30001c0000 , (ulong) 0x83c200000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x380800000000 , (ulong) 0x301c000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383020000000 , (ulong) 0x4081c000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202030000000 , (ulong) 0x101808080000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383020000000 , (ulong) 0x818280000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc04040000 , (ulong) 0x101018080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40c1c0000 , (ulong) 0x141810000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101c0000 , (ulong) 0x380c000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40c1c0000 , (ulong) 0x3810200000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x380800000000 , (ulong) 0x3018040000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x382040000000 , (ulong) 0x41818040000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202030000000 , (ulong) 0x101808040000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8302020000000 , (ulong) 0x1818240000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc04040000 , (ulong) 0x201018080000), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x4040c1000 , (ulong) 0x241818000000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x101c0000 , (ulong) 0x20180c000000), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x2041c0000 , (ulong) 0x201818200000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x382040000000 , (ulong) 0x1c18040000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8302020000000 , (ulong) 0x1818140000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x4040c1000 , (ulong) 0x281818000000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x2041c0000 , (ulong) 0x201838000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383020000000 , (ulong) 0x40818040000), (ulong)0x80000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x383020000000 , (ulong) 0x818240000), (ulong)0x4000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40c1c0000 , (ulong) 0x241810000000), (ulong)0x2000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40c1c0000 , (ulong) 0x201810200000), (ulong)0x100000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x201000000000 , (ulong) 0x100818080000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10300000000000 , (ulong) 0x3818080000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10300040000000 , (ulong) 0x7818080000), (ulong)0x400000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x201000000000 , (ulong) 0x281c000000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x206000000000 , (ulong) 0x10181c000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8206000000000 , (ulong) 0x1010181c000000), (ulong)0x20000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8040000 , (ulong) 0x3814000000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x6040000 , (ulong) 0x3818080000), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x6041000 , (ulong) 0x3818080800), (ulong)0x400 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8040000 , (ulong) 0x101810080000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc0800 , (ulong) 0x10181c000000), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x2000c0800 , (ulong) 0x10181e000000), (ulong)0x20000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10300020000000 , (ulong) 0x7818080000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x286000000000 , (ulong) 0x1010181c000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x6140000 , (ulong) 0x3818080800), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x4000c0800 , (ulong) 0x10181e000000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x201000000000 , (ulong) 0x10081c000000), (ulong)0x10000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10201000000000 , (ulong) 0x2010081c000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x280000000000 , (ulong) 0x103010181c000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x201000000000 , (ulong) 0x2818080000), (ulong)0x4000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x205000000000 , (ulong) 0x402818080000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x200020000000 , (ulong) 0x40f818080000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8040000 , (ulong) 0x101814000000), (ulong)0x2000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xa040000 , (ulong) 0x101814020000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x400040000 , (ulong) 0x10181f020000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8040000 , (ulong) 0x3810080000), (ulong)0x800 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8040800 , (ulong) 0x3810080400), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x140000 , (ulong) 0x3818080c08), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10300000000000 , (ulong) 0x381c000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202040000000 , (ulong) 0x101010181c000000), (ulong)0x8000000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x206000000000 , (ulong) 0x101818080000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8300000000000 , (ulong) 0xf818080000), (ulong)0x40000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x6040000 , (ulong) 0x101818080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc1000 , (ulong) 0x10181f000000), (ulong)0x200000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc0800 , (ulong) 0x381c000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x204040000 , (ulong) 0x3818080808), (ulong)0x1000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10302000000000 , (ulong) 0x187c000000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x306000000000 , (ulong) 0x8081818080000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x60c0000 , (ulong) 0x101818101000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40c0800 , (ulong) 0x3e18000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x10301000040000 , (ulong) 0x281c080000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x207000040000 , (ulong) 0x10081c080000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x20000e040000 , (ulong) 0x103810080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x2000080c0800 , (ulong) 0x103814000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x200000000000 , (ulong) 0x101010181c000000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x300000000000 , (ulong) 0x1010081c1c000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x200000000000 , (ulong) 0xf818080000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202000000000 , (ulong) 0xd838180000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40000 , (ulong) 0x10181f000000), (ulong)0x400000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x4040000 , (ulong) 0x181c1b000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40000 , (ulong) 0x3818080808), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0xc0000 , (ulong) 0x3838100808), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x380000000000 , (ulong) 0x381c000000), (ulong)0x20000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x302040000000 , (ulong) 0x408181c000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x202020000000 , (ulong) 0x101818080000), (ulong)0x80000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x8302000000000 , (ulong) 0x1838480000), (ulong)0x200000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x404040000 , (ulong) 0x101818080000), (ulong)0x100000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x40c1000 , (ulong) 0x121c18000000), (ulong)0x40000000000 );
        WhiteDict.Add(System.Tuple.Create((ulong) 0x1c0000 , (ulong) 0x381c000000), (ulong)0x400000000 );
        ulong tmp_mask = 0x8000000000000000;
        for (int i=0;i<8;i++) {
            for (int j=0;j<8;j++) {
                //ulong tmp_put = ((ulong)0x0000000000000001)<<(63-8*i-j);
                string output =  ((char)(j+97)).ToString() + (i+1).ToString();
                PutTOOutput.Add(tmp_mask,output);
                tmp_mask >>= 1;
            }
        }

        player1Information = new PlayerInformation(isHuman1, MaxNodes1, WeightNumberOfHands1, WeightNumberOfSettledStones1, WeightDangerousHands1,WeightCellPoints1);
        player2Information = new PlayerInformation(isHuman2, MaxNodes2, WeightNumberOfHands2, WeightNumberOfSettledStones2, WeightDangerousHands2,WeightCellPoints2);
                
        if (player_id==0) {
            blackInformation = player1Information;
            whiteInformation = player2Information;
        }
        else {
            blackInformation = player2Information;
            whiteInformation = player1Information;
        }

        while (true)
        {
            playerboard = 0x0000000000000000;
            opponentboard = 0x0000000000000000;
            for (int i = 0; i < 8; i++)
            {
                string line = Console.ReadLine();
                for (int j = 0; j < 8; j++) {
                    if (line[j]==player_char) {
                        playerboard |= (mask>>(8*i+j));
                    }
                    else if (line[j]==opponent_char) {
                        opponentboard |= (mask>>(8*i+j));
                    }
                }
            }
            int actionCount = int.Parse(Console.ReadLine());
            string[] possible_actions= new string[actionCount];
            for (int i = 0; i < actionCount; i++)
            {
                string action = Console.ReadLine(); // the action
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
            //Console.Error.WriteLine(put.ToString("x"));
            Console.WriteLine(PutTOOutput[put]);
            //string decided_action = player.PutToIO(put);
            //Console.WriteLine(decided_action);
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
        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(rest_depth==0) {
            //res = board.BitCount(boardToEvaluate.PlayerBoard) - board.BitCount(boardToEvaluate.OpponentBoard);
            res = BitOperations.PopCount(boardToEvaluate.PlayerBoard) - 32;
            
            if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1;
            return res;
        }
        List<ulong> puts = boardToEvaluate.MakePlayerLegalPutList();
        if (puts.Count==0) {
            res = BitOperations.PopCount(boardToEvaluate.PlayerBoard) - BitOperations.PopCount(boardToEvaluate.OpponentBoard);
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

        //PlayerInformation playerInformation = blackInformation;
        //if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
        //int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation) ;
        int maxDepth = 4;
        // 再帰の終了条件: 読みの深さが上限に達した場合
        if(nowDepth == maxDepth) {
            //Console.Error.WriteLine(maxDepth);
            res =  boardToEvaluate.CalcDifferenceNumberOfHands()    * 0.3
                 + boardToEvaluate.CalcDifferenceSettledStone()     * 1
                 - boardToEvaluate.CountDifferenceDangerousHands()  * 2
                 + boardToEvaluate.CalcDifferenceCellPoints()       * 0.2;
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
                tmp_value = EvaluationValue(newBoard,nowDepth + 1, alpha,beta);
                if (tmp_value > res) {
                    res = tmp_value;
                    alpha = res;
                }
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

            //PlayerInformation playerInformation = blackInformation;
            //if(board.NowTurn == Board.WhiteTurn) playerInformation = whiteInformation;
            //int maxDepth = CalcMaxDepthFromPlayerInformation(playerInformation);
            //maxDepth = 4;
            int maxDepth = 4;
            //Console.Error.WriteLine(maxDepth);
            //Console.Error.WriteLine("maxDepth "+ maxDepth.ToString());

            // 再帰の終了条件: 読みの深さが上限に達した場合
            if(nowDepth == maxDepth) {
                res =  boardToEvaluate.CalcDifferenceNumberOfHands()    * 0.3
                    + boardToEvaluate.CalcDifferenceSettledStone()     * 1
                    - boardToEvaluate.CountDifferenceDangerousHands()  * 2
                    + boardToEvaluate.CalcDifferenceCellPoints()       * 0.2
                    - boardToEvaluate.CalcOpenness(recput,origBoard) *0.3;
                if(boardToEvaluate.NowTurn != board.NowTurn) res *= -1.0;
                EvalDict1[boardToEvaluate] = res;
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
                    tmp_value = EvaluationValue_Memo_Open(boardToEvaluate,recput,newBoard,nowDepth + 1, alpha,beta);
                    if (tmp_value > res) {
                        res = tmp_value;
                        alpha = res;
                    }
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
                }
            }
            EvalDict1[boardToEvaluate] = res;
            return res;
    }

    int CalcMaxDepthFromPlayerInformation(PlayerInformation playerInformation) {
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
        if (rest_depth <= 11) {
            const double INF = 1e9;
            List<ulong> puts = board.MakePlayerLegalPutList();
            //AIInformation.LastNumberOfHands.Add(puts.Count);
            double[] evals = new double[puts.Count];
            bestEval = -INF;
            ulong bestput = puts[0];
            for (int i = 0; i < puts.Count; ++i) {
                Board newBoard = new Board(board);
                newBoard.UpdateBoard(puts[i]);
                evals[i] = EvaluationValue_fullsearch_new(newBoard,rest_depth-1,0,0);
                if (evals[i] > bestEval) {
                    bestEval = evals[i]; 
                    bestput = puts[i];
                }
            }
            //AIInformation.UpdateLastAvgNumberOfHands();
            //List<ulong> bestPuts = new List<ulong>();
            /*for (int i = 0; i < puts.Count; ++i) {
                if (evals[i] == bestEval) bestPuts.Add(puts[i]);
            }*/
            //Random rand = new Random();
            //int bi = rand.Next(0,bestPuts.Count);
            //return bestPuts[bi];
            return bestput;           
        }
        else if (rest_depth <= 35) {
            const double INF = 1e9;
            List<ulong> puts = board.MakePlayerLegalPutList();
            //AIInformation.LastNumberOfHands.Add(puts.Count);
            //AIInformation.UpdateLastAvgNumberOfHands();
            double[] evals = new double[puts.Count];
            bestEval = -INF;
            ulong bestput=puts[0];
            for (int i = 0; i < puts.Count; ++i) {
                Board newBoard = new Board(board);
                newBoard.UpdateBoard(puts[i]);
                evals[i] = EvaluationValue(newBoard, 1, -INF, INF);
                if (evals[i] > bestEval) {
                    bestEval = evals[i]; 
                    bestput = puts[i];
                }
            }
            /*List<ulong> bestPuts = new List<ulong>();
            for (int i = 0; i < puts.Count; ++i) {
                if (evals[i] == bestEval) bestPuts.Add(puts[i]);
            }*/
            //AIInformation.UpdateLastAvgNumberOfHands();
            //Random rand = new Random();
            //int bi = rand.Next(0,bestPuts.Count);
            //return bestPuts[bi];
            return bestput;
        }
        else {

            const double INF = 1e9;
            List<ulong> puts = board.MakePlayerLegalPutList();
            AIInformation.LastNumberOfHands.Add(puts.Count);
            //AIInformation.UpdateLastAvgNumberOfHands();
            if (board.NowTurn == Board.BlackTurn) {
                //Console.Error.WriteLine("black");
                var check = System.Tuple.Create(blackBoard,whiteBoard);
                if (BlackDict.ContainsKey(check)) {
                    Console.Error.WriteLine("hit b");
                    return BlackDict[check];
                }
            }
            else {
                //Console.Error.WriteLine("white");
                var check = System.Tuple.Create(whiteBoard,blackBoard);
                if (WhiteDict.ContainsKey(check)) {
                    Console.Error.WriteLine("hit w");
                    return WhiteDict[check];
                }
            }
            ulong bestput = puts[0];
            double[] evals = new double[puts.Count];
            //double bestEval = -INF;
            bestEval = -INF;
            for (int i = 0; i < puts.Count; ++i) {
                Board newBoard = new Board(board);
                newBoard.UpdateBoard(puts[i]);
                evals[i] = EvaluationValue_Memo_Open(board,puts[i],newBoard, 1, -INF, INF);
                if (evals[i] > bestEval) {
                    bestEval = evals[i]; 
                    bestput = puts[i];
                }
            }
            //AIInformation.UpdateLastAvgNumberOfHands();
            return bestput;
        }
    }
}
public class PlayerInformation
{
    public bool IsHuman{ get; set; }
    public long MaxNodes{ get; set; }
    public double LastAvgNumberOfHands{ get; set; }
    public List<double> LastNumberOfHands{ get; set; }
    public double WeightNumberOfHands{ get; set; }
    public double WeightNumberOfSettledStones{ get; set; }
    public double WeightDangerousHands{ get; set; }
    public double WeightCellPoints{ get; set; }

    public PlayerInformation(bool IsHuman, long MaxNodes, double WeightNumberOfHands, double WeightNumberOfSettledStones, double WeightDangerousHands, double WeightCellPoints) {
        this.IsHuman = IsHuman;
        this.MaxNodes = MaxNodes;
        this.LastAvgNumberOfHands = 10.0;
        this.LastNumberOfHands = new List<double>();
        this.WeightNumberOfHands            = WeightNumberOfHands;
        this.WeightNumberOfSettledStones    = WeightNumberOfSettledStones;
        this.WeightDangerousHands           = WeightDangerousHands;
        this.WeightCellPoints               = WeightCellPoints;
    }

    public PlayerInformation(PlayerInformation playerInformation) {
        this.IsHuman                        = playerInformation.IsHuman;
        this.MaxNodes                       = playerInformation.MaxNodes;
        this.LastAvgNumberOfHands           = playerInformation.LastAvgNumberOfHands;
        this.LastNumberOfHands              = playerInformation.LastNumberOfHands;
        this.WeightNumberOfHands            = playerInformation.WeightNumberOfHands;
        this.WeightNumberOfSettledStones    = playerInformation.WeightNumberOfSettledStones;
        this.WeightDangerousHands           = playerInformation.WeightDangerousHands;
        this.WeightCellPoints               = playerInformation.WeightCellPoints;
    }

    public void UpdateLastAvgNumberOfHands() {
        LastAvgNumberOfHands = 1.0;
        int n = LastNumberOfHands.Count;
        foreach(double x in LastNumberOfHands) {
            LastAvgNumberOfHands *= System.Math.Pow(x, 1.0 / n);
        }
        LastNumberOfHands.Clear();
    }
}
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
  
    public bool CanPut(ulong put) {
        ulong legalBoard = MakePlayerLegalBoard();
        return (put & legalBoard) == put;
    }

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
    
    public bool IsPass() {
        // 手番側の合法手ボードを生成
        ulong playerLegalBoard = MakePlayerLegalBoard();

        // 相手側の合法手ボードを生成
        ulong opponentLegalBoard = MakeOpponentLegalBoard();

        // 手番側だけがパスの場合
        return playerLegalBoard == 0x0000000000000000 && opponentLegalBoard != 0x0000000000000000;
    }

    public bool IsGameFinished() {
        ulong playerLegalBoard = MakePlayerLegalBoard();
        ulong opponentLegalBoard = MakeOpponentLegalBoard();
        // 両手番とも置く場所がない場合
        return playerLegalBoard == 0x0000000000000000 && opponentLegalBoard == 0x0000000000000000;
    }

    public void SwapBoard() {
        // ボードの入れ替え
        ulong tmp = PlayerBoard;
        PlayerBoard = OpponentBoard;
        OpponentBoard = tmp;
        // 色の入れ替え
        NowTurn *= -1;
    }

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

    private ulong MakeOpponentLegalBoard() {
        SwapBoard();
        ulong opponentLegalBoard = MakePlayerLegalBoard();
        SwapBoard();
        return opponentLegalBoard;
    }

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

    public int BitCount(ulong put) {
        return BitOperations.PopCount(put);
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
        return res;        
    }

    private int CalcPlayerNumberOfHands() {
        ulong playerLegalBoard = MakePlayerLegalBoard();
        int playerNumberOfHands = BitOperations.PopCount(playerLegalBoard);
        return playerNumberOfHands;
    }

    private int CalcOpponentNumberOfHands() {
        SwapBoard();
        int opponentNumberOfHands = CalcPlayerNumberOfHands();
        SwapBoard();
        return opponentNumberOfHands;
    }

    public int CalcDifferenceNumberOfHands() {
        return CalcPlayerNumberOfHands() - CalcOpponentNumberOfHands();
    }

    private int CalcPlayerSettledStone() {
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
        return BitOperations.PopCount(playerSettledStoneBoard);
    }

    private int CalcOpponentSettledStone() {
        SwapBoard();
        int opponentSettledStone = CalcPlayerSettledStone();
        SwapBoard();
        return opponentSettledStone;
    }
  
    public int CalcDifferenceSettledStone() {
        return CalcPlayerSettledStone() - CalcOpponentSettledStone();
    }

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

        int res =   PtCenter    * BitOperations.PopCount(PlayerBoard & BitCenter)
                  + PtBoxEdge   * BitOperations.PopCount(PlayerBoard & BitBoxEdge)
                  + PtBoxCorner * BitOperations.PopCount(PlayerBoard & BitBoxCorner)
                  + PtMiddleB   * BitOperations.PopCount(PlayerBoard & BitMiddleB)
                  + PtMiddleA   * BitOperations.PopCount(PlayerBoard & BitMiddleA)
                  + PtX         * BitOperations.PopCount(PlayerBoard & BitX)
                  + PtB         * BitOperations.PopCount(PlayerBoard & BitB)
                  + PtA         * BitOperations.PopCount(PlayerBoard & BitA)
                  + PtC         * BitOperations.PopCount(PlayerBoard & BitC)
                  + PtCorner    * BitOperations.PopCount(PlayerBoard & BitCorner);
        return res;
    }

    private int CalcOpponentCellPoints() {
        SwapBoard();
        int res = CalcPlayerCellPoints();
        SwapBoard();
        return res;
    }

    public int CalcDifferenceCellPoints() {
        return CalcPlayerCellPoints() - CalcOpponentCellPoints();
    }

    private int CountPlayerDangerousHands() {
        int res = 0;
        ulong blankBoard = ~(PlayerBoard | OpponentBoard);
        // 左上
        if((blankBoard & 0x8000000000000000) > 0) res += BitOperations.PopCount(PlayerBoard & 0x40c0000000000000);
        // 右上
        if((blankBoard & 0x0100000000000000) > 0) res += BitOperations.PopCount(PlayerBoard & 0x0203000000000000);
        // 左下
        if((blankBoard & 0x0000000000000080) > 0) res += BitOperations.PopCount(PlayerBoard & 0x000000000000c040);
        // 右下
        if((blankBoard & 0x0000000000000001) > 0) res += BitOperations.PopCount(PlayerBoard & 0x0000000000000302);
        return res;
    }

    private int CountOpponentDangerousHands() {
        SwapBoard();
        int res = CountPlayerDangerousHands();
        SwapBoard();
        return res;
    }

    public int CountDifferenceDangerousHands() {
        return CountPlayerDangerousHands() - CountOpponentDangerousHands();
    }

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
}