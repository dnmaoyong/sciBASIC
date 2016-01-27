﻿Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports ______NETWORK__ = Microsoft.VisualBasic.DataVisualization.Network.FileStream.Network(Of
    Microsoft.VisualBasic.DataVisualization.Network.FileStream.Node,
    Microsoft.VisualBasic.DataVisualization.Network.FileStream.NetworkEdge)

<[PackageNamespace]("DataVisualization.Network", Publisher:="xie.guigang@gmail.com")>
Public Module NetworkAPI

    <ExportAPI("Read.Network")>
    Public Function ReadnetWork(file As String) As FileStream.NetworkEdge()
        Return file.LoadCsv(Of FileStream.NetworkEdge)(False).ToArray
    End Function

    <ExportAPI("Find.NewSession")>
    Public Function CreatePathwayFinder(Network As Generic.IEnumerable(Of FileStream.NetworkEdge)) As PathFinder(Of FileStream.NetworkEdge)
        Return New PathFinder(Of FileStream.NetworkEdge)(Network.ToArray)
    End Function

    <ExportAPI("Find.Path.Shortest")>
    Public Function FindShortestPath(finder As PathFinder(Of FileStream.NetworkEdge), start As String, ends As String) As FileStream.NetworkEdge()
        Dim ChunkBuffer = finder.FindShortestPath(start, ends)
        Dim List As List(Of FileStream.NetworkEdge) = New List(Of FileStream.NetworkEdge)
        For Each Line In ChunkBuffer
            Call List.AddRange(Line.Value)
        Next
        Return List.ToArray
    End Function

    <ExportAPI("Get.NetworkEdges")>
    Public Function GetNHetworkEdges(Network As ______NETWORK__) As Microsoft.VisualBasic.DataVisualization.Network.FileStream.NetworkEdge()
        Return Network.Edges
    End Function

    <ExportAPI("Get.NetworkNodes")>
    Public Function GetNetworkNodes(Network As ______NETWORK__) As Microsoft.VisualBasic.DataVisualization.Network.FileStream.Node()
        Return Network.Nodes
    End Function

    <ExportAPI("Save")>
    Public Function SaveNetwork(network As ______NETWORK__, <Parameter("DIR.Export")> Export As String) As Boolean
        Return network.Save(Export, Encodings.UTF8)
    End Function

    <ExportAPI("Write.Network")>
    Public Function WriteNetwork(Network As FileStream.NetworkEdge(), <Parameter("Path.Save")> SaveTo As String) As Boolean
        Call Network.SaveTo(SaveTo, False)
        Return True
    End Function
End Module
