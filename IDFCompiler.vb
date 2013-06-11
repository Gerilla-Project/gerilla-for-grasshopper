'
' Created by SharpDevelop.
' User: Ben
' Date: 11/16/2011
' Time: 9:22 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry

Public Class IDFCompiler
	Inherits GH_Component
     
    'Constructor
    Public Sub New()
        MyBase.New("Gerilla IDF Compiler","IDF Compiler","Compiles Gerilla IDF File For EnergyPlus","Gerilla","Simulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
Return New Guid ("90733B82-AFED-46FC-9CA3-DFE255A25E95")
        End Get
    End Property
    
    Protected Overrides ReadOnly property internal_icon_24x24 As System.drawing.Bitmap
 		Get
    		Return gerillaResource.ge_Icon_Red
 	    End Get
	End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddBooleanParameter("Write IDF","Write","Write IDF file", GH_ParamAccess.item)
        'pManager.AddTextParameter("SiteData","Site Data","Site Data", "default", GH_ParamAccess.list)
        pManager.AddTextParameter("Zone IDF","Zone", "Zone IDF Entries", GH_ParamAccess.tree)
        pManager.AddTextParameter("Surface IDF","Surface", "Surface IDF Entries",GH_ParamAccess.tree)
'        pManager.AddTextParameter("Material IDF","Material", "Material IDF Entries", "default materials", GH_ParamAccess.tree)
        pManager.AddTextParameter("Assembly IDF","Assembly", "Assembly IDF Entries", GH_ParamAccess.tree, "default constructions")
        pManager.AddTextParameter("Run Period IDF","Run Period", "Run Period IDF Entry", GH_ParamAccess.list, "default: full year simulation")
        'pManager.AddTextParameter("FileName","FileName", "File Name for IDF",GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.AddBooleanParameter("EnergyPlus Simulation Trigger","Simulate","EnergyPlus Simulation Trigger", GH_ParamAccess.item)
        pmanager.AddTextParameter("IDF File","IDF","IDF File", GH_ParamAccess.item)
        'pManager.AddTextParameter("Materials","M","M")
    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
    	
    	
        'Get data from GH inputs
        Dim writeIDF As New Boolean
        DA.GetData(0,WriteIDF)
        
'        Dim siteData As New List(Of String)
'        DA.GetDataList(1,SiteData)
        
        Dim gerillaZones As New Data.GH_Structure(Of GH_String)
        DA.GetDataTree(1,gerillaZones)
        
        Dim gerillaSurfaces As New Data.GH_Structure(Of gh_String)
        DA.GetDataTree(2,gerillaSurfaces)
        
'        Dim gerillaMaterials As New Data.GH_Structure(Of gh_String)
'        DA.GetDataTree(3,gerillaMaterials)
        
        Dim gerillaConstructions As New Data.GH_Structure(Of gh_String)
        DA.GetDatatree(3,gerillaConstructions)
        
        Dim runPeriod As New List(Of String)
        DA.GetDataList(4,runPeriod)
        
'        Dim fileName As String = Nothing
'        DA.GetData(7,fileName)
'        
    	'Construct IDF file replacing header sections on gerillaIDFTemplate.idf file
       	Dim i As Integer
       	'Dim j As Integer
    	'Dim output As New List(Of String)
    
    	Dim idfBaseFile As String = My.Computer.FileSystem.ReadAllText("C:\Gerilla\gerillaIDFTemplate.idf")
    	Dim materialList As New List(Of String)
'    	If siteData(0) = "default" Then
'    	else
'    
'    	For i = 0 To siteData.Count - 1
'    	  output.Add(siteData(i))
'  		Next
'    	End If

'ZoneHVAC:EquipmentConnections,
'    gerillaZones,                !- Zone Name
'    gerillaZones Equipment,      !- Zone Conditioning Equipment List Name
'    gerillaZones Supply Inlet,   !- Zone Air Inlet Node or NodeList Name
'    ,                        !- Zone Air Exhaust Node or NodeList Name
'    gerillaZones Zone Air Node,  !- Zone Air Node Name
'    gerillaZones Return Outlet;  !- Zone Return Air Node Name
'ZoneHVAC:EquipmentList,
'    gerillaZones Equipment,      !- Name
'    ZoneHVAC:IdealLoadsAirSystem,  !- Zone Equipment 1 Object Type
'    gerillaZones Purchased Air,  !- Zone Equipment 1 Name
'    1,                       !- Zone Equipment 1 Cooling Sequence
'    1;                       !- Zone Equipment 1 Heating or No-Load Sequence

		For i = 0 To gerillaZones.PathCount-1
			'if last zone in list, use ";" at end of line instead of ","
    		If i = gerillaZones.PathCount-1 Then
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneList!!!!!", "Zone"&gerillaZones.Path(i).Dimension(0).ToString & ";" & vbNewLine & "!!!!!ZoneList!!!!!")
    			
    			Else
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneList!!!!!", "Zone"&gerillaZones.Path(i).Dimension(0).ToString & "," & vbNewLine & "!!!!!ZoneList!!!!!")
    			End If
    			
    			'ZoneHVAC:EquipmentConnections IDF
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "ZoneHVAC:EquipmentConnections," & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & ",		!- Zone Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " Equipment,      !- Zone Conditioning Equipment List Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " INLETS,   !- Zone Air Inlet Node or NodeList Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Zone Air Exhaust Node or NodeList Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " Zone Air Node,  !- Zone Air Node Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " Return Outlet;  !- Zone Return Air Node Name" & vbNewLine & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "ZoneHVAC:EquipmentList," & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " Equipment,      !- Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "ZoneHVAC:IdealLoadsAirSystem,  !- Zone Equipment 1 Object Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " Ideal Loads,  !- Zone Equipment 1 Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "1,                       !- Zone Equipment 1 Cooling Sequence" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
    			idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "1;                       !- Zone Equipment 1 Heating or No-Load Sequence" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				
'				ZoneHVAC:EquipmentList,
'    ZONE 1 EQUIPMENT,        !- Name
'    ZoneHVAC:IdealLoadsAirSystem,  !- Zone Equipment 1 Object Type
'    ZONE 1 Ideal Loads,      !- Zone Equipment 1 Name
'    1,                       !- Zone Equipment 1 Cooling Sequence
'    1;                       !- Zone Equipment 1 Heating or No-Load Sequence
		
				
'				  ZoneHVAC:IdealLoadsAirSystem,
'    ZONE 1 Ideal Loads,      !- Name
'    ZONE 1 INLETS,           !- Zone Supply Air Node Name
'    50,                      !- Heating Supply Air Temperature {C}
'    13,                      !- Cooling Supply Air Temperature {C}
'    0.015,                   !- Heating Supply Air Humidity Ratio {kg-H2O/kg-air}
'    0.010,                   !- Cooling Supply Air Humidity Ratio {kg-H2O/kg-air}
'    NoLimit,                 !- Heating Limit
'    autosize,                !- Maximum Heating Air Flow Rate {m3/s}
'    NoLimit,                 !- Cooling Limit
'    autosize,                !- Maximum Cooling Air Flow Rate {m3/s}
'    NoOutdoorAir,            !- Outdoor Air
'    0.0;                     !- Outdoor Air Flow Rate {m3/s}

'  NodeList,
'    ZONE 1 INLETS,           !- Name
'    ZONE 1 INLET;            !- Node 1 Name
				''''EPlus V6.0
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "ZoneHVAC:IdealLoadsAirSystem,  !- Zone Equipment 1 Object Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " Ideal Loads,  !- Zone Equipment 1 Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " INLETS,   !- Zone Supply Air Node Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "50,                      !- Heating Supply Air Temperature {C}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "13,                      !- Cooling Supply Air Temperature {C}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "0.015,                   !- Heating Supply Air Humidity Ratio {kg-H2O/kg-air}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "0.01,                    !- Cooling Supply Air Humidity Ratio {kg-H2O/kg-air}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "NoLimit,                 !- Heating Limit" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "autosize,                !- Maximum Heating Air Flow Rate {m3/s}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "NoLimit,                 !- Cooling Limit" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "autosize,                !- Maximum Cooling Air Flow Rate {m3/s}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "NoOutdoorAir,            !- Outdoor Air" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "0.0;                     !- Outdoor Air Flow Rate {m3/s}" & vbNewLine & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "NodeList," & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " INLETS,           !- Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
'				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " INLET;            !- Node 1 Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				
				
				
				''''EPlus V7.0
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "ZoneHVAC:IdealLoadsAirSystem,  !- Zone Equipment 1 Object Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " Ideal Loads,  !- Zone Equipment 1 Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                       !- Availability Schedule Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString &" Inlet,   !- Zone Supply Air Node Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Zone Exhaust Air Node Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")				
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "50,                      !- Heating Supply Air Temperature {C}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "13,                      !- Cooling Supply Air Temperature {C}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "0.015,                   !- Heating Supply Air Humidity Ratio {kg-H2O/kg-air}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "0.009,                    !- Cooling Supply Air Humidity Ratio {kg-H2O/kg-air}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "NoLimit,                 !- Heating Limit" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "autosize,                !- Maximum Heating Air Flow Rate {m3/s}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Maximum Sensible Heating Capacity {W}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "NoLimit,                 !- Cooling Limit" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "autosize,                !- Maximum Cooling Air Flow Rate {m3/s}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Maximum Total Cooling Capacity {W}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Heating Availability Schedule Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Cooling Availability Schedule Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "ConstantSupplyHumidityRatio,  !- Dehumidification Control Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Cooling Sensible Heat Ratio {dimensionless}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "ConstantSupplyHumidityRatio,  !- Humidification Control Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Design Specification Outdoor Air Object Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Outdoor Air Inlet Node Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Demand Controlled Ventilation Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Outdoor Air Economizer Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Heat Recovery Type" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ",                        !- Sensible Heat Recovery Effectiveness {dimensionless}" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", ";                        !- Latent Heat Recovery Effectiveness {dimensionless}" & vbNewLine & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				'Copied below from v6 stuff
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "NodeList," & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " INLETS,           !- Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				idfBaseFile = Replace(idfBaseFile, "!!!!!ZoneEquipConnections!!!!!", "Zone" & gerillaZones.Path(i).Dimension(0).ToString & " INLET;            !- Node 1 Name" & vbNewLine & "!!!!!ZoneEquipConnections!!!!!")
				
'ZoneHVAC:IdealLoadsAirSystem,
'    ZONE ONE Purchased Air,  !- Name
'    ,                        !- Availability Schedule Name
'    ZONE ONE Supply Inlet,   !- Zone Supply Air Node Name
'    ,                        !- Zone Exhaust Air Node Name
'    50,                      !- Maximum Heating Supply Air Temperature {C}
'    13,                      !- Minimum Cooling Supply Air Temperature {C}
'    0.015,                   !- Maximum Heating Supply Air Humidity Ratio {kg-H2O/kg-air}
'    0.01,                    !- Minimum Cooling Supply Air Humidity Ratio {kg-H2O/kg-air}
'    NoLimit,                 !- Heating Limit
'    ,                        !- Maximum Heating Air Flow Rate {m3/s}
'    ,                        !- Maximum Sensible Heating Capacity {W}
'    NoLimit,                 !- Cooling Limit
'    ,                        !- Maximum Cooling Air Flow Rate {m3/s}
'    ,                        !- Maximum Total Cooling Capacity {W}
'    ,                        !- Heating Availability Schedule Name
'    ,                        !- Cooling Availability Schedule Name
'    ConstantSupplyHumidityRatio,  !- Dehumidification Control Type
'    ,                        !- Cooling Sensible Heat Ratio {dimensionless}
'    ConstantSupplyHumidityRatio,  !- Humidification Control Type
'    ,                        !- Design Specification Outdoor Air Object Name
'    ,                        !- Outdoor Air Inlet Node Name
'    ,                        !- Demand Controlled Ventilation Type
'    ,                        !- Outdoor Air Economizer Type
'    ,                        !- Heat Recovery Type
'    ,                        !- Sensible Heat Recovery Effectiveness {dimensionless}
'    ;                        !- Latent Heat Recovery Effectiveness {dimensionless}

    			For j = 0 To gerillaZones.Branch(i).Count-1
    			idfBaseFile = Replace(idfBaseFile, "!!!!!Zones!!!!!",gerillaZones.Branch(i).Item(j).ToString & vbNewLine & "!!!!!Zones!!!!!" )	
    			
    		Next
    	Next
    
   		For i = 0 To gerillaSurfaces.PathCount-1
    		For j = 0 To gerillaSurfaces.Branch(i).Count-1
      			idfBaseFile = Replace(idfBaseFile, "!!!!!BuildingSurfaces!!!!!", gerillaSurfaces.Branch(i).Item(j).ToString & vbNewLine & "!!!!!BuildingSurfaces!!!!!")
    		Next
    	Next
    	
'    	If gerillaMaterials.Branch(0).Item(0).ToString = "default materials" Then
'    	Else	
'    	
'		For i = 0 To gerillaMaterials.PathCount-1
'			For j = 0 To gerillaMaterials.Branch(i).Count-1
'				idfBaseFile = Replace(idfBaseFile, "!!!!!Materials!!!!!", gerillaMaterials.Branch(i).Item(j).ToString & vbNewLine & "!!!!!Materials!!!!!")
'    		Next
'    	Next
'    	End If
    	
    	If gerillaConstructions.Branch(0).Item(0).ToString = "default constructions" Then
    	Else	
    		
    	
    
    	For i = 0 To gerillaConstructions.PathCount-1
    		For j = 0 To gerillaConstructions.Branch(i).Count-1
    			idfBaseFile = Replace(idfBaseFile, "!!!!!Constructions!!!!!", gerillaConstructions.Branch(i).Item(j).ToString & vbNewLine & "!!!!!Constructions!!!!!")
    		Next
    		For j = 2 To gerillaConstructions.Branch(i).Count-1
    			Dim materialLength As Integer 
    			If gerillaConstructions.Branch(i).Item(j).ToString.IndexOf(",") <> -1 Then
    				materialLength = gerillaConstructions.Branch(i).Item(j).ToString.IndexOf(",")
    			Else
    				materialLength = gerillaConstructions.Branch(i).Item(j).ToString.IndexOf(";")
    			End If
    		'Dim materialStart As Integer = gerillaConstructions.Branch(i).Item(j).ToString.	
    		Dim materialName As String = gerillaConstructions.Branch(i).Item(j).ToString.Substring(0,materialLength)
    		
    		If materialList.Contains(materialName.Trim) = False Then
    			materialList.Add(materialName.trim)
    		End If
    		
    		
    		
    		Next
      	Next
    	End If
    	
    	For m As Integer = 0 To materialList.Count-1
    		Dim materialPath As String = "C:\Gerilla\Libraries\Materials\"& materialList(m) &".txt"
    		materialList.Add(materialPath)
    		Dim materialIDF As String = My.Computer.FileSystem.ReadAllText(materialPath)
    		materialList.Add(materialIDF)
    		idfBaseFile = Replace(idfBaseFile, "!!!!!Materials!!!!!", materialIDF.ToString & vbNewLine & "!!!!!Materials!!!!!")
    	Next
    	
    	
    	
    	
    	If RunPeriod(0) = "default: full year simulation" Then
    		idfBaseFile = Replace(idfBaseFile,"!!!!!RunPeriod!!!!!", "RunPeriod," & vbNewLine & "!!!!!RunPeriod!!!!!")
    		idfBaseFile = Replace(idfBaseFile,"!!!!!RunPeriod!!!!!", "    ,                        !- Name" & vbNewLine & "!!!!!RunPeriod!!!!!")
    		idfBaseFile = Replace(idfBaseFile,"!!!!!RunPeriod!!!!!", "    1,          !-Begin Month" & vbNewLine & "!!!!!RunPeriod!!!!!")
    		idfBaseFile = Replace(idfBaseFile,"!!!!!RunPeriod!!!!!", "    1,          !-Begin Day of Month" & vbNewLine & "!!!!!RunPeriod!!!!!")
    		idfBaseFile = Replace(idfBaseFile,"!!!!!RunPeriod!!!!!", "    12,          !-End Month" & vbNewLine & "!!!!!RunPeriod!!!!!")
    		idfBaseFile = Replace(idfBaseFile,"!!!!!RunPeriod!!!!!", "    31,          !-End Day of Month" & vbNewLine & "!!!!!RunPeriod!!!!!")
    	
    	Else
    		For i = 0 To RunPeriod.Count - 1
    			idfBaseFile = Replace(idfBaseFile,"!!!!!RunPeriod!!!!!", RunPeriod(i) & vbNewLine & "!!!!!RunPeriod!!!!!")
  
   			Next
    		
    	End If
    
    
    'Trigger for Batch File
    Dim BatchTrigger As Boolean
    BatchTrigger = False
    
    'Write IDF to file
    If WriteIDF = True Then
    Dim filePath As String = "C:\EnergyPlusV8-0-0\" & GerillaProjectSetup.PublicProjectName & ".idf"	
    'Dim filePath As String = GerillaProjectSetup.PublicProjectFilePath & "\" & GerillaProjectSetup.PublicProjectName & "\" & GerillaProjectSetup.PublicProjectName & ".idf"
    
    Dim ObjectWriter As New System.IO.StreamWriter(FilePath)
       
        ObjectWriter.Write(idfBaseFile)
        ObjectWriter.Close()
      	
      	'Done = True
    	BatchTrigger = True
    End If
    
	'Set Outputs
        DA.SetData(0, BatchTrigger)
        DA.SetData(1, idfBaseFile)
		'DA.SetDataList(2, materialList)
    End Sub

End Class
