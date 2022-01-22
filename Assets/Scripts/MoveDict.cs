using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess {
	public class MoveDict : MonoBehaviour
	{
		public readonly static Dictionary<string, List<(int, int, int)>[,,]> MoveDictionary;
		static (int, int, int)[] positions;
		static bool[] bMask3 = { false, false, false, true };
		static bool[] bMask2 = { false, false, true, false };
		static bool[] bMask1 = { false, true, false, false };

		static bool isValidPos(int r, int c)
		{
			return (0 <= r && r < 8 && 0 <= c && c < 12);
		}

		static (int, int, int)[] Valid((int, int, int)[] pos)
		{
			List<(int, int, int)> validPos = new List<(int, int, int)>();
			foreach ((int b, int r, int c) in pos)
			{
				if (1 <= b && b <= 3 &&
					0 <= r && r < 8 &&
					0 <= c && c < 12)
					validPos.Add((b, r, c));
			}
			return validPos.ToArray();
		}

		static List<(int, int, int)>[,,] Generate(int rshift, int cshift)
		{
			List<(int, int, int)>[,,] ret = new List<(int, int, int)>[4, 8, 12];

			foreach ((int b, int r, int c) in positions)
			{
				int row = r + rshift;
				int col = c + cshift;
				ret[b, r, c] = new List<(int, int, int)>();
				while (isValidPos(row, col))
				{
					ret[b, r, c].Add((b, row, col));
					row += rshift;
					col += cshift;
				}
			}
			return ret;
		}

		static List<(int, int, int)>[,,] Generate(int rshift, int cshift, bool[] boards)
		{
			List<(int, int, int)>[,,] ret = new List<(int, int, int)>[4, 8, 12];

			foreach ((int b, int r, int c) in positions)
			{
				int row = r + rshift;
				int col = c + cshift;
				ret[b, r, c] = new List<(int, int, int)>();
				while (isValidPos(row, col) && boards[b])
				{
					ret[b, r, c].Add((b, row, col));
					row += rshift;
					col += cshift;
				}
			}
			return ret;
		}

		static List<(int, int, int)>[,,] GenFromList(List<(int, int, int)> shifts)
		{
			List<(int, int, int)>[,,] ret = new List<(int, int, int)>[4, 8, 12];

			foreach ((int b, int r, int c) in positions)
			{
				ret[b, r, c] = new List<(int, int, int)>();
				foreach ((int board, int rshift, int cshift) in shifts)
				{
					int row = r + rshift;
					int col = c + cshift;
					if (isValidPos(row, col))
					{
						ret[b, r, c].Add((board, row, col));
					}
				}
			}
			return ret;
		}

		static List<(int, int, int)>[,,] GenFromList(List<(int, int, int)> shifts, bool[] boards)
		{
			List<(int, int, int)>[,,] ret = new List<(int, int, int)>[4, 8, 12];

			foreach ((int b, int r, int c) in positions)
			{
				ret[b, r, c] = new List<(int, int, int)>();
				if (boards[b])
				{
					foreach ((int board, int rshift, int cshift) in shifts)
					{
						int row = r + rshift;
						int col = c + cshift;
						if (isValidPos(row, col))
						{
							ret[b, r, c].Add((board, row, col));
						}
					}
				}
			}
			return ret;
		}

		static List<(int, int, int)>[,,] Combine(List<List<(int, int, int)>[,,]> rules)
		{
			List<(int, int, int)>[,,] ret = new List<(int, int, int)>[4, 8, 12];
			foreach ((int b, int r, int c) in positions)
			{
				ret[b, r, c] = new List<(int, int, int)>();
				foreach (var rule in rules)
				{
					foreach (var x in rule[b, r, c])
					{
						if (!ret[b,r,c].Contains(x))
							ret[b, r, c].Add(x);
					}
				}
			}
			return ret;
		}

		static List<(int, int, int)>[,,] Combine(List<List<(int, int, int)>[,,]> rules, bool[] boards)
		{
			List<(int, int, int)>[,,] ret = new List<(int, int, int)>[4, 8, 12];
			foreach ((int b, int r, int c) in positions)
			{
				ret[b, r, c] = new List<(int, int, int)>();
				foreach (var rule in rules)
				{
					foreach (var x in rule[b, r, c])
					{
						if (boards[b] && !ret[b, r, c].Contains(x))
							ret[b, r, c].Add(x);
					}
				}
			}
			return ret;
		}

		static MoveDict()
		{
			positions = new (int, int, int)[288];
			for (int b = 3; b > 0; b--)
			{
				for (int r = 0; r < 8; r++)
				{
					for (int c = 0; c < 12; c++)
					{
						int ind = (3 - b) * 96 + r * 12 + c;
						positions[ind] = (b, r, c);
					}
				}
			}

			MoveDictionary = new Dictionary<string, List<(int, int, int)>[,,]>();

			// Define the types of MoveDictionary
			MoveDictionary["forward"] = Generate(1, 0);
			MoveDictionary["backward"] = Generate(-1, 0);
			MoveDictionary["left"] = Generate(0, -1);
			MoveDictionary["right"] = Generate(0, 1);
			MoveDictionary["diagUR"] = Generate(1, 1);
			MoveDictionary["diagUL"] = Generate(1, -1);
			MoveDictionary["diagDR"] = Generate(-1, 1);
			MoveDictionary["diagDL"] = Generate(-1, -1);
			MoveDictionary["top"] = GenFromList(new List<(int, int, int)> { (3, 0, 0) });
			MoveDictionary["mid"] = GenFromList(new List<(int, int, int)> { (2, 0, 0) });
			MoveDictionary["bot"] = GenFromList(new List<(int, int, int)> { (1, 0, 0) });

			MoveDictionary["B3King"] = GenFromList(new List<(int, int, int)>{
				(3, 1, 0), (3, 1, 1), (3, 0, 1), (3, -1, 1),
				(3, -1, 0), (3, -1, -1), (3, 0, -1), (3, 1, -1) });
			MoveDictionary["B2King"] = GenFromList(new List<(int, int, int)>{
				(2, 1, 0), (2, 1, 1), (2, 0, 1), (2, -1, 1),
				(2, -1, 0), (2, -1, -1), (2, 0, -1), (2, 1, -1) });
			MoveDictionary["B1King"] = GenFromList(new List<(int, int, int)>{
				(1, 1, 0), (1, 1, 1), (1, 0, 1), (1, -1, 1),
				(1, -1, 0), (1, -1, -1), (1, 0, -1), (1, 1, -1) });

			// {1} Sylph MoveDictionary
			MoveDictionary["TopWSylphTake"] = GenFromList(new List<(int, int, int)> { (3, 1, 0), (2, 0, 0) }, bMask3);
			MoveDictionary["TopBSylphTake"] = GenFromList(new List<(int, int, int)> { (3, -1, 0), (2, 0, 0) }, bMask3);
			MoveDictionary["TopWSylph"] = GenFromList(new List<(int, int, int)> { (3, 1, 1), (3, 1, -1)}, bMask3);
			MoveDictionary["TopBSylph"] = GenFromList(new List<(int, int, int)> { (3, -1, 1), (3, -1, -1)}, bMask3);
			MoveDictionary["MidWSylph"] = GenFromList(new List<(int, int, int)> {
				(3, 0, 0), (3, 1, 0), (3, 1, 2), (3, 1, 4),
				(3, 1, 6), (3, 1, 8), (3, 1, 10)}, bMask2);
			MoveDictionary["MidBSylph"] = GenFromList(new List<(int, int, int)>{
				(3, 0, 0), (3, 6, 0), (3, 6, 2), (3, 6, 4),
				(3, 6, 6), (3, 6, 8), (3, 6, 10) }, bMask2);

			// {2 Griffon MoveDictionary
			MoveDictionary["TopGrif"] = GenFromList(new List<(int, int, int)>{
				(3, 3, 2), (3, 3, -2), (3, -3, 2), (3, -3, -2),
				(3, 2, 3), (3, -2, 3), (3, 2, -3), (3, -2, -3),
				(2, 1, 1), (2, 1, -1), (2, -1, 1), (2, -1, -1)}, bMask3);
			MoveDictionary["MidGrif"] = GenFromList(new List<(int, int, int)>{
				(2, 1, 1), (2, 1, -1), (2, -1, 1), (2, -1, -1),
				(3, 1, 1), (3, 1, -1), (3, -1, 1), (3, -1, -1)}, bMask2);

			// {3} Dragon MoveDictionary
			MoveDictionary["Dragon"] = Combine(new List<List<(int, int, int)>[,,]> {
				MoveDictionary["B3King"], MoveDictionary["diagUR"], MoveDictionary["diagUL"],
				MoveDictionary["diagDR"], MoveDictionary["diagDL"] }, bMask3);

			MoveDictionary["SwoopDragon"] = GenFromList(new List<(int, int, int)>{
				(2, 0, 0), (2, 1, 0), (2, 0, 1), (2, -1, 0), (2, 0, -1)}, bMask3);

			// {4} Oliphant MoveDictionary
			MoveDictionary["Oliphant"] = Combine(new List<List<(int, int, int)>[,,]>{
				MoveDictionary["forward"], MoveDictionary["backward"], MoveDictionary["left"], MoveDictionary["right"]}, bMask2);

			// {5} Unicorn MoveDictionary
			MoveDictionary["Unicorn"] = GenFromList(new List<(int, int, int)>{
				(2, 2, 1), (2, 2, -1), (2, -2, 1), (2, -2, -1),
				(2, 1, 2), (2, 1, -2), (2, -1, 2), (2, -1, -2)});

			// {6} Hero MoveDictionary
			MoveDictionary["TopHero"] = GenFromList(new List<(int, int, int)> {
				(3, 1, 1), (3, 1, -1), (3, -1, 1), (3, -1, -1),
				(2, 1, 1), (2, 1, -1), (2, -1, 1), (2, -1, -1)}, bMask3);
			MoveDictionary["MidHero"] = GenFromList(new List<(int, int, int)>{
				(2, 1, 1), (2, 1, -1), (2, -1, 1), (2, -1, -1),
				(2, 2, 2), (2, 2, -2), (2, -2, 2), (2, -2, -2),
				(3, 1, 1), (3, 1, -1), (3, -1, 1), (3, -1, -1),
				(1, 1, 1), (1, 1, -1), (1, -1, 1), (1, -1, -1)}, bMask2);
			MoveDictionary["BotHero"] = GenFromList(new List<(int, int, int)> {
				(1, 1, 1), (1, 1, -1), (1, -1, 1), (1, -1, -1),
				(2, 1, 1), (2, 1, -1), (2, -1, 1), (2, -1, -1)}, bMask1);

			// {7} Thief MoveDictionary
			MoveDictionary["Thief"] = Combine(new List<List<(int, int, int)>[,,]> {
			MoveDictionary["diagUR"], MoveDictionary["diagUL"],
			MoveDictionary["diagDR"], MoveDictionary["diagDL"] }, bMask2);

			// {8} Cleric MoveDictionary
			MoveDictionary["TopCleric"] = GenFromList(new List<(int, int, int)>{
				(3, 1, 0), (3, 1, 1), (3, 0, 1), (3, -1, 1),
				(3, -1, 0), (3, -1, -1), (3, 0, -1), (3, 1, -1),
				(2, 0, 0)}, bMask3);
			MoveDictionary["MidCleric"] = GenFromList(new List<(int, int, int)>{
				(2, 1, 0), (2, 1, 1), (2, 0, 1), (2, -1, 1),
				(2, -1, 0), (2, -1, -1), (2, 0, -1), (2, 1, -1),
				(3, 0, 0), (1, 0, 0)}, bMask2);
			MoveDictionary["BotCleric"] = GenFromList(new List<(int, int, int)>{
				(1, 1, 0), (1, 1, 1), (1, 0, 1), (1, -1, 1),
				(1, -1, 0), (1, -1, -1), (1, 0, -1), (1, 1, -1),
				(2, 0, 0)}, bMask1);

			// {9} Mage MoveDictionary
			MoveDictionary["TopMage"] = GenFromList(new List<(int, int, int)>{
				(3, 0, 0), (3, 1, 0), (3, 0, 1), (3, -1, 0), (3, 0, -1),
				(3, 0, 0)}, bMask3);
			MoveDictionary["MidMage"] = Combine(new List<List<(int, int, int)>[,,]> {
			MoveDictionary["forward"], MoveDictionary["backward"], MoveDictionary["left"], MoveDictionary["right"],
			MoveDictionary["diagUL"], MoveDictionary["diagUR"], MoveDictionary["diagDL"], MoveDictionary["diagDR"],
			MoveDictionary["top"], MoveDictionary["bot"]}, bMask2);
			MoveDictionary["BotMage"] = GenFromList(new List<(int, int, int)>{
				(1, 0, 0), (1, 1, 0), (1, 0, 1), (1, -1, 0), (1, 0, -1),
				(2, 0, 0)}, bMask1);

			// {10} King MoveDictionary
			MoveDictionary["TopKing"] = GenFromList(new List<(int, int, int)> { (2, 0, 0) }, bMask3);
			MoveDictionary["MidKing"] = MoveDictionary["MidCleric"];
			MoveDictionary["BotKing"] = GenFromList(new List<(int, int, int)> { (2, 0, 0) }, bMask1);

			// {11} Paladin MoveDictionary
			MoveDictionary["TopPaladin"] = GenFromList(new List<(int, int, int)> {
				// King MoveDictionary
				(3, 1, 0), (3, 1, 1), (3, 0, 1), (3, -1, 1),
				(3, -1, 0), (3, -1, -1), (3, 0, -1), (3, 1, -1),
				// 3D knight MoveDictionary
				(2, 2, 0), (2, 0, 2), (2, -2, 0), (2, 0, -2),
				(1, 1, 0), (1, 0, 1), (1, -1, 0), (1, 0, -1)}, bMask3);
			MoveDictionary["MidPaladin"] = GenFromList(new List<(int, int, int)> {
				// King MoveDictionary
				(2, 1, 0), (2, 1, 1), (2, 0, 1), (2, -1, 1),
				(2, -1, 0), (2, -1, -1), (2, 0, -1), (2, 1, -1),
				// Knight MoveDictionary
				(2, 2, 1), (2, 2, -1), (2, -2, 1), (2, -2, -1),
				(2, 1, 2), (2, 1, -2), (2, -1, 2), (2, -1, -2),
				// 3D knight MoveDictionary
				(3, 2, 0), (3, 0, 2), (3, -2, 0), (3, 0, -2),
				(1, 2, 0), (1, 0, 2), (1, -2, 0), (1, 0, -2)}, bMask2);
			MoveDictionary["BotPaladin"] = GenFromList(new List<(int, int, int)> {
				// King MoveDictionary
				(1, 1, 0), (1, 1, 1), (1, 0, 1), (1, -1, 1),
				(1, -1, 0), (1, -1, -1), (1, 0, -1), (1, 1, -1),
				// 3D knight MoveDictionary
				(2, 2, 0), (2, 0, 2), (2, -2, 0), (2, 0, -2),
				(3, 1, 0), (3, 0, 1), (3, -1, 0), (3, 0, -1) }, bMask1);

			MoveDictionary["Paladin"] = Combine(new List<List<(int, int, int)>[,,]> {
			MoveDictionary["TopPaladin"], MoveDictionary["MidPaladin"], MoveDictionary["BotPaladin"] });

			// {12} Warrior MoveDictionary
			MoveDictionary["WWarrior"] = GenFromList(new List<(int, int, int)> { (2, 1, 0) }, bMask2);
			MoveDictionary["BWarrior"] = GenFromList(new List<(int, int, int)> { (2, -1, 0) }, bMask2);
			MoveDictionary["WWarriorTake"] = GenFromList(new List<(int, int, int)> { (2, 1, 1), (2, 1, -1) }, bMask2);
			MoveDictionary["BWarriorTake"] = GenFromList(new List<(int, int, int)> { (2, -1, 1), (2, -1, -1) }, bMask2);

			// {13} Basilisk MoveDictionary
			MoveDictionary["WBasilisk"] = GenFromList(new List<(int, int, int)> { (1, 1, 0), (1, 1, 1), (1, 1, -1) }, bMask1);
			MoveDictionary["BBasilisk"] = GenFromList(new List<(int, int, int)> { (1, -1, 0), (1, -1, 1), (1, -1, -1) }, bMask1);
			MoveDictionary["WBackwardsBasilisk"] = GenFromList(new List<(int, int, int)> { (1, -1, 0) }, bMask1);
			MoveDictionary["BBackwardsBasilisk"] = GenFromList(new List<(int, int, int)> { (1, 1, 0) }, bMask1);

			// {14} Elemental MoveDictionary
			MoveDictionary["BotElemental"] = GenFromList(new List<(int, int, int)>{
				(1, 1, 0), (1, 0, 1), (1, -1, 0), (1, 0, -1),
				(1, 2, 0), (1, 0, 2), (1, -2, 0), (1, 0, -2),
				(2, 1, 0), (2, 0, 1), (2, -1, 0), (2, 0, -1)}, bMask1);
			MoveDictionary["BotElementalSlide"] = GenFromList(new List<(int, int, int)> {
				(1, 1, 1), (1, 1, -1), (1, -1, 1), (1, -1, -1) }, bMask1);
			MoveDictionary["MidElemental"] = GenFromList(new List<(int, int, int)> {
				(1, 1, 0), (1, 0, 1), (1, -1, 0), (1, 0, -1)}, bMask2);

			// {15} Dwarf MoveDictionary
			MoveDictionary["BotWDwarf"] = GenFromList(new List<(int, int, int)> {
				(1, 1, 0), (1, 0, 1), (1, 0, -1)}, bMask1);
			MoveDictionary["BotWDwarfTake"] = GenFromList(new List<(int, int, int)> {
				(1, 1, 1), (1, 1, -1), (2, 0, 0)}, bMask1);

			MoveDictionary["BotBDwarf"] = GenFromList(new List<(int, int, int)> {
				(1, -1, 0), (1, 0, 1), (1, 0, -1)}, bMask1);
			MoveDictionary["BotBDwarfTake"] = GenFromList(new List<(int, int, int)> {
				(1, -1, 1), (1, -1, -1), (2, 0, 0)}, bMask1);

			MoveDictionary["MidWDwarf"] = GenFromList(new List<(int, int, int)> {
				(2, 1, 0), (2, 0, 1), (2, 0, -1), (1, 0, 0)}, bMask2);
			MoveDictionary["MidWDwarfTake"] = GenFromList(new List<(int, int, int)> {
				(2, 1, 1), (2, 1, -1)}, bMask2);

			MoveDictionary["MidBDwarf"] = GenFromList(new List<(int, int, int)> {
				(2, -1, 0), (2, 0, 1), (2, 0, -1), (1, 0, 0)}, bMask2);
			MoveDictionary["MidBDwarfTake"] = GenFromList(new List<(int, int, int)> {
				(2, -1, 1), (2, -1, -1)}, bMask2);
		}

		public static void PrintMoveDictionaryet((int, int, int) pos, string moveType)
		{
			List<(int, int, int)> m = MoveDictionary[moveType][pos.Item1, pos.Item2, pos.Item3];

			string ret = "";
			for (int b = 3; b > 0; b--)
			{
				for (int r = 7; r >= 0; r--)
				{
					string row = "" + (r + 1) + "\t";
					for (int c = 0; c < 12; c++)
					{
						if (pos.Item1 == b && pos.Item2 == r && pos.Item3 == c)
							row += "O";
						else if (m.Contains((b, r, c)))
							row += "x";
						else
							row += "-";
						row += " ";
					}
					ret += row + "\n";
				}
				ret += "\n\n";
			}
			print(ret);
		}
	}
}
