﻿Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace Drawing2D.Vector.Text

    ''' <summary>
    ''' 利用GDI+绘制旋转文字，矩形内可以根据布局方式排列文本
    ''' </summary>
    ''' <remarks>http://www.xuebuyuan.com/1613072.html</remarks>
    Public Class GraphicsText

        Dim _graphics As Graphics

        Public Sub New(g As Graphics)
            _graphics = g
        End Sub

        ''' <summary>
        ''' 绘制根据矩形旋转文本
        ''' </summary>
        ''' <param name="s">文本</param>
        ''' <param name="font">字体</param>
        ''' <param name="brush">填充</param>
        ''' <param name="layoutRectangle">局部矩形</param>
        ''' <param name="format">布局方式</param>
        ''' <param name="angle">角度</param>
        Public Sub DrawString(s$, font As Font, brush As Brush, layoutRectangle As RectangleF, format As StringFormat, angle!)
            Dim size As SizeF = _graphics.MeasureString(s, font) ' 求取字符串大小                     
            Dim sizeRotate As SizeF = ConvertSize(size, angle)   ' 根据旋转角度，求取旋转后字符串大小          
            Dim rotatePt As PointF = GetRotatePoint(sizeRotate, layoutRectangle, format)   ' 根据旋转后尺寸、布局矩形、布局方式计算文本旋转点          
            Dim newFormat As New StringFormat(format) With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }  ' 重设布局方式都为Center

            ' 绘制旋转后文本
            Call DrawString(s, font, brush, rotatePt, angle, newFormat)
        End Sub

        ''' <summary>
        ''' 绘制根据点旋转文本，一般旋转点给定位文本包围盒中心点
        ''' </summary>
        ''' <param name="s">文本</param>
        ''' <param name="font">字体</param>
        ''' <param name="brush">填充</param>
        ''' <param name="point">旋转点</param>
        ''' <param name="format">布局方式</param>
        ''' <param name="angle">角度</param>
        Public Sub DrawString(s$, font As Font, brush As Brush, point As PointF, angle!, Optional format As StringFormat = Nothing)
            If format Is Nothing Then
                format = New StringFormat
            End If

            SyncLock _graphics
                Call DrawStringInternal(s, font, brush, point, format, angle)
            End SyncLock
        End Sub

        Private Sub DrawStringInternal(s$, font As Font, brush As Brush, point As PointF, format As StringFormat, angle!)
            Dim mtxSave As Matrix = _graphics.Transform  ' Save the matrix
            Dim mtxRotate As Matrix = _graphics.Transform

            Call mtxRotate.RotateAt(angle, point)

            With _graphics
                .Transform = mtxRotate
                .DrawString(s, font, brush, point, format)
                .Transform = mtxSave   ' Reset the matrix
            End With
        End Sub

        Private Function ConvertSize(size As SizeF, angle!) As SizeF
            Dim matrix As New Matrix()

            Call matrix.Rotate(angle)

            ' 旋转矩形四个顶点
            Dim pts As PointF() = New PointF(3) {}
            pts(0).X = -size.Width / 2.0F
            pts(0).Y = -size.Height / 2.0F
            pts(1).X = -size.Width / 2.0F
            pts(1).Y = size.Height / 2.0F
            pts(2).X = size.Width / 2.0F
            pts(2).Y = size.Height / 2.0F
            pts(3).X = size.Width / 2.0F
            pts(3).Y = -size.Height / 2.0F
            matrix.TransformPoints(pts)

            ' 求取四个顶点的包围盒
            Dim left As Single = Single.MaxValue
            Dim right As Single = Single.MinValue
            Dim top As Single = Single.MaxValue
            Dim bottom As Single = Single.MinValue

            For Each pt As PointF In pts
                ' 求取并集
                If pt.X < left Then
                    left = pt.X
                End If
                If pt.X > right Then
                    right = pt.X
                End If
                If pt.Y < top Then
                    top = pt.Y
                End If
                If pt.Y > bottom Then
                    bottom = pt.Y
                End If
            Next

            Dim result As New SizeF(right - left, bottom - top)
            Return result
        End Function

        Private Function GetRotatePoint(size As SizeF, layoutRectangle As RectangleF, format As StringFormat) As PointF
            Dim x!, y!

            Select Case format.Alignment
                Case StringAlignment.Near
                    x = layoutRectangle.Left + size.Width / 2.0F

                Case StringAlignment.Center
                    x = (layoutRectangle.Left + layoutRectangle.Right) / 2.0F

                Case StringAlignment.Far
                    x = layoutRectangle.Right - size.Width / 2.0F

                Case Else

            End Select

            Select Case format.LineAlignment
                Case StringAlignment.Near
                    y = layoutRectangle.Top + size.Height / 2.0F

                Case StringAlignment.Center
                    y = (layoutRectangle.Top + layoutRectangle.Bottom) / 2.0F

                Case StringAlignment.Far
                    y = layoutRectangle.Bottom - size.Height / 2.0F

                Case Else

            End Select

            Return New PointF(x, y)
        End Function
    End Class
End Namespace