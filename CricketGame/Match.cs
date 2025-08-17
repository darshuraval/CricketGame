using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketGame
{
	internal class Match
	{
		public List<Player> Team1 { get; set; }
		public List<Player> Team2 { get; set; }
		public TeamName TossWinner { get; set; }
		public TeamName Batting { get; set; }
		public TeamName Bowling { get; set; }
		public Queue<Player> BattingOrder { get; set; } = new Queue<Player>();
		public Queue<Player> BowlingOrder { get; set; } = new Queue<Player>();
		public Player currentBatman { get; set; }
		public Player secondBatman { get; set; }
		public Player currentBowler { get; set; }
		public TeamName WinningTeam { get; set; }
		public TeamName LosingTeam { get; set; }
		public List<Innings> InningsList { get; set; } = new List<Innings>();


		public Match(List<Player> team1, List<Player> team2)
		{
			if (team1 == null || team2 == null)
				throw new ArgumentNullException("Teams cannot be null");
			if(team1.Count != Rules.PlayerLimit || team2.Count != Rules.PlayerLimit)
				throw new ArgumentException("Each team must have exactly " + Rules.PlayerLimit + " players.");
			if (team1.Any(p => p.TeamName != team1[0].TeamName) || team2.Any(p => p.TeamName != team2[0].TeamName))
				throw new ArgumentException("All players in a team must belong to the same team.");
			if (team1.GetTeamName() == team2.GetTeamName())
				throw new ArgumentException("Teams cannot be the same.");
			if (team1.Count != team2.Count)
				throw new ArgumentException("Both teams must have the same number of players.");
			
			this.Team1 = team1;
			this.Team2 = team2;
			
			Toss();
			SetQueue();
		}

		private void SetQueue()
		{
			if(Batting == Team1.GetTeamName())
			{
				foreach (var player in Team1)
				{
					BattingOrder.Enqueue(player);
				}
				foreach (var player in Team2)
				{
					if (player.Type == PlayerType.Bowler || player.Type == PlayerType.AllRounder)
					{
						BowlingOrder.Enqueue(player);
					}
				}
				currentBatman = BattingOrder.Dequeue();
				secondBatman = BattingOrder.Dequeue();
				currentBowler = BowlingOrder.Dequeue();
				BowlingOrder.Enqueue(currentBowler); // Re-queue the bowler for next overs
			}
			else
			{
				foreach (var player in Team2)
				{
					BattingOrder.Enqueue(player);
				}
				foreach (var player in Team1)
				{
					if (player.Type == PlayerType.Bowler || player.Type == PlayerType.AllRounder)
					{
						BowlingOrder.Enqueue(player);
					}
				}
				currentBatman = BattingOrder.Dequeue();
				secondBatman = BattingOrder.Dequeue();
				currentBowler = BowlingOrder.Dequeue();
				BowlingOrder.Enqueue(currentBowler); // Re-queue the bowler for next overs
			}
		}

		void Toss()
		{
			int t = new Random().Next(1, 100);
			if (t % 2 == 0)
			{
				TossWinner = Team1.GetTeamName();
				Console.WriteLine("Team Won : " + Team1.GetTeamName());
				Console.Write("Enter Choice 1. Batting, 2. Bowling: ");
				if (Convert.ToInt16(Console.ReadLine()) == 1)
				{

					Batting = Team1.GetTeamName();
					Bowling = Team2.GetTeamName();
				}
				else
				{
					Batting = Team2.GetTeamName();
					Bowling = Team1.GetTeamName();
				}
			}
			else
			{
				TossWinner = Team2.GetTeamName();
				Console.WriteLine("Team Won : " + Team2);
				Console.Write("Enter Choice 1. Batting, 2. Bowling: ");
				if (Convert.ToInt16(Console.ReadLine()) == 1)
				{
					Batting = Team2.GetTeamName();
					Bowling = Team1.GetTeamName();
				}
				else
				{
					Batting = Team1.GetTeamName();
					Bowling = Team2.GetTeamName();
				}
			}
			Console.WriteLine("Batting : " + Batting);
			Console.WriteLine("Bowling : " + Bowling);
		}

		internal void Play()
		{
			for (int i = 0; i < 2; i++)
			{
				Innings inning = new Innings
				{
					BattingTeam = Batting,
					BowlingTeam = Bowling
				};
				for (int over = 1; over <= Rules.OverLimit; over++)
				{
					Over o = new Over { OverNumber = over };
					for (int ball = 1; ball <= Rules.BallLimit; ball++)
					{
						Ball b = new Ball();
						Console.WriteLine($"Over {over}, Ball {ball}");
						Console.Write("Enter Runs (0-7) or -1, -2 for Wicket or 5, 7 for No Ball : ");
						int runs = Convert.ToInt32(Console.ReadLine());
						if (runs < Rules.MinRunPerBall || runs > Rules.MaxRunPerBall)
						{
							Console.WriteLine($"Invalid input! Runs must be between {Rules.MinRunPerBall} and {Rules.MaxRunPerBall}.");
							ball--; // Repeat the ball
							continue;
						}
						b.Batman = currentBatman;
						b.Bowler = currentBowler;
						currentBowler.BowlingStats.BallsBowled++;
						currentBatman.BattingStats.BallsFaced++;

						b.Remark = $"Over {over}, Ball {ball}";
						if (runs == -1 || runs == -2)
						{
							b.isWicket = true;
							inning.Wickets++;
							currentBowler.BowlingStats.Wickets++;
							Console.WriteLine($"{currentBatman.Name} is out!");

							currentBatman = secondBatman; // Swap batsmen
							if (BattingOrder.Count > 0)
								secondBatman = BattingOrder.Dequeue();
						}
						else if (runs >= 0 && runs <= 7)
						{
							if (runs == 5 || runs == 7)
							{
								ball--;
								b.isExtra = true;
								inning.Extra++;
								Console.WriteLine("No ball! Extra +1 run.");
							}
							if(runs == 1 || runs == 3)
							{
								SwapBatman();
							}
							b.Runs = runs;
							inning.TotalRuns += runs;
							Console.WriteLine($"{currentBatman.Name} scored {runs} runs.");
						}
						else
						{
							Console.WriteLine("Invalid input, try again.");
							b.Runs = 0; // No runs for invalid input
						}
						o.Balls.Add(b);
						ScoreBoard(inning);
					}
					inning.Overs.Add(o);
				}
				InningsList.Add(inning);
			}
		}

		private void SwapBatman()
		{
			//swap batsmen
			Player temp = currentBatman;
			currentBatman = secondBatman;
			secondBatman = temp;
		}

		public void ScoreBoard(Innings inning)
		{
			Console.WriteLine("\n================== SCOREBOARD ==================");
			Console.WriteLine($"Batting: {inning.BattingTeam} | Bowling: {inning.BowlingTeam}");
			Console.WriteLine($"Total Runs: {inning.TotalRuns} | Wickets: {inning.Wickets} | Extras: {inning.Extra}");
			Console.WriteLine("------------------------------------------------");

			// 🏏 Batsman Stats
			Console.WriteLine("Batsmen:");
			var batsmen = inning.Overs
				.SelectMany(o => o.Balls)
				.Where(b => b.Batman != null)
				.GroupBy(b => b.Batman)
				.Select(g => new
				{
					Player = g.Key,
					Runs = g.Sum(x => x.Runs),
					Balls = g.Count(x => !x.isExtra),
					Outs = g.Any(x => x.isWicket)
				});

			foreach (var batsman in batsmen)
			{
				string status = batsman.Outs ? "OUT" : "NOT OUT";
				Console.WriteLine($"{batsman.Player.Name,-15} {batsman.Runs} ({batsman.Balls} balls) [{status}]");
			}

			Console.WriteLine("------------------------------------------------");

			// 🎯 Bowler Stats
			Console.WriteLine("Bowlers:");
			var bowlers = inning.Overs
				.SelectMany(o => o.Balls)
				.Where(b => b.Bowler != null)
				.GroupBy(b => b.Bowler)
				.Select(g => new
				{
					Player = g.Key,
					RunsGiven = g.Sum(x => x.Runs),
					Balls = g.Count(),
					Wickets = g.Count(x => x.isWicket),
					Extras = g.Count(x => x.isExtra)
				});

			foreach (var bowler in bowlers)
			{
				double overs = bowler.Balls / 6 + (bowler.Balls % 6) / 10.0;
				Console.WriteLine($"{bowler.Player.Name,-15} {overs} overs | Runs: {bowler.RunsGiven} | Wkts: {bowler.Wickets} | Extras: {bowler.Extras}");
			}

			Console.WriteLine("================================================\n");
		}

	}
	class Ball
	{
		public int Runs { get; set; }
		public bool isExtra { get; set; }
		public bool isWicket { get; set; }
		public Player Batman { get; set; }
		public Player Bowler { get; set; }
		public string Remark { get; set; }
	}
	class Over
	{
		public int OverNumber { get; set; }
		public List<Ball> Balls { get; set; } = new List<Ball>();
	}

	class Innings
	{
		public TeamName BattingTeam { get; set; }
		public TeamName BowlingTeam { get; set; }
		public List<Over> Overs { get; set; } = new List<Over>();
		public int TotalRuns { get; set; }
		public int Wickets { get; set; }
		public int Extra { get; set; }
	}
}
