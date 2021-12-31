using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dragonchess
{
    public class ClickySquareController : MonoBehaviour
    {
        public Piece pieceScript;
        public GameObject clickySquarePrefab;
        public GameObject Upper;
        public GameObject Middle;
        public GameObject Lower;

        GameObject[,] clickySquares = new GameObject[8, 12];

        // Start is called before the first frame update
        void Start()
        {
            Board UpperBoard = GameController.getUpperBoard();
            Board MiddleBoard = GameController.getMiddleBoard();
            Board LowerBoard = GameController.getLowerBoard();

            RectTransform upperR = Upper.GetComponent<RectTransform>();
            float xCoord, yCoord;
            float square_width = upperR.rect.width / 12.0f;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    xCoord = square_width * c;
                    yCoord = square_width * r;
                    Vector3 pos = new Vector2(xCoord, yCoord);

                    AddClickyAt(UpperBoard, Upper, pos, r, c);
                    AddClickyAt(MiddleBoard, Middle, pos, r, c);
                    AddClickyAt(LowerBoard, Lower, pos, r, c);
                }
            }
        }

        public void AddClickyAt(Board board, GameObject boardObj, Vector3 pos, int row, int col)
        {
            GameObject newButton = Instantiate(clickySquarePrefab, boardObj.transform, false);
            newButton.GetComponent<RectTransform>().anchoredPosition = pos;
            newButton.GetComponent<ClickySquare>().square = board.squares[row, col];
            newButton.GetComponent<ClickySquare>().board = board;
        }
    }
}
