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
    [SerializeField]private Text score_text;
    
    void Start(){
        
        script=othello.GetComponent<OthelloAIForAIvsHuman2>();
        board =script.board;
        //Debug.Log("board "+board);
        score_text= this.GetComponent<Text> ();
    }

    // Update is called once per frame
    void Update()
    {
        
        //gameObject.GetComponent<Text>().text = string.Format("White Stone {0}",BitCount(Board.PlayerBoard));
        
       
        int num = board.BitCount(board.PlayerBoard);
        //int num = Board.BitCount(0x8000000000000000);
        score_text.text = num.ToString();
        //Debug.Log("score "+num);
        //Debug.Log("text "+score_text.text);
    }
}
