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

    private double MeshArea(Mesh mesh)
    {
       double Area = mesh.bounds.size.x * mesh.bounds.size.z;

        return Area;

    }

    public double DoTheMaths(double x,double z,double minsize,double maxsize,double area,double prop,double area_this)
    {
        double f = 0;
        if (x < z)
        {
            f += Math.Pow(minsize - x, 2);
            f += Math.Pow(maxsize - z, 2);
            f += Math.Pow(prop - x / z, 2);
        }
        else
        {
            f += Math.Pow(minsize - z, 2);
            f += Math.Pow(maxsize - x, 2);
            f += Math.Pow(prop - z / x, 2);
        }
          f += Math.Pow(area - area_this, 2);

        return f;
    }

    private double CalculateHouseFitnessSum()
    {
        double total_fitness = 0;
        
        for (int i = 0; i < TheArray.Count; i++)
        {
            switch (TheArray[i])
            {
                //kitchen
                case 'k':
                   
                   double kminsize = 2.81;
                   double kmaxsize = 3.42;
                   double karea = 9.68;
                   double kprop = 0.93;
                   double kx = FinalMeshes[i].bounds.size.x;
                   double kz = FinalMeshes[i].bounds.size.z;
                   double karea_this = MeshArea(FinalMeshes[i]);

                   total_fitness += DoTheMaths( kx, kz,kminsize,kmaxsize, karea,kprop, karea_this);                
                    break;

                //living room
                case 'l':
                    double lminsize = 4.19;
                    double lmaxsize = 5.83;
                    double larea = 24.37;
                    double lprop = 0.83;
                    double lx = FinalMeshes[i].bounds.size.x;
                    double lz = FinalMeshes[i].bounds.size.z;
                    double larea_this = MeshArea(FinalMeshes[i]);

                    total_fitness += DoTheMaths(lx, lz,lminsize, lmaxsize, larea, lprop, larea_this);
                    break;

                //bedroom
                case 'b':
                    double bminsize = 3.83;
                    double bmaxsize = 4.86;
                    double barea = 18.7;
                    double bprop = 0.86;
                    double bx = FinalMeshes[i].bounds.size.x;
                    double bz = FinalMeshes[i].bounds.size.z;
                    double barea_this = MeshArea(FinalMeshes[i]);

                    total_fitness += DoTheMaths(bx, bz, bminsize, bmaxsize, barea, bprop, barea_this);
                    break;

                //bathroom wc
                case 'w':
                    double wminsize = 1.62;
                    double wmaxsize = 2.77;
                    double warea = 4.46;
                    double wprop = 0.86;
                    double wx = FinalMeshes[i].bounds.size.x;
                    double wz = FinalMeshes[i].bounds.size.z;
                    double warea_this = MeshArea(FinalMeshes[i]);

                    total_fitness += DoTheMaths(wx, wz, wminsize, wmaxsize, warea, wprop, warea_this);
                    break;                  

                //office-workspace
                case 'o':
                    double ominsize = 3.42;
                    double omaxsize = 4.07;
                    double oarea = 13.95;
                    double oprop = 0.9;
                    double ox = FinalMeshes[i].bounds.size.x;
                    double oz = FinalMeshes[i].bounds.size.z;
                    double oarea_this = MeshArea(FinalMeshes[i]);

                    total_fitness += DoTheMaths(ox, oz, ominsize, omaxsize, oarea, oprop, oarea_this);

                    break;
                //storage
                case 's':
                    double sminsize =0.6;
                    double smaxsize = 3.09;
                    double sarea = 1.85;
                    double sprop = 0.86;
                    double sx = FinalMeshes[i].bounds.size.x;
                    double sz = FinalMeshes[i].bounds.size.z;
                    double sarea_this = MeshArea(FinalMeshes[i]);

                    total_fitness += DoTheMaths(sx, sz, sminsize, smaxsize, sarea, sprop, sarea_this);

                    break;
            }
        }
        return total_fitness;
    }

    public override double CalculateFitness()
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

        strResult += " " + (int)CurrentFitness;
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

        g.CopyGeneInfo(CrossingGene);

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










