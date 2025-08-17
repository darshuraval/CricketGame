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
			BattingOrder.Clear();
			BowlingOrder.Clear();

			if (Batting == Team1.GetTeamName())
			{
				foreach (var player in Team1)
					BattingOrder.Enqueue(player);

				foreach (var player in Team2) // Ensure all bowlers are enqueued
				{
					if (player.Type == PlayerType.Bowler || player.Type == PlayerType.AllRounder)
						BowlingOrder.Enqueue(player);
				}
			}
			else
			{
				foreach (var player in Team2)
					BattingOrder.Enqueue(player);

				foreach (var player in Team1)
				{
					if (player.Type == PlayerType.Bowler || player.Type == PlayerType.AllRounder)
						BowlingOrder.Enqueue(player);
				}
			}

			// If no "Bowler" or "AllRounder" is found, allow batsmen to bowl
			if (BowlingOrder.Count == 0)
			{
				foreach (var player in (Bowling == Team1.GetTeamName() ? Team1 : Team2))
					BowlingOrder.Enqueue(player);
			}

			currentBatman = BattingOrder.Dequeue();
			secondBatman = BattingOrder.Dequeue();

			currentBowler = BowlingOrder.Dequeue();
			BowlingOrder.Enqueue(currentBowler);
		}


		void Toss()
		{
			int t = new Random().Next(1, 100);
			if (t % 2 == 0)
			{
				TossWinner = Team1.GetTeamName();
				Console.WriteLine("Team Won : " + Team1.GetTeamName());
				Console.Write("Enter Choice 1. Batting, 2. Bowling: ");
				int choice = 1;
				if (choice == 1)
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
				//int choice = Convert.ToInt16(Console.ReadLine());
				int choice = 1; // Simulating user input for testing
				if (choice == 1)
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
						int runs = new Random().Next(-2, 7);
						Console.ReadKey();
						//int runs = Convert.ToInt32(Console.ReadLine());
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
							currentBatman.BattingStats.IsOut = true;
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
								currentBowler.BowlingStats.RunsConceded++; // add extra to bowler
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
						currentBatman.BattingStats.Runs = currentBatman.BattingStats.Runs + b.Runs;
						ScoreBoard(inning);
					}
					inning.Overs.Add(o);
					currentBowler = BowlingOrder.Dequeue();
					BowlingOrder.Enqueue(currentBowler);
				}
				InningsList.Add(inning);
				// Swap teams for next innings

				TeamName prevBatting = Batting;
				Batting = Bowling;
				Bowling = prevBatting;
				SetQueue();

				Console.WriteLine($"Innings {i + 1} completed. Switching teams for next innings.");
					Console.WriteLine($"Batting: {Batting}, Bowling: {Bowling}");
					Console.WriteLine("Press any key to continue to next innings...");
					Console.ReadKey();
			}

			//set Winning and Losing Team
			if (InningsList[0].TotalRuns > InningsList[1].TotalRuns)
			{
				WinningTeam = InningsList[0].BattingTeam;
				LosingTeam = InningsList[1].BattingTeam;
			}
			else if (InningsList[0].TotalRuns < InningsList[1].TotalRuns)
			{
				WinningTeam = InningsList[1].BattingTeam;
				LosingTeam = InningsList[0].BattingTeam;
			}
			else
			{
				WinningTeam = TeamName.None; // Draw
				LosingTeam = TeamName.None;
			}
		}

		private void SwapBatman()
		{
			//swap batsmen
			Player temp = currentBatman;
			currentBatman = secondBatman;
			secondBatman = temp;
		}
		public void FinalScoreBoard()
		{
			foreach (var inning in InningsList)
			{
				ScoreBoard(inning, 0);
			}
		}
		public void ScoreBoard(Innings inning, int clear = 1)
		{
			if(clear == 1) Console.Clear();
			Console.WriteLine("====================================");
			Console.WriteLine($"Scoreboard - {inning.BattingTeam}");
			Console.WriteLine("====================================");

			// Team total
			int legalBalls = inning.Overs.SelectMany(o => o.Balls).Count(b => !b.isExtra);
			double oversAsDecimal = legalBalls / 6 + (legalBalls % 6) / 10.0;

			Console.WriteLine($"Total: {inning.TotalRuns}/{inning.Wickets}  ({oversAsDecimal:0.0} overs)");
			Console.WriteLine($"Extras: {inning.Extra}");
			Console.WriteLine("------------------------------------");

			// Batting lineup
			Console.WriteLine("BATTING:");
			foreach (var player in (inning.BattingTeam == Team1.GetTeamName() ? Team1 : Team2))
			{
				string status = player.BattingStats.IsOut ? "OUT" : "NOT OUT";
				string tempName = player.Name;
				if (currentBatman == player || currentBowler == player)
				{
					tempName += " *";
				}
				if (secondBatman == player)
				{
					tempName += " **";
				}
				Console.WriteLine($"{tempName,-12} {player.BattingStats.Runs} ({player.BattingStats.BallsFaced}) {status}");
			}

			Console.WriteLine("------------------------------------");

			// Bowling lineup
			Console.WriteLine("BOWLING:");
			foreach (var player in (inning.BowlingTeam == Team1.GetTeamName() ? Team1 : Team2))
			{
				if (player.BowlingStats.BallsBowled > 0)
				{
					double bowlerOvers = player.BowlingStats.BallsBowled / 6 +
										 (player.BowlingStats.BallsBowled % 6) / 10.0;
					Console.WriteLine($"{player.Name,-12} {bowlerOvers:0.0} overs, {player.BowlingStats.RunsConceded} runs, {player.BowlingStats.Wickets} wkts");
				}
			}

			Console.WriteLine("====================================");
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
