using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation
{
    public bool IsHuman{ get; set; }
    public int MaxNodes{ get; set; }
    public double LastAvgNumberOfHands{ get; set; }
    public List<double> LastNumberOfHands{ get; set; }
    public double WeightNumberOfHands{ get; set; }
    public double WeightNumberOfSettledStones{ get; set; }
    public double WeightDangerousHands{ get; set; }

    public PlayerInformation(bool IsHuman, int MaxNodes, double WeightNumberOfHands, double WeightNumberOfSettledStones, double WeightDangerousHands) {
        this.IsHuman = IsHuman;
        this.MaxNodes = MaxNodes;
        this.LastAvgNumberOfHands = 10.0;
        this.LastNumberOfHands = new List<double>();
        this.WeightNumberOfHands            = WeightNumberOfHands;
        this.WeightNumberOfSettledStones    = WeightNumberOfSettledStones;
        this.WeightDangerousHands           = WeightDangerousHands;
    }

    public PlayerInformation(PlayerInformation playerInformation) {
        this.IsHuman                        = playerInformation.IsHuman;
        this.MaxNodes                       = playerInformation.MaxNodes;
        this.LastAvgNumberOfHands           = playerInformation.LastAvgNumberOfHands;
        this.LastNumberOfHands              = playerInformation.LastNumberOfHands;
        this.WeightNumberOfHands            = playerInformation.WeightNumberOfHands;
        this.WeightNumberOfSettledStones    = playerInformation.WeightNumberOfSettledStones;
        this.WeightDangerousHands           = playerInformation.WeightDangerousHands;
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