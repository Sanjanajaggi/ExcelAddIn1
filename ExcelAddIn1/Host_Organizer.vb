Imports System.Windows.Forms
Imports System.Windows.Forms.Integration
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Shapes

Public Class Host_Organizer
    Dim elementHost As ElementHost = Nothing
    Dim organizer As New UserControl3

    Private Sub Host_Organizer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        elementHost = New ElementHost()
        elementHost.AutoSize = True
        elementHost.Dock = DockStyle.Fill

        elementHost.Child = organizer
        Me.Controls.Add(elementHost)
    End Sub
End Class