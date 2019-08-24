using Sitecore.Commerce.Core;
using System;

namespace Ajsuth.Foundation.Minions.Engine.Models
{
    public class MinionRunModel : Model
	{
		public MinionRunModel()
		{
			LastStartTime = DateTime.UtcNow;
			ItemsProcessed = 0;
			Status = "In Progress";
		}
		
		public DateTime LastStartTime { get; set; }
		public int ItemsProcessed { get; set; }
		public DateTime? CompletedTime { get; set; }
		public string Status { get; set; }
		public string Message { get; set; }

		public void RunComplete(string status, string message)
		{
			Status = status;
			Message = message;
			CompletedTime = DateTime.UtcNow;
		}

		public void RunComplete(string status, int itemsProcessed, string message)
		{
			ItemsProcessed = itemsProcessed;
			RunComplete(status, message);
		}
	}
}
