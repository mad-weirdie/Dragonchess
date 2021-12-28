using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dragonchess
{
    public class GameController : MonoBehaviour
    {
        public TextAsset board_init;
        public MoveController mController;

        GameObject selectedPiece;
        List<(Square, Move.MoveType)> hightlightedSquares = new List<(Square, Move.MoveType)>();

        public GameObject UpperBoardGameObject;
        public GameObject MiddleBoardGameObject;
        public GameObject LowerBoardGameObject;

        public List<GameObject> piecePrefabs;

        public Material UW_material;
        public Material UB_material;
        public Material MW_material;
        public Material MB_material;
        public Material LW_material;
        public Material LB_material;

        public Material black_pieces_mat;
        public Material white_pieces_mat;

        static Board UpperBoard;
        static Board MiddleBoard;
        static Board LowerBoard;

        static public Board getUpperBoard()
        {
            return UpperBoard;
        }

        static public Board getMiddleBoard()
        {
            return MiddleBoard;
        }

        static public Board getLowerBoard()
        {
            return LowerBoard;
        }

        // Start is called before the first frame update
        void Start()
        {
            UpperBoard = new Board(UpperBoardGameObject, 6, UW_material, UB_material);
            MiddleBoard = new Board(MiddleBoardGameObject, 7, MW_material, MB_material);
            LowerBoard = new Board(LowerBoardGameObject, 8, LW_material, LB_material);
            

            List<Board> boards = new List<Board> { LowerBoard, MiddleBoard, UpperBoard };
            int b = 0;
            foreach (Board currentBoard in boards)
            {
                for (int r = 0; r < Board.height; r++)
                {
                    for (int c = 0; c < Board.width; c++)
                    {
                        // Set material colors based on which layer we're instantiating
                        Material mat;
                        if ((r + c) % 2 == 0)
                            mat = currentBoard.upper_mat;
                        else
                            mat = currentBoard.lower_mat;

                        currentBoard.AddSquareAt(mat, b, r, c);
                    }
                }
                b++;
            }

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
                    m = white_pieces_mat;
                }
                else
                {
                    c = Color.Black;
                    m = black_pieces_mat;
                }

                piece = piecePrefabs[piece_type];
                CurrentBoard.AddPieceAt(piece, m, c, row, col);
                CurrentBoard.squares[row, col].occupied = true;
                //print("row: " + script.location.row);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            // Check where we clicked
            if (Physics.Raycast(ray, out hit))
            {
                // Locate GameObject of whatever we clicked on (piece or square)
                GameObject hitGameObject = hit.transform.gameObject;
                mController.DisplayMoves(hitGameObject);
            }
        }
    }
}
