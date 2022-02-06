using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess {
	using static MoveDict;
	using static MoveTypeDict;
	using static Piece;

	public class ThreatDict : MonoBehaviour
	{
		public readonly static Dictionary<(int, int, int), List<(int, int, int, int)>> ThreatsToWhiteKing;
		public readonly static Dictionary<(int, int, int), List<(int, int, int, int)>> ThreatsToBlackKing;

		// Pieces that can attack from the upper board
		static public string[] TopThreats =
		{
			"TopWSylphTake", "TopBSylphTake",			// {0}
			"TopGrif",									// {1}
			"Dragon", "SwoopDragon",					// {2}
			"TopHero",									// {6}
			"TopCleric",								// {8}
			"TopMage",									// {9}
			"TopKing",									// {10}
			"TopPaladin"								// {11}
		};

		// Pieces that can attack from the middle board (2)
		static public string[] MidThreats =
		{
			"MidGrif",									// {1}
			"WWarriorTake", "BWarriorTake",				// {3}
			"Oliphant",									// {4}
			"Unicorn",									// {5}
			"MidHero",									// {6}
			"Thief",									// {7}
			"MidCleric",								// {8}
			"MidMage",									// {9}
			"MidKing",									// {10}
			"MidPaladin",								// {11}
			"MidWDwarfTake", "MidBDwarfTake",			// {12}
			"MidElemental"
		};

		// Pieces that can attack from the lower board
		static public string[] BotThreats =
		{
			"MidCleric",								// {8}
			"BotMage",									// {9}
			"BotKing",									// {10}
			"BotPaladin",								// {11}
			"BotWDwarfTake", "BotBDwarfTake",			// {12}
			"WBasilisk", "BBasilisk",					// {13}
			"BotElemental",								// {14}
		};

		// Pieces that can attack from the upper board
		static public string[] TopThreatsToWhite =
		{
			"TopBSylphTake",							// {0}
			"TopGrif",									// {1}
			"Dragon", "SwoopDragon",					// {2}
			"TopHero",									// {6}
			"TopCleric",								// {8}
			"TopMage",									// {9}
			"TopKing",									// {10}
			"TopPaladin"								// {11}
		};

		// Pieces that can attack from the middle board (2)
		static public string[] MidThreatsToWhite =
		{
			"MidGrif",									// {1}
			"BWarriorTake",								// {3}
			"Oliphant",									// {4}
			"Unicorn",									// {5}
			"MidHero",									// {6}
			"Thief",									// {7}
			"MidCleric",								// {8}
			"MidMage",									// {9}
			"MidKing",									// {10}
			"MidPaladin",								// {11}
			"MidBDwarfTake",							// {12}
			"MidElemental"
		};

		// Pieces that can attack from the lower board
		static public string[] BotThreatsToWhite =
		{
			"MidCleric",								// {8}
			"BotMage",									// {9}
			"BotKing",									// {10}
			"BotPaladin",								// {11}
			"BotBDwarfTake",							// {12}
			"BBasilisk",								// {13}
			"BotElemental",								// {14}
		};

		// Pieces that can attack from the upper board
		static public string[] TopThreatsToBlack =
		{
			"TopWSylphTake",							// {0}
			"TopGrif",									// {1}
			"Dragon", "SwoopDragon",					// {2}
			"TopHero",									// {6}
			"TopCleric",								// {8}
			"TopMage",									// {9}
			"TopKing",									// {10}
			"TopPaladin"								// {11}
		};

		// Pieces that can attack from the middle board (2)
		static public string[] MidThreatsToBlack =
		{
			"MidGrif",									// {1}
			"WWarriorTake",								// {3}
			"Oliphant",									// {4}
			"Unicorn",									// {5}
			"MidHero",									// {6}
			"Thief",									// {7}
			"MidCleric",								// {8}
			"MidMage",									// {9}
			"MidKing",									// {10}
			"MidPaladin",								// {11}
			"MidWDwarfTake",							// {12}
			"MidElemental"
		};

		// Pieces that can attack from the lower board
		static public string[] BotThreatsToBlack =
		{
			"MidCleric",								// {8}
			"BotMage",									// {9}
			"BotKing",									// {10}
			"BotPaladin",								// {11}
			"BotWDwarfTake",							// {12}
			"WBasilisk",								// {13}
			"BotElemental",								// {14}
		};

		static ThreatDict()
		{
			ThreatsToWhiteKing = new Dictionary<(int,int,int), List<(int, int, int, int)>>();
			ThreatsToBlackKing = new Dictionary<(int,int,int), List<(int, int, int, int)>>();

			for (int p = 0; p < positions.Length; p++)
			{
				(int b, int r, int c) = positions[p];
				ThreatsToWhiteKing[(b,r,c)] = new List<(int, int, int, int)>();
				ThreatsToBlackKing[(b,r,c)] = new List<(int, int, int, int)>();
			}

			for (int p = 0; p < positions.Length; p++)
			{
				(int b, int r, int c) = positions[p];
				string[] threatsToW;
				string[] threatsToB;
				if (b == 3)
				{
					threatsToW = TopThreatsToWhite;
					threatsToB = TopThreatsToBlack;
				}
				else if (b == 2)
				{
					threatsToW = MidThreatsToWhite;
					threatsToB = MidThreatsToBlack;
				}
				else
				{
					threatsToW = BotThreatsToWhite;
					threatsToB = BotThreatsToBlack;
				}

				for (int w = 0; w < threatsToW.Length; w++)
				{
					string threat = threatsToW[w];
					int t = MoveTypes[threat];
					List<(int, int, int)> threatList = MoveDictionary[threat][b, r, c];
					for (int j = 0; j < threatList.Count; j++)
					{
						var x = threatList[j];
						if (!ThreatsToWhiteKing[x].Contains((t, b, r, c)))
							ThreatsToWhiteKing[x].Add((t, b, r, c));
					}	
				}

				for (int i = 0; i < threatsToB.Length; i++)
				{
					string threat = threatsToW[i];
					int t = MoveTypes[threat];
					List<(int, int, int)> threatList = MoveDictionary[threat][b, r, c];
					for (int j = 0; j < threatList.Count; j++)
					{
						var x = threatList[j];
						if (!ThreatsToBlackKing[x].Contains((t, b, r, c)))
							ThreatsToBlackKing[x].Add((t, b, r, c));
					}
				}
			}

			for (int p = 0; p < positions.Length; p++)
			{
				var pos = positions[p];
				ThreatsToWhiteKing[pos].Sort();
				ThreatsToBlackKing[pos].Sort();
			}
		}
	}
}
