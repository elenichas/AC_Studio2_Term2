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
    
    public List<char> TheArray = new List<char>();

    //the string as given from the user(no commas expected)
    public string HouseProg;

    //the list of characters taken from the HouseProg
    public List<char> letters;

    public List<Mesh> FinalMeshes;

    //the dictionarywith pairs of individuals and their fitnesses
    public  List <string>  GenomeFitnessPairs;

    public override int CompareTo(object a)
    {
        ListGenome Gene1 = this;
        ListGenome Gene2 = (ListGenome)a;
        if (Gene1.CurrentFitness > Gene2.CurrentFitness)
            return 1;
        else if (Gene1.CurrentFitness < Gene2.CurrentFitness)
            return -1;
        else
            return 0;
    }


    public override void SetCrossoverPoint(int crossoverPoint)
    {
        CrossoverPoint = crossoverPoint;
    }

    public ListGenome()
    {

    }


    public ListGenome(int HouseProgramLength, string HouseProg, List<Mesh> FinalMeshes)
    {
         
        Lengthother = HouseProgramLength;
        this.HouseProg = HouseProg;
        this.FinalMeshes = FinalMeshes;
        
        letters = HouseProg.ToList();

        for (int i = 0; i < Lengthother; i++)
        {
            char gene = GenerateGeneValue();

            TheArray.Add(gene);

        }
    }

    public override void Initialize()
    {

    }

    public override bool CanDie(float fitness)
    {
        if (CurrentFitness >= (fitness))
        {
            return true;
        }

        return false;
    }


    public override bool CanReproduce(float fitness)
    {
        if (CurrentFitness <=(fitness))
        {
            return true;
        }

        return false;
    }


    public override char GenerateGeneValue( )
    {
        char RandRoom = letters[UnityEngine.Random.Range(0, letters.Count)];
        letters.Remove(RandRoom);

        return RandRoom;

    }

    public override void Mutate()
    {
        var GeneToMutate = UnityEngine.Random.Range(0, HouseProg.Length);
        var geneToFlipWith = Enumerable.Range(0, HouseProg.Length)
            .Except(new List<int> { GeneToMutate }).OrderBy(x => ListGenome.TheSeed.NextDouble()).First();

        var g1 = TheArray[GeneToMutate];
        var g2 = TheArray[geneToFlipWith];
        TheArray[GeneToMutate] = g2;
        TheArray[geneToFlipWith] = g1;

    }

    private float MeshArea(Mesh mesh)
    {
        float Area = mesh.bounds.size.x * mesh.bounds.size.z;

        return Area;

    }


    private float CalculateHouseFitnessSum()
    {
        float total_fitness = 0;
        //Debug.Log("number of meshes  " + FinalMeshes.Count + "  number of letters" + TheArray.Count);
        for (int i = 0; i < TheArray.Count; i++)
        {
            switch (TheArray[i])
            {
                //kitchen
                case 'k':

                    float idealK = 30;
                    float areak = MeshArea(FinalMeshes[i]);
                    float rfk = Math.Abs(idealK - areak);
                    total_fitness += rfk;
                    break;

                //living room
                case 'l':

                    float idealL = 25;
                    float areal = MeshArea(FinalMeshes[i]);
                    float rfl = Math.Abs(idealL - areal);
                    total_fitness += rfl;
                    break;

                //bedroom
                case 'b':

                    float idealB = 20;
                    float areab = MeshArea(FinalMeshes[i]);
                    float rfb = Math.Abs(idealB - areab);
                    total_fitness += rfb;
                    break;

                //bathroom wc
                case 'w':

                    float idealW = 4;
                    float areaw = MeshArea(FinalMeshes[i]);
                    float rfw = Math.Abs(idealW - areaw);
                    total_fitness += rfw;
                    break;

                //office-workspace
                case 'o':

                    float idealO = 10;
                    float areao = MeshArea(FinalMeshes[i]);
                    float rfo = Math.Abs(idealO - areao);
                    total_fitness += rfo;
                    break;


                //storage
                case 's':

                    float idealS = 2.5f;
                    float areas = MeshArea(FinalMeshes[i]);
                    float rfs = Math.Abs(idealS - areas);
                    total_fitness += rfs;
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

    public override string ToMyString()
    {
        string strResult = "";
        for (int i = 0; i < Lengthother; i++)
        {
            strResult = strResult + (TheArray[i]).ToString();
        }

        strResult += " " + CurrentFitness;
        return strResult;
            
    }
    public override string ToMyStringOnlyG()
    {
        string strResult = "";
        for (int i = 0; i < Lengthother; i++)
        {
            strResult = strResult + (TheArray[i]).ToString();
        }

       
        return strResult;

    }

    public override void CopyGeneInfo(Genome dest)
    {
        ((ListGenome)dest).FinalMeshes = this.FinalMeshes;
        ((ListGenome)dest).CrossoverPoint = this.CrossoverPoint;
        ((ListGenome)dest).CurrentFitness = this.CurrentFitness;
        ((ListGenome)dest).HouseProg = this.HouseProg;
        ((ListGenome)dest).Lengthother = this.Lengthother;
        ((ListGenome)dest).letters = this.letters;
        ((ListGenome)dest).MutationIndex = this.MutationIndex;
        ((ListGenome)dest).TheArray = this.TheArray;

    }

    public override Genome Crossover(Genome g)
    { 

        ListGenome CrossingGene = new ListGenome();

        CopyGeneInfo(CrossingGene);
        var crossOverPt = (int)TheArray.Count / 2;
        var coin = ListGenome.TheSeed.NextDouble();
        for (int i = 0; i < crossOverPt; i++)
        {
            if (coin < 0.5)
            {
                CrossingGene.TheArray[i] = this.TheArray[i];
            }
            else
            {
                CrossingGene.TheArray[i] = ((ListGenome)g).TheArray[i];

            }
        }
        for (int i = crossOverPt; i < TheArray.Count; i++)
        {
            if (coin > 0.5)
            {
                CrossingGene.TheArray[i] = this.TheArray[i];
            }
            else
            {
                CrossingGene.TheArray[i] = ((ListGenome)g).TheArray[i];

            }
        }

        return CrossingGene;
    }

}










