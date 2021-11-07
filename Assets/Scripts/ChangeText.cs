using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject score_object = null;
    public GameObject othello;
    public OthelloAIForAIvsHuman2 script;
    [SerializeField] private Board board;
    [SerializeField] private double best_evaluation_value;    
    [SerializeField]private Text num_of_stones;
    //[SerializeField]private Text stone_num;   
    void Start(){
        
        script = othello.GetComponent<OthelloAIForAIvsHuman2>();
        board =script.board;
        //best_evaluation_value = script.bestEval;
        //int expressed_ev = System.Math.Round(best_evaluation_value*10);
        //Debug.Log("board "+board);
        //white_text = this.GetComponent<Text> ();
        num_of_stones = this.GetComponent<Text> ();
        //stone_num = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //gameObject.GetComponent<Text>().text = string.Format("White Stone {0}",BitCount(Board.PlayerBoard));
        
        best_evaluation_value = script.bestEval;
        int white_num = board.BitCount(board.PlayerBoard);
        int black_num = board.BitCount(board.OpponentBoard);
        int expressed_ev = (int)System.Math.Round(best_evaluation_value*1000);
        //Debug.Log("best_ev"+best_evaluation_value);
        //Debug.Log("ev"+ expressed_ev);
        //int num = Board.BitCount(0x8000000000000000);
        //stone_num.text = "White Stone: " + white_num.ToString() + "\n" + "Black Stone" + black_num.ToString();
        num_of_stones.text = "Black Stone: " + black_num.ToString() + "\nWhite Stone: " + white_num.ToString() + "\nEval_value: " + expressed_ev.ToString();


        
        //Debug.Log("score "+num);
        //Debug.Log("text "+score_text.text);
    }
}
