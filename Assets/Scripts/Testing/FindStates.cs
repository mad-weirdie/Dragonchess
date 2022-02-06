using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using M = MonoBehaviour;
	using PT = PlayerType;
	using D = AIDifficulty;

	using static Gamestate;
	using static BitMoveController;
	using static BitMove;

	public class FindStates : MonoBehaviour
	{
		// How many games we should play
		static int numIterations = 10;
		public GameController GC;
		public static Dictionary<long, int> stateEvals;

		static FindStates()
		{
			stateEvals = new Dictionary<long, int>();
		}

		// Start is called before the first frame update
		void Start()
		{
			int totalMoves = 0;
			int numWhiteWins = 0;
			int numBlackWins = 0;
			int numStalemates = 0;
			if (GameController.TestsEnabled)
			{
				for (int gameNum = 0; gameNum < numIterations; gameNum++)
				{
					Gamestate state = NewGame();
					int numMoves = 0;

					while (GetGameStatus(state) != Status.Checkmate)
					{
						// Get the next move, and do it!
						List<int> moves = BitMoveController.GetPossibleMoves(state, state.WhiteToMove);

						if (moves.Count == 0)
							break;
						
						int move = moves[Random.Range(0, moves.Count)];
						DoBitMove(ref state, move);
						UpdateGameStatus(state);
						long stateHash = Zobrist.GetStateHash(state);
						stateEvals[stateHash] = EvaluateGamestate(state);
						numMoves++;
					}
					// -----------------------------------------------------------------------------
					totalMoves += numMoves;
					if (GetGameStatus(state) == Status.Checkmate && state.WhiteToMove)
					{
						numBlackWins++;
						print("CHECKMATE: BLACK WINS.");
					}
					else if (GetGameStatus(state) == Status.Checkmate && !state.WhiteToMove)
					{
						numWhiteWins++;
						print("CHECKMATE: WHITE WINS.");
					}
					else
					{
						numStalemates++;
						print("STALEMATE.");
					}
				}
				print("Total moves: " + totalMoves + " in " + numIterations + " games.");
				print("Num states: " + stateEvals.Keys.Count);
				print("Num white wins: " + numWhiteWins);
				print("Num black wins: " + numBlackWins);
				print("Num draws: " + numStalemates);
			}
		}
	}
}
