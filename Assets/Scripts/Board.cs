using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Board : MonoBehaviour
    {
        public GameObject Sylph;
        public GameObject Griffon;
        public GameObject Dragon;
        public GameObject Warrior;
        public GameObject Oliphant;
        public GameObject Unicorn;
        public GameObject Hero;
        public GameObject Thief;
        public GameObject Cleric;
        public GameObject Mage;
        public GameObject King;
        public GameObject Paladin;
        public GameObject Dwarf;
        public GameObject Basilisk;
        public GameObject Elemental;

        public static int width = 12;
        public static int height = 8;

        public Material UW_material;
        public Material UB_material;
        public Material MW_material;
        public Material MB_material;
        public Material LW_material;
        public Material LB_material;
        public Material black_pieces_mat;
        public Material white_pieces_mat;

        public float square_scale;
        public float board_width;
        public float base_height;
        public float layer_spacing;

        string[] Letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };

        public GameObject UpperBoardGameObject;
        public GameObject MiddleBoardGameObject;
        public GameObject LowerBoardGameObject;

        Square[,] UpperBoard = new Square[height, width];
        Square[,] MiddleBoard = new Square[height, width];
        Square[,] LowerBoard = new Square[height, width];

        void AddPieceAt(GameObject piece, Material mat, int r, int c, Layer l)
        {

            Vector3 pos;
            if (l == Layer.Upper)
                pos = UpperBoard[r, c].cubeObject.transform.position;
            else if (l == Layer.Middle)
                pos = MiddleBoard[r, c].cubeObject.transform.position;
            else
                pos = LowerBoard[r, c].cubeObject.transform.position;

            pos.y += 1.0f/square_scale;
            Quaternion q = new Quaternion(0, 0, 0, 1);
            GameObject obj = GameObject.Instantiate(piece, pos, q);
            obj.transform.localScale = new Vector3(1/square_scale, 1/board_width, 1/square_scale);
            obj.GetComponent<Renderer>().material = mat;
        }

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
                    CurrentSquares = LowerBoard;
                }
                else if (b == 1)
                { 

                    CurrentBoard = MiddleBoardGameObject;
                    CurrentSquares = MiddleBoard;
                }
                else
                { 
                    CurrentBoard = UpperBoardGameObject;
                    CurrentSquares = UpperBoard;
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

                        Layer currentLayer;
                        // Set material colors based on which layer we're instantiating
                        if (b == 0)
                        {
                            currentLayer = Layer.Lower;
                            if ((r + c) % 2 == 0)
                                cube.GetComponent<Renderer>().material = LB_material;
                            else
                                cube.GetComponent<Renderer>().material = LW_material;
                        }
                        else if (b == 1)
                        {
                            currentLayer = Layer.Middle;
                            if ((r + c) % 2 == 0)
                                cube.GetComponent<Renderer>().material = MB_material;
                            else
                                cube.GetComponent<Renderer>().material = MW_material;
                        }
                        else
                        {
                            currentLayer = Layer.Upper;
                            if ((r + c) % 2 == 0)
                                cube.GetComponent<Renderer>().material = UB_material;
                            else
                                cube.GetComponent<Renderer>().material = UW_material;
                        }

                        
                        // Attach GameObject to the squares matrix
                        CurrentSquares[r, c] = new Square(r, c, currentLayer);
                        CurrentSquares[r, c].cubeObject = cube;
                    }
                }
            }


            // ------------ INSTANTIATE UPPER BOARD ------------
            // Sylphs
            for (int c = 0; c < Board.width; c+=2)
            {
                AddPieceAt(Sylph, white_pieces_mat, 1, c, Layer.Upper);
                AddPieceAt(Sylph, black_pieces_mat, 6, c, Layer.Upper);
            }

            // Griffons
            AddPieceAt(Griffon, white_pieces_mat, 0, 2, Layer.Upper);
            AddPieceAt(Griffon, white_pieces_mat, 0, 9, Layer.Upper);
            AddPieceAt(Griffon, black_pieces_mat, 7, 2, Layer.Upper);
            AddPieceAt(Griffon, black_pieces_mat, 7, 9, Layer.Upper);

            // Dragons
            AddPieceAt(Dragon, white_pieces_mat, 0, 5, Layer.Upper);
            AddPieceAt(Dragon, black_pieces_mat, 7, 5, Layer.Upper);


            // ------------ INSTANTIATE MIDDLE BOARD ------------
            // Warriors
            for (int c = 0; c < Board.width; c++)
            {
                AddPieceAt(Warrior, white_pieces_mat, 1, c, Layer.Middle);
                AddPieceAt(Warrior, black_pieces_mat, 6, c, Layer.Middle);
            }

            // Oliphants
            AddPieceAt(Oliphant, white_pieces_mat, 0, 0, Layer.Middle);
            AddPieceAt(Oliphant, black_pieces_mat, 7, 0, Layer.Middle);
            AddPieceAt(Oliphant, white_pieces_mat, 0, 11, Layer.Middle);
            AddPieceAt(Oliphant, black_pieces_mat, 7, 11, Layer.Middle);

            // Unicorns
            AddPieceAt(Unicorn, white_pieces_mat, 0, 1, Layer.Middle);
            AddPieceAt(Unicorn, black_pieces_mat, 7, 1, Layer.Middle);
            AddPieceAt(Unicorn, white_pieces_mat, 0, 10, Layer.Middle);
            AddPieceAt(Unicorn, black_pieces_mat, 7, 10, Layer.Middle);

            // Heroes
            AddPieceAt(Hero, white_pieces_mat, 0, 2, Layer.Middle);
            AddPieceAt(Hero, black_pieces_mat, 7, 2, Layer.Middle);
            AddPieceAt(Hero, white_pieces_mat, 0, 9, Layer.Middle);
            AddPieceAt(Hero, black_pieces_mat, 7, 9, Layer.Middle);

            // Thieves
            AddPieceAt(Thief, white_pieces_mat, 0, 3, Layer.Middle);
            AddPieceAt(Thief, black_pieces_mat, 7, 3, Layer.Middle);
            AddPieceAt(Thief, white_pieces_mat, 0, 8, Layer.Middle);
            AddPieceAt(Thief, black_pieces_mat, 7, 8, Layer.Middle);

            // Clerics
            AddPieceAt(Cleric, white_pieces_mat, 0, 4, Layer.Middle);
            AddPieceAt(Cleric, black_pieces_mat, 7, 4, Layer.Middle);

            // Mages
            AddPieceAt(Mage, white_pieces_mat, 0, 5, Layer.Middle);
            AddPieceAt(Mage, black_pieces_mat, 7, 5, Layer.Middle);

            // Kings
            AddPieceAt(King, white_pieces_mat, 0, 6, Layer.Middle);
            AddPieceAt(King, black_pieces_mat, 7, 6, Layer.Middle);

            // Paladins
            AddPieceAt(Paladin, white_pieces_mat, 0, 7, Layer.Middle);
            AddPieceAt(Paladin, black_pieces_mat, 7, 7, Layer.Middle);

            // ------------ INSTANTIATE LOWER BOARD ------------
            // Dwarves
            for (int c = 1; c < Board.width; c+=2)
            {
                AddPieceAt(Dwarf, white_pieces_mat, 1, c, Layer.Lower);
                AddPieceAt(Dwarf, black_pieces_mat, 6, c, Layer.Lower);
            }

            // Basilisks
            AddPieceAt(Basilisk, white_pieces_mat, 0, 0, Layer.Lower);
            AddPieceAt(Basilisk, black_pieces_mat, 7, 0, Layer.Lower);
            AddPieceAt(Basilisk, white_pieces_mat, 0, 11, Layer.Lower);
            AddPieceAt(Basilisk, black_pieces_mat, 7, 11, Layer.Lower);

            // Elementals
            AddPieceAt(Elemental, white_pieces_mat, 0, 5, Layer.Lower);
            AddPieceAt(Elemental, black_pieces_mat, 7, 5, Layer.Lower);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
