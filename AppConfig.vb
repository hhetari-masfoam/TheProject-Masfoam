Imports System.Configuration

Public Module AppConfig

    Public ReadOnly Property MainConnectionString As String
        Get
            Dim cs = ConfigurationManager.ConnectionStrings("MainDB")

            If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
                Throw New ApplicationException("Connection string 'MainDB' غير موجود في App.config")
            End If

            Return cs.ConnectionString
        End Get
    End Property

End Module
