using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess {

	public class MoveTypeDict : MonoBehaviour
	{
		public readonly static Dictionary<string, int> MoveTypes;

		static MoveTypeDict()
		{
			MoveTypes = new Dictionary<string, int>();

			MoveTypes["TopWSylphTake"] = 0;
			MoveTypes["TopBSylphTake"] = 0;
			MoveTypes["TopWSylph"] = 0;
			MoveTypes["TopBSylph"] = 0;
			MoveTypes["MidWSylph"] = 0;
			MoveTypes["MidBSylph"] = 0;

			MoveTypes["TopGrif"] = 1;
			MoveTypes["MidGrif"] = 1;

			MoveTypes["Dragon"] = 2;
			MoveTypes["SwoopDragon"] = 2;

			MoveTypes["WWarrior"] = 3;
			MoveTypes["BWarrior"] = 3;
			MoveTypes["WWarriorTake"] = 3;
			MoveTypes["BWarriorTake"] = 3;

			MoveTypes["Oliphant"] = 4;

			MoveTypes["Unicorn"] = 5;

			MoveTypes["TopHero"] = 6;
			MoveTypes["MidHero"] = 6;
			MoveTypes["BotHero"] = 6;

			MoveTypes["Thief"] = 7;

			MoveTypes["TopCleric"] = 8;
			MoveTypes["MidCleric"] = 8;
			MoveTypes["BotCleric"] = 8;

			MoveTypes["TopMage"] = 9;
			MoveTypes["MidMage"] = 9;
			MoveTypes["BotMage"] = 9;

			MoveTypes["TopKing"] = 10;
			MoveTypes["MidKing"] = 10;
			MoveTypes["BotKing"] = 10;

			MoveTypes["TopPaladin"] = 11;
			MoveTypes["MidPaladin"] = 11;
			MoveTypes["BotPaladin"] = 11;

			MoveTypes["BotWDwarf"] = 12;
			MoveTypes["BotWDwarfTake"] = 12;
			MoveTypes["BotBDwarf"] = 12;
			MoveTypes["BotBDwarfTake"] = 12;
			MoveTypes["MidWDwarf"] = 12;
			MoveTypes["MidWDwarfTake"] = 12;
			MoveTypes["MidBDwarf"] = 12;
			MoveTypes["MidBDwarfTake"] = 12;

			MoveTypes["WBasilisk"] = 13;
			MoveTypes["BBasilisk"] = 13;
			MoveTypes["WBackwardsBasilisk"] = 13;
			MoveTypes["BBackwardsBasilisk"] = 13;

			MoveTypes["BotElemental"] = 14;
			MoveTypes["BotElementalSlide"] = 14;
			MoveTypes["MidElemental"] = 14;
		}
	}
}
