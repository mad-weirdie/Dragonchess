using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dragonchess
{
    using static GameUI;
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
            Board UpperBoard = GameController.state.boards[3];
            Board MiddleBoard = GameController.state.boards[2];
            Board LowerBoard = GameController.state.boards[1];

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

                    AddClickyAt(3, Upper, pos, r, c);
                    AddClickyAt(2, Middle, pos, r, c);
                    AddClickyAt(1, Lower, pos, r, c);
                }
            }
            
        }

        public void AddClickyAt(int board, GameObject boardObj, Vector3 pos, int row, int col)
        {
            GameObject newButton = Instantiate(clickySquarePrefab, boardObj.transform, false);
            newButton.GetComponent<RectTransform>().anchoredPosition = pos;
            newButton.GetComponent<ClickySquare>().square = GameUI.GetSquareAt(board, row, col);
            newButton.GetComponent<ClickySquare>().board = board;
        }
    }
}
