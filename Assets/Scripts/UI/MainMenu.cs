using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Dragonchess
{
	public class MainMenu : MonoBehaviour
	{
		public TMP_Dropdown P1;
		public TMP_Dropdown P2;
		public void StartGame()
		{
			int p1 = P1.value;
			int p2 = P2.value;
			/*
			if (p1 == 0)
				GameController.P1_type = PlayerType.Human;
			else {
				GameController.P1_type = PlayerType.AI;
				GameController.AI_1 = (AIDifficulty)(p1 - 1);
			}

			if (p2 == 0)
				GameController.P2_type = PlayerType.Human;
			else
			{
				GameController.P2_type = PlayerType.AI;
				GameController.AI_2 = (AIDifficulty)(p2 - 1);
			}*/

			//GameController.TestsEnabled = false;
			//GameController.GameFromFileEnabled = false;
			SceneManager.LoadScene("Game", LoadSceneMode.Single);
		}
	}
}
