﻿#Region "Microsoft.VisualBasic::986be76f9d00fe98d412ddf8de128180, Data_science\Mathematica\Plot\Plots\BarPlot\Data.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class BarDataSample
    ' 
    '         Properties: data, StackedSum, Tag
    ' 
    '         Function: ToString
    ' 
    '     Class BarDataGroup
    ' 
    '         Properties: Index, Samples, Serials
    ' 
    '         Function: Asc, Desc, FromDistributes, Reorder, sortInternal
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace BarPlot

    ''' <summary>
    ''' Named value of double vector.
    ''' </summary>
    Public Class BarDataSample : Implements INamedValue

        ''' <summary>
        ''' 分组名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Tag As String Implements INamedValue.Key
        ''' <summary>
        ''' 当前分组下的每一个序列的数据值
        ''' </summary>
        ''' <returns></returns>
        Public Property data As Double()

        ''' <summary>
        ''' The sum of <see cref="data"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property StackedSum As Double
            Get
                Return data.Sum
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class BarDataGroup : Inherits ProfileGroup

        ''' <summary>
        ''' 与<see cref="BarDataSample.data"/>里面的数据顺序是一致的
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property Serials As NamedValue(Of Color)()

        ''' <summary>
        ''' 分组数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Samples As BarDataSample()
        Public Property Index As NamedVectorFactory

        Public Overrides Function ToString() As String
            Return Samples.Keys.GetJson
        End Function

        ''' <summary>
        ''' 按照百分比降序排序
        ''' </summary>
        ''' <returns></returns>
        Public Function Desc() As BarDataGroup
            ' 通过平均值生成orders
            Return Reorder(sortInternal(AddressOf Enumerable.Average, desc:=True))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function sortInternal(method As Func(Of IEnumerable(Of Double), Double), desc As Boolean) As String()
            Return Serials _
                .SeqIterator _
                .Select(Function(s)
                            Dim name$ = s.value.Name
                            Dim samples = Me.Samples.SerialDatas(s, name)
                            Return (avg:=method(samples.Values), name:=name)
                        End Function) _
                .Sort(Function(t) t.avg, desc:=desc) _
                .Select(Function(t) t.name) _
                .ToArray
        End Function

        Public Function Asc() As BarDataGroup
            Return Reorder(sortInternal(AddressOf Enumerable.Average, desc:=False))
        End Function

        ''' <summary>
        ''' 如果系列在这里面，则会一次排在前面，否则任然是按照原始的顺序排布
        ''' </summary>
        ''' <param name="orders$"></param>
        ''' <returns></returns>
        Public Function Reorder(ParamArray orders$()) As BarDataGroup
            Dim oldOrders = Me.Serials _
                .Keys _
                .SeqIterator _
                .ToDictionary(Function(x) x.value,
                              Function(x) x.i)
            Dim newOrders As New List(Of SeqValue(Of String))

            For Each name In orders
                newOrders += New SeqValue(Of String) With {
                    .i = oldOrders(name),
                    .value = name
                }
                oldOrders.Remove(name)
            Next

            ' 可能还会剩下一部分的对象是没有在orders列表之中的，则将他们补齐
            newOrders += oldOrders _
                .Select(Function(x)
                            Return New SeqValue(Of String) With {
                                .i = x.Value,
                                .value = x.Key
                            }
                        End Function)

            Dim serials = newOrders _
                .Select(Function(i) Me.Serials(i)) _
                .ToArray
            Dim groups As New List(Of BarDataSample)

            For Each g In Me.Samples
                groups += New BarDataSample With {
                    .Tag = g.Tag,
                    .data = newOrders _
                        .Select(Function(i) g.data(i)) _
                        .ToArray
                }
            Next

            Return New BarDataGroup With {
                .Index = Index,
                .Samples = groups,
                .Serials = serials
            }
        End Function

        ''' <summary>
        ''' 这个应该是生成直方图的数据
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="base!"></param>
        ''' <param name="color$"></param>
        ''' <returns></returns>
        Public Shared Function FromDistributes(data As IEnumerable(Of Double), Optional base! = 10.0F, Optional color$ = "darkblue") As BarDataGroup
            Dim source = data.Distributes(base!)
            Dim bg As Color = color.ToColor(onFailure:=Drawing.Color.DarkBlue)
            Dim values As New List(Of Double)
            Dim serials = LinqAPI.Exec(Of NamedValue(Of Color)) _
 _
                () <= From lv As Integer
                      In source.Keys
                      Let tag As String = lv.ToString
                      Select New NamedValue(Of Color) With {
                          .Name = tag,
                          .Value = bg
                      }

            For Each x As NamedValue(Of Color) In serials
                values += source(CInt(x.Name)).Value
            Next

            Return New BarDataGroup With {
                .Serials = serials,
                .Samples = {
                    New BarDataSample With {
                        .Tag = "Distribution",
                        .data = values
                    }
                }
            }
        End Function
    End Class
End Namespace
