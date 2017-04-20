﻿Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting

Namespace BarPlot

    ''' <summary>
    ''' 以条形图的方式可视化绘制两个离散的信号的比对的图形
    ''' </summary>
    Public Module AlignmentPlot

        ''' <summary>
        ''' 以条形图的方式可视化绘制两个离散的信号的比对的图形
        ''' </summary>
        ''' <param name="query">The query signals</param>
        ''' <param name="subject">The subject signal values</param>
        ''' <param name="cla$">Color expression for <paramref name="query"/></param>
        ''' <param name="clb$">Color expression for <paramref name="subject"/></param>
        ''' <returns></returns>
        <Extension>
        Public Function PlotAlignment(query As Dictionary(Of Double, Double),
                                      subject As Dictionary(Of Double, Double),
                                      Optional xrange As DoubleRange = Nothing,
                                      Optional yrange As DoubleRange = Nothing,
                                      Optional size$ = "960,600",
                                      Optional padding$ = g.DefaultPadding,
                                      Optional cla$ = "steelblue",
                                      Optional clb$ = "brown",
                                      Optional xlab$ = "X",
                                      Optional ylab$ = "Y",
                                      Optional title$ = "Alignments Plot",
                                      Optional tickCSS$ = CSSFont.Win7Normal,
                                      Optional titleCSS$ = CSSFont.Win7Large,
                                      Optional bw! = 8) As GraphicsData
            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)

                    Dim rect As Rectangle = region.PlotRegion
                    Dim yLength! = yrange.Length
                    Dim xLength! = xrange.Length
                    Dim ymid! = rect.Height / 2 + region.Padding.Top
                    Dim width! = rect.Width
                    Dim height! = rect.Height / 2
                    Dim yscale = Function(y!)
                                     Return (y / yLength) * (height)
                                 End Function
                    Dim xscale = Function(x!)
                                     Return (x / xLength) * width
                                 End Function

                    With rect
                        Dim axisPen As New Pen(Color.Black, 2)
                        Dim dy = yrange.Length / 5
                        Dim y!
                        Dim gridPen As New Pen(Color.Gray, 1) With {
                            .DashStyle = DashStyle.Dot
                        }
                        Dim dt! = 15
                        Dim tickPen As New Pen(Color.Black, 1)
                        Dim tickFont As Font = CSSFont.TryParse(tickCSS).GDIObject
                        Dim drawlabel = Sub(c As IGraphics, label$)
                                            Dim tsize = c.MeasureString(label, tickFont)
                                            Call c.DrawString(label, tickFont, Brushes.Black, New Point(.Left - dt - tsize.Width, y - tsize.Height / 2))
                                        End Sub

                        If TypeOf g Is Graphics2D Then
                            DirectCast(g, Graphics2D).Stroke = tickPen
                        End If

                        For i As Integer = 0 To 5
                            Dim label$ = (i * dy).ToString("F2") & "%"

                            y = ymid - yscale(i * dy) ' 上半部分
                            Call g.DrawLine(tickPen, New PointF(.Left, y), New Point(.Left - dt, y))
                            Call g.DrawLine(gridPen, New Point(.Left, y), New Point(.Right, y))
                            Call drawlabel(g, label)

                            If i = 0 Then
                                Continue For
                            End If

                            y = ymid + yscale(i * dy) ' 下半部分
                            Call g.DrawLine(tickPen, New PointF(.Left, y), New Point(.Left - dt, y))
                            Call g.DrawLine(gridPen, New Point(.Left, y), New Point(.Right, y))
                            Call drawlabel(g, label)
                        Next

                        ' Y 坐标轴
                        Call g.DrawLine(axisPen, .Location, New Point(.Left, .Bottom))
                        ' X 坐标轴
                        Call g.DrawLine(axisPen, New Point(.Left, ymid), New Point(.Right, ymid))

                        Dim left!
                        Dim ba As New SolidBrush(cla.TranslateColor)
                        Dim bb As New SolidBrush(clb.TranslateColor)

                        For Each o In query
                            y = o.Value
                            y = ymid - yscale(y)
                            left = .Left + xscale(o.Key)
                            rect = Rectangle(y, left, left + bw, ymid)
                            g.FillRectangle(ba, rect)
                        Next
                        For Each o In subject
                            y = o.Value
                            y = ymid + yscale(y)
                            left = .Left + xscale(o.Key)
                            rect = Rectangle(ymid, left, left + bw, y)
                            g.FillRectangle(bb, rect)
                        Next

                        rect = region.PlotRegion

                        Dim box As Rectangle

                        box = New Rectangle(New Point(rect.Right - 300, rect.Top + 20), New Size(20, 20))
                        Call g.FillRectangle(ba, box)

                        box = New Rectangle(New Point(box.Left, box.Top + 20), box.Size)
                        Call g.FillRectangle(ba, box)

                        Dim titleFont As Font = CSSFont.TryParse(titleCSS).GDIObject
                        Dim titleSize = g.MeasureString(title, titleFont)
                        Dim tl As New Point(
                            rect.Left + (rect.Width - titleSize.Width) / 2,
                            (region.Padding.Top - titleSize.Height) / 2)

                        Call g.DrawString(title, titleFont, Brushes.Black, tl)
                    End With
                End Sub

            If xrange Is Nothing Then
                xrange = New DoubleRange(query.Keys.Join(subject.Keys).ToArray)
            End If
            If yrange Is Nothing Then
                yrange = New DoubleRange(query.Values.Join(subject.Values).ToArray)
            End If

            Return g.GraphicsPlots(
                size.SizeParser, padding,
                "white",
                plotInternal)
        End Function
    End Module
End Namespace