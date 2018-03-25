//using UnityEditor;
//
//namespace Editor
//{
//	public static class TagsEditor {
//		static void AddTags()
//		{
//			//add tags
//			var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
//			if ((asset == null) || (asset.Length <= 0)) return;
//			SerializedObject so = new SerializedObject(asset[0]);
//			SerializedProperty tags = so.FindProperty("tags");
// 
//			tags.InsertArrayElementAtIndex(0);
//			tags.GetArrayElementAtIndex(0).stringValue = GlobalVariables.TagBonus;
//			tags.InsertArrayElementAtIndex(1);
//			tags.GetArrayElementAtIndex(1).stringValue = GlobalVariables.TagCoins;
//			tags.InsertArrayElementAtIndex(2);
//			tags.GetArrayElementAtIndex(2).stringValue = GlobalVariables.TagEnemy;
//			tags.InsertArrayElementAtIndex(3);
//			tags.GetArrayElementAtIndex(3).stringValue = GlobalVariables.TagPlayer;
//			tags.InsertArrayElementAtIndex(4);
//			tags.GetArrayElementAtIndex(4).stringValue = GlobalVariables.TagWall;
//			tags.InsertArrayElementAtIndex(5);
//			tags.GetArrayElementAtIndex(5).stringValue = GlobalVariables.TagFence;
//			so.ApplyModifiedProperties();
//			so.Update();
//		}
//	}
//}
