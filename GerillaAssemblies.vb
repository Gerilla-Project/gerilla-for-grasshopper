'Import the GH stuff

Imports Grasshopper.Kernel

Imports Grasshopper.Kernel.Types

Imports Rhino.Geometry

Imports System.IO

Imports System.Windows.Forms

Public Class GerillaAssemblies
	
	Inherits GH_Component
	
	'Constructor
	Public Sub New()
		
		MyBase.New("Gerilla Construction Assembly", "Construction Assembly", "Make Custom Gerilla Construction Assemblies", "Gerilla", "Materials")
		
	End Sub
	
	Public Overrides ReadOnly Property ComponentGuid As System.Guid
		
		Get
			Return New Guid("1431995A-8DA6-4812-97A6-BC02BB12B2F9") 
		End Get
		
	End Property
	
	Protected Overrides ReadOnly property Icon As System.drawing.Bitmap
		Get
			Return gerillaResource.GerillaIconAssembly
		End Get
	End Property
	
	Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
		
		pManager.AddTextParameter("Name", "Name", "Assembly Name", GH_ParamAccess.item, "Default Assembly")
		pManager.AddTextParameter("Material", "Material", "Material",  GH_ParamAccess.list, "Material")
		pManager.AddBooleanParameter("ProjectLibrary", "Save to Project Library", "Project Library", GH_ParamAccess.item, False)
		pManager.AddBooleanParameter("GerillaLibrary", "Save to Gerilla Library", "Gerilla Library", GH_ParamAccess.item, False)
		
	End Sub
	
	
	Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
		
		pManager.AddTextParameter("Assembly Name", "Assembly Name", "Assembly Name", GH_ParamAccess.list)
		pManager.AddTextParameter("Construction Assembly IDF", "IDF", "Construction Assembly For IDF Compiler", GH_ParamAccess.list)
		
	End Sub
	
	
	Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
		
		'Private Variables
		
		Dim myName As String = Nothing
		Dim myMaterial As New List (Of String)
		Dim myProjectDatabase As Boolean = Nothing
		Dim myGerillaDatabase As Boolean = Nothing 
		
		'Assign inputs to variables
		
		If (Not DA.GetData(0,myName)) Then Return
		If (Not DA.GetDataList(1,myMaterial)) Then Return
		If (Not DA.GetData(2,myProjectDatabase)) Then Return		
		If (Not DA.GetData(3,myGerillaDatabase)) Then Return
		
		'Perform node function
		
		'Assemble Construction
		Dim myAssemblyOutput As New List (Of String)
		
		myAssemblyOutput.Add("Construction,")
		myAssemblyOutput.Add(myName & "			!-Name")
		
		For i As Integer = 0 To myMaterial.Count - 1
			
			If i <> myMaterial.Count-1 Then
				
				myAssemblyOutput.Add(myMaterial(i) & ",			!-Layer " & i+1)
				
			Else 
				
				myAssemblyOutput.Add(myMaterial(i) & ";			!-Layer " & i+1)
				
			End If
			
		Next
		
		myAssemblyOutput.Add("")
		
		'Save to Project Library
		
		If myProjectDatabase = True Then
			
			If System.IO.File.Exists(GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Assemblies\" & myName & ".txt") = True Then
				
				Dim ProjectDatabaseMessageBox As DialogResult
				
				ProjectDatabaseMessageBox = MessageBox.Show _
												("This assembly already exists in the Project Library. Do you want to overwrite?", _
												 "Save Assembly to Project Library", _
												 MessageBoxButtons.YesNo)
				
				If ProjectDatabaseMessageBox = DialogResult.Yes Then
					Dim ObjectWriter As New System.IO.StreamWriter(GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Assemblies\" & myName & ".txt")
					For j As Integer = 0 To myAssemblyOutput.Count - 1
						ObjectWriter.WriteLine(myAssemblyOutput(j))	
					Next
					ObjectWriter.Close()
					MessageBox.Show("Assembly Updated")
				ElseIf ProjectDatabaseMessageBox = DialogResult.No Then
					MessageBox.Show("Assembly NOT Saved")
				End If
				
			Else
				
				Dim ObjectWriter As New System.IO.StreamWriter(GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\Assemblies\" & myName & ".txt")
				For j As Integer = 0 To myAssemblyOutput.Count - 1
					ObjectWriter.WriteLine(myAssemblyOutput(j))	
				Next
				ObjectWriter.Close()
				MessageBox.Show("Assembly Updated")
				
			End If
			
		End If
		
		
		'Save to Gerilla Library
		
		Dim GerillaAssemblyLibraryPath As String = "C:\geRILLA\Libraries\Assemblies\"
		
		If myGerillaDatabase = True Then
			
			If System.IO.File.Exists(GerillaAssemblyLibraryPath & myName & ".txt") = True Then
				
				Dim ProjectDatabaseMessageBox As DialogResult
				
				ProjectDatabaseMessageBox = MessageBox.Show _
												("This assembly already exists in the Gerilla Library. Do you want to overwrite?", _
												 "Save Assembly to Gerilla Library", _
												 MessageBoxButtons.YesNo)
				
				If ProjectDatabaseMessageBox = DialogResult.Yes Then
					'Save the Assembly to the Gerilla Database
					Dim GerillaObjectWriter As New System.IO.StreamWriter(GerillaAssemblyLibraryPath & myName & ".txt")
					For j As Integer = 0 To myAssemblyOutput.Count - 1
						GerillaObjectWriter.WriteLine(myAssemblyOutput(j))	
					Next	
					GerillaObjectWriter.Close()
					MessageBox.Show("Assembly Updated in Gerilla Library")
				ElseIf ProjectDatabaseMessageBox = DialogResult.No Then
					MessageBox.Show("Assembly NOT Saved")
				End If
				
			Else
				
				'Save the Assembly to the Gerilla Database
				Dim GerillaObjectWriter As New System.IO.StreamWriter(GerillaAssemblyLibraryPath & myName & ".txt")
				For j As Integer = 0 To myAssemblyOutput.Count - 1
					GerillaObjectWriter.WriteLine(myAssemblyOutput(j))	
				Next
				GerillaObjectWriter.Close()				
				
				MessageBox.Show("Material Saved")
				
			End If
			
		End If
		
		'Write the results to the output node
		DA.SetData(0, myName)
		DA.SetDataList(1, myAssemblyOutput)
		
    End Sub
    
    
End Class