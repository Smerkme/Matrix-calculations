  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using System.Data;
  using System.Windows.Forms;

  namespace DataTableTest.Model {
    public class Matrix {

      public int row { get; set; }
      public int column { get; set; }

      public DataTable table { get; set; }
      public DataColumn[] dataColumn;

      public string[] columnNames;

      public Matrix(DataTable table, string name) {
        this.table = table.Copy();
        this.table.TableName = name;
        this.row = table.Rows.Count;
        this.column = table.Columns.Count; 
        this.columnNames = new string[table.Columns.Count];
        this.dataColumn = null;

        for (int i = 0; i < table.Columns.Count; i++) {
          this.columnNames[i] = table.Columns[i].ColumnName;
        }
      }

      public Matrix(int row, int column, string tableName) {
        this.table = new DataTable(tableName);
        Pool.dataSet.Tables.Add(this.table);
        this.row = row;
        this.column = column;

        // ++++ add column
        dataColumn = new DataColumn[this.column];
        this.columnNames = new string[dataColumn.Length];
        string columnName = "";
        for (int i = 0; i < dataColumn.Length; i++) {
          columnName = "Col" + i;
          dataColumn[i] = new DataColumn(columnName);
          this.columnNames[i] = columnName;
        }

        table.Columns.AddRange(dataColumn);

        //++++ add rows
        DataRow dataRow;
        foreach (DataColumn col in dataColumn) {
          dataRow = table.NewRow();
          table.Rows.Add(dataRow);
        }
      }

      public string getDataTable(DataSet dataSet, DataView view, int verticalPos, int horizontalPos, int skipCount, string[] schemeColumn,
                       string tableName) {
        DataTable resTable = new DataTable();

        Console.WriteLine(horizontalPos.ToString() + " " + verticalPos.ToString());
        Console.WriteLine(skipCount);

        resTable = view.ToTable(tableName, true, schemeColumn)
           .AsEnumerable().Skip(skipCount).Take(horizontalPos).CopyToDataTable();

        dataSet.Tables.Add(resTable);

        return resTable.TableName;
      }

      public void splitVertical(int verticalPos) {
        //Vertical split of sourse table 
        string[] leftPart = new string[verticalPos];
        string[] rightPart = new string[columnNames.Length - verticalPos];

        Array.Copy(columnNames, 0, leftPart, 0, leftPart.Length);
        Array.Copy(columnNames, verticalPos, rightPart, 0, rightPart.Length);

        DataView view = new DataView(this.table);

        DataTable LR = view.ToTable("matrix" + Convert.ToString(Pool.dataSet.Tables.Count + 1),
          true, leftPart);
        Pool.dataSet.Tables.Add(LR);

        LR = view.ToTable("matrix" + Convert.ToString(Pool.dataSet.Tables.Count + 1), true, rightPart);
        Pool.dataSet.Tables.Add(LR);
      }

      public void splitHorizontal(int pos) {
        // Split Horizontal the this.table into two parts Up and Down
        DataTable Up = this.table.AsEnumerable().Take(pos).CopyToDataTable();
        DataTable Down = this.table.AsEnumerable().Skip(pos).Take(this.table.Rows.Count - pos).CopyToDataTable();

        Up.TableName = this.table.TableName + "Hup";
        Down.TableName = this.table.TableName + "Hdown";

        Pool.dataSet.Tables.Add(Up);
        Pool.dataSet.Tables.Add(Down);
      }
      public String[] splitHV(int verticalPos, int horizontalPos) {
        //Vertical split of sourse table 

        string[] leftPart = new string[verticalPos];
        string[] rightPart = new string[this.column - verticalPos];

        Array.Copy(columnNames, 0, leftPart, 0, leftPart.Length);
        Array.Copy(columnNames, verticalPos, rightPart, 0, rightPart.Length);

        DataView view = new DataView(this.table);

        string[] tableNames = new string[4];

        tableNames[0] = getDataTable(Pool.dataSet, view, verticalPos, horizontalPos, 0,
                                          leftPart, this.table.TableName + "11");


        tableNames[1] = getDataTable(Pool.dataSet, view, verticalPos - 1, horizontalPos, 0,
                                          rightPart, this.table.TableName + "12");

        tableNames[2] = getDataTable(Pool.dataSet, view, verticalPos, this.row - horizontalPos,
                                          horizontalPos, leftPart, this.table.TableName + "21");

        tableNames[3] = getDataTable(Pool.dataSet, view, verticalPos - 1, this.row - horizontalPos,
                                             horizontalPos, rightPart, this.table.TableName + "22");

        return tableNames;

      }
    }

    public static class BinaryMatrixOperation {

      public static void Merge(Matrix matrix1, Matrix matrix2) {
        DataTable dataTable = matrix1.table.Copy();
        dataTable.TableName = "matrix" + Convert.ToString(Pool.dataSet.Tables.Count + 1);
        dataTable.Merge(matrix2.table, true);
        Pool.dataSet.Tables.Add(dataTable);
      }

      public static DataTable matrixAddition(DataTable table1, DataTable table2) {
        DataTable res = new DataTable("matrix" + Convert.ToString(Pool.dataSet.Tables.Count + 1));
        DataColumn column;
        for (int i = 0; i < table1.Columns.Count; i++) {
          column = new DataColumn("Col" + i);
          res.Columns.Add(column);
        }
        DataRow row;
        for (int i = 0; i < table1.Rows.Count; i++) {
          row = res.NewRow();
          for (int j = 0; j < table2.Columns.Count; j++) {
            row[j] = Convert.ToInt32(table1.Rows[i][j]) + Convert.ToInt32(table2.Rows[i][j]);
          }
          res.Rows.Add(row);
        }
        try {
          Pool.dataSet.Tables.Add(res);
        }
        catch(Exception ex) {
          MessageBox.Show(Convert.ToString(ex));
        }
        return res;
      }

      public static DataTable matrixSubtraction(DataTable table1, DataTable table2) {
        DataTable res = new DataTable("matrix" + Convert.ToString(Pool.dataSet.Tables.Count + 1));
        DataColumn column;
        for (int i = 0; i < table1.Columns.Count; i++) {
          column = new DataColumn("Col" + i);
          res.Columns.Add(column);
        }
        DataRow row;
        for (int i = 0; i < table1.Rows.Count; i++) {
          row = res.NewRow();
          for (int j = 0; j < table2.Rows.Count; j++) {
            row[j] = Convert.ToInt32(table1.Rows[i][j]) - Convert.ToInt32(table2.Rows[i][j]);
          }
          res.Rows.Add(row);
        }
        try {
          Pool.dataSet.Tables.Add(res);
        }
        catch(Exception ex) {
          MessageBox.Show(Convert.ToString(ex));
        }
        return res;
      }
    }

    public static class UnaryMatrixOperation {
      public static void splitVertical(Matrix matrix, int verticalPos) {

        //Vertical split of sourse table 
        string[] leftPart = new string[verticalPos];
        string[] rightPart = new string[matrix.columnNames.Length - verticalPos];

        Array.Copy(matrix.columnNames, 0, leftPart, 0, leftPart.Length);
        Array.Copy(matrix.columnNames, verticalPos, rightPart, 0, rightPart.Length);

        DataView view = new DataView(matrix.table);

        DataTable LR = view.ToTable(matrix.table.TableName + "Vleft", true, leftPart);
        Pool.dataSet.Tables.Add(LR);

        LR = view.ToTable(matrix.table.TableName + "Vright", true, rightPart);
        Pool.dataSet.Tables.Add(LR);
      }

      public static void splitHorizontal(Matrix matrix, int pos) {
        // Split Horizontal the this.table into two parts Up and Down
        DataTable Up = matrix.table.AsEnumerable().Take(pos).CopyToDataTable();
        DataTable Down = matrix.table.AsEnumerable().Skip(pos).Take(matrix.table.Rows.Count - pos).CopyToDataTable();

        Up.TableName = matrix.table.TableName + "Hup";
        Down.TableName = matrix.table.TableName + "Hdown";
   
        Pool.dataSet.Tables.Add(Up);
        Pool.dataSet.Tables.Add(Down);
      }

      public static String[] splitHV(Matrix matrix, int verticalPos, int horizontalPos) {
        //Vertical split of sourse table 

        string[] leftPart = new string[verticalPos];
        string[] rightPart = new string[matrix.column - verticalPos];

        Array.Copy(matrix.columnNames, 0, leftPart, 0, leftPart.Length);
        Array.Copy(matrix.columnNames, verticalPos, rightPart, 0, rightPart.Length);

        DataView view = new DataView(matrix.table);

        string[] tableNames = new string[4];

        tableNames[0] = matrix.getDataTable(Pool.dataSet, view, verticalPos, horizontalPos, 0,
                                          leftPart, matrix.table.TableName + "11");


        tableNames[1] = matrix.getDataTable(Pool.dataSet, view, verticalPos - 1, horizontalPos, 0,
                                          rightPart, matrix.table.TableName + "12");

        tableNames[2] = matrix.getDataTable(Pool.dataSet, view, verticalPos, matrix.row - horizontalPos,
                                          horizontalPos, leftPart, matrix.table.TableName + "21");

        tableNames[3] = matrix.getDataTable(Pool.dataSet, view, verticalPos - 1, matrix.row - horizontalPos,
                                             horizontalPos, rightPart, matrix.table.TableName + "22");

        return tableNames;

      }

      public static void transpostion(Matrix matrix) {
        DataTable table = new DataTable(matrix.table.TableName);
        DataColumn column;
        for(int i = 0; i < matrix.column; i++) {
          column = new DataColumn("Col" + i);
          column.DataType = matrix.table.Columns[i].DataType;
          table.Columns.Add(column);
        }
        DataRow row;
        for (int i = 0; i < matrix.row; i++) {
          row = table.NewRow();
          for (int j = 0; j < matrix.column; j++) {
            row[j] = matrix.table.Rows[j][i];
          }
          table.Rows.Add(row);
        }
        Pool.dataSet.Tables.Add(table);
      }
    }

    public static class MainFormOperation {
      public static DataTable CreateNewDataTable(int columnsCount) {
        DataTable table = new DataTable("matrix" + 
          Convert.ToString(Pool.dataSet.Tables.Count + 1));

        // ++++ add column
        DataColumn column;
        for (int i = 0; i < columnsCount; i++) {
          column = new DataColumn();
          column.ColumnName = "Col" + i;
          column.DataType = Type.GetType("System.Int32");
          table.Columns.Add(column);
        }

        //++++ add rows
        DataRow dataRow;
        foreach (DataColumn col in table.Columns) {
          dataRow = table.NewRow();
          table.Rows.Add(dataRow);
        }

        return table;
      }
    }
  }
