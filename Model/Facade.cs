using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTableTest.Model {
  public class Facade {
    static CreateTableFacade createTableFacade = new CreateTableFacade();
    public static ChangeTableFacade changeTableFacade = new ChangeTableFacade();
    public static void RunCreateTable() { createTableFacade.Run(); }
    public static void RunChangeTable(Matrix matrix) { changeTableFacade.Run(matrix); }
  }

  internal class CreateTableFacade {
    public void Run() {
      View.CreateTable createTable = new View.CreateTable();
      createTable.ShowDialog();
    }

  }

  public class ChangeTableFacade {
    public void Run(Matrix matrix) {
      View.ChangeTable changeTable = new View.ChangeTable();

      this.matrix1 = matrix;

      changeTable.ShowDialog();
    }
    public Matrix matrix1 { set; get; }
  }
}

