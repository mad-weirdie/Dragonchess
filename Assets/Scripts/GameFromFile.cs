using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dragonchess
{
    using GC = GameController;
    public class GameFromFile : MonoBehaviour
    {
        public MoveController MC;
        public GameController G;
        static public List<Move> moveLog;
        static int currentLine = 0;
        static int currentMove = 0;
        static public bool isWhitesTurn;
        static public string[] lines;
        
        // Start is called before the first frame update
        void Start()
        {
            moveLog = new List<Move>();
            isWhitesTurn = true;
            // Read in starting board state from the board_init text file
            lines = File.ReadAllLines("Assets/Resources/game_example_1.txt");
        }

        static Move ReadLine()
        {
            string line = lines[currentLine];

            string[] vals = line.Split('\t');
            string move_num = vals[0];
            string P1_move = vals[1];
            string P2_move = vals[2];

            Move m;

            if (isWhitesTurn) {
                m = ParseMove(P1_move, GC.P1);
                isWhitesTurn = false;
            }
            else
            {
                m = ParseMove(P2_move, GC.P2);
                isWhitesTurn = true;
            }

            moveLog.Add(m);
            if (isWhitesTurn)
            {
                currentLine++;
            }
            return m;
        }

        static public Move ParseMove(string line, Player pNum)
		{
            Player P1 = pNum;
            Player P2 = GameController.GetEnemy(pNum);

            Square start = null;
            Square end = null;
            Piece piece = null;
            Move.MoveType moveType = Move.MoveType.NULL;

            // Remove the checkmate indicator
            if (line.Contains("+"))
            {
                print("check");
                line = line.Replace("+", "");
            }

            // Get piece type from the first character of the move
            PieceType t1 = CharToType(P1, line[0].ToString());

            List<Piece> potentialPieces = new List<Piece>();
            foreach (Piece p in P1.pieces)
			{
                if (p.type == t1)
                    potentialPieces.Add(p);
			}

            // Figure out the type of move
            if (line.Contains("x"))
			{
                moveType = Move.MoveType.Capture;
                string[] moveInfo = line.Split('x');
                PieceType attacker_type = CharToType(P1, moveInfo[0].ToString());
                PieceType captured_type = CharToType(P2, moveInfo[1].ToString());

                if (moveInfo[0].Length > 1)
                {
                    start = StringToSquare(moveInfo[0].Substring(1));
                    piece = start.piece;
                    // Case 1: We are given disambiguated pos for both pieces
                    if (moveInfo[1].Length > 1)
                    {
                        end = StringToSquare(moveInfo[1].Substring(1));                    
                    }
                    // Case 2: We are given a disambiguated pos for the attacking
					// piece but not for the captured piece
                    else
                    {
                        foreach (Move m in piece.GetMoves())
                        {
                            if (m.type == Move.MoveType.Capture && m.end.piece.type == captured_type)
                            {
                                end = m.end;
                                break;
                            }
                            else if (m.type == Move.MoveType.Swoop && m.end.piece.type == captured_type)
                            {
                                moveType = Move.MoveType.Swoop;
                                end = m.end;
                                break;
                            }
                        }
					}
                }
                else
				{
                    // Case 3: we are given a disambiguated pos for the captured
                    // piece but not for the attacking piece
                    if (moveInfo[1].Length > 1)
                    {
                        end = StringToSquare(moveInfo[1].Substring(1));
                        foreach (Piece p in potentialPieces)
						{
                            foreach (Move m in p.GetMoves())
							{
                                if (m.end == end) {
                                    start = m.start;
                                    piece = p;
                                }
							}
						}
                    }
                    // Case 4: we are not given information about the pos of either piece
                    else
					{
                        foreach (Piece p in potentialPieces)
						{
                            foreach (Move m in p.GetMoves())
							{
                                //print("Potential move: " + m.type + " " + m.start.SquareName() + " " + m.end.SquareName());
                                if (m.type == Move.MoveType.Capture && m.end.piece.type == captured_type)
								{
                                    start = m.start;
                                    end = m.end;
                                    piece = p;
								}
                                else if (m.type == Move.MoveType.Swoop && m.end.piece.type == captured_type)
                                {
                                    moveType = Move.MoveType.Swoop;
                                    start = m.start;
                                    end = m.end;
                                    piece = p;
                                }
							}
						}
					}
                }
            }

            /*else if (P1_move.Contains("#"))
			{
                moveType = Move.MoveType.Swoop;
                string[] moveInfo = P1_move.Split('#');
            }*/
            else
			{
                moveType = Move.MoveType.Regular;

                // If a character was included to differentiate an ambiguous move
                if (line.Contains("-"))
                {
                    string[] pos = line.Substring(1).Split('-');
                    start = StringToSquare(pos[0]);
                    end = StringToSquare(pos[1]);
                    piece = start.piece;
                }
                // If we're only given the piece's letter followed by where it moves to
                else
				{
                    end = StringToSquare(line.Substring(1));
                    foreach (Piece p in potentialPieces)
                    {
                        foreach (Move m in p.GetMoves())
                        {
                            if (m.end == end)
							{
                                start = m.start;
                                piece = p;
                                break;
							}
                        }
                    }
                }
			}
            
            return (new Move(piece, start, end, moveType));
        }

        static public Square StringToSquare(string s)
		{
            int b = (int)char.GetNumericValue(s[0]);
            int c = Array.FindIndex(Square.Letters, l => l == (s[1]).ToString());
            int r = (int)char.GetNumericValue(s[2]) - 1;

            Board board;

            if (b == 1)
                board = GC.getUpperBoard();
            else if (b == 2)
                board = GC.getMiddleBoard();
            else
                board = GC.getLowerBoard();

            return board.GetSquare(r, c);
		}

        static public PieceType CharToType(Player player, string c)
		{
            PieceType t = PieceType.NULL;

            foreach (Piece p in player.pieces)
            {
                if (p.nameChar == c)
                {
                    t = p.type;
                    break;
                }
            }
            return t;
        }

        public void DoNext()
		{
            if (currentMove+1 > currentLine*2)
			{
                Move m = ReadLine();
                MC.DoMove(m.piece, m);
            }
            else
			{
                MC.DoMove(moveLog[currentMove].piece, moveLog[currentMove]);
			}
            currentMove++;
        }

        public void UndoPrev()
        {
            if (currentMove == 0)
			{
                print("No moves to undo!");
                return;
			}
            currentMove--;
            Move prev = moveLog[currentMove];

            if (prev.type == Move.MoveType.Capture ||
                prev.type == Move.MoveType.Swoop)
			{
                Piece cap = prev.captured;

			}
        }
    }
}
