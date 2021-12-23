using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Board : MonoBehaviour
    {
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

        Square[][] UpperBoard;
        Square[][] MiddleBoard;
        Square[][] LowerBoard;

        void Start()
        {
            for (int b = 0; b < 3; b++)
            {
                GameObject CurrentBoard;
                if (b == 0)
                    CurrentBoard = LowerBoardGameObject;
                else if (b == 1)
                    CurrentBoard = MiddleBoardGameObject;
                else
                    CurrentBoard = UpperBoardGameObject;

                for (int r = 0; r < Board.height; r++)
                {
                    for (int c = 0; c < Board.width; c++)
                    {
                        string name = Letters[c] + (r + 1).ToString();

                        GameObject cube = CurrentBoard.transform.Find(name).gameObject;
                        
                        cube.transform.localScale = new Vector3(square_scale, board_width, square_scale);
                        cube.transform.position = new Vector3(c * square_scale, base_height + layer_spacing * b, r * square_scale);

                        if (b == 0)
                        {
                            if ((r + c) % 2 == 0)
                                cube.GetComponent<Renderer>().material = LB_material;
                            else
                                cube.GetComponent<Renderer>().material = LW_material;
                        }
                        if (b == 1)
                        {
                            if ((r + c) % 2 == 0)
                                cube.GetComponent<Renderer>().material = MB_material;
                            else
                                cube.GetComponent<Renderer>().material = MW_material;
                        }
                        if (b == 2)
                        {
                            if ((r + c) % 2 == 0)
                                cube.GetComponent<Renderer>().material = UB_material;
                            else
                                cube.GetComponent<Renderer>().material = UW_material;
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
