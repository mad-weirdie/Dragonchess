using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Dragonchess
{
	using static Game;
	public class GameUI : MonoBehaviour
	{
		static string[] Letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };

		public List<GameObject> boardObj;
		public List<GameObject> piecePrefabs;
		public List<Material> materials;
		public static float square_scale = 1.0f;
		public static float board_width = 1.0f;

		public static Material highlight_1;
		public static Material highlight_2;
		public static Material highlight_3;
		public static Material invisible;
		public static Material frozenMat;
		public static Material white_pieces;
		public static Material black_pieces;

		public static Material threatsToW;
		public static Material threatsToB;

		public static List<Material> boardMaterials;

		public GameObject black_text;
		public GameObject white_text;

		public static GameObject b_text;
		public static GameObject w_text;

		static List<Move> highlightedMoves;
		static Piece selectedPiece;
		List<Board> m_boards;

		public static GameObject upperBoard;
		public static GameObject middleBoard;
		public static GameObject lowerBoard;

		static public SquareObject[,] upperSquares;
		static public SquareObject[,] middleSquares;
		static public SquareObject[,] lowerSquares;

		static public List<PieceObject> P1_Pieces;
		static public List<PieceObject> P2_Pieces;
		static public List<GameObject> pieceObjectPrefabs;

		public GameObject log_prefab;
		public static GameObject movelog;

		public GameObject log_parent_object;
		public static GameObject content;

		public GameObject GameOver;
		public GameObject whiteWinText;
		public GameObject blackWinText;
		public GameObject drawText;

		public static GameObject GO;
		public static GameObject WWT;
		public static GameObject BWT;
		public static GameObject DT;

		// Load in the materials into static-accessible variables
		void Start()
		{
			highlightedMoves = new List<Move>();
			boardMaterials = new List<Material>();
			pieceObjectPrefabs = new List<GameObject>();

			highlight_1 = materials[6];
			highlight_2 = materials[7];
			highlight_3 = materials[8];
			invisible = materials[9];
			white_pieces = materials[10];
			black_pieces = materials[11];
			frozenMat = materials[13];

			threatsToW = materials[14];
			threatsToB = materials[15];

			b_text = black_text;
			w_text = white_text;

			for (int i = 0; i < 6; i++)
				boardMaterials.Add(materials[i]);
			for (int i = 0; i < piecePrefabs.Count; i++)
				pieceObjectPrefabs.Add(piecePrefabs[i]);

			upperBoard = boardObj[0];
			middleBoard = boardObj[1];
			lowerBoard = boardObj[2];
			movelog = log_prefab;
			content = log_parent_object;

			GO = GameOver;
			WWT = whiteWinText;
			BWT = blackWinText;
			DT = drawText;
		}

		// Initializes a new GameUI to reflect the passed-in state
		public void GameUIInit(Game state)
		{
			upperSquares = new SquareObject[8, 12];
			middleSquares = new SquareObject[8, 12];
			lowerSquares = new SquareObject[8, 12];

			NewBoardUI(state.boards[1]);
			NewBoardUI(state.boards[2]);
			NewBoardUI(state.boards[3]);

			P1_Pieces = new List<PieceObject>();
			P2_Pieces = new List<PieceObject>();
			for (int i = 0; i < state.P1.pieces.Count; i++)
				AddPieceUI(state.P1.pieces[i]);
			for (int i = 0; i < state.P2.pieces.Count; i++)
				AddPieceUI(state.P2.pieces[i]);
		}

		// Creates a new board UI, instantiating square GameObject elements
		public void NewBoardUI(Board board)
		{
			int level = board.layer_int_val;
			Material upper_mat;
			Material lower_mat;

			if (level == 3)
			{
				upper_mat = boardMaterials[0];
				lower_mat = boardMaterials[1];
			}
			else if (level == 2)
			{
				upper_mat = boardMaterials[2];
				lower_mat = boardMaterials[3];
			}
			else
			{
				upper_mat = boardMaterials[4];
				lower_mat = boardMaterials[5];
			}

			for (int r = 0; r < Board.height; r++)
			{
				for (int c = 0; c < Board.width; c++)
				{
					Material mat;
					if ((r + c) % 2 == 0)
						mat = upper_mat;
					else
						mat = lower_mat;

					AddSquareAt(board.layer_int_val, mat, invisible, r, c);
				}
			}
		}

		// Completely clears out current UI
		public void ClearUI()
		{
			for (int i = 0; i < P1_Pieces.Count; i++)
				Destroy(P1_Pieces[i].gameObject);
			for (int i = 0; i < P2_Pieces.Count; i++)
				Destroy(P2_Pieces[i].gameObject);

			for (int i = 3; i > 0; i--)
			{
				SquareObject[,] temp = GetSquares(i);
				for (int r = 0; r < temp.GetLength(0); r++)
				{
					for (int c = 0; c < temp.GetLength(1); c++)
						temp[r, c].piece = null;
				}
			}

			for (int i = GameController.moveNum-1; i >= 0; i--)
			{
				MovelistRemove(i);
			}
			GameController.moveNum = 0;
			GameController.moveLog.Clear();
		}

		//====================================================================================

		public static void AddSquareAt(int board, Material mat, Material inv, int r, int c)
		{
			SquareObject[,] squares = GetSquares(board);
			GameObject boardObj = GetBoardObject(board);

			// Maintains nice names for the squares, ie "A3", "E9", etc.
			string name = Letters[c] + (r + 1).ToString();
			GameObject cube = boardObj.transform.Find(name).gameObject;
			cube.GetComponent<Renderer>().material = mat;

			SquareObject squareObject = cube.AddComponent<SquareObject>();
			squareObject.properMaterial = mat;
			squareObject.board = board;
			squareObject.row = r;
			squareObject.col = c;

			squares[r, c] = squareObject;

			// This is used to show available moves when clicking on a square
			AddDotTo(squareObject);
		}

		public static void AddPieceUI(Piece piece)
		{
			int board = piece.pos.board;
			int r = piece.pos.row;
			int c = piece.pos.col;
			// New piece GameObject prefab to instantiate
			GameObject pieceGameObject = pieceObjectPrefabs[(int)piece.type];
			GameObject boardObj = GetBoardObject(board);
			SquareObject[,] squares = GetSquares(board);
			SquareObject squareObject = squares[r, c];

			// get location to place piece GameObject at (center of square object)
			Vector3 pos = squareObject.transform.position;

			// Set correct layer for camera rendering
			int renderLayer = 3 + squareObject.gameObject.layer;
			pieceGameObject.layer = renderLayer;
			for (int i = 0; i < pieceGameObject.transform.childCount; i++)
			{
				Transform child = pieceGameObject.transform.GetChild(i);
				child.gameObject.layer = pieceGameObject.layer = renderLayer;
			}

			// Instantiate the gameobject with correct scale and pos translation
			Vector3 v = new Vector3(0.7f * square_scale, 0.7f * square_scale, 0.7f * square_scale);
			Quaternion q = new Quaternion(0, 0, 0, 1);
			pos.y += 1.0f / square_scale;

			GameObject obj = GameObject.Instantiate(pieceGameObject, pos, q);
			PieceObject pieceObj = obj.GetComponent<PieceObject>();
			obj.transform.localScale = v;
			squareObject.piece = pieceObj;
			pieceObj.piece = piece;
			pieceObj.pos = squareObject;

			if (piece.color == Color.White)
			{
				P1_Pieces.Add(pieceObj);
				obj.GetComponent<Renderer>().material = white_pieces;
			}
			else
			{
				P2_Pieces.Add(pieceObj);
				obj.GetComponent<Renderer>().material = black_pieces;
			}

			// Set the basilisk's freezy-square danger-zone!
			if (pieceObj.type == PieceType.Basilisk)
			{

			}
		}

		public static void AddDotTo(SquareObject square)
		{
			GameObject newDot = Instantiate(Resources.Load("dot", typeof(GameObject))) as GameObject;
			Transform t = newDot.transform;
			newDot.transform.parent = square.transform;
			newDot.transform.localPosition = t.position;
			newDot.layer = square.gameObject.layer;
			square.dot = newDot;
		}

		//====================================================================================

		public static GameObject GetBoardObject(int board)
		{
			if (board == 3)
				return upperBoard;
			else if (board == 2)
				return middleBoard;
			else
				return lowerBoard;
		}

		public static SquareObject[,] GetSquares(int board)
		{
			if (board == 3)
				return upperSquares;
			else if (board == 2)
				return middleSquares;
			else
				return lowerSquares;
		}

		public static SquareObject GetSquareAt(int board, int r, int c)
		{
			SquareObject[,] squares = GetSquares(board);
			return squares[r, c];
		}

		//====================================================================================

		public static void OnMoveUpdateUI(Game state, Move move)
		{
			// Get the associated squares and pieces involved in the move
			Square start = move.start;
			Square end = move.end;
			Piece piece = move.piece;
			ResetSquareColor(start);

			// Get references to the associated GameObjects
			SquareObject startSquare = GetSquareAt(start.board, start.row, start.col);
			SquareObject endSquare = GetSquareAt(end.board, end.row, end.col);
			PieceObject pieceObject = startSquare.piece;

			// Disable the captured piece's GameObject
			if (move.type == MoveType.Capture || move.type == MoveType.Swoop)
			{
				PieceObject cap = endSquare.piece;
				// If we're capturing a basilisk, check to see if we need to unfreeze someone
				if (cap.type == PieceType.Basilisk)
				{
					if (Basilisk.HasFrozen(state, cap.piece, end))
					{
						// Get GameObject references for the associated frozen piece
						SquareObject s = GetSquareAt(2, end.row, end.col);
						Piece frozen = s.piece.piece;
						ResetPieceColor(frozen);
					}
				}	

				Destroy(cap.gameObject);
				if (move.piece.player.color == Color.White)
					P2_Pieces.Remove(cap);
				else
					P1_Pieces.Remove(cap);
			}

			// Don't move piece for a dragon swoop
			if (move.type != MoveType.Swoop)
			{
				// Move the piece on the board display
				Vector3 pos = endSquare.transform.position;
				pos.y += 1.0f / GameUI.square_scale;
				pieceObject.transform.position = pos;
				endSquare.dot.GetComponent<Renderer>().material = invisible;

				// Set proper rendering layer (for the overhead cameras)
				pieceObject.gameObject.layer = endSquare.gameObject.layer + 3;
				for (int i = 0; i < pieceObject.transform.childCount; i++)
				{
					Transform child = pieceObject.transform.GetChild(i);
					child.gameObject.layer = endSquare.gameObject.layer + 3;
				}

				// Reassign references for the associated PieceObject
				endSquare.piece = pieceObject;
				pieceObject.pos = endSquare;
			}

			// Check for basilisk side effects...
			if (piece.type == PieceType.Basilisk)
			{
				// 1. check to see if we need to unfreeze someone
				if (Basilisk.HasFrozen(state, piece, start))
				{
					// Get GameObject references for the associated frozen piece
					SquareObject s = GetSquareAt(2, start.row, start.col);
					Piece frozen = s.piece.piece;
					ResetPieceColor(frozen);
				}
				// 2. check to see if we need to freeze someone new
				if (Basilisk.HasFrozen(state, piece, end))
				{
					// Get GameObject references for the associated frozen piece
					SquareObject s = GetSquareAt(2, end.row, end.col);
					PieceObject frozen = s.piece;
					frozen.GetComponent<Renderer>().material = frozenMat;
				}
			}
			else
			{
				// 3. check to see if we're moving on top of the enemy basilisk
				if (state.boards[1].squares[end.row, end.col].occupied)
				{
					Piece p = state.boards[1].squares[end.row, end.col].piece;
					if (p.type == PieceType.Basilisk && Basilisk.HasFrozen(state, p, end))
					{
						// Get GameObject references for the associated frozen piece
						SquareObject s = GetSquareAt(2, end.row, end.col);
						PieceObject frozen = s.piece;
						frozen.GetComponent<Renderer>().material = frozenMat;
					}
				}
			}
		}

		public static void HighlightMoves(Piece piece, List<Move> moves)
		{
			SquareObject square = GetSquareAt(piece.pos.board, piece.pos.row, piece.pos.col);
			square.dot.GetComponent<Renderer>().material = highlight_1;
			selectedPiece = piece;

			// Highlight all the possible moves generated for a piece
			for (int i = 0; i < moves.Count; i++)
			{
				Move move = moves[i];
				int board = move.end.board;
				int r = move.end.row;
				int c = move.end.col;

				GameObject dot = GetSquares(board)[r, c].dot;
				highlightedMoves.Add(move);

				if (move.type == MoveType.Regular)
					dot.GetComponent<Renderer>().material = highlight_2;
				else if (move.type == MoveType.Capture)
					dot.GetComponent<Renderer>().material = highlight_3;
				else
					dot.GetComponent<Renderer>().material = highlight_3;
			}
		}

		public static void HighlightThreats(List<Piece> pieces, bool isWhite)
		{
			Material threatmat;
			if (isWhite) threatmat = threatsToW; else threatmat = threatsToB;
			for (int i = 0; i < pieces.Count; i++)
			{
				Piece piece = pieces[i];
				SquareObject square = GetSquareAt(piece.pos.board, piece.pos.row, piece.pos.col);
				square.dot.GetComponent<Renderer>().material = threatmat;
			}
		}

		public static void ShowKingInCheck(Player player)
		{
			Game state = GameController.state;
			Piece king = GetKing(state, player);

			SquareObject square = GetSquareAt(king.pos.board, king.pos.row, king.pos.col);
			square.dot.GetComponent<Renderer>().material = highlight_3;
		}

		//------------------------------------------------------------------------------------

		public static void UnhighlightMoves()
		{
			for (int i = 0; i < highlightedMoves.Count; i++)
			{
				Move move = highlightedMoves[i];
				int board = move.end.board;
				int r = move.end.row;
				int c = move.end.col;

				GameObject dot = GetSquares(board)[r, c].dot;
				dot.GetComponent<Renderer>().material = invisible;
			}
			highlightedMoves.Clear();
		}

		public static void DeselectAll()
		{
			UnhighlightMoves();
			if (selectedPiece != null)
			{
				ResetSquareColor(selectedPiece.pos);
				selectedPiece = null;
			}
			EventSystem.current.SetSelectedGameObject(null);
		}

		public static void ResetSquareColor(Square s)
		{
			SquareObject square = GetSquares(s.board)[s.row, s.col];
			square.dot.GetComponent<Renderer>().material = invisible;
		}

		public static void ResetPieceColor(Piece p)
		{
			SquareObject pos = GetSquareAt(p.pos.board, p.pos.row, p.pos.col);
			PieceObject piece = pos.piece;
			if (piece.piece.color == Color.White)
				piece.GetComponent<Renderer>().material = white_pieces;
			else
				piece.GetComponent<Renderer>().material = black_pieces;
		}

		//====================================================================================

		public static void MovelistAdd(int moveNum, string movetext)
		{
			if (moveNum % 2 == 0)
			{
				GameObject newLog = Instantiate(movelog, Vector3.zero, Quaternion.identity);
				newLog.name = string.Format("{0}", moveNum/2 + 1);
				newLog.transform.SetParent(content.transform, false);

				GameObject gtext = newLog.transform.Find("move_num").gameObject;
				TextMeshProUGUI movenum_text =  gtext.GetComponent<TextMeshProUGUI>();
				movenum_text.text = string.Format("{0}.", moveNum/2 + 1);

				GameObject white_move = newLog.transform.Find("white_move").gameObject;
				TextMeshProUGUI white_text = white_move.transform.Find("text").GetComponent<TextMeshProUGUI>();
				white_text.text = movetext;

				GameObject black_move = newLog.transform.Find("black_move").gameObject;
				TextMeshProUGUI black_text = black_move.transform.Find("text").GetComponent<TextMeshProUGUI>();
				black_text.text = "";
				black_move.SetActive(false);
			}
			else
			{
				string logname = string.Format("{0}", moveNum/2 + 1);
				GameObject newLog = content.transform.Find(logname).gameObject;
				GameObject black_move = newLog.transform.Find("black_move").gameObject;
				black_move.SetActive(true);
				TextMeshProUGUI black_text = black_move.transform.Find("text").GetComponent<TextMeshProUGUI>();
				black_text.text = movetext;
			}
		}

		public static void MovelistRemove(int moveNum)
		{
			if (moveNum % 2 == 0)
			{
				string logname = string.Format("{0}", moveNum / 2 + 1);
				GameObject oldLog = content.transform.Find(logname).gameObject;
				Destroy(oldLog);
			}
			else
			{
				string logname = string.Format("{0}", moveNum / 2 + 1);
				GameObject log = content.transform.Find(logname).gameObject;
				GameObject black_move = log.transform.Find("black_move").gameObject;
				black_move.SetActive(false);
			}
		}

		public static void SetActiveText(Player player)
		{
			// Switch the currently active player text
			if (player.color == Color.White)
			{
				w_text.SetActive(true);
				b_text.SetActive(false);
			}
			else
			{
				b_text.SetActive(true);
				w_text.SetActive(false);
			}
		}

		//====================================================================================

		public static bool IsAvailableMove(GameObject clickedObj, ref Move move)
		{
			SquareObject s = clickedObj.GetComponent<SquareObject>();
			PieceObject p = clickedObj.GetComponent<PieceObject>();
			if (p != null)
				s = p.pos;

			if (s == null)
				return false;
			for (int i = 0; i < highlightedMoves.Count; i++)
			{
				Move m = highlightedMoves[i];
				Square end = m.end;
				if (end.row == s.row && end.col == s.col && end.board == s.board)
				{
					move = m;
					return true;
				}
			}
			return false;
		}

		public static bool PieceSelected()
		{
			return (selectedPiece != null);
		}

		public static bool IsPiece(GameObject g)
		{
			return(g.GetComponent<PieceObject>());
		}

		public static bool IsPlayerPiece(Player player, GameObject clickedObj, ref Piece piece)
		{
			if (!IsPiece(clickedObj))
				return false;
			
			PieceObject pieceObject = clickedObj.GetComponent<PieceObject>();
			for (int i = 0; i < player.pieces.Count; i++)
			{
				Piece p = player.pieces[i];
				if (pieceObject.piece == p)
				{
					piece = p;
					return true;
				}
			}
			return false;
		}

		public List<Board> boards
		{
			get
			{
				return m_boards;
			}
		}

		public static void ShowGameOverMenu(Status endStatus, bool whiteToMove)
		{
			GO.SetActive(true);
			if (endStatus == Status.Checkmate && whiteToMove)
				BWT.SetActive(true);
			else if (endStatus == Status.Checkmate && !whiteToMove)
				WWT.SetActive(true);
			else
				DT.SetActive(true);
		}

		public void DisableGameOverMenu()
		{
			GameOver.SetActive(false);
		}
	}
}
