'
' Created by SharpDevelop.
' User: Ben
' Date: 11/17/2011
' Time: 8:43 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry

Public Class Zone
     Inherits GH_Component
     
    'Constructor
    Public Sub New()
        MyBase.New("Gerilla Zone","Zone","Create Gerilla Zone IDF Entries And Sort Zone Surfaces","Gerilla","Zone")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
Return New Guid ("3B13C426-3E3B-4283-A07E-6568119AB1AC")
        End Get
    End Property
    
    Protected Overrides ReadOnly property icon As System.drawing.Bitmap
         Get
            Return gerillaResource.GerillaIconZone   
         End Get
    End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddBrepParameter("Gerilla Zones","Zone","Zones As Closed Breps For EnergyPlus Simulation", GH_ParamAccess.tree)
        pManager.AddVectorParameter("North Vector","North Vector","North Vector For Project", GH_ParamAccess.item, Vector3d.YAxis)
    End Sub
    
    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pmanager.AddTextParameter("Gerilla Zone IDF","IDF","Gerilla Zone For IDF Compiler", GH_ParamAccess.item)
        'pManager.AddBrepParameter("Zone Floors","F","Zone Floors") 
        'pManager.AddBrepParameter("Zone Roofs", "R", "Zone Roofs")
        'pManager.AddBrepParameter("Zone Walls", "W", "Zone Walls")
        pManager.AddBrepParameter("Zone Surfaces", "Zone Surfaces", "Gerilla Zone Surfaces For Gerilla Surface Component For Connstruction Assembly Assignment", GH_ParamAccess.tree)
        'pManager.Register_GeometryParam("BrepFaces", "BF", "BrepFaces")
        
    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        
        'input data containers
        Dim inZones As New Data.GH_Structure(Of GH_Brep)
         Dim northVector As Vector3d
        
        'retrieve input data and place in data containers
        If (Not DA.GetDataTree(0, inZones)) Then Return
        If (Not DA.GetData(1, northVector)) Then Return
        
        'output data containers
        Dim outSurfaces As New Grasshopper.DataTree(Of Brep)
        Dim outZones As New Grasshopper.DataTree(Of Brep)
        Dim outFloor As New Grasshopper.DataTree(Of Brep)
        Dim outRoof As New Grasshopper.DataTree(Of Brep)
        Dim outWalls As New Grasshopper.DataTree(Of Brep)
        'Dim outZones As New list(Of Brep)
        Dim outIDF As New Grasshopper.DataTree(Of String)
        Dim relativeNorth As Double=Vector3d.VectorAngle(northVector, Vector3d.YAxis, Plane.WorldXY)
        relativeNorth = (relativeNorth*180)/Math.PI
        
        Dim outBrepFaces As New List(Of BrepFace)
        'Dim faceGeometryBase As GeometryBase
       
        
        'give each brep face a unique branch path
        'the first dimension signifies the zone #, the second dimension signifies face #
        Dim zoneID As Integer = 1
        For i As Integer=0 To inZones.Branches.Count-1
            
            For j As Integer=0 To inZones.Branch(i).Count-1
                
                Dim myBrep As GH_Brep = inZones.Branch(i).Item(j)
                Dim rhinoBrep As Brep = CType(myBrep.Value, Brep)
                
                
                Dim zonePath As New Data.GH_Path(zoneID)
                outZones.Add(rhinoBrep,zonePath)
                
                Dim myBrepFaces As New List(Of BrepFace)(rhinoBrep.Faces)
                For b As Integer = 0 To myBrepFaces.Count-1
                    
                    
                    
                outBrepFaces.Add(myBrepFaces(b))
                
                Next
                
                Dim floorID As Integer=1
                Dim wallID As Integer=1
                Dim roofID As Integer=1
                
                 For f As Integer=0 To myBrepFaces.Count-1
                    

                    Dim zoneSurface As brep = myBrepFaces(f).ToBrep
                    Dim normalAngle As Double = Math.Abs(Vector3d.VectorAngle(zoneSurface.Faces(0).NormalAt(0,0),Vector3d.ZAxis))
                    Dim vectorPath As New Data.GH_Path(f)

                    If normalAngle > 3.05 Then    
                        Dim floorPath As New Data.GH_Path(1, zoneID, floorID)
                        'Dim floorSurface As Brep = zoneSurface.Flip 
                        
                        outfloor.Add(zoneSurface, floorPath)
                        floorID = floorID + 1
                        
                    ElseIf normalAngle >= 0.785 Then
                        
                        Dim wallPath As New Data.GH_Path(2, zoneID, wallID)
                        'Dim wallSurface As Brep = zoneSurface.
                        outWalls.Add(zoneSurface,wallPath)
                        wallID = wallID + 1
                        
                    Else                   
                        Dim roofPath As New Data.GH_Path(3, zoneID, roofID)
                        outRoof.Add(zoneSurface, roofPath)
                        roofID = roofID + 1
                           
                       End if
                    
                Next f
                zoneID = zoneID + 1
            Next j
        Next i
        
        
        For i As Integer=0 To outZones.BranchCount-1
            Dim idfPath As New Data.GH_Path(outZones.Path(i))
            
            
            Dim zoneVolProp As VolumeMassProperties = Rhino.Geometry.VolumeMassProperties.Compute(outZones.Branch(i).Item(0))
            Dim roofAreaProp As AreaMassProperties = Rhino.Geometry.AreaMassProperties.Compute(outRoof.Branch(i).Item(0))
            
            Dim floorPath As New Data.GH_Path(1, i+1, 1)
            Dim zoneFloor As Brep = outFloor.Branch(floorPath).Item(0)
            Dim floorVertices As Point3d() = zoneFloor.DuplicateVertices()
            
        
            'outIDF.Add("!-   ===========  ALL OBJECTS IN CLASS: ZONE ===========",idfPath)
             outIDF.Add("",idfPath)
             outIDF.Add("Zone,",idfPath)
             outIDF.Add("  Zone" & (i+1).ToString & ",        !- Name",idfPath)
             outIDF.Add("  " & relativeNorth.ToString & ",        !- Direction of Relative North {deg}",idfPath)
             outIDF.Add("  " & floorVertices(0).X.ToString & ",        !- X Origin {m}",idfPath)
             outIDF.Add("  " & floorVertices(0).Y.ToString & ",        !- Y Origin {m}",idfPath)
             outIDF.Add("  " & floorVertices(0).Z.ToString & ",        !- Z Origin {m}",idfPath)
             outIDF.Add("  1,        !- Type",idfPath)
             outIDF.Add("  1,        !- Multiplier",idfPath)
             outIDF.Add("  " & roofAreaProp.Centroid.Z.ToString &  ",        !- Ceiling Height {m}",idfPath)
             outIDF.Add("  " & zoneVolProp.Volume & ";        !- Volume {m3}",idfPath)
             outIDF.Add("",idfPath)
             
        Next
        
        'test and tag floor to roof adjacencies
        For i As Integer=0 To outfloor.BranchCount-1
            
            Dim intCurves As Curve()
            Dim intPoints As Point3d()
            Dim testFloor As Brep = outfloor.Branch(i).Item(0)
            Dim joinedIntCurves As Curve()
            
            'perform Brep to Brep between each floor and each roof
            'if intersection results in a closed curve, then they are adjacent    
            For j As Integer = 0 To outRoof.BranchCount-1
                Dim testRoof As Brep = outRoof.Branch(j).Item(0)

                Rhino.Geometry.Intersect.Intersection.BrepBrep(testFloor, testRoof, 0.001, intCurves, intPoints)
                joinedIntCurves = Rhino.Geometry.Curve.JoinCurves(intCurves)
                
                Dim pathFloor As New Data.GH_Path
                Dim pathRoof As New Data.GH_Path
                
                
                For k As Integer = 0 To joinedIntCurves.Length-1
                    If joinedIntCurves(k).IsClosed = True Then
                        
                        'append adjacent roof path dimensions to floor path
                        pathFloor = outfloor.Path(i).AppendElement(outRoof.Path(j).Dimension(0))
                        pathFloor = pathFloor.AppendElement(outRoof.Path(j).Dimension(1))
                        pathFloor = pathFloor.AppendElement(outRoof.Path(j).Dimension(2))
                        
                        'append adjacent floor path dimensions to roof path
                        pathRoof = outRoof.Path(j).AppendElement(outfloor.Path(i).Dimension(0))
                        pathRoof = pathRoof.AppendElement(outfloor.Path(i).Dimension(1))
                        pathRoof = pathRoof.AppendElement(outfloor.Path(i).Dimension(2))
                        
                        'remove floor and roof with old paths and re-add with new adjacency tagged paths
                        outFloor.RemovePath(outFloor.Path(i))
                        outFloor.Add(testFloor,pathFloor)
                        outRoof.RemovePath(outRoof.Path(j))
                        outRoof.Add(testRoof, pathRoof)
                    End If
                Next k
            Next j
            
            'after testing the ith floor against all of the roofs 
            'add the null 0,0,0 suffix to the ith floor if there are no adjacencies
            If outFloor.Path(i).Length = 3 Then
                    Dim nullPath As New Data.GH_Path
                    nullPath = outfloor.Path(i).AppendElement(0)
                    nullPath = nullPath.AppendElement(0)
                    nullPath = nullPath.AppendElement(0)
                    outFloor.RemovePath(outFloor.Path(i))
                    outFloor.Add(testfloor,nullPath)    
            End if        
                
        Next i
        
            'after testing all of the floors against all of the roofs 
            'add the null 0,0,0 suffix to roofs without adjacencies
            For r As Integer = 0 To outRoof.BranchCount-1
                If outRoof.Path(r).Length = 3 Then
                    Dim nullPath As New Data.GH_Path
                    Dim tempRoof As Brep = outRoof.Branch(r).Item(0)
                    
                    nullPath = outRoof.Path(r).AppendElement(0)
                    nullPath = nullPath.AppendElement(0)
                    nullPath = nullPath.AppendElement(0)
                    outRoof.RemovePath(outroof.Path(r))
                    outRoof.Add(tempRoof,nullPath)    
                    End If
               Next
    
    For i As Integer = 0 To outWalls.BranchCount-1
        Dim wallIntCurves As Curve()
        Dim wallIntPts As Point3d()
        Dim joinedIntWallCurve As Curve()
        Dim testWall As Brep = outWalls.Branch(i).Item(0)
        
        
        For j As Integer = i+1 To outWalls.BranchCount-1
            Dim adjacentWall As Brep = outWalls.Branch(j).Item(0)
            
            Dim testWallPath As New Data.GH_Path
            Dim adjacentWallPath As New Data.GH_Path
            
            Rhino.Geometry.Intersect.Intersection.BrepBrep(testWall,adjacentWall,0.001,wallIntCurves,wallIntPts)
            joinedIntWallCurve = Rhino.Geometry.Curve.JoinCurves(wallIntCurves)
            
            For k As Integer=0 To joinedIntWallCurve.Length-1
                If joinedIntWallCurve(k).IsClosed = True Then
                    
                    'append adjacent wall path dimensions to test wall path 
                    testWallPath = outWalls.Path(i).AppendElement(outWalls.Path(j).Dimension(0))
                    testWallPath = testWallPath.AppendElement(outWalls.Path(j).Dimension(1))
                    testWallPath = testWallPath.AppendElement(outWalls.Path(j).Dimension(2))
                    
                    'append test wall path dimensions to adjacent wall path 
                    adjacentWallPath = outWalls.Path(j).AppendElement(outWalls.Path(i).Dimension(0))
                    adjacentWallPath = adjacentWallPath.AppendElement(outWalls.Path(i).Dimension(1))
                    adjacentWallPath = adjacentWallPath.AppendElement(outWalls.Path(i).Dimension(2))
                    
                    'remove wall surfaces with old paths and re-add with new adjacency tagged paths
                    outWalls.RemovePath(outWalls.Path(i))
                    outWalls.Add(testWall, testWallPath)
                    outwalls.RemovePath(outWalls.Path(j))
                    outWalls.Add(adjacentWall, adjacentWallPath)
                    
                End If
            Next k
            
        Next j
        If outWalls.Path(i).Length = 3 Then
            Dim nullPath As New Data.GH_Path()
            nullPath = outWalls.Path(i).AppendElement(0)
            nullPath = nullPath.AppendElement(0)
            nullPath = nullPath.AppendElement(0)
            
            outWalls.RemovePath(outWalls.Path(i))
            outWalls.Add(testWall, nullPath)
        End If
        
        Next i
        
        For i As Integer=0 To outFloor.BranchCount-1
            Dim floorPath As New Data.GH_Path(outFloor.Path(i))
            outSurfaces.Add(outFloor.Branch(i).Item(0),floorPath)
        Next
        
        For i As Integer=0 To outWalls.BranchCount-1
            Dim wallPath As New Data.GH_Path(outWalls.Path(i))
            outSurfaces.Add(outWalls.Branch(i).Item(0),wallPath)
        Next
        
        For i As Integer=0 To outRoof.BranchCount-1
            Dim roofPath As New Data.GH_Path(outRoof.Path(i))
            outSurfaces.Add(outRoof.Branch(i).Item(0), roofPath)
        Next

         
         DA.SetDataTree(0, outIDF)
        'DA.SetDataTree(1, outFloor)
        'DA.SetDataTree(2, outRoof)
        'DA.SetDataTree(3, outWalls)
        DA.SetDataTree(1, outSurfaces)
        'DA.SetDataTree(5, outZones)
        'DA.SetDataList(2, outBrepFaces)


    End Sub

End Class
