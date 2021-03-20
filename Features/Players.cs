﻿using System.IO;
using EFT.Trainer.Extensions;
using UnityEngine;

namespace EFT.Trainer.Features
{
	public class Players : MonoBehaviour, IEnableable
	{
		public static readonly Color PlayerColor = Color.blue;
		public static readonly Color BotColor = Color.yellow;
		public static readonly Color BossColor = Color.red;

		public bool Enabled { get; set; } = true;

		private Shader _outline;

		private void Awake()
		{
			var bundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "outline"));
			_outline = bundle.LoadAsset<Shader>("assets/outline.shader");
		}

		private void Update()
		{
			if (!Enabled)
				return;

			var hostiles = GameState.Current?.Hostiles;
			if (hostiles == null)
				return;

			foreach (var ennemy in hostiles)
			{
				if (!ennemy.IsValid())
					continue;

				var color = GetPlayerColor(ennemy);
				SetShaders(ennemy, _outline, color);
			}
		}

		private static Color GetPlayerColor(Player player)
		{
			// we can use null propagation here because Profile/Info do not derive from UnityObject
			var settings = player.Profile?.Info?.Settings;
			if (settings != null && settings.IsBoss())
				return BossColor;

			return player.IsAI ? BotColor : PlayerColor;
		}

		private static void SetShaders(Player player, Shader shader, Color color)
		{
			var skins = player.PlayerBody.BodySkins;
			foreach (var skin in skins.Values)
			{
				if (skin == null)
					continue;

				foreach (var renderer in skin.GetRenderers())
				{
					if (renderer == null)
						continue;

					var material = renderer.material;
					if (material == null)
						continue;

					material.shader = shader;

					material.SetColor("_FirstOutlineColor", Color.red);
					material.SetFloat("_FirstOutlineWidth", 0.02f);
					material.SetColor("_SecondOutlineColor", color);
					material.SetFloat("_SecondOutlineWidth", 0.0025f);
				}
			}
		}
	}
}
