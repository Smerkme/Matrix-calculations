using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DataTableTest.Model {
  public class Pool  {
    static DataSet dataSet1 = null;
    public static Type type = null;
    private Pool() { dataSet1 = new DataSet();  }

    public static DataSet dataSet {
      get {
        if (dataSet1 == null) {
            new Pool();
        }
        return dataSet1;
      }
    }
    
  }
}
