using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Dragonchess {
	using static Piece;
	public class LoadState : MonoBehaviour
	{
		public static Dictionary<char, int> pieceCodeDict;
		public static char[] pieceCodes = { 's', 'g', 'r', 'w', 'o', 'u', 'h', 't', 'c', 'm', 'k', 'p', 'd', 'b', 'e' };

		static LoadState()
		{
			pieceCodeDict = new Dictionary<char, int>();
			for (int i = 0; i < pieceCodes.Length; i++)
			{
				pieceCodeDict[pieceCodes[i]] = i;
				pieceCodeDict[Char.ToUpper(pieceCodes[i])] = i;
			}
		}

		public static void NewStateFromFile(GameController GC, string filename = "Assets/Out/Gamestate1.txt")
		{
			// Clear the old game UI and data
			Game state = GameController.state;
			GC.Display.ClearUI();

			string[] lines = File.ReadAllLines(filename);
			if (lines.Length != 28)
			{
				print("error: incorrect number of lines: " + lines.Length);
				return;
			}

			// This iterates through the 3 boards in the file
			for (int ind = 0; ind <= 18; ind += 9)
			{
				// First by row...
				for (int row = 0; row < 8; row++)
				{
					string[] chars = lines[row+ind].Trim().Split(' ');
					// Then by column...
					for (int col = 0; col < chars.Length; col++)
					{
						char c = chars[col][0];
						if (c != '.')
						{
							int level = 3 - (ind / 9);
							int piece_type = pieceCodeDict[c];
							// Add the new piece, linking it to a piece GameObject as well
							if (!Char.IsUpper(c))
								NewPiece(state, piece_type, state.P1, level, 7-row, col);
							else
								NewPiece(state, piece_type, state.P2, level, 7-row, col);
						}
					}
				}
			}
			GC.Display.GameUIInit(state);
			GameController.SwitchTurn(state);

			string toMove = lines[27].Trim();
			print("Setting " + toMove + " to move.");
			if (String.Equals(toMove, "white"))
			{
				state.ActivePlayer = state.P1;
				state.EnemyPlayer = state.P2;
			}
			else if (String.Equals(toMove, "black"))
			{
				state.ActivePlayer = state.P2;
				state.EnemyPlayer = state.P1;
			}
			else
			{
				print("ERROR: UNRECOGNIZED ARGUMENT: " + toMove);
			}
			GameUI.SetActiveText(state.ActivePlayer);
		}
	}
}
