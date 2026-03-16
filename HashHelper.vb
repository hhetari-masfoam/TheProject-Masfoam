Imports System.Security.Cryptography
Imports System.Text

Public Module HashHelper

    Public Function ComputeSha256Base64(input As String) As String
        If input Is Nothing Then input = ""

        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(input)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return Convert.ToBase64String(hash)
        End Using
    End Function

End Module