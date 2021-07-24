using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
	public List<Genome> genomes;
	public List<Genome> lastGenerationGenomes;


	public int populationSize = 500;
	public double crossoverRate = 0.7f;
	public double mutationRate = 0.001f;
	public int chromosomeLength = 70;
	public int geneLength = 2;
	public int fittestGenome;
	public double bestFitnessScore;
	public double totalFitnessScore;
	public int generation;
	public bool busy;

	public GeneticAlgorithm()
	{
		busy = false;
		genomes = new List<Genome>();
		lastGenerationGenomes = new List<Genome>();
	}

	public void Mutate(List<int> bits)
	{
		for (int i = 0; i < bits.Count; i++)
		{

			if (UnityEngine.Random.value < mutationRate)
			{

				bits[i] = bits[i] == 0 ? 1 : 0;
			}
		}
	}

	public void Crossover(List<int> mom, List<int> dad, List<int> baby1, List<int> baby2)
	{
		if (UnityEngine.Random.value > crossoverRate || mom == dad)
		{
			baby1.AddRange(mom);
			baby2.AddRange(dad);

			return;
		}

		System.Random rnd = new System.Random();

		int crossoverPoint = rnd.Next(0, chromosomeLength - 1);

		for (int i = 0; i < crossoverPoint; i++)
		{
			baby1.Add(mom[i]);
			baby2.Add(dad[i]);
		}

		for (int i = crossoverPoint; i < mom.Count; i++)
		{
			baby1.Add(dad[i]);
			baby2.Add(mom[i]);
		}
	}

	public Genome RouletteWheelSelection()
	{
		double slice = UnityEngine.Random.value * totalFitnessScore;
		double total = 0;
		int selectedGenome = 0;

		for (int i = 0; i < populationSize; i++)
		{
			total += genomes[i].fitness;

			if (total > slice)
			{
				selectedGenome = i;
				break;
			}
		}
		return genomes[selectedGenome];
	}

	public void UpdateFitnessScores()
	{
		fittestGenome = 0;
		bestFitnessScore = 0;
		totalFitnessScore = 0;

		for (int i = 0; i < populationSize; i++)
		{

			totalFitnessScore += genomes[i].fitness;

			if (genomes[i].fitness > bestFitnessScore)
			{
				bestFitnessScore = genomes[i].fitness;
				fittestGenome = i;

				if (genomes[i].fitness == 1)
				{
					busy = false;
					return;
				}
			}
		}
	}

	public void CreateStartPopulation()
	{
		genomes.Clear();

		for (int i = 0; i < populationSize; i++)
		{
			Genome baby = new Genome(chromosomeLength);
			genomes.Add(baby);
		}
	}

	public void Run()
	{
		CreateStartPopulation();
		busy = true;
	}

	public void Epoch()
	{
		if (!busy) return;
		UpdateFitnessScores();

		if (!busy)
		{
			lastGenerationGenomes.Clear();
			lastGenerationGenomes.AddRange(genomes);
			return;
		}

		int numberOfNewBabies = 0;

		List<Genome> babies = new List<Genome>();
		while (numberOfNewBabies < populationSize)
		{
			// select 2 parents
			Genome mom = RouletteWheelSelection();
			Genome dad = RouletteWheelSelection();
			Genome baby1 = new Genome();
			Genome baby2 = new Genome();
			Crossover(mom.bits, dad.bits, baby1.bits, baby2.bits);
			Mutate(baby1.bits);
			Mutate(baby2.bits);
			babies.Add(baby1);
			babies.Add(baby2);

			numberOfNewBabies += 2;
		}


		lastGenerationGenomes.Clear();
		lastGenerationGenomes.AddRange(genomes);
		genomes = babies;

		generation++;
	}
}
