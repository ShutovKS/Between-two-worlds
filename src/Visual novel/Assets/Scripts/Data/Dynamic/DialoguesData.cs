#region

using System;
using UnityEngine;

#endregion

namespace Data.Dynamic
{
	[Serializable]
	public class DialoguesData
	{
		public string idLastDialogue;
		public string titleText;
		public byte[] backgroundBytes;
		public bool isDataExist;
		[NonSerialized] public Texture2D background;

		public void Serialize()
		{
			if (background != null)
			{
				backgroundBytes = background.EncodeToPNG();
			}
		}

		public void Deserialize()
		{
			if (backgroundBytes is { Length: > 0 })
			{
				var texture = new Texture2D(2, 2);
				texture.LoadImage(backgroundBytes);
				background = texture;
			}
		}
	}
}