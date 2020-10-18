#if UNITY_EDITOR
using UnityEditor;

namespace CoopGame.AssetPostprocessors {

	public class ModelPostprocessor : UnityEditor.AssetPostprocessor{
		private void OnPreprocessModel()
		{
			var modelImporter = (ModelImporter)assetImporter;
			modelImporter.animationType = ModelImporterAnimationType.Human;
			
		}
	}
}
#endif