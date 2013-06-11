'
' Created by SharpDevelop.
' User: Ben
' Date: 11/16/2011
' Time: 11:23 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry
'Imports Microsoft.Office.Interop.Excel
'Imports Microsoft.Office.Interop
Imports System.IO

Public Class GerillaResults
	 Inherits GH_Component
     
    'Constructor
    Public Sub New()
        MyBase.New("EnergyPlus Results","Results","EnergyPlus Simulation Results","Gerilla","Simulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
		Return New Guid ("666DEB7B-8E4D-458F-AD09-EA6DD6D2C4D9")
        End Get
    End Property
    
    Protected Overrides ReadOnly property icon As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconResults
 	    End Get
	End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        
    End Sub
    
    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
    	pManager.AddTextParameter("Zone Heating Monthly (J/month)","Zone Heating Monthly (J/month)","Zone Heating Monthly (J/month)", GH_ParamAccess.tree)
    	pManager.AddTextParameter("Zone Cooling Monthly (J/month)","Zone Cooling Monthly (J/month)","Zone Cooling Monthly (J/month)", GH_ParamAccess.tree)
    	'pManager.AddTextParameter("District Heating Monthly (J/month)","DistrictHeatingMontly (J/month)","District Heating Monthly (J/month)", GH_ParamAccess.tree)
    	
    End Sub
    
    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
    	
    	Dim projectFilePath As String = GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Results\" & GerillaProjectSetup.PublicProjectName
    	
    	Dim zoneHeating As New Grasshopper.DataTree(Of String)
    	Dim zoneHeatingResults As New ePlusResults
    	
    	zoneHeatingResults.GetResults(projectFilePath, 7, 80, 33, zoneHeating)
    	
    	Dim zoneCooling As New Grasshopper.DataTree(Of String)
    	Dim zoneCoolingResults As New ePlusResults
    	
    	zoneCoolingResults.GetResults(projectFilePath, 58, 101, 33, zoneCooling)
    	
    	'Dim districtHeating As New Grasshopper.DataTree(Of String)
    	'Dim districtHeatingResults As New ePlusResults
    	
    	'districtHeatingResults.GetDistrictResults(projectFilePath, 183, districtHeating)


		DA.SetDataTree(0, zoneHeating)
		DA.SetDataTree(1, zoneCooling)
		'DA.SetDataTree(2, districtHeating)

    End Sub
    
    End Class