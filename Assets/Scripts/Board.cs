using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Board
    {
        public GameObject boardGameObject;
        public List<GameObject> piecePrefabs;

        public static int width = 12;
        public static int height = 8;
        public float square_scale = 2.0f;
        public float board_width = 1.0f;
        public float base_height = 5;
        public float layer_spacing = 6;
        public int m_layer;

        Square[,] m_squares;

        public Board(int l)
        {
            m_squares = new Square[height, width];
            m_layer = l;
        }

        public Square[,] squares
        {
            get
            {
                return m_squares;
            }
        }

        public void SetSquare(int row, int col, Square s)
        {
            m_squares[row, col] = s;
        }

        public Square GetSquare(int row, int col)
        {
            return m_squares[row, col];
        }

        public void AddPieceAt(GameObject piece, Material mat, int r, int c)
        {
            Vector3 pos = m_squares[r, c].cubeObject.transform.position;
            piece.layer = m_layer+3;
            foreach (Transform child in piece.transform)
                child.gameObject.layer = m_layer+3;

            pos.y += 1.0f / square_scale;
            Quaternion q = new Quaternion(0, 0, 0, 1);
            GameObject obj = GameObject.Instantiate(piece, pos, q);
            obj.transform.localScale = new Vector3(1 / square_scale, 1 / square_scale, 1 / square_scale);
            obj.GetComponent<Renderer>().material = mat;

            // Attach square location to new piece GameObject we've instantiated
            Piece pieceScript = obj.GetComponent<Piece>();
            pieceScript.location = m_squares[r, c];
            pieceScript.board = this;

            m_squares[r, c].occupied = true;
            m_squares[r, c].piece = pieceScript;

        }

    }
}
