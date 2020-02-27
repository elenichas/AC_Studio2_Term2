 
using UnityEngine;
 

public class RandomRuleGen 
{
    string AllRules;
    char[] HouseRule;

    public RandomRuleGen ( int length)
    {
        AllRules = "ABCDEFGHIJKL";
        HouseRule = new char[length];
    }
     
      public string MakeRule()
      {      
        for (int i = 0; i < HouseRule.Length; i++)
        {
            HouseRule[i] = AllRules[Random.Range(0,AllRules.Length)];
        }
        string RandRule = new string(HouseRule);
        return RandRule;
       
      }


}
