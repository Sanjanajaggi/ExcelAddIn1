Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Office.Tools.Ribbon
Imports Syncfusion.OCRProcessor
Imports Syncfusion.Pdf.Graphics
Imports Syncfusion.Pdf.Parsing
Imports Syncfusion.Windows.PdfViewer

Public Class Ribbon1
    Public Shared Property markedRegions As Dictionary(Of Integer, List(Of System.Drawing.RectangleF))
    Public Shared Property extractedText As List(Of String)
    Private Shared Property documentCount As Integer = 0
    Private Shared Property pdfFilePath As String = ""

    ' Maintain a list of annotations
    Private Shared annotations As List(Of System.Drawing.RectangleF) = New List(Of System.Drawing.RectangleF)()

    Private Sub ToggleButton1_Click(sender As Object, e As RibbonControlEventArgs) Handles ToggleButton1.Click
        Globals.ThisAddIn.TaskPane.Visible =
        TryCast(sender, Microsoft.Office.Tools.Ribbon.RibbonToggleButton).Checked
    End Sub

    Private Sub Button1_Click(sender As Object, e As RibbonControlEventArgs) Handles Button1.Click
        If UserControl1.pdfViewer IsNot Nothing Then
            UserControl1.pdfViewer.AnnotationMode = Syncfusion.Windows.PdfViewer.PdfDocumentView.PdfViewerAnnotationMode.Rectangle
            If markedRegions IsNot Nothing Then
                markedRegions.Clear()
            End If
            markedRegions = New Dictionary(Of Integer, List(Of System.Drawing.RectangleF))()
            UserControl1.pdfViewer.RectangleAnnotationSettings.RectangleColor = Colors.Red

            ' Save annotations to file with the same name as the PDF file

        End If
    End Sub

    Private Sub ProcessOCR(sender As Object, e As RibbonControlEventArgs) Handles Button5.Click
        If UserControl1.pdfViewer IsNot Nothing Then
            If extractedText IsNot Nothing Then
                extractedText.Clear()
            End If
            extractedText = New List(Of String)()

            ' Save annotations to file with the same name as the PDF file
            If Not String.IsNullOrEmpty(pdfFilePath) Then
                Dim annotationFilePath As String = Path.ChangeExtension(pdfFilePath, "annotations.xml")
                ExportAnnotations(annotationFilePath)
            End If

            UserControl1.pdfViewer.AnnotationMode = Syncfusion.Windows.PdfViewer.PdfDocumentView.PdfViewerAnnotationMode.None

            ' Load annotations from the file with the same name as the PDF file
            If Not String.IsNullOrEmpty(pdfFilePath) Then
                Dim annotationFilePath As String = Path.ChangeExtension(pdfFilePath, "annotations.xml")
                ImportAnnotations(annotationFilePath)
            End If

            UserControl1.pdfViewer.AnnotationMode = Syncfusion.Windows.PdfViewer.PdfDocumentView.PdfViewerAnnotationMode.None
            If markedRegions IsNot Nothing AndAlso markedRegions.Count > 0 Then
                Dim convertor As PdfUnitConvertor = New PdfUnitConvertor()
                Using processor As New OCRProcessor(System.AppDomain.CurrentDomain.BaseDirectory + "../../Tesseract binaries")
                    For Each pages As KeyValuePair(Of Integer, List(Of System.Drawing.RectangleF)) In markedRegions
                        ' Language to process the OCR
                        processor.Settings.Language = Languages.English
                        Dim image As BitmapSource = UserControl1.pdfViewer.ExportAsImage(pages.Key)
                        ' BitmapSource to bitmap conversion
                        Dim encoder As New PngBitmapEncoder()
                        encoder.Frames.Add(BitmapFrame.Create(image))
                        Dim bitmap As Bitmap = Nothing
                        Using stream As New MemoryStream()
                            encoder.Save(stream)
                            bitmap = New Bitmap(stream)
                        End Using
                        For Each rect In pages.Value
                            ' Point to Pixel conversion
                            Dim bounds As RectangleF = convertor.ConvertToPixels(rect, PdfGraphicsUnit.Point)
                            Using clonedImage As Bitmap = bitmap.Clone(bounds, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                                Dim ocrText As String = processor.PerformOCR(clonedImage, System.AppDomain.CurrentDomain.BaseDirectory + "../../Tessdata/")
                                extractedText.Add(ocrText)
                                ' Add the rectangle to the list of annotations
                                annotations.Add(rect)
                                ' Redraw the annotations
                                UserControl1.pdfViewer.InvalidateVisual()
                            End Using
                        Next
                        bitmap.Dispose()
                    Next
                End Using
                documentCount += 1
                Using writer As New StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "../../ExtractedInformation" + documentCount.ToString() + ".csv")
                    ' Write the header row
                    writer.WriteLine("Value")

                    ' Write dictionary data to the CSV file
                    For Each kvp As String In extractedText
                        writer.WriteLine($"{kvp}")
                    Next
                End Using
                Dim rng As Excel.Range = Globals.ThisAddIn.Application.Range("A" + documentCount.ToString())
                rng.Value2 = extractedText(0)
                Dim rng2 As Excel.Range = Globals.ThisAddIn.Application.Range("B" + documentCount.ToString())
                rng2.Value2 = extractedText(1)
            End If
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As RibbonControlEventArgs) Handles Button3.Click
        Dim myForm As New Host_Organizer
        myForm.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As RibbonControlEventArgs) Handles Button2.Click
        If UserControl1.pdfViewer IsNot Nothing Then
            UserControl1.pdfViewer.AnnotationMode = Syncfusion.Windows.PdfViewer.PdfDocumentView.PdfViewerAnnotationMode.Rectangle
            If markedRegions IsNot Nothing Then
                markedRegions.Clear()
            End If
            markedRegions = New Dictionary(Of Integer, List(Of System.Drawing.RectangleF))()
            UserControl1.pdfViewer.RectangleAnnotationSettings.RectangleColor = Colors.Green

            ' Save annotations to file with the same name as the PDF file
            ExportAnnotations("Annotation.fdf")
        End If
    End Sub

    ' Export annotations to a file
    Private Sub ExportAnnotations(annotationFilePath As String)
        Using writer As New StreamWriter(annotationFilePath)
            For Each rect In annotations
                writer.WriteLine($"{rect.X},{rect.Y},{rect.Width},{rect.Height}")
            Next
        End Using
    End Sub

    ' Import annotations from a file
    Private Sub ImportAnnotations(annotationFilePath As String)
        If File.Exists(annotationFilePath) Then
            Dim lines As String() = File.ReadAllLines(annotationFilePath)
            annotations.Clear()
            For Each line In lines
                Dim parts As String() = line.Split(",")
                If parts.Length = 4 Then
                    Dim rect As New System.Drawing.RectangleF(
                        Single.Parse(parts(0)),
                        Single.Parse(parts(1)),
                        Single.Parse(parts(2)),
                        Single.Parse(parts(3))
                    )
                    annotations.Add(rect)
                End If
            Next
        End If
    End Sub
End Class
