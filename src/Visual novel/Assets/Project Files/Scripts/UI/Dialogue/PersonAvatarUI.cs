#region

using Units.Tools;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace UI.Dialogue
{
	public class PersonAvatarUI : MonoBehaviour
	{
		[SerializeField] private GameObject _avatarGO;
		[SerializeField] private Image _avatarImage;

		public void SetActionAvatar(bool value)
		{
			_avatarGO.SetActive(value);
		}

		public void SetAvatar(Texture2D texture2D)
		{
			_avatarImage.sprite = texture2D.ToSprite();
		}
	}
}