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

Public Class RunPeriod
	 Inherits GH_Component
     
    'Constructor
    Public Sub New()
        MyBase.New("Gerilla Run Period","Run Period","Specify Run Period For energyPlus Simulation","Gerilla","Simulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
Return New Guid ("CD057D1B-8684-4495-AAA3-687A247CD0DE")
        End Get
    End Property
    
    Protected Overrides ReadOnly property icon As System.drawing.Bitmap
 		Get
    		Return gerillaResource.GerillaIconRunPeriod  
 	    End Get
	End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Begin Day Of Month","Begin Day Of Month","Beginning Day of Month", GH_ParamAccess.item, 1)
        pManager.AddIntegerParameter("Begin Month","Begin Month","Beginning Month", GH_ParamAccess.item, 1)
        pManager.AddIntegerParameter("End Month","End Month","Ending Month", GH_ParamAccess.item, 12)
        pManager.AddIntegerParameter("End Day Of Month","End Day Of Month","Ending Day of Month", GH_ParamAccess.item, 31)
        
    End Sub
    
    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pmanager.AddTextParameter("Gerilla Run Period IDF","IDF","Gerilla Run Period For IDF Compiler", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        
        'Get data from GH inputs
        Dim BeginDayOfMonth As Integer
        DA.GetData(0,BeginDayOfMonth)
        
        Dim BeginMonth As Integer
        DA.GetData(1,BeginMonth)
        
        Dim EndMonth As Integer
        DA.GetData(2,EndMonth)
        
        Dim EndDayOfMonth As Integer
        DA.GetData(3,EndDayOfMonth)
        
        Dim IDFstring As New List(Of String)

        IDFstring.Add("RunPeriod,")
        IDFstring.Add("    ,                        !- Name")
        IDFstring.Add("    "&BeginMonth & ",                      !- Begin Month")
        IDFstring.Add("    "&BeginDayOfMonth & ",                      !- Begin Day of Month")
        IDFstring.Add("    "&EndMonth & ",                      !- End Month")
        IDFstring.Add("    "&EndDayOfMonth & ",                      !- End Day of Month")
'        IDFstring.Add(DayOfWeekForStartDay & ",          !- Day of Week for Start Day")
'        IDFstring.Add("Yes,                     !- Use Weather File Holidays and Special Days")
'        IDFstring.Add("Yes,                     !- Use Weather File Daylight Saving Period")
'        IDFstring.Add("No,                      !- Apply Weekend Holiday Rule")
'        IDFstring.Add("Yes,                     !- Use Weather File Rain Indicators")
'        IDFstring.Add("Yes,                     !- Use Weather File Snow Indicators")
'        IDFstring.Add("1;                       !- Number of Times Runperiod to be Repeated")
'    
'        IDFstring.Add("SimulationControl,")
'        IDFstring.Add("No,                      !-Do Zone Sizing Calculation")
'        IDFstring.Add("No,                      !-Do System Sizing Calculation")
'        IDFstring.Add("No,                      !-Do Plant Sizing Calculation")

'    If RunSimForSizingPeriods = True Then
'      IDFstring.Add("Yes,                     !-Run Simulation for Sizing Periods")
'    Else
'      IDFstring.Add("No,                      !-Run Simulation for Sizing Periods")
'    End If
'
'    If RunSimForWeatherFileRunPeriods = True Then
'      IDFstring.Add("Yes;                     !-Run Simulation for Weather File Run Periods")
'    Else
'      IDFstring.Add("No;                      !-Run Simulation for Weather File Run Periods")
'    End If
    
    
'Set Outputs
        DA.SetDataList(0,IDFstring)

    End Sub

End Class
