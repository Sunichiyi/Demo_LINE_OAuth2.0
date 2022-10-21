<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="default.aspx.vb" Inherits="Demo_LINE_OAuth2._0._default" %>

<!DOCTYPE html>

<html lang="en" class="h-100">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <webopt:bundlereference runat="server" path="~/bundles/css" />

    <title>Demo LINE OAuth 2.0</title>
    <link rel="icon" href="images/cgu_icon.svg" />

    <script src="//cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        function alert(msg, type) {
            Swal.fire({
                title: msg,
                icon: type
            });
        }
    </script>

    <style>
        .btn-line {
            padding: 0;
            background: #06C755;
        }
        .btn-line:hover {
            background: #05b34c;
        }
        .btn-line:active {
            background: #048b3b;
        }
        .text-line {
            font-size: 11px;
            padding: 16px 32px !important;
        }
    </style>
</head>
<body class="d-flex flex-column h-100">
    <header style="border-top: solid 10px #fb7832;">
        <div class="container">
            <!-- Fixed navbar -->
            <nav class="navbar navbar-expand-sm navbar-light" runat="server" id="navbar">
                <a class="navbar-brand" href="Default.aspx">
                    Demo LINE OAuth 2.0
                </a>

                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation" runat="server" id="button">
                    <span class="navbar-toggler-icon"></span>
                </button>

                <div class="collapse navbar-collapse justify-content-end" id="navbarSupportedContent">
                    <ul class="navbar-nav">
                        <li class="nav-item py-1" runat="server" id="LINE_info" Visible="false">
                            <asp:Image ID="Image_picture" CssClass="rounded" Width="32" Height="32" runat="server"/>
                            <asp:Label ID="Label_name" CssClass="font-weight-normal px-1" runat="server"></asp:Label>
                        </li>
                        <li class="nav-item" runat="server" id="Buttom_Logout" Visible="false">
                            <u><a class="btn btn-danger" href="logout.aspx">登出</a></u>
                        </li>
                        <li class="nav-item" runat="server" id="Buttom_Login" Visible="false">
                            <u>
                                <a class="btn text-white font-weight-bold btn-line" href="login.aspx">
                                    <%-- 參考 https://developers.line.biz/en/docs/line-login/login-button/ --%>
                                    <img style="padding: 1px;" src="images/line_32.png"/><span class="text-line">Log in with LINE</span>
                                </a>
                            </u>
                        </li>
                    </ul>
                </div>
            </nav>
        </div>
    </header>

    <!-- Begin page content -->
    <main role="main" class="flex-shrink-0">
        <div class="container">
            <div class="row">
                <div class="col">
                    <form runat="server">
                        <asp:ScriptManager runat="server">
                            <Scripts>
                                <%--To learn more about bundling scripts in ScriptManager see https://go.microsoft.com/fwlink/?LinkID=301884 --%>
                                <%--Framework Scripts--%>
                                <asp:ScriptReference Name="jquery" />
                                <asp:ScriptReference Name="bootstrap" />
                                <%--Site Scripts--%>
                            </Scripts>
                        </asp:ScriptManager>
                        <div class="row justify-content-center py-3" style="border-top: 5px solid #fb7832;">
                            <div class="col">
                                <asp:Button ID="Button_Subscribe" CssClass="btn btn-success" runat="server" Text="訂閱 Line Notify 通知" Visible="false"/>
                                <asp:Button ID="Button_Revoke" CssClass="btn btn-danger" runat="server" Text="取消訂閱" Visible="false"/>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </main>

    <footer class="footer mt-auto py-3">
        <div class="container text-center">
            <asp:Label ID="copyright" runat="server"></asp:Label>
        </div>
    </footer>
</body>
</html>
