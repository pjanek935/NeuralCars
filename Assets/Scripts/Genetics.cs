using UnityEngine;
using System.Collections;
using Assets;
using System;

public class Genetics 
{
    public enum CrossType { ARYTM, ONE_POINT, REGULAR }
    public enum MutationType { REGULAR, IRREGULAR }

    System.Random rand = new System.Random((int) DateTime.Now.Ticks);

    public void Crossover(double[] parent1, double[] parent2,
        out double[] child1, out double[] child2, CrossType type)
    {
        int size = parent1.Length;
        child1 = new double[size];
        child2 = new double[size];

        switch (type)
        {
            case CrossType.ARYTM:

                arytmCrossover(parent1, parent2, child1, child2);

                break;

            case CrossType.ONE_POINT:

                onePointCrossover(parent1, parent2, child1, child2);

                break;

            case CrossType.REGULAR:

                regularCrossover(parent1, parent2, child1, child2);

                break;

            default:

                break;
        }

    }

    void arytmCrossover(double[] parent1, double[] parent2,
        double[] child1, double[] child2)
    {
        int size = parent1.Length;
        double alpha = rand.NextDouble();

        for (int i = 0; i < size; i++)
        {
            child1[i] = alpha * parent1[i] + (1 - alpha) * parent2[i];
            child2[i] = alpha * parent2[i] + (1 - alpha) * parent1[i];
        }
    }

    void regularCrossover(double[] parent1, double[] parent2,
        double[] child1, double[] child2)
    {
        int size = parent1.Length;
        bool flag = true;

        for (int i = 0; i < size; i++)
        {
            if (flag)
            {
                child1[i] = parent1[i];
                child2[i] = parent2[i];
            }
            else
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }

            flag = ! flag;
        }
    }

    void onePointCrossover(double[] parent1, double[] parent2,
        double[] child1, double[] child2)
    {
        int size = parent1.Length;
        int crossPoint = rand.Next(0, size);

        for (int i = 0; i < crossPoint; i++)
        {
            child1[i] = parent1[i];
            child2[i] = parent2[i];
        }

        for (int i = crossPoint; i < size; i++)
        {
            child1[i] = parent2[i];
            child2[i] = parent1[i];
        }
    }

    public void Mutation(double[] chromosome, float probability)
    {
        for (int i = 0; i < chromosome.Length; i++)
        {
            double mutate = rand.NextDouble ();

            if (mutate < probability)
            {
                double r = rand.NextDouble () / 2 - 0.25f;
                chromosome[i] += r;

                if (chromosome[i] > 1)
                {
                    chromosome[i] = 1;
                }
                else if (chromosome[i] < -1)
                {
                    chromosome[i] = -1;
                }
            }
        }
    }

    public void Mutation2(double[] chromosome, float probability, float width)
    {
        for (int i = 0; i < chromosome.Length; i++)
        {
            double mutate = rand.NextDouble ();

            if (mutate < probability)
            {
                double r = rand.NextDouble () * width - width / 2;
                chromosome[i] += r;

                if (chromosome[i] > 1)
                {
                    chromosome[i] = 1;
                }
                else if (chromosome[i] < -1)
                {
                    chromosome[i] = -1;
                }
            }
        }
    }

    public int RouletteSelect(double[] weights)
    {
        //Calculate weights sum
        double weightSum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            weightSum += weights [i];
        }

        //Get random val
        double randVal = rand.NextDouble () * weightSum;

        //Get random index based on weights
        for (int i = 0; i < weights.Length; i++)
        {
            randVal -= weights [i];

            if (randVal <= 0)
            {
                return i;
            }
        }

        return weights.Length - 1;
    }
}
