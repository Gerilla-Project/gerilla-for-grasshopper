''Import the GH stuff
'
'Imports Grasshopper.Kernel
'
'Imports Grasshopper.Kernel.Types
'
'Imports Rhino.Geometry
'
'Imports System 
'
'Imports System.IO
'
'Imports System.Diagnostics
'
'Public Class AllProjectMaterials
'	
'Inherits GH_Component
'    
'    'Constructor
'    Public Shared PublicAllProjectMaterialsList As New List(Of String)
'    Public Shared PublicAllGerillaMaterialsList As New List(Of String)
'    
'    Public Sub New()
'    	
'    	MyBase.New("All Project Materials", "All Project Materials", "All Project Materials", "Gerilla", "Library")
'    	
'    End Sub
'    
'    Public Overrides ReadOnly Property ComponentGuid As System.Guid
'
'        Get
' 		Return New Guid("E1195491-A4FD-4604-80EC-5C59BFFC9D80") 'www.createguid.com
'        End Get
'
'    End Property
'    
'    
'    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
'    	
'    	'This node has no inputs
'		
'    End Sub
'
'
'    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
'    	
'		pManager.AddTextParameter("All Materials", "All Materials", "All Materials")
'
'    End Sub
'    
'    
'    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
'    	    	
'    	'Private Variables
'    	
'    	'Trying to use a list to assembly all the variables - didn't work properly to clear the data.
'    	
'    	Dim AllProjectMaterialsStringList As New List (Of String)
'    	Dim AllGerillaMaterialsStringList As New List (Of String)
'    	
''    	PublicAllProjectMaterialsList = AllProjectMaterialsList
''    	PublicAllGerillaMaterialsList = AllGerillaMaterialsList
'    	
''    	If (Not DA.GetData(0, grabProjectName)) Then Return
'    	
'    	'Use a Dictionary to complie the list values
'    	
'
'    	
'    	'Assign Outputs
'    	DA.SetDataList(0, AllProjectMaterialsStringList)
'    	
''    	PublicAllGerillaMaterialsList.Clear
'    	
'    End Sub
'
'End Class