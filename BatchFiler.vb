'
' Created by SharpDevelop.
' User: Ben
' Date: 11/14/2011
' Time: 11:58 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
Imports System.IO

Public Class BatchFiler
	'The EnergyPlus Simulation component accepts the weather file and a boolean that sets up the batch file and runs the simulation.
	'NOTE: I don't think this needs to be its own component, and it could be rolled into the main IDFCompiler component. - Brendan 2013-06-29.
	Inherits GH_Component
		
	'Constructor
    Public Sub New()
    	MyBase.New("EnergyPlus Simulation","Simulate","Initiate EnergyPlus Simulation", "Gerilla","Simulation")
    End Sub
    
    'Guid
    Public Overrides ReadOnly Property ComponentGuid As System.Guid
    	Get
    		Return New Guid("22B1F0E8-A8BA-414B-B2DF-D10B0CEF905F")  ''www.createguid.com 
        End Get
    End Property
    
    'Icon
    Protected Overrides ReadOnly property icon As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconSimulate
 	    End Get
	End Property
	
	'Inputs
	Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
		'Input 0: boolean to run the solution. Input 1: weather file file path.
		pManager.AddBooleanParameter("Run EnergyPlus Simulation","Run","Simulate Output From IDF Compiler", GH_ParamAccess.item, False)
    	pManager.AddTextParameter("Weather File Path","Weather File Path","Input Full File Path For Weather File Without The File Extension; ie. c:\MyDocuments\Gerilla\Weatherfiles\NewYorkTMY3.epw", GH_ParamAccess.item)   	
	End Sub
    
    'Outputs
	Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)    	
		'No outputs	
	End Sub
	
	'Solve
	Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
		
		'Get the inputs.
 		Dim run As Boolean
 		Dim weatherPath As String = Nothing
 		
  		If (Not DA.GetData(0, run)) Then Return
 		If (Not DA.GetData(1, weatherPath)) Then Return
 		
 		'Assemble the output path from the Gerilla Project Information component.
 		Dim outputPath As String = (GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Results\" & GerillaProjectSetup.PublicProjectName)
 		
 		'Read the batch file template.
 		Dim batchFile As String = My.Computer.FileSystem.ReadAllText("C:\Gerilla\gerillaBatchTemplate.bat") 		 		
 		
 		'Add the appropriate file paths to the batch file template.
 		batchFile = Replace(batchFile, "=%~1", "=C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName)
 		batchFile = Replace(batchFile, "=%~2", "=C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName)
 		batchFile = Replace(batchFile, "=%~4", "=" & weatherPath)
 		batchFile = Replace(batchFile, "=%~~10", "=" & outputPath)
        
 		If run Then
 			'Write out the batch file and run it.
 			Dim newBatch As New StreamWriter("C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName & "Run.bat")
 			newBatch.Write(batchFile)
 			newBatch.Close
 	
 			System.Diagnostics.Process.Start("C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName & "Run.bat")
 	
 		End If
 	 		
	End Sub
	
End Class
