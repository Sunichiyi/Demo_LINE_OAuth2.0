Public Class DBConn
    Public Function GetConn() As String
        '參考 https://stackoverflow.com/questions/12490348/get-connection-string-from-web-config-in-asp-net
        Try
            Dim configconn As ConnectionStringSettings = ConfigurationManager.ConnectionStrings("dsn_demodb")
            Return configconn.ConnectionString
        Catch ex As Exception
            Throw New HttpException("database connection settings have not been set in web.config file")
        End Try
    End Function
End Class
