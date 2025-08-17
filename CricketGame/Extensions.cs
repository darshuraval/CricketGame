using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketGame
{
	internal static class Extensions
	{
		public static TeamName GetTeamName(this List<Player> players)
		{
			if (players == null || players.Count == 0)
				return 0;
			return players[0].TeamName;
		}
	}
}
