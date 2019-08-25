using Sitecore.Commerce.Core;
using System;

namespace Ajsuth.Foundation.Minions.Engine.Models
{
    public class MinionRunModel : MinionRunResultsModel
	{
		public MinionRunModel() : base()
		{
			LastStartTime = DateTime.UtcNow;
			Status = "In Progress";
		}
		
		public DateTime LastStartTime { get; set; }

		public DateTime? CompletedTime { get; set; }

		public string Status { get; set; }

		public string Message { get; set; }

		public void Complete(string message, int itemsProcessed)
		{
			ItemsProcessed = itemsProcessed;
			Complete(message);
		}

		public void Complete(string message)
		{
			DidRun = true;
			Finalise("Complete", message);
		}

		public void Abort(string message)
		{
			Finalise("Aborted", message);
		}

		public void Error(string message, bool hasMoreItems, bool didRun = false)
		{
			HasMoreItems = hasMoreItems;
			DidRun = didRun;
			Finalise("Error", message);
		}

		public void Finalise(string status, string message)
		{
			Status = status;
			Message = message;
			CompletedTime = DateTime.UtcNow;
		}
	}
}
