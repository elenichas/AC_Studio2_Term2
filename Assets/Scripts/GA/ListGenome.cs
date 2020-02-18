using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 

 
	/// <summary>
	/// Summary description for ListGenome.
	/// </summary>
	public class ListGenome : Genome
	{
        public static System.Random TheSeed = new System.Random((int)DateTime.Now.Ticks);
        List<char> TheArray = new List<char>();
      
		int TheMin = 0;
		int TheMax = 10;

       public string HouseProg;
       public List<Mesh> FinalMeshes;

    List<char> letters;

    public override int CompareTo(object a)
		{
			ListGenome Gene1 = this;
			ListGenome Gene2 = (ListGenome)a;
			return Math.Sign(Gene2.CurrentFitness  -  Gene1.CurrentFitness);
		}


		public override void SetCrossoverPoint(int crossoverPoint)
		{
			CrossoverPoint = 	crossoverPoint;
		}

		public ListGenome()
		{

		}


		public ListGenome(int HouseProgramLength,string HouseProg,List<Mesh>FinalMeshes)
		{
              
          Length = HouseProgramLength;
           this.HouseProg = HouseProg;
           this.FinalMeshes = FinalMeshes;
           letters = HouseProg.ToList();

        for (int i = 0; i < Length; i++)
			{
			   char gene = GenerateGeneValue(HouseProg);
              
			   TheArray.Add(gene);
              
			}
		}

		public override void Initialize()
		{

		}

		public override bool CanDie(float fitness)
		{
			if (CurrentFitness >= (int)(fitness * 100.0f))
			{
				return true;
			}

			return false;
		}


		public override bool CanReproduce(float fitness)
		{
			if (ListGenome.TheSeed.Next(100) <= (int)(fitness * 100.0f))
			{
				return true;
			}

			return false;
		}


		public override char GenerateGeneValue(string HouseProg)
		{
          
         
         char RandRoom = letters[UnityEngine.Random.Range(0, letters.Count)];
         letters.Remove(RandRoom);
           
           return RandRoom;
		}

		public override void Mutate(string HouseProg)
		{
			MutationIndex = UnityEngine.Random.Range(1,HouseProg.Length);
            TheArray.Reverse(MutationIndex-1,MutationIndex);
			//char val = GenerateGeneValue(HouseProg );
			//TheArray[MutationIndex] = val;

		}

       private float MeshArea(Mesh mesh)
       {
        float Area = mesh.bounds.size.x * mesh.bounds.size.y;

        return Area;

       }

		 
       private float CalculateHouseFitnessSum()
        {
          float total_fitness = 0;
        for (int i = 0; i < TheArray.Count; i++)
        { 
            switch(TheArray[i])
            {
                case 'k':
             
                float idealK = 400;
                float areak = MeshArea(FinalMeshes[i]);
                float rfk = Math.Abs(idealK - areak);
                total_fitness += rfk;
                    break;

                case 'l':
           
                float idealL = 144;
               float areal = MeshArea(FinalMeshes[i]); 
                float rfl = Math.Abs(idealL - areal);
                total_fitness += rfl;
                    break;

                case 'b':
            
                float idealB = 80;
                float areab = MeshArea(FinalMeshes[i]); ;
                float rfb = Math.Abs(idealB - areab);
                total_fitness += rfb;
                    break;

                case 'w':
             
                float idealW = 72;
                float areaw = MeshArea(FinalMeshes[i]); ;
                float rfw = Math.Abs(idealW - areaw);
                total_fitness += rfw;
                    break;

                case 'o':
            
                float idealO = 90;
                float areao = MeshArea(FinalMeshes[i]); ;
                float rfo = Math.Abs(idealO - areao);
                total_fitness += rfo;
                    break;


                case 's':
            
                float idealS = 70;
                float areas = MeshArea(FinalMeshes[i]); 
               float rfs = Math.Abs(idealS - areas);
                total_fitness += rfs;
                    break;

                case 't':
            
                float idealY = 65;
                float areay = MeshArea(FinalMeshes[i]); 
                float rfy = Math.Abs(idealY - areay);
                total_fitness += rfy;
                    break;
            }
        }


          return total_fitness;
        }

		public override float CalculateFitness()
		{
          CurrentFitness = CalculateHouseFitnessSum();
 
			return CurrentFitness;
		}

		public override string ToString()
		{
			string strResult = "";
			for (int i = 0; i < Length; i++)
			{
			  strResult = strResult + (TheArray[i]).ToString() + " ";
			}

			strResult += "-->" + CurrentFitness.ToString();

			return strResult;
		}

		public override void CopyGeneInfo(Genome dest)
		{
			ListGenome theGene = (ListGenome)dest;
			theGene.Length = Length;
			theGene.TheMin = TheMin;
			theGene.TheMax = TheMax;
		}


    }
 
