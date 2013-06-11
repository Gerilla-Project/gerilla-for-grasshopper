'Import the GH stuff

Imports Grasshopper.Kernel

Imports Grasshopper.Kernel.Types

Imports Rhino.Geometry

Imports System 

Imports System.IO

Imports System.Diagnostics

Public Class GerillaProjectSetup
	
Inherits GH_Component
    
    'Constructor
    Public Shared PublicProjectName As String = Nothing
    Public Shared PublicProjectFilePath As String = Nothing 
    Public Shared PublicProjectLocation As String = Nothing 
    
    Public Sub New()
    	
    	MyBase.New("Gerilla Project Information", "Project Information", "Set Gerilla Project Information", "Gerilla", "Simulation")
    	
    End Sub
    
    Public Overrides ReadOnly Property ComponentGuid As System.Guid

        Get
 		Return New Guid("731C2C6F-55DC-4A82-AE93-05108471D5E2") 'www.createguid.com
        End Get

    End Property
    
    Protected Overrides ReadOnly property internal_icon_24x24 As System.drawing.Bitmap
 		Get
    		Return gerillaResource.ge_Icon_Blue
 	    End Get
    End Property
    
    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)

		pManager.AddTextParameter("Project Name", "Project Name", "Project Name", GH_ParamAccess.item, "No Name Input")
		pManager.AddTextParameter("Project File Path", "Project File Path", "Project File Path", GH_ParamAccess.item, "No Path Input")
		'pManager.AddTextParameter("Project Location", "Project Location", "Project Location","No Location Input", GH_ParamAccess.item)
		pManager.AddBooleanParameter("Create Project", "Create a New Gerilla Project", "Create a New Gerilla Project", GH_ParamAccess.item, False)
		
    End Sub


    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
    	
    	'This node has no outputs.

    End Sub
    
    
    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
    	
    	'Private Variables
    	
    	Dim grabProjectName As String = Nothing
    	Dim grabProjectFilePath As String = Nothing
    	Dim grabProjectLocation As String = Nothing
    	Dim CreateNewProject As Boolean = False
    	
    	If (Not DA.GetData(0, grabProjectName)) Then Return
    	If (Not DA.GetData(1, grabProjectFilePath)) Then Return
    	'If (Not DA.GetData(2, grabProjectLocation)) Then Return 
    	If (Not DA.GetData(2, CreateNewProject)) Then Return 
    	
    	'Make these variables available to the other nodes in Gerilla
    	
    	PublicProjectName = grabProjectName
    	PublicProjectFilePath = grabProjectFilePath 
    	PublicProjectLocation = grabProjectLocation
    	
    	'Create the project folders
    	
		If CreateNewProject = True Then
			'''''Commented out for video to prevent windows box from popping up everytime we recompute'''''
'			If System.IO.Directory.Exists(PublicProjectFilePath & "\" & PublicProjectName) = True Then
'					
'				MsgBox("This project already exists. There is no need to recreate it.",vbOK,"New Gerilla Project")
'
'			Else
'				
				Directory.CreateDirectory(PublicProjectFilePath & "\" & PublicProjectName)
				Directory.CreateDirectory(PublicProjectFilePath & "\" & PublicProjectName & "\Assemblies")
				Directory.CreateDirectory(PublicProjectFilePath & "\" & PublicProjectName & "\Materials")
				Directory.CreateDirectory(PublicProjectFilePath & "\" & PublicProjectName & "\Results")
				
'				MsgBox("The '" & PublicProjectName & "' Project has been created.",vbOK,"New Gerilla Project")
'				
'			End If
			
		End If
 
    	'Save Project File Path to a Text File
'    	Dim ObjectWriter As New System.IO.StreamWriter("C:\geRILLA\Libraries\ProjectFilePaths\OpenProjectFilePath.txt")
'    	ObjectWriter.WriteLine(grabProjectFilePath)
'    	ObjectWriter.Close()
				
    End Sub
    
End Class