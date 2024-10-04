using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
	private static scenes TargetScene;

	public enum scenes {
		GameScene,
		MainMenu,
		LoadingScreen
	}

	public static void Load(scenes targetScene) {
		Loader.TargetScene = targetScene;

		SceneManager.LoadScene(Loader.scenes.LoadingScreen.ToString());
	}

	public static void LoaderCallback() {
		SceneManager.LoadScene(Loader.TargetScene.ToString());
	}
}
