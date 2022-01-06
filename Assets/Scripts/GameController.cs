﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Dragonchess
{
    public enum Color { White, Black };
    public enum Layer { Upper, Middle, Lower };
    public enum PlayerType { Human, AI };

    public class GameController : MonoBehaviour
    {
        public static int layerMask;
        public static Player ActivePlayer;
        public static Player P1;
        public static Player P2;
        Color turn;

        public PlayerType P1_type;
        public PlayerType P2_type;

        public TextAsset board_init;
        public MoveController moveController;
        public AI AIController;

        public List<GameObject> boardObj;
        public List<GameObject> piecePrefabs;
        public List<Material> materials;
        public GameObject black_text;
        public GameObject white_text;

        public Material invisible;

        static List<(Square, Move.MoveType)> hightlightedSquares;
        static List<Board> boards;

        GameObject selectedPiece;

        static int moveNum;
        static public List<string> moveLog;

        public bool GameFromFileEnabled;
        public bool gameOver;
        public GameFromFile GFF;
        // --------------------------------------------------------------------

        public delegate Move getMoveDelegate(Player p);
        //public event getMoveDelegate getMoveEvent;

        // Start is called before the first frame update
        void Start()
        {
            gameOver = false;
            layerMask = ~0;
            moveNum = 0;
            moveLog = new List<string>();
            hightlightedSquares = new List<(Square, Move.MoveType)>();

            // If reading game data from a file (for debugging mostly)
            if (GameFromFileEnabled)
            {
                P1 = new Human(Color.White, PlayerType.Human);
                P2 = new Human(Color.Black, PlayerType.Human);
            }
            // If accepting moves from Human/AI input
            else
            {
                if (P1_type == PlayerType.Human)
                    P1 = new Human(Color.White, PlayerType.Human);
                else
                    P1 = new AI(Color.White, PlayerType.AI);

                if (P2_type == PlayerType.Human)
                    P2 = new Human(Color.Black, PlayerType.Human);
                else
                    P2 = new AI(Color.Black, PlayerType.AI);
            }
            NewGame();
        }

        // Initialize new game
        void NewGame()
        {
            Board UpperBoard = new Board(boardObj[0], 6, materials[0], materials[1]);
            Board MiddleBoard = new Board(boardObj[1], 7, materials[2], materials[3]);
            Board LowerBoard = new Board(boardObj[2], 8, materials[4], materials[5]);
            boards = new List<Board> { LowerBoard, MiddleBoard, UpperBoard };

            turn = Color.White;
            ActivePlayer = P1;

            int b = 0;
            // Instantiate all the square objects for the three board layers
            // Set material colors based on which layer we're instantiating
            foreach (Board currentBoard in boards)
            {
                for (int r = 0; r < Board.height; r++)
                {
                    for (int c = 0; c < Board.width; c++)
                    {
                        Material mat;
                        if ((r + c) % 2 == 0)
                            mat = currentBoard.upper_mat;
                        else
                            mat = currentBoard.lower_mat;

                        currentBoard.AddSquareAt(mat, invisible, b, r, c);
                    }
                }
                b++;
            }

            // Read in starting board state from the board_init text file
            string[] lines = File.ReadAllLines("Assets/Files/board_init.txt");
            foreach (string line in lines)
            {
                Board CurrentBoard;
                GameObject piece;
                Material m;
                Color c;

                string[] vals = line.Split(' ');
                int level = int.Parse(vals[0]);
                int piece_type = int.Parse(vals[1]);
                int color = int.Parse(vals[2]);
                int row = int.Parse(vals[3]);
                int col = int.Parse(vals[4]);

                if (level == 3)
                    CurrentBoard = UpperBoard;
                else if (level == 2)
                    CurrentBoard = MiddleBoard;
                else
                    CurrentBoard = LowerBoard;

                if (color == 0)
                {
                    c = Color.White;
                    m = materials[9];
                }
                else
                {
                    c = Color.Black;
                    m = materials[10];
                }

                piece = piecePrefabs[piece_type];
                CurrentBoard.AddPieceAt(piece, m, c, row, col);
                CurrentBoard.squares[row, col].occupied = true;
            }
        }

        void ResetGame()
		{
            OnApplicationQuit();
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        void OnClick()
        {
            if (gameOver)
                ResetGame();

            if (!GameFromFileEnabled)
            {
                // Get next AI move
                if (ActivePlayer.type == PlayerType.AI)
                {
                    Move next = ActivePlayer.GetMove();
                    ActivePlayer.prevMove = new Move(next.piece, next.start, next.end, next.type);
                    Piece piece = next.start.piece;
                    moveController.DoMove(piece, next);
                }
                // Or get next human move
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    RaycastHit hit;
                    // Check where we clicked
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                    {
                        // Locate GameObject of whatever we clicked on (piece or square)
                        GameObject hitGameObject = hit.transform.gameObject;
                        moveController.MoveSelect(hitGameObject);
                    }
                }
            }
            else
                return;
        }

        void OnNext()
		{
            if (GameFromFileEnabled)
                GFF.DoNext();
            else
                OnClick();
		}

        void OnPrev()
		{
            if (GameFromFileEnabled)
                GFF.UndoPrev();
        }

        public Move GetNextMove(Player player)
		{
            return player.GetMove();
		}

        // Saves the game's move data to a file
        public static void LogMove(Player player, bool isCheck)
        {
            // Uppercase char - indicate the name of the piece
            Move move = player.prevMove;

            string moveText = move.piece.nameChar;
            // If the move was a capture, indicate with an 'x'
            if (move.type == Move.MoveType.Capture)
                moveText += "x";
            // If the move was a remote capture, indicate with an '-'
            if (move.type == Move.MoveType.Swoop)
                moveText += "#";
            moveText += (move.end.board.m_layer - 5);
            // Coordinates of the square the piece moved to
            moveText += move.end.SquareName();
            // If the move put the opponent in check, indicate with a '+'
            if (isCheck)
                moveText += "+";

            if (player.color == Color.White)
                moveLog.Add((1 + moveNum) + ")\t" + moveText);
            else
            {
                moveLog.Add('\t' + moveText + '\n');
                moveNum++;
            }
        }

        public void DeletePrevLogEntry(Player player)
		{
            moveLog.RemoveAt(moveNum);
            moveNum--;
        }

        void OnApplicationQuit()
        {
            string path = "Assets/Resources/game_out.txt";
            StreamWriter w = new StreamWriter(path);

            foreach (string line in moveLog)
            {
                w.Write(line);
            }

            w.Close();
        }

        public Color CurrentTurn()
        {
            return turn;
        }

        public void SwitchTurn()
        {
            // See if P1's move put P2 in check
            if (IsCheck(P2))
            {
                print("PLAYER 2'S KING IN CHECK.");
                P2.inCheck = true;
                Square.SetColor(GetKing(P2).pos, materials[11]);
                if (IsCheckmate(P2))
                {
                    print("GAME OVER: CHECKMATE. PLAYER 1 WINS.");
                    gameOver = true;
                    return;
                }
            }
            else
            {
                P2.inCheck = false;
                Square.SetColor(GetKing(P2).pos, GetKing(P2).pos.properMaterial);
            }
            // See if P2's move put P1 in check
            if (IsCheck(P1))
            {
                P1.inCheck = true;
                print("PLAYER 1'S KING IN CHECK.");
                Square.SetColor(GetKing(P1).pos, materials[11]);
                if (IsCheckmate(P1))
                {
                    print("GAME OVER: CHECKMATE. PLAYER 2 WINS.");
                    gameOver = true;
                    return;
                }
            }
            else
            {
                P1.inCheck = false;
                Square.SetColor(GetKing(P1).pos, GetKing(P1).pos.properMaterial);
            }

            // Switch the currently active player
            if (ActivePlayer == P1)
            {
                white_text.SetActive(false);
                black_text.SetActive(true);
                turn = Color.Black;
                ActivePlayer = P2;
            }
            else
            {
                black_text.SetActive(false);
                white_text.SetActive(true);
                turn = Color.White;
                ActivePlayer = P1;
            }

        }

        // Evaluate whether or not player p's king is in check
        public static bool IsCheck(Player p)
        {
            Piece king = GetKing(p);
            Player enemy = GetEnemy(p);
            Square current = king.pos;

            foreach (Piece enemy_piece in enemy.pieces)
            {
                if (enemy_piece.type != PieceType.King)
                {
                    foreach (Move enemy_move in enemy_piece.GetMoves())
                    {
                        if (enemy_move.type == Move.MoveType.Capture || enemy_move.type == Move.MoveType.Swoop)
                        {
                            if (enemy_move.end == king.pos)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        // Given that the king is in check, see if it is also checkmate
        public static bool IsCheckmate(Player player)
        {
            List<(Piece p, Move m)> safe_moves = new List<(Piece p, Move m)>();

            // Iterate through all the pieces - see if we can move ANY of
            // the player's pieces to escape check. If so, it is not checkmate.
            foreach (Piece p in player.pieces)
            {
                List<Move> potential_moves = p.GetMoves();
                MoveController.RemoveIllegal(player, ref potential_moves);
                if (potential_moves.Count > 0)
                    return false;
            }
            return true;
        }

        public static Player GetEnemy(Player player)
		{
            if (player == P1)
                return P2;
            else
                return P1;
        }

        public static Piece GetKing(Player p)
        {
            return p.pieces.Find(x => (x.type == PieceType.King));
        }

        static public Board getUpperBoard()
        {
            return boards[2];
        }

        static public Board getMiddleBoard()
        {
            return boards[1];
        }

        static public Board getLowerBoard()
        {
            return boards[0];
        }

        public static void AddDotAt(Square s, int layer)
        {
            GameObject newDot = Instantiate(Resources.Load("dot", typeof(GameObject))) as GameObject;
            Transform t = newDot.transform;
            newDot.transform.parent = s.cubeObject.transform;
            newDot.transform.localPosition = t.position;
            newDot.layer = layer;
            s.dot = newDot;
        }
    }
}
