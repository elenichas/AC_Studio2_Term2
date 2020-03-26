using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


//the House class includes all the Rules of the shape grammar
//and the basic mesh representation of the house generated 
//with a specific sequence of Rules(ex. ABC house)
public class House 
{ 
       readonly string Rule;

       public List<List<Room>> Rooms;
     
    public House(string Rule,int num)
    {     
        //The list of lists that hold all the Rooms created in each step
        //not all to be visualized
        Rooms = new List<List<Room>>();

        //Create the parentMesh and parentRoom and store it in Rooms[0][0]
        Mesh parentrec = new Mesh();
        Vector3[] verticesp = new Vector3[4];

        verticesp[0] = new Vector3(0, 0, 0);
        verticesp[1] = new Vector3(0, 0, num);
        verticesp[2] = new Vector3(num+3, 0, num);
        verticesp[3] = new Vector3(num+3, 0,0);

        parentrec.vertices = verticesp;
        parentrec.triangles = new int[] { 0, 3, 2, 0, 2, 1 };
        parentrec.RecalculateNormals();

        Room parent = new Room
        {
            Rec = parentrec,
            State = true
        };

        List<Room> temp = new List<Room>
        {
            parent
        };
        Rooms.Add(temp);
        
        this.Rule = Rule;                              
    }
        //Method that "reads" the rule and applies it letter by letter, for all the active Rooms in current state
        // checks Rooms[i] and created Rooms[i+1] to put the children created
        public void CreateRooms()
        {
            Mesh currentRoom = new Mesh();
            for (int i = 0; i < Rule.Length; i++)
            {
                //Check the item in list i and  create the next list(i+1) to add them
                List<Room> next = new List<Room>();
                Rooms.Add(next);

                char current = Rule[i];

                //check every room in list i to further subdivide
                for (int j = 0; j < Rooms[i].Count; j++)
                {
                    if (Rooms[i][j].State == true)
                    {
                        currentRoom = Rooms[i][j].Rec;

                        //////////////////6 RULES FOR DISSECTING THE LONGEST SIDE ////////////

                        switch (current)
                        {
                            //Dissect in longest side(1/2 - 1/2) next rule applies to both Rooms.
                            case 'A':
                                next.AddRange(RuleA(currentRoom)); break;

                            //Dissect in longest side(1/3 - 2/3) next rule applies to both Rooms.
                            case 'B':
                                next.AddRange(RuleB(currentRoom)); break;

                            //Dissect in longest side(2/3- 1/3)  next rule applies to both Rooms.
                            case 'C':
                                next.AddRange(RuleC(currentRoom)); break;

                            //Dissect in longest side(1/2 - 1/2) next rule applies to first Room.
                            case 'D':
                                next.AddRange(RuleD(currentRoom)); break;

                            //Dissect in longest side(2/3- 1/3)  next rule applies to first Room.
                            case 'E':
                                next.AddRange(RuleE(currentRoom)); break;

                            //Dissect in longest side(1/2- 1/2)  next rule applies to second Room.
                            case 'F':
                                next.AddRange(RuleF(currentRoom)); break;

                            //////////////////6 RULES FOR DISSECTING THE SHORTEST SIDE/////////////
                            //Dissect in shortest side(1/2 - 1/2) next rule applies to both Rooms.
                            case 'G':
                                next.AddRange(RuleG(currentRoom)); break;

                            //Dissect in shortest side(1/3 - 2/3) next rule applies to both Rooms.
                            case 'H':
                               next.AddRange(RuleH(currentRoom)); break;

                            //Dissect in shortest side(2/3 - 1/3) next rule applies to both Rooms.
                            case 'I':
                                next.AddRange(RuleI(currentRoom)); break;

                            //Dissect in shortest side(1/2 - 1/2) next rule applies to first Room.
                            case 'J':
                                next.AddRange(RuleJ(currentRoom)); break;

                            //Dissect in shortest side(2/3- 1/3)  next rule applies to first Room.
                            case 'K':
                                next.AddRange(RuleK(currentRoom)); break;

                            //Dissect in shortest side(1/2- 1/2)  next rule applies to second Room.
                             case 'L':
                                next.AddRange(RuleL(currentRoom)); break;
                        }
                    }
                }
            }
        }
        
        //My final Rooms are all the deactivated plus the leafs of the tree(last ones created)
        public List<Room> GetFinalRooms()
        {
            List<Room> FinalRooms = new List<Room>();
            for (int i = 0; i < Rooms.Count; i++)
            {
                if (i == Rooms.Count - 1)
                    FinalRooms.AddRange(Rooms[i]);
                for (int j = 0; j < Rooms[i].Count; j++)
                {
                    if ((Rooms[i][j].State == false) && (i != Rooms.Count - 1))
                    {
                        FinalRooms.Add(Rooms[i][j]);
                    }
                }
            }

        return FinalRooms;
        }

        public List<Room> RuleA(Mesh currentRoom)
        {
            return LongestSide(currentRoom, 0.5f, true, true);
        }
        public List<Room> RuleB(Mesh currentRoom)
        {
            return LongestSide(currentRoom, 0.4f, true, true);
        }
        public List<Room> RuleC(Mesh currentRoom)
        {
            return LongestSide(currentRoom, 0.6f, true, true);
        }
        public List<Room> RuleD(Mesh currentRoom)
        {
            return LongestSide(currentRoom, 0.5f, true, false);
        }
        public List<Room> RuleE(Mesh currentRoom)
        {
            return LongestSide(currentRoom, 0.6f, true, false);
        }
        public List<Room> RuleF(Mesh currentRoom)
        {
            return LongestSide(currentRoom, 0.5f, false, true);
        }
        /////////////////////////////////////////////////////////
        public List<Room> RuleG(Mesh currentRoom)
        {
            return ShortestSide(currentRoom, 0.5f, true, true);
        }
        public List<Room> RuleH(Mesh currentRoom)
        {
            return ShortestSide(currentRoom, 0.4f, true, true);
        }
        public List<Room> RuleI(Mesh currentRoom)
        {
            return ShortestSide(currentRoom, 0.6f, true, true);
        }
        public List<Room> RuleJ(Mesh currentRoom)
        {
            return ShortestSide(currentRoom, 0.5f,true, false);
        }
        public List<Room> RuleK(Mesh currentRoom)
        {
            return ShortestSide(currentRoom, 0.6f, true, false);
        }
        public List<Room> RuleL(Mesh currentRoom)
        {
            return ShortestSide(currentRoom, 0.5f, false, true);
        }

    //Get the Number of Rooms the current Rule can give
        public int GetRoomNum()
        {
            int RoomNum = 0;
            for (int i = 0; i < Rooms.Count; i++)
            {
                if (i == Rooms.Count - 1)
                    RoomNum += Rooms[i].Count;
                for (int j = 0; j < Rooms[i].Count; j++)
                {
                    if ((Rooms[i][j].State == false) && (i != Rooms.Count - 1))
                    {
                        RoomNum++;
                    }
                }
            }
            return RoomNum;

        }

        public List<Room> LongestSide(Mesh currentRoom, float num1, bool state1, bool state2)
        {
            Room child1 = new Room(); Room child2 = new Room();
            List<Room> LongestSide = new List<Room>();
            Bounds bounds = currentRoom.bounds;

            if (bounds.size.z >= bounds.size.x)
            {
              Mesh mesh1 = new Mesh();
              Vector3[] vertices1 = new Vector3[4];

              vertices1[0] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z);
              vertices1[1] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z);
              vertices1[2] = new Vector3(currentRoom.vertices[0].x+ bounds.size.x, 0, currentRoom.vertices[0].z+ bounds.size.z * num1);
              vertices1[3] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z + bounds.size.z * num1);
             
              mesh1.vertices = vertices1;
              mesh1.triangles = new int[] { 0,3, 2, 0, 2, 1};
              mesh1.RecalculateNormals();

              child1.Rec = mesh1;
              ///////////////////////////////////////////////////////////////////////////////////////////////////////
              
              Mesh mesh2 = new Mesh();
              Vector3[] vertices2 = new Vector3[4];

              vertices2[0] = new Vector3(currentRoom.vertices[0].x,0, currentRoom.vertices[0].z + bounds.size.z*num1);
              vertices2[1] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z + bounds.size.z * num1);
              vertices2[2] = new Vector3(currentRoom.vertices[0].x + bounds.size.x, 0, currentRoom.vertices[0].z+bounds.size.z);
              vertices2[3] = new Vector3(currentRoom.vertices[0].x , 0, currentRoom.vertices[0].z+bounds.size.z);

              mesh2.vertices = vertices2;
              mesh2.triangles = new int[] { 0,3, 2, 0, 2, 1 };

              mesh2.RecalculateNormals();

              child2.Rec = mesh2;
            
              child1.State = state1; child2.State = state2;
            }

            //the bounds.z is smaller than bounds.x
            else
            {
              Mesh mesh1 = new Mesh();
              Vector3[] vertices1 = new Vector3[4];

             vertices1[0] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z);
             vertices1[1] = new Vector3(currentRoom.vertices[0].x+bounds.size.x*num1, 0, currentRoom.vertices[0].z);
             vertices1[2] = new Vector3(currentRoom.vertices[0].x + bounds.size.x*num1, 0, currentRoom.vertices[0].z + bounds.size.z);
             vertices1[3] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z+bounds.size.z);

             mesh1.vertices = vertices1;
             mesh1.triangles = new int[] { 0, 3, 2, 0, 2, 1};
             mesh1.RecalculateNormals();
             mesh1.RecalculateTangents();
             child1.Rec = mesh1;
             //////////////////////////////////////////////////////////

             Mesh mesh2 = new  Mesh();
             Vector3[] vertices2 = new Vector3[4];

             vertices2[0] = new Vector3(currentRoom.vertices[0].x + bounds.size.x*num1, 0, currentRoom.vertices[0].z);
             vertices2[1] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z);
             vertices2[2] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z+bounds.size.z);
             vertices2[3] = new Vector3(currentRoom.vertices[0].x + bounds.size.x*num1, 0, currentRoom.vertices[0].z+bounds.size.z );

             mesh2.vertices = vertices2;
             mesh2.triangles = new int[] { 0,3, 2, 0, 2, 1 };

             mesh2.RecalculateNormals();
             mesh2.RecalculateTangents();
             child2.Rec = mesh2;

             child1.State = state1; child2.State = state2;
             
            }
            LongestSide.Add(child1); LongestSide.Add(child2);
             
            return LongestSide;
        }
        public List<Room> ShortestSide(Mesh currentRoom, float num1, bool state1, bool state2)
        {
            Room child1 = new Room(); Room child2 = new Room();
            List<Room> ShortestSide = new List<Room>();
            Bounds bounds = currentRoom.bounds;

        if (bounds.size.z <= bounds.size.x)
            {
            Mesh mesh1 = new Mesh();
            Vector3[] vertices1 = new Vector3[4];

            vertices1[0] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z);
            vertices1[1] = new Vector3(currentRoom.vertices[0].x + bounds.size.x,0, currentRoom.vertices[0].z) ;
            vertices1[2] = new Vector3(currentRoom.vertices[0].x + bounds.size.x, 0, currentRoom.vertices[0].z +bounds.size.z*num1);
            vertices1[3] = new Vector3(currentRoom.vertices[0].x , 0, currentRoom.vertices[0].z + bounds.size.z * num1);

            mesh1.vertices = vertices1;
            mesh1.triangles = new int[] { 0, 3, 2, 0, 2, 1 };
            mesh1.RecalculateNormals();
            mesh1.RecalculateTangents();

            child1.Rec = mesh1;
            /////////////////////////////////////////////////////////////
           
            Mesh mesh2 = new Mesh();
            Vector3[] vertices2 = new Vector3[4];

            vertices2[0] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z+bounds.size.z*num1 );
            vertices2[1] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z + bounds.size.z * num1);
            vertices2[2] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z+bounds.size.z);
            vertices2[3] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z +bounds.size.z);

            mesh2.vertices = vertices2;
            mesh2.triangles = new int[] { 0, 3, 2, 0, 2, 1 };
            mesh2.RecalculateNormals();
            mesh2.RecalculateTangents();

            child2.Rec = mesh2;

            child1.State = state1; child2.State = state2;
        }
            else
            {

             Mesh mesh1 = new Mesh();
             Vector3[] vertices1 = new Vector3[4];

             vertices1[0] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z);
             vertices1[1] = new Vector3(currentRoom.vertices[0].x + bounds.size.x * num1, 0, currentRoom.vertices[0].z);
             vertices1[2] = new Vector3(currentRoom.vertices[0].x + bounds.size.x * num1, 0, currentRoom.vertices[0].z+bounds.size.z);
             vertices1[3] = new Vector3(currentRoom.vertices[0].x, 0, currentRoom.vertices[0].z + bounds.size.z);

             mesh1.vertices = vertices1;
             mesh1.triangles = new int[] { 0, 3, 2, 0, 2, 1 };
             mesh1.RecalculateNormals();
             mesh1.RecalculateTangents();
             child1.Rec = mesh1;
            /////////////////////////////////////////////////////////////////////////
             Mesh mesh2 = new Mesh();

             Vector3[] vertices2 = new Vector3[4];

             vertices2[0] = new Vector3(currentRoom.vertices[0].x + bounds.size.x * num1, 0, currentRoom.vertices[0].z);
             vertices2[1] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z);
             vertices2[2] = new Vector3(currentRoom.vertices[0].x+bounds.size.x, 0, currentRoom.vertices[0].z+bounds.size.z);
             vertices2[3] = new Vector3(currentRoom.vertices[0].x +bounds.size.x * num1, 0, currentRoom.vertices[0].z + bounds.size.z);

             mesh2.vertices = vertices2;
             mesh2.triangles = new int[] { 0, 3, 2, 0, 2,1 };
             mesh2.RecalculateNormals();
             mesh2.RecalculateTangents();
             child2.Rec = mesh2;

             child1.State = state1; child2.State = state2;
            }           
            ShortestSide.Add(child1); ShortestSide.Add(child2);
            return ShortestSide;
        }
    
}
