/*using System.IO;
using BepInEx;
using UnityEngine;
//This will be for making the fingers move when I get around to it.
namespace SN1MC
{
    class AssetLoader
    {
        private const string assetsDir = "SubnauticaMotionControls/SubVRAssets/";

        public static GameObject LeftHandBase;
        public static GameObject RightHandBase;

        public AssetLoader()
        {
            var subnauticavr = LoadBundle("subvr");
            LeftHandBase = LoadAsset<GameObject>(subnauticavr, "vr_glove_left_model_slim.prefab");
            RightHandBase = LoadAsset<GameObject>(subnauticavr, "vr_glove_right_model_slim.prefab");
        }

        private T LoadAsset<T>(AssetBundle bundle, string prefabName) where T : UnityEngine.Object
        {
            var asset = bundle.LoadAsset<T>($"{prefabName}");
            if (asset)
                return asset;
            else
            {
                Debug.Log($"Failed to load asset {prefabName}");
                return null;
            }

        }

        private static AssetBundle LoadBundle(string assetName)
        {
            var myLoadedAssetBundle =
                AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, Path.Combine(assetsDir, assetName)));

            Debug.Log("PluginPath: " + Paths.PluginPath);
            if (myLoadedAssetBundle == null)
            {
                Debug.Log($"Failed to load AssetBundle {assetName}");
                return null;
            }

            return myLoadedAssetBundle;
        }

    }
}*/