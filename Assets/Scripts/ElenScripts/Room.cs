using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//Each Room is an object with a Mesh representation and a bool that reflects its state 
//if the State is true the room is active and can be further divided
//if the the State is false the next rule in the sequence will not be applied to it and
//it will not be subdivided any further
public class Room 
{
        public Mesh Rec { get; set; }
        public bool State { get; set; }
       //the Shape grammar could potentially assing uses to Rooms as well
       //The GA could the find "good houses" in the grammar instead of just "good labels" for specific house
       // public string Label { get; set; }
        public Room()
        {
          

        }
}
