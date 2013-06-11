'Import the GH stuff

Imports Grasshopper.Kernel

Imports Grasshopper.Kernel.Types

Imports Rhino.Geometry

Imports System.IO

Public Class GerillaMaterials
	
Inherits GH_Component
    
    'Constructor
    Public Sub New()
    	
    	MyBase.New("Gerilla Material Maker", "Material Maker", "Create Custom Gerilla Materials", "Gerilla", "Materials")
    	
    End Sub
    
    Public Overrides ReadOnly Property ComponentGuid As System.Guid

        Get
 		Return New Guid("B8E442C4-AB3A-4C33-8C15-95271811C5EC") 'www.createguid.com
        End Get

    End Property
    
    Protected Overrides ReadOnly property icon As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconMaterialMaker
 	    End Get
	End Property
    

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)

		pManager.AddTextParameter("Name", "Name", "Material Name", GH_ParamAccess.item, "Default Name")
		pManager.AddIntegerParameter("Roughness", "Roughness", "Material Roughness; 1 = Smoothest, 6 = Roughest", GH_ParamAccess.item, 0)
		pManager.AddTextParameter("Thickness", "Thickness (m)", "Thickness (m)", GH_ParamAccess.item, "0")
		pManager.AddTextParameter("Conductivity", "Conductivity (W/m-K)", "Conductivity (W/m-K)", GH_ParamAccess.item, "0")
		pManager.AddTextParameter("Density", "Density (kg/m3)", "Density (kg/m3)", GH_ParamAccess.item, "0")
		pManager.AddTextParameter("SpecificHeat", "Specific Heat (J/kg-K)", "Specific Heat (J/kg-K)", GH_ParamAccess.item, "0")
		pManager.AddTextParameter("ThermalAbsorptance", "Thermal Absorptance (0 to 0.99)", "Thermal Absorptance (0 to 0.99)", GH_ParamAccess.item, "0")
		pManager.AddTextParameter("SolarAbsorptance", "Solar Absorptance (0 to 0.99)", "Solar Absorptance (0 to 0.99)", GH_ParamAccess.item, "0")
		pManager.AddTextParameter("VisibleAbsorptance", "Visible Absorptance (0 to 0.99)", "Visible Absorptance (0 to 0.99)", GH_ParamAccess.item,"0")
		pManager.AddBooleanParameter("ProjectLibrary", "Save to Project Library", "Project Library", GH_ParamAccess.item, False)
		pManager.AddBooleanParameter("GerillaLibrary", "Save to Gerilla Library", "Gerilla Library", GH_ParamAccess.item, False)

    End Sub


    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
    	
    	pManager.AddTextParameter("Material Name", "Material Name", "Material Name", GH_ParamAccess.item)
		pManager.AddTextParameter("Material IDF", "IDF", "Custom Gerilla Material For IDF Compiler", GH_ParamAccess.item)

    End Sub
    
    
    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
    	
    	'Private Variables
    	
    	Dim myName As String = Nothing
    	Dim myRoughness As Integer = Nothing
	   	Dim myThickness As String = Nothing
		Dim myConductivity As String = Nothing
		Dim myDensity As String = Nothing
		Dim mySpecificHeat As String = Nothing
		Dim myThermalAbsorptance As String = Nothing
		Dim mySolarAbsorptance As String = Nothing
		Dim myVisibleAbsorptance As String = Nothing
		Dim myProjectDatabase As Boolean = Nothing
		Dim myGerillaDatabase As Boolean = Nothing 
    	
		
		If (Not DA.GetData(0,myName)) Then Return
		If (Not DA.GetData(1,myRoughness)) Then Return
		If (Not DA.GetData(2,myThickness)) Then Return
		If (Not DA.GetData(3,myConductivity)) Then Return		
		If (Not DA.GetData(4,myDensity)) Then Return
		If (Not DA.GetData(5,mySpecificHeat)) Then Return		
		If (Not DA.GetData(6,myThermalAbsorptance)) Then Return	
		If (Not DA.GetData(7,mySolarAbsorptance)) Then Return			
		If (Not DA.GetData(8,myVisibleAbsorptance)) Then Return
		If (Not DA.GetData(9,myProjectDatabase)) Then Return		
		If (Not DA.GetData(10,myGerillaDatabase)) Then Return
		
		
		Dim myMaterialOutput As New List (of String)
		
		
		myMaterialOutput.Add("Material,")
		myMaterialOutput.Add(myName & "		!- Name")
		
		Select Case myRoughness
            Case 1 
                myMaterialOutput.Add("VerySmooth        !- Roughness")
            Case 2 
                myMaterialOutput.Add("Medium Smooth        !- Roughness")
            Case 3 
                myMaterialOutput.Add("Smooth        !- Roughness")
            Case 4
                myMaterialOutput.Add("Rough        !- Roughness")
            Case 5
                myMaterialOutput.Add("MediumRough        !- Roughness")
            Case 6
                myMaterialOutput.Add("VeryRough        !- Roughness")
            Case Else
                myMaterialOutput.Add("Smooth        !- Roughness")
        End Select
        
        myMaterialOutput.Add(myThickness & "		!- Thickness")
		myMaterialOutput.Add(myConductivity & "		!- Conductivity {W/m-K}") 
		myMaterialOutput.Add(myDensity & "		!- Density {kg/m3}")
		myMaterialOutput.Add(mySpecificHeat & "		!- Specific Heat {J/kg-K}")
		myMaterialOutput.Add(myThermalAbsorptance & "		!- Thermal Absorptance")
		myMaterialOutput.Add(mySolarAbsorptance & "		!- Solar Absorptance")
		myMaterialOutput.Add(myVisibleAbsorptance & "		!- Visible Absorptance")
		myMaterialOutput.Add("")
		
		'Save to Project Library

		Dim ProjectDatabaseMessageBox As Integer 
		
		If myProjectDatabase = True Then
			
			If System.IO.File.Exists(GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Materials\" & myName & ".txt") = True Then
				
				Select Case (ProjectDatabaseMessageBox = MsgBox("This material already exists in the Project Library. Do you want to overwrite?",vbYesNo,"Save Material"))
				Case vbYes
					Dim ObjectWriter As New System.IO.StreamWriter(GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Materials\" & myName & ".txt")
					For j As Integer = 0 To myMaterialOutput.Count - 1
						ObjectWriter.WriteLine(myMaterialOutput(j))	
					Next
				ObjectWriter.Close()
				MsgBox("Material Updated")
				Case vbNo
					MsgBox("Material NOT Saved")
				End Select 
				
			Else
				
				Dim ObjectWriter As New System.IO.StreamWriter(GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Materials\" & myName & ".txt")
				For j As Integer = 0 To myMaterialOutput.Count - 1
					ObjectWriter.WriteLine(myMaterialOutput(j))	
				Next
				ObjectWriter.Close()
				MsgBox("Material Updated")
				
			End If
			
		End If
		
		
		'Save to Gerilla Library
		
		Dim GerillaMaterialLibraryPath As String = "C:\geRILLA\Libraries\Materials\"
		
		Dim GerillaDatabaseMessageBox As Integer 
		
		If myGerillaDatabase = True Then
			
			If System.IO.File.Exists(GerillaMaterialLibraryPath & myName & ".txt") = True Then
				
				Select Case (GerillaDatabaseMessageBox = MsgBox("This material already exists in the Gerilla Library. Do you want to overwrite?",vbYesNo,"Save Material"))
						
					Case vbYes
						
						'Save the material to the Gerilla Database
						Dim GerillaObjectWriter As New System.IO.StreamWriter(GerillaMaterialLibraryPath & myName & ".txt")
						For j As Integer = 0 To myMaterialOutput.Count - 1
							GerillaObjectWriter.WriteLine(myMaterialOutput(j))	
						Next	
						GerillaObjectWriter.Close()
						
						MsgBox("Material Updated in Gerilla Library")
					
					Case vbNo
						MsgBox("Material NOT Saved")
						
				End Select
				
			Else
				
				'Save the material to the Gerilla Database
				Dim GerillaObjectWriter As New System.IO.StreamWriter(GerillaMaterialLibraryPath & myName & ".txt")
				For j As Integer = 0 To myMaterialOutput.Count - 1
					GerillaObjectWriter.WriteLine(myMaterialOutput(j))	
				Next
				GerillaObjectWriter.Close()				
			
				MsgBox("Material Saved")
				
			End If
			
		End If
		
		DA.SetData(0, myName)
		DA.SetDataList(1, myMaterialOutput)
    	
    End Sub

End Class