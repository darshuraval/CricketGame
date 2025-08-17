using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CricketGame
{
	internal class Function
	{
		Players p = new Players();
		Match match1;
		Match match2;
		Match match3;

		public Function()
		{
			SetPlayer();
			//PrintAllPlayer();
			StartMatchAsync();
		}
		public async Task StartMatchAsync()
		{
			List<Player> team1 = p.GetTeam(TeamName.Mumbai);
			List<Player> team2 = p.GetTeam(TeamName.Chennai);
			List<Player> team3 = p.GetTeam(TeamName.Gujarat);
			
			match1 = new Match(team1, team2);
			match1.Play();

			//match2 = new Match(p.GetTeam(match1.LosingTeam), team3);
			//match2.Play();

			//match3 = new Match(p.GetTeam(match1.WinningTeam), p.GetTeam(match2.WinningTeam));
			//match3.Play();

			Console.WriteLine("Tournament Finished!");
		}
		public void SetPlayer()
		{
			p.AddPlayer("Darshan", PlayerType.Batmen, TeamName.Mumbai);
			p.AddPlayer("Meet", PlayerType.Batmen, TeamName.Mumbai);
			p.AddPlayer("Philip", PlayerType.Bowler, TeamName.Mumbai);
			p.AddPlayer("Aangi", PlayerType.Bowler, TeamName.Mumbai);
			p.AddPlayer("Akshar", PlayerType.AllRounder, TeamName.Mumbai);

			p.AddPlayer("Raj", PlayerType.Batmen, TeamName.Chennai);
			p.AddPlayer("Mohit", PlayerType.Batmen, TeamName.Chennai);
			p.AddPlayer("Ramesh", PlayerType.Bowler, TeamName.Chennai);
			p.AddPlayer("Nirav", PlayerType.Bowler, TeamName.Chennai);
			p.AddPlayer("Devangi", PlayerType.AllRounder, TeamName.Chennai);

			p.AddPlayer("Prakash", PlayerType.Batmen, TeamName.Gujarat);
			p.AddPlayer("Karan", PlayerType.Batmen, TeamName.Gujarat);
			p.AddPlayer("Kishan", PlayerType.Bowler, TeamName.Gujarat);
			p.AddPlayer("Ronak", PlayerType.Bowler, TeamName.Gujarat);
			p.AddPlayer("Ekta", PlayerType.AllRounder, TeamName.Gujarat);

			Console.WriteLine("Team Player Assigned...\n");
		}
		public void PrintAllPlayer()
		{
			foreach (var item in p)
			{
				Console.WriteLine(item.Name + ", Team Name : " + item.TeamName);
			}

			List<Player> t1 = p.GetTeam(TeamName.Mumbai);
			List<Player> t2 = p.GetTeam(TeamName.Chennai);
			List<Player> t3 = p.GetTeam(TeamName.Gujarat);

			Console.WriteLine("\n\nTeam 1 : " + TeamName.Mumbai);
			foreach (var item in t1)
			{
				Console.WriteLine(item.Name + ", Team Name : " + item.TeamName);
			}
			Console.WriteLine("\n\nTeam 2 : " + TeamName.Chennai);
			foreach (var item in t2)
			{
				Console.WriteLine(item.Name + ", Team Name : " + item.TeamName);
			}
			Console.WriteLine("\n\nTeam 3 : " + TeamName.Gujarat);
			foreach (var item in t3)
			{
				Console.WriteLine(item.Name + ", Team Name : " + item.TeamName);
			}
		}
	}
}