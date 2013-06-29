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
	'The ePlusResults class is used by the EnergyPlus Results component (defined by the GerillaResults class) to extract the heating and cooling loads from the .eso results file. 
	
	Public Sub GetResults(ByVal resultsFilePath As String, firstIntegerTag As Integer, secondIntegerTag As Integer, stepSize As Integer, resultsOut As Grasshopper.DataTree(Of String))
		'Returns the monthly heating and cooling loads for each zone as a Grasshopper Data Tree. 
		'firstIntegerTag and secondIntegerTag are the tags that begin the lines of the first and second zone results of a particular type in the .eso.
		'stepSize is the difference between the tags that begin the 3rd and higher lines.
		'For example, Zone1 Heating tags for firstIntergerTag = 7, secondIntergerTag = 80, and stepSize = 33 would be: 7, 80, 113, 146, 179, etc.
		'Much of this should be rewritten using the String.StartsWith method
		
		resultsFilePath = resultsFilePath & ".eso"
    	Dim resultsLine As String = Nothing
    	Dim resultsReader As System.IO.StreamReader
    	
    	Dim numZones As Integer = 1  	
    	
    	For i As Integer = 0 To 99
    		'Counts the number of zones by counting the number of times the IntegerTag appears (incrementing the tag by stepSize*i)
    		'the counter begins at the secondIntegerTag and accounts for the firstIntegerTag by starting at numZones=1.
    		
    		'NOTE: This will always run 100 times, it should be rewritten to loop until the IntegerTag is not found, that way it only runs once for each zone. -Brendan 2013-06-29
    		
    		resultsReader = System.IO.File.OpenText(resultsFilePath)
    		Dim loopEnd As Boolean = False
    		
    		Do While resultsReader.Peek()>= 0 And loopEnd = False
    			'Reads results line by line looking for a line beginning with the appropriate integer tag. Loop ends when it finds one or reaches the end of the file.    			  			
    			resultsLine = resultsReader.ReadLine
				Dim intTest As Integer = secondIntegerTag + stepSize*i
				
				If resultsLine.Substring(0,intTest.ToString.Length) = intTest.ToString Then
    				numZones = numZones+1
    				loopEnd = True				
				End If
				
    		Loop
    	
    		resultsReader.Close
    		
    	Next
    	
    	'Create GH_Path object for Zone1Results.
		resultsReader = System.IO.File.OpenText(resultsFilePath)		
		Dim zone1ResultsPath As New Data.GH_Path(1)
		
		Do While resultsReader.Peek()>= 0
			'Loops through the results file searching for a line beginning with the firstIntegerTag, parses that data, and adds it to the resultsOut Grasshopper DataTree.			
			resultsLine = resultsReader.ReadLine
			
    		Dim stringTest As String = firstIntegerTag.ToString & ","
    		Dim resultsLineString As String = resultsLine.Substring(0, stringTest.Length)
    		
    		If resultsLine.Substring(0,resultsLineString.Length) = stringTest And resultsLine.Substring(resultsLineString.Length,2) <> "9," Then
    			'NOTE: The <> "9," part filters out the lines in the Data Dictionary.    			
    			'This method appears to be unreliable, as it had to be changed from'<> "7,"' to '<> "9,"' to get it to work when updating from EnergyPlusV6 to EnergyPlusV8.
    			'Changing to a method that uses the fact that the data dictionary always ends with "End of Data Dictionary" seems like it would be more reliable. - Brendan 2013-06-29    			
 				Dim resultsArr() as String = Split(resultsLine, ",")
 				resultsOut.Add(resultsArr(1).Trim(), zone1ResultsPath)   	 				
    		End If
    		
    	Loop    	
    	resultsReader.Close
    	
    	'Get the results for the remaining zones.
    	For i As Integer = 1 To numZones-1
    		'Gets the results for lines beginning with the secondIntegerTag and beyond, incremented by step size, parses that data, and adds it to the resultsOut Grasshopper DataTree.
    		Dim resultsPath As New Data.GH_Path(i+1)
    		Dim intTest As Integer = secondIntegerTag + stepSize*(i-1)
    		Dim intTestString As String = intTest.ToString & ","
    		resultsReader = System.IO.File.OpenText(resultsFilePath)
    		
    		Do While resultsReader.Peek()>= 0
    			resultsLine = resultsReader.ReadLine
    			
    			If resultsLine.Substring(0,intTestString.Length) = intTestString And resultsLine.Substring(intTestString.Length, 2) <> "9," Then
    				'The <> "9," part is supposed to filter out the lines in the Data Dictionary. See above for comment on this method.
    				Dim resultsArr() as String = Split(resultsLine, ",")
    				resultsOut.Add(resultsArr(1).Trim(), resultsPath)
    			End If	
    		Loop
    		resultsReader.Close	
    	Next
    	
	End Sub
	
	Public Sub GetDistrictResults(ByVal resultsFilePath As String, firstIntegerTag As Integer, resultsOut As Grasshopper.DataTree(Of String))
		'NOTE: District Results are not currently used in Gerilla. -Brendan 2013-06-29.
		
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
