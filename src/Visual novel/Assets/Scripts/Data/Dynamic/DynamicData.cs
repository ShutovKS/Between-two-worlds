#region

using System;

#endregion

namespace Data.Dynamic
{
	[Serializable]
	public class DynamicData
	{
		public DialoguesData[] dialogues = new DialoguesData[6];

		public void Serialize()
		{
			foreach (var dialoguesData in dialogues)
			{
				dialoguesData?.Serialize();
			}
		}

		public void Deserialize()
		{
			foreach (var dialoguesData in dialogues)
			{
				dialoguesData?.Deserialize();
			}
		}
	}
}