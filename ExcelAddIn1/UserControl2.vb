Imports System.Windows.Forms
Imports System.Windows.Forms.Integration

Public Class UserControl2
    Dim elementHost As ElementHost = Nothing
    Dim pdfViewer As New UserControl1
    Private Sub UserControl2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        elementHost = New ElementHost()
        elementHost.AutoSize = True
        elementHost.Dock = DockStyle.Fill
        elementHost.Child = pdfViewer
        Me.Controls.Add(elementHost)
    End Sub
End Class
