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
		Inherits GH_Component	
		
	''Constructor
    Public Sub New()
    	MyBase.New("EnergyPlus Simulation","Simulate","Initiate EnergyPlus Simulation", "Gerilla","Simulation")
    End Sub
 
    Public Overrides ReadOnly Property ComponentGuid As System.Guid
    	Get
    		Return New Guid("22B1F0E8-A8BA-414B-B2DF-D10B0CEF905F")  ''www.createguid.com
 
        End Get
    End Property
    
    Protected Overrides ReadOnly property icon As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconSimulate
 	    End Get
	End Property
    
    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
    	pManager.AddBooleanParameter("Run EnergyPlus Simulation","Run","Simulate Output From IDF Compiler", GH_ParamAccess.item, False)
    	pManager.AddTextParameter("Weather File Path","Weather File Path","Input Full File Path For Weather File Without The File Extension; ie. c:\MyDocuments\Gerilla\Weatherfiles\NewYorkTMY3.epw", GH_ParamAccess.item)
    	'pManager.AddTextParameter("Output File Path","W","Input folder file path for simulation results; ie. c:\MyDocuments\Gerilla\myProject\Outputs\", "C:\EnergyPlusV8-0-0\Outputs\" ,GH_ParamAccess.item)
    	'pManager.AddTextParameter("IDF Project Name", "I","Input Project Name for IDF",GH_ParamAccess.item)
    End Sub
 
    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
    	'pManager.AddTextParameter("Batch File", "B", "Batch file that is executed")
    	
    End Sub
 
 	Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
 		
 		Dim run As Boolean
 		'Dim projectName As String = nothing
 		Dim weatherPath As String = Nothing
 		Dim outputPath As String = (GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Results\" & GerillaProjectSetup.PublicProjectName)
 		
 		Dim batchFile As String
 		
 		If (Not DA.GetData(0, run)) Then Return
 		'If (Not DA.GetData(1, projectName)) Then Return
 		If (Not DA.GetData(1, weatherPath)) Then Return
 		
 			
 		batchFile = My.Computer.FileSystem.ReadAllText("C:\Gerilla\gerillaBatchTemplate.bat")
 		
 		batchFile = Replace(batchFile, "=%~1", "=C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName)
 		batchFile = Replace(batchFile, "=%~2", "=C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName)
 		'batchFile = Replace(batchFile, "=%~1", "=" & GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName)
 		'batchFile = Replace(batchFile, "=%~2", "=" & GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName)
 		batchFile = Replace(batchFile, "=%~4", "=" & weatherPath)
 		batchFile = Replace(batchFile, "=%~~10", "=" & outputPath)
 		

        
        'DA.SetData(0, batchFile)
        
 		If run = True Then
 			
 			'Dim newBatch As New StreamWriter("C:\EnergyPlusV8-0-0\newGerillaEPLRun.bat")
 			'Dim newBatch As New StreamWriter(GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\" & GerillaProjectSetup.PublicProjectName & "Run.bat")
 			Dim newBatch As New StreamWriter("C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName & "Run.bat")
 			newBatch.Write(batchFile)
 			newBatch.Close
 	
' 			System.Diagnostics.Process.Start("C:\EnergyPlusV8-0-0\newGerillaEPLRun.bat")
 			System.Diagnostics.Process.Start("C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName & "Run.bat")
 	
 		End If
 	
 		
	 	End Sub
End Class
