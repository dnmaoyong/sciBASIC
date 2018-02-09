﻿Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.DataMining.AprioriRules.Entities
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace AprioriRules.Impl

    Module StrongRules

        <Extension>
        Public Function GenerateRules(allFrequentItems As Dictionary(Of String, TransactionTokensItem)) As HashSet(Of Rule)
            Dim rulesList As New HashSet(Of Rule)()
            Dim concats = LinqAPI.Exec(Of Rule()) _
 _
                () <= From item
                      In allFrequentItems.AsParallel
                      Where item.Key.Length > 1
                      Select item.concatRules()

            For Each rule In concats.IteratesALL
                If Not rulesList.Contains(rule) Then
                    Call rulesList.Add(rule)
                End If
            Next

            Return rulesList
        End Function

        <Extension>
        Private Function concatRules(token As KeyValuePair(Of String, TransactionTokensItem)) As Rule()
            Dim subsetsList As IEnumerable(Of String) = GenerateSubsets(token.Key)
            Dim list As New List(Of Rule)

            For Each subset As String In subsetsList
                Dim remaining$ = GetRemaining(subset, token.Key)
                Dim rule As New Rule(subset, remaining, 0)

                Call list.Add(rule)
            Next

            Return list.ToArray
        End Function

        Public Function GenerateSubsets(item As String) As IEnumerable(Of String)
            Dim allSubsets As IEnumerable(Of String) = New String() {}
            Dim subsetLength As Integer = item.Length / 2

            For i As Integer = 1 To subsetLength
                Dim subsets As New List(Of String)()
                GenerateSubsetsRecursive(item, i, New Char(item.Length - 1) {}, subsets)
                allSubsets = allSubsets.Concat(subsets)
            Next

            Return allSubsets
        End Function

        Public Sub GenerateSubsetsRecursive(item$, subsetLength%, temp As Char(), subsets As IList(Of String), Optional q% = 0, Optional r% = 0)
            If q = subsetLength Then
                Dim sb As New StringBuilder()

                For i As Integer = 0 To subsetLength - 1
                    sb.Append(temp(i))
                Next

                subsets.Add(sb.ToString())
            Else
                For i As Integer = r To item.Length - 1
                    temp(q) = item(i)
                    GenerateSubsetsRecursive(item, subsetLength, temp, subsets, q + 1, i + 1)
                Next
            End If
        End Sub

        Public Function GetRemaining(child As String, parent As String) As String
            For i As Integer = 0 To child.Length - 1
                Dim index As Integer = parent.IndexOf(child(i))
                parent = parent.Remove(index, 1)
            Next

            Return parent
        End Function

        Public Function GetStrongRules(minConfidence#, rules As HashSet(Of Rule), allFrequentItems As Dictionary(Of String, TransactionTokensItem)) As IList(Of Rule)
            Dim strongRules As New List(Of Rule)()

            For Each rule As Rule In rules
                Dim xy As String = Apriori.SorterSortTokens(rule.X & rule.Y)
                strongRules.AddStrongRule(rule, xy, minConfidence, allFrequentItems)
            Next

            strongRules.Sort()

            Return strongRules
        End Function

        <Extension>
        Public Sub AddStrongRule(strongRules As List(Of Rule), rule As Rule, XY$, minConfidence#, allFrequentItems As Dictionary(Of String, TransactionTokensItem))
            Dim confidence# = allFrequentItems.GetConfidence(rule.X, XY)

            If confidence >= minConfidence Then
                Dim newRule As New Rule(rule.X, rule.Y, confidence)
                strongRules.Add(newRule)
            End If

            confidence = allFrequentItems.GetConfidence(rule.Y, XY)

            If confidence >= minConfidence Then
                Dim newRule As New Rule(rule.Y, rule.X, confidence)
                strongRules.Add(newRule)
            End If
        End Sub

        <Extension>
        Public Function GetConfidence(allFrequentItems As Dictionary(Of String, TransactionTokensItem), X$, XY$) As Double
            If Not (allFrequentItems.ContainsKey(X) AndAlso allFrequentItems.ContainsKey(XY)) Then
                Return 0
            End If

            Dim supportX As Double = allFrequentItems(X).Support
            Dim supportXY As Double = allFrequentItems(XY).Support
            Return supportXY / supportX
        End Function

        Public Function GetMaximalItemSets(closedItemSets As Dictionary(Of String, Dictionary(Of String, Double))) As IList(Of String)
            Dim maximalItemSets As New List(Of String)()

            For Each item In closedItemSets
                Dim parents = item.Value

                If parents.Count = 0 Then
                    Call maximalItemSets.Add(item.Key)
                End If
            Next

            Return maximalItemSets
        End Function
    End Module
End Namespace