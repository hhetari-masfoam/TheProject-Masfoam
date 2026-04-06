Imports System.Configuration
Imports System.ComponentModel

Public Module AppConfig

    Public ReadOnly Property MainConnectionString As String
        Get
            If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
                Return "Data Source=(local);Initial Catalog=master;Integrated Security=True;"
            End If

            Dim cs = ConfigurationManager.ConnectionStrings("MainDB")

            If cs Is Nothing OrElse String.IsNullOrWhiteSpace(cs.ConnectionString) Then
                Throw New ApplicationException("Connection string 'MainDB' غير موجود في App.config")
            End If

            Return cs.ConnectionString
        End Get
    End Property

End Module