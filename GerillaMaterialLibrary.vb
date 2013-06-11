'Import the GH stuff

Imports Grasshopper.Kernel

Imports Grasshopper.Kernel.Types

Imports Rhino.Geometry

Imports System 

Imports System.IO

Imports System.Diagnostics

Public Class GerillaMaterialDatabase
	
Inherits GH_Component
    
    'Constructor
    Public Sub New()
    	
    	MyBase.New("Gerilla Material Library", "Gerilla Materials", "Select Material From Gerilla Material Library", "Gerilla", "Materials")
    	
    End Sub
    
    
    
    Public Overrides ReadOnly Property ComponentGuid As System.Guid

        Get
 		Return New Guid("7383B94B-8BD6-420C-99E7-D97B6C5C9A3C") 'www.createguid.com
        End Get

    End Property
    
    	Protected Overrides ReadOnly property internal_icon_24x24 As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconLibrary   '''.MakeTransparent(System.Drawing.Color.White)
 	    End Get
		End Property
    
    
    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)

		pManager.AddIntegerParameter("Select Gerilla Material", "Select Material", "Select Material From Gerilla Material Library", GH_ParamAccess.item, 0)
		
    End Sub


    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
    	
    	pManager.AddTextParameter("Gerilla Material IDF", "IDF", "Gerilla Material For IDF Compiler", GH_ParamAccess.item)
    	
    End Sub
    
    
    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
    	    	
    	'Private Variables
    	
    	Dim GerillaMaterialCount As Integer = Nothing
    	Dim GerillaMaterialsList As New ArrayList
    	Dim SelectedMaterialNumber As Integer = Nothing
    	Dim MaterialChoice As String = Nothing 
    	Dim GerillaFilePath As String = ("C:\geRILLA\Libraries\Materials\")
    	
		If (Not DA.GetData(0,SelectedMaterialNumber)) Then Return
		
		'Find out how many files are in the Project Material Library
    	GerillaMaterialCount = System.IO.Directory.GetFiles(GerillaFilePath).Length()
'    	MsgBox(ProjectMaterialCount)
'		MsgBox(ProjectFilePath)

		Dim FullMaterialPath As String 
		Dim MaterialFileName As String
		Dim MaterialName As String 
		
		'Get the list of Material Names
    	For i As Integer = 0 To GerillaMaterialCount - 1 
    		
    		FullMaterialPath = System.IO.Directory.GetFiles(GerillaFilePath)(i)
    		
    		MaterialFileName = Replace(FullMaterialPath, GerillaFilePath, "")
    		MaterialName = Replace(MaterialFileName, ".txt", "")
    		
    		GerillaMaterialsList.Add(MaterialName)
    		
    	Next
    	
    	If SelectedMaterialNumber < GerillaMaterialCount Then
    		
    		MaterialChoice = GerillaMaterialsList(SelectedMaterialNumber)
    		
    	Else
    		
    		MaterialChoice = "ERROR: No material choosen." 
    		MsgBox("ERROR: No material choosen. Choose a number that is within the bounds of the Project Material Library.")
    		
    	End If
    	
'    	'Write the material choice to the Gerilla Materials List
'    	AllProjectMaterials.PublicAllGerillaMaterialsList.Add(MaterialChoice)
'    	
'    	Dim InstanceGuid As Guid
'    	
'    	MsgBox(InstanceGuid.ToString)
    	
'    	Dim GerillaMaterialDictionary As GerillaDictionary(InstanceGuid.ToString, MaterialChoice)
    	
		'Assign Outputs
    	DA.SetData(0, MaterialChoice)

    End Sub
    
End Class