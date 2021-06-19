using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileBrowser : MonoBehaviour
{
	public void test(){
		return 1;
	}

	public void OpenBrowser(string a){
		OnGUI();
	}

    public static bool FileBrowser( ref string location, ref Vector2 directoryScroll, ref Vector2 fileScroll ){
    	bool complete;
    	DirectoryInfo directoryInfo;
    	DirectoryInfo directorySelection;
    	FileInfo fileSelection;
    	int contentWidth;
 
 
    	// Our return state - altered by the "Select" button
    	complete = false;
 
    	// Get the directory info of the current location
    	fileSelection = new FileInfo( location );
    	if( (fileSelection.Attributes & FileAttributes.Directory) == FileAttributes.Directory )
    	{
    		directoryInfo = new DirectoryInfo( location );
    	}
    	else
    	{
    		directoryInfo = fileSelection.Directory;
    	}
 
 
    	if( location != "/" && GUI.Button( new Rect( 10, 20, 410, 20 ), "Up one level" ) )
    	{
        	directoryInfo = directoryInfo.Parent;
        	location = directoryInfo.FullName;
    	}
 
 
    	// Handle the directories list
    	GUILayout.BeginArea( new Rect( 10, 40, 200, 300 ) );
        GUILayout.Label( "Directories:" );
        directoryScroll = GUILayout.BeginScrollView( directoryScroll );
       	directorySelection = BehaveLibrary.Resources.SelectList( directoryInfo.GetDirectories(), null ) as DirectoryInfo;
    	GUILayout.EndScrollView();
    	GUILayout.EndArea();
 
    	if( directorySelection != null )
    	// If a directory was selected, jump there
    	{
    	    location = directorySelection.FullName;
    	}
 
 
    	// Handle the files list
    	GUILayout.BeginArea( new Rect( 220, 40, 200, 300 ) );
        GUILayout.Label( "Files:" );
        fileScroll = GUILayout.BeginScrollView( fileScroll );
   		fileSelection = BehaveLibrary.Resources.SelectList( directoryInfo.GetFiles(), null ) as FileInfo;
    	GUILayout.EndScrollView();
    	GUILayout.EndArea();
 
    	if( fileSelection != null )
    	// If a file was selected, update our location to it
    	{
        	location = fileSelection.FullName;
    	}
 
 
    	// The manual location box and the select button
    	GUILayout.BeginArea( new Rect( 10, 350, 410, 20 ) );
    	GUILayout.BeginHorizontal();		
    	location = GUILayout.TextArea( location );
 
    	contentWidth = ( int )GUI.skin.GetStyle( "Button" ).CalcSize( new GUIContent( "Select" ) ).x;
   		if( GUILayout.Button( "Select", GUILayout.Width( contentWidth ) ) )
   		{
   			complete = true;
   		}
    	GUILayout.EndHorizontal();
    	GUILayout.EndArea();
 
 
    	return complete;
	}


	public void FileBrowserWindow( int idx )
	{
		if( FileBrowser( ref location, ref directoryScroll, ref fileScroll ) )
		{
			fileBrowser = false;
		}
	}
 
	public void OnGUI()
	{
		if( fileBrowser )
		{
			GUI.Window( 0, new Rect( ( Screen.width - 430 ) / 2, ( Screen.height - 380 ) / 2, 430, 380 ), FileBrowserWindow, "Browse" );
			return;
		}
	}


	

}
