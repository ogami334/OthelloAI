using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        var player = new Player();
        int id = int.Parse(Console.ReadLine()); // id of your player.
        //Console.Error.WriteLine(id);
        int boardSize = int.Parse(Console.ReadLine());
        //Console.Error.WriteLine(boardSize);

        // game loop
        while (true)
        {
            for (int i = 0; i < boardSize; i++)
            {
                string line = Console.ReadLine(); // rows from top to bottom (viewer perspective).
            }
            int actionCount = int.Parse(Console.ReadLine()); // number of legal actions for this turn.
            string[] possible_actions= new string[actionCount];
            for (int i = 0; i < actionCount; i++)
            {
                string action = Console.ReadLine(); // the action
                ulong tmp_put = player.IOToPut(action);
                Console.Error.WriteLine(tmp_put);
                action = player.PutToIO(tmp_put);
                possible_actions[i] = action;
            }
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine(possible_actions[0]); // a-h1-8

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
}








