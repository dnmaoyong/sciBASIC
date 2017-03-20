﻿Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Imaging
Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.StyledBarplot
Imports Microsoft.VisualBasic.Data.ChartPlots.Dendrogram

Module Module1

    Sub Main()

        Dim t As New Tree("root")

        Call t.ChildNodes.Add(New Tree("123"))
        Call t.ChildNodes.Add(New Tree("333"))
        Call t.ChildNodes.Last.AddChild(New Tree("233"))
        Call t.ChildNodes.Last.AddChild(New Tree("abc"))
        Call t.ChildNodes.Last.AddChild(New Tree("666"))

        Call t.Plot.SaveAs("x:\fff.png")


        Pause()

        Dim s = {
            New BarSerial With {.Brush = "red", .Label = "label<sub>1</sub>", .Value = 123},
            New BarSerial With {.Brush = "red", .Label = "label<sub>1</sub>", .Value = 123},
            New BarSerial With {.Brush = "cyan", .Label = "label<sub>1</sub>", .Value = 123},
            New BarSerial With {.Brush = "red", .Label = "label<sub>1</sub>", .Value = 123},
            New BarSerial With {.Brush = "orange", .Label = "label<sub>1</sub>", .Value = 13},
            New BarSerial With {.Brush = "blue", .Label = "label<sub>1</sub>", .Value = 223},
            New BarSerial With {.Brush = "yellow", .Label = "label<sub>1</sub>", .Value = 223},
            New BarSerial With {.Brush = "black", .Label = "label<sub>1</sub>", .Value = 223},
            New BarSerial With {.Brush = "red", .Label = "label<sub>1</sub>", .Value = 223}
        }

        Call s.Plot().SaveAs("x:/test.png")

        Pause()

        'Dim x = "-60,23"
        'Dim y = "-0.5,2"

        'Dim mapper As New Mapper(x, y)

        'Using g As GDIPlusDeviceHandle = New Size(1600, 1200).CreateGDIDevice

        '    Call g.Graphics.DrawAxis(g.Size, "padding: 50 50 50 50", mapper, True,
        '                             xlabel:="<span style=""color:green"">log<sub>2</sub>(Fold Change)</span>",
        '                             ylabel:="-log<sub>10</sub>(P-value)",
        '                             ylayout:=YAxisLayoutStyles.Centra)

        '    Call g.Save("x:\test.png", ImageFormats.Png)

        'End Using

        'Pause()


        Call AxisScalling.GetAxisValues(98,, -83.4023).GetJson.__DEBUG_ECHO
        Call AxisScalling.GetAxisValues(198,, -83.4023).GetJson.__DEBUG_ECHO

        '   Pause()

        Call AxisScalling.GetAxisValues(98,, 83.4023).GetJson.__DEBUG_ECHO
        Call AxisScalling.GetAxisValues(98,, 3).GetJson.__DEBUG_ECHO

        Call AxisScalling.GetAxisValues(7.9,, 1.3).GetJson.__DEBUG_ECHO
        Call AxisScalling.GetAxisValues(-7.9,, -21.3).GetJson.__DEBUG_ECHO

        Call AxisScalling.GetAxisValues(0.98,, 0.03, [decimal]:=2).GetJson.__DEBUG_ECHO
        Call AxisScalling.GetAxisValues(2.98,, 0.03, [decimal]:=2).GetJson.__DEBUG_ECHO

        Call AxisScalling.GetAxisValues(-0.0198,, -0.903, [decimal]:=2).GetJson.__DEBUG_ECHO

        Pause()




    End Sub

End Module
