Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Data
Imports System.Collections
Imports System.Text.RegularExpressions
Imports DevExpress.Web

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Private conditionTemplate As String
	Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
	End Sub
	Protected Sub combo_ItemRequestedByValue(ByVal source As Object, ByVal e As DevExpress.Web.ListEditItemRequestedByValueEventArgs)
		Dim value As String
		If e.Value IsNot Nothing Then
			conditionTemplate = "WHERE [ProductID] = {0}"
			value = e.Value.ToString()
		Else
			value = String.Empty
		End If
		BindComboBox(source, value, 0, 1)
	End Sub
	Protected Sub combo_ItemsRequestedByFilterCondition(ByVal source As Object, ByVal e As DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs)
		If (Not String.IsNullOrEmpty(e.Filter)) Then
			conditionTemplate = "WHERE [ProductName] like '{0}%'"
		End If
		BindComboBox(source, e.Filter, e.BeginIndex, e.EndIndex)
	End Sub
	Private Sub BindComboBox(ByVal source As Object, ByVal value As String, ByVal beginIndex As Integer, ByVal endIndex As Integer)
		Dim cb As ASPxComboBox = TryCast(source, ASPxComboBox)
		cb.DataSource = GetDataSet(value, beginIndex, endIndex)
		cb.DataBind()
	End Sub
	Private Function GetDataSet(ByVal value As String, ByVal beginIndex As Integer, ByVal endIndex As Integer) As IEnumerable
		Const commandTemplate As String = "SELECT * FROM (" & "    SELECT ROW_NUMBER() OVER (ORDER BY [ProductID]) AS rownumber, [ProductID], [ProductName] " & "    FROM [Products] {0}" & ") AS foo " & "WHERE rownumber >= {1} AND rownumber <= {2}"

		Dim command As String
		Dim valueIsEmpty As Boolean = String.IsNullOrEmpty(value)
		If valueIsEmpty Then
			command = String.Format(commandTemplate, String.Empty, beginIndex, endIndex)
		Else
			command = String.Format(commandTemplate, String.Format(conditionTemplate, value), beginIndex, endIndex)
		End If

		Dim dataSource As New SqlDataSource(ConfigurationManager.ConnectionStrings("NorthwindConnectionString").ConnectionString, command)
		Dim dataSet As IEnumerable = dataSource.Select(DataSourceSelectArguments.Empty)
		
		Return dataSet
	End Function
	Private Function AddColor(ByVal m As Match) As String
		Return String.Format("<span style='background-color:Orange'>{0}</span>", m.Value)
	End Function
End Class