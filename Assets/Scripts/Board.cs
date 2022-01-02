using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Board
    {
        string[] Letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
        public GameObject boardGameObject;
        public List<GameObject> piecePrefabs;

        public static int width = 12;
        public static int height = 8;
        public static float square_scale = 1.0f;
        public static float board_width = 1.0f;
        public static float base_height = 5;
        public static float layer_spacing = 6;

        public int m_layer;

        public Material upper_mat;
        public Material lower_mat;

        Square[,] m_squares;

        public Board(GameObject obj, int l, Material upper, Material lower)
        {
            m_squares = new Square[height, width];
            m_layer = l;
            upper_mat = upper;
            lower_mat = lower;
            boardGameObject = obj;
        }

        public Square[,] squares
        {
            get
            {
                return m_squares;
            }
        }

        public Square GetSquare(int row, int col)
        {
            return m_squares[row, col];
        }

        public void AddSquareAt(Material mat, Material inv, int b, int r, int c)
        {
            // Maintains nice names for the squares, ie "A3", "E9", etc.
            string name = Letters[c] + (r + 1).ToString();
            GameObject cube = this.boardGameObject.transform.Find(name).gameObject;
            cube.GetComponent<Renderer>().material = mat;

            // Attach GameObject to the squares matrix
            m_squares[r, c] = new Square(r, c, this);
            m_squares[r, c].cubeObject = cube;
            m_squares[r, c].properMaterial = mat;
            m_squares[r, c].invisible = inv;

            GameController.AddDotAt(m_squares[r, c], m_squares[r,c].cubeObject.layer);
        }

        public void AddPieceAt(GameObject piece, Material mat, Color color, int r, int c)
        {
            Vector3 pos = m_squares[r, c].cubeObject.transform.position;
            piece.layer = m_layer+3;
            foreach (Transform child in piece.transform)
                child.gameObject.layer = m_layer+3;

            pos.y += 1.0f / square_scale;
            Quaternion q = new Quaternion(0, 0, 0, 1);
            GameObject obj = GameObject.Instantiate(piece, pos, q);
            obj.transform.localScale = new Vector3(0.7f*square_scale, 0.7f*square_scale, 0.7f*square_scale);
            obj.GetComponent<Renderer>().material = mat;

            // Attach square pos to new piece GameObject we've instantiated
            Piece pieceScript = obj.GetComponent<Piece>();
            pieceScript.pos = m_squares[r, c];
            pieceScript.board = this;
            pieceScript.color = color;

            // Set the basilisk's freezy-square danger-zone!
            if (pieceScript.type == PieceType.Basilisk)
                obj.GetComponent<Basilisk>().danger_square = m_squares[r, c];

            m_squares[r, c].occupied = true;
            m_squares[r, c].piece = pieceScript;

            if (color == Color.White)
                GameController.P1.AddPiece(pieceScript);
            else
                GameController.P2.AddPiece(pieceScript);

        }

    }
}
