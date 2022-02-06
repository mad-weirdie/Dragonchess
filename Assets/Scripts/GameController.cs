using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Dragonchess
{
	using static Game;
	using static GameUI;
	using static Piece;
	using static BitPiece;
	using static MoveController;
	using static Gamestate;
	using static BitMoveController;
	public class GameController : MonoBehaviour
	{
		public delegate void OnClickDelegate(Game state);
		public static OnClickDelegate clickDelegate;

		public static PlayerType P1_type;
		public static AIDifficulty AI_1;
		public static PlayerType P2_type;
		public static AIDifficulty AI_2;

		public TextAsset board_init;
		public MoveController MC;
		public AI AIController;

		static public List<(Move, bool)> moveLog;
		public static bool GameFromFileEnabled = true;
		public static bool TestsEnabled = false;
		public GameFromFile GFF;
		public static int moveNum;

		public static int layerMask;
		public static Game state;
		public GameUI Display;

		public static bool lockTesting;

		// --------------------------------------------------------------------
		
		// Start is called before the first frame update
		void Start()
		{
			print("starting new game...");
			lockTesting = false;
			moveLog = new List<(Move, bool)>();
			moveNum = 0;
			layerMask = ~0;

			NewGame();
		}

		void OnPrintSState()
		{
			Gamestate sState = new Gamestate(state);
			Gamestate.PrintState(sState, 1);
		}

		// Initialize new game
		public void NewGame()
		{
			state = new Game(GameFromFileEnabled, P1_type, AI_1, P2_type, AI_2);

			// Read in starting board state from the board_init text file
			string[] lines = File.ReadAllLines("Assets/Files/board_init.txt");
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				string[] vals = line.Split(' ');
				int level = int.Parse(vals[0]);
				int piece_type = int.Parse(vals[1]);
				int color = int.Parse(vals[2]);
				int row = int.Parse(vals[3]);
				int col = int.Parse(vals[4]);

				Player player;
				if (color == 0)
					player = state.P1;
				else
					player = state.P2;

				// Add the new piece, linking it to a piece GameObject as well
				NewPiece(state, piece_type, player, level, row, col);
			}
			Display.GameUIInit(state);
		}

		// Completely reloads the scene
		public static void ResetGame()
		{
			OnApplicationQuit();
			SceneManager.LoadScene("Game", LoadSceneMode.Single);
		}

		public void ResetBoard()
		{
			Display.ClearUI();
			NewGame();
		}

		void OnClick()
		{
			if (!state.gameOver)
				state.ActivePlayer.OnClick(state);
		}

		// User hits right arrow key
		void OnNext()
		{
			if (!lockTesting)
			{
				if (GameFromFileEnabled)
					GFF.GetNext(state);
				else
					state.ActivePlayer.GetMove(state);
			}
		}

		// User hits left arrow key: undo move
		void OnPrev()
		{
			if (GameFromFileEnabled)
				GFF.UndoPrev(state);
			if (moveNum > 0)
			{
				(Move prev, bool b) = moveLog[moveLog.Count-1];
				print("Undoing move: " + prev.MoveToString());
				UndoMove(state, prev, true);
				UnlogMove(prev.piece.player);
				SwitchTurn(state);
			}
		}

		// Unlock the arrow key controls (ENTER key)
		void OnUnlock()
		{
			print("UNLOCKING CONTROLS");
			lockTesting = false;
		}

		void OnLoadState()
		{
			state = new Game(GameFromFileEnabled, P1_type, AI_1, P2_type, AI_2);
			LoadState.NewStateFromFile(this);
		}

		public static void OnMoveReceived(Game state, Player player, Move move)
		{
			
			DoMove(state, move, !TestsEnabled);
			player.prevMove = move;
			Gamestate sState = new Gamestate(state);
			LogMove(player, IsCheck(sState, state.EnemyPlayer.color == Color.White));

			SwitchTurn(state);
			sState = new Gamestate(state);
			if (GetGameStatus(sState) == Status.Checkmate)
			{
				print("GAMEOVER.");
				GameUI.ShowGameOverMenu(GetGameStatus(sState), sState.WhiteToMove);
			}
			if (state.ActivePlayer.type == PlayerType.AI &&
				state.EnemyPlayer.type == PlayerType.Human)
				state.ActivePlayer.GetMove(state);
		}

		public static void MainMenu()
		{
			SceneManager.LoadScene("Menu", LoadSceneMode.Single);
		}

		public static void Quit()
		{
			Application.Quit();
		}

		public void ReturnToBoard()
		{
			Display.DisableGameOverMenu();
		}

		public static void LogMove(Player player, bool isCheck)
		{
			moveLog.Add((player.prevMove, isCheck));
			MovelistAdd(moveNum, player.prevMove.MoveToLogString(isCheck));
			moveNum++;
		}

		public static void UnlogMove(Player player)
		{
			(Move move, bool wasCheck) = moveLog[moveNum - 1];
			MovelistRemove(moveNum - 1);
			moveLog.RemoveAt(moveNum-1);
			moveNum--;
		}

		public static void SwitchTurn(Game state)
		{
			state.ActivePlayer = state.EnemyPlayer;
			state.EnemyPlayer = GetEnemy(state, state.ActivePlayer);
			GameUI.SetActiveText(state.ActivePlayer);

			Gamestate sState = new Gamestate(state);

			// See if P1's move put P2 in check
			// NOTE: "false" denotes the king being black/white
			if (IsCheck(sState, false))
			{
				ShowKingInCheck(state.P2);
				lockTesting = true;		// stops me from missing the checks
				// Now, see whether the check is also checkmate
				if (IsCheckmate(sState, false))
				{
					print("GAME OVER: CHECKMATE. PLAYER 1 WINS.");
					state.gameOver = true;
					return;
				}
			}
			else
			{
				ResetSquareColor(GetKing(state, state.P2).pos);
			}
			// See if P2's move put P1 in check
			// NOTE: "true" denotes the king being black/white
			if (IsCheck(sState, true))
			{
				ShowKingInCheck(state.P1);	// just highlights the king red
				lockTesting = true;     // stops me from missing the checks
				// Now, see whether the check is also checkmate
				if (IsCheckmate(sState, true))
				{
					print("GAME OVER: CHECKMATE. PLAYER 2 WINS.");
					state.gameOver = true;
					return;
				}
			}
			else
			{
				ResetSquareColor(GetKing(state, state.P1).pos);
			}
		}

		// Saves the game's move data to a file
		static void OnApplicationQuit()
		{
			string path = "Assets/Resources/game_out.txt";
			StreamWriter w = new StreamWriter(path);

			for (int i = 0; i < moveNum; i++)
			{
				(Move move, bool wasCheck) = moveLog[i];
				string moveText = move.MoveToLogString(wasCheck);
				
				if (i % 2 == 0)
					w.Write((1 + i / 2) + ")\t" + moveText);
				else
				{
					w.Write('\t' + moveText + '\n');
				}
			}

			w.Close();
		}
	}
}
