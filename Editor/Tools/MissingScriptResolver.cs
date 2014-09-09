// ***********************************************************
//	Copyright 2013 Daikon Forge, http://www.daikonforge.com
//	All rights reserved.
//
//	This source code is free for all non-commercial uses.
//
//	THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY EXPRESS OR
//	IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
// ***********************************************************

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

using UnityObject = UnityEngine.Object;

[CustomEditor( typeof( MonoBehaviour ) )]
public class MissingScriptResolver : Editor
{

	#region Static variables 

	private const string HELP_INFO = @"This component has a missing script. 

Possible candidates for the missing script are shown below. Click the button that represents the missing script to assign that script.

This component's properties are shown below to help you determine which script is correct.";

	// Will hold a ScriptMatcher reference for each discovered type
	private static List<ScriptMatcher> types = null;

	// Cached list of candidate scripts for each component
	private static Dictionary<UnityObject, List<ScriptLookup>> candidates = new Dictionary<UnityObject, List<ScriptLookup>>();

	#endregion

	#region Menu add-ins 

	[MenuItem( "Window/Find Missing Scripts In Prefabs", priority = 1 )]
	public static void FindMissingScriptsInPrefabs()
	{

		var progressTime = Environment.TickCount;

		#region Load all assets in project before searching

		var allAssetPaths = AssetDatabase.GetAllAssetPaths();
		for( int i = 0; i < allAssetPaths.Length; i++ )
		{

			if( Environment.TickCount - progressTime > 250 )
			{
				progressTime = Environment.TickCount;
				EditorUtility.DisplayProgressBar( "Find Missing Scripts", "Searching prefabs", (float)i / (float)allAssetPaths.Length );
			}

			AssetDatabase.LoadMainAssetAtPath( allAssetPaths[ i ] );

		}

		EditorUtility.ClearProgressBar();

		#endregion

		var prefabs = Resources
			.FindObjectsOfTypeAll( typeof( GameObject ) )
			.Cast<GameObject>()
			.Where( x => x.transform.parent == null && isPrefab( x ) )
			.OrderBy( x => x.name )
			.ToList();

		var brokenPrefabs = prefabs
			.Where( x => x.GetComponentsInChildren<Component>( true ).Any( c => c == null ) )
			.ToList();

		var message = "";
		if( brokenPrefabs.Count > 0 )
		{

			for( int i = 0; i < brokenPrefabs.Count; i++ )
			{
				var prefab = brokenPrefabs[ i ];
				var path = AssetDatabase.GetAssetPath( prefab );
				Debug.LogWarning( "Prefab has missing scripts: " + path, prefab );
			}

			message = string.Format( "Found {0} prefabs with missing scripts. The full list can be viewed in the console pane", brokenPrefabs.Count );

		}
		else
		{
			message = string.Format( "Searched {0} prefabs. No missing scripts detected.", prefabs.Count );
		}

		EditorUtility.DisplayDialog( "Find Missing Scripts", message, "OK" );

	}

	[MenuItem( "Window/Find Missing Scripts In Scene", priority = 0 )]
	public static void FindMissingScriptsInScene()
	{
		
		var broken = findBrokenObjectsInScene();
		if( broken.Count == 0 )
		{
			
			EditorUtility.DisplayDialog( "No missing scripts", "There are no objects with missing scripts in this scene.", "YAY!" );
			
			// Make sure static lists are cleaned up
			types = null;
			candidates = null;
			
			return;

		}

		// Grab the object highest up in the scene hierarchy
		var sorted = broken
			.Select( x => new { target = x, path = getObjectPath( x ) } )
			.OrderBy( x => x.path )
			.First();

		Debug.LogWarning( string.Format( "{0} objects in this scene have missing scripts, selecting '{1}' first", broken.Count, sorted.target.name ) );
		Selection.activeGameObject = sorted.target;

	}

	#endregion

	#region Unity events

	public override void OnInspectorGUI()
	{

		try
		{

			// If the inspected component does not have a missing script, 
			// revert to base Inspector functionality
			var scriptProperty = this.serializedObject.FindProperty( "m_Script" );
			if( scriptProperty == null || scriptProperty.objectReferenceValue != null )
			{
				base.OnInspectorGUI();
				return;
			}

			// Make sure that the inspector is not viewing a prefab directly.
			// Not sure what's up, but doing so with this custom inspector 
			// crashes Unity with 100% reproducability.
			var behavior = target as MonoBehaviour;
			if( behavior != null && isPrefab( behavior.gameObject ) )
			{

				var prefabMessage = @"The Missing Script Resolver cannot edit prefabs directly. Please move this prefab into a scene before editing";
				EditorGUILayout.HelpBox( prefabMessage, MessageType.Error );

				types = null;
				candidates = null;

				base.OnInspectorGUI();
				return;

			}

			// Ensure candidate list contains an entry for the inspected component
			if( candidates == null ) candidates = new Dictionary<UnityObject, List<ScriptLookup>>();
			if( !candidates.ContainsKey( target ) )
			{

				// Find all MonoScript instances that are a possible match
				// for the component currently in the inspector, and sort
				// them from mostly likely match to least likely match.
				var candidateLookup =
					getDefinedScriptTypes()
					.Select( c => new ScriptLookup() { Matcher = c, Score = c.ScoreMatch( serializedObject ) } )
					.Where( c => c.Score > 0 )
					.OrderByDescending( c => c.Score )
					.ToList();

				candidates[ target ] = candidateLookup;

			}

			// Retrieve the list of possible matching scripts
			var possibleMatches = candidates[ target ];

			GUILayout.Label( "Missing Script", "HeaderLabel" );

			// Show help information 
			EditorGUILayout.HelpBox( HELP_INFO, MessageType.Warning );

			// If there are no possible matches found, let the user know that they
			// must manually assign the missing script.
			if( possibleMatches.Count == 0 )
			{
				EditorGUILayout.HelpBox( "No matching scripts found.", MessageType.Error );
				base.OnInspectorGUI();
				return;
			}

			// Let the developer decide how many possible matches to display. 
			// This was done because sometimes there are a number of scripts
			// whose fields appear to be a match based on name and type, such
			// as when one script inherits from another without adding any 
			// additional serialized properties.
			var candidateCountConfig = EditorPrefs.GetInt( "DaikonForge.MissingScriptCount", 3 );
			var candidateCount = Mathf.Max( EditorGUILayout.IntField( "Num Scripts to Show", candidateCountConfig ), 1 );
			if( candidateCount != candidateCountConfig )
			{
				EditorPrefs.SetInt( "DaikonForge.MissingScriptCount", candidateCount );
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space( 25 );
			EditorGUILayout.BeginVertical();

			for( int i = 0; i < Mathf.Min( candidateCount, possibleMatches.Count ); i++ )
			{

				// Display a button to select the candidate script
				var candidate = possibleMatches[ i ];
				if( !GUILayout.Button( candidate.Matcher.Name, "minibutton" ) )
					continue;

				// Make this operation undo-able
#if UNITY_4_3
				Undo.RegisterCompleteObjectUndo( target, "Assign missing script" );
#else
				Undo.RegisterSceneUndo( "Assign missing script" );
#endif

				// Assign the selected MonoScript 
				scriptProperty.objectReferenceValue = candidate.Matcher.Script;
				scriptProperty.serializedObject.ApplyModifiedProperties();
				scriptProperty.serializedObject.Update();

				// Save the scene in case Unity crashes
				EditorUtility.SetDirty( this.target );
				EditorApplication.SaveScene();
				EditorApplication.SaveAssets();

				// Check for more objects with missing scripts
				if( Selection.activeGameObject.activeInHierarchy )
				{

					// If there are no more missing scripts in this scene, 
					// let the developer know and clean up static lists
					var broken = findBrokenObjectsInScene();
					if( broken.Count == 0 )
					{

						EditorUtility.DisplayDialog( "No missing scripts", "There are no objects with missing scripts in this scene", "YAY!" );

						// Make sure static lists are cleaned up
						types = null;
						candidates = null;

					}
					else
					{

						// Select the next object with missing scripts. This may be
						// the current object if there are more components with 
						// missing scripts on this object. Yay for sorting!
						Selection.activeGameObject = broken.First();

					}

				}

			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

		}
		catch( Exception err )
		{
			Debug.LogError( err );
		}

		GUILayout.Label( "Component Properties", "HeaderLabel" );
		EditorGUI.indentLevel += 1;

		base.OnInspectorGUI();

		EditorGUI.indentLevel -= 1;

	}

	#endregion

	#region Private utility functions 

	/// <summary>
	/// Returns a value indicating whether an object is a prefab
	/// </summary>
	private static bool isPrefab( GameObject item )
	{

		if( item == null )
			return false;

		return
			item != null &&
			PrefabUtility.GetPrefabParent( item ) == null &&
			PrefabUtility.GetPrefabObject( item ) != null;

	}

	/// <summary>
	/// Returns the nesting level of a GameObject
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	private static string getObjectPath( GameObject x )
	{

		var path = new System.Text.StringBuilder( 1024 );

		var depth = 0;

		while( x != null && x.transform.parent != null )
		{
			path.Append( x.name );
			path.Append( "/" );
			x = x.transform.parent.gameObject;
			depth += 1;
		}

		return depth.ToString( "D12" ) + "/" + path.ToString();

	}

	/// <summary>
	/// Returns a list of GameObject instances in the current scene that 
	/// have at least one Missing Script, sorted by their path in the 
	/// scene hierarchy
	/// </summary>
	/// <returns></returns>
	private static List<GameObject> findBrokenObjectsInScene()
	{

		// Find all of the GameObjects in the scene and sort them
		// by the "path" in the scene hierarchy
		var brokenObjects = Resources
			.FindObjectsOfTypeAll( typeof( GameObject ) )
			.Cast<GameObject>()
			.Where( x => x.activeInHierarchy && x.GetComponents<Component>().Any( c => c == null ) )
			.OrderBy( x => getObjectPath( x ) )
			.ToList();

		return brokenObjects;

	}

	/// <summary>
	/// Return a list containing a ScriptMatcher instance for 
	/// each MonoScript defined in the current project.
	/// Note that Unity defines a MonoScript even for types 
	/// defined in referenced assemblies, not just user scripts.
	/// </summary>
	/// <returns></returns>
	private static List<ScriptMatcher> getDefinedScriptTypes()
	{

		if( types != null )
			return types;

		// Get the list of all MonoScript instances in the project that
		// are not abstract or unclosed generic types
		types = Resources
			.FindObjectsOfTypeAll( typeof( MonoScript ) )
			.Where( x => x.GetType() == typeof( MonoScript ) ) // Fix for Unity crash
			.Cast<MonoScript>()
			.Select( x => new ScriptMatcher( x ) )
			.Where( x => x.Type != null && !x.Type.IsAbstract && !x.Type.IsGenericType )
			.ToList();

		// Ignore any MonoScript types defined by Unity, as it's extremely 
		// unlikely that they could ever be missing
		var editorAssembly = typeof( Editor ).Assembly;
		var engineAssembly = typeof( MonoBehaviour ).Assembly;
		types.RemoveAll( x => x.Type.Assembly == editorAssembly || x.Type.Assembly == engineAssembly );

		return types;

	}

	#endregion

	#region Nested utility classes 

	private class ScriptLookup
	{
		public ScriptMatcher Matcher;
		public float Score;
	}

	/// <summary>
	/// Used to determine the likelihood that a particular MonoScript
	/// is a match for a component with the "Missing Script" issue
	/// </summary>
	private class ScriptMatcher
	{

		#region Private data members 

		private MonoScript script;
		private Type type;
		private List<FieldInfo> fields;

		#endregion

		#region Constructor

		public ScriptMatcher( MonoScript script )
		{
			this.script = script;
			this.type = script.GetClass();
			this.fields = GetAllFields( type ).ToList();
		}

		#endregion

		#region Public properties

		public MonoScript Script { get { return this.script; } }

		public Type Type { get { return this.type; } }

		public string Name { get { return type.Name; } }

		#endregion

		#region Public methods

		/// <summary>
		/// Generates a score indicating how likely the script is to 
		/// be a match for the serialized object, with values closer
		/// to 1 being most likely and values closer to 0 being the
		/// least likely.
		/// </summary>
		/// <param name="target">The component with the Missing Script issue</param>
		public float ScoreMatch( SerializedObject target )
		{

			int count = 0;

			var iter = target.GetIterator();
			iter.Next( true );
			while( iter.Next( false ) )
			{
				var field = fields.Find( f => f.Name == iter.name );
				if( field != null )
				{
					switch( iter.propertyType )
					{
						case SerializedPropertyType.ArraySize:
							if( field.FieldType.HasElementType ) count += 1;
							else if( typeof( IEnumerable ).IsAssignableFrom( field.FieldType ) ) count += 1;
							break;
						case SerializedPropertyType.AnimationCurve:
							if( field.FieldType == typeof( AnimationCurve ) ) count += 1;
							break;
						case SerializedPropertyType.Boolean:
							if( field.FieldType == typeof( bool ) ) count += 1;
							break;
						case SerializedPropertyType.Bounds:
							if( field.FieldType == typeof( Bounds ) ) count += 1;
							break;
						case SerializedPropertyType.Color:
							if( field.FieldType == typeof( Color32 ) ) count += 1;
							else if( field.FieldType == typeof( Color ) ) count += 1;
							break;
						case SerializedPropertyType.Enum:
							if( field.FieldType.IsEnum ) count += 1;
							break;
						case SerializedPropertyType.Float:
							if( typeof( float ).IsAssignableFrom( field.FieldType ) ) count += 1;
							break;
						case SerializedPropertyType.Integer:
							if( typeof( int ).IsAssignableFrom( field.FieldType ) ) count += 1;
							else if( typeof( uint ).IsAssignableFrom( field.FieldType ) ) count += 1;
							else if( field.FieldType.IsEnum ) count += 1;
							break;
						case SerializedPropertyType.ObjectReference:
							if( !field.FieldType.IsValueType ) count += 1;
							break;
						case SerializedPropertyType.Rect:
							if( field.FieldType == typeof( Rect ) ) count += 1;
							break;
						case SerializedPropertyType.String:
							if( field.FieldType == typeof( string ) ) count += 1;
							break;
						case SerializedPropertyType.Vector2:
							if( field.FieldType == typeof( Vector2 ) ) count += 1;
							break;
						case SerializedPropertyType.Vector3:
							if( field.FieldType == typeof( Vector3 ) ) count += 1;
							break;
						default:
							count += 1;
							break;
					}

				}

			}

			if( count == 0 )
				return 0f;

			return (float)count / fields.Count;

		}

		#endregion

		#region Private methods 

		/// <summary>
		/// Returns all instance fields on an object, including inherited fields
		/// </summary>
		private static FieldInfo[] GetAllFields( Type type )
		{

			// http://stackoverflow.com/a/1155549/154165

			if( type == null )
				return new FieldInfo[ 0 ];

			BindingFlags flags =
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Instance |
				BindingFlags.DeclaredOnly;

			return
				type.GetFields( flags )
				.Concat( GetAllFields( type.BaseType ) )
				.Where( f => !f.IsDefined( typeof( HideInInspector ), true ) )
				.ToArray();

		}

		#endregion

	}

	#endregion

}