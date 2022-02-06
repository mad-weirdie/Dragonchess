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
	using static BitMoveController;
	using static BitMove;
	using static Game;
    using static GameController;
    using static MoveController;
	using static BitPiece;

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
            //lines = File.ReadAllLines("Assets/Resources/game_example_1.txt");
			lines = File.ReadAllLines("Assets/Resources/fast_checkmate_game.txt");
		}

		static Move ReadLine(Game state)
        {
            string line = lines[linesRead];

            string[] vals = line.Split('\t');
            string move_num = vals[0];
            string P1_move = vals[1];
            string P2_move = vals[2];

            Move m;

            if (isWhitesTurn) {
                m = ParseMove(state, P1_move, true);
                isWhitesTurn = false;
            }
            else
            {
                m = ParseMove(state, P2_move, false);
                isWhitesTurn = true;
                linesRead++;
            }
            movesParsed++;
            moveLog.Add(m);
            return m;
        }

        static public Move ParseMove(Game g, string line, bool whiteToMove)
		{
			Gamestate state = new Gamestate(g);
			int color = 0;
			if (!whiteToMove)
				color = 1;

			int piece = -1;
			int sBoard = -1;
			int sIndex = -1;
			int eBoard = -1;
			int eIndex = -1;

            MoveType moveType = MoveType.NULL;
            // Remove the checkmate indicator
            if (line.Contains("+"))
                line = line.Replace("+", "");

            // Get piece type from the first character of the move
            List<int> potentialPieces = new List<int>();
			List<int> playerPieces = GetPieces(state, whiteToMove);
            PieceType pieceType = CharToType(playerPieces, whiteToMove, line[0].ToString());

			for (int i = 0; i < playerPieces.Count; i++)
			{
				int p = playerPieces[i];
				if ((PieceType)PieceType(p) == pieceType)
                    potentialPieces.Add(p);
			}

            // Figure out the type of move
            if (line.Contains("x"))
			{
                moveType = MoveType.Capture;
                string[] moveInfo = line.Split('x');
                PieceType attacker_type = CharToType(playerPieces, whiteToMove, moveInfo[0].ToString());
				PieceType captured_type = CharToType(playerPieces, whiteToMove, moveInfo[1].ToString());

				if (moveInfo[0].Length > 1)
                {
					// Case 1: We are given disambiguated pos for both pieces
					(sBoard, sIndex) = StringToIndex(state, moveInfo[0].Substring(0));
					piece = NewBitPiece(color, (int)pieceType, sBoard, sIndex / 12, sIndex % 12);
                    if (moveInfo[1].Length > 1)
                        (eBoard, eIndex) = StringToIndex(state, moveInfo[1].Substring(0));
						
					// Case 2: We are given a disambiguated pos for the attacking
					// piece but not for the captured piece
                    else
                    {
						List<int> moves = GetBitMoves(state, piece, true);
						for (int i = 0; i < moves.Count; i++)
                        {
							int m = moves[i];
							MoveType MT = (MoveType)BitMoveType(m);
							PieceType CT = (PieceType)PieceType(GetEndPiece(state, m));
							if (MT == MoveType.Capture && CT == captured_type)
                            {
								eBoard = EndBoard(m);
								eIndex = EndIndex(m);
                                break;
                            }
                            else if (MT == MoveType.Swoop && CT == captured_type)
                            {
                                moveType = MoveType.Swoop;
								eBoard = EndBoard(m);
								eIndex = EndIndex(m);
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
                        (eBoard, eIndex) = StringToIndex(state, moveInfo[1].Substring(1));
						for (int i = 0; i < potentialPieces.Count; i++)
						{
							int p = potentialPieces[i];
							List<int> moves = GetBitMoves(state, p);
							for (int j = 0; j < moves.Count; j++)
							{
								int m = moves[j];
                                if (EndBoard(m) == eBoard && EndIndex(m) == eIndex) {
                                    sBoard = StartBoard(m);
                                    sIndex = StartIndex(m);
									piece = p;
									break;
                                }
							}
						}
					}
                    // Case 4: we are not given information about the pos of either piece
                    else
					{
						for (int i = 0; i < potentialPieces.Count; i++)
						{
							int p = potentialPieces[i];
							List<int> moves = GetBitMoves(state, p);
							for (int j = 0; j < moves.Count; j++)
							{
								int m = moves[j];
								MoveType MT = (MoveType)BitMoveType(m);
								PieceType CT = (PieceType)PieceType(GetEndPiece(state, m));
								if (MT == MoveType.Capture && CT == captured_type)
								{
									sIndex = StartBoard(m);
									sIndex = StartIndex(m);
                                    eBoard = EndBoard(m);
									eIndex = EndIndex(m);
                                    piece = p;
								}
                                else if (MT == MoveType.Swoop && CT == captured_type)
                                {
                                    moveType = MoveType.Swoop;
									sIndex = StartBoard(m);
									sIndex = StartIndex(m);
									eBoard = EndBoard(m);
									eIndex = EndIndex(m);
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
                    (sBoard, sIndex) = StringToIndex(state, pos[0]);
                    (eBoard, eIndex) = StringToIndex(state, pos[1]);
                }
                // If we're only given the piece's letter followed by where it moves to
                else
				{
                    (eBoard, eIndex) = StringToIndex(state, line.Substring(1));
					for (int i = 0; i < potentialPieces.Count; i++)
					{
						int p = potentialPieces[i];
						List<int> moves = GetBitMoves(state, p);
						for (int j = 0; j < moves.Count; j++)
						{
							int m = moves[j];
							if (EndIndex(m) == eIndex)
							{
								sBoard = StartBoard(m);
                                sIndex = StartIndex(m);
                                piece = p;
                                break;
							}
                        }
                    }
                }
			}

			if (sBoard == -1)
				print("Error: sBoard not found");
			if (sIndex == -1)
				print("Error: sIndex not found");
			if (eBoard == -1)
				print("Error: eBoard not found");
			if(eIndex == -1)
				print("Error: eIndex not found");

			int bitMove = CreateBitMove((int)moveType, sBoard, sIndex, eBoard, eIndex, 0);
			Move newMove = BitMoveToMove(g, bitMove);
			if (newMove.type == MoveType.Capture || newMove.type == MoveType.Swoop)
			{
				if (newMove.end.piece == null)
					print("ERROR: END PIECE NULL");
				newMove.captured = newMove.end.piece;
			}
			return newMove;
		}

        static public (int,int) StringToIndex(Gamestate state, string s)
		{
            int b = (int)char.GetNumericValue(s[0]);
            int c = Array.FindIndex(Square.Letters, l => l == (s[1]).ToString());
            int r = (int)char.GetNumericValue(s[2]) - 1;
			return (b, r * 12 + c);
		}

        static public PieceType CharToType(List<int> pieces, bool whiteToMove, string c)
		{
            PieceType t = PieceType.EMPTY;

			for (int i = 0; i < pieces.Count; i++)
			{
				int p = pieces[i];
				int pType = PieceType(p);
				if (Char.ToUpper(pieceCodes[pType+1]) == c[0])
                {
                    t = (PieceType)(PieceType(p));
                    break;
                }
            }
            return t;
        }

        public void GetNext(Game state)
		{
            if (moveIndex >= movesParsed)
			{
                Move m = ReadLine(state);
				OnMoveReceived(state, state.ActivePlayer, m);
            }
            else
			{
				OnMoveReceived(state, state.ActivePlayer, moveLog[moveIndex]);
			}
            moveIndex++;
        }

        public void UndoPrev(Game state)
        {
            if (moveIndex == 0)
			{
                print("No moves to undo!");
                return;
			}

            moveIndex--;
        }
    }
}
