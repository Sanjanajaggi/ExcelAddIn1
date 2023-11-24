Imports System.Diagnostics
Imports System.IO
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Input
Imports Syncfusion.Pdf.Parsing

Public Class UserControl3
    Private selectedButton As Button
    Public Shared Property pdfViewer As Syncfusion.Windows.PdfViewer.PdfViewerControl = New Syncfusion.Windows.PdfViewer.PdfViewerControl()


    Public Sub New()
        InitializeComponent()
        Dim directories As String() = Directory.GetDirectories(System.AppDomain.CurrentDomain.BaseDirectory + "\Document Organizer")
        For Each directory As String In directories
            AddButton(Path.GetFileName(directory))

        Next
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim clickedButton As Button = CType(e.OriginalSource, Button)

        If selectedButton IsNot Nothing AndAlso Not Object.ReferenceEquals(clickedButton, selectedButton) Then
            selectedButton.BorderThickness = New Thickness(0)
            selectedButton.Foreground = New SolidColorBrush(Colors.Black)
            selectedButton.FontWeight = FontWeights.Normal
        End If

        selectedButton = clickedButton
        selectedButton.BorderThickness = New Thickness(2)
        selectedButton.Foreground = New SolidColorBrush(Colors.DarkGoldenrod)
        selectedButton.FontWeight = FontWeights.Bold

        Dim selectedFolderName As String = TryCast(selectedButton.Content, String)
        Dim folderPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "\Document Organizer", selectedFolderName)

        Dim files As String() = Directory.GetFiles(folderPath)
        FilesList.ItemsSource = files.Select(Function(x) New FileInfo(x).Name)

        For Each item As Object In FoldersList.Children
            If TypeOf item Is Button AndAlso Not Object.ReferenceEquals(item, selectedButton) Then
                Dim button As Button = CType(item, Button)
                button.BorderThickness = New Thickness(0)
            End If
        Next
    End Sub

    Private Sub FilesList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim selectedItem As String = TryCast(FilesList.SelectedItem, String)
        If selectedItem IsNot Nothing Then
            Dim selectedFolderName As String = TryCast(selectedButton.Content, String)
            If selectedFolderName IsNot Nothing Then
                Dim filePath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "\Document Organizer", selectedFolderName, selectedItem)
                Try
                    If File.Exists(filePath) Then
                        OpenPdfFile(filePath)
                    Else
                        MessageBox.Show("File does not exist.")
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error opening file: " & ex.Message)
                End Try
            End If
        End If
    End Sub
    ' Use the PdfViewerControl to open the PDF file.
    Private Sub OpenPdfWithSyncfusion(pdfFilePath As String)
        ' Specify the path to your PDF document.
        ' Check if the PDF document file exists.
        If System.IO.File.Exists(pdfFilePath) Then
            Try
                ' Load the PDF document into the existing PdfViewerControl.
                UserControl1.pdfViewer.Load(pdfFilePath)
                Dim fileNameWithoutExtension As String
                'Dim fileName As String = pdfViewer.DocumentInfo.FileName
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pdfFilePath)

                Dim fileNameWithFdfExtension As String = fileNameWithoutExtension & ".fdf"
                Dim filePath As String = System.AppDomain.CurrentDomain.BaseDirectory + "Annotations\" & fileNameWithFdfExtension
                If File.Exists(filePath) Then
                    UserControl1.pdfViewer.ImportAnnotations(filePath, AnnotationDataFormat.Fdf)
                End If
            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)

            End Try
        Else
            MessageBox.Show("The specified PDF file does not exist.")
        End If
        Globals.ThisAddIn.TaskPane.Visible = True
    End Sub

    ' Call this method when the selected file is a PDF.
    Private Sub OpenPdfFile(filePath As String)
        If Path.GetExtension(filePath).ToLower() = ".pdf" Then
            OpenPdfWithSyncfusion(filePath)
        Else
            ' Open the file using the default system program.
            Process.Start(filePath)
        End If
    End Sub
    Private Sub AddNewGroupButton_Click(sender As Object, e As RoutedEventArgs)
        Dim newFolderName As String = Microsoft.VisualBasic.Interaction.InputBox("Enter the new folder name", "New Folder", "")

        If Not String.IsNullOrEmpty(newFolderName) Then
            Dim newPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "\Document Organizer", newFolderName)

            Try
                If Not Directory.Exists(newPath) Then
                    Directory.CreateDirectory(newPath)
                    MessageBox.Show("New folder created successfully.")

                    AddButton(newFolderName)
                Else
                    MessageBox.Show("Folder already exists.")
                End If
            Catch ex As Exception
                MessageBox.Show("Error creating folder: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub AddButton(folderName As String)

        Dim newButton As New Button() With {
            .Content = folderName,
            .HorizontalContentAlignment = HorizontalAlignment.Left,
            .Margin = New Thickness(5, 5, 5, 0), ' Adjust the margin values as needed
            .BorderBrush = New SolidColorBrush(Colors.Transparent),
            .Background = New SolidColorBrush(Colors.Transparent)
            }
        AddHandler newButton.Click, AddressOf Button_Click
        AddHandler newButton.MouseEnter, AddressOf Button_MouseEnter
        AddHandler newButton.MouseLeave, AddressOf Button_MouseLeave
        FoldersList.Children.Add(newButton)
    End Sub

    Private Sub Button_MouseEnter(sender As Object, e As MouseEventArgs)
        Dim button As Button = CType(sender, Button)
        button.Background = New SolidColorBrush(Colors.Goldenrod)
        button.BorderBrush = New SolidColorBrush(Colors.Goldenrod)

        button.BorderThickness = New Thickness(2)
        button.Cursor = Cursors.Hand
        'button.Foreground = New SolidColorBrush(Colors.Goldenrod)
        'button.FontWeight = FontWeights.Bold

    End Sub

    Private Sub Button_MouseLeave(sender As Object, e As MouseEventArgs)
        Dim button As Button = CType(sender, Button)
        button.Background = New SolidColorBrush(Colors.Transparent)
        button.BorderBrush = New SolidColorBrush(Colors.Transparent)
        button.BorderThickness = New Thickness(0)
        button.Cursor = Cursors.Arrow
        'button.Foreground = New SolidColorBrush(Colors.Black)
        'button.FontWeight = FontWeights.Normal

    End Sub
    Private Sub AddFilesButton_Click(sender As Object, e As RoutedEventArgs)
        If selectedButton Is Nothing Then
            MessageBox.Show("Please select a folder first.")
            Return
        End If

        Dim selectedFolderName As String = TryCast(selectedButton.Content, String)
        Dim folderPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "\Document Organizer", selectedFolderName)

        Dim openFileDialog As New Microsoft.Win32.OpenFileDialog()
        openFileDialog.Multiselect = True

        If openFileDialog.ShowDialog() = True Then
            For Each filePath As String In openFileDialog.FileNames
                Try
                    Dim fileName As String = Path.GetFileName(filePath)
                    Dim destPath As String = Path.Combine(folderPath, fileName)

                    File.Copy(filePath, destPath, True)
                Catch ex As Exception
                    MessageBox.Show("Error copying files: " & ex.Message)
                End Try
            Next

            MessageBox.Show("Files copied successfully.")

            ' Refresh the right section
            RefreshFilesList(selectedFolderName)
        End If
    End Sub

    Private Sub RefreshFilesList(folderName As String)
        Dim folderPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "\Document Organizer", folderName)
        Dim files As String() = Directory.GetFiles(folderPath)
        FilesList.ItemsSource = files.Select(Function(x) New FileInfo(x).Name)
    End Sub

    Private Sub DeleteButton_Click(sender As Object, e As RoutedEventArgs)
        If selectedButton Is Nothing Then
            MessageBox.Show("Please select a folder to delete.")
            Return
        End If

        Dim selectedFolderName As String = TryCast(selectedButton.Content, String)
        Dim folderPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "\Document Organizer", selectedFolderName)

        Try
            Directory.Delete(folderPath, True)
            MessageBox.Show("Folder deleted successfully.")
            FoldersList.Children.Remove(selectedButton)
            FilesList.ItemsSource = Nothing
        Catch ex As Exception
            MessageBox.Show("Error deleting folder: " & ex.Message)
        End Try
    End Sub
End Class
