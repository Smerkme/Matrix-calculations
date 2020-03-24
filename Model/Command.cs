using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace DataTableTest.Model {

  abstract public class Command {
    public abstract void Execute();
    public abstract void UnExecute();
  }

  public class Log {
    int currpos = 0; 
    public List<Command> list = new List<Command>();

    public bool isCommand() { return currpos > 0; } 

    public void Do_(Command cmd) {
      list.Add(cmd);
      currpos = list.Count;
    }

    public Command Undo_() {
      if (currpos > 0) {
        --currpos;
        return list[currpos];
      }
      return null;
    }

    public bool isAll() { 
      if (list.Count == 0)
        return true;
      return (list.Count == currpos);
    }

    public Command Redo_() {
      if (currpos == list.Count)
        return null;

      ++currpos;
      return list[currpos - 1];

    }

  }

  public class SaveDataTable : Command {

    public DataTable table;
    public SaveDataTable(DataTable sourseTable) {
      this.table = sourseTable;
    }

    public override void Execute() {
      try {
        Pool.dataSet.Tables.Add(this.table);
      } catch(Exception ex) {
        MessageBox.Show(Convert.ToString(ex));
      }
    }
    public override void UnExecute() {
      Pool.dataSet.Tables.Remove(Pool.dataSet.Tables[Pool.dataSet.Tables.Count - 1]);
    }
  }//endclass
  public class User {

    Log log = new Log();
    public void put(Command cmd) {
      log.Do_(cmd);
      cmd.Execute();
    }
    public void unput() {
      if (log.isCommand())
        log.Undo_().UnExecute();
    }

    public void reput() {  
      if (log.isAll())
        return; 
      log.Redo_().Execute();
    }

  }
}
