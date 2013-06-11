'
' Created by SharpDevelop.
' User: Ben
' Date: 12/12/2011
' Time: 2:15 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry

Public Class ePlusResults
	
	Public Sub GetResults(ByVal resultsFilePath As String, firstIntegerTag As Integer, secondIntegerTag As Integer, stepSize As Integer, resultsOut As Grasshopper.DataTree(Of String))
		
	
		resultsFilePath = resultsFilePath & ".eso"
    	Dim resultsLine As String = Nothing
    	Dim resultsReader As System.IO.StreamReader
    	
    	Dim numZones As Integer = 1
    	For i As Integer = 0 To 99
    		resultsReader = System.IO.File.OpenText(resultsFilePath)
    		Dim loopEnd As Boolean = False
    		Do While resultsReader.Peek()>= 0 And loopEnd = False
				resultsLine = resultsReader.ReadLine
				Dim intTest As Integer = secondIntegerTag + stepSize*i
    			If resultsLine.Substring(0,intTest.ToString.Length) = intTest.ToString Then
    				numZones = numZones+1
    				loopEnd = True
    			End If
    		Loop
    		resultsReader.Close   	
		Next     	

		resultsReader = System.IO.File.OpenText(resultsFilePath)
		Dim zone1ResultsPath As New Data.GH_Path(1)
		Do While resultsReader.Peek()>= 0
    		resultsLine = resultsReader.ReadLine
    		Dim stringTest As String = firstIntegerTag.ToString & ","
    		Dim resultsLineString As String = resultsLine.Substring(0, stringTest.Length)
    		If resultsLine.Substring(0,resultsLineString.Length) = stringTest And resultsLine.Substring(resultsLineString.Length,2) <> "7," Then 
    		'If resultsLine.Substring(0,2) = "7," And resultsLine.Substring(2,2) <> "7," Then
    			resultsOut.Add(resultsLine.Substring(firstIntegerTag.ToString.Length+1), zone1ResultsPath)
    		End If
    	Loop
    	
    	resultsReader.Close
    	
    	For i As Integer = 1 To numZones-1
    		Dim resultsPath As New Data.GH_Path(i+1)
    		Dim intTest As Integer = secondIntegerTag + stepSize*(i-1)
    		Dim intTestString As String = intTest.ToString & ","
    		resultsReader = System.IO.File.OpenText(resultsFilePath)
    		
    		Do While resultsReader.Peek()>= 0
    			resultsLine = resultsReader.ReadLine
    			
    			If resultsLine.Substring(0,intTestString.Length) = intTestString And resultsLine.Substring(intTestString.Length, 2) <> "7," Then
    				resultsOut.Add(resultsLine.Substring(intTestString.Length), resultsPath)
    			End If	
    		Loop
    		resultsReader.Close	
    	Next
	End Sub
	
	Public Sub GetDistrictResults(ByVal resultsFilePath As String, firstIntegerTag As Integer, resultsOut As Grasshopper.DataTree(Of String))
		
		resultsFilePath = resultsFilePath & ".mtr"
    	Dim resultsLine As String = Nothing
    	Dim resultsReader As System.IO.StreamReader
    	
'    	Dim numZones As Integer = 1
'    	For i As Integer = 0 To 99
'    		resultsReader = System.IO.File.OpenText(resultsFilePath)
'    		Dim loopEnd As Boolean = False
'    		Do While resultsReader.Peek()>= 0 And loopEnd = False
'				resultsLine = resultsReader.ReadLine
'				Dim intTest As Integer = secondIntegerTag + stepSize*i
'    			If resultsLine.Substring(0,intTest.ToString.Length) = intTest.ToString Then
'    				numZones = numZones+1
'    				loopEnd = True
'    			End If
'
'    		Loop
'    		
'    		resultsReader.Close   	
'		Next     	
    	
    	
    	

		resultsReader = System.IO.File.OpenText(resultsFilePath)
		Dim districtResultsPath As New Data.GH_Path(1)
		Do While resultsReader.Peek()>= 0
    		resultsLine = resultsReader.ReadLine
    	Dim stringTest As String = firstIntegerTag.ToString & ","
    	Dim resultsLineString As String = resultsLine.Substring(0, stringTest.Length)
    	'If resultsLine.Substring(0,2) = "7," And resultsLine.Substring(2,2) <> "7," Then
    	If resultsLine.Substring(0,resultsLineString.Length) = stringTest And resultsLine.Substring(resultsLineString.Length,2) <> "7," Then 
    			resultsOut.Add(resultsLine.Substring(resultsLineString.Length), districtResultsPath)
    		End If
    	Loop
    	
    	resultsReader.Close
    	
'    	For i As Integer = 1 To numZones-1
'    		Dim resultsPath As New Data.GH_Path(i+1)
'    		Dim intTest As Integer = secondIntegerTag + stepSize*(i-1)
'    		
'    		resultsReader = System.IO.File.OpenText(resultsFilePath)
'    		
'    		Do While resultsReader.Peek()>= 0
'    			resultsLine = resultsReader.ReadLine
'    			
'    			If resultsLine.Substring(0,intTest.ToString.Length) = intTest.ToString And resultsLine.Substring(intTest.ToString.Length+1, 2) <> "7," Then
'    				resultsOut.Add(resultsLine.Substring(intTest.ToString.Length-1), resultsPath)
'    			End If	
'    		Loop
'    		resultsReader.Close	
'    	Next
    End Sub
End Class
