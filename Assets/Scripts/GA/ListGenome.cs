using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



// based on :https://www.c-sharpcorner.com/article/implementing-a-genetic-algorithms-in-C-Sharp-and-net/
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

    //public override int CompareTo(object a)
  //  {
    //    ListGenome Gene1 = this;
   //     ListGenome Gene2 = (ListGenome)a;
   //     if (Gene1.CurrentFitness > Gene2.CurrentFitness)
   //         return 1;
   //     else if (Gene1.CurrentFitness < Gene2.CurrentFitness)
   //         return -1;
   //     else
   //         return 0;
  //  }
    public override int CompareTo(object a)
    {
        ListGenome Gene1 = this;
        ListGenome Gene2 = (ListGenome)a;
        return Math.Sign(Gene2.CurrentFitness - Gene1.CurrentFitness);
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


    public override char GenerateGeneValue()
    {
        char RandRoom = letters[UnityEngine.Random.Range(0, letters.Count)];
        letters.Remove(RandRoom);
        return RandRoom;
    }

    //ORDERED CHANGING MUTATION(just take 2 random and flip them)
    public override void Mutate()
    {
        var GeneToMutate = UnityEngine.Random.Range(0, TheArray.Count);
        var geneToFlipWith = Enumerable.Range(0, TheArray.Count)
            .Except(new List<int> { GeneToMutate }).OrderBy(x => ListGenome.TheSeed.NextDouble()).First();

        var g1 = TheArray[GeneToMutate];
        var g2 = TheArray[geneToFlipWith];
        TheArray[GeneToMutate] = g2;
        TheArray[geneToFlipWith] = g1;
    }
    public override void MutateOther()
    {
        var GeneToMutate = UnityEngine.Random.Range(0, TheArray.Count);
       // var geneToFlipWith = 0;
        if (GeneToMutate == TheArray.Count)
        {
           var geneToFlipWith = GeneToMutate - 1;
            var g2 = TheArray[geneToFlipWith];
            var g1 = TheArray[GeneToMutate];
        }
        else
        {
           var geneToFlipWith = GeneToMutate + 1;
            var g2 = TheArray[geneToFlipWith];
            var g1 = TheArray[GeneToMutate];
        }
           

       
       
         
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
            f += Math.Pow(minsize - x,2);
            f += Math.Pow(maxsize - z,2);
            f += Math.Pow(prop - x / z,2);
        }
        else
        {
            f += Math.Pow(minsize - z,2);
            f += Math.Pow(maxsize - x,2);
            f += Math.Pow(prop - z / x,2);
        }
          f += Math.Abs(area - area_this);
       
        return f;
    }

    private double CalculateHouseFitnessSum()
    {
        double total_fitness = 0;
        double karea_this = 0;
        double larea_this = 0;
        double barea_this = 0;
        double warea_this = 0;
        double oarea_this = 0;
        double narea_this = 0;
        double sarea_this = 0;
        double harea_this = 0;

        //proximity
        for (int i = 0; i < TheArray.Count - 1; i++)
        {
            if ((TheArray[i] == 'k') & (TheArray[i + 1] == 'l'))
            {
                total_fitness -= 1000;
               // Debug.Log("yes");
            }
            
        }
        for (int i = 0; i < TheArray.Count - 1; i++)
        {
            if ((TheArray[i] == 'l') & (TheArray[i + 1] == 'k'))
            {
                total_fitness -= 1000;
               // Debug.Log("yes2");
            }

        }



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
                   karea_this = MeshArea(FinalMeshes[i]);
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
                    larea_this = MeshArea(FinalMeshes[i]);
                    total_fitness += DoTheMaths(lx, lz,lminsize, lmaxsize, larea, lprop, larea_this);
                    
                    //penalty for too big
                   if (larea_this > larea)
                       total_fitness += 1000;
                    //penalty for too small
                   else if (larea_this < larea)
                        total_fitness += 1000;
                    break;

                //bedroom
                case 'b':
                    double bminsize = 3.83;
                    double bmaxsize = 4.86;
                    double barea = 18.7;
                    double bprop = 0.86;
                    double bx = FinalMeshes[i].bounds.size.x;
                    double bz = FinalMeshes[i].bounds.size.z;
                    barea_this = MeshArea(FinalMeshes[i]);
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
                    warea_this = MeshArea(FinalMeshes[i]);
                    total_fitness += DoTheMaths(wx, wz, wminsize, wmaxsize, warea, wprop, warea_this);
                    //penalty for too big
                    if (warea_this >  warea)
                        total_fitness += 1000;
                    //penalty for too small
                   else if (warea_this < warea)
                        total_fitness += 1000;
                    break;                  

                //office-workspace
                case 'o':
                    double ominsize = 3.42;
                    double omaxsize = 4.07;
                    double oarea = 13.95;
                    double oprop = 0.9;
                    double ox = FinalMeshes[i].bounds.size.x;
                    double oz = FinalMeshes[i].bounds.size.z;
                    oarea_this = MeshArea(FinalMeshes[i]);
                    total_fitness += DoTheMaths(ox, oz, ominsize, omaxsize, oarea, oprop, oarea_this);
                    break;

                //nursery
                case 'n':
                    double nminsize =2.9;
                    double nmaxsize = 3.0;
                    double narea = 5.0;
                    double nprop = 0.86;
                    double nx = FinalMeshes[i].bounds.size.x;
                    double nz = FinalMeshes[i].bounds.size.z;
                    narea_this = MeshArea(FinalMeshes[i]);
                    total_fitness += DoTheMaths(nx, nz, nminsize, nmaxsize, narea, nprop, narea_this);
                    break;

                //storage
                case 's':
                    double sminsize = 1.5;
                    double smaxsize = 2.0;
                    double sarea = 3.0;
                    double sprop = 0.9;
                    double sx = FinalMeshes[i].bounds.size.x;
                    double sz = FinalMeshes[i].bounds.size.z;
                    sarea_this = MeshArea(FinalMeshes[i]);
                    total_fitness += DoTheMaths(sx, sz, sminsize, smaxsize, sarea, sprop, sarea_this);
                    if (sarea_this > sarea)
                        total_fitness += 1000;
                    break;

                //help space,laundy etc
                case 'h':
                    double hminsize = 1.5;
                    double hmaxsize = 2.0;
                    double harea = 2.0;
                    double hprop = 0.9;
                    double hx = FinalMeshes[i].bounds.size.x;
                    double hz = FinalMeshes[i].bounds.size.z;
                    harea_this = MeshArea(FinalMeshes[i]);
                   total_fitness += DoTheMaths(hx, hz, hminsize, hmaxsize, harea, hprop, harea_this);
                    if (harea_this > harea)
                        total_fitness += 1000;
                    break;
            }
            //extra penalties for room area comparison          
            if (warea_this > larea_this)
               total_fitness += 1000;
           else if (warea_this > barea_this)
                total_fitness += 1000;
            else if(harea_this>larea_this)
               
                total_fitness += 1000;
            if (narea_this > barea_this)
               total_fitness += 1000;
        }
        //remaping (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        // return (total_fitness - 0) / (30000 - 0) * (100- 0) + 0; 
        
        return total_fitness / 100;
       
    }
    public override double CalculateFitness()
    {
        CurrentFitness = CalculateHouseFitnessSum();
        return CurrentFitness;
    }

    public override int ToMyStringOnlyF()
    {      
        return (int)CurrentFitness;           
    }
    public override string ToMyStringOnlyG()
    {
        string strResult = "";
        for (int i = 0; i < TheArray.Count; i++)
        {
            strResult = strResult + (TheArray[i]).ToString();
        }
        return strResult;
    }

    public override void CopyGeneInfo(Genome dest)
    {
        ((ListGenome)dest).FinalMeshes = this.FinalMeshes;
        ((ListGenome)dest).CrossoverPoint = this.CrossoverPoint;
        //((ListGenome)dest).CurrentFitness = this.CurrentFitness;
        ((ListGenome)dest).HouseProg = this.HouseProg;
        ((ListGenome)dest).Lengthother = this.Lengthother;
        ((ListGenome)dest).letters = this.letters;
        //((ListGenome)dest).MutationIndex = this.MutationIndex;
        //((ListGenome)dest).TheArray = this.TheArray;

    }

    //THE CROSSOVER THE ORIGINAL ALGORITHM HAD (NOOOP)
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

    //MY SUPER DUPER AMAZING ORDERED CROSSOVER NO PUBLICATES GUARANTEE
    public override Genome OrderedCrossover(Genome g)
    {
        ListGenome CrossingGene = new ListGenome();
         
        g.CopyGeneInfo(CrossingGene);

        var crossOverPt = (int)TheArray.Count / 2;
       
        //take half of genes from the existing parent
        for (int i = 0; i < crossOverPt; i++)
        {          
            CrossingGene.TheArray.Add (this.TheArray[i]);                
        }

        //take the other half from the parent g passed as a parameter
        //if the letter doesn't exist add it with the order it exist to parent g
        for (int i = crossOverPt; i< TheArray.Count; i++)
        {
            for (int j = 0; j < ((ListGenome)g).TheArray.Count; j++)
            {               
                if (!(CrossingGene.TheArray.Contains(((ListGenome)g).TheArray[j])))
                    CrossingGene.TheArray.Add(((ListGenome)g).TheArray[j]);                
            }
        }
         return CrossingGene;
    }

}










