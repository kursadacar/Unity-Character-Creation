#if UNITY_EDITOR
using UnityEditor;

namespace CoopGame.AssetPostprocessor {

	public class TexturePostprocessor : UnityEditor.AssetPostprocessor{
		private void OnPreprocessTexture()
		{
			var textureImporter = (TextureImporter)assetImporter;
			textureImporter.alphaIsTransparency = true;
		}
	}
}
#endif