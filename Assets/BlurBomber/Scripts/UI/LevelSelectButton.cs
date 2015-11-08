using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectButton : MonoBehaviour {

	public string levelName = "Level1";

	public void LoadLevel () {
		Application.LoadLevel(levelName);
	}
}
