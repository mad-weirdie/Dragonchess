using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class CameraController : MonoBehaviour
    {
        public Camera main_camera;

        public Camera focus_upper;
        public Camera focus_middle;
        public Camera focus_lower;

        public Camera over_upper;
        public Camera over_middle;
        public Camera over_lower;

        int CurrentBoardNum;


        // Start is called before the first frame update
        void Start()
        {
            // 0 - main camera
            // 3 - upper board
            // 2 - middle board
            // 1 - lower board
            CurrentBoardNum = 0;
            main_camera.enabled = (true);
        }

        public void SetLayerMask(int boardNum)
        {
            int layerMask;

            // Lower layer mask
            if (boardNum == 1)
                layerMask = (1 << 8) | (1 << 11);
            // Middle layer mask
            else if (boardNum == 2)
                layerMask = (1 << 7) | (1 << 10);
            // Upper layer mask
            else if (boardNum == 3)
                layerMask = (1 << 6) | (1 << 9);
            // Show all layers
            else
                layerMask = ~0;

            GameController.layerMask = layerMask;
        }

        public Camera GetCamera(int boardNum)
        {
            if (boardNum == 0)
                return main_camera;
            else if (boardNum == 1)
                return focus_lower;
            else if (boardNum == 3)
                return focus_upper;
            else
                return focus_middle;
        }

        public void SetBoard(int boardNum)
        {
            print("board num: " + boardNum);
            for (int i = 0; i < 4; i++)
            {
                if (i == boardNum)
                    GetCamera(i).enabled = true;
                else
                    GetCamera(i).enabled = false;
            }
            SetLayerMask(boardNum);
        }

        public void BoardUp()
        {
            if (CurrentBoardNum == 0)
            {
                CurrentBoardNum = 3;
                SetBoard(CurrentBoardNum);
                return;
            }

            if (CurrentBoardNum < 3)
                CurrentBoardNum++;
            SetBoard(CurrentBoardNum);
        }

        public void BoardDown()
        {
            if (CurrentBoardNum == 0)
            {
                CurrentBoardNum = 1;
                SetBoard(CurrentBoardNum);
                return;
            }

            if (CurrentBoardNum > 0)
                CurrentBoardNum--;
            SetBoard(CurrentBoardNum);
        }

        public void ViewAll()
        {
            CurrentBoardNum = 0;
            SetBoard(0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
