'
' Created by SharpDevelop.
' User: Ben
' Date: 11/30/2011
' Time: 10:54 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry

Public Class ZoneSurface
	Inherits GH_Component
	
	' Constructor
	Public Sub New()
		MyBase.New("Gerilla Surface","Surface","Create Gerilla Surface IDF Entries","Gerilla","Zone")
	End Sub
	
	' Guid
	Public Overrides ReadOnly Property ComponentGuid As System.Guid
		Get
			Return New Guid ("CBF85842-E610-45D7-859E-E7FAA8D6EC86")
		End Get
	End Property
	
	' Icon
	Protected Overrides ReadOnly property icon As System.drawing.Bitmap
		Get
			Return gerillaResource.GerillaIconSurface
		End Get
	End Property
	
	' Inputs
	Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
		pManager.AddBrepParameter("Gerilla Surfaces","Surfaces","Gerilla Surfaces From Gerilla Zone Surface Output", GH_ParamAccess.tree)
		pManager.AddTextParameter("Assembly Name","Assembly Name","Construction Assembly Name From Construction Assembly Library", GH_ParamAccess.item, "LTWALL")
		pManager.AddIntegerParameter("Zone Index","Zone Index","Index For Gerilla Zones To Apply Assembly To", GH_ParamAccess.tree)
		pManager.AddIntegerParameter("Floor Index","Floor Index","Index For Gerilla Floors to Apply Assembly To", GH_ParamAccess.tree, 0)
		pManager.AddIntegerParameter("Wall Index","Wall Index","Index For Gerilla Walls to Apply Assembly To", GH_ParamAccess.tree, 0)
		pManager.AddIntegerParameter("Roof Index","Roof Index","Index For Gerilla Roofs to Apply Assembly To", GH_ParamAccess.tree, 0)
	End Sub
	
	' Outputs
	Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
		pManager.AddTextParameter("Gerilla Surface IDF","IDF","Gerilla Surface For IDF Compiler", GH_ParamAccess.item)
		pManager.AddBrepParameter("Floor Surfaces","Floor Surface","Gerilla Floor Surfaces", GH_ParamAccess.tree)
		pManager.AddBrepParameter("Wall Surfaces","Wall Surface","Gerilla Wall Surfaces", GH_ParamAccess.tree)
		pManager.AddBrepParameter("Roof Surfaces","Roof Surfaces","Gerilla Roof Surfaces", GH_ParamAccess.tree)
	End Sub
	
	Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
		Dim inSurfaces As New Data.GH_Structure(Of GH_Brep)
		
		Dim inConstruction As String = Nothing
		
		Dim zoneIndex As New Data.GH_Structure(Of GH_Integer)
		Dim zoneIndexList As New List(Of GH_Integer)
		
		Dim wallIndex As New data.GH_Structure(Of GH_Integer)
		Dim wallIndexList As New List(Of GH_Integer)
		Dim wallPathList As New List(Of Data.GH_Path)
		Dim selectedWalls As New Data.GH_Structure(Of GH_Brep)
		
		Dim floorIndex as new Data.GH_Structure(Of GH_Integer)
		Dim floorIndexList As New List(Of GH_Integer)
		Dim floorPathList As New List(Of Data.GH_Path)
		Dim selectedFloors As New Data.GH_Structure(Of GH_Brep)
		
		Dim roofIndex as new Data.GH_Structure(Of GH_Integer)
		Dim roofIndexList As New List(Of GH_Integer)
		Dim roofPathList As New List(Of Data.GH_Path)
		Dim selectedRoofs As New Data.GH_Structure(Of GH_Brep)
		
		If (Not DA.GetDataTree(0, inSurfaces)) Then Return
		If (Not DA.GetData(1, inConstruction)) Then Return
		If (Not DA.GetDataTree(2, zoneIndex)) Then Return
		DA.GetDataTree(3, floorIndex)
		DA.GetDataTree(4, wallIndex)
		DA.GetDataTree(5, roofIndex)
		
		' ZONES
		' -----
		' Put all zone indices into a list.
		' NOTE: In the example project, the zones are input as a simple list, so I'm not sure why this is written to dig through a tree. - Brendan
		For i As Integer=0 To zoneIndex.Branches.Count-1
			For j As Integer=0 To zoneIndex.Branch(i).Count-1
				zoneIndexList.Add(zoneIndex.Branch(i).Item(j))
			Next
		Next
		
		' WALLS
		' -----
		' Put all wall indices into a list.
		For i As Integer=0 To wallIndex.Branches.Count-1
			For j As Integer=0 To wallIndex.Branch(i).Count-1
				wallIndexList.Add(wallIndex.Branch(i).Item(j))
			Next
		Next
		
		' Compose selected wall paths and put them into a list.
		For i As Integer=0 To zoneIndexList.Count-1
			For j As Integer=0 To wallIndexList.Count-1
				Dim wallPath As New Data.GH_Path(2,zoneIndexList(i).Value,wallIndexList(j).Value)
				wallPathList.Add(wallPath)
			Next
		Next
		
		' Assign the wall breps the appropriate paths
		For i As Integer=0 To wallPathList.Count-1
			Dim selectWallIndex As New Data.GH_Path(wallPathList(i))
			
			For j As Integer=0 To insurfaces.PathCount-1
				Dim selectWall As GH_Brep = insurfaces.Branch(j).Item(0)
				Dim selectWallPath As New Data.GH_Path(insurfaces.Path(j))
				
				If selectWallIndex.Dimension(0) = selectWallPath.Dimension(0) Then
					If selectWallIndex.Dimension(1) = selectWallPath.Dimension(1) Then
						If selectWallIndex.Dimension(2) = selectWallPath.Dimension(2) Then
							selectedWalls.Append(selectWall,selectWallPath)
						End If
					End If
				End If
			Next
		Next
		
		' FLOORS
		' ------
		' Put all floor indices into a list.
		For i As Integer=0 To floorIndex.Branches.Count-1
			For j As Integer=0 To floorIndex.Branch(i).Count-1
				floorIndexList.Add(floorIndex.Branch(i).Item(j))
			Next
		Next
		
		' Compose selected floor paths and put them into a list.
		For i As Integer=0 To zoneIndexList.Count-1
			For j As Integer=0 To floorIndexList.Count-1
				Dim floorPath As New Data.GH_Path(1,zoneIndexList(i).Value,floorIndexList(j).Value)
				floorPathList.Add(floorPath)
			Next
		Next
		
		' Assign the floor breps the appropriate paths
		For i As Integer=0 To floorPathList.Count-1
			Dim selectFloorIndex As New Data.GH_Path(floorPathList(i))
			
			For j As Integer=0 To inSurfaces.PathCount-1
				Dim selectFloor As GH_Brep = inSurfaces.Branch(j).Item(0)
				Dim selectFloorPath As New Data.GH_Path(inSurfaces.Path(j))
				
				If selectFloorIndex.Dimension(0) = selectFloorPath.Dimension(0) Then
					If selectFloorIndex.Dimension(1) = selectFloorPath.Dimension(1) Then
						If selectFloorIndex.Dimension(2) = selectFloorPath.Dimension(2) Then
							selectedFloors.Append(selectFloor,selectFloorPath)	
						End If
					End If
				End If
			Next
		Next
		
		
		' ROOFS
		' -----
		' Put all floor indices into a list.
		For i As Integer=0 To roofIndex.Branches.Count-1
			For j As Integer=0 To roofIndex.Branch(i).Count-1
				roofIndexList.Add(roofIndex.Branch(i).Item(j))
			Next
		Next
		
		' Compose selected wall paths and put them into a list.
		For i As Integer=0 To zoneIndexList.Count-1
			For j As Integer=0 To roofIndexList.Count-1
				Dim roofPath As New Data.GH_Path(3,zoneIndexList(i).Value,roofIndexList(j).Value)
				roofPathList.Add(roofPath)
			Next
		Next
		
		' Assign the roof breps the appropriate paths
		For i As Integer=0 To roofPathList.Count-1
			Dim selectRoofIndex As New Data.GH_Path(roofPathList(i))
			
			For j As Integer=0 To inSurfaces.PathCount-1
				Dim selectRoof As GH_Brep = inSurfaces.Branch(j).Item(0)
				Dim selectRoofPath As New Data.GH_Path(inSurfaces.Path(j))
				
				If selectRoofIndex.Dimension(0) = selectRoofPath.Dimension(0) Then
					If selectRoofIndex.Dimension(1) = selectRoofPath.Dimension(1) Then
						If selectRoofIndex.Dimension(2) = selectRoofPath.Dimension(2) Then
							selectedRoofs.Append(selectRoof,selectRoofPath)
						End If
					End If
				End If
			Next
		Next
		
		' Create the IDF entries for the surfaces.
		Dim wallIDFOut As New Grasshopper.DataTree(Of String)
		
		Dim gerillaWall As new gerillaSurface
		gerillaWall.CompileSurfaceIDF(selectedWalls,inConstruction,wallIDFOut)
		
		Dim gerillaFloor As New gerillaSurface
		gerillaFloor.CompileSurfaceIDF(selectedFloors, inConstruction,wallIDFOut)
		
		Dim gerillaRoof As New gerillaSurface
		gerillaRoof.CompileSurfaceIDF(selectedRoofs, inConstruction,wallIDFOut)
		
		' Set the GH component output data.
		DA.SetDataTree(0, wallIDFOut)
		DA.SetDataTree(1, selectedFloors)
		DA.SetDataTree(2, selectedWalls)
		DA.SetDataTree(3, selectedRoofs)
		
	End Sub
	
End Class
