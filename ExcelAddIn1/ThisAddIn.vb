Public Class ThisAddIn
    Private myUserControl As UserControl2
    Private WithEvents taskPaneValue As Microsoft.Office.Tools.CustomTaskPane
    Public ReadOnly Property TaskPane() As Microsoft.Office.Tools.CustomTaskPane
        Get
            Return taskPaneValue
        End Get
    End Property
    Private Sub ThisAddIn_Startup() Handles Me.Startup
        myUserControl = New UserControl2
        taskPaneValue = Me.CustomTaskPanes.Add(
            myUserControl, "MyCustomTaskPane")
    End Sub

    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown

    End Sub
    Private Sub taskPaneValue_VisibleChanged(ByVal sender As Object,
   ByVal e As System.EventArgs) Handles taskPaneValue.VisibleChanged
        Globals.Ribbons.Ribbon1.ToggleButton1.Checked = taskPaneValue.Visible
    End Sub
End Class
