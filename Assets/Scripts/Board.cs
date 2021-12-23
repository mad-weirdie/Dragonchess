using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Board : MonoBehaviour
    {
        public GameObject Sylph;

        public static int width = 12;
        public static int height = 8;

        public Material UW_material;
        public Material UB_material;

        public Material MW_material;
        public Material MB_material;

        public Material LW_material;
        public Material LB_material;

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

        void Start()
        {
            for (int b = 0; b < 3; b++)
            {
                // Parent GameObject where all the squares of a board are stored
                GameObject CurrentBoard;
                if (b == 0)
                    CurrentBoard = LowerBoardGameObject;
                else if (b == 1)
                    CurrentBoard = MiddleBoardGameObject;
                else
                    CurrentBoard = UpperBoardGameObject;

                // Allows for easier, scriptable editing of the boards, should one so desire
                for (int r = 0; r < Board.height; r++)
                {
                    for (int c = 0; c < Board.width; c++)
                    {
                        // Maintains nice names for the squares, ie "A3", "E9", etc.
                        string name = Letters[c] + (r + 1).ToString();
                        GameObject cube = CurrentBoard.transform.Find(name).gameObject;
                        
                        cube.transform.localScale = new Vector3(square_scale, board_width, square_scale);
                        cube.transform.position = new Vector3(c * square_scale, base_height + layer_spacing * b, r * square_scale);

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
                        UpperBoard[r, c] = new Square(r, c, currentLayer);
                        UpperBoard[r, c].cubeObject = cube;
                    }
                }
            }

            // Instantiate Sylph GameObjects
            for (int c = 0; c < Board.width; c+=2)
            {
                Vector3 pos = UpperBoard[1, c].cubeObject.transform.position;
                pos.y += 1.0f;
                Quaternion q = new Quaternion(0, 0, 0, 1);
                GameObject sylph = GameObject.Instantiate(Sylph, pos, q);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
