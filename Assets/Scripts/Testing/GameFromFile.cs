using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dragonchess
{
    using GC = GameController;
    using static Gamestate;
    using static GameController;
    using static MoveController;

    public class GameFromFile : MonoBehaviour
    {
        public MoveController MC;
        public GameController G;
        static public List<Move> moveLog;
        static int linesRead;
        static int moveIndex;
        static int movesParsed;
        static public bool isWhitesTurn;
        static public string[] lines;
        
        // Start is called before the first frame update
        void Start()
        {
            linesRead = 0;
            moveIndex = 0;
            movesParsed = 0;
            moveLog = new List<Move>();
            isWhitesTurn = true;
            // Read in starting board state from the board_init text file
            lines = File.ReadAllLines("Assets/Resources/game_example_1.txt");
        }

        static Move ReadLine(Gamestate state)
        {
            string line = lines[linesRead];

            string[] vals = line.Split('\t');
            string move_num = vals[0];
            string P1_move = vals[1];
            string P2_move = vals[2];

            Move m;

            if (isWhitesTurn) {
                m = ParseMove(state, P1_move, state.P1, state.P2);
                isWhitesTurn = false;
            }
            else
            {
                m = ParseMove(state, P2_move, state.P2, state.P1);
                isWhitesTurn = true;
                linesRead++;
            }
            movesParsed++;
            moveLog.Add(m);
            return m;
        }

        static public Move ParseMove(Gamestate state, string line, Player P1, Player P2)
		{
            Square start = null;
            Square end = null;
            Piece piece = null;
            MoveType moveType = MoveType.NULL;

            // Remove the checkmate indicator
            if (line.Contains("+"))
            {
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
                moveType = MoveType.Capture;
                string[] moveInfo = line.Split('x');
                PieceType attacker_type = CharToType(P1, moveInfo[0].ToString());
                PieceType captured_type = CharToType(P2, moveInfo[1].ToString());

                if (moveInfo[0].Length > 1)
                {
                    start = StringToSquare(state, moveInfo[0].Substring(1));
                    piece = start.piece;
                    // Case 1: We are given disambiguated pos for both pieces
                    if (moveInfo[1].Length > 1)
                    {
                        end = StringToSquare(state, moveInfo[1].Substring(1));                    
                    }
                    // Case 2: We are given a disambiguated pos for the attacking
					// piece but not for the captured piece
                    else
                    {
                        foreach (Move m in piece.GetMoves(state))
                        {
                            if (m.type == MoveType.Capture && m.end.piece.type == captured_type)
                            {
                                end = m.end;
                                break;
                            }
                            else if (m.type == MoveType.Swoop && m.end.piece.type == captured_type)
                            {
                                moveType = MoveType.Swoop;
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
                        end = StringToSquare(state, moveInfo[1].Substring(1));
                        foreach (Piece p in potentialPieces)
						{
                            foreach (Move m in p.GetMoves(state))
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
                            foreach (Move m in p.GetMoves(state))
							{
                                if (m.type == MoveType.Capture && m.end.piece.type == captured_type)
								{
                                    start = m.start;
                                    end = m.end;
                                    piece = p;
								}
                                else if (m.type == MoveType.Swoop && m.end.piece.type == captured_type)
                                {
                                    moveType = MoveType.Swoop;
                                    start = m.start;
                                    end = m.end;
                                    piece = p;
                                }
							}
						}
					}
                }
            }
            else
			{
                moveType = MoveType.Regular;

                // If a character was included to differentiate an ambiguous move
                if (line.Contains("-"))
                {
                    string[] pos = line.Substring(1).Split('-');
                    start = StringToSquare(state, pos[0]);
                    end = StringToSquare(state, pos[1]);
                    piece = start.piece;
                }
                // If we're only given the piece's letter followed by where it moves to
                else
				{
                    end = StringToSquare(state, line.Substring(1));
                    foreach (Piece p in potentialPieces)
                    {
                        foreach (Move m in p.GetMoves(state))
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

        static public Square StringToSquare(Gamestate state, string s)
		{
            int b = (int)char.GetNumericValue(s[0]);
            int c = Array.FindIndex(Square.Letters, l => l == (s[1]).ToString());
            int r = (int)char.GetNumericValue(s[2]) - 1;

            Board board;

            if (b == 1)
                board = state.upperBoard;
            else if (b == 2)
                board = state.middleBoard;
            else
                board = state.lowerBoard;

            return board.squares[r, c];
		}

        static public PieceType CharToType(Player player, string c)
		{
            PieceType t = PieceType.EMPTY;

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

        public void DoNext(Gamestate state)
		{
            if (moveIndex >= movesParsed)
			{
                Move m = ReadLine(state);

                DoMove(state, m, true);
            }
            else
			{
                DoMove(state, moveLog[moveIndex], true);
			}
            moveLog[moveIndex].piece.player.prevMove = moveLog[moveIndex];
            moveIndex++;
        }

        public void UndoPrev(Gamestate state)
        {
            if (moveIndex == 0)
			{
                print("No moves to undo!");
                return;
			}

            moveIndex--;
            Move prev = moveLog[moveIndex];
            prev.piece.player.prevMove = prev;

            if (prev.type == MoveType.Capture ||
                prev.type == MoveType.Swoop)
			{
                // Add the captured piece back
                Piece cap = prev.captured;
                cap.player.pieces.Add(cap);

                // Move the captured piece back
                Move undoCap = new Move(cap, prev.end, prev.end, MoveType.Regular);
                DoMove(state, undoCap, true);

                // Move the capturing piece back
                Move undoMove = new Move(prev.piece, prev.start, prev.start, MoveType.Regular);
                DoMove(state, undoMove, true);
            }
            else
			{
                prev.start.piece = prev.piece;
                prev.start.occupied = true;
                prev.end.occupied = false;
                prev.piece.pos = prev.start;
                prev.piece.MoveTo(state, prev.start);
            }
        }
    }
}
