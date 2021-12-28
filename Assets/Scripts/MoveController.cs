using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Dragonchess
{
    public class MoveController : MonoBehaviour
    {
        GameObject selectedPiece;

        public EventSystem eventSystem;
        public Game gameState;

        public static GameController gameController;
        public static List<(Square, Move.MoveType)> highlightedSquares = new List<(Square, Move.MoveType)>();

        public Material highlight_1;
        public Material highlight_2;
        public Material highlight_3;

        static Board UpperBoard;
        static Board MiddleBoard;
        static Board LowerBoard;

        public void Start()
        {
            UpperBoard = GameController.getUpperBoard();
            MiddleBoard = GameController.getUpperBoard();
            LowerBoard = GameController.getUpperBoard();
        }

        public bool IsPiece(int layer)
        {
            return (layer == 9 || layer == 10 || layer == 11);
        }

        public void ResetColor(Piece piece)
        {
            GameObject sObj = piece.location.cubeObject;
            sObj.GetComponent<Renderer>().material = piece.location.properMaterial;
        }

        public void PromoteWarrior(Square s, Piece piece)
        {
            if (piece.color == Color.White)
            {
                if (s.row == 7)
                {
                    Destroy(piece.pieceGameObject);
                    GameObject newHero = gameController.piecePrefabs[6];
                    MiddleBoard.AddPieceAt(newHero, gameController.white_pieces_mat, Color.White, 7, s.col);
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
                    MiddleBoard.AddPieceAt(newHero, gameController.black_pieces_mat, Color.Black, 0, s.col);
                    piece.pieceGameObject = newHero;
                    piece.type = PieceType.Hero;
                }
            }
        }

        public bool IsHighlighted(GameObject hitGameObject, ref Square square, ref Move.MoveType type)
        {
            foreach ((Square s, Move.MoveType m) in highlightedSquares)
            {
                if ((s.cubeObject == hitGameObject) && m != Move.MoveType.Swoop)
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
                    clicked = p.location.cubeObject;
            }
            if (clicked == null)
                return false;

            return (IsHighlighted(clicked, ref s, ref m));
        }

        public void HighlightSquares(GameObject selectedPiece, ArrayList possibleMoves)
        {
            Piece piece = selectedPiece.GetComponent<Piece>();
            GameObject sObj = piece.location.cubeObject;
            sObj.GetComponent<Renderer>().material = highlight_1;

            // Highlight all the possible moves generated for a piece
            foreach (Move move in possibleMoves)
            {
                Square endSquare = move.end;
                GameObject squareObj = endSquare.cubeObject;
                highlightedSquares.Add((endSquare, move.type));

                if (move.type == Move.MoveType.Regular)
                    squareObj.GetComponent<Renderer>().material = highlight_2;
                else if (move.type == Move.MoveType.Capture)
                    squareObj.GetComponent<Renderer>().material = highlight_3;
                else
                    squareObj.GetComponent<Renderer>().material = highlight_1;
            }
        }

        public void UnhighlightSquares()
        {
            foreach ((Square s, Move.MoveType m) in highlightedSquares)
            {
                s.cubeObject.GetComponent<Renderer>().material = s.properMaterial;
            }
            highlightedSquares.Clear();
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
                square = piece.location;
                Square s = new Square(); Move.MoveType m = Move.MoveType.Regular;

                // Checks for a player clicking on a higlighted enemy piece,
                // capturing it rather than displaying that piece's moves
                if (IsOccupiedEnemySquare(hitGameObject, ref s, ref m))
                {
                    if (m == Move.MoveType.Capture)
                        piece.Capture(s.piece);
                    piece.MoveTo(s);
                    gameState.SwitchTurn();
                    captured = true;
                }

                // Check to see if what we clicked was any of the highlighted squares
                if (IsHighlighted(hitGameObject, ref s, ref m))
                {
                    if (m == Move.MoveType.Capture)
                        piece.Capture(s.piece);
                    piece.MoveTo(s);
                    gameState.SwitchTurn();

                    // Check for warrior piece promotion
                    if (piece.type == PieceType.Warrior)
                        PromoteWarrior(s, piece);
                }

                // As the piece leaves, reset the color of the square underneath it
                ResetColor(piece);
                // Now that the piece has left, un-highlight all of the move option squares
                UnhighlightSquares();
            }
  
            if (!captured && IsPiece(hitGameObject.layer))
            {
                // Generate list of possible moves
                selectedPiece = hitGameObject;
                piece = selectedPiece.GetComponent<Piece>();

                // Check to make sure it's this piece's turn.
                if (gameState.turn == piece.color)
                {
                    ArrayList possibleMoves = piece.GetMoves();
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
