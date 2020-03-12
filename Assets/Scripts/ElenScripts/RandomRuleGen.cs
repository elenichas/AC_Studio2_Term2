 
using UnityEngine;
 

public class RandomRuleGen 
{
    string AllRulesL;
    string AllRulesS;
    char[] HouseRule;

    public RandomRuleGen ( int length)
    {
        AllRulesL = "ABCDEF";
        AllRulesS = "GHIJKL";
        HouseRule = new char[length];
    }

    //the Rule is created alternatively from rules tha disect the longest and 
    //then the shortest side to create more accurate results
    //it also always start with disecting the longest side for the same reason

    public string MakeRule()
      {      
        for (int i = 0; i < HouseRule.Length; i += 2)
        {
            HouseRule[i] = AllRulesL[Random.Range(0,AllRulesL.Length)];
        }
        for (int i = 1; i < HouseRule.Length; i += 2)
        {
            HouseRule[i] = AllRulesS[Random.Range(0, AllRulesL.Length)];
        }
        string RandRule = new string(HouseRule);
        return RandRule;
       
      }


}
