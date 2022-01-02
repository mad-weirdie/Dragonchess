using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Dragonchess
{
    public class MoveController : MonoBehaviour
    {
        public GameObject selectedPiece;
        public EventSystem eventSystem;
        public GameController gameController;
        public static List<(Square, Move.MoveType)> highlightedSquares = new List<(Square, Move.MoveType)>();

        public Material highlight_1;
        public Material highlight_2;
        public Material highlight_3;
        public Material invisible;

        public Material frozen;
        static Board UpperBoard;
        static Board MiddleBoard;
        static Board LowerBoard;

        public void Start()
        {
            UpperBoard = GameController.getUpperBoard();
            MiddleBoard = GameController.getMiddleBoard();
            LowerBoard = GameController.getLowerBoard();
        }

        public bool IsPiece(int layer)
        {
            return (layer == 9 || layer == 10 || layer == 11);
        }

        public void ResetColor(Piece piece)
        {
            GameObject sObj = piece.pos.dot;
            sObj.GetComponent<Renderer>().material = invisible;
        }

        public void PromoteWarrior(Square s, Piece piece)
        {
            if (piece.color == Color.White)
            {
                if (s.row == 7)
                {
                    Destroy(piece.pieceGameObject);
                    GameObject newHero = gameController.piecePrefabs[6];
                    MiddleBoard.AddPieceAt(newHero, gameController.materials[9], Color.White, 7, s.col);
                    piece.pieceGameObject = newHero;
                    piece.type = PieceType.Hero;
                }
            }
            else
            {
                if (s.row == 0)
                {
                    Destroy(piece.pieceGameObject);
                    GameObject newHero = gameController.piecePrefabs[6];
                    MiddleBoard.AddPieceAt(newHero, gameController.materials[10], Color.Black, 0, s.col);
                    piece.pieceGameObject = newHero;
                    piece.type = PieceType.Hero;
                }
            }
        }

        public bool IsHighlighted(GameObject hitGameObject, ref Square square, ref Move.MoveType type)
        {
            foreach ((Square s, Move.MoveType m) in highlightedSquares)
            {
                if ((s.cubeObject == hitGameObject))
                {
                    square = s;
                    type = m;
                    return true;
                }
            }
            return false;
        }

        public bool IsOccupiedEnemySquare(GameObject hitGameObject, ref Square s, ref Move.MoveType m)
        {
            GameObject clicked = null;
            if (IsPiece(hitGameObject.layer))
            {
                Piece p = hitGameObject.GetComponent<Piece>();
                if (p != null)
                    clicked = p.pos.cubeObject;
            }
            if (clicked == null)
                return false;

            return (IsHighlighted(clicked, ref s, ref m));
        }

        public void HighlightSquares(GameObject selectedPiece, List<Move> possibleMoves)
        {
            Piece piece = selectedPiece.GetComponent<Piece>();
            Square s = piece.pos;
            s.dot.GetComponent<Renderer>().material = highlight_1;

            // Highlight all the possible moves generated for a piece
            foreach (Move move in possibleMoves)
            {
                GameObject dot = move.end.dot;
                highlightedSquares.Add((move.end, move.type));

                if (move.type == Move.MoveType.Regular)
                    dot.GetComponent<Renderer>().material = highlight_2;
                else if (move.type == Move.MoveType.Capture)
                    dot.GetComponent<Renderer>().material = highlight_3;
                else
                    dot.GetComponent<Renderer>().material = highlight_3;
            }
        }

        public void UnhighlightSquares()
        {
            foreach ((Square s, Move.MoveType m) in highlightedSquares)
            {
                s.dot.GetComponent<Renderer>().material = invisible;
            }
            highlightedSquares.Clear();
        }

        public static void RemoveIllegal(Player p, ref List<Move> moves)
		{
            List<Move> illegal = new List<Move>();

            foreach (Move m in moves)
			{
                Piece moving_piece = m.start.piece;
                m.end.occupied = true;

                if (m.type == Move.MoveType.Capture)
				{
                    Piece captured_piece = m.end.piece;
                    m.start.occupied = false;
                    m.start.piece = null;
                    m.end.piece = moving_piece;
                    moving_piece.pos = m.end;

                    if (GameController.IsCheck(p))
					{
                        illegal.Add(m);
					}
                    m.start.occupied = true;
                    m.start.piece = moving_piece;
                    m.end.piece = captured_piece;
                    moving_piece.pos = m.start;
                    captured_piece.pos = m.end;
                }
                else if (m.type == Move.MoveType.Swoop)
				{
                    print("Checking legality of swoop move...");
                    Piece captured_piece = m.end.piece;
                    m.end.occupied = false;

                    if (GameController.IsCheck(p))
                    {
                        illegal.Add(m);
                    }
                    m.start.piece = moving_piece;
                    moving_piece.pos = m.start;
                    m.end.occupied = true;
                    m.end.piece = captured_piece;
                }
                else
				{
                    m.start.occupied = false;
                    m.start.piece = null;
                    m.end.piece = moving_piece;
                    moving_piece.pos = m.end;

                    if (GameController.IsCheck(p))
                    {
                        illegal.Add(m);
                    }

                    m.start.occupied = true;
                    m.end.occupied = false;
                    m.end.piece = null;
                    m.start.piece = moving_piece;
                    moving_piece.pos = m.start;
                }
			}
            foreach (Move illegal_move in illegal)
                moves.Remove(illegal_move);
        }

        public void DisplayMoves(GameObject hitGameObject)
        {
            Piece piece;
            Square square;
            bool captured = false;

            // If we had a piece selected before this...
            if (selectedPiece != null)  
            {
                // Get the attached script of the piece GameObject
                piece = selectedPiece.GetComponent<Piece>();
                square = piece.pos;
                Square s = new Square(); Move.MoveType m = Move.MoveType.Regular;

                // Checks for a player clicking on a higlighted enemy piece,
                // capturing it rather than displaying that piece's moves
                if (IsOccupiedEnemySquare(hitGameObject, ref s, ref m))
                {
                    if (m == Move.MoveType.Swoop)
                    {
                        piece.RemoteCapture(s.piece);
                        GameController.ActivePlayer.prevMove = new Move(piece, square, square, m);
                        captured = true;
                    }
                    if (m == Move.MoveType.Capture)
                    {
                        piece.Capture(s.piece);
                        piece.MoveTo(s);
                        GameController.ActivePlayer.prevMove = new Move(piece, square, s, m);
                        captured = true;
                    }

                    // Check for warrior piece promotion
                    if (piece.type == PieceType.Warrior)
                        PromoteWarrior(s, piece);

                    gameController.SwitchTurn();
                }

                // Check to see if what we clicked was any of the highlighted squares
                else if (IsHighlighted(hitGameObject, ref s, ref m))
                {
                    if (m == Move.MoveType.Swoop)
                    {
                        piece.RemoteCapture(s.piece);
                        GameController.ActivePlayer.prevMove = new Move(piece, square, square, m);
                        captured = true;
                    }
                    if (m == Move.MoveType.Capture)
                    {
                        piece.Capture(s.piece);
                        piece.MoveTo(s);
                        captured = true;
                    }
                    if (m != Move.MoveType.Swoop)
                    {
                        piece.MoveTo(s);
                        GameController.ActivePlayer.prevMove = new Move(piece, square, s, m);
                    }

                    // Check for warrior piece promotion
                    if (piece.type == PieceType.Warrior)
                        PromoteWarrior(s, piece);

                    gameController.SwitchTurn();
                }
                eventSystem.SetSelectedGameObject(null);
                // As the piece leaves, reset the color of the square underneath it
                ResetColor(piece);
                // Now that the piece has left, un-highlight all of the move option squares
                UnhighlightSquares();
            }
  
            // Player clicked on one of their own pieces 
            if (!captured && IsPiece(hitGameObject.layer))
            {
                // Generate list of possible moves
                selectedPiece = hitGameObject;
                piece = selectedPiece.GetComponent<Piece>();

                if (piece.frozen)
                {
                    print("This piece is frozen.");
                }
                else
                {
                    // Check to make sure it's this piece's turn.
                    if (gameController.CurrentTurn() == piece.color)
                    {
                        List<Move> possibleMoves = piece.GetMoves();
                        if (piece.color == Color.White)
                        {
                            RemoveIllegal(GameController.P1, ref possibleMoves);
                        }
                        else if (piece.color == Color.Black)
                        {
                            RemoveIllegal(GameController.P2, ref possibleMoves);
                        }
                        HighlightSquares(selectedPiece, possibleMoves);
                    }
                    else
                    {
                        print("It's not that color's turn!");
                        eventSystem.SetSelectedGameObject(null);
                    }
                }
            }
        }
    }
}
