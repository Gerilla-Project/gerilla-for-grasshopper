'
' Created by SharpDevelop.
' User: Ben
' Date: 11/8/2011
' Time: 4:09 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry

Public Class GerillaWindow
	Inherits GH_Component	
		
	''Constructor
    Public Sub New()
    	MyBase.New("Gerilla Window","Window","Create Gerilla Window IDF Entries", "Gerilla","Zone")
    End Sub
 
    Public Overrides ReadOnly Property ComponentGuid As System.Guid
    	Get
    		Return New Guid("4CBD1018-395B-4F04-BDAA-10C6BC7C376A")  ''www.createguid.com
 
        End Get
    End Property
    
    Protected Overrides ReadOnly property icon As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconWindow  
 	    End Get
	End Property
    
    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
    	pManager.AddBrepParameter("Window Surface Reference", "Surface Reference","Gerilla Surface That Window Is On", GH_ParamAccess.tree)
    	pManager.AddBrepParameter("Window Brep","Window", "Window Brep", GH_ParamAccess.tree)
    	pManager.AddTextParameter("Assembly Name","Assembly Name", "Construction Assembly Name From Assembly Library", GH_ParamAccess.item)
    	
    End Sub
 
    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
    	pManager.AddTextParameter("Gerilla Window IDF", "IDF", "Gerilla Window For IDF Compiler", GH_ParamAccess.item)

    End Sub
 
 	Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
 		
 		Dim inSurfaces As New Data.GH_Structure(Of GH_Brep)
 		Dim inWindows As New Data.GH_Structure(Of GH_Brep)
 		Dim inConstruction As String = Nothing
 		
 		Dim gerillaWindowList As New List(Of Brep)
 		

	 	''Retrieve Input Data
	 	If (Not DA.GetDataTree(0, inSurfaces)) Then Return
	 	If (Not DA.GetDataTree(1, inWindows)) Then Return
	 	If (Not DA.GetData(2, inConstruction)) Then Return
	 	
	 	Dim windowIDFOut As New Grasshopper.DataTree(Of String)
	 	Dim intCurveOut As New Grasshopper.DataTree(Of Curve)
	 	Dim testWindowOut As New Grasshopper.DataTree(Of Brep)
	 	
	 	For i As Integer=0 To inWindows.Branches.Count-1
        	
        	For j As Integer=0 To inWindows.Branch(i).Count-1
        		
        		Dim ghBrep As GH_Brep = inWindows.Branch(i).Item(j)
        		Dim windowBrep As Brep = CType(ghBrep.Value, Brep)
        		
        		'If windowBrep.
        		'AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Window Brep not planar!")
        		
        		gerillaWindowList.Add(windowBrep)
        		
        	Next J
	 	Next i
	 	
	 	For i As Integer=0 To gerillaWindowList.Count-1
	 		Dim testWindow As Brep = gerillaWindowList(i)
	 		
	 		'extrude window surface and then move back half of the vector to get test window box 
	 		'with thickness on either side of original window surface for the BrepBrep intersection
	 		Dim windowVector As Vector3d = Rhino.Geometry.Vector3d.Divide(testWindow.Faces(0).NormalAt(0,0), 10)
	 		Dim windowCurves As Curve() = testWindow.DuplicateEdgeCurves()
	 		Dim joinedWindowCurve As Curve() = Curve.JoinCurves(windowCurves)
	 		Dim testWindowExtrude As Surface = Rhino.Geometry.Surface.CreateExtrusion(joinedWindowCurve(0),windowVector)
	 		Dim moveWindowExtrude As Vector3d = Rhino.Geometry.Vector3d.Multiply(windowVector, -.5)
	 		testWindowExtrude.Translate(moveWindowExtrude)
	 		Dim testWindowBox As Brep = testWindowExtrude.ToBrep
	 		
	 		For j As Integer=0 To inSurfaces.Branches.Count-1
	 			Dim testGHBrep As GH_Brep = inSurfaces.Branch(j).Item(0)
	 			Dim testSurface As Brep = CType(testGHBrep.Value, Brep)
	 			Dim surfacePath As New Data.GH_Path(inSurfaces.Path(j))
	 			
	 			Dim zoneSurfaceType As String
	 			
	 			Dim intCurve As Curve()
	 			Dim intPoint As Point3d()
	 			Dim joinedIntCurve As Curve()
	 			
	 			'BrepBrep intersection and then join intCurves to see if the test window is on the test surface 			
	 			Rhino.Geometry.Intersect.Intersection.BrepBrep(testWindowBox,testSurface, 0.001, intCurve,intPoint)
	 			
	 			joinedIntCurve = Curve.JoinCurves(intCurve)
	 			For c As Integer = 0 To intCurve.Length-1
	 	
	 			Next
	 			
	 			For k As Integer=0 To joinedIntCurve.Length-1
	 				If joinedIntCurve(k).IsClosed = True Then
	 					intCurveOut.Add(joinedIntCurve(k),surfacePath)
	 					testWindowOut.Add(testWindowBox, surfacePath)	 					
		 				Dim surfaceVector As Vector3d = testSurface.Faces(0).NormalAt(0,0)
		 				
		 				If windowVector <> surfaceVector Then
		 					testWindow.Flip
		 				End If
		 				
		 				Dim windowVertices As Point3d()
		 				windowVertices=testWindow.DuplicateVertices()
		 				
	 					If surfacePath.Dimension(0) = 1 Then
	 						zoneSurfaceType = "Floor"
	 					Else If surfacePath.Dimension(0) = 2 Then
	 						zoneSurfaceType = "Wall"
	 					Else
	 						zoneSurfaceType = "Roof"
	 					End If
	 					
	 					windowIDFOut.Add("FenestrationSurface:Detailed,",surfacePath)
		 				windowIDFOut.Add("  Z" & surfacePath.Dimension(1).ToString & zoneSurfaceType.Chars(0).ToString & surfacePath.Dimension(2).ToString & " Window" & intCurveOut.Branch(surfacePath).Count.ToString & ",     !- Name",surfacePath)
		 				windowIDFOut.Add("  Window,     !- Surface Type",surfacePath)
		 				windowIDFOut.Add("  " & inConstruction & ",     !- Construction Name", surfacePath)
		 				windowIDFOut.Add("  Zone" & surfacePath.Dimension(1) & " " & zoneSurfaceType & surfacePath.Dimension(2) & ",     !- Building Surface Name",surfacePath)
		 				windowIDFOut.Add("  ,     !- Outside Boundary Condition Object",surfacePath)
		 				windowIDFOut.Add("  autocalculate,     !- View Factor to Ground",surfacePath)
		 				windowIDFOut.Add("  ,     !- Shading Control Name",surfacePath)
		 				windowIDFOut.Add("  1,     !- Multiplier",surfacePath)
		 				windowIDFOut.Add("  " & windowVertices.Length & ",     !- Number of Vertices", surfacePath)
		 				
		 				For v As Integer = 0 To windowVertices.Length-1
		 					Dim myPoint As Point3d = windowVertices(v)
		 		
		 					Dim xString As String
	 						Dim yString As String
	 						Dim zString As String
	 						Dim xyzString As String
	 			
	 						xString = myPoint.X.ToString
	 						yString = myPoint.Y.ToString
	 						zString = myPoint.Z.ToString
	 			
	 						If k = windowVertices.Length-1 Then
	 							xyzString = xString & ", " & yString & ", " & zString & "; "
	 						Else
	 							xyzString = xString & ", " & yString & ", " & zString & ", "
	 						End If
	 			
	 						windowIDFOut.Add(xyzString, surfacePath)
		 				Next v
		 			windowIDFOut.Add(" ", surfacePath)	
	 				End If
	 				
	 			Next k
	 				 			
	 		Next j
	 		
	 	Next i
	 	
	 	DA.SetDataTree(0, windowIDFOut)

	 	End Sub
End Class




'FenestrationSurface:Detailed,
'    EAST WINDOW,             !- Name
'    Window,                  !- Surface Type
'    DOUBLE PANE WINDOW,      !- Construction Name
'    ZONE SURFACE EAST,       !- Building Surface Name
'    ,                        !- Outside Boundary Condition Object
'    autocalculate,           !- View Factor to Ground
'    ,                        !- Shading Control Name
'    ,                        !- Frame and Divider Name
'    1,                       !- Multiplier
'    4,                       !- Number of Vertices
'    8,1.5,2.35,  !- X,Y,Z ==> Vertex 1 {m}
'    8,1.5,0.35,  !- X,Y,Z ==> Vertex 2 {m}
'    8,4.5,0.35,  !- X,Y,Z ==> Vertex 3 {m}
'    8,4.5,2.35;  !- X,Y,Z ==> Vertex 4 {m}