using System.Collections;
using System.Collections.Generic;
using System;
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
        int guard = -2147483648;
        for (int i=0;i<legal_actions.Count;i++) {
            if (root_node.children[i].visited>guard) {
                index = i;
                guard = root_node.children[i].visited;
            }
        }
        return legal_actions[index];
    }
}