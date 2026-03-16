Public Module CurrentUser

    Public EmployeeID As Integer
    Public EmpName As String
    Public LoginName As String
    Public RoleID As Integer
    Public IsAdmin As Boolean

    Public Sub Clear()
        EmployeeID = 0
        EmpName = String.Empty
        LoginName = String.Empty
        RoleID = 0
        IsAdmin = False
    End Sub

    Public Function IsLoggedIn() As Boolean
        Return EmployeeID > 0
    End Function

End Module
