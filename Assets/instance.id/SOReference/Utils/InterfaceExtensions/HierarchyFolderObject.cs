// -- HierarchyFolderObject.cs -------------------------------------------
// https://gist.github.com/yasirkula/6add4dd2a392524fe3bd7c9882c839e4 ----
// -----------------------------------------------------------------------

#define ENABLE_LOGGING // Logs the folder objects that were flattened to the console
//#define SIMULATE_BUILD_BEHAVIOUR_ON_PLAY_MODE // Simulates Execution.AtBuildTime when entering Play Mode in the Editor, as well

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
#endif
using UnityEngine;

[DisallowMultipleComponent]
public class HierarchyFolderObject : MonoBehaviour
{
	public enum Execution
	{
		AtBuildTime = 0,
		OnAwake = 1,
		ViaScript = 2
	};

	public enum ActiveState
	{
		DontChange = 0,
		InheritFromActiveInHierarchy = 1,
		True = 2,
		False = 3
	}

	[Tooltip( "Determines whether detached child objects' GameObjects will be active or inactive" )]
	public ActiveState ChildrenActiveState = ActiveState.DontChange;

	[Tooltip( "If less than or equal to 1: child objects will simply be unparented (detached)\n" +
			  "Otherwise: child objects will be split into groups of specified size and each group will be parented to an empty object at the root of the scene" )]
	public int GroupSize = 0;

	[Tooltip( "Determines when the child objects are detached from this folder object\n\n" +
			  "AtBuildTime: While building the game, has no runtime overhead (objects instantiated at runtime will fallback to OnAwake)\n" +
			  "OnAwake: When the Awake function of this folder object is called at runtime\n" +
			  "ViaScript: When the Flatten() function of this folder object is called via a script" )]
	public Execution ExecutionTime = Execution.AtBuildTime;

	private void Awake()
	{
		if( ExecutionTime != Execution.ViaScript )
		{
#if ENABLE_LOGGING
			Debug.Log( string.Concat( "Flattened folder object at runtime: ", name ) );
#endif

			Flatten();
			Destroy( this );
		}
	}

	public void Flatten()
	{
		Transform tr = transform;
		if( tr.childCount == 0 )
			return;

		switch( ChildrenActiveState )
		{
			case ActiveState.InheritFromActiveInHierarchy:
				for( int i = tr.childCount - 1; i >= 0; i-- )
					tr.GetChild( i ).gameObject.SetActive( tr.GetChild( i ).gameObject.activeInHierarchy );

				break;
			case ActiveState.False:
				for( int i = tr.childCount - 1; i >= 0; i-- )
					tr.GetChild( i ).gameObject.SetActive( false );

				break;
			case ActiveState.True:
				for( int i = tr.childCount - 1; i >= 0; i-- )
					tr.GetChild( i ).gameObject.SetActive( true );

				break;
		}

		if( GroupSize <= 1 )
			tr.DetachChildren();
		else
		{
			Transform parent = null;
			for( int i = tr.childCount - 1, count = GroupSize; i >= 0; i-- )
			{
				if( ++count > GroupSize )
				{
					parent = new GameObject().transform;
					count = 1;
				}

				tr.GetChild( i ).SetParent( parent, true );
			}
		}
	}

#if UNITY_EDITOR
	[PostProcessScene( 2 )]
	public static void OnPostprocessScene()
	{
#if !SIMULATE_BUILD_BEHAVIOUR_ON_PLAY_MODE
		if( !BuildPipeline.isBuildingPlayer )
			return;
#endif

		Scene scene = SceneManager.GetActiveScene();
		if( scene.isLoaded )
		{
			GameObject[] roots = scene.GetRootGameObjects();
			for( int i = 0; i < roots.Length; i++ )
				FlattenFoldersRecursive( roots[i].transform );
		}
		else
		{
			// For some reason, the processed scene has its isLoaded flag set as false sometimes
			// In this case, use reflection to call the GetRootGameObjectsInternal function directly
			// (calling GetRootGameObjects when isLoaded is false throws an exception)
			MethodInfo method = typeof( Scene ).GetMethod( "GetRootGameObjectsInternal", BindingFlags.Static | BindingFlags.NonPublic );
			FieldInfo handle = typeof( Scene ).GetField( "m_Handle", BindingFlags.Instance | BindingFlags.NonPublic );

			List<GameObject> roots = new List<GameObject>( scene.rootCount );
			method.Invoke( scene, new object[] { handle.GetValue( scene ), roots } );
			for( int i = 0; i < roots.Count; i++ )
				FlattenFoldersRecursive( roots[i].transform );
		}
	}

	private static void FlattenFoldersRecursive( Transform obj )
	{
		for( int i = obj.childCount - 1; i >= 0; i-- )
			FlattenFoldersRecursive( obj.GetChild( i ) );

		HierarchyFolderObject folderObject = obj.GetComponent<HierarchyFolderObject>();
		if( folderObject && folderObject.ExecutionTime == Execution.AtBuildTime )
		{
#if ENABLE_LOGGING
			Debug.Log( string.Concat( "Flattened folder object: ", obj.name, " (", obj.gameObject.scene.name, ")" ) );
#endif

			folderObject.Flatten();
			DestroyImmediate( folderObject );
		}
	}
#endif
}