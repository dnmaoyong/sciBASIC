﻿#Region "Microsoft.VisualBasic::aeb106bf96344a4acb3e0a6e9cc02065, ..\sciBASIC#\mime\application%vnd.openxmlformats-officedocument.spreadsheetml.sheet\Excel\File.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO.Compression
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Office.Excel.XML.xl.worksheets
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.OpenXml
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

''' <summary>
''' ``*.xlsx`` document file
''' </summary>
Public Class File : Implements IFileReference

    Public Property ContentTypes As ContentTypes
    Public Property _rels As _rels
    Public Property docProps As docProps
    Public Property xl As xl

    Dim _filePath As DefaultValue(Of String)

    Public Property FilePath As String Implements IFileReference.FilePath
        Get
            Return _filePath.Value
        End Get
        Friend Set(value As String)
            _filePath = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return FilePath
    End Function

    ''' <summary>
    ''' 使用序列化写入数据到xlsx文件之中
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="data"></param>
    ''' <param name="sheetName$"></param>
    ''' <returns></returns>
    Public Function WriteSheetTable(Of T)(data As IEnumerable(Of T), sheetName$) As Boolean
        Dim table As csv = data.ToCsvDoc
        Return WriteSheetTable(table, sheetName)
    End Function

    Public Function WriteSheetTable(table As csv, sheetName$) As Boolean

    End Function

    Public Function WriteXlsx(Optional path$ = Nothing) As Boolean
        Return Me.Save(path Or _filePath)
    End Function

    Public Function GetTable(sheetName$) As csv
        Dim worksheet As worksheet = xl.GetWorksheet(sheetName)

        If worksheet Is Nothing Then
            Return Nothing
        Else
            Return xl.GetTableData(worksheet)
        End If
    End Function

    Public Function LoadDataSet(Of T As Class)(sheetName$) As T()
        Return GetTable(sheetName).AsDataSource(Of T)
    End Function

    Public Shared Function CreatePackage(tmp$, xlsx$) As Boolean
        Try
            Call GZip.DirectoryArchive(tmp, xlsx, ArchiveAction.Replace, Overwrite.Always, CompressionLevel.Fastest)
        Catch ex As Exception
            Dim debug$ = New Dictionary(Of String, String) From {
                {NameOf(tmp), tmp},
                {NameOf(xlsx), xlsx}
            }.GetJson
            ex = New Exception(debug, ex)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' 读取Excel文件
    ''' </summary>
    ''' <param name="path$">``*.xlsx``</param>
    ''' <returns></returns>
    Public Shared Function Open(path$) As File
        Return IO.CreateReader(xlsx:=path)
    End Function
End Class