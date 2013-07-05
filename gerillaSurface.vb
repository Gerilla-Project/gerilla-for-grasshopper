'
' Created by SharpDevelop.
' User: Ben
' Date: 11/28/2011
' Time: 10:30 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry

Public Class gerillaSurface
	
	' Compile and format inputs for surface IDF.
	Public Sub CompileSurfaceIDF(ByVal gerillaSurfaces As Data.GH_Structure(Of GH_Brep), surfaceConstruction As string, surfaceOutIDF As Grasshopper.DataTree(Of String))
		
		For i As Integer = 0 To gerillaSurfaces.Branches.Count-1
			Dim ghBrep As GH_Brep = gerillaSurfaces.Branch(i).Item(0)
			Dim mySurface As Brep = CType(ghBrep.Value, Brep)
			Dim surfacePath As New Data.GH_Path(gerillaSurfaces.Path(i))
			Dim surfaceType As String
			Dim outsideCondition As String
			Dim viewFactor As String
			Dim outsideObject As String
			Dim outsideSurfaceType As String
			Dim sunExposure As String
			Dim windExposure As String
			
			' Determine surface type.
			If surfacePath.Dimension(0) = 1 Then
				surfaceType = "Floor"
			Else If surfacePath.Dimension(0) = 2 Then
				surfaceType = "Wall"
			Else
				surfaceType = "Roof"
			End If
			
			' Determine outside condition.
			If surfacePath.Dimension(3) > 0 Then
				outsideCondition = "Surface"
				
				' Determine surface type of adjacent surface.
				If surfacePath.Dimension(3) = 1 Then
					outsideSurfaceType = "Floor"
				Else If surfacePath.Dimension(3) = 2 Then
					outsideSurfaceType = "Wall"
				Else
					outsideSurfaceType = "Roof"
				End If 
				
				outsideObject = "Zone" & surfacePath.Dimension(4).ToString & " " & outsideSurfaceType & surfacePath.Dimension(5)
				sunExposure = "NoSun"
				windExposure = "NoWind"
				viewFactor = "0.0"
				
			Else If surfaceType = "Floor" Then
				' Floors with no adjacent surfaces are on the ground.
				' NOTE: this will be incorrect for above ground zones with no zone beneath them, e.g. cantilevered spaces. - Brendan
				outsideCondition = "Ground"
				outsideObject = ""
				sunExposure = "NoSun"
				windExposure = "NoWind"
				viewFactor = "0.0"
			Else If surfaceType = "Wall"
				' Walls with no adjacent surfaces are exposed to outdoors.
				outsideCondition = "Outdoors"
				outsideObject = ""
				sunExposure = "SunExposed"
				windExposure = "WindExposed"
				viewFactor = "0.5"
			Else
				' Roofs with no adjacent surfaces are exposed to outdoors.
				outsideCondition = "Outdoors"
				outsideObject = ""
				sunExposure = "SunExposed"
				windExposure = "WindExposed"
				viewFactor = "0.0"
				
			End If
			
			' Compose the IDF information for the surface.
			Dim surfaceName As String = "Zone" & gerillaSurfaces.Path(i).Dimension(1).ToString & " " & surfaceType & gerillaSurfaces.Path(i).Dimension(2).ToString	 			
			
			surfaceOutIDF.Add("BuildingSurface:Detailed,", surfacePath)
			surfaceOutIDF.Add("     " & surfaceName & ",     !- Name",surfacePath)	
			surfaceOutIDF.Add("     " & surfaceType & ",     !- Surface Type", surfacePath)
			surfaceOutIDF.Add("     " & surfaceConstruction & ",     !- Construction Name",surfacePath)
			surfaceOutIDF.Add("     Zone" &  surfacePath.Dimension(1).ToString & ",     !- Zone Name",surfacePath)
			surfaceOutIDF.Add("     " & outsideCondition & ",     !- Outside Boundary Condition",surfacePath)
			surfaceOutIDF.Add("     " & outsideObject & ",     !- Outside Boundary Condition Object",surfacePath)
			surfaceOutIDF.Add("     " & sunExposure & ",     !- Sun Exposure", surfacePath)
			surfaceOutIDF.Add("     " & windExposure & ",     !- Wind Exposure", surfacePath)
			surfaceOutIDF.Add("     " & viewFactor & ",     !- View Factor to Ground",surfacePath)
			
			Dim surfaceVertices As Point3d() = mySurface.DuplicateVertices()
			
			
			surfaceOutIDF.Add("     " & surfaceVertices.Length & ",     !- Number of Vertices",surfacePath)
			
			' Compose the surface vertices.
			For k As Integer = 0 To surfaceVertices.Length-1
				Dim myPoint As Point3d = surfaceVertices(k)
				
				Dim xString As String
				Dim yString As String
				Dim zString As String
				Dim xyzString As String
				
				xString = myPoint.X.ToString
				yString = myPoint.Y.ToString
				zString = myPoint.Z.ToString
				
				If k = surfaceVertices.Length-1 Then
					xyzString = xString & ", " & yString & ", " & zString & "; "
				Else
					xyzString = xString & ", " & yString & ", " & zString & ", "
				End If
				
				surfaceOutIDF.Add(xyzString, surfacePath)
			Next
						
		Next
		
	End Sub
	
End Class
