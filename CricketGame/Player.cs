using System;
using System.Collections;
using System.Collections.Generic;

namespace CricketGame
{
	public class Player
	{
		static int nextId = 0;
		public int Id { get; set; }
		public string Name { get; set; }
		public PlayerType Type { get; set; }
		public TeamName TeamName { get; set; }
		public BattingStats BattingStats { get; set; } = new BattingStats();
		public BowlingStats BowlingStats { get; set; } = new BowlingStats();


		public Player(string Name, PlayerType Type, TeamName teamName)
		{
			this.Id = nextId++;
			this.Name = Name;
			this.Type = Type;
			this.TeamName = teamName;
		}
	}
	public enum PlayerType
	{
		None = 0,
		Batmen = 1,
		Bowler = 2,
		AllRounder = 3
	}
	public enum TeamName
	{
		None = 0,
		Mumbai = 1,
		Chennai = 2,
		Gujarat = 3
	}
	public class Players : IEnumerable<Player>
	{
		List<Player> list = new List<Player>();

		public void AddPlayer(string Name, PlayerType Type, TeamName TeamID)
		{
			list.Add(new Player(Name, Type, TeamID));
		}

		public List<Player> GetTeam(TeamName t)
		{
			List<Player> lst = new List<Player>();
			foreach (var p in list)
			{
				if (p.TeamName == t)
				{
					lst.Add(p);
				}
			}
			return lst;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Player> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		internal List<Player> GetList()
		{
			return list;
		}
	}
	public class BattingStats
	{
		public int Runs { get; set; } = 0;
		public int BallsFaced { get; set; } = 0;
		public int Fours { get; set; } = 0;
		public int Sixes { get; set; } = 0;

		public double StrikeRate => BallsFaced > 0 ? (Runs * 100.0 / BallsFaced) : 0.0;

		public bool IsOut { get; internal set; }
	}

	public class BowlingStats
	{
		public int BallsBowled { get; set; } = 0;
		public int RunsConceded { get; set; } = 0;
		public int Wickets { get; set; } = 0;

		public double Economy => BallsBowled > 0 ? (RunsConceded * 6.0 / BallsBowled) : 0.0;
	}
}