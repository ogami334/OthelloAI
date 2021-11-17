using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class MonteCarloTreeSearch {
    public void Train(Node root_node,int simulation) {
        root_node.Expand();
        for(int i=0;i<simulation;i++) {
            root_node.Evaluate();
        }
    }

    public ulong SelectAction(Node root_node) {
        List <ulong> legal_actions;
        legal_actions = root_node.board.MakePlayerLegalPutList();
        List <int> visit_list = new List<int>();
        int index = -1;
        //int guard = -2147483648;
        double guard = -10000.0;
        File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\Mtest.txt","-1"+System.Environment.NewLine);
        File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\Mtest.txt","number_of_regal " + legal_actions.Count.ToString()+System.Environment.NewLine);
        //Debug.Log("action_count "+legal_actions.Count);
        for (int i=0;i<legal_actions.Count;i++) {
            double win_rate = ((double) root_node.children[i].reward)/ (double) root_node.children[i].visited;
            File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\Mtest.txt","visited " + root_node.children[i].visited.ToString()+System.Environment.NewLine);
            File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\Mtest.txt","reward " + root_node.children[i].reward.ToString()+System.Environment.NewLine);
            File.AppendAllText(@"C:\Users\denjo\Downloads\12ゲームAI リバーシ\Othello\Mtest.txt","win_rate " + win_rate.ToString()+System.Environment.NewLine);
            //Debug.Log("index "+i);
            //Debug.Log("visited"+ root_node.children[i].visited);
            /*if (root_node.children[i].visited>guard) {
                index = i;
                guard = root_node.children[i].visited;
            }*/
            if (win_rate > guard) {
                index = i;
                guard = win_rate;
            }
        }
        //Debug.Log("selected_index  " + index);
        //Debug.Log("visited  " + root_node.children[index].visited);


        return legal_actions[index];
    }
}