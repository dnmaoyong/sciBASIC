﻿Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering.Hierarchy

'
'*****************************************************************************
' Copyright 2013 Lars Behnke
' <p/>
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
' <p/>
' http://www.apache.org/licenses/LICENSE-2.0
' <p/>
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' *****************************************************************************
'

Public Class Cluster

    Public Property Distance As Distance

    Public ReadOnly Property WeightValue As Double
        Get
            Return Distance.Weight
        End Get
    End Property

    Public ReadOnly Property DistanceValue As Double
        Get
            Return Distance.Distance
        End Get
    End Property

    Public Property Parent As Cluster
    Public Property Name As String
    Public ReadOnly Property Children As IList(Of Cluster)
    Public ReadOnly Property LeafNames As List(Of String)

    Public Sub New(name$)
        Me.Name = name
        LeafNames = New List(Of String)
        Children = New List(Of Cluster)
        Distance = New Distance
    End Sub

    Public Sub AddLeafName(lname$)
        LeafNames.Add(lname)
    End Sub

    Public Sub AppendLeafNames(lnames As IEnumerable(Of String))
        LeafNames.AddRange(lnames)
    End Sub

    Public Sub AddChild(cluster As Cluster)
        Children.Add(cluster)
    End Sub

    Public Function contains(cluster As Cluster) As Boolean
        Return Children.Contains(cluster)
    End Function

    Public Overrides Function ToString() As String
        Return "Cluster " & Name
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If
        If Me Is obj Then
            Return True
        End If

        If Me.GetType() IsNot obj.GetType() Then
            Return False
        End If

        Dim other As Cluster = CType(obj, Cluster)

        If Name Is Nothing Then
            If other.Name IsNot Nothing Then
                Return False
            End If
        ElseIf Not Name.Equals(other.Name) Then
            Return False
        End If

        Return True
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return If(Name Is Nothing, 0, Name.GetHashCode())
    End Function

    Public ReadOnly Property Leaf As Boolean
        Get
            Return Children.Count = 0
        End Get
    End Property

    ''' <summary>
    ''' 计算出所有的叶节点的总数，包括自己的child的叶节点
    ''' </summary>
    ''' <returns></returns>
    Public Function CountLeafs() As Integer
        Return CountLeafs(Me, 0)
    End Function

    ''' <summary>
    ''' 对某一个节点的所有的叶节点进行计数
    ''' </summary>
    ''' <param name="node"></param>
    ''' <param name="count"></param>
    ''' <returns></returns>
    Public Shared Function CountLeafs(node As Cluster, count As Integer) As Integer
        If node.Leaf Then count += 1
        For Each child As Cluster In node.Children
            count += child.CountLeafs()
        Next
        Return count
    End Function

    Public ReadOnly Property TotalDistance As Double
        Get
            Dim dist As Double = If(Distance Is Nothing, 0, Distance.Distance)
            If Children.Count > 0 Then
                dist += Children(0).TotalDistance
            End If
            Return dist
        End Get
    End Property
End Class