using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuItems : MonoBehaviour {

#if UNITY_EDITOR
	[MenuItem("Progress Bars/Help")]
	public static void OpenHelp()
	{
        Application.OpenURL("http://muscipula.com/power-progress-bars-help/"); 
	}
	#endif
}
