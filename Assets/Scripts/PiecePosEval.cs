using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PiecePosEval : MonoBehaviour
{
	public readonly static Dictionary<int, int[,]> PositionEval;
	public static Dictionary<string, int> PosWeights;

	static PiecePosEval()
	{
		PositionEval = new Dictionary<int, int[,]>();
		PosWeights = new Dictionary<string, int>();
		// Rank "neutral"
		PosWeights["+0"] = 0;
		PosWeights["-0"] = 0;

		// Rank "good"
		PosWeights["+1"] = 5;
		PosWeights["+2"] = 10;
		PosWeights["+3"] = 20;
		PosWeights["+4"] = 30;
		PosWeights["+5"] = 50;

		// Rank "poor"
		PosWeights["-1"] = -5;
		PosWeights["-2"] = -10;
		PosWeights["-3"] = -20;
		PosWeights["-4"] = -30;
		PosWeights["-5"] = -50;

		string filename;

		// Sylph positions
		filename = "Assets/Files/PieceSquareTables/sylph_ps_table.txt";
		InitFromFile(0, filename);

		// Griffon positions
		filename = "Assets/Files/PieceSquareTables/griffon_ps_table.txt";
		InitFromFile(1, filename);

		// Dragon positions
		filename = "Assets/Files/PieceSquareTables/dragon_ps_table.txt";
		InitFromFile(2, filename);

		// Warrior positions
		filename = "Assets/Files/PieceSquareTables/warrior_ps_table.txt";
		InitFromFile(3, filename);

		// Oliphant positions
		filename = "Assets/Files/PieceSquareTables/oliphant_ps_table.txt";
		InitFromFile(4, filename);

		// Unicorn positions
		filename = "Assets/Files/PieceSquareTables/unicorn_ps_table.txt";
		InitFromFile(5, filename);

		// Hero positions
		filename = "Assets/Files/PieceSquareTables/hero_ps_table.txt";
		InitFromFile(6, filename);

		// Thief positions
		filename = "Assets/Files/PieceSquareTables/thief_ps_table.txt";
		InitFromFile(7, filename);

		// Cleric positions
		filename = "Assets/Files/PieceSquareTables/cleric_ps_table.txt";
		InitFromFile(8, filename);

		// Mage positions
		filename = "Assets/Files/PieceSquareTables/mage_ps_table.txt";
		InitFromFile(9, filename);

		// King positions
		filename = "Assets/Files/PieceSquareTables/king_ps_table.txt";
		InitFromFile(10, filename);

		// Paladin positions
		filename = "Assets/Files/PieceSquareTables/paladin_ps_table.txt";
		InitFromFile(11, filename);

		// Dwarf positions
		filename = "Assets/Files/PieceSquareTables/dwarf_ps_table.txt";
		InitFromFile(12, filename);

		// Basilisk positions
		filename = "Assets/Files/PieceSquareTables/basilisk_ps_table.txt";
		InitFromFile(13, filename);

		// Elemental positions
		filename = "Assets/Files/PieceSquareTables/elemental_ps_table.txt";
		InitFromFile(14, filename);

	}

	public static void InitFromFile(int type, string filename)
	{
		PositionEval[type] = new int[3, 96];
		string[] lines = File.ReadAllLines(filename);
		
		if (lines.Length != 29)
		{
			print("error: incorrect file format: " + filename);
			return;
		}

		// uppermost board
		for (int r = 0; r < 8; r++)
		{
			string[] row = lines[r+1].Split(' ');
			// Check the file is in the right format
			if (row.Length != 12)
			{
				print("error: incorrect row length in file: " + filename);
				return;
			}
			for (int c = 0; c < 12; c++)
			{
				PositionEval[type][0, r*12+c] = PosWeights[row[c]];
			}
		}

		// middle board
		for (int r = 0; r < 8; r++)
		{
			string[] row = lines[r+11].Split(' ');
			// Check the file is in the right format
			if (row.Length != 12)
			{
				print("error: incorrect row length in file: " + filename);
				return;
			}
			for (int c = 0; c < 12; c++)
			{
				PositionEval[type][1, r * 12 + c] = PosWeights[row[c]];
			}
		}

		// lowermost board
		for (int r = 0; r < 8; r++)
		{
			string[] row = lines[r+21].Split(' ');
			// Check the file is in the right format
			if (row.Length != 12)
			{
				print("error: incorrect row length in file: " + filename);
				return;
			}
			for (int c = 0; c < 12; c++)
			{
				PositionEval[type][2, r * 12 + c] = PosWeights[row[c]];
			}
		}
	}
}
