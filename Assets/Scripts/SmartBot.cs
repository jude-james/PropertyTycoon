using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Tiles;
using Unity.Barracuda;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SmartBot : Player
{
    private readonly WaitForSeconds _decisionMakingTime = new(2.5f); // Simulate bot thinking and give time for UI elements to show
    private readonly WaitForSeconds _bidDecisionMakingTime = new(1f);


    private Model buyModel;
    private Model jailModel;
    private Model generalModel;
    private Model bidModel;
    private int turnNumber = 0;

    private new void Start()
    {
        var assetName = "GeneralModel.onnx";
        var modelBinary = Resources.Load<NNModel>(assetName.Substring(0, assetName.LastIndexOf('.')));
        generalModel = ModelLoader.Load(modelBinary);

        assetName = "BuyModel.onnx";
        modelBinary = Resources.Load<NNModel>(assetName.Substring(0, assetName.LastIndexOf('.')));
        buyModel = ModelLoader.Load(modelBinary);

        assetName = "JailModel.onnx";
        modelBinary = Resources.Load<NNModel>(assetName.Substring(0, assetName.LastIndexOf('.')));
        jailModel = ModelLoader.Load(modelBinary);

        assetName = "BidModel.onnx";
        modelBinary = Resources.Load<NNModel>(assetName.Substring(0, assetName.LastIndexOf('.')));
        bidModel = ModelLoader.Load(modelBinary);
        base.Start();
    }



    protected override void RollDiceDecision()
    {
        OnRollDice();
    }

    protected override void EndTurnDecision()
    {
        // ----------------------CODE GOES HERE----------------------
        // Check Bot.cs for simple version
        IWorker generalWorker = WorkerFactory.CreateWorker(generalModel);

        /*
        Inputs for bid network:
        1: money/1500
        2: -(propertyCost/200)
        3: Property Colour(as a fraction of 7 e.g. Brown = 0/7, Blue = 1/7... Deep Blue = 1/7)
        5-11: Percentage completion of each colour set (e.g. own 2 Browns = 1, own 1 Blue = 1/3, own 1 Deep Blue = 1/2)
        12: Number of stations owned
        13: Number of utils owned
        14: No. of turns
        */
        float i1 = Money/1500;
        List<float> colourCount = TitleDeedsToColourPercentage(TitleDeeds);
        float i2 = colourCount[0]/2; //Brown
        float i3 = colourCount[1]/3; // Blue
        float i4 = colourCount[2]/3; //Purple
        float i5 = colourCount[3]/3; //Orange
        float i6 = colourCount[4]/3; // Red
        float i7 = colourCount[5]/3; // Yellow
        float i8 = colourCount[6]/3; // Green
        float i9 = colourCount[7]/2; // Deep Blue
        float i10 = countStations(TitleDeeds);
        float i11 = countUtilities(TitleDeeds);
        float i12 = 0f;
        //float i13 = sumHouseVal(TitleDeeds) This is the sum of the house values owned by the bot, if this becomes a pain, instead, count the number of houses and x average house price
        float i13 = turnNumber/100;
        float[] inputs = new float[13] {i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13};
        Tensor inputTensor = new Tensor(1,1,13,1,inputs);
        generalWorker.Execute(inputTensor);
        Tensor outputs = generalWorker.PeekOutput();
        generalWorker?.Dispose();
        inputTensor.Dispose();

        /*
        if (CanBuy() && outputs[2] > 0.5)
        {
            OnBuy() (buys a house on a random property)
        }
        */
        OnEndTurn();
        turnNumber++;
    }

    protected override void InJailDecision()
    {
        UIManager.Instance.ShowInJailPrompt(false, false, false);
        UIManager.Instance.ShowBotDecisionDialog();
        
        StartCoroutine(InJailDecisionCoroutine());
    }

    private IEnumerator InJailDecisionCoroutine()
    {
        yield return _decisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();

        // ----------------------CODE GOES HERE----------------------
        IWorker jailWorker = WorkerFactory.CreateWorker(jailModel);

        /*
        Inputs for Jail network:
        1: Number of jail cards
        2: Amount of money
        3: Turns currently spent in jail
        4: Current Turn
        */
        float[] inputs = {GetOutOfJailFreeCards.Count,Money,RoundsInJail,turnNumber};

        Tensor inputTensor = new Tensor(1,1,4,1,inputs);
        jailWorker.Execute(inputTensor);
        float output = jailWorker.PeekOutput()[0];
        jailWorker?.Dispose();
        inputTensor.Dispose();

        if (GetOutOfJailFreeCards.Count > 0 && output > 0.5) 
        {
            OnGetOutOfJailFree();
        }
        else if (Money >= PostBailAmount && output > 0.5)
        {
            OnPostBail();
        }
        else
        {
            OnRemainInJail();
        }


    }
    
    public override void ForSaleDecision(Property property)
    {
        UIManager.Instance.ShowForSalePrompt(false, false, property);
        UIManager.Instance.ShowBotDecisionDialog();

        StartCoroutine(ForSaleDecisionCoroutine(property));
    }

    private IEnumerator ForSaleDecisionCoroutine(Property property)
    {
        yield return _decisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();
        
        // ----------------------CODE GOES HERE----------------------
        IWorker buyWorker = WorkerFactory.CreateWorker(buyModel);

        /*
        Inputs for bid network:
        1: money/1500
        2: -(propertyCost/200)
        3: Property Colour(as a fraction of 7 e.g. Brown = 0/7, Blue = 1/7... Deep Blue = 1/7)
        5-11: Percentage completion of each colour set (e.g. own 2 Browns = 1, own 1 Blue = 1/3, own 1 Deep Blue = 1/2)
        12: Number of stations owned
        13: Number of utils owned
        14: No. of turns
        */
        float i1 = Money/1500;
        float i2 = -(property.Cost/400);
        float i3 = propertyNoToColour(property.PropertyNumber)/7;
        List<float> colourCount = TitleDeedsToColourPercentage(TitleDeeds);
        float i4 = colourCount[0]/2; //Brown
        float i5 = colourCount[1]/3; // Blue
        float i6 = colourCount[2]/3; //Purple
        float i7 = colourCount[3]/3; //Orange
        float i8 = colourCount[4]/3; // Red
        float i9 = colourCount[5]/3; // Yellow
        float i10 = colourCount[6]/3; // Green
        float i11 = colourCount[7]/2; // Deep Blue
        float i12 = countStations(TitleDeeds);
        float i13 = countUtilities(TitleDeeds);
        float i14 = turnNumber/100;
        float[] inputs = new float[14] {i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14};
        Tensor inputTensor = new Tensor(1,1,14,1,inputs);
        buyWorker.Execute(inputTensor);
        float output = buyWorker.PeekOutput()[0];
        buyWorker?.Dispose();
        inputTensor.Dispose();
        Debug.Log(output);

        var canBuy = Money >= property.Cost;
        if (Board.Instance.CanAuction() && canBuy) // if both, the smart bot can make a choice
        {
            if (output > 0.565)
                OnBuy();
            else
                OnAuction();
        }
        else // otherwise it is forced
        {
            if (canBuy)
                OnBuy();
            else
                OnAuction();
        }
    }

    public override void BidDecision()
    {
        CurrentBidder = this;
        
        UIManager.Instance.UpdateAuctionPrompt(false, false, Name, Sprite);
        UIManager.Instance.ShowBotDecisionDialog();

        StartCoroutine(BidDecisionCoroutine());
    }

    private IEnumerator BidDecisionCoroutine()
    {
        yield return _bidDecisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();
        
        // ----------------------CODE GOES HERE----------------------

        IWorker bidWorker = WorkerFactory.CreateWorker(bidModel);
        /*
        Inputs for bid network:
        1: money/1500
        2: -(propertyCost/200)
        3: AuctionPrice
        4: Property Colour(as a fraction of 7 e.g. Brown = 0/7, Blue = 1/7... Deep Blue = 1/7)
        5-12: Percentage completion of each colour set (e.g. own 2 Browns = 1, own 1 Blue = 1/3, own 1 Deep Blue = 1/2)
        13: Number of stations owned
        14: Number of utils owned
        */
        float i1 = Money/1500;
        float i2 = -(Board.Instance.AuctionProperty.Cost/400);
        float i3 = Board.Instance.AuctionPrice/(Board.Instance.AuctionProperty.Cost*0.5f);
        float i4 = propertyNoToColour(Board.Instance.AuctionProperty.PropertyNumber);
        List<float> colourCount = TitleDeedsToColourPercentage(TitleDeeds);
        float i5 = colourCount[0]/2; //Brown
        float i6 = colourCount[1]/3; // Blue
        float i7 = colourCount[2]/3; //Purple
        float i8 = colourCount[3]/3; //Orange
        float i9 = colourCount[4]/3; // Red
        float i10 = colourCount[5]/3; // Yellow
        float i11 = colourCount[6]/3; // Green
        float i12 = colourCount[7]/2; // Deep Blue
        float i13 = countStations(TitleDeeds);
        float i14 = countUtilities(TitleDeeds);
        float[] inputs = new float[14] {i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14};

        Tensor inputTensor = new Tensor(1,1,14,1,inputs);
        bidWorker.Execute(inputTensor);
        float output = bidWorker.PeekOutput()[0];
        bidWorker?.Dispose();
        inputTensor.Dispose();
        Debug.Log(output);

        var canBid = Money >= Board.Instance.AuctionPrice + Board.Instance.BidAmount;
        if (canBid && output > 0.5)
        {
            OnBid();
        }
        else
        {
            OnFold();
        }
    }
    private float countStations(Property[] TitleDeeds)
    {
        int counter = 0;
        foreach (Property property in TitleDeeds){
            if (property is Station)
            {
                counter++;
            }
        }
        return counter;
    }
    private float countUtilities(Property[] TitleDeeds)
    {
        int counter = 0;
        foreach (Property property in TitleDeeds){
            if (property is Utility){
                counter++;
            }
        }
        return counter;
    }

    // This takes the number of a property, and matches it to the numerical value
    // of its colour.
    private float propertyNoToColour(int propno)
    {
        switch (propno){
            case 0:
            case 1:
                return 0;
            case 3:
            case 4:
            case 5:
                return 1;
            case 6:
            case 8:
            case 9:
                return 2;
            case 11:
            case 12:
            case 13:
                return 3;
            case 14:
            case 15:
            case 16:
                return 4;
            case 18:
            case 19:
            case 21:
                return 5;
            case 22:
            case 23:
            case 24:
                return 6;
            case 26:
            case 27:
                return 7;
            default:
                return 0;
        }
    }
    /*
    This method takes the bot's title deeds, and returns a list of length 8, 
    where each index holds the number of properties of a certain colour, 
    e.g. can return [0,1,3,2,0,0,0,1], 
    which would mean the player has 1 Blue, 3 purple etc.
    */
    private List<float> TitleDeedsToColourPercentage(Property[] TitleDeeds)
    {
        //First makes a list where all properties owned by the player are represented by a number from 0 to 7 for their colour
        // The colour of a property is based on the property number (function for this is above).
        List<float> propertiesToColours = new List<float>();
        foreach (Property property in TitleDeeds)
        {
            if (property != null && property is Street){
                propertiesToColours.Add(propertyNoToColour(property.PropertyNumber));
            }
            
        }
        /* 
        Then it counts the number of instance of each number, 
        and places them where the index represents the corresponding colour
        e.g. index 0 is Brown, index 1 is Blue.....
        
        */
        propertiesToColours.Add(7);
        propertiesToColours.Sort();
        var g = propertiesToColours.GroupBy( i => i );
        List<float> result = new List<float>();
        int i = 0;
        foreach( var grp in g )
        {
            if (!(grp.Key == i)){
            // If there is no values for a certain colour, add 0.
                while(!(grp.Key == i))
                {
                    result.Add(0);
                    i++;
                }
            }
            i++;
            result.Add(grp.Count());
        }
        result[7] = result[7]-1;
        return result;
    }
    
    protected override void RaiseFundsDecision()
    {
        UIManager.Instance.ShowBotDecisionDialog();
        
        StartCoroutine(RaiseFundsDecisionCoroutine());
    }

    private IEnumerator RaiseFundsDecisionCoroutine()
    {
        yield return _decisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();
        
        // ----------------------CODE GOES HERE----------------------
        if (CanMortgage())
        {
            var mortgageableProperties = GetMortgageableProperties();
            foreach (var property in mortgageableProperties)
            {
                property.Mortgage();
                if (Money >= 0)
                {
                    CompleteTurn();
                    yield break;
                }
            }
        }
        
        if (CanSellProperty())
        {
            var sellableProperties = GetSellableProperties();
            foreach (var property in sellableProperties)
            {
                property.SellProperty();
                if (Money >= 0)
                {
                    CompleteTurn();
                    yield break;
                }
            }
        }
        
    }
}
