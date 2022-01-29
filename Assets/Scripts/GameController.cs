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
	using static Gamestate;
	using static GameUI;
	using static Piece;
	using static MoveController;
	using static SimpleState;
	public class GameController : MonoBehaviour
	{
		public delegate void OnClickDelegate(Gamestate state);
		public static OnClickDelegate clickDelegate;

		public PlayerType P1_type;
		public PlayerType P2_type;

		public TextAsset board_init;
		public MoveController MC;
		public AI AIController;

		static public List<(Move, bool)> moveLog;
		public bool GameFromFileEnabled;
		public GameFromFile GFF;
		static int moveNum;

		public static int layerMask;
		public static Gamestate state;
		public GameUI Display;

		// --------------------------------------------------------------------
		// Start is called before the first frame update
		void Start()
		{
			moveLog = new List<(Move, bool)>();
			moveNum = 0;
			layerMask = ~0;

			NewGame();
		}

		void OnPrintSState()
		{
			SimpleState sState = new SimpleState(state);
			SimpleState.PrintState(sState, 22);
		}

		// Initialize new game
		void NewGame()
		{
			state = new Gamestate(GameFromFileEnabled, P1_type, P2_type);

			// Read in starting board state from the board_init text file
			string[] lines = File.ReadAllLines("Assets/Files/board_init.txt");

			foreach (string line in lines)
			{
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
		void ResetGame()
		{
			OnApplicationQuit();
			SceneManager.LoadScene("Game", LoadSceneMode.Single);
		}

		void OnClick()
		{
			state.ActivePlayer.OnClick(state);

			if (state.gameOver)
				ResetGame();
		}

		// User hits right arrow key
		void OnNext()
		{
			if (GameFromFileEnabled)
				GFF.DoNext(state);
			else
				state.ActivePlayer.GetMove(state);
			//SimpleState s = new SimpleState(state);
		}

		// User hits left arrow key: undo move
		void OnPrev()
		{
			if (GameFromFileEnabled)
				GFF.UndoPrev(state);
			else if (moveNum > 0)
			{
				(Move prev, bool b) = moveLog[moveLog.Count-1];
				print("Undoing move: " + prev.MoveToString());
				UndoMove(state, prev, true);
				UnlogMove(prev.piece.player);
				SwitchTurn(state);
			}
		}

		public static void OnMoveReceived(Gamestate state, Player player, Move move)
		{
			if (state.ActivePlayer != player)
			{
				print("ERROR: INCORRECT PLAYER TRYING TO MAKE A MOVE.");
				return;
			}
			else
			{
				DoMove(state, move, true);
				player.prevMove = move;
				List<Piece> threats = ThreatsInRange(state, state.EnemyPlayer);
				LogMove(player, IsCheck(state, state.EnemyPlayer, threats));

				SwitchTurn(state);
				if (state.ActivePlayer.type == PlayerType.AI &&
					state.EnemyPlayer.type == PlayerType.Human)
					state.ActivePlayer.GetMove(state);
			}
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

		public static void SwitchTurn(Gamestate state)
		{
			state.ActivePlayer = state.EnemyPlayer;
			state.EnemyPlayer = GetEnemy(state, state.ActivePlayer);
			GameUI.SetActiveText(state.ActivePlayer);

			SimpleState sState = new SimpleState(state);
			Piece k;
			int[] king = new int[3];
			k = GetKing(state, state.P2);
			king[0] = k.pos.board;
			king[1] = k.pos.row;
			king[2] = k.pos.col;

			// See if P1's move put P2 in check
			if (IsSSCheck(sState, 1))
			{
				print("PLAYER 2'S KING IN CHECK.");
				state.P2.inCheck = true;
				ShowKingInCheck(state.P2);

				if (IsCheckmate(state, state.P2))
				{
					print("GAME OVER: CHECKMATE. PLAYER 1 WINS.");
					state.gameOver = true;
					return;
				}
			}
			else
			{
				state.P2.inCheck = false;
				ResetSquareColor(GetKing(state, state.P2).pos);
			}
			
			k = GetKing(state, state.P1);
			king[0] = k.pos.board;
			king[1] = k.pos.row;
			king[2] = k.pos.col;

			// See if P2's move put P1 in check
			if (IsSSCheck(sState, 0))
			{
				state.P1.inCheck = true;
				ShowKingInCheck(state.P1);

				print("PLAYER 1'S KING IN CHECK.");
				if (IsCheckmate(state, state.P1))
				{
					print("GAME OVER: CHECKMATE. PLAYER 2 WINS.");
					state.gameOver = true;
					return;
				}
			}
			else
			{
				state.P1.inCheck = false;
				ResetSquareColor(GetKing(state, state.P1).pos);
			}
		}

		// Saves the game's move data to a file
		void OnApplicationQuit()
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
