Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Optimization

Public Class BundleConfig
    ' 如需統合的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=303951
    Public Shared Sub RegisterBundles(ByVal bundles As BundleCollection)
        bundles.Add(New Bundle("~/bundles/css").Include(
                     "~/Content/bootstrap.min.css",
                                "~/css/sticky-footer-navbar.css"))

        BundleTable.EnableOptimizations = True
    End Sub
End Class
