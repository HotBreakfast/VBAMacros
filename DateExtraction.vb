Sub DateExtraction()
    Dim sh As Worksheet
    Dim DestSh As Worksheet

    Dim Last As Long
    Dim shLast As Long
    Dim CopyRng As Range
    Dim StartRow As Long

    With Application
        .ScreenUpdating = False
        .EnableEvents = False
    End With
    
''''''''''''''''''''''''''''''''''''''''''''''
'Save Report with a new file name            '
''''''''''''''''''''''''''''''''''''''''''''''
    
    'Create Timestamp
     vNow = Now()
     vMthStr = CStr(Month(vNow))
     vDayStr = CStr(Day(vNow))
    'Add leading zeroes to month, day, hour, minutes
     If Len(vMthStr) = 1 Then
        vMthStr = "0" & vMthStr
     End If
     If Len(vDayStr) = 1 Then
        vDayStr = "0" & vDayStr
     End If

    'Get date string in yyyymmddhhnn format.
     vDateStr = Year(vNow) & vMthStr & vDayStr

    SheetPrefix = "Workload Heatmap - "
    Sheetname = SheetPrefix & vDateStr & ".xlsx"
    
    'File name
    strSheet = Sheetname
    'File Path
    strPath = "C:\Users\ctwellma\Documents\AS\Reports\Global Utilization Report\Workload Heat Map\"
    
    'File Name
    strSheet = strPath & strSheet
    
    'Save As
    ActiveWorkbook.saveas Filename:=strSheet, FileFormat:=xlOpenXMLWorkbook, CreateBackup:=False

    ' Delete unneccssary sheets.
    Application.DisplayAlerts = False
    On Error Resume Next
    ActiveWorkbook.Worksheets("LOVs").Delete
    ActiveWorkbook.Worksheets("Project Tracking - GPAGE").Delete
    ActiveWorkbook.Worksheets("Archive Desk Complete").Delete
    ActiveWorkbook.Worksheets("Archive Cold Projects").Delete
    ActiveWorkbook.Worksheets("Archive Closed Projects").Delete
    ActiveWorkbook.Worksheets("New WM Mapping").Delete
    ActiveWorkbook.Worksheets("Complete - Presales - Scoping").Delete
    ActiveWorkbook.Worksheets("Cold Projects").Delete
    ActiveWorkbook.Worksheets("Closed Projects").Delete
    ActiveWorkbook.Worksheets("Project Tracking - MILASKIN").Delete
    ActiveWorkbook.Worksheets("Project Tracking - AMALAN").Delete
    
    On Error GoTo 0
    

    ' Add a Combine all sheets to a new one
    Set DestSh = ActiveWorkbook.Worksheets.Add
    DestSh.Name = "PM Project Dates"

    ' Fill in the start row.
    StartRow = 2
    

    ' Loop through all worksheets and copy the data to the
    ' summary worksheet.
    For Each sh In ActiveWorkbook.Worksheets
        If sh.Name <> DestSh.Name Then

            ' Find the last row with data on the summary
            ' and source worksheets.
            Last = Lastrow(DestSh)
            shLast = Lastrow(sh)

            ' If source worksheet is not empty and if the last
            ' row >= StartRow, copy the range.
            If shLast > 0 And shLast >= StartRow Then
                'Set the range that you want to copy
                Set CopyRng = sh.Range(sh.Rows(StartRow), sh.Rows(shLast))

               ' Test to see whether there are enough rows in the summary
               ' worksheet to copy all the data.
                If Last + CopyRng.Rows.Count > DestSh.Rows.Count Then
                   MsgBox "There are not enough rows in the " & _
                   "summary worksheet to place the data."
                   GoTo ExitTheSub
                End If

                ' This statement copies values and formats.
                CopyRng.Copy
                With DestSh.Cells(Last + 1, "A")
                    .PasteSpecial xlPasteValues
                    .PasteSpecial xlPasteFormats
                    Application.CutCopyMode = False
                End With

            End If

        End If
    Next

ExitTheSub:

'Add Column Headers Back
Application.Goto DestSh.Cells(1)
ActiveCell.EntireRow.Select
Selection.Insert Shift:=xlDown
Sheets("Project Pipeline").Select
Range("A1:AZ1").Select
Application.CutCopyMode = False
Selection.Copy
Sheets("PM Project Dates").Select
Selection.PasteSpecial Paste:=xlPasteValues, Operation:=xlNone, SkipBlanks:=False, Transpose:=False
Sheets("PM Project Dates").Cells.Select
Selection.clearformats
    
'Delete Source Worksheets now that we're done with them
ActiveWorkbook.Worksheets("Project Tracking - BBODEN").Delete
ActiveWorkbook.Worksheets("Project Tracking - SMILESNI").Delete
ActiveWorkbook.Worksheets("Project Tracking - EVOGEL").Delete

'Loop through to eliminate inactive Projects
Last = Lastrow(DestSh)
Firstrow = ActiveSheet.UsedRange.Cells(2).Row
    Lrow = Last + Firstrow - 1
    
    With DestSh
        .DisplayPageBreaks = False
            
            For Lrow = Last To Firstrow Step -1
                If IsError(.Cells(Lrow, "G").Value) Then


                ElseIf .Cells(Lrow, "G").Value <> "Active" And .Cells(Lrow, "G").Value <> "Presales" And .Cells(Lrow, "G").Value <> "Pipeline" And .Cells(Lrow, "G").Value <> "Not Available" Then
                    .Rows(Lrow).EntireRow.Delete


                End If
            Next
            
            For Lrow = Last To Firstrow Step -1
                If IsError(.Cells(Lrow, "T").Value) Then


                ElseIf .Cells(Lrow, "T").Value = "" Then
                    .Rows(Lrow).EntireRow.Delete


                End If
            Next
    End With

'Cleanup
'Add Column Headers Back
Application.Goto DestSh.Cells(1)
ActiveCell.EntireRow.Select
Selection.Insert Shift:=xlDown
Sheets("Project Pipeline").Select
Range("A1:AZ1").Select
Application.CutCopyMode = False
Selection.Copy
Sheets("PM Project Dates").Select
Selection.PasteSpecial Paste:=xlPasteValues, Operation:=xlNone, SkipBlanks:=False, Transpose:=False
Sheets("PM Project Dates").Cells.Select
Selection.clearformats
ActiveWorkbook.Worksheets("Project Pipeline").Delete

'Move PM to the head of the class (First Column)
    Columns("T:T").Select
    Selection.Cut
    Columns("A:A").Select
    Selection.Insert Shift:=xlToRight

'Get Rid of Extra Columns
    Sheets("PM Project Dates").Select
    [B:N].Delete
    [C:C].Delete
    [E:XFD].Delete
    

''''''''''''''''''''''''''''''''''''''''''''''''''
'Add New Sheet and import PID Health report data '
''''''''''''''''''''''''''''''''''''''''''''''''''
    Set NewSh = ActiveWorkbook.Worksheets.Add
    NewSh.Name = "PIDHealthData"

'Open PID Health Report
    Workbooks.Open Filename:= _
        "C:\Users\ctwellma\Documents\AS\Reports\PID Health Report\PID Heath Latest.xlsx"
'Copy PID Health Detail
    Sheets("Detail").Select
    '****************************************
    '***Need to make this a dynamic selection
    Range("A2:BR262").Select
    Selection.Copy
    Application.Goto NewSh.Cells(1)

    Selection.PasteSpecial Paste:=xlPasteValues, Operation:=xlNone, SkipBlanks _
        :=False, Transpose:=False
    Application.CutCopyMode = False
    Selection.NumberFormat = "General"

'Move PID to the head of the class (First Column)
    Columns("G:G").Select
    Selection.Cut
    Columns("A:A").Select
    Selection.Insert Shift:=xlToRight
    'Name "PIDHealth" as a Range
    '****************************************
    '***Need to make this a dynamic selection
    Range("A1:BQ19").Select
    ActiveSheet.ListObjects.Add(xlSrcRange, Range("$A$1:$BR$261"), , xlYes).Name = _
        "PIDHealth"
    'Format As Table
    Range("PIDHealth[#All]").Select
    ActiveSheet.ListObjects("PIDHealth").TableStyle = "TableStyleLight1"
    
    Windows("PID Heath Latest.xlsx").Activate
    ActiveWindow.Close

''''''''''''''''''''''''''''''''''''''''''''''''''
'Add New Sheet to join PID health and PMO Data   '
''''''''''''''''''''''''''''''''''''''''''''''''''
    Sheets.Add After:=Sheets(Sheets.Count)
    Sheets("Sheet3").Select
    Sheets("Sheet3").Name = "Raw Data"
    Sheets("PM Project Dates").Select
    Range("A1").Select
    Range(Selection, Selection.End(xlDown)).Select
    Range(Selection, Selection.End(xlToRight)).Select
    Selection.Copy
    Sheets("Raw Data").Select
    Range("A1").Select
    ActiveSheet.Paste
    Range("L1").Select
    Sheets("PIDHealthData").Select
    Range("PIDHealth[[#Headers],[Project Number]]").Select
    Range(Selection, Selection.End(xlToRight)).Select
    Selection.Copy
    Sheets("Raw Data").Select
    Range("L1").Select
    ActiveSheet.Paste
    Selection.Font.Bold = False
    Range("L2").Select
    ActiveCell.FormulaR1C1 = _
        "=INDEX(PIDHealth,MATCH('Raw Data'!RC1,PIDHealthData!C1,0)-1,MATCH('Raw Data'!R1C,PIDHealthData!R1,0))"
    Range("L2").Select
    Selection.AutoFill Destination:=Range("L2:L67")
    Range("L2:L67").Select
    Selection.AutoFill Destination:=Range("L2:CC67"), Type:=xlFillDefault
    Range("L2:CC67").Select
    
    'Name Range
    Range("A1").Select
    Range(Selection, Selection.End(xlDown)).Select
    Range(Selection, Selection.End(xlToRight)).Select
    ActiveSheet.ListObjects.Add(xlSrcRange, Range("A1:CC67"), , xlYes).Name = _
        "CombinedRawData"
    Range("CombinedRawData[#All]").Select
    ActiveSheet.ListObjects("CombinedRawData").TableStyle = "TableStyleLight1"
    
    'Filter out extra data from PID Health
    '*** NEED TO DEVELOP THIS PROCESS *****
    ''''''''''''''''''''''''''''''''''''''
    
'Add Dynamic Summary
    Sheets.Add After:=Sheets(Sheets.Count)
    Sheets("Sheet4").Select
    Sheets("Sheet4").Name = "Dynamic Summary"
    Range("A1:Z1").Select
    With Selection.Validation
        .Delete
        .Add Type:=xlValidateList, AlertStyle:=xlValidAlertStop, Operator:= _
        xlBetween, Formula1:="='Raw Data'!$A$1:$CC$1"
        .IgnoreBlank = True
        .InCellDropdown = True
        .InputTitle = ""
        .ErrorTitle = ""
        .InputMessage = ""
        .ErrorMessage = ""
        .ShowInput = True
        .ShowError = True
    End With
    
    'Add Default Headers and format widths
    Range("A1").Value = "Project ID (PID):"
    Columns("A:A").Select
    Selection.ColumnWidth = 10
    'Selection.NumberFormat = "General"
    Range("B1").Value = "PID Status:"
    Columns("B:B").Select
    Selection.ColumnWidth = 13.5
    'Selection.NumberFormat = "@"
    Range("C1").Value = "Customer Name"
    Columns("C:C").Select
    Selection.ColumnWidth = 40
    'Selection.NumberFormat = "@"
    Range("D1").Value = "Practice DCPM Assigned"
    Columns("D:D").Select
    Selection.ColumnWidth = 18
    'Selection.NumberFormat = "@"
    Range("E1").Value = "AS Approved Cost Budget"
    Columns("E:E").Select
    Selection.ColumnWidth = 17
    'Selection.NumberFormat = "_($* #,##0.00_);_($* (#,##0.00);_($* ""-""??_);_(@_)"
    Range("F1").Value = "Actual Costs"
    Columns("F:F").Select
    Selection.ColumnWidth = 17
    'Selection.NumberFormat = "_($* #,##0.00_);_($* (#,##0.00);_($* ""-""??_);_(@_)"
    Range("G1").Value = "% Over / Under  Budget"
    Columns("G:G").Select
    Selection.ColumnWidth = 13
    'Selection.NumberFormat = "%"
    Range("H1").Value = "As Approved Budgeted Hours"
    Columns("H:H").Select
    Selection.ColumnWidth = 15
    'Selection.NumberFormat = "0.00"
    Range("I1").Value = "Total hours"
    Columns("I:I").Select
    Selection.ColumnWidth = 10
    'Selection.NumberFormat = "0.00"
    Range("J1").Value = "Delivery Manager"
    Columns("J:J").Select
    Selection.ColumnWidth = 18
    'Selection.NumberFormat = "@"
    Range("K1").Value = "Actual PreSales Cost % of Actual Total Cost"
    Columns("K:K").Select
    Selection.ColumnWidth = 10
    'Selection.NumberFormat = "%"
    
    'Format Headers
    Range("A1:K1").Select
    With Selection.Interior
        .Pattern = xlSolid
        .PatternColorIndex = xlAutomatic
        .ThemeColor = xlThemeColorLight1
        .TintAndShade = 0.499984740745262
        .PatternTintAndShade = 0
    End With
    Rows("1:1").RowHeight = 30
    Rows("1:1").Select
    With Selection
        .HorizontalAlignment = xlGeneral
        .VerticalAlignment = xlBottom
        .WrapText = True
        .Orientation = 0
        .AddIndent = False
        .IndentLevel = 0
        .ShrinkToFit = False
        .ReadingOrder = xlContext
        .MergeCells = False
    End With
    With Selection.Font
        .ThemeColor = xlThemeColorDark1
        .TintAndShade = 0
        .Bold = True
    End With
          
    
    
    'Input Formula for Dynamic table
    Range("A2").Select
    ActiveCell.Formula = _
        "=INDEX('Raw Data'!$A$1:$CC$67,ROWS($A$2:A2)+1,MATCH(A$1,'Raw Data'!$1:$1,0))"
    Range("A2").Select
    Selection.AutoFill Destination:=Range("A2:A67")
    Range("B2").Select
    ActiveCell.FormulaR1C1 = _
        "=IF((ISERROR(INDEX('Raw Data'!R1C1:R67C81,MATCH(RC1,'Raw Data'!C1,0),MATCH('Dynamic Summary'!R1C,'Raw Data'!R1,0)))),""-"",INDEX('Raw Data'!R1C1:R67C81,MATCH(RC1,'Raw Data'!C1,0),MATCH('Dynamic Summary'!R1C,'Raw Data'!R1,0)))"
    'ActiveCell.FormulaR1C1 = _
    '    "=INDEX('Raw Data'!R1C1:R67C81,MATCH(RC1,'Raw Data'!C1,0),MATCH('Dynamic Summary'!R1C,'Raw Data'!R1,0))"
    Range("B2").Select
    Selection.AutoFill Destination:=Range("B2:B67"), Type:=xlFillDefault
    Range("B2:B67").Select
    Selection.AutoFill Destination:=Range("B2:K67"), Type:=xlFillDefault
    Range("A2:K67").Select
    Selection.NumberFormat = "General"
    Columns("E:F").Select
    Selection.Style = "Currency"
    Columns("G:G").Select
    Selection.Style = "Percent"
    Columns("H:I").Select
    Selection.Style = "Comma"
    Columns("K:K").Select
    Selection.Style = "Percent"

    Sheets("Raw Data").Select
    Range("CombinedRawData[[#Headers],[Project ID (PID):]]").Select
    Range(Selection, Selection.End(xlDown)).Select
    Range(Selection, Selection.End(xlToRight)).Select
    Application.CutCopyMode = False
    Selection.Copy
    Range("CombinedRawData[[#Headers],[Project ID (PID):]]").Select
    Selection.PasteSpecial Paste:=xlPasteValues, Operation:=xlNone, SkipBlanks _
        :=False, Transpose:=False
    Sheets("PM Project Dates").Select
    Application.CutCopyMode = False
    ActiveWindow.SelectedSheets.Delete
    Sheets("PIDHealthData").Select
    ActiveWindow.SelectedSheets.Delete
    Sheets("Dynamic Summary").Select
    
    'Format Over Budget
    Range("G2").Select
    Range(Selection, Selection.End(xlDown)).Select
    Selection.FormatConditions.Add Type:=xlCellValue, Operator:=xlGreater, _
        Formula1:="=0"
    Selection.FormatConditions(Selection.FormatConditions.Count).SetFirstPriority
    With Selection.FormatConditions(1).Font
        .Color = -16776961
        .TintAndShade = 0
    End With
    Selection.FormatConditions(1).StopIfTrue = False
    ActiveWorkbook.Save
    
    
'Turn on Updates
    With Application
        .Calculation = xlCalculationAutomatic
        .ScreenUpdating = True
        .EnableEvents = True
        .DisplayAlerts = True
    End With

End Sub
