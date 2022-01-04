using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Dragonchess
{
    using GC = GameController;
    public class MoveController : MonoBehaviour
    {
        public GameObject selectedObj;
        public EventSystem eventSystem;
        public GameController G;
        public static List<(Square, Move.MoveType)> highlightedSquares = new List<(Square, Move.MoveType)>();

        public Material highlight_1;
        public Material highlight_2;
        public Material highlight_3;
        public Material invisible;

        public GameObject hero;

        public Material frozen;
        static Board UpperBoard;
        static Board MiddleBoard;
        static Board LowerBoard;

        public void Start()
        {
            UpperBoard = GC.getUpperBoard();
            MiddleBoard = GC.getMiddleBoard();
            LowerBoard = GC.getLowerBoard();
        }

        public bool IsSquare(GameObject g)
		{
            int layer = g.layer;
            return (layer == 6 || layer == 7 || layer == 8);
        }

        public bool IsPiece(GameObject g)
        {
            int layer = g.layer;
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
                    GameObject newHero = G.piecePrefabs[6];
                    MiddleBoard.AddPieceAt(newHero, G.materials[9], Color.White, 7, s.col);
                    piece.pieceGameObject = newHero;
                    piece.type = PieceType.Hero;
                }
            }
            else
            {
                if (s.row == 0)
                {
                    Destroy(piece.pieceGameObject);
                    GameObject newHero = G.piecePrefabs[6];
                    MiddleBoard.AddPieceAt(newHero, G.materials[10], Color.Black, 0, s.col);
                    piece.pieceGameObject = newHero;
                    piece.type = PieceType.Hero;
                }
            }
        }

        public bool CheckMove(GameObject clickedObj, ref Square square)
        {
            foreach (Move m in Piece.ObjToPiece(selectedObj).availableMoves)
            {
                if ((m.end.cubeObject == clickedObj))
                {
                    square = m.end;
                    return true;
                }
            }
            return false;
        }

        public bool IsEnemyPiece(GameObject clickedObj)
        {
            if (selectedObj == null)
                return false;
            Piece P1 = Piece.ObjToPiece(selectedObj);
            Piece P2 = Piece.ObjToPiece(clickedObj);

            return (P1.player != P2.player);
        }

        public void HighlightSquares(Piece piece)
        {
            Square s = piece.pos;
            s.dot.GetComponent<Renderer>().material = highlight_1;

            // Highlight all the possible moves generated for a piece
            foreach (Move move in piece.availableMoves)
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
                if (m.type == Move.MoveType.Capture)
                {
                    // Pretend to move attacking piece
                    Piece attacking_piece = m.start.piece;
                    Piece captured_piece = m.end.piece;
                    m.start.occupied = false;

                    // Pretend to capture attacked piece
                    attacking_piece.pos = m.end;
                    m.end.occupied = true;
                    m.end.piece = attacking_piece;

                    // Remove captured piece from enemy's list of pieces
                    GC.GetEnemy(p).pieces.Remove(captured_piece);

                    if (GC.IsCheck(p))
                    {
                        illegal.Add(m);
                    }

                    // Move attacking piece back to original location
                    m.start.occupied = true;
                    m.start.piece = attacking_piece;
                    m.end.piece = captured_piece;
                    attacking_piece.pos = m.start;
                    captured_piece.pos = m.end;

                    // Re-add captured piece back to enemy's list of pieces
                    GC.GetEnemy(p).pieces.Add(captured_piece);
                }
                else if (m.type == Move.MoveType.Swoop)
                {
                    // Pretend to capture the attacked piece
                    Piece attacking_piece = m.start.piece;
                    Piece captured_piece = m.end.piece;
                    m.end.occupied = false;

                    // Remove captured piece from enemy's list of pieces
                    GC.GetEnemy(p).pieces.Remove(captured_piece);

                    if (GC.IsCheck(p))
                    {
                        illegal.Add(m);
                    }
                    // Un-capture the attacked piece
                    m.end.occupied = true;
                    m.end.piece = captured_piece;

                    // Re-add captured piece back to enemy's list of pieces
                    GC.GetEnemy(p).pieces.Add(captured_piece);
                }
                else if (m.type == Move.MoveType.Regular)
                {
                    Piece moving_piece = m.start.piece;
                    m.start.occupied = false;
                    m.end.occupied = true;
                    moving_piece.pos = m.end;
                    m.end.piece = moving_piece;

                    if (GC.IsCheck(p))
                    {
                        illegal.Add(m);
                    }

                    m.start.occupied = true;
                    m.end.occupied = false;
                    m.start.piece = moving_piece;
                    moving_piece.pos = m.start;
                    m.end.piece = null;

                }
                else
                {
                    print("ERROR: Unknown move type.");
                }
            }
            foreach (Move illegal_move in illegal)
                moves.Remove(illegal_move);
        }

        public void MoveSelect(GameObject clickedObj)
        {
            Piece piece = null;
            Square goal = null;

            if (IsSquare(clickedObj) && selectedObj != null)
            {
                piece = Piece.ObjToPiece(selectedObj);
                CheckMove(clickedObj, ref goal);
            }
            else if (IsPiece(clickedObj) && selectedObj != null)
            {
                piece = Piece.ObjToPiece(selectedObj);
                if (IsEnemyPiece(clickedObj))
                {
                    Piece enemy = Piece.ObjToPiece(clickedObj);
                    if (piece.CanMoveTo(enemy.pos))
                        goal = enemy.pos;
                }
            }

            eventSystem.SetSelectedGameObject(null);
            if (goal != null)
            {
                DoMove(piece, piece.GetMoveWithGoal(goal));
                // As the piece leaves, reset the color of the square underneath it
                ResetColor(piece);
                // Now that the piece has left, un-highlight all of the move option squares
                UnhighlightSquares();
                selectedObj = null;

                return;
            }

            if (IsPiece(clickedObj) && selectedObj == null)
            {
                print("wow");
                // Generate list of possible moves
                selectedObj = clickedObj;
                piece = selectedObj.GetComponent<Piece>();

                // Check to make sure it's this piece's turn.
                if (piece.player == GC.ActivePlayer && !piece.frozen)
                {
                    List<Move> possibleMoves = piece.GetMoves();
                    RemoveIllegal(piece.player, ref possibleMoves);
                    piece.availableMoves = possibleMoves;
                    HighlightSquares(piece);
                }
                else
                {
                    print("It's not that side's turn!");
                    eventSystem.SetSelectedGameObject(null);
                    // As the piece leaves, reset the color of the square underneath it
                    ResetColor(piece);
                    // Now that the piece has left, un-highlight all of the move option squares
                    UnhighlightSquares();
                    selectedObj = null;
                }
                if (piece.frozen)
                {
                    print("This piece is frozen.");
                }
            }
            eventSystem.SetSelectedGameObject(null);
        }

        public void DoMove(Piece p, Move m)
        {
            m.piece = p;
            GC.ActivePlayer.prevMove = m;

            if (m.type == Move.MoveType.Swoop)
			{
                m.captured = m.end.piece;
                p.RemoteCapture(m.end.piece);
            }
            else if (m.type == Move.MoveType.Capture)
			{
                m.captured = m.end.piece;
                p.Capture(m.end.piece);
                print(m.captured);
                p.MoveTo(m.end);
            }
            else    // m == Move.MoveType.Regular
			{
                if (p == null)
                    print("error: piece is null");
                p.MoveTo(m.end);
            }

            // Check for warrior piece promotion
            if (p.type == PieceType.Warrior)
                PromoteWarrior(m.end, p);

            G.SwitchTurn();
        }
    }
}
