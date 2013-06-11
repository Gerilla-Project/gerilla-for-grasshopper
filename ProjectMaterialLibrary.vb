'Import the GH stuff

Imports Grasshopper.Kernel

Imports Grasshopper.Kernel.Types

Imports Rhino.Geometry

Imports System 

Imports System.IO

Imports System.Diagnostics

Public Class ProjectMaterialDatabase
	
Inherits GH_Component
    
    'Constructor
    Public Sub New()
    	
    	MyBase.New("Project Material Library", "Project Materials", "Select Material From Project Material Library", "Gerilla", "Materials")
    	
    End Sub
    
    
    
    Public Overrides ReadOnly Property ComponentGuid As System.Guid

        Get
 		Return New Guid("D4FFC6D5-3A80-484C-BA90-2BD997A4B908") 'www.createguid.com
        End Get

    End Property
    
    Protected Overrides ReadOnly property internal_icon_24x24 As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconLibrary2  
 	    End Get
	End Property
    
    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)

		pManager.AddIntegerParameter("Select Project Material", "Select Material", "Select Project Material", 0, GH_ParamAccess.item)
		
    End Sub


    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
    	
    	pManager.AddTextParameter("Project Material", "IDF", "Project Material For IDF Compiler", GH_ParamAccess.item)
    	
    End Sub
    
    
    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
    	
    	'Private Variables
    	
    	Dim ProjectMaterialCount As Integer = Nothing
    	Dim ProjectMaterialsList As New ArrayList
    	Dim SelectedMaterialNumber As Integer = Nothing
    	Dim MaterialChoice As String = Nothing 
    	Dim ProjectFilePath As String = (GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Materials")
    	
		If (Not DA.GetData(0,SelectedMaterialNumber)) Then Return
		
		'Find out how many files are in the Project Material Library
    	ProjectMaterialCount = System.IO.Directory.GetFiles(ProjectFilePath).Length()
'    	MsgBox(ProjectMaterialCount)
'		MsgBox(ProjectFilePath)

		Dim FullMaterialPath As String 
		Dim MaterialFileName As String
		Dim MaterialName As String 
		
		'Get the list of Material Names
    	For i As Integer = 0 To ProjectMaterialCount - 1 
    		
    		FullMaterialPath = System.IO.Directory.GetFiles(ProjectFilePath)(i)
    		
    		MaterialFileName = Replace(FullMaterialPath, ProjectFilePath & "\", "")
    		MaterialName = Replace(MaterialFileName, ".txt", "")
    		
    		ProjectMaterialsList.Add(MaterialName)
    		
    	Next
    	
    	If SelectedMaterialNumber < ProjectMaterialCount Then
    		
    		MaterialChoice = ProjectMaterialsList(SelectedMaterialNumber)
    		
    	Else
    		
    		MaterialChoice = "ERROR: No material choosen." 
    		MsgBox("ERROR: No material choosen. Choose a number that is within the bounds of the Project Material Library.")
    		
    	End If
    	
		'Assign Outputs
    	DA.SetData(0, MaterialChoice)

    End Sub
    
End Class