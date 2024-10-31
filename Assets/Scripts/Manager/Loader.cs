using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
	private static scenes TargetScene;

	public enum scenes {
		FighterDungeon,
		KOTHDungeon,
		MainMenu,
		LoadingScreen,
		LobbyScene,
		CharacterSelectScene,
		InbetweenScene,
		WinScene
	}

	public static void Load(scenes targetScene) {
		Loader.TargetScene = targetScene;

		SceneManager.LoadScene(Loader.scenes.LoadingScreen.ToString());
	}

	public static void LoadNetwork(scenes targetScene) {
		NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
	}

	public static void LoaderCallback() {
		SceneManager.LoadScene(Loader.TargetScene.ToString());
	}
}
