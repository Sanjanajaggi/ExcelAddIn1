Imports System.Drawing
Imports System.IO
Imports System.Windows
Imports Syncfusion.Pdf
Imports Syncfusion.Pdf.Graphics
Imports Syncfusion.Pdf.Parsing
Imports Syncfusion.Windows.PdfViewer

Public Class UserControl1
    Public Shared Property pdfViewer As Syncfusion.Windows.PdfViewer.PdfViewerControl = New Syncfusion.Windows.PdfViewer.PdfViewerControl()
    Private Shared markedRegions As New Dictionary(Of Integer, List(Of RectangleF))()
    Private pdfDocument As PdfDocument = New PdfDocument()
    Private overlayDocument As PdfDocument = New PdfDocument()
    Dim fileNameWithoutExtension As String

    Public Sub New()
        InitializeComponent()
        home.Children.Add(pdfViewer)
        AddHandler pdfViewer.ShapeAnnotationChanged, AddressOf pdfViewer_ShapeAnnotationChanged
    End Sub

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        Dim pdfDocumentPath As String = System.AppDomain.CurrentDomain.BaseDirectory + "../../Data/Form-1.pdf"
        If File.Exists(pdfDocumentPath) Then
            Try
                pdfViewer.Load(pdfDocumentPath)
                Dim fileName As String = pdfViewer.DocumentInfo.FileName
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName)

                Dim fileNameWithFdfExtension As String = fileNameWithoutExtension & ".fdf"
                Dim filePath As String = System.AppDomain.CurrentDomain.BaseDirectory + "\Annotations\" & fileNameWithFdfExtension
                If File.Exists(filePath) Then
                    pdfViewer.ImportAnnotations(filePath, AnnotationDataFormat.Fdf)
                End If

            Catch ex As Exception
                MessageBox.Show("An error occurred: " & ex.Message)
            End Try
        Else
            MessageBox.Show("The specified PDF file does not exist.")
        End If
    End Sub

    Private Sub pdfViewer_ShapeAnnotationChanged(sender As Object, e As Syncfusion.Windows.PdfViewer.ShapeAnnotationChangedEventArgs)
        If e.Type = Syncfusion.Windows.PdfViewer.ShapeAnnotationType.Rectangle AndAlso e.Action = Syncfusion.Windows.PdfViewer.AnnotationChangedAction.Add Then
            If markedRegions.ContainsKey(UserControl1.pdfViewer.CurrentPageIndex - 1) Then
                markedRegions(UserControl1.pdfViewer.CurrentPageIndex - 1).Add(e.NewBounds)
            Else
                Dim bounds As List(Of System.Drawing.RectangleF) = New List(Of System.Drawing.RectangleF)()
                bounds.Add(e.NewBounds)
                markedRegions.Add(UserControl1.pdfViewer.CurrentPageIndex - 1, bounds)
            End If
        ElseIf e.Type = Syncfusion.Windows.PdfViewer.ShapeAnnotationType.Rectangle AndAlso e.Action = Syncfusion.Windows.PdfViewer.AnnotationChangedAction.Remove Then
            If markedRegions.ContainsKey(UserControl1.pdfViewer.CurrentPageIndex - 1) Then
                Dim modifiedBounds As List(Of RectangleF) = markedRegions(UserControl1.pdfViewer.CurrentPageIndex - 1)
                If modifiedBounds.Contains(e.NewBounds) Then
                    modifiedBounds.Remove(e.NewBounds)
                    markedRegions(UserControl1.pdfViewer.CurrentPageIndex - 1) = modifiedBounds
                End If
            End If
        End If
        Dim fileName As String = pdfViewer.DocumentInfo.FileName
        fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName)
        Dim fileNameWithFdfExtension As String = fileNameWithoutExtension & ".fdf"
        pdfViewer.ExportAnnotations(System.AppDomain.CurrentDomain.BaseDirectory + "\Annotations\" & fileNameWithFdfExtension, AnnotationDataFormat.Fdf)

    End Sub



End Class