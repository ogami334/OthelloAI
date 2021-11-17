using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Node {
    public Board board = new Board();
    public int reward = 0;
    public int visited = 0;
    public int expand_base = 10;
    public List<Node> children = new List<Node>();

    public int sum1 = 0;
    public int sum2 = 0;

    public int isblack = 0;


    public Node() {
        this.board = board;
        this.reward = 0;
        this.visited = 0;
        this.expand_base = 10;
        this.children = new List<Node>();
        this.sum1 = 0;
        this.sum2 = 0;
        this.isblack = 0;
    }

    /*public Node(Node node) {
        this.board = node.board;
        this.reward = node.reward;
        this.visited = node.visited;
        this.expand_base = node.expand_base;
        this.children = node.children;
        this.sum1 = 0;
        this.sum2 = 0;
    }*/


    public double Ucb1(int sn, int n ,int w) {
        //Debug.Log("w "+w);
        //Debug.Log("n "+n);
        //Debug.Log("SN "+sn);
        //Debug.Log("w/n "+w/n);
        //Debug.Log("w/n "+ ((double)w)/ ((double)n) );
        return  ((double)w)/((double)n) + 3.5*Math.Pow((2*Math.Log(sn)/n),0.5);
        //return -w/n;
    }


    public Node NextChildBasedUcb() {
        int sn = 0;
        foreach (Node child in children) {
            sn += child.visited;
            if (child.visited == 0) {
                return child;
            }
        }
        int index = -1;
        double guard = -100000;
        for (int i=0;i<children.Count;i++) {
            double tmp = Ucb1(sn,children[i].visited,children[i].reward);
            if (tmp > guard) {
                index = i;
                guard = tmp;
            }
        }
        //Debug.Log("selected"+index);
        return children[index];
    }

    public int Evaluate() {
        int value;
        if (board.IsLose()) {
            value = -1;
            if (isblack==0) {
                value *= -1;
            }
            reward += value;
            visited += 1;
            return value;
        }
        else if (board.IsWin()) {
            value = 1;
            if (isblack==0) {
                value *= -1;
            }
            reward += value;
            visited += 1;
            return value;
        }
        else if (board.IsDraw()) {
            value = 0;
            if (isblack==0) {
                value *= -1;
            }
            reward += value;
            visited += 1;
            return value;
        }

        if (children.Count==0) {
            value = PlayOut(board);
            reward += value;
            visited += 1;
            if (visited==expand_base) {
                Expand();
            }
            return value;
        }
        else {
            value = NextChildBasedUcb().Evaluate();
            reward += value;
            visited += 1;
            return value;
        }
    }

    public void Expand() {
        List <ulong> legalPutList = board.MakePlayerLegalPutList();
        foreach (ulong legalput in legalPutList) {
            Node NextNode = new Node();
            //NextNode.board = NextNode.GenerateUpdatedBoard(NextNode.board,legalput);
            NextNode.board = NextNode.GenerateUpdatedBoard(board,legalput);
            children.Add(NextNode);
        }
    }

    public Board GenerateUpdatedBoard(Board board,ulong put) {
        Board newBoard = new Board(board);
        newBoard.UpdateBoard(put);
        return newBoard;
    }

    public int PlayOut(Board board) {
        int value;
        if (board.IsLose()) {
            value = -1;
            //Debug.Log("PlayerBoard" + board.PlayerBoard.ToString("x"));
            //Debug.Log("OpponentBoard"+board.OpponentBoard.ToString("x"));
            //Debug.Log("Lose");
            if (isblack==0) {
                value *= -1;
            }
            return value;
        }
        if (board.IsDraw()) {
            value = 0;
            //Debug.Log("PlayerBoard" + board.PlayerBoard.ToString("x"));
            //Debug.Log("OpponentBoard"+board.OpponentBoard.ToString("x"));
            //Debug.Log("Draw");
            return value;
        }
        if (board.IsWin()) {
            value = 1;
            //Debug.Log("PlayerBoard" + board.PlayerBoard.ToString("x"));
            //Debug.Log("OpponentBoard"+board.OpponentBoard.ToString("x"));
            //Debug.Log("Win");
            if (isblack==0) {
                value *= -1;
            }
            return value;
        }
        return PlayOut(GenerateUpdatedBoard(board,board.Random_action()));
    }
}

    