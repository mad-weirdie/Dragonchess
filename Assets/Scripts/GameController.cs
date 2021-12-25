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
        string[] Letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
        public TextAsset board_init;

        GameObject selectedPiece;
        List<Square> hightlightedSquares = new List<Square>();

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
        public Material highlightMaterial;

        static Board UpperBoard = new Board(6);
        static Board MiddleBoard = new Board(7);
        static Board LowerBoard = new Board(8);

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
            for (int b = 0; b < 3; b++)
            {
                // Parent GameObject where all the squares of a board are stored
                GameObject CurrentBoard;
                Square[,] CurrentSquares;
                if (b == 0)
                {
                    CurrentBoard = LowerBoardGameObject;
                    CurrentSquares = LowerBoard.squares;
                }
                else if (b == 1)
                {

                    CurrentBoard = MiddleBoardGameObject;
                    CurrentSquares = MiddleBoard.squares;
                }
                else
                {
                    CurrentBoard = UpperBoardGameObject;
                    CurrentSquares = UpperBoard.squares;
                }

                // Allows for easier, scriptable editing of the boards, should one so desire
                for (int r = 0; r < Board.height; r++)
                {
                    for (int c = 0; c < Board.width; c++)
                    {
                        // Maintains nice names for the squares, ie "A3", "E9", etc.
                        string name = Letters[c] + (r + 1).ToString();
                        GameObject cube = CurrentBoard.transform.Find(name).gameObject;

                        //cube.transform.localScale = new Vector3(square_scale, board_width, square_scale);
                        //cube.transform.position = new Vector3(c * square_scale, base_height + layer_spacing * b, r * square_scale);

                        Board currentBoard;
                        Material mat;
                        // Set material colors based on which layer we're instantiating
                        if (b == 0)
                        {
                            currentBoard = LowerBoard;
                            if ((r + c) % 2 == 0)
                                mat = LB_material;
                            else
                                mat = LW_material;
                        }
                        else if (b == 1)
                        {
                            currentBoard = MiddleBoard;
                            if ((r + c) % 2 == 0)
                                mat = MB_material;
                            else
                                mat = MW_material;
                        }
                        else
                        {
                            currentBoard = UpperBoard;
                            if ((r + c) % 2 == 0)
                                mat = UB_material;
                            else
                                mat = UW_material;
                        }
                        cube.GetComponent<Renderer>().material = mat;

                        // Attach GameObject to the squares matrix
                        CurrentSquares[r, c] = new Square(r, c, currentBoard);
                        CurrentSquares[r, c].cubeObject = cube;
                        CurrentSquares[r, c].properMaterial = mat;
                    }
                }
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
                Piece script = CurrentBoard.squares[row, col].cubeObject.GetComponent<Piece>();
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
            Piece piece;
            Square square;
            GameObject sObj;
            RaycastHit hit;
            int layer;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitGameObject = hit.transform.gameObject;
                if (selectedPiece != null)
                {
                    piece = selectedPiece.GetComponent<Piece>();
                    square = piece.location;
                    layer = hitGameObject.layer;

                    if (layer == 6 || layer == 7 || layer == 8)
                    {
                        foreach (Square s in hightlightedSquares)
                        {
                            if (s.cubeObject == hitGameObject)
                            {
                                Vector3 pos = s.cubeObject.transform.position;
                                pos.y += 1.0f / Board.square_scale;
                                piece.pieceGameObject.transform.position = pos;
                                sObj = piece.location.cubeObject;
                                sObj.GetComponent<Renderer>().material = square.properMaterial;
                                piece.location = s;

                                piece.pieceGameObject.layer = s.board.m_layer+3;
                                foreach (Transform child in piece.transform)
                                    child.gameObject.layer = s.board.m_layer + 3;
                            }
                        }
                    }
                    sObj = piece.location.cubeObject;
                    sObj.GetComponent<Renderer>().material = square.properMaterial;

                    foreach (Square s in hightlightedSquares)
                    {
                        s.cubeObject.GetComponent<Renderer>().material = s.properMaterial;
                    }
                    hightlightedSquares.Clear();
                }

                layer = hitGameObject.layer;
                if (layer == 9 || layer == 10 || layer == 11)
                {

                    selectedPiece = hitGameObject;
                    piece = selectedPiece.GetComponent<Piece>();
                    sObj = piece.location.cubeObject;
                    sObj.GetComponent<Renderer>().material = highlightMaterial;
                    
                    // Generate list of possible moves
                    ArrayList possibleMoves = piece.GetMoves();

                    foreach (Move move in possibleMoves)
                    {
                        Square endSquare = move.end;
                        GameObject squareObj = endSquare.cubeObject;
                        hightlightedSquares.Add(endSquare);
                        if (move.type == Move.MoveType.Regular)
                            squareObj.GetComponent<Renderer>().material = LB_material;
                        else if (move.type == Move.MoveType.Capture)
                            squareObj.GetComponent<Renderer>().material = LW_material;
                        else
                            squareObj.GetComponent<Renderer>().material = highlightMaterial;
                        print("possible move: (" + endSquare.row + ", " + endSquare.col + ")");
                    }
                }

            }
        }

    }
}
