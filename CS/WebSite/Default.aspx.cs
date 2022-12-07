using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using DevExpress.Web;

public partial class _Default : System.Web.UI.Page {
	string conditionTemplate;
	protected void Page_Init(object sender, EventArgs e) {
		//combo.DataBind();
	}
	protected void combo_ItemRequestedByValue(object source, DevExpress.Web.ListEditItemRequestedByValueEventArgs e) {
		string value = e.Value == null ? String.Empty : e.Value.ToString();
		if (e.Value != null) {
			conditionTemplate = "WHERE [ProductID] = {0}";
		}
		BindComboBox(source, value, 0, 1);
	}
	protected void combo_ItemsRequestedByFilterCondition(object source, DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e) {		
		if (!String.IsNullOrEmpty(e.Filter)) {
			conditionTemplate = "WHERE [ProductName] like '{0}%'";
		}
		BindComboBox(source, e.Filter, e.BeginIndex, e.EndIndex);
	}
	private void BindComboBox(object source, string value, int beginIndex, int endIndex) {
		ASPxComboBox cb = source as ASPxComboBox;
		cb.DataSource = GetDataSet(value, beginIndex, endIndex);
		cb.DataBind();
	}
	private IEnumerable GetDataSet(string value, int beginIndex, int endIndex) {
		const string commandTemplate =
			"SELECT * FROM (" +
			"    SELECT ROW_NUMBER() OVER (ORDER BY [ProductID]) AS rownumber, [ProductID], [ProductName] " +
			"    FROM [Products] {0}" +
			") AS foo " +
			"WHERE rownumber >= {1} AND rownumber <= {2}";

		string command;
		bool valueIsEmpty = String.IsNullOrEmpty(value);
		if (valueIsEmpty)
			command = String.Format(commandTemplate, String.Empty, beginIndex, endIndex);
		else
			command = String.Format(commandTemplate, String.Format(conditionTemplate, value), beginIndex, endIndex);

		SqlDataSource dataSource = new SqlDataSource(ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString, command);
		IEnumerable dataSet = dataSource.Select(DataSourceSelectArguments.Empty);
		//if (!valueIsEmpty) {
		//    Regex r = new Regex("^" + value, RegexOptions.IgnoreCase);
		//    foreach (DataRowView item in dataSet) {
		//        item.Row["ProductName"] = r.Replace(item.Row["ProductName"].ToString(), AddColor);
		//    }
		//}
		return dataSet;
	}
	private string AddColor(Match m) {
		return String.Format("<span style='background-color:Orange'>{0}</span>", m.Value);
	}
}