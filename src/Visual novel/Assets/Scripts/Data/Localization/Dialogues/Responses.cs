#region

using System;

#endregion

namespace Data.Localization.Dialogues
{
	[Serializable]
	public class Responses : IPhrase
	{
		public Response[] ResponseList { get; set; }
		public string ID { get; set; }

		[Serializable]
		public class Response
		{
			public string AnswerText { get; set; }
			public string IDNextDialog { get; set; }
		}
	}
}