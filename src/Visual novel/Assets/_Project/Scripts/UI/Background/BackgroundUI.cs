﻿#region

using Units.Tools;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace UI.Background
{
	public class BackgroundUI : MonoBehaviour
	{
		[SerializeField] private Image _backgroundImage;

		[SerializeField] private GameObject _backgroundScreenGameObject;

		public void SetBackgroundImage(Texture2D texture2D)
		{
			_backgroundImage.color = Color.white;
			_backgroundImage.sprite = texture2D.ToSprite();
		}

		public void SetBackgroundColor(Color color)
		{
			_backgroundImage.color = color;
			_backgroundImage.sprite = null;
		}

		public void SetActivePanel(bool value)
		{
			_backgroundScreenGameObject.SetActive(value);
		}
	}
}