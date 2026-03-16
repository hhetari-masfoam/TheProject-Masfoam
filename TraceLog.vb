Imports System.Diagnostics

Public Module TraceLog
    Public Sub T(tag As String, Optional info As String = "")
        Debug.WriteLine(
            DateTime.Now.ToString("HH:mm:ss.fff") &
            " | " & tag &
            If(String.IsNullOrWhiteSpace(info), "", " | " & info)
        )
    End Sub
End Module
