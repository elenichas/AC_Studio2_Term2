using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for Genome.
/// </summary>
public abstract class Genome : IComparable
	{
    public long Lengthother;
    public int CrossoverPoint;
    public int MutationIndex;
    public float CurrentFitness = 0.0f;
    abstract public void Initialize();
    abstract public void Mutate( );
    abstract public Genome Crossover(Genome g);
    abstract public char GenerateGeneValue( );
    abstract public float CalculateFitness();
    abstract public bool CanReproduce(float fitness);
    abstract public void SetCrossoverPoint(int crossoverPoint);
    abstract public bool CanDie(float fitness);
    abstract public string ToMyString();
    abstract public string ToMyStringOnlyG();
    abstract public void CopyGeneInfo(Genome g);
    abstract public int CompareTo(object a);


}

