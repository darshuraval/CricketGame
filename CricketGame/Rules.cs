using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketGame
{
	internal class Rules
	{
		public static int BallLimit { get; set; } = 6;
		public static int OverLimit { get; set; } = 2; // change to 5
		public static int MinRunPerBall { get; set; } = -2;
		public static int MaxRunPerBall { get; set; } = 7;
		public static int PlayerLimit { get; set; } = 5;
		public static int BatmanLimit { get; set; } = 2;
		public static int BowlerLimit { get; set; } = 2;
		public static int AllRounderLimit { get; set; } = 1;
	}
}
