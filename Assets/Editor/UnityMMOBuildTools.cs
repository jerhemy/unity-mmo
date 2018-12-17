// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
using UnityEditor;
using System.IO;
using UnityEngine;


namespace UnityMMO.Editor
{
	public class UnityMMOBuildTools
	{
		[MenuItem("Unity MMO/Build AssetBundles")]
		public static void BuildAllAssetBundles()
		{
			string assetBundleDirectory = "Assets/AssetBundles";
			if (!Directory.Exists(assetBundleDirectory))
			{
				Directory.CreateDirectory(assetBundleDirectory);
			}

			BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
		}

		[MenuItem("Unity MMO/Get Asset Bundle names")]
		public static void GetNames()
		{
			var names = AssetDatabase.GetAllAssetBundleNames();
			JsonUtility.ToJson(names);
		}
	}
}